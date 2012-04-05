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
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

    using Newtonsoft.Json;

    /// <summary>
    /// The dm command.
    /// </summary>
    public sealed class TweetCommand : TwitterCommandBase
    {
        #region Constants and Fields

        private readonly string _UploadCommandBaseUri;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetCommand"/> class.
        /// </summary>
        /// <param name="twitterApi">
        /// The twitter object.
        /// </param>
        internal TweetCommand(TwitterApi twitterApi)
            : base(twitterApi, "statuses")
        {
            this._UploadCommandBaseUri  = TwitterApi.UploadApiUri + "/" + this.TwitterApi.ApiVersion.ToString("D") +
                                          "/statuses";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show dm objects of up to 100 members who retweeted the status. 
        /// </summary>
        /// <param name="id">The numerical ID of the desired status.</param>
        /// <param name="count">
        /// Specifies the number of retweets to try and retrieve, up to a maximum of 100. The typeValue of count 
        /// is best thought of as a limit to the number of Tweets to return because suspended or deleted content 
        /// is removed after the count has been applied.
        /// </param>
        /// <param name="page">Specifies the page of results to retrieve.</param>
        /// <returns></returns>
        public List<TwitterUser> RetweetedBy(string id, int count = 10, int page = 1)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            var uriBuilder      = new UriBuilder(this.CommandBaseUri);
            uriBuilder.Path     += String.Format("/{0}/retweeted_by.json", id);
            uriBuilder.Query    = String.Format("count={0}&page={1}", count > 100 ? 100 : count, page >= 1 ? page : 1);

            var response = this.TwitterApi.Authenticated ? 
                           this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Get, null) : 
                           this.TwitterApi.ExecuteUnauthenticatedRequest(uriBuilder.Uri);
            
            var users       = new List<TwitterUser>();
            var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
            users.AddRange(from Dictionary<string, object> jsonObj in jsonArray 
                           select TwitterUser.FromDictionary(jsonObj));

            return users;
        }

        /// <summary>
        /// Show user ids of up to 100 users who retweeted the status.
        /// </summary>
        /// <param name="id">The numerical ID of the desired status.</param>
        /// <param name="count">
        /// Specifies the number of retweets to try and retrieve, up to a maximum of 100. The typeValue of count 
        /// is best thought of as a limit to the number of Tweets to return because suspended or deleted content 
        /// is removed after the count has been applied.
        /// </param>
        /// <param name="page">Specifies the page of results to retrieve.</param>
        /// <returns></returns>
        public string[] RetweetedByIds(string id, int count = 10, int page = 1)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uriBuilder      = new UriBuilder(this.CommandBaseUri);
            uriBuilder.Path     += String.Format("/{0}/retweeted_by/ids.json", id);
            uriBuilder.Query    = String.Format("count={0}&page={1}", count > 100 ? 100 : count, page >= 1 ? page : 1);

            var response = this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Get, null);

            var jsonArray       = (ArrayList)JSON.Instance.Parse(response);
            string[] idsOfUsers = (from string jsonObj in jsonArray
                                   select jsonObj).ToArray();

            return idsOfUsers;
        }

        /// <summary>
        /// Returns up to 100 of the first retweets of a given tweet.
        /// </summary>
        /// <param name="id">The numerical ID of the desired status.</param>
        /// <param name="count">Specifies the number of records to retrieve. Must be less than or equal to 100.</param>
        /// <param name="trimUser">
        /// When set to either true, each tweet returned in a timeline will include a dm object including 
        /// only the status authors numerical ID. 
        /// </param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public List<TwitterTweet> Retweets(string id, int count = 10, bool trimUser = false, bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uriBuilder      = new UriBuilder(this.CommandBaseUri);
            uriBuilder.Path     += String.Format("/retweets/{0}.json", id);
            uriBuilder.Query    = String.Format("count={0}&trim_user={1}&include_entities={2}",
                                                count > 100 ? 100 : count,
                                                trimUser,
                                                includeEntities);

            var response = this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Get, null);

            var retweets    = new List<TwitterTweet>();
            var jsonArray   = (ArrayList)JSON.Instance.Parse(response);
            retweets.AddRange(from Dictionary<string, object> jsonObj in jsonArray
                              select TwitterTweet.FromDictionary(jsonObj));

            return retweets;
        }

        /// <summary>
        /// Returns a single status, specified by the id parameter below.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="trimUser"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterTweet Show(string id, bool trimUser = false, bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            var uriBuilder      = new UriBuilder(this.CommandBaseUri);
            uriBuilder.Path     += String.Format("/show/{0}.json", id);
            uriBuilder.Query    = String.Format("trim_user={0}&include_entities={1}", trimUser, includeEntities);

            var response = this.TwitterApi.Authenticated ?
                           this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Get, null) :
                           this.TwitterApi.ExecuteUnauthenticatedRequest(uriBuilder.Uri);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var tweet   = TwitterTweet.FromDictionary(jsonObj);

            return tweet;
        }

        /// <summary>
        /// Destroys the status specified by the required ID parameter. The authenticating dm must be 
        /// the author of the specified status. Returns the destroyed status if successful. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="trimUser"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterTweet Destroy(string id, bool trimUser = false, bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uriBuilder  = new UriBuilder(this.CommandBaseUri);
            uriBuilder.Path += String.Format("/destroy/{0}.json", id);

            var postData = new Dictionary<string, string>
                {
                    { "trim_user", trimUser ? "true" : "false" },
                    { "include_entities", includeEntities ? "true" : "false" }
                };

            var response = this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var tweet   = TwitterTweet.FromDictionary(jsonObj);

            return tweet;
        }

        /// <summary>
        /// Retweets a tweet. Returns the original tweet with retweet details embedded.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="trimUser"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterTweet Retweet(string id, bool trimUser = false, bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uriBuilder  = new UriBuilder(this.CommandBaseUri);
            uriBuilder.Path += String.Format("/retweet/{0}.json", id);

            var postData = new Dictionary<string, string>
                {
                    { "trim_user", trimUser ? "true" : "false" },
                    { "include_entities", includeEntities ? "true" : "false" }
                };

            var response = this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Post, postData);

            var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var tweet   = TwitterTweet.FromDictionary(jsonObj);

            return tweet;
        }

        /// <summary>
        /// Updates the authenticating dm's status, also known as tweeting.
        /// To upload an image to accompany the tweet, use UpdateWithMedia method.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="inReplyToStatusId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="placeId"></param>
        /// <param name="displayCoordinates"></param>
        /// <param name="trimUser"></param>
        /// <param name="includeEntities"></param>
        /// <returns></returns>
        public TwitterTweet Update(
                                    string status,
                                    string inReplyToStatusId = null,
                                    double latitude = double.NaN,
                                    double longitude = double.NaN,
                                    string placeId = null,
                                    bool displayCoordinates = true,
                                    bool trimUser = false,
                                    bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(status))
            {
                throw new ArgumentException();
            }

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string>
                {
                    { "status", status },
                    { "display_coordinates", displayCoordinates ? "true" : "false" },
                    { "trim_user", trimUser ? "true" : "false" },
                    { "include_entities", includeEntities ? "true" : "false" }
                };

            if (!string.IsNullOrEmpty(inReplyToStatusId))
            {
                postData.Add("in_reply_to_status_id", inReplyToStatusId);
            }

            if (!double.IsNaN(latitude))
            {
                postData.Add("lat", latitude.ToString("G", CultureInfo.InvariantCulture));
            }

            if (!double.IsNaN(longitude))
            {
                postData.Add("long", longitude.ToString("G", CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(placeId))
            {
                postData.Add("place_id", placeId);
            }

            var uriBuilder  = new UriBuilder(this.CommandBaseUri);
            uriBuilder.Path += "/update.json";

            var response = this.TwitterApi.ExecuteAuthenticatedRequest(uriBuilder.Uri, HttpMethod.Post, postData);

            var jsonObj         = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var twitterTweet    = TwitterTweet.FromDictionary(jsonObj);

            return twitterTweet;
        }

        /// <summary>
        /// Updates the authenticating dm's status and attaches media for upload. 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="inReplyToStatusId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="placeId"></param>
        /// <param name="displayCoordinates"></param>
        /// <param name="possiblySensitive"></param>
        /// <param name="mediaList"></param>
        /// <returns></returns>
        public TwitterTweet UpdateWithMedia(
                                            string status,
                                            string inReplyToStatusId = null,
                                            string latitude = null,
                                            string longitude = null,
                                            string placeId = null,
                                            bool displayCoordinates = true,
                                            bool possiblySensitive = false,
                                            IList<byte[]> mediaList = null)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException();
            }

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("status", status),
                    new KeyValuePair<string, object>(
                        "display_coordinates", displayCoordinates ? "true" : "false"),
                    new KeyValuePair<string, object>(
                        "possibly_sensitive", possiblySensitive ? "true" : "false")
                };

            if (!string.IsNullOrEmpty(inReplyToStatusId))
            {
                postData.Add(new KeyValuePair<string, object>("in_reply_to_status_id", inReplyToStatusId));
            }

            if (!string.IsNullOrEmpty(latitude))
            {
                postData.Add(new KeyValuePair<string, object>("lat", latitude));
            }

            if (!string.IsNullOrEmpty(longitude))
            {
                postData.Add(new KeyValuePair<string, object>("long", longitude));
            }

            if (!string.IsNullOrEmpty(placeId))
            {
                postData.Add(new KeyValuePair<string, object>("place_id", placeId));
            }

            if (mediaList != null)
            {
                postData.AddRange(mediaList.Take(this.TwitterApi.Configuration.MaxMediaPerUpload)
                                           .Select(media => new KeyValuePair<string, object>("media[]", media)));
            }

            var uriBuilder  = new UriBuilder(this._UploadCommandBaseUri);
            uriBuilder.Path += ("/update_with_media.json");

            var response = this.TwitterApi.ExecuteAuthenticatedRequestForMultipartFormData(uriBuilder.Uri, postData);

            var jsonObj         = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var twitterTweet    = TwitterTweet.FromDictionary(jsonObj);;

            return twitterTweet;
        }

        #endregion
    }
}