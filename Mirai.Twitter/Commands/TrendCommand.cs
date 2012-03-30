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

namespace Mirai.Twitter.Commands
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

    using fastJSON;

    public sealed class TrendCommand : TwitterCommandBase
    {
        #region Constructors and Destructors

        internal TrendCommand(TwitterApi twitterApi)
            : base(twitterApi, "trends")
        {
        }

        #endregion

        #region Public Methods

        public TwitterTrendTopic RetrieveDailyTrends(bool exclude = false)
        {
            return this.RetrieveTrends("daily");
        }

        public TwitterTrendTopic[] RetrieveTrendTopics(string woeid, bool exclude = false)
        {
            if (String.IsNullOrEmpty(woeid))
                throw new ArgumentException();

            var uri         = new Uri(this.CommandBaseUri + String.Format("/{0}.json?exclude={1}",
                                                                          woeid,
                                                                          exclude ? "true" : "false"));

            TwitterTrendTopic[] topics = null;
            try
            {
                var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

                var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
                topics          = (from Dictionary<string, object> topic in jsonArray
                          select TwitterTrendTopic.FromDictionary(topic)).ToArray();
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return topics;
        }

        public TwitterTrendLocation[] RetrieveTrendLocations(double? latitude = null, double? longitude = null)
        {
            var queryBuilder = new StringBuilder();
            if (latitude.HasValue && (latitude.Value >= -180 && latitude.Value <= 180))
                queryBuilder.AppendFormat("lat={0}&", latitude.Value);
            if (longitude.HasValue && (longitude.Value >= -180 && longitude.Value <= 180))
                queryBuilder.AppendFormat("long={0}", longitude.Value);
            if (queryBuilder.Length > 0)
                queryBuilder.Insert(0, "?");

            var uri         = new Uri(this.CommandBaseUri + "/available.json" + queryBuilder);
            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
            var locations   = (from Dictionary<string, object> loc in jsonArray
                               select TwitterTrendLocation.FromDictionary(loc)).ToArray();

            return locations;
        }

        public TwitterTrendTopic RetrieveWeeklyTrends(bool exclude = false)
        {
            return this.RetrieveTrends("weekly");
        }

        #endregion

        #region Private Methods

        private TwitterTrendTopic RetrieveTrends(string method, bool exclude = false)
        {
            var uri         = new Uri(this.CommandBaseUri + 
                                      String.Format("/{0}.json?date={1}&exclude={2}", 
                                                    method,
                                                    DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                                    exclude ? "true" : "false"));

            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var topic       = TwitterTrendTopic.FromDictionary(jsonObj);

            return topic;
        }

        #endregion
    }
}
