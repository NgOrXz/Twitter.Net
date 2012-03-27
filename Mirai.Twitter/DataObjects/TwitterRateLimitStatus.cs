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

    public abstract class TwitterRateLimitStatusBase
    {
        [TwitterKey("remaining_hits")]
        public int RemainingHits { get; internal set; }

        [TwitterKey("reset_time")]
        public DateTime ResetTime { get; internal set; }

        [TwitterKey("reset_time_in_seconds")]
        public int ResetTimeInSeconds { get; internal set; }
    }


    public sealed class TwitterRateLimitStatus : TwitterRateLimitStatusBase
    {
        [TwitterKey("photos")]
        public TwitterPhotoRateLimitStatus PhotoRateLimitStatus { get; internal set; }

        [TwitterKey("hourly_limit")]
        public int HourlyLimit { get; internal set; }


        public static TwitterRateLimitStatus FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var rateLimit = new TwitterRateLimitStatus();
            if (dictionary.Count == 0)
                return rateLimit;

            var pis = rateLimit.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(
                                                                                    propertyInfo,
                                                                                    typeof(TwitterKeyAttribute));
                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(rateLimit, value.ToString().ToInt32(), null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    propertyInfo.SetValue(rateLimit, value.ToString().ToDateTime(), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterPhotoRateLimitStatus))
                {
                    propertyInfo.SetValue(rateLimit, 
                        TwitterPhotoRateLimitStatus.FromDictionary(value as Dictionary<string, object>), null);
                }
            }

            return rateLimit;
        }
    }


    public sealed class TwitterPhotoRateLimitStatus : TwitterRateLimitStatusBase
    {
        [TwitterKey("daily_limit")]
        public int DailyLimit { get; internal set; }


        public static TwitterPhotoRateLimitStatus FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var photoRateLimit = new TwitterPhotoRateLimitStatus();
            if (dictionary.Count == 0)
                return photoRateLimit;

            var pis = photoRateLimit.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(
                                                                                    propertyInfo,
                                                                                    typeof(TwitterKeyAttribute));
                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(photoRateLimit, value.ToString().ToInt32(), null);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    propertyInfo.SetValue(photoRateLimit, value.ToString().ToDateTime(), null);
                }
            }

            return photoRateLimit;
        }
    }
}