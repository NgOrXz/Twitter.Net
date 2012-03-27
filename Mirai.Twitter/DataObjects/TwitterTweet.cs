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

namespace Mirai.Twitter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Mirai.Twitter.Core;

    /// <summary>
    /// The twitter user.
    /// </summary>
    public sealed class TwitterTweet
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ids of the Contributors.
        /// </summary>
        [TwitterKey("contributors")]
        public string[] Contributors { get; set; }

        /// <summary>
        /// Gets or sets Coordinates.
        /// </summary>
        [TwitterKey("coordinates")]
        public TwitterCoordinate Coordinates { get; set; }

        /// <summary>
        /// Gets or sets CreatedAt.
        /// </summary>
        [TwitterKey("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets Entities.
        /// </summary>
        [TwitterKey("entities")]
        public TwitterEntity Entities { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether favorited.
        /// </summary>
        [TwitterKey("favorited")]
        public bool Favorited { get; set; }

        // This property ignored currently.
        // About geo see: http://www.geojson.org/geojson-spec.html and 
        // https://dev.twitter.com/docs/api/1/get/statuses/show/%3Aid
        [TwitterKey("geo")]
        public TwitterGeometry Geometry { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [TwitterKey("id_str")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets InReplyToScreenName.
        /// </summary>
        [TwitterKey("in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }

        /// <summary>
        /// Gets or sets InReplyToUserId.
        /// </summary>
        [TwitterKey("in_reply_to_user_id_str")]
        public string InReplyToUserId { get; set; }

        [TwitterKey("place")]
        public TwitterPlace Place { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether PossiblySensitive.
        /// </summary>
        [TwitterKey("possibly_sensitive")]
        public bool PossiblySensitive { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether PossiblySensitiveEditable.
        /// </summary>
        [TwitterKey("possibly_sensitive_editable")]
        public bool PossiblySensitiveEditable { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether Retweeted.
        /// </summary>
        [TwitterKey("retweeted")]
        public bool Retweeted { get; set; }

        /// <summary>
        /// Gets or sets RetweetedStatus.
        /// </summary>
        [TwitterKey("retweeted_status")]
        public TwitterTweet RetweetedStatus { get; set; }

        /// <summary>
        /// Gets or sets Source.
        /// Format: <a href="http://testtweet.codeplex.com" rel="nofollow">TestTweet2011</a>
        /// </summary>
        [TwitterKey("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets Text.
        /// Format: update with a picture~~~~~ http://t.co/ngUKvcT
        /// </summary>
        [TwitterKey("text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a typeValue indicating whether Truncated.
        /// </summary>
        [TwitterKey("truncated")]
        public bool Truncated { get; set; }

        /// <summary>
        /// Gets or sets User.
        /// </summary>
        [TwitterKey("user")]
        public TwitterUser User { get; set; }

        #endregion

        public static TwitterTweet FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var tweet = new TwitterTweet();
            if (dictionary.Count == 0)
                return tweet;

            var pis = tweet.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));
                
                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(String) || propertyInfo.PropertyType == typeof(Boolean))
                {
                    propertyInfo.SetValue(tweet, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    propertyInfo.SetValue(tweet, value.ToString().ToDateTime(), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterCoordinate))
                {
                    propertyInfo.SetValue(tweet, TwitterCoordinate.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPlace))
                {
                    propertyInfo.SetValue(tweet, TwitterPlace.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterUser))
                {
                    propertyInfo.SetValue(tweet, TwitterUser.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterEntity))
                {
                    propertyInfo.SetValue(tweet, TwitterEntity.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(string[]))
                {
                    var arrList = value as ArrayList;
                    var ids     = (from string id in arrList select id).ToArray();

                    propertyInfo.SetValue(tweet, ids, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterTweet))
                {
                    propertyInfo.SetValue(tweet, FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterGeometry))
                {
                    propertyInfo.SetValue(tweet, TwitterGeometry.Create(value as Dictionary<string, object>), null);
                }
            }

            return tweet;
        }
    }
}