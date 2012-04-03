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

    public sealed class TwitterUser : TwitterObject
    {
        #region Public Properties

        [TwitterKey("contributors_enabled")]
        public bool ContributorsEnabled { get; set; }

        [TwitterKey("created_at")]
        public DateTime CreatedAt { get; set; }

        [TwitterKey("description")]
        public string Description { get; set; }

        [TwitterKey("geo_enabled")]
        public bool GeoEnabled { get; set; }

        [TwitterKey("favorites_count")]
        public int FavoritesCount { get; set; }

        [TwitterKey("followers_count")]
        public int FollowersCount { get; set; }

        [TwitterKey("following")]
        public bool Following { get; set; }

        [TwitterKey("follow_request_sent")]
        public bool FollowRequestSent { get; set; }

        [TwitterKey("friends_count")]
        public int FriendsCount { get; set; }

        [TwitterKey("geo")]
        public TwitterPointGeometry Geometry { get; set; }

        [TwitterKey("id_str")]
        public string Id { get; set; }

        [TwitterKey("is_translator")]
        public bool IsTranslator { get; set; }

        [TwitterKey("lang")]
        public string Language { get; set; }

        [TwitterKey("listed_count")]
        public int ListedCount { get; set; }

        [TwitterKey("location")]
        public string Location { get; set; }

        [TwitterKey("name")]
        public string Name { get; set; }

        [TwitterKey("notifications")]
        public bool Notifications { get; set; }

        [TwitterKey("profile_background_color")]
        public TwitterColor ProfileBackgroundColor { get; set; }

        [TwitterKey("profile_background_image_url")]
        public Uri ProfileBackgroundImageUrl { get; set; }

        [TwitterKey("profile_background_tile")]
        public bool ProfileBackgroundTile { get; set; }

        [TwitterKey("profile_link_color")]
        public TwitterColor ProfileLinkColor { get; set; }

        [TwitterKey("profile_image_url")]
        public Uri ProfileImageUrl { get; set; }

        [TwitterKey("profile_sidebar_border_color")]
        public TwitterColor ProfileSidebarBorderColor { get; set; }

        [TwitterKey("profile_sidebar_fill_color")]
        public TwitterColor ProfileSidebarFillColor { get; set; }

        [TwitterKey("profile_text_color")]
        public TwitterColor ProfileTextColor { get; set; }

        [TwitterKey("profile_use_background_image")]
        public bool ProfileUseBackgroundImage { get; set; }

        [TwitterKey("protected")]
        public bool Protected { get; set; }

        [TwitterKey("screen_name")]
        public string ScreenName { get; set; }

        [TwitterKey("show_all_inline_media")]
        public bool ShowAllInlineMedia { get; set; }

        [TwitterKey("status")]
        public TwitterTweet Status { get; set; }

        [TwitterKey("statuses_count")]
        public int StatusesCount { get; set; }

        [TwitterKey("time_zone")]
        public string TimeZone { get; set; }

        [TwitterKey("url")]
        public Uri Url { get; set; }

        [TwitterKey("utc_offset")]
        public int UtcOffset { get; set; }

        [TwitterKey("verified")]
        public bool Verified { get; set; }

        #endregion



        #region Public Methods

        public static TwitterUser FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterUser>(dictionary);
        }

        public static TwitterUser Parse(string jsonString)
        {
            return Parse<TwitterUser>(jsonString);
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
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(this, value.ToString().ToInt32(), null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    propertyInfo.SetValue(this, value.ToString().ToDateTime(), null);
                }
                else if (propertyInfo.PropertyType == typeof(Uri))
                {
                    propertyInfo.SetValue(this, new Uri(value.ToString()), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterColor))
                {
                    propertyInfo.SetValue(this, TwitterColor.FromString(value.ToString()), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPointGeometry))
                {
                    propertyInfo.SetValue(this, TwitterPointGeometry.FromDictionary(value as Dictionary<string, object>), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterTweet))
                {
                    propertyInfo.SetValue(this, TwitterTweet.FromDictionary(value as Dictionary<string, object>), null);
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
                else if (propertyInfo.PropertyType == typeof(DateTime))
                    jsonBuilder.AppendFormat("\"{0}\",", ((DateTime)value).ToString("ddd MMM dd HH:mm:ss +0000 yyyy"));
                else if (propertyInfo.PropertyType == typeof(TwitterColor))
                    jsonBuilder.AppendFormat("\"{0}\",", ((TwitterColor)value));
                else if (propertyInfo.PropertyType == typeof(TwitterPointGeometry))
                    jsonBuilder.AppendFormat("{0},", ((TwitterPointGeometry)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(TwitterTweet))
                    jsonBuilder.AppendFormat("{0},", ((TwitterTweet)value).ToJsonString());
                else if (propertyInfo.PropertyType == typeof(Boolean))
                    jsonBuilder.AppendFormat("{0},", value.ToString().ToLowerInvariant());
                else if (propertyInfo.PropertyType == typeof(Int32))
                    jsonBuilder.AppendFormat("{0},", value);
            }

            jsonBuilder.Length -= 1; // Remove trailing ',' char.
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}
