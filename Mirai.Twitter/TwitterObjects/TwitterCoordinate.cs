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

namespace Mirai.Twitter.TwitterObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    using Mirai.Twitter.Core;

    using fastJSON;

    /// <summary>
    /// The twitter coordinate.
    /// </summary>
    public sealed class TwitterCoordinate : TwitterObject
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [TwitterKey("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [TwitterKey("long")]
        public double Longitude { get; set; }

        #endregion

        #region Constructors and Destructors

        public TwitterCoordinate()
        {
        }

        public TwitterCoordinate(double longitude, double latitude)
        {
            this.Latitude   = latitude;
            this.Longitude  = longitude;
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Assume the first element is longitude, second is latitude.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TwitterCoordinate FromList(IList list)
        {
            var coordinate = new TwitterCoordinate
                {
                    Longitude   = list[0].ToString().ToDouble(),
                    Latitude    = list[1].ToString().ToDouble()
                };

            return coordinate;
        }

        /// <summary>
        /// [,]
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        /// <exception cref="JsonParseException"></exception>
        public static TwitterCoordinate Parse(string jsonString)
        {
            var jsonArray = (ArrayList)JSON.Instance.Parse(jsonString);

            return FromList(jsonArray);
        }

        /// <summary>
        /// [longitude, latitude]
        /// </summary>
        /// <returns></returns>
        public override string ToJson()
        {
            return String.Format("[{0},{1}]",
                this.Longitude.ToString(CultureInfo.InvariantCulture),
                this.Latitude.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        internal override void Init(IDictionary<string, object> dictionary)
        {
            throw new NotSupportedException();
        }
    }
}