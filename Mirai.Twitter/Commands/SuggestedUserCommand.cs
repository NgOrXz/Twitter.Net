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

    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    /// <summary>
    /// Categorical organization of users that others may be interested to follow. 
    /// </summary>
    public sealed class SuggestedUserCommand : TwitterCommandBase
    {
        #region Constructors and Destructors

        internal SuggestedUserCommand(TwitterApi twitterApi)
            : base(twitterApi, "users")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Access to Twitter's suggested dm list. This returns the list of suggested dm category. 
        /// </summary>
        /// <param name="language">
        /// Restricts the suggested category to the requested language. The language must be specified by 
        /// the appropriate two letter ISO 639-1 representation. Currently supported languages are provided by 
        /// the GET help/languages API request. Unsupported language codes will receive English (en) results. 
        /// If you use lang in this request, ensure you also include it when requesting the GET 
        /// users/suggestions/:slug list.
        /// </param>
        /// <returns>Returns the list of suggested dm category.</returns>
        public TwitterSuggestedUserCategory[] RetrieveSuggestionCategories(string language = null)
        {
            var queryString = String.IsNullOrEmpty(language) ? String.Empty : String.Format("?lang={0}", language);

            var uri         = new Uri(this.CommandBaseUri + String.Format("/suggestions.json{0}", queryString));

            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);
            var categories  = JsonConvert.DeserializeObject<TwitterSuggestedUserCategory[]>(response);

            return categories;
        }

        /// <summary>
        /// Access the users in a given category of the Twitter suggested dm list.
        /// It is recommended that end clients cache this data for no more than one hour.
        /// </summary>
        /// <param name="slug">The short name of list or a category.</param>
        /// <param name="language"></param>
        /// <returns></returns>
        public TwitterSuggestedUserCategory RetrieveSuggestionCategoryWithMembers(string slug, string language = null)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();

            var queryString = String.IsNullOrEmpty(language) ? String.Empty : String.Format("?lang={0}", language);

            var uri         = new Uri(this.CommandBaseUri + String.Format("/suggestions/{0}.json{1}", slug, queryString));

            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);
            var category    = TwitterObject.Parse<TwitterSuggestedUserCategory>(response);

            return category;
        }

        /// <summary>
        /// Access the users in a given category of the Twitter suggested dm list and return their most recent 
        /// status if they are not a protected dm.
        /// </summary>
        /// <param name="slug">The short name of list or a category.</param>
        /// <returns></returns>
        public TwitterUser[] RetrieveUsersInCategory(string slug)
        {
            if (String.IsNullOrEmpty(slug))
                throw new ArgumentException();

            var uri         = new Uri(this.CommandBaseUri + String.Format("/suggestions/{0}/members.json", slug));

            var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);
            var users       = JsonConvert.DeserializeObject<TwitterUser[]>(response);

            return users;
        }

        #endregion
    }
}