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
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Mirai.Twitter.Core;

    public sealed class TwitterCursorPagedIdCollection : TwitterCursorPagedCollection<string>
    {
        #region Public Properties

        [TwitterKey("ids")]
        public string[] Ids
        {
            get { return this.ToArray(); }
            set
            {
                this.Clear();
                this.AddRange(value);
            }
        }

        #endregion



        #region Public Methods

        public static TwitterCursorPagedIdCollection FromDictionary(Dictionary<string, object> dictionary)
        {
            return FromDictionary<TwitterCursorPagedIdCollection>(dictionary);
        }

        public static TwitterCursorPagedIdCollection Parse(string jsonString)
        {
            return Parse<TwitterCursorPagedIdCollection>(jsonString);
        }

        #endregion


        #region Overrides of TwitterObject

        internal override void Init(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (dictionary.Count == 0)
                return;

            var pis = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(this, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(string[]))
                {
                    var data = (from string id in (ArrayList)value select id).ToArray();

                    propertyInfo.SetValue(this, data, null);
                }
            }
        }

        public override string ToJsonString()
        {
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            var pis = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || (value = propertyInfo.GetValue(this, null)) == null)
                    continue;

                jsonBuilder.AppendFormat("\"{0}\":", twitterKey.Key);

                if (propertyInfo.PropertyType == typeof(string))
                    jsonBuilder.AppendFormat("\"{0}\",", value);
                else if (propertyInfo.PropertyType == typeof(string[]))
                {
                    jsonBuilder.AppendFormat("[{0}],", String.Join(",", (string[])value));
                }
            }

            jsonBuilder.Length -= 1; // Remove trailing ',' char.
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        #endregion
    }
}