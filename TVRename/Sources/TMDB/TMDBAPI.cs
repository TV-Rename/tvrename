using System;
using System.Collections.Generic;
using System.Threading;
using NLog;
using TMDbLib.Client;
using TMDbLib.Objects.Changes;

namespace TVRename.TMDB
{
    // ReSharper disable once InconsistentNaming
    internal static class API
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //As a safety measure we check that no more than 52 calls are made
        internal const int MAX_NUMBER_OF_CALLS = 50;

        public static IEnumerable<ChangesListItem> GetChangesMovies(this TMDbClient client, CancellationToken cts, UpdateTimeTracker latestUpdateTime)
        {

            //We need to ask for updates in blocks of 7 days
            //We'll keep asking until we get to a date within 7 days of today
            //(up to a maximum of 52 - if you are this far behind then you may need multiple refreshes)

            List<ChangesListItem> updatesResponses = new List<ChangesListItem>();
            bool moreUpdates = true;
            int numberOfCallsMade = 0;

            for (DateTime time = latestUpdateTime.LastSuccessfulServerUpdateDateTime();
                time <= DateTime.Now;
                time = time.AddDays(14)
                )
            {
                int maxPage = 1;
                for (int currentPage = 0; currentPage <= maxPage; currentPage++)
                {
                    if (cts.IsCancellationRequested)
                    {
                        throw new CancelledException();
                    }
                    var response = client.GetChangesMoviesAsync(currentPage, time, cancellationToken: cts).Result;
                    numberOfCallsMade ++;
                    maxPage = response.TotalPages;
                    updatesResponses.AddRange(response.Results);
                    if (numberOfCallsMade > MAX_NUMBER_OF_CALLS)
                    {
                        throw new TooManyCallsException();
                    }
                }

            }

            latestUpdateTime.RegisterServerUpdate(DateTime.Now.ToUnixTime());

            return updatesResponses;
        }

        internal class TooManyCallsException : Exception
        {
        }
        internal class CancelledException : Exception
        {
        }
    }
}
