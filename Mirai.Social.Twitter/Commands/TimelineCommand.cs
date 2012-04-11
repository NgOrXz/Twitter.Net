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
    using System.Text;

    using Mirai.Net.OAuth;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    public sealed class TimelineCommand : TwitterCommandBase
    {
        #region Nested Members
        
        private enum TimelineType
        {
            HomeTimeline,
            Mentions,
            RetweetedByMe,
            RetweetedToMe,
            RetweetsOfMe,

            UserTimeline,
            RetweetedByUser,
            RetweetedToUser
        }

        #endregion

        #region Constructors and Destructors

        internal TimelineCommand(TwitterApi twitterApi)
            : base(twitterApi, "statuses")
        {
        }

        #endregion

        #region Public Methods

        public TwitterTweet[] RetweetedByMe(int count = 20, string sinceId = null, string maxId = null,
                                            int page = 1, bool trimUser = true, bool includeRetweets = true,
                                            bool includeEntities = true, bool excludeReplies = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            return this.RetrieveTimeline(TimelineType.RetweetedByMe, null, null, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies);
        }

        public TwitterTweet[] RetweetedToMe(int count = 20, string sinceId = null, string maxId = null,
                                            int page = 1, bool trimUser = true, bool includeRetweets = true,
                                            bool includeEntities = true, bool excludeReplies = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            return this.RetrieveTimeline(TimelineType.RetweetedToMe, null, null, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies);
        }

        public TwitterTweet[] RetweetsOfMe(int count = 20, string sinceId = null, string maxId = null,
                                           int page = 1, bool trimUser = true, bool includeRetweets = true,
                                           bool includeEntities = true, bool excludeReplies = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            return this.RetrieveTimeline(TimelineType.RetweetsOfMe, null, null, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies);
        }

        public TwitterTweet[] RetweetedByUser(string userId = null, string screenName = null, int count = 20,
                                              string sinceId = null, string maxId = null,
                                              int page = 1, bool trimUser = true, bool includeRetweets = true,
                                              bool includeEntities = true, bool excludeReplies = true)
        {
            return this.RetrieveTimeline(TimelineType.RetweetedByUser, userId, screenName, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies);
        }

        public TwitterTweet[] RetweetedToUser(string userId = null, string screenName = null, int count = 20,
                                              string sinceId = null, string maxId = null,
                                              int page = 1, bool trimUser = true, bool includeRetweets = true,
                                              bool includeEntities = true, bool excludeReplies = true)
        {
            return this.RetrieveTimeline(TimelineType.RetweetedToUser, userId, screenName, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies);
        }

        /// <summary>
        /// Returns the most recent statuses, including retweets if they exist, posted by the authenticating user 
        /// and the user's they follow. This is the same timeline seen by a user when they login to twitter.com.
        /// </summary>
        /// <param name="count">
        /// Specifies the number of records to retrieve. Must be less than or equal to 200. Defaults to 20.
        /// </param>
        /// <param name="sinceId"></param>
        /// <param name="maxId"></param>
        /// <param name="page"></param>
        /// <param name="trimUser"></param>
        /// <param name="includeRetweets"></param>
        /// <param name="includeEntities"></param>
        /// <param name="excludeReplies"></param>
        /// <param name="contributorDetails"></param>
        /// <returns></returns>
        public TwitterTweet[] RetrieveHomeTimeline(int count = 20, string sinceId = null, string maxId = null,
                                                   int page = 1, bool trimUser = true, bool includeRetweets = true,
                                                   bool includeEntities = true, bool excludeReplies = true,
                                                   bool contributorDetails = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            return this.RetrieveTimeline(TimelineType.HomeTimeline, null, null, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies,
                                         contributorDetails);
        }

        public TwitterTweet[] RetrieveMentions(int count = 20, string sinceId = null, string maxId = null,
                                               int page = 1, bool trimUser = true, bool includeRetweets = true,
                                               bool includeEntities = true, bool excludeReplies = true,
                                               bool contributorDetails = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            return this.RetrieveTimeline(TimelineType.Mentions, null, null, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies,
                                         contributorDetails);
        }

        /// <summary>
        /// Returns the 20 most recent statuses posted by the authenticating user. It is also possible to 
        /// request another user's timeline by using the screen_name or user_id parameter. The other users 
        /// timeline will only be visible if they are not protected, or if the authenticating user's follow request 
        /// was accepted by the protected user.
        /// The timeline returned is the equivalent of the one seen when you view a user's profile on twitter.com.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="screenName"></param>
        /// <param name="count"></param>
        /// <param name="sinceId"></param>
        /// <param name="maxId"></param>
        /// <param name="page"></param>
        /// <param name="trimUser"></param>
        /// <param name="includeRetweets"></param>
        /// <param name="includeEntities"></param>
        /// <param name="excludeReplies"></param>
        /// <param name="contributorDetails"></param>
        /// <returns></returns>
        public TwitterTweet[] RetrieveUserTimeline(string userId = null, string screenName = null, int count = 20, 
                                                   string sinceId = null, string maxId = null,
                                                   int page = 1, bool trimUser = true, bool includeRetweets = true,
                                                   bool includeEntities = true, bool excludeReplies = true,
                                                   bool contributorDetails = true)
        {
            return this.RetrieveTimeline(TimelineType.UserTimeline, userId, screenName, count, sinceId, maxId,
                                         page, trimUser, includeRetweets, includeEntities, excludeReplies,
                                         contributorDetails);
        }

        #endregion

        #region Private Methods

        private TwitterTweet[] RetrieveTimeline(
            TimelineType timelineType,
            string userId = null,
            string screenName = null,
            int count = 20,
            string sinceId = null,
            string maxId = null,
            int page = 1,
            bool trimUser = true,
            bool includeRetweets = true,
            bool includeEntities = true,
            bool excludeReplies = true,
            bool contributorDetails = true)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat(
                "?count={0}&page={1}&trim_user={2}&include_rts={3}&include_entities={4}&" +
                "exclude_replies={5}&",
                count <= 200 ? count : 200,
                page > 1 ? page : 1,
                trimUser ? "true" : "false",
                includeRetweets ? "true" : "false",
                includeEntities ? "true" : "false",
                excludeReplies ? "true" : "false");

            if (timelineType == TimelineType.HomeTimeline ||
                timelineType == TimelineType.Mentions ||
                timelineType == TimelineType.UserTimeline)
            {
                queryBuilder.AppendFormat("contributor_details={0}&", contributorDetails ? "true" : "false");
            }

            if (!String.IsNullOrEmpty(userId))
            {
                switch (timelineType)
                {
                    case TimelineType.UserTimeline:
                        queryBuilder.AppendFormat("user_id={0}&", userId);
                        break;
                    case TimelineType.RetweetedToUser:
                    case TimelineType.RetweetedByUser:
                        queryBuilder.AppendFormat("id={0}&", userId);
                        break;
                }
            }

            if (!String.IsNullOrEmpty(screenName) &&
                (timelineType == TimelineType.UserTimeline ||
                 timelineType == TimelineType.RetweetedByUser ||
                 timelineType == TimelineType.RetweetedToUser))
            {
                queryBuilder.AppendFormat("screen_name={0}&", screenName);
            }

            if (!String.IsNullOrEmpty(sinceId))
                queryBuilder.AppendFormat("sincd_id={0}&", sinceId);
            if (!String.IsNullOrEmpty(maxId))
                queryBuilder.AppendFormat("max_id={0}", maxId);

            var twitterMethod = String.Empty;
            switch (timelineType)
            {
                case TimelineType.HomeTimeline:
                    twitterMethod = "/home_timeline.json";
                    break;
                case TimelineType.Mentions:
                    twitterMethod = "/mentions.json";
                    break;
                case TimelineType.RetweetedByMe:
                    twitterMethod = "/retweeted_by_me.json";
                    break;
                case TimelineType.RetweetedToMe:
                    twitterMethod = "/retweeted_to_me.json";
                    break;
                case TimelineType.RetweetsOfMe:
                    twitterMethod = "/retweets_of_me.json";
                    break;
                case TimelineType.UserTimeline:
                    twitterMethod = "/user_timeline.json";
                    break;
                case TimelineType.RetweetedToUser:
                    twitterMethod = "/retweeted_to_user.json";
                    break;
                case TimelineType.RetweetedByUser:
                    twitterMethod = "/retweeted_by_user.json";
                    break;
            }
            var uri = new Uri(this.CommandBaseUri + twitterMethod + queryBuilder.ToString().TrimEnd('&'));
            var response = this.TwitterApi.Authenticated
                               ? this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null)
                               : this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var tweets   = JsonConvert.DeserializeObject<TwitterTweet[]>(response);

            return tweets;
        }

        #endregion

    }
}