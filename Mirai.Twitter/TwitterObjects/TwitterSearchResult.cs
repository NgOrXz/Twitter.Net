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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Mirai.Twitter.Core;

    public sealed class TwitterSearchResult : TwitterObject
    {
        #region Public Properties

        [TwitterKey("created_at")]
        public DateTime CreatedAt { get; set; }

        [TwitterKey("geo")]
        public TwitterPointGeometry Geometry { get; set; }

        [TwitterKey("entities")]
        public TwitterEntity Entities { get; set; }

        [TwitterKey("from_user")]
        public string FromUser { get; set; }

        [TwitterKey("from_user_id_str")]
        public string FromUserId { get; set; }

        [TwitterKey("from_user_id")]
        public string FromUserName { get; set; }

        [TwitterKey("id_str")]
        public string Id { get; set; }

        [TwitterKey("iso_language_code")]
        public string IsoLanguageCode { get; set; }

        [TwitterKey("metadata")]
        public TwitterSearchResultMetadata Metadata { get; set; }

        [TwitterKey("profile_image_url")]
        public Uri ProfileImageUrl { get; set; }

        [TwitterKey("profile_image_url_https")]
        public Uri ProfileImageUrlHttps { get; set; }

        [TwitterKey("source")]
        public string Source { get; set; }

        [TwitterKey("text")]
        public string Text { get; set; }

        [TwitterKey("to_uesr")]
        public string ToUser { get; set; }

        [TwitterKey("to_user_id_str")]
        public string ToUserId { get; set; }

        [TwitterKey("to_user_name")]
        public string ToUserName { get; set; }

        #endregion



        #region Public Methods

        public static TwitterSearchResult FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterSearchResult>(dictionary);
        }

        public static TwitterSearchResult Parse(string jsonString)
        {
            return Parse<TwitterSearchResult>(jsonString);
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

                if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(this, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    DateTime dt;
                    DateTime.TryParseExact(value.ToString(),
                                           "ddd, dd MMM yyyy HH:mm:ss +0000",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite,
                                           out dt);

                    propertyInfo.SetValue(this, dt, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterEntity))
                {
                    propertyInfo.SetValue(this, TwitterEntity.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPointGeometry))
                {
                    propertyInfo.SetValue(this, TwitterPointGeometry.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(Uri))
                {
                    propertyInfo.SetValue(this, new Uri(value.ToString()), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterSearchResultMetadata))
                {
                    propertyInfo.SetValue(this,
                        TwitterSearchResultMetadata.FromDictionary(value as Dictionary<string, object>), null);
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

                if (propertyInfo.PropertyType == typeof(string))
                    jsonBuilder.AppendFormat("{0},", ((string)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(Uri))
                    jsonBuilder.AppendFormat("\"{0}\",", value);
                else if (propertyInfo.PropertyType == typeof(DateTime))
                    jsonBuilder.AppendFormat("\"{0}\",", ((DateTime)value).ToString("ddd, dd MMM yyyy HH:mm:ss +0000"));
                else if (propertyInfo.PropertyType == typeof(TwitterEntity))
                    jsonBuilder.AppendFormat("{0},", ((TwitterEntity)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(TwitterPointGeometry))
                    jsonBuilder.AppendFormat("{0},", ((TwitterPointGeometry)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(TwitterSearchResultMetadata))
                    jsonBuilder.AppendFormat("{0},", ((TwitterSearchResultMetadata)value).ToJsonString());
            }

            jsonBuilder.Length -= 1; // Remove trailing ',' char.
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}