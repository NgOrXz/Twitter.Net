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

    public sealed class TwitterEntity : TwitterObject
    {
        #region Public Properties

        [TwitterKey("hashtags")]
        public TwitterHashTag[] HashTags { get; set; }

        [TwitterKey("media")]
        public TwitterMedia[] Media { get; set; }

        [TwitterKey("places")]
        public TwitterPlace[] Places { get; set; }

        [TwitterKey("urls")]
        public TwitterUrl[] Urls { get; set; }

        [TwitterKey("user_mentions")]
        public TwitterUserMention[] UserMentions { get; set; }

        #endregion


        #region Public Methods

        public static TwitterEntity FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterEntity>(dictionary);
        }

        public static TwitterEntity Parse(string jsonString)
        {
            return Parse<TwitterEntity>(jsonString);
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

                if (propertyInfo.PropertyType == typeof(TwitterHashTag[]))
                {
                    var arrList     = value as ArrayList;
                    var hashTags    = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterHashTag.FromDictionary(jsonObj)).ToArray();

                    propertyInfo.SetValue(this, hashTags, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterMedia[]))
                {
                    var arrList     = value as ArrayList;
                    var media       = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterMedia.FromDictionary(jsonObj)).ToArray();

                    propertyInfo.SetValue(this, media, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPlace[]))
                {
                    var arrList     = value as ArrayList;
                    var places      = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterPlace.FromDictionary(jsonObj)).ToArray();

                    propertyInfo.SetValue(this, places, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterUrl[]))
                {
                    var arrList     = value as ArrayList;
                    var urls        = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterUrl.FromDictionary(jsonObj)).ToArray();

                    propertyInfo.SetValue(this, urls, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterUserMention[]))
                {
                    var arrList     = value as ArrayList;
                    var userMetions = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterUserMention.FromDictionary(jsonObj)).ToArray();

                    propertyInfo.SetValue(this, userMetions, null);
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

                if (propertyInfo.PropertyType == typeof(TwitterHashTag[]))
                {
                    jsonBuilder.Append("[");
                    foreach (var tag in (TwitterHashTag[])value)
                    {
                        jsonBuilder.AppendFormat("{0},", tag.ToJsonString());
                    }
                    if (jsonBuilder[jsonBuilder.Length -1] == ',')
                        jsonBuilder.Length -= 1; // Remove trailing ',' char.

                    jsonBuilder.Append("],");
                }
                else if (propertyInfo.PropertyType == typeof(TwitterMedia[]))
                {
                    jsonBuilder.Append("[");
                    foreach (var media in (TwitterMedia[])value)
                    {
                        jsonBuilder.AppendFormat("{0},", media.ToJsonString());
                    }
                    if (jsonBuilder[jsonBuilder.Length - 1] == ',')
                        jsonBuilder.Length -= 1; // Remove trailing ',' char.

                    jsonBuilder.Append("],");
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPlace[]))
                {
                    jsonBuilder.Append("[");
                    foreach (var place in (TwitterPlace[])value)
                    {
                        jsonBuilder.AppendFormat("{0},", place.ToJsonString());
                    }
                    if (jsonBuilder[jsonBuilder.Length - 1] == ',')
                        jsonBuilder.Length -= 1; // Remove trailing ',' char.

                    jsonBuilder.Append("],");
                }
                else if (propertyInfo.PropertyType == typeof(TwitterUrl[]))
                {
                    jsonBuilder.Append("[");
                    foreach (var url in (TwitterUrl[])value)
                    {
                        jsonBuilder.AppendFormat("{0},", url.ToJsonString());
                    }
                    if (jsonBuilder[jsonBuilder.Length - 1] == ',')
                        jsonBuilder.Length -= 1; // Remove trailing ',' char.

                    jsonBuilder.Append("],");
                }
                else if (propertyInfo.PropertyType == typeof(TwitterUserMention[]))
                {
                    jsonBuilder.Append("[");
                    foreach (var mention in (TwitterUserMention[])value)
                    {
                        jsonBuilder.AppendFormat("{0},", mention.ToJsonString());
                    }
                    if (jsonBuilder[jsonBuilder.Length - 1] == ',')
                        jsonBuilder.Length -= 1; // Remove trailing ',' char.

                    jsonBuilder.Append("],");
                }
            }

            jsonBuilder.Length -= 1; // Remove trailing ',' char.
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}