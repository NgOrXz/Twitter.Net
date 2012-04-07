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

namespace Mirai.Social.Twitter.TwitterObjects
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The twitter .
    /// </summary>
    public sealed class TwitterTweet : TwitterObject
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ids of the Contributors.
        /// </summary>
        [JsonProperty("contributors")]
        public string[] Contributors { get; set; }

        /// <summary>
        /// Gets or sets Coordinates.
        /// </summary>
        [JsonProperty("coordinates")]
        public TwitterPointGeometry Coordinates { get; set; }

        /// <summary>
        /// Gets or sets CreatedAt.
        /// </summary>
        [JsonProperty("created_at")]
        [JsonConverter(typeof(TwitterDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets Entities.
        /// </summary>
        [JsonProperty("entities")]
        public TwitterEntity Entities { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether favorited.
        /// </summary>
        [JsonProperty("favorited")]
        public bool Favorited { get; set; }

        // About geo see: http://www.geojson.org/geojson-spec.html and 
        // https://dev.twitter.com/docs/api/1/get/statuses/show/%3Aid
        [JsonProperty("geo")]
        public TwitterPointGeometry Geometry { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [JsonProperty("id_str")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets InReplyToScreenName.
        /// </summary>
        [JsonProperty("in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }

        /// <summary>
        /// Gets or sets InReplyToUserId.
        /// </summary>
        [JsonProperty("in_reply_to_user_id_str")]
        public string InReplyToUserId { get; set; }

        [JsonProperty("place")]
        public TwitterPlace Place { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether PossiblySensitive.
        /// </summary>
        [JsonProperty("possibly_sensitive")]
        public bool PossiblySensitive { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether PossiblySensitiveEditable.
        /// </summary>
        [JsonProperty("possibly_sensitive_editable")]
        public bool PossiblySensitiveEditable { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether Retweeted.
        /// </summary>
        [JsonProperty("retweeted")]
        public bool Retweeted { get; set; }

        /// <summary>
        /// Gets or sets RetweetedStatus.
        /// </summary>
        [JsonProperty("retweeted_status")]
        public TwitterTweet RetweetedStatus { get; set; }

        /// <summary>
        /// Gets or sets Source.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether Truncated.
        /// </summary>
        [JsonProperty("truncated")]
        public bool Truncated { get; set; }

        /// <summary>
        /// Gets or sets User.
        /// </summary>
        [JsonProperty("user")]
        public TwitterUser User { get; set; }

        #endregion

    }
}