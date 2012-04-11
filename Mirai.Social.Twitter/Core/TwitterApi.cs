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


namespace Mirai.Social.Twitter.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;

    using Mirai.Net.OAuth;
    using Mirai.Social.Twitter.Commands;
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    public sealed class TwitterApi
    {
        #region Twitter Time Zones

        public static readonly TwitterTimeZoneInfo[] TimeZones = new[]
            {
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-11:00) International Date Line West",
                        Name = "Pacific/Midway",
                        UtcOffset = -39600
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-11:00) Midway Island",
                        Name = "Pacific/Midway",
                        UtcOffset = -39600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-11:00) Samoa", Name = "Pacific/Pago_Pago", UtcOffset = -39600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-10:00) Hawaii", Name = "Pacific/Honolulu", UtcOffset = -36000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-09:00) Alaska", Name = "America/Juneau", UtcOffset = -32400 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-08:00) Pacific Time (US & Canada)",
                        Name = "America/Los_Angeles",
                        UtcOffset = -28800
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-08:00) Tijuana", Name = "America/Tijuana", UtcOffset = -28800 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-07:00) Mountain Time (US & Canada)",
                        Name = "America/Denver",
                        UtcOffset = -25200
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-07:00) Arizona", Name = "America/Phoenix", UtcOffset = -25200 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-07:00) Chihuahua",
                        Name = "America/Chihuahua",
                        UtcOffset = -25200
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-07:00) Mazatlan", Name = "America/Mazatlan", UtcOffset = -25200 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-06:00) Central Time (US & Canada)",
                        Name = "America/Chicago",
                        UtcOffset = -21600
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-06:00) Saskatchewan",
                        Name = "America/Regina",
                        UtcOffset = -21600
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-06:00) Guadalajara",
                        Name = "America/Mexico_City",
                        UtcOffset = -21600
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-06:00) Mexico City",
                        Name = "America/Mexico_City",
                        UtcOffset = -21600
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-06:00) Monterrey",
                        Name = "America/Monterrey",
                        UtcOffset = -21600
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-06:00) Central America",
                        Name = "America/Guatemala",
                        UtcOffset = -21600
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-05:00) Eastern Time (US & Canada)",
                        Name = "America/New_York",
                        UtcOffset = -18000
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-05:00) IIndiana (East)",
                        Name = "America/Indiana/Indianapolis",
                        UtcOffset = -18000
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-05:00) Bogota", Name = "America/Bogota", UtcOffset = -18000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-05:00) Lima", Name = "America/Lima", UtcOffset = -18000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-05:00) Quito", Name = "America/Lima", UtcOffset = -18000 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-04:00) Atlantic Time (Canada)",
                        Name = "America/Halifax",
                        UtcOffset = -14400
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-04:30) Caracas", Name = "America/Caracas", UtcOffset = -12600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-04:00) La Paz", Name = "America/La_Paz", UtcOffset = -14400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-04:00) Santiago", Name = "America/Santiago", UtcOffset = -14400 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-03:30) Newfoundland",
                        Name = "America/St_Johns",
                        UtcOffset = -12600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-03:00) Brasilia", Name = "America/Sao_Paulo", UtcOffset = -10800 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-03:00) Buenos Aires",
                        Name = "America/Argentina/Buenos_Aires",
                        UtcOffset = -10800
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-03:00) Georgetown", Name = "America/Guyana", UtcOffset = -10800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-03:00) Greenland", Name = "America/Godthab", UtcOffset = -10800 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-02:00) Mid-Atlantic",
                        Name = "Atlantic/South_Georgia",
                        UtcOffset = -7200
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT-01:00) Azores", Name = "Atlantic/Azores", UtcOffset = -3600 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT-01:00) Cape Verde Is",
                        Name = "Atlantic/Cape_Verde",
                        UtcOffset = -3600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT) Dublin", Name = "Europe/Dublin", UtcOffset = 0 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT) Edinburgh", Name = "Europe/London", UtcOffset = 0 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT) Lisbon", Name = "Europe/Lisbon", UtcOffset = 0 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT) London", Name = "Europe/London", UtcOffset = 0 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT) Casablanca", Name = "Africa/Casablanca", UtcOffset = 0 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT) Monrovia", Name = "Africa/Monrovia", UtcOffset = 0 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Belgrade", Name = "Europe/Belgrade", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Bratislava", Name = "Europe/Bratislava", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Budapest", Name = "Europe/Budapest", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Ljubljana", Name = "Europe/Ljubljana", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Prague", Name = "Europe/Prague", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Sarajevo", Name = "Europe/Sarajevo", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Skopje", Name = "Europe/Skopje", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Warsaw", Name = "Europe/Warsaw", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Zagreb", Name = "Europe/Zagreb", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Brussels", Name = "Europe/Brussels", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Copenhagen", Name = "Europe/Copenhagen", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Madrid", Name = "Europe/Madrid", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Paris", Name = "Europe/Paris", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Amsterdam", Name = "Europe/Amsterdam", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Berlin", Name = "Europe/Berlin", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Bern", Name = "Europe/Berlin", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Rome", Name = "Europe/Rome", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Stockholm", Name = "Europe/Stockholm", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+01:00) Vienna", Name = "Europe/Vienna", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+01:00) West Central Africa",
                        Name = "Africa/Algiers",
                        UtcOffset = 3600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Bucharest", Name = "Europe/Bucharest", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Cairo", Name = "Africa/Cairo", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Helsinki", Name = "Europe/Helsinki", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Kyiv", Name = "Europe/Kiev", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Riga", Name = "Europe/Riga", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Sofia", Name = "Europe/Sofia", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Tallinn", Name = "Europe/Tallinn", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Vilnius", Name = "Europe/Vilnius", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Athens", Name = "Europe/Athens", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Istanbul", Name = "Europe/Istanbul", UtcOffset = 7200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+03:00) Minsk", Name = "Europe/Minsk", UtcOffset = 10800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Jerusalem", Name = "Asia/Jerusalem", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Harare", Name = "Africa/Harare", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+02:00) Pretoria", Name = "Africa/Johannesburg", UtcOffset = 3600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:00) Moscow", Name = "Europe/Moscow", UtcOffset = 14400 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+04:00) St. Petersburg",
                        Name = "Europe/Moscow",
                        UtcOffset = 14400
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:00) Volgograd", Name = "Europe/Moscow", UtcOffset = 14400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+03:00) Kuwait", Name = "Asia/Kuwait", UtcOffset = 10800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+03:00) Riyadh", Name = "Asia/Riyadh", UtcOffset = 10800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+03:00) Nairobi", Name = "Africa/Nairobi", UtcOffset = 10800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+03:00) Baghdad", Name = "Asia/Baghdad", UtcOffset = 10800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+03:00) Tehran", Name = "Asia/Tehran", UtcOffset = 10800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:00) Abu Dhabi", Name = "Asia/Muscat", UtcOffset = 14400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:00) Muscat", Name = "Asia/Muscat", UtcOffset = 14400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:00) Baku", Name = "Asia/Baku", UtcOffset = 14400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:00) Tbilisi", Name = "Asia/Tbilisi", UtcOffset = 14400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:00) Yerevan", Name = "Asia/Yerevan", UtcOffset = 14400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+04:30) Kabul", Name = "Asia/Kabul", UtcOffset = 16200 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+06:00) Ekaterinburg",
                        Name = "Asia/Yekaterinburg",
                        UtcOffset = 21600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:00) Islamabad", Name = "Asia/Karachi", UtcOffset = 18000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:00) Karachi", Name = "Asia/Karachi", UtcOffset = 18000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:00) Tashkent", Name = "Asia/Tashkent", UtcOffset = 18000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:30) Chennai", Name = "Asia/Kolkata", UtcOffset = 19800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:30) Kolkata", Name = "Asia/Kolkata", UtcOffset = 19800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:30) Mumbai", Name = "Asia/Kolkata", UtcOffset = 19800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:30) New Delhi", Name = "Asia/Kolkata", UtcOffset = 19800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+05:45) Kathmandu", Name = "Asia/Kathmandu", UtcOffset = 20700 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+06:00) Astana", Name = "Asia/Dhaka", UtcOffset = 21600 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+06:00) Dhaka", Name = "Asia/Dhaka", UtcOffset = 21600 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+06:00) Sri Jayawardenepura",
                        Name = "Asia/Colombo",
                        UtcOffset = 21600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+06:00) Almaty", Name = "Asia/Almaty", UtcOffset = 21600 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+07:00) Novosibirsk",
                        Name = "Asia/Novosibirsk",
                        UtcOffset = 25200
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+06:30) Rangoon", Name = "Asia/Rangoon", UtcOffset = 23400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+07:00) Bangkok", Name = "Asia/Bangkok", UtcOffset = 25200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+07:00) Hanoi", Name = "Asia/Bangkok", UtcOffset = 25200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+07:00) Jakarta", Name = "Asia/Jakarta", UtcOffset = 25200 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+08:00) Krasnoyarsk",
                        Name = "Asia/Krasnoyarsk",
                        UtcOffset = 28800
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+08:00) Beijing", Name = "Asia/Shanghai", UtcOffset = 28800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+08:00) Chongqing", Name = "Asia/Chongqing", UtcOffset = 28800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+08:00) Hong Kong", Name = "Asia/Hong_Kong", UtcOffset = 28800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+08:00) Urumqi", Name = "Asia/Urumqi", UtcOffset = 28800 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+08:00) Kuala Lumpur",
                        Name = "Asia/Kuala_Lumpur",
                        UtcOffset = 28800
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+08:00) Singapore", Name = "Asia/Singapore", UtcOffset = 28800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+08:00) Taipei", Name = "Asia/Taipei", UtcOffset = 28800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+08:00) Perth", Name = "Australia/Perth", UtcOffset = 28800 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+09:00) Irkutsk", Name = "Asia/Irkutsk", UtcOffset = 32400 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+08:00) Ulaan Bataar",
                        Name = "Asia/Ulaanbaatar",
                        UtcOffset = 28800
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+09:00) Seoul", Name = "Asia/Seoul", UtcOffset = 32400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+09:00) Osaka", Name = "Asia/Tokyo", UtcOffset = 32400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+09:00) Sapporo", Name = "Asia/Tokyo", UtcOffset = 32400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+09:00) Tokyo", Name = "Asia/Tokyo", UtcOffset = 32400 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+10:00) Yakutsk", Name = "Asia/Yakutsk", UtcOffset = 36000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+09:30) Darwin", Name = "Australia/Darwin", UtcOffset = 34200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+09:30) Adelaide", Name = "Australia/Adelaide", UtcOffset = 34200 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+10:00) Canberra",
                        Name = "Australia/Melbourne",
                        UtcOffset = 36000
                    },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+10:00) Melbourne",
                        Name = "Australia/Melbourne",
                        UtcOffset = 36000
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+10:00) Sydney", Name = "Australia/Sydney", UtcOffset = 36000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+10:00) Brisbane", Name = "Australia/Brisbane", UtcOffset = 36000 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+10:00) Hobart", Name = "Australia/Hobart", UtcOffset = 36000 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+11:00) Vladivostok",
                        Name = "Asia/Vladivostok",
                        UtcOffset = 39600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+10:00) Guam", Name = "Pacific/Guam", UtcOffset = 36000 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+10:00) Port Moresby",
                        Name = "Pacific/Port_Moresby",
                        UtcOffset = 36000
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+12:00) Magadan", Name = "Asia/Magadan", UtcOffset = 43200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+11:00) Solomon Is", Name = "Asia/Magadan", UtcOffset = 39600 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+11:00) New Caledonia",
                        Name = "Pacific/Noumea",
                        UtcOffset = 39600
                    },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+12:00) Fiji", Name = "Pacific/Fiji", UtcOffset = 43200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+12:00) Kamchatka", Name = "Asia/Kamchatka", UtcOffset = 43200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+12:00) Marshall Is", Name = "Pacific/Majuro", UtcOffset = 43200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+12:00) Auckland", Name = "Pacific/Auckland", UtcOffset = 43200 },
                new TwitterTimeZoneInfo
                    { DisplayName = "(GMT+12:00) Wellington", Name = "Pacific/Auckland", UtcOffset = 43200 },
                new TwitterTimeZoneInfo
                    {
                        DisplayName = "(GMT+13:00) Nuku'alofa",
                        Name = "Pacific/Tongatapu",
                        UtcOffset = 46800
                    }
            };

        #endregion

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

            this._Timer = new Timer(_ =>
                                    {
                                        var newConfig = this.RetrieveConfiguration();
                                        if (newConfig != null)
                                            Interlocked.Exchange(ref this._Configuration, newConfig);
                                    }, null, 1000, 1000 * 3600 * 24);
        }

        ~TwitterApi()
        {
            this._Timer.Dispose();
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
        /// Processes the user authorization for the non Pin-based flow authentication.
        /// </summary>
        /// <param name="postData"></param>
        /// <returns>Returns the extra, non-OAuth parameters.</returns>
        public void ProcessUserAuthorization(IDictionary<string, string> postData)
        {
            this._OAuth.ProcessUserAuthorization(postData);
        }

        /// <summary>
        /// Processes the user authorization.
        /// </summary>
        /// <param name="oauthVerifier"></param>
        /// <param name="postData"></param>
        /// <returns>Returns the extra, non-OAuth parameters.</returns>
        public void ProcessUserAuthorization(string oauthVerifier, IDictionary<string, string> postData = null)
        {
            try
            {
                this._OAuth.ProcessUserAuthorization(oauthVerifier, postData);
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.ProtocolError)
                    throw;

                throw new TwitterException(e.Message, e);
            }
        }

        /// <summary>
        /// Begins an OAuth authorization request. 
        /// </summary>
        /// <param name="oauthCallback"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public Uri RequestUserAuthorization(string oauthCallback, IDictionary<string, string> postData = null)
        {
            try
            {
                return this._OAuth.RequestUserAuthorization(oauthCallback, postData);
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

        public string ExecuteAuthenticatedRequestForMultipartFormData(Uri resourceUrl, 
                                                                      IDictionary<string, object> postData)
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
                                                    IDictionary<string, string> postData = null)
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
            catch (WebException)
            {
            }

            return config;
        }

        #endregion
    }
}
