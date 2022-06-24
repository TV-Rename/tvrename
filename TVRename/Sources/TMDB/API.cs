using System;
using System.Collections.Generic;
using System.Threading;
using TMDbLib.Client;
using TMDbLib.Objects.Changes;
using TMDbLib.Objects.General;
using System.Threading.Tasks;
using System.Net.Http;

namespace TVRename.TMDB;

// ReSharper disable once InconsistentNaming
internal static class API
{
    //As a safety measure we check that no more than 52 calls are made
    private const int MAX_NUMBER_OF_CALLS = 50;

    public static IEnumerable<ChangesListItem> GetChangesMovies(this TMDbClient client, CancellationToken cts, UpdateTimeTracker latestUpdateTime)
        => GetChanges(cts, latestUpdateTime, client.GetMoviesChangesAsync);

    public static IEnumerable<ChangesListItem> GetChangesShows(this TMDbClient client, CancellationToken cts, UpdateTimeTracker latestUpdateTime)
        => GetChanges(cts,latestUpdateTime, client.GetTvChangesAsync);

    private static IEnumerable<ChangesListItem> GetChanges(CancellationToken cts, UpdateTimeTracker latestUpdateTime, Func<int, DateTime?, DateTime?, CancellationToken, Task<SearchContainer<ChangesListItem>>> changeMethod)
    {
        //We need to ask for updates in blocks of 14 days
        //We'll keep asking until we get to a date within 14 days of today
        //(up to a maximum of 52 - if you are this far behind then you may need multiple refreshes)
        try
        {
            List<ChangesListItem> updatesResponses = new();
            int numberOfCallsMade = 0;

            for (DateTime time = latestUpdateTime.LastSuccessfulServerUpdateDateTime();
                 time <= DateTime.Now;
                 time = time.AddDays(14)
                )
            {
                int maxPage = 1;
                for (int currentPage = 0; currentPage < maxPage; currentPage++)
                {
                    if (cts.IsCancellationRequested)
                    {
                        throw new TaskCanceledException("Manual Cancellation");
                    }
                    SearchContainer<ChangesListItem> response = changeMethod(currentPage, time, null, cts).Result;
                    numberOfCallsMade++;
                    maxPage = response.TotalPages;
                    updatesResponses.AddRange(response.Results);
                    if (numberOfCallsMade > MAX_NUMBER_OF_CALLS)
                    {
                        throw new TooManyCallsException();
                    }
                }
            }
            return updatesResponses;
        }
        catch (AggregateException aex) when (aex.InnerException is HttpRequestException ex)
        {
            throw new SourceConnectivityException(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            throw new SourceConnectivityException(ex.Message);
        }
        catch (Exception e)
        {
            throw new SourceConnectivityException(e.Message);
        }
    }

    public class TooManyCallsException : Exception
    {
    }

    public static string WebsiteShowUrl(CachedSeriesInfo ser)
    {
        return WebsiteShowUrl(ser.TmdbCode);
    }

    public static string WebsiteShowUrl(ShowConfiguration si)
    {
        return WebsiteShowUrl(si.TmdbCode);
    }

    public static string WebsiteShowUrl(int seriesId)
    {
        return $"https://www.themoviedb.org/tv/{seriesId}";
    }
    public static string WebsiteMovieUrl(int seriesId)
    {
        return $"https://www.themoviedb.org/movie/{seriesId}";
    }
}
