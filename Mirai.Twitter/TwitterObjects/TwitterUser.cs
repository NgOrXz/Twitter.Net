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

namespace Mirai.Twitter.TwitterObjects
{
    using System;

    using Newtonsoft.Json;

    public sealed class TwitterUser : TwitterObject
    {
        #region Public Properties

        [JsonProperty("contributors_enabled")]
        public bool ContributorsEnabled { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(TwitterDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("geo_enabled")]
        public bool GeoEnabled { get; set; }

        [JsonProperty("favorites_count")]
        public int? FavoritesCount { get; set; }

        [JsonProperty("followers_count")]
        public int? FollowersCount { get; set; }

        [JsonProperty("following")]
        public bool Following { get; set; }

        [JsonProperty("follow_request_sent")]
        public bool FollowRequestSent { get; set; }

        [JsonProperty("friends_count")]
        public int FriendsCount { get; set; }

        [JsonProperty("geo")]
        public TwitterPointGeometry Geometry { get; set; }

        [JsonProperty("id_str")]
        public string Id { get; set; }

        [JsonProperty("is_translator")]
        public bool IsTranslator { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("listed_count")]
        public int? ListedCount { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notifications")]
        public bool Notifications { get; set; }

        [JsonProperty("profile_background_color")]
        [JsonConverter(typeof(TwitterColorConverter))]
        public TwitterColor ProfileBackgroundColor { get; set; }

        [JsonProperty("profile_background_image_url")]
        public Uri ProfileBackgroundImageUrl { get; set; }

        [JsonProperty("profile_background_tile")]
        public bool ProfileBackgroundTile { get; set; }

        [JsonProperty("profile_link_color")]
        [JsonConverter(typeof(TwitterColorConverter))]
        public TwitterColor ProfileLinkColor { get; set; }

        [JsonProperty("profile_image_url")]
        public Uri ProfileImageUrl { get; set; }

        [JsonProperty("profile_sidebar_border_color")]
        [JsonConverter(typeof(TwitterColorConverter))]
        public TwitterColor ProfileSidebarBorderColor { get; set; }

        [JsonProperty("profile_sidebar_fill_color")]
        [JsonConverter(typeof(TwitterColorConverter))]
        public TwitterColor ProfileSidebarFillColor { get; set; }

        [JsonProperty("profile_text_color")]
        [JsonConverter(typeof(TwitterColorConverter))]
        public TwitterColor ProfileTextColor { get; set; }

        [JsonProperty("profile_use_background_image")]
        public bool ProfileUseBackgroundImage { get; set; }

        [JsonProperty("protected")]
        public bool Protected { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("show_all_inline_media")]
        public bool ShowAllInlineMedia { get; set; }

        [JsonProperty("status")]
        public TwitterTweet Status { get; set; }

        [JsonProperty("statuses_count")]
        public int? StatusesCount { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("utc_offset")]
        public int? UtcOffset { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        #endregion

    }
}
