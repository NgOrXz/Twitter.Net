﻿// ------------------------------------------------------------------------------------------------------
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

namespace Mirai.Social.Twitter.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Mirai.Net.OAuth;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Account-level configuration settings for users.
    /// </summary>
    public sealed class AccountCommand : TwitterCommandBase
    {
        #region Constants and Fields

        public static readonly int ProfileBackgroundImageSize   = 800 * 1024;
        public static readonly int ProfileImageSize             = 700 * 1024;

        #endregion

        #region Constructors and Destructors

        internal AccountCommand(TwitterApi twitterApi)
            : base(twitterApi, "account")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ends the session of the authenticating user, returning a null cookie. Use this method 
        /// to sign users out of client-facing applications like widgets. 
        /// </summary>
        public void EndSession()
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri = new Uri(this.CommandBaseUri + "/end_session.json");
            this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, null);
        }

        public TwitterAccountSettings RetrieveAccountSettings()
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + "/settings.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var settings    = TwitterObject.Parse<TwitterAccountSettings>(response);

            return settings;
        }

        /// <summary>
        /// Returns the remaining number of API requests available to the requesting user before the API limit 
        /// is reached for the current hour. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// When calling this method without authentication you will receive the rate limit for the IP you 
        /// are requesting from. To receive the rate limit for the authenticating user you must make the request 
        /// authenticated.
        /// </remarks>
        public TwitterRateLimitStatus RetrieveRateLimitStatus()
        {
            var uri         = new Uri(this.CommandBaseUri + "/rate_limit_status.json");
            var response    = this.TwitterApi.Authenticated ?
                                this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null) :
                                this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

            var rateLimit   = JsonConvert.DeserializeObject<TwitterRateLimitStatus>(response);

            return rateLimit;
        }

        /// <summary>
        /// Updates the authenticating user's settings.
        /// </summary>
        /// <param name="trendLocationId"></param>
        /// <param name="sleepTimeEnabled"></param>
        /// <param name="startSleepTime"></param>
        /// <param name="endSleepTime"></param>
        /// <param name="timeZone">The time zone dates and times should be displayed in for the user.</param>
        /// <param name="language">
        /// The language which Twitter should render in for this user. The language must be specified by 
        /// the appropriate two letter ISO 639-1 representation.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// While all parameters for this method are optional, at least one or more should be provided when executing 
        /// this request.
        /// </remarks>
        public TwitterAccountSettings UpdateAccountSettings(string trendLocationId = null, bool? sleepTimeEnabled = null,
                                                            string startSleepTime = null, string endSleepTime = null,
                                                            string timeZone = null, string language = null)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>();

            if (!String.IsNullOrWhiteSpace(trendLocationId))
                postData.Add("trend_location_woeid", trendLocationId);
            if (sleepTimeEnabled.HasValue)
                postData.Add("sleep_time_enabled", sleepTimeEnabled.Value ? "true" : "false");
            if (!String.IsNullOrWhiteSpace(startSleepTime))
                postData.Add("start_sleep_time", startSleepTime);
            if (!String.IsNullOrWhiteSpace(endSleepTime))
                postData.Add("end_sleep_time", endSleepTime);
            if (!String.IsNullOrWhiteSpace(timeZone))
                postData.Add("time_zone", timeZone);
            if (!String.IsNullOrWhiteSpace(language))
                postData.Add("lang", language);

            if (postData.Count == 0)
                throw new ArgumentException("While all parameters for this method are optional, " + 
                                            "at least one or more should be provided when executing this request.");

            var uri         = new Uri(this.CommandBaseUri + "/settings.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var settings    = TwitterObject.Parse<TwitterAccountSettings>(response);

            return settings;
        }

        /// <summary>
        /// Sets values that users are able to set under the "Account" tab of their settings page. 
        /// Only the parameters specified will be updated. 
        /// </summary>
        /// <param name="name">Full name associated with the profile. Maximum of 20 characters.</param>
        /// <param name="url">URL associated with the profile.</param>
        /// <param name="location">The city or country describing where the user of the account is located. </param>
        /// <param name="description">A description of the user owning the account. Maximum of 160 characters.</param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns>Returns updated user if successful. Otherwise, null.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TwitterException"></exception>
        public TwitterUser UpdateProfile(string name = null, Uri url = null, string location = null,
                                         string description = null, bool includeEntities = true, bool skipStatus = false)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (url != null)
            {
                if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
                    throw new ArgumentException("Invalid uri.", "url");
            }

            var postData = new Dictionary<string, string>
                {
                    { "include_entities", includeEntities ? "true" : "false" },
                    { "skip_status", skipStatus ? "true" : "false" }
                };

            if (name != null)
                postData.Add("name", name);
            if (url != null)
                postData.Add("url", url.ToString());
            if (location != null)
                postData.Add("location", location);
            if (description != null)
                postData.Add("description", description);

            var uri = new Uri(this.CommandBaseUri + "/update_profile.json");

            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);
            var twitterUser = TwitterObject.Parse<TwitterUser>(response);

            return twitterUser;
        }

        /// <summary>
        /// Updates the authenticating user's profile background image. This method can also be used 
        /// to enable or disable the profile background image.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tile">Whether or not to tile the background image. </param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <param name="use">Determines whether to display the profile background image or not. </param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TwitterException"></exception>
        public TwitterUser UpdateProfileBackgroundImage(string fileName = null, bool tile = false, 
                                                        bool includeEntities = true, 
                                                        bool skipStatus = false, bool use = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var imageType = new[] { ".GIF", ".JPG", ".JPEG", ".PNG" };
            if (fileName != null && !imageType.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid image format.", "fileName");

            var postData = new Dictionary<string, object>
                {
                    { "tile", tile ? "true" : "false" },
                    { "include_entities", includeEntities ? "true" : "false" },
                    { "skip_status", skipStatus ? "true" : "false" },
                    { "use", use ? "true" : "false" }
                };

            if (fileName != null)
            {
                var fileInfo = new FileInfo(fileName);
                if (!fileInfo.Exists)
                    throw new ArgumentException("The specified file does not exist.", "fileName");
                if (fileInfo.Length > ProfileBackgroundImageSize)
                    throw new TwitterException("The size of the image file must be less than 800KB.");

                var data = new byte[fileInfo.Length];
                using (var fs = fileInfo.OpenRead())
                {
                    fs.Read(data, 0, data.Length);
                }

                postData.Add("image", Convert.ToBase64String(data));
            }

            var uri = new Uri(this.CommandBaseUri + "/update_profile_background_image.json");

            var response    = this.TwitterApi.ExecuteAuthenticatedRequestForMultipartFormData(uri, postData);
            var twitterUser = TwitterObject.Parse<TwitterUser>(response);
            
            return twitterUser;
        }

        /// <summary>
        /// Sets one or more hex values that control the color scheme of the authenticating 
        /// user's profile page on twitter.com. 
        /// </summary>
        /// <param name="backgroundColor">Profile background color.</param>
        /// <param name="linkColor">Profile link color.</param>
        /// <param name="sidebarBorderColor">Profile sidebar's border color.</param>
        /// <param name="sidebarFillColor">Profile sidebar's background color.</param>
        /// <param name="textColor">Profile text color.</param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TwitterException"></exception>
        public TwitterUser UpdateProfileColors(TwitterColor? backgroundColor = null, TwitterColor? linkColor = null,
                                               TwitterColor? sidebarBorderColor = null, 
                                               TwitterColor? sidebarFillColor = null, TwitterColor? textColor = null, 
                                               bool includeEntities = true, bool skipStatus = false)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>
                {
                    { "include_entities", includeEntities ? "true" : "false" },
                    { "skip_status", skipStatus ? "true" : "false" }
                };

            if (backgroundColor.HasValue)
                postData.Add("profile_background_color", backgroundColor.Value.ToString());
            if (linkColor.HasValue)
                postData.Add("profile_link_color", linkColor.Value.ToString());
            if (sidebarBorderColor.HasValue)
                postData.Add("profile_sidebar_border_color", sidebarBorderColor.Value.ToString());
            if (sidebarFillColor.HasValue)
                postData.Add("profile_sidebar_fill_color", sidebarFillColor.Value.ToString());
            if (textColor.HasValue)
                postData.Add("profile_text_color", textColor.Value.ToString());

            var uri = new Uri(this.CommandBaseUri + "/update_profile_colors.json");

            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);
            var twitterUser = TwitterObject.Parse<TwitterUser>(response);

            return twitterUser;
        }

        /// <summary>
        /// Updates the authenticating user's profile image. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="includeEntities"></param>
        /// <param name="skipStatus"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TwitterException"></exception>
        public TwitterUser UpdateProfileImage(string fileName, bool includeEntities = true, bool skipStatus = false)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentException();

            var imageType = new[] { ".GIF", ".JPG", ".JPEG", ".PNG" };
            if (!imageType.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid image format.", "fileName");

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
                throw new ArgumentException("The specified file does not exist.", "fileName");

            if (fileInfo.Length > ProfileImageSize)
                throw new TwitterException("The size of the image file must be less than 700KB.");

            var data = new byte[fileInfo.Length];
            using (var fs = fileInfo.OpenRead())
            {
                fs.Read(data, 0, data.Length);
            }

            var postData = new Dictionary<string, object>
                {
                    { "image", Convert.ToBase64String(data) },
                    { "include_entities", includeEntities ? "true" : "false" },
                    { "skip_status", skipStatus ? "true" : "false" }
                };

            var uri = new Uri(this.CommandBaseUri + "/update_profile_image.json");

            var response    = this.TwitterApi.ExecuteAuthenticatedRequestForMultipartFormData(uri, postData);
            var twitterUser = TwitterObject.Parse<TwitterUser>(response);

            return twitterUser;
        }

        /// <summary>
        /// Retrieves the current count of friends, followers, updates (statuses) and favorites of the authenticating user.
        /// </summary>
        /// <param name="friends">The current count of friends of the authenticating user.</param>
        /// <param name="statuses">The current count of statuses(updates) of the authenticating user.</param>
        /// <param name="followers">The current count of followers of the authenticating user.</param>
        /// <param name="favorites">The current count of favorites of the authenticating user.</param>
        public void Totals(out int friends, out int statuses, out int followers, out int favorites)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + "/totals.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);
            var jsonObj     = JObject.Parse(response);

            friends = statuses = followers = favorites = -1;

            JToken value;
            if (jsonObj.TryGetValue("friends", out value))
                friends     = Int32.Parse(value.ToString());
            if (jsonObj.TryGetValue("updates", out value))
                statuses    = Int32.Parse(value.ToString());
            if (jsonObj.TryGetValue("followers", out value))
                followers   = Int32.Parse(value.ToString());
            if (jsonObj.TryGetValue("favorites", out value))
                favorites   = Int32.Parse(value.ToString());
        }

        /// <summary>
        /// Use this method to test if supplied user credentials are valid.
        /// </summary>
        /// <param name="twitterUser"></param>
        /// <returns>
        /// Returns true if credentials are valid. Otherwise, false.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TwitterException"></exception>
        public bool VerifyCredentials(out TwitterUser twitterUser)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            twitterUser = null;

            var valid   = false;
            var uri     = new Uri(this.CommandBaseUri + "/verify_credentials.json");

            try
            {
                var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

                twitterUser     = TwitterObject.Parse<TwitterUser>(response);
                valid           = true;
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.Unauthorized)
                    throw;
            }

            return valid;
        }

        #endregion
    }
}