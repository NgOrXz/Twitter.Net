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

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

    using fastJSON;

    public sealed class ListCommand : TwitterCommandBase
    {
        internal ListCommand(TwitterApi twitterApi)
            : base(twitterApi, "lists")
        {
        }

        #region Public Methods

        /// <summary>
        /// Returns all tweets the authenticating or specified user subscribes to, including their own. 
        /// If no user is given, the authenticating user is used. 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TwitterList[] RetrieveAllLists(string screenName = null, string userId = null)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("?");

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("&user_id={0}", userId);

            var uri         = new Uri(this.CommandBaseUri + String.Format("/all.json{0}",
                                      queryBuilder.Length > 1 ? queryBuilder.ToString().TrimEnd('&') : ""));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
            var lists       = (from Dictionary<string, object> tweet in jsonArray
                               select TwitterList.FromDictionary(tweet)).ToArray();

            return lists;
        }

        /// <summary>
        /// Obtain a collection of the tweets the specified user is subscribed to, 20 tweets per page by default. 
        /// Does not include the user's own tweets.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedListCollection RetrieveSubscriptions(string screenName, string userId = null,
                                                                      int count = 20, string cursor = "-1")
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");
            if (String.IsNullOrEmpty(cursor))
                throw new ArgumentException();
            if (count < 0 || count > 1000)
                throw new ArgumentOutOfRangeException("count");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&count={1}&", cursor, count);

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("&user_id={0}", userId);

            var uri         = new Uri(this.CommandBaseUri +
                                      String.Format("/subscriptions.json{0}", queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var lists   = TwitterCursorPagedListCollection.FromDictionary(jsonObj);

            return lists;
        }

        /// <summary>
        /// Removes the specified member from the list. The authenticated user must be the list's owner to remove 
        /// members from the list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TwitterList RemoveMemberFromListById(string listId, string screenName, string userId = null)
        {
            return this.RemoveMemberFromList(listId, null, userId, screenName, null, null);
        }

        /// <summary>
        /// Removes the specified member from the list. The authenticated user must be the list's owner to remove 
        /// members from the list.
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ownerScreenName"></param>
        /// <param name="screenName"></param>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TwitterList RemoveMemberFromListBySlug(string slug, string ownerScreenName, string screenName,
                                               string ownerId = null, string userId = null)
        {
            return this.RemoveMemberFromList(null, slug, userId, screenName, ownerId, ownerScreenName);
        }

        /// <summary>
        /// Returns the lists the specified user has been added to.
        /// If user id or screen name are not provided the memberships for the authenticating user are returned.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="cursor"></param>
        /// <param name="ownedList"></param>
        /// <returns></returns>
        public TwitterCursorPagedListCollection RetrieveMembership(string screenName = null, string userId = null,
                                                                   string cursor = "-1", bool ownedList = false)
        {
            if (String.IsNullOrEmpty(cursor))
                throw new ArgumentException();

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&filter_to_owned_lists={1}&", cursor, ownedList ? "true" : "false");

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("&user_id={0}", userId);

            var uri         = new Uri(this.CommandBaseUri +
                                      String.Format("/memberships.json{0}", queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var lists   = TwitterCursorPagedListCollection.FromDictionary(jsonObj);

            return lists;
        }

        /// <summary>
        /// Returns tweet timeline for members of the specified list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="sinceId"></param>
        /// <param name="maxId"></param>
        /// <param name="perPage"></param>
        /// <param name="page"></param>
        /// <param name="includeEntities"></param>
        /// <param name="includeRetweets"></param>
        /// <returns></returns>
        public TwitterTweet[] RetrieveTweetsOfListMembersById(string listId, string sinceId = null, 
                                                              string maxId = null, int perPage = 10, int page = 1, 
                                                              bool includeEntities = true, bool includeRetweets = true)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();

            return this.RetrieveTweetsOfListMembers(listId, null, null, null, sinceId, maxId, perPage,
                                                    page, includeEntities, includeRetweets);
        }

        /// <summary>
        /// Returns tweet timeline for members of the specified list.
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ownerScreenName"></param>
        /// <param name="ownerId"></param>
        /// <param name="sinceId"></param>
        /// <param name="maxId"></param>
        /// <param name="perPage"></param>
        /// <param name="page"></param>
        /// <param name="includeEntities"></param>
        /// <param name="includeRetweets"></param>
        /// <returns></returns>
        public TwitterTweet[] RetrieveTweetsOfListMembersBySlug(string slug, string ownerScreenName, 
                                                                string ownerId = null, string sinceId = null, 
                                                                string maxId = null, int perPage = 10, int page = 1, 
                                                                bool includeEntities = true, bool includeRetweets = true)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.RetrieveTweetsOfListMembers(null, slug, ownerScreenName, ownerId, sinceId, 
                                                    maxId, perPage, page, includeEntities, includeRetweets);
        }

        /// <summary>
        /// Returns the tweets of the specified (or authenticated) user. Private tweets will be included 
        /// if the authenticated user is the same as the user whose tweets are being returned. 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedListCollection RetrieveUserLists(string screenName, string userId = null, 
                                                              string cursor = "-1")
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");
            if (String.IsNullOrEmpty(cursor))
                throw new ArgumentException();

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&", cursor);

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("&user_id={0}", userId);

            var uri         = new Uri(this.CommandBaseUri + 
                                      String.Format(".json{0}",queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var lists   = TwitterCursorPagedListCollection.FromDictionary(jsonObj);

            return lists;
        }

        #endregion

        #region Private Methods

        private TwitterList RemoveMemberFromList(string listId, string slug, string userId, string screenName,
                                          string ownerId, string ownerScreenName)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(listId))
                postData.Add("list_id", listId);
            if (!String.IsNullOrEmpty(slug))
                postData.Add("slug", slug);
            if (!String.IsNullOrEmpty(ownerId))
                postData.Add("owner_id", ownerId);
            if (!String.IsNullOrEmpty(ownerScreenName))
                postData.Add("owner_screen_name", ownerScreenName);
            if (!String.IsNullOrEmpty(userId))
                postData.Add("owner_id", userId);
            if (!String.IsNullOrEmpty(screenName))
                postData.Add("screen_name", screenName);

            var uri         = new Uri(this.CommandBaseUri + "/members/destroy.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var list   = TwitterList.FromDictionary(jsonObj);

            return list;
        }
        
        private TwitterTweet[] RetrieveTweetsOfListMembers(string listId, string slug,
            string ownerScreenName, string ownerId, string sinceId, string maxId,
            int perPage = 10, int page = 1, bool includeEntities = true, bool includeRetweets = true)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?per_page={0}&page={1}&include_entities={2}&include_rts={3}&", 
                perPage, page, includeEntities ? "true" : "false", includeRetweets ? "true" : "false");

            if (!String.IsNullOrEmpty(listId))
                queryBuilder.AppendFormat("list_id={0}&", listId);
            if (!String.IsNullOrEmpty(slug))
                queryBuilder.AppendFormat("slug={0}&", slug);
            if (!String.IsNullOrEmpty(ownerId))
                queryBuilder.AppendFormat("owner_id={0}&", ownerId);
            if (!String.IsNullOrEmpty(ownerScreenName))
                queryBuilder.AppendFormat("owner_screen_name={0}&", ownerScreenName);
            if (!String.IsNullOrEmpty(sinceId))
                queryBuilder.AppendFormat("since_id={0}&", sinceId);
            if (!String.IsNullOrEmpty(maxId))
                queryBuilder.AppendFormat("max_id={0}", maxId);

            var uri         = new Uri(this.CommandBaseUri + String.Format("/statuses.json{0}",
                                      queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
            var tweets      = (from Dictionary<string, object> tweet in jsonArray
                               select TwitterTweet.FromDictionary(tweet)).ToArray();

            return tweets;
        }

        #endregion
    }
}
