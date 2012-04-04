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


namespace Mirai.Twitter.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Commands;

    using Newtonsoft.Json;

    public sealed class TwitterApi
    {
        #region Constants and Fields

        internal static readonly string ApiBaseUri     = "https://api.twitter.com";
        internal static readonly string UploadApiUri   = "https://upload.twitter.com";
        internal static readonly string SearchApiUri   = "http://search.twitter.com";

        private readonly OAuth _OAuth;
        private readonly Timer _Timer;

        private TwitterConfiguration _Configuration;

        #endregion

        #region Constructors and Destructors

        public TwitterApi(string consumerKey, string consumerSecret, ApiVersion apiVersion = ApiVersion.V1)
            : this(consumerKey, consumerSecret, "", "", TokenType.InvalidToken, apiVersion)
        {
            
        }

        public TwitterApi(string consumerKey, string consumerSecret, 
                          string accessToken, string tokenSecret, ApiVersion apiVersion = ApiVersion.V1)
            : this(consumerKey, consumerSecret, accessToken, tokenSecret, TokenType.AccessToken, apiVersion)
        {

        }

        private TwitterApi(string consumerKey, string consumerSecret,
            string accessToken, string tokenSecret, TokenType tokenType, ApiVersion apiVersion = ApiVersion.V1)
        {
            var consumerCredential = new ConsumerCredential(consumerKey, consumerSecret);
            consumerCredential.SetToken(accessToken, tokenSecret, tokenType);

            var serviceProviderDescription = new ServiceProviderDescription(
                    new OAuthEndPoint("https://api.twitter.com/oauth/request_token"),
                    new OAuthEndPoint("https://api.twitter.com/oauth/authorize", HttpMethod.Get),
                    new OAuthEndPoint("https://api.twitter.com/oauth/access_token"),
                    ProtocolVersion.V10A);

            this._OAuth         = new OAuth(consumerCredential, serviceProviderDescription);
            this._OAuth.Realm   = ApiBaseUri;
            this._OAuth.Proxy   = null;

            this.ApiVersion     = apiVersion;

            this._Configuration = new TwitterConfiguration();

            this._Timer = new Timer(
                                    _ =>
                                    {
                                        var newConfig = this.RetrieveConfiguration();
                                        if (newConfig != null)
                                            Interlocked.Exchange(ref this._Configuration, newConfig);
                                    }, null, 1000, 3600 * 24);
        }

        #endregion

        internal OAuth OAuth
        {
            get { return this._OAuth; }
        }


        #region Public Properties

        public ApiVersion ApiVersion { get; internal set; }

        public bool Authenticated
        {
            get { return this._OAuth.Authenticated; }
        }

        public TwitterConfiguration Configuration
        {
            get { return this._Configuration; }
        }

        public bool LogEnabled
        {
            get { return this._OAuth.LogEnabled; }
            set { this._OAuth.LogEnabled = value; }
        }

        public TextWriter LogStream
        {
            get { return this._OAuth.LogStream; }
            set { this._OAuth.LogStream = value; }
        }

        public IWebProxy Proxy
        {
            get { return this._OAuth.Proxy; }
            set { this._OAuth.Proxy = value; }
        }

        public string UserAgent
        {
            get { return this._OAuth.UserAgent; }
            set { this._OAuth.UserAgent = value; }
        }

        #endregion


        #region Twitter Commands

        public AccountCommand AccountCommand
        {
            get
            {
                AccountCommand accountCmd = null;
                LazyInitializer.EnsureInitialized(ref accountCmd, () => new AccountCommand(this));

                return accountCmd;
            }
        }

        public BlockCommand BlockCommand
        {
            get
            {
                BlockCommand blockCmd = null;
                LazyInitializer.EnsureInitialized(ref blockCmd, () => new BlockCommand(this));

                return blockCmd;
            }
        }

        public DirectMessageCommand DirectMessageCommand
        {
            get
            {
                DirectMessageCommand directMessageCmd = null;
                LazyInitializer.EnsureInitialized(ref directMessageCmd, () => new DirectMessageCommand(this));

                return directMessageCmd;
            }
        }

        public FavoriteCommand FavoriteCommand
        {
            get
            {
                FavoriteCommand favoriteCmd = null;
                LazyInitializer.EnsureInitialized(ref favoriteCmd, () => new FavoriteCommand(this));

                return favoriteCmd;
            }
        }

        public FriendshipCommand FriendshipCommand
        {
            get
            {
                FriendshipCommand friendshipCmd = null;
                LazyInitializer.EnsureInitialized(ref friendshipCmd, () => new FriendshipCommand(this));

                return friendshipCmd;
            }
        }

        public GeoCommand GeoCommand
        {
            get
            {
                GeoCommand geoCmd = null;
                LazyInitializer.EnsureInitialized(ref geoCmd, () => new GeoCommand(this));

                return geoCmd;
            }
        }

        public ListCommand ListCommand
        {
            get
            {
                ListCommand listCmd = null;
                LazyInitializer.EnsureInitialized(ref listCmd, () => new ListCommand(this));

                return listCmd;
            }
        }

        public NotificationCommand NotificationCommand
        {
            get
            {
                NotificationCommand notificationCmd = null;
                LazyInitializer.EnsureInitialized(ref notificationCmd, () => new NotificationCommand(this));

                return notificationCmd;
            }
        }

        public SavedSearchCommand SavedSearchCommand
        {
            get
            {
                SavedSearchCommand savedSearchCmd = null;
                LazyInitializer.EnsureInitialized(ref savedSearchCmd, () => new SavedSearchCommand(this));

                return savedSearchCmd;
            }
        }

        public SearchCommand SearchCommand
        {
            get
            {
                SearchCommand searchCommand = null;
                LazyInitializer.EnsureInitialized(ref searchCommand, () => new SearchCommand(this));

                return searchCommand;
            }
        }

        public SpamReportingCommand SpamReportingCommand
        {
            get
            {
                SpamReportingCommand spanReportingCmd = null;
                LazyInitializer.EnsureInitialized(ref spanReportingCmd, () => new SpamReportingCommand(this));

                return spanReportingCmd;
            }
        }

        public SuggestedUserCommand SuggestedUserCommand
        {
            get
            {
                SuggestedUserCommand suggestedUserCmd = null;
                LazyInitializer.EnsureInitialized(ref suggestedUserCmd, () => new SuggestedUserCommand(this));

                return suggestedUserCmd;
            }
        }

        public TimelineCommand TimelineCommand
        {
            get
            {
                TimelineCommand timelineCmd = null;
                LazyInitializer.EnsureInitialized(ref timelineCmd, () => new TimelineCommand(this));

                return timelineCmd;
            }
        }

        public TrendCommand TrendCommand
        {
            get
            {
                TrendCommand trendCmd = null;
                LazyInitializer.EnsureInitialized(ref trendCmd, () => new TrendCommand(this));

                return trendCmd;
            }
        }

        public TweetCommand TweetCommand
        {
            get
            {
                TweetCommand tweetCmd = null;
                LazyInitializer.EnsureInitialized(ref tweetCmd, () => new TweetCommand(this));

                return tweetCmd;
            }
        }

        public UserCommand UserCommand
        {
            get
            {
                UserCommand userCmd = null;
                LazyInitializer.EnsureInitialized(ref userCmd, () => new UserCommand(this));

                return userCmd;
            }
        }

        #endregion


        #region Public Methods
        
        /// <summary>
        /// Acquires the access token for the non Pin-based flow authentication.
        /// </summary>
        /// <param name="postData"></param>
        public void AcquireAccessToken(IEnumerable<KeyValuePair<string, string>> postData)
        {
            this._OAuth.AcquireAccessToken(postData);
        }

        public void AcquireAccessToken(string oauthVerifier, IDictionary<string, string> postData = null)
        {
            try
            {
                this._OAuth.AcquireAccessToken(oauthVerifier, postData);
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.ProtocolError)
                    throw;

                throw new TwitterException(e.Message, e);
            }
        }

        public Uri AcquireRequestToken(string oauthCallback, IDictionary<string, string> postData = null)
        {
            try
            {
                return this._OAuth.AcquireRequestToken(oauthCallback, postData);
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.ProtocolError)
                    throw;

                throw new TwitterException(e.Message, e);
            }
        }

        public string ExecuteAuthenticatedRequest(Uri resourceUri,
                                                  HttpMethod httpMethod,
                                                  IDictionary<string, string> postData)
        {
            string result;

            try
            {
                result = this._OAuth.ExecuteAuthenticatedRequest(resourceUri, httpMethod, postData);
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.ProtocolError)
                    throw;

                throw new TwitterException(e.Message, e);
            }

            return result;
        }

        public string ExecuteAuthenticatedRequestForMultipartFormData(
                                                Uri resourceUrl, IEnumerable<KeyValuePair<string, object>> postData)
        {
            try
            {
                return this._OAuth.ExecuteAuthenticatedRequestForMultipartFormData(resourceUrl, postData);
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.ProtocolError)
                    throw;

                throw new TwitterException(e.Message, e);
            }
        }

        public string ExecuteUnauthenticatedRequest(Uri resourceUri,
                                                    HttpMethod httpMethod = HttpMethod.Get,
                                                    IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            try
            {
                return this._OAuth.ExecuteUnauthenticatedRequest(resourceUri, httpMethod, postData);
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.ProtocolError)
                    throw;

                throw new TwitterException(e.Message, e);
            }

        }

        public TwitterLanguage[] GetSupportedLanguages()
        {
            var uriBuilder  = new UriBuilder(ApiBaseUri + "/" + this.ApiVersion.ToString("D"));
            uriBuilder.Path += "/help/languages.json";

            var response    = this.ExecuteUnauthenticatedRequest(uriBuilder.Uri);
            var languages   = JsonConvert.DeserializeObject<TwitterLanguage[]>(response);

            return languages;
        }

        #endregion


        #region Private Methods

        private TwitterConfiguration RetrieveConfiguration()
        {
            var uriBuilder = new UriBuilder(ApiBaseUri + "/" + this.ApiVersion.ToString("D"));
            uriBuilder.Path += "/help/configuration.json";

            TwitterConfiguration config = null;
            try
            {
                var response    = this.ExecuteUnauthenticatedRequest(uriBuilder.Uri);
                config          = JsonConvert.DeserializeObject<TwitterConfiguration>(response);
            }
            catch (WebException e)
            {
            }

            return config;
        }

        #endregion

    }
}
