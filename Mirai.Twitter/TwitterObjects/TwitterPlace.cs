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

    using Mirai.Twitter.Core;

    public sealed class TwitterPlace
    {
        [TwitterKey("attributes")]
        public TwitterPlaceAttributes Attributes { get; set; }

        [TwitterKey("contained_within")]
        public TwitterPlace[] ContainedWithin { get; set; }

        [TwitterKey("country")]
        public string Country { get; set; }

        [TwitterKey("country_code")]
        public string CountryCode { get; set; }

        [TwitterKey("full_name")]
        public string FullName { get; set; }

        [TwitterKey("id")]
        public string Id { get; set; }

        [TwitterKey("name")]
        public string Name { get; set; }

        [TwitterKey("place_type")]
        public TwitterPlaceType? PlaceType { get; set; }

        [TwitterKey("url")]
        public Uri Url { get; set; }


        public static TwitterPlace FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var twitterPlace = new TwitterPlace();
            if (dictionary.Count == 0)
                return twitterPlace;

            var pis = twitterPlace.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(String))
                {
                    propertyInfo.SetValue(twitterPlace, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(Uri))
                {
                    propertyInfo.SetValue(twitterPlace, new Uri(value.ToString()), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPlaceType?))
                {
                    TwitterPlaceType placeType;
                    if (Enum.TryParse(value.ToString(), true, out placeType))
                        propertyInfo.SetValue(twitterPlace, placeType, null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPlaceAttributes))
                {
                    propertyInfo.SetValue(twitterPlace, 
                        TwitterPlaceAttributes.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPlace[]))
                {
                    var jsonArray   = (ArrayList)value;
                    var places      = (from Dictionary<string, object> place in jsonArray
                                       select TwitterPlace.FromDictionary(place)).ToArray();

                    propertyInfo.SetValue(twitterPlace, places, null);
                }
            }

            return twitterPlace;
        }
    }
}