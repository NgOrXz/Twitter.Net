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

    using Mirai.Twitter.Core;

    public abstract class TwitterGeometry
    {
        protected readonly List<TwitterCoordinate> CoordinatesList;

        protected TwitterGeometry()
        {
            this.CoordinatesList = new List<TwitterCoordinate>();
        }

        /// <summary>
        /// If there are no coordinate, return an empty array not null.
        /// </summary>
        public TwitterCoordinate[] Coordinates
        {
            get { return this.CoordinatesList != null ? this.CoordinatesList.ToArray() : new TwitterCoordinate[] { }; }
        }

        /// <summary>
        /// If failure, return null.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static TwitterGeometry Create(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (dictionary.Count == 0)
                return null;

            object typeValue;
            if (dictionary.TryGetValue("type", out typeValue) == false || typeValue == null)
                return null;

            object coordinatesValue;
            if (dictionary.TryGetValue("coordinates", out coordinatesValue) == false || coordinatesValue == null)
                return null;

            if (((string)typeValue).Equals("Point", StringComparison.OrdinalIgnoreCase))
            {
                var strArr = (ArrayList)coordinatesValue;
                var point = new TwitterPointGeometry(strArr[0].ToString().ToDouble(), strArr[1].ToString().ToDouble());

                return point;
            }
            
            if (((string)typeValue).Equals("Polygon", StringComparison.OrdinalIgnoreCase))
            {
                var polygon = new TwitterPolygonGeometry();
                foreach (ArrayList coordinate in (ArrayList)coordinatesValue)
                {
                    polygon.Add(new TwitterCoordinate(coordinate[0].ToString().ToDouble(),
                                                      coordinate[1].ToString().ToDouble()));
                }

                return polygon;
            }

            return null;
        }
    }

    public sealed class TwitterPointGeometry : TwitterGeometry
    {
        public TwitterPointGeometry(double latitude, double longitude)
        {
            var coordinate = new TwitterCoordinate(latitude, longitude);
            this.CoordinatesList.Add(coordinate);
        }

        public double Latitude
        {
            get { return this.CoordinatesList.Count > 0 ? this.CoordinatesList[0].Latitude : Double.NaN; }
        }

        public double Longitude
        {
            get { return this.CoordinatesList.Count > 0 ? this.CoordinatesList[0].Longitude : Double.NaN; }
        }

        /// <summary>
        /// Returns the json representation.
        /// { "type":"Point", "coordinate":[latitude, longitude] }
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(
                "{{ \"type\":\"Point\", \"coordinates\":{0} }}",
                this.CoordinatesList.Count > 0 ? this.CoordinatesList[0].ToString() : String.Empty);
        }
    }

    public sealed class TwitterPolygonGeometry : TwitterGeometry
    {
        public void Add(TwitterCoordinate coordinate)
        {
            this.CoordinatesList.Add(coordinate);
        }

        public void Clear()
        {
            this.CoordinatesList.Clear();
        }

        public bool Remove(TwitterCoordinate coordinate)
        {
            return this.CoordinatesList.Remove(coordinate);
        }
    }
}