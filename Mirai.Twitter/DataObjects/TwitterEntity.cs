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

    public sealed class TwitterEntity
    {
        [TwitterKey("hashtags")]
        public List<TwitterHashTag> HashTags { get; set; }

        [TwitterKey("media")]
        public List<TwitterMedia> Media { get; set; }

        [TwitterKey("places")]
        public List<TwitterPlace> Places { get; set; }

        [TwitterKey("urls")]
        public List<TwitterUrl> Urls { get; set; }

        [TwitterKey("user_mentions")]
        public List<TwitterUserMention> UserMentions { get; set; }


        public static TwitterEntity FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var entity = new TwitterEntity();
            if (dictionary.Count == 0)
                return entity;

            var pis = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));
                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(List<TwitterHashTag>))
                {
                    var arrList     = value as ArrayList;
                    var hashTags    = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterHashTag.FromDictionary(jsonObj)).ToList();

                    propertyInfo.SetValue(entity, hashTags, null);
                }
                else if (propertyInfo.PropertyType == typeof(List<TwitterMedia>))
                {
                    var arrList     = value as ArrayList;
                    var media       = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterMedia.FromDictionary(jsonObj)).ToList();

                    propertyInfo.SetValue(entity, media, null);
                }
                else if (propertyInfo.PropertyType == typeof(List<TwitterPlace>))
                {
                    var arrList     = value as ArrayList;
                    var places      = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterPlace.FromDictionary(jsonObj)).ToList();

                    propertyInfo.SetValue(entity, places, null);
                }
                else if (propertyInfo.PropertyType == typeof(List<TwitterUrl>))
                {
                    var arrList     = value as ArrayList;
                    var urls        = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterUrl.FromDictionary(jsonObj)).ToList();

                    propertyInfo.SetValue(entity, urls, null);   
                }
                else if (propertyInfo.PropertyType == typeof(List<TwitterUserMention>))
                {
                    var arrList     = value as ArrayList;
                    var userMetions = (from Dictionary<string, object> jsonObj in arrList
                                       select TwitterUserMention.FromDictionary(jsonObj)).ToList();

                    propertyInfo.SetValue(entity, userMetions, null);
                }
            }

            return entity;
        }
    }
}