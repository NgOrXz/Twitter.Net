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

namespace Mirai.Twitter.Commands
{
    using System;
    using System.Reflection;
    using System.Text;

    using Mirai.Twitter.Core;

    public sealed class SearchCommandOptions
    {
        //[TwitterKey("callback")]
        //public string Callback { get; set; }

        [TwitterKey("geocode")]
        public TwitterGeoCode GeoCode { get; set; }

        [TwitterKey("include_entities")]
        public bool? IncludeEntities { get; set; }

        [TwitterKey("lang")]
        public string Language { get; set; }

        [TwitterKey("locale")]
        public string Locale { get; set; }

        [TwitterKey("max_id")]
        public string MaxId { get; set; }

        [TwitterKey("page")]
        public int? Page { get; set; }

        [TwitterKey("result_type")]
        public TwitterSearchReusltType? ReusltType { get; set; }

        [TwitterKey("show_user")]
        public bool? ShowUser { get; set; }

        [TwitterKey("since_id")]
        public string SinceId { get; set; }

        [TwitterKey("rpp")]
        public int? TweetsPerPage { get; set; }

        [TwitterKey("until")]
        public DateTime? Until { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            var pis = typeof(SearchCommandOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));
                
                if (twitterKey == null)
                    continue;

                var value = propertyInfo.GetValue(this, null);
                if (value != null)
                {
                    if (propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        sb.AppendFormat("{0}={1}&", twitterKey.Key, ((DateTime)value).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        sb.AppendFormat("{0}={1}&", twitterKey.Key, value.ToString().ToLowerInvariant());
                    }
                }
            }

            sb.Length -= 1;

            return sb.ToString();
        }
    }
}