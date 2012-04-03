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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Mirai.Twitter.Core;

    /// <summary>
    /// The twitter .
    /// </summary>
    public sealed class TwitterTweet : TwitterObject
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
        public TwitterPointGeometry Coordinates { get; set; }

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

        // About geo see: http://www.geojson.org/geojson-spec.html and 
        // https://dev.twitter.com/docs/api/1/get/statuses/show/%3Aid
        [TwitterKey("geo")]
        public TwitterPointGeometry Geometry { get; set; }

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
        /// </summary>
        [TwitterKey("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets Text.
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

        #region Public Methods

        public static TwitterTweet FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterTweet>(dictionary);
        }

        public static TwitterTweet Parse(string jsonString)
        {
            return Parse<TwitterTweet>(jsonString);
        }

        #endregion


        #region Overrides of TwitterObject

        internal override void Init(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (dictionary.Count == 0)
                return;

            var pis = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(String) || propertyInfo.PropertyType == typeof(Boolean))
                {
                    propertyInfo.SetValue(this, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    propertyInfo.SetValue(this, value.ToString().ToDateTime(), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPlace))
                {
                    propertyInfo.SetValue(this, TwitterPlace.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterUser))
                {
                    propertyInfo.SetValue(this, TwitterUser.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterEntity))
                {
                    propertyInfo.SetValue(this, TwitterEntity.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(string[]))
                {
                    var arrList = value as ArrayList;
                    var ids     = (from string id in arrList select id).ToArray();

                    propertyInfo.SetValue(this, ids, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterTweet))
                {
                    propertyInfo.SetValue(this, FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPointGeometry))
                {
                    var pointGeometry = TwitterPointGeometry.FromDictionary(value as Dictionary<string, object>);
                    if (twitterKey.Key == "coordinates")
                        pointGeometry.IsCoordinate = true; 

                    propertyInfo.SetValue(this, pointGeometry, null);
                }
            }
        }

        public override string ToJsonString()
        {
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            var pis = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || (value = propertyInfo.GetValue(this, null)) == null)
                    continue;

                jsonBuilder.AppendFormat("\"{0}\":", twitterKey.Key);

                if (propertyInfo.PropertyType == typeof(String))
                    jsonBuilder.AppendFormat("{0},", ((string)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(DateTime))
                    jsonBuilder.AppendFormat("\"{0}\",", ((DateTime)value).ToString("ddd MMM dd HH:mm:ss +0000 yyyy"));
                else if (propertyInfo.PropertyType == typeof(TwitterUser))
                    jsonBuilder.AppendFormat("{0},", ((TwitterUser)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(TwitterPlace))
                    jsonBuilder.AppendFormat("{0},", ((TwitterPlace)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(TwitterEntity))
                    jsonBuilder.AppendFormat("{0},", ((TwitterEntity)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(TwitterPointGeometry))
                    jsonBuilder.AppendFormat("{0},", ((TwitterPointGeometry)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(TwitterTweet))
                    jsonBuilder.AppendFormat("{0},", ((TwitterTweet)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(Boolean))
                    jsonBuilder.AppendFormat("{0},", value.ToString().ToLowerInvariant());
                else if (propertyInfo.PropertyType == typeof(String[]))
                {
                    jsonBuilder.AppendFormat("[{0}],", String.Join(",", (string[])value));
                }
            }

            jsonBuilder.Length -= 1; // Remove trailing ',' char.
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}