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

namespace Mirai.Net.OAuth
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ServiceProviderDescription
    {
        private OAuthEndPoint _RequestTokenEndPoint;
        private OAuthEndPoint _AuthorizationEndPoint;
        private OAuthEndPoint _AccessTokenEndPoint;

        #region Public Properties

        public OAuthEndPoint RequestTokenEndPoint
        {
            get { return this._RequestTokenEndPoint; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                this._RequestTokenEndPoint = value;
            }
        }

        public OAuthEndPoint AuthorizationEndPoint
        {
            get { return this._AuthorizationEndPoint; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                this._AuthorizationEndPoint = value;
            }
        }

        public OAuthEndPoint AccessTokenEndPoint
        {
            get { return this._AccessTokenEndPoint; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                this._AccessTokenEndPoint = value;
            }
        }

        public ProtocolVersion ProtocolVersion { get; set; }

        #endregion

        #region Constructors and Destructors

        public ServiceProviderDescription(OAuthEndPoint requestTokenEndPoint,
                                          OAuthEndPoint authorizationEndPoint, 
                                          OAuthEndPoint accessTokenEndPoint, 
                                          ProtocolVersion protocolVersion = ProtocolVersion.V10)
        {
            this.RequestTokenEndPoint   = requestTokenEndPoint;
            this.AuthorizationEndPoint  = authorizationEndPoint;
            this.AccessTokenEndPoint    = accessTokenEndPoint;
            this.ProtocolVersion        = protocolVersion;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class OAuthEndPoint
    {
        private Uri _Uri;
        private HttpMethod _HttpMethod;

        public Uri ResourceUri
        {
            get { return this._Uri; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Scheme != Uri.UriSchemeHttp && value.Scheme != Uri.UriSchemeHttps)
                    throw new ArgumentException("Invalid Uri schema");

                this._Uri = value;
            }
        }

        public HttpMethod HttpMethod
        {
            get { return this._HttpMethod; }
            set { this._HttpMethod = value; }
        }

        public OAuthEndPoint(string resourceUri, HttpMethod httpMethod = HttpMethod.Post)
            : this(new Uri(resourceUri, UriKind.Absolute), httpMethod)
        {
        }

        public OAuthEndPoint(Uri resourceUri, HttpMethod httpMethod = HttpMethod.Post)
        {
            this.ResourceUri    = resourceUri;
            this._HttpMethod    = httpMethod;
        }
    }
}
