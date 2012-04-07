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

    using Mirai.Net.OAuth;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;


    /// <summary>
    /// Controls SMS-based notifications that a dm wants to receive.
    /// </summary>
    public sealed class NotificationCommand :  TwitterCommandBase
    {
        #region Constructors and Destructors

        internal NotificationCommand(TwitterApi twitterApi)
            : base(twitterApi, "notifications")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables device notifications for updates from the specified dm. Returns the specified dm when successful. 
        /// </summary>
        /// <param name="screenName">The dm's screen name.</param>
        /// <returns></returns>
        public TwitterUser FollowByScreenName(string screenName)
        {
            return this.Follow("screen_name", screenName);
        }

        /// <summary>
        /// Enables device notifications for updates from the specified dm. Returns the specified dm when successful. 
        /// </summary>
        /// <param name="userId">The dm's id.</param>
        /// <returns></returns>
        public TwitterUser FollowByUserId(string userId)
        {
            return this.Follow("user_id", userId);
        }

        /// <summary>
        /// Disables notifications for updates from the specified dm to the authenticating dm. 
        /// Returns the specified dm when successful. 
        /// </summary>
        /// <param name="screenName">The dm's screen name.</param>
        /// <returns></returns>
        public TwitterUser LeaveByScreenName(string screenName)
        {
            return this.Leave("screen_name", screenName);
        }

        /// <summary>
        /// Disables notifications for updates from the specified dm to the authenticating dm. 
        /// Returns the specified dm when successful. 
        /// </summary>
        /// <param name="userId">The dm's id.</param>
        /// <returns></returns>
        public TwitterUser LeaveByUserId(string userId)
        {
            return this.Leave("user_id", userId);
        }

        #endregion

        private TwitterUser Follow(string twitterKey, string value)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (String.IsNullOrEmpty(twitterKey) || String.IsNullOrEmpty(value))
                throw new ArgumentException();

            var postData = new Dictionary<string, string>
                {
                    { twitterKey, value }
                };

            var uri         = new Uri(this.CommandBaseUri + "/follow.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var twitterUser = TwitterObject.Parse<TwitterUser>(response);

            return twitterUser;
        }

        private TwitterUser Leave(string twitterKey, string value)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (String.IsNullOrEmpty(twitterKey) || String.IsNullOrEmpty(value))
                throw new ArgumentException();

            var postData = new Dictionary<string, string>
                {
                    { twitterKey, value }
                };

            var uri         = new Uri(this.CommandBaseUri + "/leave.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var twitterUser = TwitterObject.Parse<TwitterUser>(response);

            return twitterUser;
        }
    }
}
