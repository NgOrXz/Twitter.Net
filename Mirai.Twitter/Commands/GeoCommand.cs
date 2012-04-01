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
    using System.Linq;
    using System.Net;
    using System.Text;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

    using fastJSON;

    public sealed class GeoCommand : TwitterCommandBase
    {
        #region Constructors and Destructors

        internal GeoCommand(TwitterApi twitterApi)
            : base(twitterApi, "geo")
        {
        }

        #endregion

        #region Public Methods

        public TwitterPlace CreatePlace(string name, string containedWithin, string token, string latitude, string longitude, string streetAddress = null)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(containedWithin) || String.IsNullOrEmpty(token) ||
                String.IsNullOrEmpty(latitude) || String.IsNullOrEmpty(longitude))
                throw new ArgumentException();

            var postData = new Dictionary<string, string>
                {
                    { "name", name },
                    { "contained_within", containedWithin },
                    { "token", token },
                    { "lat", latitude },
                    { "long", longitude }
                };

            if (!String.IsNullOrEmpty(streetAddress))
                postData.Add("attribute:street_address", streetAddress);

            var uri         = new Uri(this.CommandBaseUri + "/place.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var place       = TwitterPlace.FromDictionary(jsonObj);

            return place; 
        }

        /// <summary>
        /// Returns all the information about a known place.
        /// </summary>
        /// <param name="placeId">A place in the world. These IDs can be retrieved from ReverseGeocode method.</param>
        /// <returns></returns>
        public TwitterPlace RetrievePlaceById(string placeId)
        {
            if (String.IsNullOrEmpty(placeId))
                throw new ArgumentException();

            var uri = new Uri(this.CommandBaseUri + String.Format("/id/{0}.json", placeId));

            TwitterPlace place = null;
            try
            {
                var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
                place           = TwitterPlace.FromDictionary(jsonObj);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return place;
        }

        /// <summary>
        /// Locates places near the given coordinates which are similar in name.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="name"></param>
        /// <param name="containedWithin">
        /// This is the place_id which you would like to restrict the search results to. Setting this value means 
        /// only places within the given place_id will be found.
        /// </param>
        /// <param name="streetAddress"></param>
        /// <returns></returns>
        public TwitterGeoSimilarPlaces RetrieveSimilarPlaces(string latitude, string longitude, string name,
                                                             string containedWithin = null, string streetAddress = null)
        {
            if (String.IsNullOrEmpty(latitude) || String.IsNullOrEmpty(longitude) || String.IsNullOrEmpty(name))
                throw new ArgumentException();

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?lat={0}&long={1}&name={2}&", latitude, longitude, name);

            if (!String.IsNullOrEmpty(containedWithin))
                queryBuilder.AppendFormat("contained_within={0}&", containedWithin);
            if (!String.IsNullOrEmpty(streetAddress))
                queryBuilder.AppendFormat("attribute:street_address={0}", streetAddress);

            var uri = new Uri(this.CommandBaseUri + "/similar_places.json" + queryBuilder.ToString().TrimEnd('&'));

            TwitterGeoSimilarPlaces similarPlaces = null;
            try
            {
                var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
                similarPlaces   = TwitterGeoSimilarPlaces.FromDictionary((Dictionary<string, object>)jsonObj["result"]);
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return similarPlaces;
        }

        /// <summary>
        /// Given a latitude and a longitude, searches for up to 20 places that can be used as 
        /// a placeId when updating a status.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="accuracy"></param>
        /// <param name="granularity"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public TwitterPlace[] ReverseGeoCode(string latitude, string longitude, string accuracy = null,
                                             TwitterPlaceType granularity = TwitterPlaceType.Neighborhood,
                                             int? maxResults = null)
        {
            if (String.IsNullOrEmpty(latitude) || String.IsNullOrEmpty(longitude))
                throw new ArgumentException();

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?lat={0}&long={1}&granularity={2}&", 
                                      latitude, longitude, granularity.ToString().ToLowerInvariant());

            if (!String.IsNullOrEmpty(accuracy))
                queryBuilder.AppendFormat("accuracy={0}&", accuracy);
            if (maxResults.HasValue)
                queryBuilder.AppendFormat("max_results={0}", maxResults);

            var uri = new Uri(this.CommandBaseUri + "/reverse_geocode.json" + queryBuilder.ToString().TrimEnd('&'));

            TwitterPlace[] places = null;
            try
            {
                var response    = this.TwitterApi.ExecuteUnauthenticatedRequest(uri);

                var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
                places          = TwitterGeoSimilarPlaces.FromDictionary(
                                                                    (Dictionary<string, object>)jsonObj["result"]).Places;
                                   
            }
            catch (TwitterException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return places;
        }

        #endregion
    }
}