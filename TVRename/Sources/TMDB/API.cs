using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using TMDbLib.Client;
using TMDbLib.Objects.Changes;
using TMDbLib.Objects.General;

namespace TVRename.TMDB
{
    // ReSharper disable once InconsistentNaming
    internal static class API
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //As a safety measure we check that no more than 52 calls are made
        private const int MAX_NUMBER_OF_CALLS = 50;

        public static IEnumerable<ChangesListItem> GetChangesMovies(this TMDbClient client, CancellationToken cts, UpdateTimeTracker latestUpdateTime)
        {
            //We need to ask for updates in blocks of 7 days
            //We'll keep asking until we get to a date within 7 days of today
            //(up to a maximum of 52 - if you are this far behind then you may need multiple refreshes)

            List<ChangesListItem> updatesResponses = new List<ChangesListItem>();
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
                        throw new CancelledException();
                    }
                    SearchContainer<ChangesListItem>? response = client.GetMoviesChangesAsync(currentPage, time, cancellationToken: cts).Result;
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

        public static IEnumerable<ChangesListItem> GetChangesShows(this TMDbClient client, CancellationToken cts, UpdateTimeTracker latestUpdateTime)
        {
            //We need to ask for updates in blocks of 7 days
            //We'll keep asking until we get to a date within 7 days of today
            //(up to a maximum of 52 - if you are this far behind then you may need multiple refreshes)

            List<ChangesListItem> updatesResponses = new List<ChangesListItem>();
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
                        throw new CancelledException();
                    }
                    SearchContainer<ChangesListItem>? response = client.GetTvChangesAsync(currentPage, time, cancellationToken: cts).Result;
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

        private class TooManyCallsException : Exception
        {
        }

        private class CancelledException : Exception
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
}
