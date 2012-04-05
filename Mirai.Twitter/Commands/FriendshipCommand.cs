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
    using System.Linq;
    using System.Text;

    using Mirai.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;

    public sealed class FriendshipCommand : TwitterCommandBase
    {
        #region Constructors and Destructors

        internal FriendshipCommand(TwitterApi twitterApi)
            : base(twitterApi, "friendships")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Allows the authenticating users to follow the dm specified in the ID parameter.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="follow">Enable notifications for the target dm.</param>
        /// <returns></returns>
        public TwitterUser Create(string screenName, string userId = null, bool follow = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>
                {
                    { "follow", follow ? "true" : "false" }
                };

            if (screenName != null)
            {
                postData.Add("screen_name", screenName);
            }

            if (userId != null)
            {
                postData.Add("user_id", userId);
            }

            if (postData.Count == 1)
                throw new ArgumentException("Either the userIds or screenNames is required for this method.");

            var uri         = new Uri(this.CommandBaseUri + "/create.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var user    = TwitterUser.FromDictionary(jsonObj);

            return user;
        }

        /// <summary>
        /// Allows the authenticating users to unfollow the dm specified in the ID parameter.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntities"></param>
        /// <returns>Returns the unfollowed dm when successful.</returns>
        public TwitterUser Destroy(string screenName, string userId = null, bool includeEntities = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>
                {
                    { "include_entities", includeEntities ? "true" : "false" }
                };

            if (screenName != null)
            {
                postData.Add("screen_name", screenName);
            }

            if (userId != null)
            {
                postData.Add("user_id", userId);
            }

            if (postData.Count == 1)
                throw new ArgumentException("Either the userIds or screenNames is required for this method.");

            var uri         = new Uri(this.CommandBaseUri + "/destroy.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var user    = TwitterUser.FromDictionary(jsonObj);

            return user;
        }

        public TwitterRelationship[] Lookup(IEnumerable<string> screenNames, IEnumerable<string> userIds = null)
        {
            var queryBuilder = new StringBuilder();
            if (screenNames != null && screenNames.Any())
                queryBuilder.AppendFormat("screen_name={0}&", String.Join(",", screenNames));
            if (userIds != null && userIds.Any())
                queryBuilder.AppendFormat("user_id={0}", String.Join(",", userIds));

            if (queryBuilder.Length == 0)
                throw new ArgumentException("Either the userIds or screenNames is required for this method.");

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri +
                                      String.Format("/lookup.json?{0}", queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
            var rels         = (from Dictionary<string, object> rel in jsonArray
                                select TwitterRelationship.FromDictionary(rel)).ToArray();

            return rels;
        }

        /// <summary>
        /// Returns an array of numeric IDs for every dm the specified dm is following.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedIdCollection RetrieveIdsForFriends(string screenName, string userId = null, string cursor = "-1")
        {
            return this.RetrieveIds(screenName, userId, cursor, "friends");
        }

        /// <summary>
        /// Returns an array of numeric IDs for every dm following the specified dm.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedIdCollection RetrieveIdsForFollowers(string screenName, string userId = null, string cursor = "-1")
        {
            return this.RetrieveIds(screenName, userId, cursor, "followers");
        }

        /// <summary>
        /// Returns an array of numeric IDs for every dm who has a pending request to follow the authenticating dm.
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedIdCollection RetrieveIdsForIncomingRequests(string cursor = "-1")
        {
            return this.RetrieveIdsForFollowingRequests(cursor, "incoming");
        }

        /// <summary>
        /// Returns an array of numeric IDs for every protected dm for whom the authenticating dm has a pending 
        /// follow request.
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedIdCollection RetrieveIdsForOutgoingRequests(string cursor = "-1")
        {
            return this.RetrieveIdsForFollowingRequests(cursor, "outgoing");
        }

        /// <summary>
        /// Returns an array of user_ids that the currently authenticated dm does not want to see retweets from.
        /// </summary>
        /// <returns></returns>
        public string[] RetrieveNoRetweetIds()
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + "/no_retweet_ids.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
            var ids         = (from string id in jsonArray select id).ToArray();

            return ids;
        }

        /// <summary>
        /// Returns detailed information about the relationship between two users.
        /// </summary>
        /// <param name="sourceScreenName"></param>
        /// <param name="targetScreenName"></param>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public TwitterRelationship Show(string sourceScreenName, string targetScreenName, 
                                        string sourceId = null, string targetId = null)
        {
            if (String.IsNullOrEmpty(sourceId) && String.IsNullOrEmpty(sourceScreenName))
                throw new ArgumentException("Either a sourceId or sourceScreenName is required for this method.");
            if (String.IsNullOrEmpty(targetId) && String.IsNullOrEmpty(targetScreenName))
                throw new ArgumentException("Either a targetId or targetScreenName is required for this method.");

            var queryBuilder = new StringBuilder();
            if (!String.IsNullOrEmpty(sourceId))
                queryBuilder.AppendFormat("source_id={0}&", sourceId);
            if (!String.IsNullOrEmpty(sourceScreenName))
                queryBuilder.AppendFormat("source_screen_name={0}&", sourceScreenName);
            if (!String.IsNullOrEmpty(targetId))
                queryBuilder.AppendFormat("target_id={0}&", targetId);
            if (!String.IsNullOrEmpty(targetScreenName))
                queryBuilder.AppendFormat("target_screen_name={0}", targetScreenName);

            var uri         = new Uri(this.CommandBaseUri + 
                                      String.Format("/show.json?{0}", queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var rel         = TwitterRelationship.FromDictionary(jsonObj["relationship"] as Dictionary<string, object>);

            return rel;
        }

        /// <summary>
        /// Allows one to enable or disable retweets and device notifications from the specified dm. 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="device"></param>
        /// <param name="retweets"></param>
        /// <returns></returns>
        public TwitterRelationship Update(string screenName, string userId = null, 
                                          bool? device = null, bool? retweets = null)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("Either a userId or screenName is required for this method.");

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(screenName))
                postData.Add("screen_name", screenName);
            if (!String.IsNullOrEmpty(userId))
                postData.Add("user_id", userId);
            if (device.HasValue)
                postData.Add("device", device.Value ? "true" : "false");
            if (retweets.HasValue)
                postData.Add("retweets", retweets.Value ? "true" : "false");

            var uri         = new Uri(this.CommandBaseUri + "/update.json");

            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var rel         = TwitterRelationship.FromDictionary(jsonObj["relationship"] as Dictionary<string, object>);

            return rel;
        }

        #endregion

        private TwitterCursorPagedIdCollection RetrieveIdsForFollowingRequests(string cursor, string type)
        {
            if (String.IsNullOrEmpty(cursor))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + String.Format("/{0}.json?cursor={1}", type, cursor));
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);
            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var ids         = TwitterCursorPagedIdCollection.FromDictionary(jsonObj);

            return ids;
        }

        private TwitterCursorPagedIdCollection RetrieveIds(string screenName, string userId, string cursor, string userType)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("Either a userId or screenName is required for this method.");
            if (String.IsNullOrEmpty(cursor))
                throw new ArgumentException();

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&", cursor);

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("user_id={0}", userId);

            var uri         = new Uri(TwitterApi.ApiBaseUri + "/" + this.TwitterApi.ApiVersion.ToString("D") + 
                                      String.Format("/{0}/ids.json{1}", userType, queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var ids         = TwitterCursorPagedIdCollection.FromDictionary(jsonObj);

            return ids;
        }
    }
}