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
    using System.Collections.Generic;
    using System.Reflection;

    using Mirai.Twitter.Core;

    public sealed class TwitterRelationshipUser
    {
        [TwitterKey("all_replies")]
        public bool? AllReplies { get; set; }

        [TwitterKey("blocking")]
        public bool? Blocking { get; set; }

        [TwitterKey("can_dm")]
        public bool? CanDM { get; set; }

        [TwitterKey("followed_by")]
        public bool FollowedBy { get; set; }

        [TwitterKey("following")]
        public bool Following { get; set; }

        [TwitterKey("id_str")]
        public string Id { get; set; }

        [TwitterKey("marked_spam")]
        public bool? MarkedSpam { get; set; }

        [TwitterKey("notifications_enabled")]
        public bool? NotificationsEnabled { get; set; }

        [TwitterKey("screen_name")]
        public string ScreenName { get; set; }

        [TwitterKey("want_retweets")]
        public bool? WantRetweets { get; set; }


        public static TwitterRelationshipUser FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var relationshipUser = new TwitterRelationshipUser();
            if (dictionary.Count == 0)
                return relationshipUser;

            var pis = relationshipUser.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));
                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(String))
                {
                    propertyInfo.SetValue(relationshipUser, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(Boolean) || propertyInfo.PropertyType == typeof(Boolean?))
                {
                    propertyInfo.SetValue(relationshipUser, value, null);
                }
            }

            return relationshipUser;
        }
    }
}