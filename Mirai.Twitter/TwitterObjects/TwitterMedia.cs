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
    using System.Reflection;
    using System.Text;

    using Mirai.Twitter.Core;

    public sealed class TwitterMedia : TwitterObject
    {
        #region Public Properties

        [TwitterKey("display_url")]
        public string DisplayUrl { get; set; }

        [TwitterKey("expanded_url")]
        public Uri ExpanedUrl { get; set; }

        [TwitterKey("id_str")]
        public string Id { get; set; }

        [TwitterKey("indices")]
        public int[] Indices { get; set; }

        [TwitterKey("type")]
        public TwitterMediaType MediaType { get; set; }

        [TwitterKey("media_url")]
        public Uri MediaUrl { get; set; }

        [TwitterKey("media_url_https")]
        public Uri MediaUrlHttps { get; set; }

        [TwitterKey("sizes")]
        public TwitterSizes Sizes { get; set; }

        [TwitterKey("url")]
        public Uri Url { get; set; }

        #endregion



        #region Public Methods

        public static TwitterMedia FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterMedia>(dictionary);
        }

        public static TwitterMedia Parse(string jsonString)
        {
            return Parse<TwitterMedia>(jsonString);
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
                else if (propertyInfo.PropertyType == typeof(Uri))
                {
                    propertyInfo.SetValue(this, new Uri(value.ToString()), null);
                }
                else if (propertyInfo.PropertyType == typeof(int[]))
                {
                    var arrList = (ArrayList)value;
                    var indices = new int[arrList.Count];
                    for (var i = 0; i < arrList.Count; i++)
                        indices[i] = arrList[i].ToString().ToInt32();

                    propertyInfo.SetValue(this, indices, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterMediaType))
                {
                    TwitterMediaType mediaType;
                    Enum.TryParse(value.ToString(), true, out mediaType);

                    propertyInfo.SetValue(this, mediaType, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterSizes))
                {
                    propertyInfo.SetValue(this, TwitterSizes.FromDictionary(value as Dictionary<string, object>), null);
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
                else if (propertyInfo.PropertyType == typeof(Uri))
                    jsonBuilder.AppendFormat("\"{0}\",", value);
                else if (propertyInfo.PropertyType == typeof(TwitterMediaType))
                    jsonBuilder.AppendFormat("\"{0}\",", ((TwitterMediaType)value).ToString().ToLowerInvariant());
                else if (propertyInfo.PropertyType == typeof(int[]))
                {
                    jsonBuilder.Append("[");
                    foreach (var index in (int[])value)
                    {
                        jsonBuilder.AppendFormat("{0},", index);
                    }
                    if (jsonBuilder[jsonBuilder.Length -1] == ',')
                        jsonBuilder.Length -= 1; // Remove trailing ',' char.

                    jsonBuilder.Append("],");
                }
                else if (propertyInfo.PropertyType == typeof(TwitterSizes))
                    jsonBuilder.AppendFormat("{0},", ((TwitterSizes)value).ToJsonString());
            }

            jsonBuilder.Length -= 1; // Remove trailing ',' char.
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}