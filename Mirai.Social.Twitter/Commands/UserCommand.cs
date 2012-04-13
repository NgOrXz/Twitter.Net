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
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Mirai.Net.OAuth;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    public sealed class UserCommand : TwitterCommandBase
    {
        #region Nested Members

        public enum ProfileImageSize
        {
            /// <summary>
            /// 73px by 73px
            /// </summary>
            Bigger, 

            /// <summary>
            /// 48px by 48px
            /// </summary>
            Normal, 

            /// <summary>
            /// 24px by 24px
            /// </summary>
            Mini,

            Original
        }

        #endregion

        #region Constructors and Destructors

        internal UserCommand(TwitterApi twitterApi) : base(twitterApi, "users")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return up to 100 users worth of extended information, specified by either ID, screen name, or 
        /// combination of the two. The author's most recent status (if the authenticating user has permission) 
        /// will be returned inline.
        /// </summary>
        /// <param name="screenNames"></param>
        /// <param name="userIds"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterUser[] Lookup(IEnumerable<string> screenNames, IEnumerable<string> userIds, 
                                    bool includeEntities = true)
        {
            var postData = new Dictionary<string, string>
                {
                    { "include_entities", includeEntities ? "true" : "false" }
                };

            if (screenNames != null && screenNames.Any())
            {
                postData.Add("screen_name", String.Join(", ", screenNames));
            }

            if (userIds != null && userIds.Any())
            {
                postData.Add("user_id", String.Join(", ", userIds));
            }

            if (postData.Count == 1)
                throw new ArgumentException("Either the userIds or screenNames is required for this method.");

            var uri = new Uri(this.CommandBaseUri + "/lookup.json");

            var response = this.TwitterApi.Authenticated ?
                           this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData) :
                           this.TwitterApi.ExecuteUnauthenticatedRequest(uri, HttpMethod.Post, postData);

            var users    = JsonConvert.DeserializeObject<TwitterUser[]>(response);

            return users;
        }

        /// <summary>
        /// Access the profile image in various sizes for the user with the indicated screenName. 
        /// If no size is provided the normal image is returned.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="profileImageSize"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method should only be used by application developers to lookup or check the profile image URL 
        /// for a user. This method must not be used as the image source URL presented to users of your application.
        /// </remarks>
        public Uri RetrieveProfileImageUri(string screenName, ProfileImageSize profileImageSize = ProfileImageSize.Normal)
        {
            if (String.IsNullOrEmpty(screenName))
                throw new ArgumentException();

            var uri = new Uri(this.CommandBaseUri + String.Format("/profile_image/{0}.json?size={1}", 
                                                                   screenName, 
                                                                   profileImageSize.ToString().ToLowerInvariant()));

            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);
            var pattern     = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*";
            var match       = Regex.Match(response, pattern);
            if (!match.Success)
                throw new TwitterException("Unrecognized response.\n Server Response: \n" + response);
            
            return new Uri(match.Value);
        }

        /// <summary>
        /// Returns an array of users that the specified user can contribute to.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        public TwitterUser[] RetrieveContributees(string screenName, string userId = null, bool includeEntities = true,
                                                  bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("Either a userId or screenName is required for this method.");

            return this.RetrieveUsers(screenName, userId, includeEntities, skipStatus, "contributees");
        }

        /// <summary>
        /// Returns an array of users who can contribute to the specified account.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        public TwitterUser[] RetrieveContributors(string screenName, string userId = null, bool includeEntities = true,
                                                  bool skipStatus = false)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("Either a userId or screenName is required for this method.");

            return this.RetrieveUsers(screenName, userId, includeEntities, skipStatus, "contributors");
        }

        /// <summary>
        /// Runs a search for users similar to Find People button on Twitter.com.
        /// </summary>
        /// <param name="q">The user query to run against people user.</param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterUser[] Search(string q, int page = 1, int perPage = 10, bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(q))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uriBuilder      = new UriBuilder(this.CommandBaseUri + "/search.json");
            uriBuilder.Query    = String.Format("q={0}&page={1}&per_page={2}&include_entities={3}",
                                                q, page > 1 ? page : 1, perPage <= 20 ? perPage : 20,
                                                includeEntities ? "true" : "false");

            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Get, null);

            var users       = JsonConvert.DeserializeObject<TwitterUser[]>(response);

            return users;
        }

        /// <summary>
        /// Returns extended information of a given user, specified by ID or screen name as per the 
        /// required id parameter. The author's most recent status will be returned inline.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterUser Show(string screenName, string userId = null, bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("Either a userId or screenName is required for this method.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?include_entities={0}&", includeEntities ? "true" : "false");

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("user_id={0}", userId);

            var uri         = new Uri(this.CommandBaseUri + "/show.json" + queryBuilder.ToString().TrimEnd('&'));
            var response    = this.TwitterApi.Authenticated ?
                                  this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                                  this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var user        = TwitterObject.Parse<TwitterUser>(response);

            return user;
        }

        #endregion

        #region Private Methods

        private TwitterUser[] RetrieveUsers(
            string screenName,
            string userId,
            bool includeEntities,
            bool skipStatus,
            string userType)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("Either a userId or screenName is required for this method.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat(
                "?include_entities={0}&skip_status={1}&",
                includeEntities ? "true" : "false",
                skipStatus ? "true" : "false");

            if (!String.IsNullOrEmpty(screenName))
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            if (!String.IsNullOrEmpty(userId))
                queryBuilder.AppendFormat("user_id={0}", userId);

            var uri = new Uri(
                this.CommandBaseUri + String.Format(
                    "/{0}.json{1}",
                    userType,
                    queryBuilder.ToString().TrimEnd('&')));
            var response = this.TwitterApi.Authenticated
                               ? this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null)
                               : this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var users = JsonConvert.DeserializeObject<TwitterUser[]>(response);

            return users;
        }

        #endregion

    }
}