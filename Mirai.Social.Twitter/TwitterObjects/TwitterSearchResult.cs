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

    public sealed class TwitterSearchResult : TwitterObject
    {
        #region Public Properties

        [JsonProperty("created_at")]
        [JsonConverter(typeof(TwitterSearchReaultDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("geo")]
        public TwitterPointGeometry Geometry { get; set; }

        [JsonProperty("entities")]
        public TwitterEntity Entities { get; set; }

        [JsonProperty("from_user")]
        public string FromUser { get; set; }

        [JsonProperty("from_user_id_str")]
        public string FromUserId { get; set; }

        [JsonProperty("from_user_name")]
        public string FromUserName { get; set; }

        [JsonProperty("id_str")]
        public string Id { get; set; }

        [JsonProperty("iso_language_code")]
        public string IsoLanguageCode { get; set; }

        [JsonProperty("metadata")]
        public TwitterSearchResultMetadata Metadata { get; set; }

        [JsonProperty("profile_image_url")]
        public Uri ProfileImageUrl { get; set; }

        [JsonProperty("profile_image_url_https")]
        public Uri ProfileImageUrlHttps { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("to_uesr")]
        public string ToUser { get; set; }

        [JsonProperty("to_user_id_str")]
        public string ToUserId { get; set; }

        [JsonProperty("to_user_name")]
        public string ToUserName { get; set; }

        #endregion

    }
}