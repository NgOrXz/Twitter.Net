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
    using System.Net;
    using System.Text;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

    using fastJSON;

    public sealed class BlockCommand : TwitterCommandBase
    {
        internal BlockCommand(TwitterApi twitterApi)
            : base(twitterApi, "blocks")
        {
        }


        #region PUblic Methods

        /// <summary>
        /// Un-blocks the user specified in the ID parameter for the authenticating user. 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        public TwitterUser BlockUser(string screenName, string userId = null, bool includeEntities = true, bool skipStatus = false)
        {
            return this.BlockUnblockUser("/create.json", screenName, userId, includeEntities, skipStatus);
        }

        public TwitterUser UnblockUser(string screenName, string userId = null, bool includeEntities = true, bool skipStatus = false)
        {
            return this.BlockUnblockUser("/destroy.json", screenName, userId, includeEntities, skipStatus);
        }

        /// <summary>
        /// Returns if the authenticating user is blocking a target user. 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns>Return the blocked user's object if a block exists. Otherwise, null.</returns>
        public TwitterUser IsBlocked(string screenName, string userId = null, bool includeEntities = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?include_entities={0}&skip_status={1}&", includeEntities ? "true" : "false", skipStatus ? "true" : "false");

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("&user_id={0}", userId);

            var uri = new Uri(this.CommandBaseUri + String.Format("/exists.json{0}", queryBuilder.ToString().TrimEnd('&')));

            TwitterUser user = null;
            try
            {
                var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);
                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
                user            = TwitterUser.FromDictionary(jsonObj);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return user;
        }

        /// <summary>
        /// Returns an array of user objects that the authenticating user is blocking.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedUserCollection RetrieveBlockedUsers(int page = 1, int perPage = 20, bool includeEntities = true,
                                                                     bool skipStatus = false, string cursor = "-1")
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?page={0}&per_page={1}&include_entities={2}&skip_status={3}&cursor={4}",
                                      page > 1 ? page : 1, perPage, includeEntities ? "true" : "false", 
                                      skipStatus ? "true" : "false", !String.IsNullOrEmpty(cursor) ? cursor : "-1");

            var uri = new Uri(this.CommandBaseUri +
                                      String.Format("/blocking.json{0}", queryBuilder.ToString()));

            var response = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var users   = TwitterCursorPagedUserCollection.FromDictionary(jsonObj);

            return users;
        }

        public TwitterCursorPagedIdCollection RetrieveIdsOfBlockedUsers(string cursor = "-1")
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri = new Uri(this.CommandBaseUri +
                              String.Format("/blocking/ids.json?cursor={0}", !String.IsNullOrEmpty(cursor) ? cursor : "-1"));

            var response = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var ids     = TwitterCursorPagedIdCollection.FromDictionary(jsonObj);

            return ids;
        }

        #endregion

        #region Private Methods

        private TwitterUser BlockUnblockUser(string method, string screenName, string userId, bool includeEntities, bool skipStatus)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>
                {
                    { "include_entities", includeEntities.ToString().ToLowerInvariant() },
                    { "skip_status", skipStatus.ToString().ToLowerInvariant() }
                };

            if (!String.IsNullOrEmpty(userId))
                postData.Add("user_id", userId);
            if (!String.IsNullOrEmpty(screenName))
                postData.Add("screen_name", screenName);

            var uri = new Uri(this.CommandBaseUri + method);

            TwitterUser user = null;
            try
            {
                var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);
                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
                user            = TwitterUser.FromDictionary(jsonObj);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    throw;
            }

            return user;
        }

        #endregion
    }
}
