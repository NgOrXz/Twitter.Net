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

    using Newtonsoft.Json;

    public sealed class ListCommand : TwitterCommandBase
    {
        internal ListCommand(TwitterApi twitterApi)
            : base(twitterApi, "lists")
        {
        }

        #region Public Methods

        public TwitterList AddMemberToListById(string listId, string screenName, string userId = null)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");
            
            return this.AddRemoveMembers("/members/create.json", listId, null, null, null, 
                                         !String.IsNullOrEmpty(userId) ? new[] { userId } : null, 
                                         !String.IsNullOrEmpty(screenName) ? new[] { screenName } : null);
        }

        public TwitterList AddMemberToListBySlug(string slug, string ownerScreenName, string screenName,
                                                 string ownerId, string userId = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");

            return this.AddRemoveMembers("/members/create.json", null, slug, ownerId, ownerScreenName,
                                         !String.IsNullOrEmpty(userId) ? new[] { userId } : null,
                                         !String.IsNullOrEmpty(screenName) ? new[] { screenName } : null);
        }

        /// <summary>
        /// Adds multiple members to a list. The authenticated user must own the list to be able to add members to it. 
        /// Note that lists can't have more than 500 members, and you are limited to adding up to 100 members to 
        /// a list at a time with this method.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="userIds"></param>
        /// <param name="screenNames"></param>
        /// <returns></returns>
        public TwitterList AddMembersToListById(string listId, IEnumerable<string> userIds, IEnumerable<string> screenNames)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();
            if (userIds == null && screenNames == null)
                throw new ArgumentException();

            return this.AddRemoveMembers("/members/create_all.json", listId, null, null, null, userIds, screenNames);
        }

        /// <summary>
        /// Adds multiple members to a list. The authenticated user must own the list to be able to add members to it. 
        /// Note that lists can't have more than 500 members, and you are limited to adding up to 100 members to 
        /// a list at a time with this method.
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ownerScreenName"></param>
        /// <param name="screenNames"></param>
        /// <param name="ownerId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public TwitterList AddMembersToListBySlug(string slug, string ownerScreenName, IEnumerable<string> screenNames,
                                                  string ownerId = null, IEnumerable<string> userIds = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException();
            if (userIds == null && screenNames == null)
                throw new ArgumentException();

            return this.AddRemoveMembers("/members/create_all.json", null, slug, ownerId, ownerScreenName, 
                                         userIds, screenNames);
        }

        /// <summary>
        /// Check if the specified user is a member of the specified list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntites"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        public TwitterUser Contains(string listId, string screenName, string userId, 
                                    bool includeEntites = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();
            if (userId == null && screenName == null)
                throw new ArgumentException();

            return this.ContainsOrSubscribes("members/show", listId, null, null, null, userId, screenName,
                includeEntites, skipStatus);
        }

        /// <summary>
        /// Check if the specified user is a member of the specified list.
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ownerScreenName"></param>
        /// <param name="screenName"></param>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntites"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        public TwitterUser Contains(string slug, string ownerScreenName, string screenName, string ownerId,
                                    string userId, bool includeEntites = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException();
            if (userId == null && screenName == null)
                throw new ArgumentException();

            return this.ContainsOrSubscribes("members/show", null, slug, ownerId, ownerScreenName,
                userId, screenName, includeEntites, skipStatus);
        }

        public TwitterList CreateList(string name, TwitterListMode mode = TwitterListMode.Public, string description = null)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>
                {
                    { "name", name },
                    { "mode", mode.ToString().ToLowerInvariant() }
                };

            if (!String.IsNullOrEmpty(description))
                postData.Add("description", description);

            var uri         = new Uri(this.CommandBaseUri + "/create.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var list    = TwitterList.FromDictionary(jsonObj);

            return list;
        }

        public TwitterList DestroyListById(string listId)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();

            return this.DestroyList(listId, null, null, null);
        }

        public TwitterList DestroyListBySlug(string slug, string ownerScreenName, string ownerId = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.DestroyList(null, slug, ownerId, ownerScreenName);
        }

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
            if (count < 0 || count > 1000)
                throw new ArgumentOutOfRangeException("count");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&count={1}&", !String.IsNullOrEmpty(cursor) ? cursor : "-1", count);

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

        public TwitterList RemoveMembersFromListById(string listId, IEnumerable<string> userIds, 
                                                     IEnumerable<string> screenNames)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();
            if (userIds == null && screenNames == null)
                throw new ArgumentException();

            return this.AddRemoveMembers("/members/destroy_all.json", listId, null, null, null, userIds, screenNames);
        }

        public TwitterList RemoveMembersFromListBySlug(string slug, string ownerScreenName, IEnumerable<string> screenNames,
                                                       string ownerId = null, IEnumerable<string> userIds = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException();
            if (userIds == null && screenNames == null)
                throw new ArgumentException();

            return this.AddRemoveMembers("/members/destroy_all.json", null, slug, ownerId, ownerScreenName,
                                         userIds, screenNames);
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
            return this.AddRemoveMembers("/members/destroy.json", listId, null, null, null,
                                         !String.IsNullOrEmpty(userId) ? new[] { userId } : null,
                                         !String.IsNullOrEmpty(screenName) ? new[] { screenName } : null);
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
            return this.AddRemoveMembers("/members/destroy.json", null, slug, ownerId, ownerScreenName,
                                         !String.IsNullOrEmpty(userId) ? new[] { userId } : null,
                                         !String.IsNullOrEmpty(screenName) ? new[] { screenName } : null);
        }

        public TwitterCursorPagedUserCollection RetrieveMembersById(string listId, string cursor = "-1",
                                                                    bool includeEntities = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException("A list id must be provided.");

            return this.RetrieveUsers("members", listId, null, null, null, cursor, includeEntities, skipStatus);
        }

        public TwitterCursorPagedUserCollection RetrieveMembersBySlug(string slug, string ownerScreenName, 
                                                                      string ownerId = null, string cursor = "-1", 
                                                                      bool includeEntities = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.RetrieveUsers("members", null, slug, ownerId, ownerScreenName, cursor, includeEntities, skipStatus);
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
        public TwitterCursorPagedListCollection RetrieveMemberships(string screenName = null, string userId = null,
                                                                    string cursor = "-1", bool ownedList = false)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&filter_to_owned_lists={1}&", 
                                      !String.IsNullOrEmpty(cursor) ? cursor : "-1", ownedList ? "true" : "false");

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
        /// Returns the subscribers of the specified list. Private list subscribers will only be shown 
        /// if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="cursor"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        public TwitterCursorPagedUserCollection RetrieveSubscribersById(string listId, string cursor = "-1",
                                                                        bool includeEntities = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException("A list id must be provided.");

            return this.RetrieveUsers("subscribers", listId, null, null, null, cursor, includeEntities, skipStatus);
        }

        /// <summary>
        /// Returns the subscribers of the specified list. Private list subscribers will only be shown 
        /// if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ownerScreenName"></param>
        /// <param name="ownerId"></param>
        /// <param name="cursor"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        public TwitterCursorPagedUserCollection RetrieveSubscribersBySlug(string slug, string ownerScreenName, string ownerId = null,
                                                                          string cursor = "-1", bool includeEntities = true, 
                                                                          bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.RetrieveUsers("subscribers", null, slug, ownerId, ownerScreenName, cursor, 
                                      includeEntities, skipStatus);
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
        /// Returns the specified list. Private lists will only be shown if the authenticated user owns 
        /// the specified list. 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public TwitterList RetrieveUserCreatedListById(string listId)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();

            return this.RetrieveUserCreatedList(listId, null, null, null);
        }

        /// <summary>
        /// Returns the specified list. Private lists will only be shown if the authenticated user owns 
        /// the specified list.
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ownerScreenName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public TwitterList RetrieveUserCreatedListBySlug(string slug, string ownerScreenName, string ownerId = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.RetrieveUserCreatedList(null, slug, ownerScreenName, ownerId);
        }

        /// <summary>
        /// Returns the lists of the specified (or authenticated) user. Private lists will be included 
        /// if the authenticated user is the same as the user whose lists are being returned. 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public TwitterCursorPagedListCollection RetrieveUserCreatedLists(string screenName, string userId = null, 
                                                                         string cursor = "-1")
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&", !String.IsNullOrEmpty(cursor) ? cursor : "-1");

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

        /// <summary>
        /// Check if the specified user is a subscriber of the specified list. 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntites"></param>
        /// <param name="skipStatus"></param>
        /// <returns>Returns the user if they are subscriber. Otherwise null.</returns>
        public TwitterUser Subscribes(string listId, string screenName, string userId,
                                      bool includeEntites = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();
            if (userId == null && screenName == null)
                throw new ArgumentException();

            return this.ContainsOrSubscribes("subscribers/show", listId, null, null, null, userId, screenName,
                includeEntites, skipStatus);
        }

        /// <summary>
        /// Check if the specified user is a subscriber of the specified list.
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ownerScreenName"></param>
        /// <param name="screenName"></param>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntites"></param>
        /// <param name="skipStatus"></param>
        /// <returns>Returns the user if they are subscriber. Otherwise null.</returns>
        public TwitterUser Subscribes(string slug, string ownerScreenName, string screenName, string ownerId,
                                      string userId, bool includeEntites = true, bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException();
            if (userId == null && screenName == null)
                throw new ArgumentException();

            return this.ContainsOrSubscribes("subscribers/show", null, slug, ownerId, ownerScreenName,
                userId, screenName, includeEntites, skipStatus);
        }

        public TwitterList SubscribeToListById(string listId)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();

            return this.SubscribeToList(listId, null, null, null);
        }

        public TwitterList SubscribeToListBySlug(string slug, string ownerScreenName, string ownerId = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.SubscribeToList(null, slug, ownerScreenName, ownerId);
        }

        public TwitterList UnsubscribeFromListById(string listId)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();

            return this.UnsubscribeFromList(listId, null, null, null);
        }

        public TwitterList UnsubscribeFromListBySlug(string slug, string ownerScreenName, string ownerId = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.UnsubscribeFromList(null, slug, ownerScreenName, ownerId);
        }

        public TwitterList UpdateListById(string listId, string name = null, 
                                          TwitterListMode mode = TwitterListMode.Public, string description = null)
        {
            if (String.IsNullOrEmpty(listId))
                throw new ArgumentException();

            return this.UpdateList(listId, null, null, null, name, mode, description);
        }

        public TwitterList UpdateListBySlug(string slug, string ownerScreenName, string name = null, 
                                            TwitterListMode mode = TwitterListMode.Public, 
                                            string description = null, string ownerId = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();
            if (String.IsNullOrEmpty(ownerId) && String.IsNullOrEmpty(ownerScreenName))
                throw new ArgumentException("A owner id or screen name must be provided.");

            return this.UpdateList(null, slug, ownerId, ownerScreenName, name, mode, description);
        }

        #endregion

        #region Private Methods

        private TwitterList AddRemoveMembers(string method, string listId, string slug, string ownerId, 
            string ownerScreenName, IEnumerable<string> userIds, IEnumerable<string> screenNames)
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
            if (userIds != null)
                postData.Add("user_id", String.Join(",", userIds));
            if (screenNames != null)
                postData.Add("screen_name", String.Join(",", screenNames));

            var uri = new Uri(this.CommandBaseUri + method);

            TwitterList list = null;
            try
            {
                var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);
                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
                list            = TwitterList.FromDictionary(jsonObj);

            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.BadRequest)
                    throw;
            }
            
            return list;
        }

        private TwitterUser ContainsOrSubscribes(string method, string listId, string slug, string ownerId,
            string ownerScreenName, string userId, string screenName, bool includeEntities, bool skipStatus)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?include_entities={0}&skip_status={1}&",
                                      includeEntities ? "true" : "false",
                                      skipStatus ? "true" : "false");

            if (!String.IsNullOrEmpty(listId))
                queryBuilder.AppendFormat("list_id={0}&", listId);
            if (!String.IsNullOrEmpty(slug))
                queryBuilder.AppendFormat("slug={0}&", slug);
            if (!String.IsNullOrEmpty(ownerId))
                queryBuilder.AppendFormat("owner_id={0}&", ownerId);
            if (!String.IsNullOrEmpty(ownerScreenName))
                queryBuilder.AppendFormat("owner_screen_name={0}&", ownerScreenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("user_id={0}&", userId);
            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);

            var uri = new Uri(this.CommandBaseUri + String.Format("/{0}.json{1}", 
                              method, queryBuilder.ToString().TrimEnd('&')));

            TwitterUser user = null;
            try
            {
                var response = this.TwitterApi.Authenticated ?
                               this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                               this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

                var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
                user        = TwitterUser.FromDictionary(jsonObj);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }
            
            return user;
        }

        private TwitterList DestroyList(string listId, string slug, string ownerId, string ownerScreenName)
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

            var uri = new Uri(this.CommandBaseUri + "/destroy.json");

            TwitterList list = null;
            try
            {
                var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);
                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
                list            = TwitterList.FromDictionary(jsonObj);  
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return list;
        }

        private TwitterList RetrieveUserCreatedList(string listId, string slug, string ownerScreenName, string ownerId)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("?");

            if (!String.IsNullOrEmpty(listId))
                queryBuilder.AppendFormat("list_id={0}&", listId);
            if (!String.IsNullOrEmpty(slug))
                queryBuilder.AppendFormat("slug={0}&", slug);
            if (!String.IsNullOrEmpty(ownerId))
                queryBuilder.AppendFormat("owner_id={0}&", ownerId);
            if (!String.IsNullOrEmpty(ownerScreenName))
                queryBuilder.AppendFormat("owner_screen_name={0}&", ownerScreenName);

            var uri = new Uri(this.CommandBaseUri + String.Format("/show.json{0}",
                                      queryBuilder.ToString().TrimEnd('&')));

            TwitterList list = null;
            try
            {
                var response = this.TwitterApi.Authenticated ?
                               this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

                var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
                list        = TwitterList.FromDictionary(jsonObj);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    throw;
            }

            return list;
        }

        private TwitterCursorPagedUserCollection RetrieveUsers(string method, string listId, string slug, 
                                                               string ownerId, string ownerScreenName, 
                                                               string cursor = "-1", bool includeEntities = true, 
                                                               bool skipStatus = false)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?cursor={0}&include_entities={1}&skip_status={2}&",
                                      !String.IsNullOrEmpty(cursor) ? cursor : "-1", 
                                      includeEntities ? "true" : "false", 
                                      skipStatus ? "true" : "false");

            if (!String.IsNullOrEmpty(listId))
                queryBuilder.AppendFormat("list_id={0}&", listId);
            if (!String.IsNullOrEmpty(slug))
                queryBuilder.AppendFormat("slug={0}&", slug);
            if (!String.IsNullOrEmpty(ownerId))
                queryBuilder.AppendFormat("owner_id={0}&", ownerId);
            if (!String.IsNullOrEmpty(ownerScreenName))
                queryBuilder.AppendFormat("owner_screen_name={0}&", ownerScreenName);

            var uri         = new Uri(this.CommandBaseUri + String.Format("/{0}.json{1}",
                                      method, queryBuilder.ToString().TrimEnd('&')));

            var response    = this.TwitterApi.Authenticated ?
                              this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                              this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var users   = TwitterCursorPagedUserCollection.FromDictionary(jsonObj);

            return users;
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

        private TwitterList SubscribeToList(string listId, string slug, string ownerScreenName, string ownerId)
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

            var uri         = new Uri(this.CommandBaseUri + "/subscribers/create.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var list    = TwitterList.FromDictionary(jsonObj);

            return list;
        }

        private TwitterList UnsubscribeFromList(string listId, string slug, string ownerScreenName, string ownerId)
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

            var uri         = new Uri(this.CommandBaseUri + "/subscribers/destroy.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var list    = TwitterList.FromDictionary(jsonObj);

            return list;
        }

        private TwitterList UpdateList(string listId, string slug, string ownerId, string ownerScreenName,
                                       string name, TwitterListMode mode, string description)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>
                {
                    { "mode", mode.ToString().ToLowerInvariant() }
                };

            if (!String.IsNullOrEmpty(listId))
                postData.Add("list_id", listId);
            if (!String.IsNullOrEmpty(slug))
                postData.Add("slug", slug);
            if (!String.IsNullOrEmpty(ownerId))
                postData.Add("owner_id", ownerId);
            if (!String.IsNullOrEmpty(ownerScreenName))
                postData.Add("owner_screen_name", ownerScreenName);
            if (!String.IsNullOrEmpty(name))
                postData.Add("name", name);
            if (!String.IsNullOrEmpty(description))
                postData.Add("description", description);

            var uri         = new Uri(this.CommandBaseUri + "/update.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var list    = TwitterList.FromDictionary(jsonObj);

            return list;
        }

        #endregion
    }
}
