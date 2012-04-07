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

namespace Mirai.Social.Twitter.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [Serializable]
    public class TwitterException : Exception
    {
        public TwitterError Error
        {
            get { return this.Errors != null ? this.Errors[0] : null; }
        }

        public TwitterError[] Errors { get; internal set; }

        public string Response { get; private set; }

        public HttpStatusCode StatusCode { get; internal set; }


        public TwitterException(string message) : this(message, null)
        {
            
        }

        public TwitterException(string message, Exception innerException)
            : base(message, innerException)
        {
            var webException = innerException as WebException;
            if (webException == null)
            {
                return;
            }

            var httpWebRresponse    = (HttpWebResponse)webException.Response;
            this.StatusCode         = httpWebRresponse.StatusCode;

            using (var streamReader = new StreamReader(httpWebRresponse.GetResponseStream()))
            {
                this.Response   = streamReader.ReadToEnd();
                var jsonObj     = JObject.Parse(this.Response);

                if (jsonObj["errors"] == null)
                {
                    var serializer = new JsonSerializer();
                    this.Errors = new[]
                        {
                            (TwitterError)serializer.Deserialize(new JTokenReader(jsonObj), typeof(TwitterError))
                        };
                }
                else
                {
                    this.Errors = (from o in jsonObj["errors"]
                                   select new TwitterError
                                       {
                                           Code = (int)o["code"], 
                                           Message = (string)o["message"]
                                       }).ToArray();
                }
            }
        }
    }
}
