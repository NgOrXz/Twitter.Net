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
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Mirai.Twitter.Core;

    internal sealed class TwitterTrendTopic
    {
        [TwitterKey("as_of")]
        public DateTime? AsOf { get; set; }

        [TwitterKey("created_at")]
        public DateTime? CreatedAt { get; set; }

        [TwitterKey("locations")]
        public TwitterTrendLocation[] Locations { get; set; }

        /// <summary>
        /// Gets the trends of the trend topic, or the trends of the first trend group if there are 
        /// multiple trend groups in the trend topic.
        /// </summary>
        public TwitterTrend[] Trends
        {
            get
            {
                var trends = !this.TrendGroups.Any() ?
                                new TwitterTrend[0] :
                                this.TrendGroups.First().ToArray();

                return trends;
            }
        }

        [TwitterKey("trends")]
        public TwitterTrendGroup[] TrendGroups { get; set; }


        public static TwitterTrendTopic FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var trendTopic = new TwitterTrendTopic();
            if (dictionary.Count == 0)
                return trendTopic;

            var pis = trendTopic.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(DateTime?))
                {
                    DateTime dateTime;
                    double seconds;
                    if (Double.TryParse(
                        value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds))
                    {
                        dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
                        propertyInfo.SetValue(trendTopic, dateTime, null);
                    }
                    else if (DateTime.TryParseExact(
                        value.ToString(),
                        "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite,
                        out dateTime))
                    {
                        propertyInfo.SetValue(trendTopic, dateTime, null);
                    }
                }
                else if (propertyInfo.PropertyType == typeof(TwitterTrendGroup[]))
                {
                    TwitterTrendGroup[] trendGroups = null;
                    if (value is ArrayList)
                    {
                        trendGroups = new[] 
                                {
                                    new TwitterTrendGroup(null, 
                                        (from Dictionary<string, object> trend in (ArrayList)value
                                         select TwitterTrend.FromDictonary(trend)).ToArray())
                                };
                    }
                    else // json object
                    {
                        trendGroups = (from jsonObj in (Dictionary<string, object>)value
                                       select new TwitterTrendGroup(DateTime.Parse(jsonObj.Key, CultureInfo.InvariantCulture),
                                                                    (from Dictionary<string, object> trend in (ArrayList)jsonObj.Value
                                                                     select TwitterTrend.FromDictonary(trend))))
                                       .OrderBy(t => t.Key)
                                       .ToArray();
                    }

                    propertyInfo.SetValue(trendTopic, trendGroups, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterTrendLocation[]))
                {
                    var locations = (from Dictionary<string, object> loc in (ArrayList)value
                                     select TwitterTrendLocation.FromDictionary(loc)).ToArray();

                    propertyInfo.SetValue(trendTopic, locations, null);
                }
            }

            return trendTopic;
        }
    }
}
