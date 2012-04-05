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
    using System.Collections.Generic;
    using System.Net;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;


    public sealed class SpamReportingCommand : TwitterCommandBase
    {
        internal SpamReportingCommand(TwitterApi twitterApi)
            : base(twitterApi, "report_spam")
        {
        }


        /// <summary>
        /// The user specified in the id is blocked by the authenticated user and reported as a spammer. 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userId"></param>
        /// <returns>Returns the user that is reported as a spammer. If the specified user does not exist, returns null.</returns>
        public TwitterUser ReportSpam(string screenName, string userId = null)
        {
            if (String.IsNullOrEmpty(screenName) && String.IsNullOrEmpty(userId))
                throw new ArgumentException("A user id or screen name must be provided.");

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(userId))
                postData.Add("user_id", userId);
            if (!String.IsNullOrEmpty(screenName))
                postData.Add("screen_name", screenName);

            var uri = new Uri(this.CommandBaseUri + ".json");

            TwitterUser user = null;
            try
            {
                var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);
                user            = TwitterObject.Parse<TwitterUser>(response);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return user;
        }
    }
}
