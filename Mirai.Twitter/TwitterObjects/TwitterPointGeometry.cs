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
    using System.Text;

    using Mirai.Twitter.Core;

    public sealed class TwitterPointGeometry : TwitterGeometry
    {
        #region Constructors and Destructors

        public TwitterPointGeometry()
        {
            this.CoordinatesList.Add(new TwitterCoordinate());
        }

        public TwitterPointGeometry(double latitude, double longitude)
        {
            var coordinate = new TwitterCoordinate(latitude, longitude);
            this.CoordinatesList.Add(coordinate);
        }

        #endregion


        #region Public Properties

        public double Latitude
        {
            get { return this.CoordinatesList[0].Latitude; }
            set { this.CoordinatesList[0].Latitude = value; }
        }

        public double Longitude
        {
            get { return this.CoordinatesList[0].Longitude; }
            set { this.CoordinatesList[0].Longitude = value; }
        }

        #endregion


        #region Public Methods

        public static TwitterPointGeometry FromDictionary(IDictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterPointGeometry>(dictionary);
        }

        public static TwitterPointGeometry Parse(string jsonString)
        {
            return Parse<TwitterPointGeometry>(jsonString);
        }

        #endregion


        #region Overrides of TwitterObject

        internal override void Init(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (dictionary.Count == 0)
                return;

            object typeValue;
            if (dictionary.TryGetValue("type", out typeValue) == false || typeValue == null)
                throw new FormatException();

            object coordinatesValue;
            if (dictionary.TryGetValue("coordinates", out coordinatesValue) == false || coordinatesValue == null)
                throw new FormatException();

            if (!((string)typeValue).Equals("Point", StringComparison.OrdinalIgnoreCase))
                throw new FormatException();

            var strArr = (ArrayList)coordinatesValue;

            this.CoordinatesList.Add(
                new TwitterCoordinate(strArr[0].ToString().ToDouble(), strArr[1].ToString().ToDouble()));
        }

        public override string ToJson()
        {
            var sb = new StringBuilder();
            if (this.CoordinatesList.Count > 0)
            {
                if (!this.IsCoordinate)
                    sb.AppendFormat("[{0},{1}]", this.CoordinatesList[0].Longitude, this.CoordinatesList[0].Latitude);
                else
                    sb.Append(this.CoordinatesList[0].ToJson());
            }

            return String.Format("{{\"type\":\"Point\",\"coordinates\":{0}}}", sb.Length > 0 ? sb.ToString() : "null");
        }

        #endregion
    }
}