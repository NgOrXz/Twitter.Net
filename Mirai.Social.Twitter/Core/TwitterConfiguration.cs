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

namespace Mirai.Social.Twitter.Core
{
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    public sealed class TwitterConfiguration
    {
        #region Public Properties

        [JsonProperty("characters_reserved_per_media")]
        public int CharactersReservedPerMedia { get; internal set; }

        [JsonProperty("non_username_paths")]
        public string[] NonUsernamePaths { get; internal set; }

        [JsonProperty("max_media_per_upload")]
        public int MaxMediaPerUpload { get; internal set; }

        [JsonProperty("photo_size_limit")]
        public int PhotoSizeLimit { get; internal set; }

        [JsonProperty("photo_sizes")]
        public TwitterSizes PhotoSizes { get; internal set; }

        [JsonProperty("short_url_length")]
        public int ShortUrlLength { get; internal set; }

        [JsonProperty("short_url_length_https")]
        public int ShortUrlLengthHttps { get; internal set; }

        #endregion



        #region Constructors and Destructors

        internal TwitterConfiguration()
        {
            this.CharactersReservedPerMedia = 21;
            this.NonUsernamePaths = new[]
                {
                    "about", "account", "accounts", "activity", "all", "announcements", "anywhere",
                    "api_rules", "api_terms", "apirules", "apps", "auth", "badges", "blog", "business",
                    "buttons", "contacts", "devices", "direct_messages", "download", "downloads",
                    "edit_announcements", "faq", "favorites", "find_sources", "find_users", "followers",
                    "following", "friend_request", "friendrequest", "friends", "goodies", "help", "home",
                    "im_account", "inbox", "invitations", "invite", "jobs", "list", "login", "logout", "me",
                    "mentions", "messages", "mockview", "newtwitter", "notifications", "nudge", "oauth",
                    "phoenix_search", "positions", "privacy", "public_timeline", "related_tweets", "replies",
                    "retweeted_of_mine", "retweets", "retweets_by_others", "rules", "saved_searches", "search",
                    "sent", "settings", "share", "signup", "signin", "similar_to", "statistics", "terms", "tos",
                    "translate", "trends", "tweetbutton", "twttr", "update_discoverability", "users", "welcome",
                    "who_to_follow", "widgets", "zendesk_auth", "media_signup", "t1_qunit_tests", "phoenix_qunit_tests"
                };
            this.MaxMediaPerUpload = 1;
            this.PhotoSizeLimit = 3145728;
            this.PhotoSizes = new TwitterSizes
                {
                    Large = new TwitterSize { Width = 1024, Height = 2048, ResizeMode = TwitterMediaResizeMode.Fit },
                    Medium = new TwitterSize { Width = 600, Height = 1200, ResizeMode = TwitterMediaResizeMode.Fit },
                    Small = new TwitterSize { Width = 340, Height = 480, ResizeMode = TwitterMediaResizeMode.Fit },
                    Thumb = new TwitterSize { Width = 150, Height = 150, ResizeMode = TwitterMediaResizeMode.Crop }
                };
            this.ShortUrlLength = 20;
            this.ShortUrlLengthHttps = 21;
        }

        #endregion

    }
}