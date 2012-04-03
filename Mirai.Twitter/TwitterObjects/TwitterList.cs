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
    using System.Reflection;
    using System.Text;

    using Mirai.Twitter.Core;

    public sealed class TwitterList : TwitterObject
    {
        #region Public Properties

        [TwitterKey("created_at")]
        public DateTime? CreatedAt { get; set; }

        [TwitterKey("description")]
        public string Description { get; set; }

        [TwitterKey("following")]
        public bool Following { get; set; }

        [TwitterKey("full_name")]
        public string FullName { get; set; }

        [TwitterKey("id_str")]
        public string Id { get; set; }

        [TwitterKey("member_count")]
        public int MemberCount { get; set; }

        [TwitterKey("mode")]
        public TwitterListMode? Mode { get; set; }

        [TwitterKey("name")]
        public string Name { get; set; }

        [TwitterKey("slug")]
        public string Slug { get; set; }

        [TwitterKey("subscriber_count")]
        public int SubscriberCount { get; set; }

        [TwitterKey("uri")]
        public string Uri { get; set; }

        [TwitterKey("user")]
        public TwitterUser User { get; set; }

        #endregion



        #region Public Methods

        public static TwitterList FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterList>(dictionary);
        }

        public static TwitterList Parse(string jsonString)
        {
            return Parse<TwitterList>(jsonString);
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
                else if (propertyInfo.PropertyType == typeof(TwitterUser))
                {
                    propertyInfo.SetValue(this, TwitterUser.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(this, value.ToString().ToInt32(), null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime?))
                {
                    propertyInfo.SetValue(this, value.ToString().ToDateTime(), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterListMode?))
                {
                    TwitterListMode resultType;
                    if (Enum.TryParse(value.ToString(), true, out resultType))
                        propertyInfo.SetValue(this, resultType, null);
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
                else if (propertyInfo.PropertyType == typeof(Int32))
                    jsonBuilder.AppendFormat("{0},", value);
                else if (propertyInfo.PropertyType == typeof(DateTime?))
                    jsonBuilder.AppendFormat("\"{0}\",", ((DateTime)value).ToString("ddd MMM dd HH:mm:ss +0000 yyyy"));
                else if (propertyInfo.PropertyType == typeof(TwitterUser))
                    jsonBuilder.AppendFormat("{0},", ((TwitterUser)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(Boolean))
                    jsonBuilder.AppendFormat("{0},", value.ToString().ToLowerInvariant());
                else if (propertyInfo.PropertyType == typeof(TwitterListMode?))
                    jsonBuilder.AppendFormat("\"{0}\",", ((TwitterListMode)value).ToString().ToLowerInvariant());
            }

            jsonBuilder.Length -= 1; // Remove trailing ',' char.
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}
