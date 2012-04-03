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

    public sealed class TwitterSearch : TwitterObject
    {
        #region Public Properties

        [TwitterKey("completed_in")]
        public double CompletedIn { get; set; }

        [TwitterKey("max_id_str")]
        public string MaxId { get; set; }

        [TwitterKey("next_page")]
        public string NextPage { get; set; }

        [TwitterKey("page")]
        public int Page { get; set; }

        [TwitterKey("query")]
        public string Query { get; set; }

        [TwitterKey("refresh_url")]
        public string RefreshUrl { get; set; }

        [TwitterKey("results")]
        public TwitterSearchResult[] Results { get; set; }

        [TwitterKey("results_per_page")]
        public int ResultsPerPage { get; set; }

        [TwitterKey("since_id")]
        public string SinceId { get; set; }

        #endregion



        #region Public Methods

        public static TwitterSearch FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterSearch>(dictionary);
        }

        public static TwitterSearch Parse(string jsonString)
        {
            return Parse<TwitterSearch>(jsonString);
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
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(this, value.ToString().ToInt32(), null);
                }
                else if (propertyInfo.PropertyType == typeof(double))
                {
                    propertyInfo.SetValue(this, value.ToString().ToDouble(), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterSearchResult[]))
                {
                    var jsonArray   = (ArrayList)value;
                    var results     = (from Dictionary<string, object> result in jsonArray
                                       select TwitterSearchResult.FromDictionary(result)).ToArray();

                    propertyInfo.SetValue(this, results, null);
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
                else if (propertyInfo.PropertyType == typeof(int) && propertyInfo.PropertyType == typeof(double))
                    jsonBuilder.AppendFormat("{0},", value);
                else if (propertyInfo.PropertyType == typeof(TwitterSearchResult[]))
                {
                    jsonBuilder.Append("[");
                    foreach (var result in (TwitterSearchResult[])value)
                    {
                        jsonBuilder.AppendFormat("{0},", result.ToJsonString());
                    }
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
