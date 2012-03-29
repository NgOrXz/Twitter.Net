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

namespace Mirai.Twitter.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using fastJSON;

    [Serializable]
    public class TwitterException : Exception
    {
        public TwitterError Error { get; internal set; }

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

            var response    = (HttpWebResponse)webException.Response;
            this.StatusCode = response.StatusCode;

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var jsonObj = (Dictionary<string, object>)JSON.Instance.Parse(streamReader.ReadToEnd());
                this.Error = TwitterError.FromDictionary(jsonObj);
            }
        }
    }
}
