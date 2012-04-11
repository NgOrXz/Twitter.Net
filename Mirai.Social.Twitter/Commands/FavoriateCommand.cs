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
    using System.Text;

    using Mirai.Net.OAuth;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    public sealed class FavoriteCommand : TwitterCommandBase
    {
        #region Constructors and Destructors

        internal FavoriteCommand(TwitterApi twitterApi)
            : base(twitterApi, "favorites")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Favorites the status specified in the ID parameter as the authenticating dm. Returns the favorite 
        /// status when successful.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        /// <remarks>
        /// This process invoked by this method is asynchronous. The immediately returned status may not 
        /// indicate the resultant favorited status of the tweet. A 200 OK response from this method will 
        /// indicate whether the intended action was successful or not.
        /// </remarks>
        public TwitterTweet Create(string id, bool includeEntities = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            var postData = new Dictionary<string, string>
                {
                    { "include_entities", includeEntities ? "true" : "false" },
                };

            var uri         = new Uri(this.CommandBaseUri + String.Format("/create/{0}.json", id));
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var twitterTweet    = TwitterObject.Parse<TwitterTweet>(response);

            return twitterTweet;
        }

        /// <summary>
        /// Un-favorites the status specified in the ID parameter as the authenticating user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns the un-favorited status in the requested format when successful. </returns>
        public TwitterTweet Destroy(string id)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            var uri         = new Uri(this.CommandBaseUri + String.Format("/destroy/{0}.json", id));
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, null);

            var twitterTweet    = TwitterObject.Parse<TwitterTweet>(response);

            return twitterTweet;
        }

        /// <summary>
        /// Returns the specified number of most recent favorite statuses for the authenticating dm or 
        /// dm specified by the idOrScreenName parameter.
        /// </summary>
        /// <param name="idOrScreenName">
        /// The ID or screen name of the dm for whom to request a list of favorite statuses.
        /// </param>
        /// <param name="count">Specifies the number of records to retrieve. Must be less than or equal to 200. 
        /// Defaults to 20.
        /// </param>
        /// <param name="sinceId">
        /// Returns results with an ID greater than (that is, more recent than) the specified ID. There are limits 
        /// to the number of Tweets which can be accessed through the API. If the limit of Tweets has occurred 
        /// since the since_id, the since_id will be forced to the oldest ID available.
        /// </param>
        /// <param name="maxId">
        /// Returns results with an ID less than (that is, older than) or equal to the specified ID.
        /// </param>
        /// <param name="page">Specifies the page of results to retrieve.</param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterTweet[] RetrieveFavorites(string idOrScreenName = null, int count = 20, string sinceId = null,
                                                string maxId = null, int page = 1, bool includeEntities = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?count={0}&page={1}&include_entities={2}",
                                      count > 200 ? 200 : count, page < 1 ? 1 : page, includeEntities ? "true" : "false");
            
            if (!String.IsNullOrEmpty(idOrScreenName))
                queryBuilder.AppendFormat("&id={0}", idOrScreenName);
            if (!String.IsNullOrEmpty(sinceId))
                queryBuilder.AppendFormat("&since_id={0}", sinceId);
            if (!String.IsNullOrEmpty(maxId))
                queryBuilder.AppendFormat("&max_id={0}", maxId);

            var uri         = new Uri(this.CommandBaseUri + ".json" + queryBuilder);

            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);
            var tweets      = JsonConvert.DeserializeObject<TwitterTweet[]>(response);

            return tweets;
        }

        #endregion
    }
}