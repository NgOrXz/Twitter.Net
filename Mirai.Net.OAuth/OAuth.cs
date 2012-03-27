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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    using Mirai.Utilities.Text;

    using Wintellect.PowerCollections;

    /// <summary>
    /// The o auth.
    /// </summary>
    public sealed class OAuth
    {
        #region Constants and Fields

        private static readonly string s_Boundary = "----------5rex6Zuq5pyq5p2l";

        /// <summary>
        /// Represents oauth_callback protocol parameter.
        /// </summary>
        public static readonly string OAuthCallback = "oauth_callback";

        /// <summary>
        /// Represents oauth_consumer_key protocol parameter.
        /// </summary>
        public static readonly string OAuthConsumerKey = "oauth_consumer_key";

        /// <summary>
        /// Represents oauth_nonce protocol parameter.
        /// </summary>
        public static readonly string OAuthNonce = "oauth_nonce";

        /// <summary>
        /// Represents oauth_signature protocol parameter.
        /// </summary>
        public static readonly string OAuthSignature = "oauth_signature";

        /// <summary>
        /// Represents oauth_signature_method protocol parameter.
        /// </summary>
        public static readonly string OAuthSignatureMethod = "oauth_signature_method";

        /// <summary>
        /// Represents oauth_timestamp protocol parameter.
        /// </summary>
        public static readonly string OAuthTimestamp = "oauth_timestamp";

        /// <summary>
        /// Represents oauth_token protocol parameter.
        /// </summary>
        public static readonly string OAuthToken = "oauth_token";

        /// <summary>
        /// Represents oauth_verifier protocol parameter.
        /// </summary>
        public static readonly string OAuthVerifier = "oauth_verifier";

        /// <summary>
        /// Represents oauth_version protocol parameter.
        /// </summary>
        public static readonly string OAuthVersion = "oauth_version";

        /// <summary>
        /// The consumer credential.
        /// </summary>
        private readonly ConsumerCredential _ConsumerCredential;

        /// <summary>
        /// The realm.
        /// </summary>
        private string _Realm;

        /// <summary>
        /// The service provider description.
        /// </summary>
        private readonly ServiceProviderDescription _ServiceProviderDescription;

        /// <summary>
        /// The OAuth signature method.
        /// </summary>
        private readonly SignatureMethod _SignatureMethod;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth"/> class.
        /// </summary>
        /// <param name="consumerCredential">
        /// The consumer credential.
        /// </param>
        /// <param name="serviceProviderDescription">
        /// The service provider description.
        /// </param>
        /// <param name="signatureMethod">
        /// The signature method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public OAuth(
                    ConsumerCredential consumerCredential,
                    ServiceProviderDescription serviceProviderDescription,
                    SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            if (consumerCredential == null)
            {
                throw new ArgumentNullException("consumerCredential");
            }

            if (serviceProviderDescription == null)
            {
                throw new ArgumentNullException("serviceProviderDescription");
            }

            this._Realm                 = String.Empty;
            this._ConsumerCredential    = consumerCredential;
            this._SignatureMethod       = signatureMethod;
            this._ServiceProviderDescription = serviceProviderDescription;

            this.UserAgent = "Mirai";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets AccessTokenEndPoint.
        /// </summary>
        public OAuthEndPoint AccessTokenEndPoint
        {
            get { return this._ServiceProviderDescription.AccessTokenEndPoint; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Authenticated
        {
            get { return this._ConsumerCredential.TokenType == TokenType.AccessToken; }
        }

        /// <summary>
        /// Gets AuthorizationEndPoint.
        /// </summary>
        public OAuthEndPoint AuthorizationEndPoint
        {
            get { return this._ServiceProviderDescription.AuthorizationEndPoint; }
        }

        /// <summary>
        /// Gets or sets the Realm.
        /// </summary>
        public string Realm
        {
            get { return this._Realm; }
            set
            {
                if (value == null)
                {
                    this._Realm = String.Empty;
                    return;
                }

                this._Realm = value;
            }
        }

        /// <summary>
        /// Gets RequestTokenEndPoint.
        /// </summary>
        public OAuthEndPoint RequestTokenEndPoint
        {
            get { return this._ServiceProviderDescription.RequestTokenEndPoint; } 
        }

        /// <summary>
        /// Gets SignatureMethod.
        /// </summary>
        public SignatureMethod SignatureMethod
        {
            get { return this._SignatureMethod; }
        }

        /// <summary>
        /// Gets Token.
        /// </summary>
        public string Token
        {
            get { return this._ConsumerCredential.Token; }
        }

        /// <summary>
        /// Gets TokenSecret.
        /// </summary>
        public string TokenSecret
        {
            get { return this._ConsumerCredential.Secret; }
        }

        /// <summary>
        /// Gets or sets the web proxy.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        public string UserAgent { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Acquires the access token for the non Pin-based flow authentication.
        /// </summary>
        /// <param name="postData"></param>
        public void AcquireAccessToken(IEnumerable<KeyValuePair<string, string>> postData)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
                throw new InvalidOperationException("This method can only be used in the non Pin-based flow authentication.");
            
            var requestUri = httpContext.Request.Url.ToString();

            var pattern = @"oauth_token=(?<token>[^&]+)&oauth_verifier=(?<verifier>[^&]+)";
            var match   = Regex.Match(requestUri, pattern, RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new OAuthResponseFormatException("The server's request is unrecognized.", requestUri);

            var token = match.Groups["token"].Value;
            if (!token.Equals(this.Token, StringComparison.Ordinal))
                throw new OAuthTokenVerificationException("Request token validation failed.", this.Token, token);

            var oauthVerifier = match.Groups["verifier"].Value;
            this.AcquireAccessToken(oauthVerifier, postData);
        }

        /// <summary>
        /// Acquires the access token.
        /// </summary>
        /// <param name="oauthVerifier">
        /// The oauth_verifier parameter.
        /// </param>
        /// <param name="postData">
        /// The extra postData. This parameter is ignored when HTTP method is set to GET / HEAD.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when oauthVerifier parameter is not specified.
        /// </exception>
        /// <exception cref="OAuthResponseFormatException">
        /// Thrown when the server returns a unrecognized content.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the currently stored token is not a request token.
        /// </exception>
        public void AcquireAccessToken(string oauthVerifier, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            string oauthParamsString;
            this.PrepareToAcquireAccessToken(oauthVerifier, postData, out oauthParamsString);

            var httpRequest     = this.PrepareRequest(
                                                        this._ServiceProviderDescription.AccessTokenEndPoint.ResourceUri,
                                                        this._ServiceProviderDescription.AccessTokenEndPoint.HttpMethod,
                                                        oauthParamsString,
                                                        postData);

            var webResp         = (HttpWebResponse)httpRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(webResp.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            var pattern = @"oauth_token=(?<token>[^&]+)&oauth_token_secret=(?<secret>[^&]+)";
            var match   = Regex.Match(result, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new OAuthResponseFormatException("The server returns a unrecognized content.", result);
            }
            this._ConsumerCredential.SetToken(
                                                match.Groups["token"].Value,
                                                match.Groups["secret"].Value,
                                                TokenType.AccessToken);
        }

        /// <summary>
        /// Acquires the request token.
        /// </summary>
        /// <param name="oauthCallback">The oauth_callback parameter.</param>
        /// <param name="postData">
        /// The extra postData. This parameter is ignored when HTTP method is set to GET / HEAD.
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when callback hasn'postDataList specified.
        /// </exception>
        /// <exception cref="OAuthResponseFormatException">
        /// Thrown when the server returns a unrecognized content
        /// or
        /// the server returns false for the oauth_callback_confirmed parameter.
        /// </exception>
        public Uri AcquireRequestToken(string oauthCallback, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            string oauthParamsString;
            this.PrepareToAcquireRequestToken(oauthCallback, postData, out oauthParamsString);

            var httpRequest     = this.PrepareRequest(
                                                this._ServiceProviderDescription.RequestTokenEndPoint.ResourceUri,
                                                this._ServiceProviderDescription.RequestTokenEndPoint.HttpMethod,
                                                oauthParamsString, 
                                                postData);

            var webResp         = (HttpWebResponse)httpRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(webResp.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            var pattern = @"oauth_token=(?<token>[^&]+)&" +
                          @"oauth_token_secret=(?<secret>[^&]+)&" +
                          @"oauth_callback_confirmed=(?<confirmed>true|false)";
            var match   = Regex.Match(result, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new OAuthResponseFormatException("The server returns a unrecognized content.", result);
            }

            this._ConsumerCredential.SetToken(
                                                match.Groups["token"].Value,
                                                match.Groups["secret"].Value,
                                                TokenType.RequestToken);

            var confirmed = match.Groups["confirmed"].Value;
            if (confirmed.ToUpperInvariant() != "TRUE")
            {
                throw new OAuthException("The server returns false for the oauth_callback parameter.");
            }

            var uriBuiler = new UriBuilder(this._ServiceProviderDescription.AuthorizationEndPoint.ResourceUri);
            if (String.IsNullOrEmpty(uriBuiler.Query))
                uriBuiler.Query = "oauth_token=" + this.Token;
            else
                uriBuiler.Query = uriBuiler.Query.Substring(1) + "&" + this.Token;

            return uriBuiler.Uri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="postData">
        /// This parameter is ignored when HTTP method is set to GET / HEAD.
        /// </param>
        /// <returns></returns>
        public string ExecuteAuthenticatedRequest(Uri resourceUri,
                                                  HttpMethod httpMethod,
                                                  IDictionary<string, string> postData)
        {
            string oauthParamsString;
            this.PrepareToExecuteAuthenticatedRequest(resourceUri, httpMethod, postData, false, out oauthParamsString);

            var webRequest = this.PrepareRequest(resourceUri, httpMethod, oauthParamsString, postData);

            var webResp         = (HttpWebResponse)webRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(webResp.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string ExecuteAuthenticatedRequestForMultipartFormData(
                                                Uri resourceUrl, IEnumerable<KeyValuePair<string, object>> postData)
        {
            string oauthParamsString;
            this.PrepareToExecuteAuthenticatedRequest(resourceUrl, HttpMethod.Post, null, true, out oauthParamsString);

            var webRequest  = this.PrepareRequestForMultipartFormData(
                                                                        resourceUrl,
                                                                        HttpMethod.Post,
                                                                        oauthParamsString,
                                                                        postData,
                                                                        s_Boundary);

            var webResp         = (HttpWebResponse)webRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(webResp.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        #endregion

        #region Methods

        private static byte[] GenerateMultipartFormData(IEnumerable<KeyValuePair<string, object>> postData, string boundary)
        {
            if (String.IsNullOrEmpty(boundary))
                throw new ArgumentException("The multipart boundary must be specified.", "boundary");

            if (postData == null)
                return new byte[0];

            var formDataStream = new MemoryStream();
            foreach (KeyValuePair<string, object> kvp in postData)
            {
                var data = kvp.Value as byte[];
                if (data != null)
                {
                    var partHeader = Encoding.UTF8.GetBytes(
                                                            String.Format(
                                                                            "--{0}\r\nContent-Disposition: form-data; " +
                                                                            "name=\"{1}\"; filename=\"{1}\"\r\n" +
                                                                            "Content-Type: application/octet-stream\r\n" +
                                                                            "Content-Transfer-Encoding: binary\r\n\r\n",
                                                                            boundary,
                                                                            kvp.Key));
                    formDataStream.Write(partHeader, 0, partHeader.Length);
                    formDataStream.Write(data, 0, data.Length);
                    formDataStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, 2);
                }
                else
                {
                    var partHeader = Encoding.UTF8.GetBytes(
                                                            String.Format(
                                                                            "--{0}\r\nContent-Disposition: form-data; " +
                                                                            "name=\"{1}\"; \r\n" +
                                                                            "Content-Type: text/plain\r\n\r\n{2}\r\n",
                                                                            boundary,
                                                                            kvp.Key,
                                                                            kvp.Value));
                    formDataStream.Write(partHeader, 0, partHeader.Length);
                }
            }

            var partFooter = Encoding.UTF8.GetBytes(String.Format("--{0}--\r\n", boundary));
            formDataStream.Write(partFooter, 0, partFooter.Length);
            formDataStream.Close();

            return formDataStream.ToArray();
        }

        /// <summary>
        /// The generate nonce.
        /// </summary>
        /// <returns>
        /// The generate nonce.
        /// </returns>
        private static string GenerateNonce()
        {
            return DateTime.Now.ToBinary().ToString("X", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Generates the signature base string.
        /// </summary>
        /// <param name="httpMethod">
        /// The http method.
        /// </param>
        /// <param name="baseUri">
        /// 
        /// </param>
        /// <param name="encodedRequestString">
        /// The encoded request string.
        /// </param>
        /// <returns>
        /// The signature base string.
        /// </returns>
        private static string GenerateSignatureBaseString(HttpMethod httpMethod, Uri baseUri, string encodedRequestString)
        {
            var baseString = new StringBuilder();
            baseString.AppendFormat(
                                    "{0}&{1}&{2}",
                                    httpMethod.ToString().ToUpperInvariant().ToPercentEncode(),
                                    baseUri.ToString().ToPercentEncode(),
                                    encodedRequestString.ToPercentEncode());

            return baseString.ToString();
        }

        /// <summary>
        /// The generate timestamp.
        /// </summary>
        /// <returns>
        /// The generate timestamp.
        /// </returns>
        private static string GenerateTimestamp()
        {
            var unixEpoch   = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timestamp   = (long)(DateTime.UtcNow - unixEpoch).TotalSeconds;

            return timestamp.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The generate request string.
        /// </summary>
        /// <param name="postData">
        /// The postData.
        /// </param>
        /// <returns>
        /// The generate request string.
        /// </returns>
        private static string GenerateRequestString(OrderedMultiDictionary<string, string> parameters)
        {
            var encodedRequestString = new StringBuilder();
            foreach (var kvp in parameters)
            {
                foreach (var v in kvp.Value)
                {
                    encodedRequestString.AppendFormat("{0}={1}&", kvp.Key.ToPercentEncode(), v.ToPercentEncode());
                }
            }

            encodedRequestString.Length -= 1;

            return encodedRequestString.ToString();
        }

        /// <summary>
        /// The generate signing key.
        /// </summary>
        /// <returns>
        /// The generate signing key.
        /// </returns>
        private string GenerateSigningKey()
        {
            var key = new StringBuilder();
            key.AppendFormat(
                            "{0}&{1}",
                            Uri.EscapeDataString(this._ConsumerCredential.ConsumerSecret),
                            string.IsNullOrEmpty(this._ConsumerCredential.Secret)
                                ? string.Empty
                                : Uri.EscapeDataString(this._ConsumerCredential.Secret));

            return key.ToString();
        }

        /// <summary>
        /// Generates the string of the protocol postData for use in authorization header.
        /// </summary>
        /// <param name="protocolParams">
        /// The protocol postData.
        /// </param>
        /// <returns>
        /// The string of the protocol postData.
        /// </returns>
        private string GenerateProtocolParametersString(OrderedMultiDictionary<string, string> protocolParams)
        {
            var oauthParamString = new StringBuilder();
            oauthParamString.AppendFormat("OAuth realm={0}, ", this._Realm);
            foreach (var kvp in protocolParams)
            {
                oauthParamString.AppendFormat(
                                                "{0}=\"{1}\", ",
                                                kvp.Key.ToPercentEncode(),
                                                kvp.Value.SingleOrDefault().ToPercentEncode());
            }

            oauthParamString.Length -= 2;

            return oauthParamString.ToString();
        }

        private static Dictionary<string, string> MergeInputParameters(
                                            Uri resourceUri, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var inputParams = new Dictionary<string, string>();

            if (resourceUri != null && !String.IsNullOrEmpty(resourceUri.Query) && resourceUri.Query.Length > 1)
            {
                var matches = Regex.Matches(resourceUri.Query, @"(?<name>[^?=&]+)=(?<value>[^?=&]+)");
                foreach (Match match in matches)
                {
                    inputParams.Add(match.Groups["name"].Value, match.Groups["value"].Value);
                }
            }

            if (postData != null)
            {
                foreach (KeyValuePair<string, string> kvp in postData)
                {
                    inputParams.Add(kvp.Key, kvp.Value);
                }
            }

            return inputParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="protocolParamsString"></param>
        /// <param name="postData">
        /// This parameter is ignored when HTTP method is set to GET / HEAD.
        /// </param>
        /// <returns></returns>
        private HttpWebRequest PrepareRequest(
                                                Uri resourceUri,
                                                HttpMethod httpMethod,
                                                string protocolParamsString,
                                                IEnumerable<KeyValuePair<string, string>> postData)
        {
            var webRequest          = this.PrepareRequestCore(resourceUri, httpMethod, protocolParamsString);
            webRequest.ContentType  = "application/x-www-form-urlencoded";

            if (httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head)
                return webRequest;

            if (postData != null && postData.Any())
            {
                var data = Encoding.UTF8.GetBytes(postData.ToNameValuePairString());
                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            return webRequest;
        }

        /// <summary>
        /// Prepares shared protocol postData.
        /// </summary>
        /// <returns>
        /// </returns>
        private OrderedMultiDictionary<string, string> PrepareCommonProtocolParameters()
        {
            var requestParams = new OrderedMultiDictionary<string, string>(
                                                                            false,
                                                                            StringComparer.Ordinal,
                                                                            StringComparer.Ordinal)
                {
                    { OAuthConsumerKey, this._ConsumerCredential.ConsumerKey }, 
                    { OAuthSignatureMethod, this._SignatureMethod.EnumToString() }, 
                    { OAuthTimestamp, GenerateTimestamp() }, 
                    { OAuthNonce, GenerateNonce() }, 
                    { OAuthVersion, this._ServiceProviderDescription.ProtocolVersion.EnumToString() }
                };

            return requestParams;
        }

        private HttpWebRequest PrepareRequestCore(Uri resourceUri, HttpMethod httpMethod, string protocolParamsString)
        {
            if (resourceUri == null)
                throw new ArgumentException("The resource Uri must be specified.", "resourceUri");
            if (String.IsNullOrEmpty(protocolParamsString))
                throw new ArgumentException("The oauth protocol's parameters must be provided.", "protocolParamsString");

            var webRequest          = (HttpWebRequest)WebRequest.Create(resourceUri);
            webRequest.Method       = httpMethod.ToString().ToUpperInvariant();
#if !DEBUG
            webRequest.Proxy        = this.Proxy;
#endif
            webRequest.UserAgent    = this.UserAgent;
            webRequest.Headers.Add(HttpRequestHeader.Authorization, protocolParamsString);

            return webRequest;
        }

        private HttpWebRequest PrepareRequestForMultipartFormData(
                                                                    Uri resourceUri,
                                                                    HttpMethod httpMethod,
                                                                    string protocolParamsString,
                                                                    IEnumerable<KeyValuePair<string, object>> postData,
                                                                    string boundary)
        {
            if (String.IsNullOrEmpty(boundary))
                throw new ArgumentException("Multipart form data boundary must be specified.", "boundary");

            var formData    = GenerateMultipartFormData(postData, boundary);
            var webRequest  = this.PrepareRequestCore(resourceUri, httpMethod, protocolParamsString);

            webRequest.ContentType      = "multipart/form-data; boundary=" + boundary;
            webRequest.ContentLength    = formData.Length;

            webRequest.ServicePoint.Expect100Continue = false;

            if (formData.Length > 0)
            {
                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(formData, 0, formData.Length);
                }
            }

            return webRequest;
        }

        private void PrepareToAcquireAccessToken(
                                                    string oauthVerifier,
                                                    IEnumerable<KeyValuePair<string, string>> postData,
                                                    out string protocolParamsString)
        {
            if (string.IsNullOrEmpty(oauthVerifier))
            {
                throw new ArgumentException("The oauth_verifier parameter must be specified.", "oauthVerifier");
            }
            if (String.IsNullOrEmpty(this._ConsumerCredential.ConsumerKey))
                throw new InvalidOperationException("Invalid consumer key.");
            if (String.IsNullOrEmpty(this._ConsumerCredential.ConsumerSecret))
                throw new InvalidOperationException("Invalid consumer secret.");
            if (this._ConsumerCredential.TokenType != TokenType.RequestToken)
                throw new InvalidOperationException("The current token is not a request token.");

            var protocolParams = this.PrepareCommonProtocolParameters();
            protocolParams.Add(OAuthToken, this._ConsumerCredential.Token);
            protocolParams.Add(OAuthVerifier, oauthVerifier);

            var inputParams = MergeInputParameters(
                                                    this._ServiceProviderDescription.AccessTokenEndPoint.ResourceUri,
                                                    postData);

            var requestParams = protocolParams.Clone();
            if (inputParams.Count > 0)
            {
                foreach (var kvp in inputParams)
                {
                    requestParams.Add(kvp.Key, kvp.Value);
                }
            }

            var encodedRequestString = GenerateRequestString(requestParams);
            var signatureBaseString  = GenerateSignatureBaseString(
                                                    this._ServiceProviderDescription.AccessTokenEndPoint.HttpMethod,
                                                    this._ServiceProviderDescription.AccessTokenEndPoint.ResourceUri.ToBaseUri(),
                                                    encodedRequestString);

            protocolParams.Add(OAuthSignature, this.SignSignatureBaseString(signatureBaseString));

            protocolParamsString = this.GenerateProtocolParametersString(protocolParams);
        }

        private void PrepareToAcquireRequestToken(
                                                    string oauthCallback,
                                                    IEnumerable<KeyValuePair<string, string>> postData,
                                                    out string protocolParamsString)
        {
            if (String.IsNullOrEmpty(oauthCallback))
            {
                throw new ArgumentException("The callback must be specified.", "oauthCallback");
            }
            if (String.IsNullOrEmpty(this._ConsumerCredential.ConsumerKey))
                throw new InvalidOperationException("Invalid consumer key.");
            if (String.IsNullOrEmpty(this._ConsumerCredential.ConsumerSecret))
                throw new InvalidOperationException("Invalid consumer secret.");

            var protocolParams  = this.PrepareCommonProtocolParameters();
            protocolParams.Add(OAuthCallback, oauthCallback);

            var inputParams     = MergeInputParameters(
                                                        this._ServiceProviderDescription.RequestTokenEndPoint.ResourceUri,
                                                        postData);

            string encodedRequestString;
            if (inputParams.Count <= 0)
            {
                encodedRequestString = GenerateRequestString(protocolParams);
            }
            else
            {
                var requestParams = protocolParams.Clone();
                foreach (var kvp in inputParams)
                {
                    requestParams.Add(kvp.Key, kvp.Value);
                }
                encodedRequestString = GenerateRequestString(requestParams);
            }

            var signatureBaseString = GenerateSignatureBaseString(
                                                this._ServiceProviderDescription.RequestTokenEndPoint.HttpMethod,
                                                this._ServiceProviderDescription.RequestTokenEndPoint.ResourceUri.ToBaseUri(),
                                                encodedRequestString);

            protocolParams.Add(OAuthSignature, this.SignSignatureBaseString(signatureBaseString));

            protocolParamsString = this.GenerateProtocolParametersString(protocolParams);
        }

        private void PrepareToExecuteAuthenticatedRequest(
                                                            Uri resourceUri,
                                                            HttpMethod httpMethod,
                                                            IEnumerable<KeyValuePair<string, string>> postData, 
                                                            bool isMultipart,
                                                            out string protocolParamsString)
        {
            if (resourceUri == null)
            {
                throw new ArgumentNullException("resourceUri");
            }
            if (this._ConsumerCredential.TokenType != TokenType.AccessToken)
                throw new InvalidOperationException("The current token is not a access token.");

            var sharedProtocolParams = this.PrepareCommonProtocolParameters();
            sharedProtocolParams.Add(OAuthToken, this._ConsumerCredential.Token);

            var requestParams = sharedProtocolParams.Clone();
            if (!isMultipart)
            {
                var ignorePostData = httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head;
                foreach (var kvp in MergeInputParameters(resourceUri, ignorePostData ? null : postData))
                {
                    requestParams.Add(kvp.Key, kvp.Value);
                }
            }

            // When in multipart/form-data mode, only oauth_* protocol parameters included in signature base string.

            var encodedRequestString    = GenerateRequestString(requestParams);
            var signatureBaseString     = GenerateSignatureBaseString(
                                                                    httpMethod,
                                                                    resourceUri.ToBaseUri(),
                                                                    encodedRequestString);

            sharedProtocolParams.Add(OAuthSignature, this.SignSignatureBaseString(signatureBaseString));

            protocolParamsString = this.GenerateProtocolParametersString(sharedProtocolParams);
        }

        /// <summary>
        /// The sign signature base string.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="signatureBaseString">
        /// The signature base string.
        /// </param>
        /// <param name="signatureMethod">
        /// The signature method.
        /// </param>
        /// <returns>
        /// The sign signature base string.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private string SignSignatureBaseString(string signatureBaseString)
        {
            var signature = string.Empty;
            if (this._SignatureMethod == SignatureMethod.HmacSha1)
            {
                HMACSHA1 hmacSha1   = new HMACSHA1(Encoding.UTF8.GetBytes(this.GenerateSigningKey()));
                byte[] hashValue    = hmacSha1.ComputeHash(Encoding.UTF8.GetBytes(signatureBaseString));
                signature = Convert.ToBase64String(hashValue);
            }

            if (this._SignatureMethod == SignatureMethod.RsaSha1 || this._SignatureMethod == SignatureMethod.Plaintext)
            {
                throw new NotImplementedException();
            }

            return signature;
        }

        #endregion
    }
}