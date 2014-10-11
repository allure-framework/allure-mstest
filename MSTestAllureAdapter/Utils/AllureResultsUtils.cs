using System;
using System.Linq;
using System.Collections.Generic;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// Utility method 
    /// Allure results utils.
    /// </summary>
    public static class AllureResultsUtils
    {
        public static long ToUnixEpochTime(this DateTime time)
        {
            return (long) (time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}

