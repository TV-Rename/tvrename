using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace TVRename.YTS;

// ReSharper disable once InconsistentNaming
public static class API
{
    // ReSharper disable once ConvertToConstant.Local
    // ReSharper disable once InconsistentNaming
    private static readonly string APIRoot = "https://yts.mx/api/v2/";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static T HandleErrorsFrom<T>(string message, Func<T> handler)
    {
        string errorMessage = $"Could not {message} from YTS";
        try
        {
            return handler();
        }
        catch (WebException ex)
        {
            Logger.LogWebException(errorMessage, ex);
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (HttpRequestException wex)
        {
            Logger.LogHttpRequestException(errorMessage, wex);
            throw new SourceConnectivityException(errorMessage, wex);
        }
        catch (System.IO.IOException iex)
        {
            Logger.Error($"{errorMessage} due to {iex.ErrorText()}");
            throw new SourceConnectivityException(errorMessage, iex);
        }
        catch (JsonReaderException jre)
        {
            Logger.Error($"{errorMessage} due to {jre.ErrorText()}");
            throw new SourceConsistencyException(jre.Message,TVDoc.ProviderType.libraryDefault,jre);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            Logger.LogHttpRequestException(errorMessage, wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage,wex);
        }
        catch (System.Threading.Tasks.TaskCanceledException ex)
        {
            Logger.Warn($"{errorMessage} due to {ex.ErrorText()}");
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (AggregateException aex) when (aex.InnerException is System.Threading.Tasks.TaskCanceledException ex)
        {
            Logger.Warn($"{errorMessage} due to {ex.ErrorText()}");
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage, ex);
        }
    }

    private static string GetMandatoryString(this JToken r, string key)
        => (string?)r[key] ?? throw new Exception($"Could not get data element '{key}' from {r}");

    private static string? GetString(this JToken r, string key)
        => (string?)r[key];

    public class YtsMovie
    {
        private readonly JObject result;
        public readonly IEnumerable<YtsDownload> Downloads;

        public YtsMovie(JObject x)
        {
            result = x;
            Downloads = x["torrents"]?.Children().OfType<JObject>().Select(j => new YtsDownload(j)) ??
                        new List<YtsDownload>();
        }

        public string Name => result.GetMandatoryString("title");
        public string Overview => result.GetMandatoryString("summary");
        public string SourceCode => result.ToString();
        public string Year => result.GetMandatoryString("year");
        public int Id => result.GetMandatoryString("id").ToInt() ?? 0;
        public string YtsUrl => result.GetMandatoryString("url");
        public string Runtime => result.GetMandatoryString("runtime");

        public IEnumerable<string> Genres =>
            result["genres"]?.Children().Select(x => x.ToObject<string>()).OfType<string>().ToArray() ??
            Array.Empty<string>();

        public string Language => result.GetMandatoryString("language");

        //Star score is out of 5 stars, we produce a 'normlised' result by adding a top mark 10/10 and a bottom mark 1/10 and recalculating
        //this is to stop a show with one 10/10 vote looking too good, this normalises it back if the number of votes is small
        public float StarScore => result.GetMandatoryString("rating").ToPercent(0);
        public string ContentRating => result.GetMandatoryString("mpa_rating");
        public string ImdbCode => result.GetMandatoryString("imdb_code");
        public string? PosterUrl => result.GetString("large_cover_image") ?? result.GetString("medium_cover_image");
        public string BackgroundUrl => result.GetMandatoryString("background_image_original");
        public string TrailerUrl => $"https://www.youtube.com/embed/{result.GetMandatoryString("yt_trailer_code")}";
    }

    public class YtsDownload
    {
        private readonly JObject result;

        public YtsDownload(JObject x)
        {
            result = x;
        }

        public string Url => result.GetMandatoryString("url");
        public string Quality => result.GetMandatoryString("quality");
        public string Size => result.GetMandatoryString("size");
    }

    private static IEnumerable<YtsMovie> GetMoviesInternal(BackgroundWorker sender, string resolution, int minRating)
    {
        List<YtsMovie> downloadedMovies = [];
        bool morePages = true;
        int page = 1;

        while (morePages)
        {
            JObject updatesJson = HttpHelper.HttpGetRequestWithRetry(
                APIRoot +
                $"list_movies.json?quality={resolution}&limit=50&page={page}&minimum_rating={minRating}&with_rt_ratings=true",
                3, 2);

            if (updatesJson["status"]?.ToString() is "ok" && updatesJson["data"]?["movies"] is not null)
            {
                JEnumerable<JObject>? x = updatesJson["data"]?["movies"]?.Children<JObject>();
                if (x != null)
                {
                    foreach (YtsMovie movie in x.Cast<JObject>()
                                 .Select(newMovie => new YtsMovie(newMovie))
                                 .Where(movie => downloadedMovies.All(m => m.Id != movie.Id)))
                    {
                        downloadedMovies.Add(movie);
                    }
                }

                page++;
                int totalEntries = updatesJson["data"]?["movie_count"]?.ToObject<int>() ?? throw new Exception();
                sender.ReportProgress(100 * page / (totalEntries / 50),$"Page {page}");
            }
            else
            {
                morePages = false;
            }
        }

        return downloadedMovies;
    }

    private static YtsMovie? GetMovieByImdbInternal(string? imdbCode)
    {
        JObject updatesJson =
            HttpHelper.HttpGetRequestWithRetry(APIRoot + $"movie_details.json?imdb_id={imdbCode}", 3, 2);

        if (updatesJson["status"]?.ToString() is "ok" && updatesJson["data"]?["movie"] is JObject o)
        {
            return new YtsMovie(o);
        }

        return null;
    }

    private static IEnumerable<YtsMovie>? GetRelatedMoviesInternal(int ytsMovieId)
    {
        JObject updatesJson =
            HttpHelper.HttpGetRequestWithRetry(APIRoot + $"movie_suggestions.json?movie_id={ytsMovieId}", 3, 2);

        if (updatesJson["status"]?.ToString() is "ok" && updatesJson["data"]?["movies"] is JArray movies)
        {
            return movies.Children<JObject>().Select(m => new YtsMovie(m));
        }

        return null;
    }

    internal static YtsMovie? GetMovieByImdb(string? imdbCode)
    {
        return HandleErrorsFrom($"get IMDB {imdbCode} movie", () => GetMovieByImdbInternal(imdbCode));
    }
    internal static IEnumerable<YtsMovie>? GetRelatedMovies(int id)
    {
        return HandleErrorsFrom($"get movies related to id {id}", () => GetRelatedMoviesInternal(id));
    }
    internal static IEnumerable<YtsMovie> GetMovies(BackgroundWorker sender, string resolution, int minRating)
    {
        return HandleErrorsFrom("get movies", () => GetMoviesInternal(sender, resolution, minRating));
    }
}
