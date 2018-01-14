using System;
namespace TVRename.Core.Extensions
{
    /// <summary>
    /// <see cref="DateTime"/> extention methods.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts the <see cref="DateTime"/> to its equivalent Unix timestamp representation.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Unix timestamp representation of the <see cref="DateTime"/>.</returns>
        public static long ToUnixTime(this DateTime date)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }
    }
}
