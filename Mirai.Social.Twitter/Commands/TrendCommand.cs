// ------------------------------------------------------------------------------------------------------
// Copyright (c) 2012, Kevin Wang
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
// following conditions are met:
//
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and 
//    the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ------------------------------------------------------------------------------------------------------

namespace Mirai.Social.Twitter.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class TrendCommand : TwitterCommandBase
    {
        #region Constructors and Destructors

        internal TrendCommand(TwitterApi twitterApi)
            : base(twitterApi, "trends")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the top 20 trending topics for each hour in a given day.
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public IEnumerable<IGrouping<DateTime, TwitterTrend>> RetrieveDailyTrends(bool exclude = false)
        {
            return this.RetrieveTrends("daily");
        }

        /// <summary>
        /// Returns the top 10 trending topics for a specific WOEID, if trending information is available for it. 
        /// </summary>
        /// <param name="woeid"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public TwitterTrendTopic[] RetrieveTrendsByWoeId(string woeid, bool exclude = false)
        {
            if (String.IsNullOrEmpty(woeid))
                throw new ArgumentException();

            var uri = new Uri(this.CommandBaseUri + String.Format("/{0}.json?exclude={1}",
                                                                  woeid,
                                                                  exclude ? "true" : "false"));

            TwitterTrendTopic[] trendTopic = null;
            try
            {
                var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

                trendTopic      = JsonConvert.DeserializeObject<TwitterTrendTopic[]>(response);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return trendTopic;
        }

        /// <summary>
        /// Returns the locations that Twitter has trending topic information for. 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public TwitterTrendLocation[] RetrieveTrendLocations(double? latitude = null, double? longitude = null)
        {
            var queryBuilder = new StringBuilder();
            if (latitude.HasValue)
            {
                if (latitude.Value < -180 || latitude.Value > 180)
                    throw new ArgumentOutOfRangeException();

                queryBuilder.AppendFormat("lat={0}&", latitude.Value);
            }
            if (longitude.HasValue)
            {
                if (longitude.Value < -180 && longitude.Value > 180)
                    throw new ArgumentOutOfRangeException();

                queryBuilder.AppendFormat("long={0}", longitude.Value);
            }
            if (queryBuilder.Length > 0)
                queryBuilder.Insert(0, "?");

            var uri         = new Uri(this.CommandBaseUri + "/available.json" + queryBuilder);
            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var locations   = JsonConvert.DeserializeObject<TwitterTrendLocation[]>(response);

            return locations;
        }

        /// <summary>
        /// Returns the top 30 trending topics for each day in a given week.
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public IEnumerable<IGrouping<DateTime, TwitterTrend>> RetrieveWeeklyTrends(bool exclude = false)
        {
            return this.RetrieveTrends("weekly");
        }

        #endregion

        #region Private Methods

        private IEnumerable<IGrouping<DateTime, TwitterTrend>> RetrieveTrends(string method, bool exclude = false)
        {
            var uri         = new Uri(this.CommandBaseUri + 
                                      String.Format("/{0}.json?date={1}&exclude={2}", 
                                                    method,
                                                    DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                                    exclude ? "true" : "false"));

            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var trends      = new List<IGrouping<DateTime, TwitterTrend>>();
            var jsonObj     = JObject.Parse(response);
            foreach (var p in ((JObject)jsonObj["trends"]).Properties())
            {
                trends.Add(new TwitterTrendGroup(DateTime.Parse(p.Name, CultureInfo.InvariantCulture),
                                                JsonConvert.DeserializeObject<TwitterTrend[]>(p.Value.ToString())));
            }

            return trends;
        }

        #endregion
    }
}
