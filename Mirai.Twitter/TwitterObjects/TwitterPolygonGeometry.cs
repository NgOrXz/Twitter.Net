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

    public sealed class TwitterPolygonGeometry : TwitterGeometry
    {
        #region Public Methods

        public void Add(TwitterCoordinate coordinate)
        {
            this.CoordinatesList.Add(coordinate);
        }

        public void Clear()
        {
            this.CoordinatesList.Clear();
        }

        public static TwitterPolygonGeometry FromDictionary(IDictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterPolygonGeometry>(dictionary);
        }

        public static TwitterPolygonGeometry Parse(string jsonString)
        {
            return Parse<TwitterPolygonGeometry>(jsonString);
        }

        public bool Remove(TwitterCoordinate coordinate)
        {
            return this.CoordinatesList.Remove(coordinate);
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

            if (!((string)typeValue).Equals("Polygon", StringComparison.OrdinalIgnoreCase))
                throw new FormatException();

            foreach (ArrayList polyArray in (ArrayList)coordinatesValue)
            {
                foreach (ArrayList c in polyArray)
                {
                    this.CoordinatesList.Add(new TwitterCoordinate(c[0].ToString().ToDouble(),
                                                                   c[1].ToString().ToDouble()));
                }
                
            }
        }

        public override string ToJsonString()
        {
            if (this.IsCoordinate)
                throw new NotImplementedException();

            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"type\":\"Polygon\",\"coordinates\":[[");
            this.CoordinatesList.ForEach(c => jsonBuilder.AppendFormat("{0},", c.ToJsonString()));
            if (jsonBuilder[jsonBuilder.Length - 1] == ',')
                jsonBuilder.Length -= 1;

            jsonBuilder.Append("]]}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}