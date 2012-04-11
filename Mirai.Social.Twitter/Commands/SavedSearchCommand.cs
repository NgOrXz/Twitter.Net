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

namespace Mirai.Social.Twitter.Commands
{
    using System;
    using System.Collections.Generic;

    using Mirai.Net.OAuth;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    public sealed class SavedSearchCommand : TwitterCommandBase
    {
        #region Constructors and Destructors

        internal SavedSearchCommand(TwitterApi twitterApi)
            : base(twitterApi, "saved_searches")
        {
        }

        #endregion

        #region Public Methods

        public TwitterSavedSearch Create(string query)
        {
            if (String.IsNullOrEmpty(query))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData    = new Dictionary<string, string> { { "query", query } };

            var uri         = new Uri(this.CommandBaseUri + "/create.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var savedSeach  = TwitterObject.Parse<TwitterSavedSearch>(response);

            return savedSeach;
        }

        public TwitterSavedSearch Destroy(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + String.Format("/destroy/{0}.json", id));
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, null);

            var savedSeach  = TwitterObject.Parse<TwitterSavedSearch>(response);

            return savedSeach;
        }

        public TwitterSavedSearch[] RetrieveSavedSearches()
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + ".json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var savedSeaches    = JsonConvert.DeserializeObject<TwitterSavedSearch[]>(response);

            return savedSeaches;
        }

        public TwitterSavedSearch Show(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + String.Format("/show/{0}.json", id));
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var savedSeach  = TwitterObject.Parse<TwitterSavedSearch>(response);

            return savedSeach;
        }

        #endregion
    }
}