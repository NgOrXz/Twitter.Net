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

namespace Mirai.Social.Twitter.TwitterObjects
{
    using System;
    using System.Collections.Generic;

    using Mirai.Utilities;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Convert = Mirai.Utilities.Convert;

    class TwitterRelationshipConnectionsConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string result = null;

            if (value is TwitterRelationshipConnections)
            {
                var existingValues = 
                    Convert.ToString<TwitterRelationshipConnections>((TwitterRelationshipConnections)value);

                var jArray  = JArray.FromObject(existingValues.Split(
                                                            new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                result      = jArray.ToString();
            }

            writer.WriteValue(result);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(TwitterRelationshipConnections) && 
                objectType != typeof(TwitterRelationshipConnections?))
                throw new NotSupportedException();

            if (reader.TokenType == JsonToken.Null)
            {
                if (!reader.ValueType.IsNullableType())
                    throw new NotSupportedException();

                return null;
            }

            var values = new List<string>();
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                values.Add(reader.Value.ToString());
            }
            
            return Convert.ToEnum<TwitterRelationshipConnections>(values);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TwitterRelationshipConnections) ||
                   objectType == typeof(TwitterRelationshipConnections?);
        }

        #endregion
    }
}