namespace Mirai.Twitter.Commands
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

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

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var savedSeach  = TwitterSavedSearch.FromDictionary(jsonObj);

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

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var savedSeach  = TwitterSavedSearch.FromDictionary(jsonObj);

            return savedSeach;
        }

        public TwitterSavedSearch[] RetrieveSavedSearches()
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + ".json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonArray       = (ArrayList)JSON.Instance.Parse(response);
            var savedSeaches    = (from Dictionary<string, object> search in jsonArray
                                   select TwitterSavedSearch.FromDictionary(search)).ToArray();

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

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var savedSeach  = TwitterSavedSearch.FromDictionary(jsonObj);

            return savedSeach;
        }

        #endregion
    }
}