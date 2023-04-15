using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Web;

namespace Models.Utilities
{
    public static class UrlUtility
    {
        /// <summary>
        /// Converts connection string url to resource
        /// </summary>
        /// <param name="connectionStringUrl"></param>
        /// <returns></returns>
        public static (Uri, IReadOnlyDictionary<string, string>) UrlToResource(string connectionStringUrl)
        {
            var isUrl = Uri.TryCreate(connectionStringUrl, UriKind.Absolute, out var url);

            if (!isUrl)
            {
                return (default, new Dictionary<string, string>());
            }

            var connectionStringBuilder = new Dictionary<string, string>
            {
                ["Host"] = url.Host,
                ["Username"] = HttpUtility.UrlDecode(url.UserInfo.Split(':').GetValue(0)?.ToString()),
                ["Password"] = HttpUtility.UrlDecode(url.UserInfo.Split(':').GetValue(1)?.ToString()),
                ["Database"] = url.LocalPath.Substring(1),
                ["ApplicationName"] = "config-api"
            };

            return (url, connectionStringBuilder);
        }
    }
}