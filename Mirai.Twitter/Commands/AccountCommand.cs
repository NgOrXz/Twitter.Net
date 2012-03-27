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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;

    using fastJSON;

    /// <summary>
    /// Account-level configuration settings for users.
    /// </summary>
    public sealed class AccountCommand : TwitterCommandBase
    {
        #region Constants and Fields

        public static readonly int ProfileBackgroundImageSize = 800 * 1024;
        public static readonly int ProfileImageSize = 700 * 1024;

        #endregion

        #region Constructors and Destructors

        internal AccountCommand(TwitterApi twitterApi)
            : base(twitterApi, "account")
        {
        }

        #endregion

        #region Public Methods

        public void EndSession()
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri = new Uri(this.CommandBaseUri + "/end_session.json");
            this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, null);
        }

        public TwitterRateLimitStatus RetrieveRateLimitStatus()
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + "/rate_limit_status.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var rateLimit   = TwitterRateLimitStatus.FromDictionary(jsonObj);

            return rateLimit;
        }

        public TwitterUser UpdateProfile(string name = null, string url = null, string location = null,
                                         string description = null, bool includeEntities = true, bool skipStatus = false)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (!String.IsNullOrEmpty(url))
            {
                var tmpUrl = new Uri(url);
                if (tmpUrl.Scheme != Uri.UriSchemeHttp && tmpUrl.Scheme != Uri.UriSchemeHttps)
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
                postData.Add("url", url);
            if (location != null)
                postData.Add("location", location);
            if (description != null)
                postData.Add("description", description);

            var uri         = new Uri(this.CommandBaseUri + "/update_profile.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var twitterUser = TwitterUser.FromDictionary(jsonObj);

            return twitterUser;
        }

        public TwitterUser UpdateProfileBackgroundImage(string fileName = null, bool tile = false, 
                                                        bool includeEntities = true, 
                                                        bool skipStatus = false, bool use = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var imageType = new[] { ".GIF", ".JPG", ".JPEG", ".PNG" };
            if (fileName != null && !imageType.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid image format.", "fileName");

            var postData = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("tile", tile ? "true" : "false" ),
                    new KeyValuePair<string, object>("include_entities", includeEntities ? "true" : "false"),
                    new KeyValuePair<string, object>("skip_status", skipStatus ? "true" : "false"),
                    new KeyValuePair<string, object>("use", use ? "true" : "false")
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

                postData.Add(new KeyValuePair<string, object>("image", Convert.ToBase64String(data)));
            }

            var uri         = new Uri(this.CommandBaseUri + "/update_profile_background_image.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequestForMultipartFormData(uri, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var twitterUser = TwitterUser.FromDictionary(jsonObj);

            return twitterUser;
        }

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

            var uri         = new Uri(this.CommandBaseUri + "/update_profile_colors.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var twitterUser = TwitterUser.FromDictionary(jsonObj);

            return twitterUser;
        }

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

            var postData = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("image", Convert.ToBase64String(data)),
                    new KeyValuePair<string, object>("include_entities", includeEntities ? "true" : "false"),
                    new KeyValuePair<string, object>("skip_status", skipStatus ? "true" : "false")
                };

            var uri         = new Uri(this.CommandBaseUri + "/update_profile_image.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequestForMultipartFormData(uri, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var twitterUser = TwitterUser.FromDictionary(jsonObj);

            return twitterUser;
        }

        /// <summary>
        /// Retrieves the current count of friends, followers, updates (statuses) and favorites of the authenticating user.
        /// </summary>
        /// <param name="friends"></param>
        /// <param name="statuses"></param>
        /// <param name="followers"></param>
        /// <param name="favorites"></param>
        public void Totals(out int friends, out int statuses, out int followers, out int favorites)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + "/totals.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);
            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);

            friends = statuses = followers = favorites = -1;

            object value;
            if (jsonObj.TryGetValue("friends", out value))
                friends     = value.ToString().ToInt32();
            if (jsonObj.TryGetValue("updates", out value))
                statuses    = value.ToString().ToInt32();
            if (jsonObj.TryGetValue("followers", out value))
                followers   = value.ToString().ToInt32();
            if (jsonObj.TryGetValue("favorites", out value))
                favorites   = value.ToString().ToInt32();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="twitterUser"></param>
        /// <returns>
        /// Returns true if credentials are valid. Otherwise, false.
        /// </returns>
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
                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);

                twitterUser = TwitterUser.FromDictionary(jsonObj);
                valid       = true;
            }
            catch (TwitterException)
            {
            }

            return valid;
        }

        #endregion
    }
}