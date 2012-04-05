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
    using System.Globalization;

    using Mirai.Utilities.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    internal class TwitterDateTimeConverter : DateTimeConverterBase
    {
        protected string DateTimeFormat;


        public TwitterDateTimeConverter()
        {
            this.DateTimeFormat = "ddd MMM dd HH:mm:ss +0000 yyyy";
        }


        public override bool CanConvert(Type typeObject)
        {
            return typeObject == typeof(DateTime) || typeObject == typeof(DateTime?);
        }

        public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(DateTime) && objectType != typeof(DateTime?))
                throw new NotSupportedException();

            if (reader.TokenType == JsonToken.Null)
            {
                if (!reader.ValueType.IsNullableType())
                    throw new NotSupportedException();

                return null;
            }

            DateTime dt;
            DateTime.TryParseExact(reader.Value.ToString(),
                                   this.DateTimeFormat,
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite,
                                   out dt);

            return dt;
        }

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            string dateTimeString = null;

            if (value is DateTime)
                dateTimeString = ((DateTime)value).ToString(this.DateTimeFormat);

            writer.WriteValue(dateTimeString);
        }
    }
}
