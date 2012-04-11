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

    public enum TokenType
    {
        AccessToken,

        InvalidToken,

        RequestToken
    }

    /// <summary>
    /// The token credential.
    /// </summary>
    public class TokenCredential
    {
        #region Constants and Fields

        private string _Secret;
        private string _Token;
        private TokenType _TokenType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes an invalid token.
        /// </summary>
        public TokenCredential() : this("", "", TokenType.InvalidToken)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="tokenType"></param>
        public TokenCredential(string token, string secret, TokenType tokenType)
        {
            this.SetToken(token, secret, tokenType);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Token Secret.
        /// </summary>
        public string Secret
        {
            get { return this._Secret; }
        }

        /// <summary>
        /// Gets the Token.
        /// </summary>
        public string Token
        {
            get { return this._Token; }
        }

        /// <summary>
        /// Gets the Token type.
        /// </summary>
        public TokenType TokenType
        {
            get { return this._TokenType; }
        }

        #endregion

        #region Public Methods

        public void SetToken(string token, string secret, TokenType tokenType)
        {
            if (token == null)
                throw new ArgumentNullException("token");
            if (secret == null)
                throw new ArgumentNullException("secret");
            if (token != String.Empty && secret != String.Empty && tokenType == TokenType.InvalidToken)
                throw new ArgumentException("The token and secret is not empty, " +
                                            "but the token type set to TokenType.InvalidToken.");
            if ((token == String.Empty || secret == String.Empty) && tokenType != TokenType.InvalidToken)
                throw new ArgumentException("Invalid token or secret.");

            this._Secret    = secret;
            this._Token     = token;
            this._TokenType = tokenType;
        }

        #endregion
    }

    /// <summary>
    /// The consumer credential.
    /// </summary>
    public sealed class ConsumerCredential : TokenCredential
    {
        #region Constants and Fields

        /// <summary>
        /// The consumer key.
        /// </summary>
        private string _ConsumerKey;

        /// <summary>
        /// The consumer secret.
        /// </summary>
        private string _ConsumerSecret;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerCredential"/> class.
        /// </summary>
        /// <param name="consumerKey">
        /// The consumer key.
        /// </param>
        /// <param name="consumerSecret">
        /// The consumer secret.
        /// </param>
        public ConsumerCredential(string consumerKey, string consumerSecret)
        {
            this.ConsumerKey    = consumerKey;
            this.ConsumerSecret = consumerSecret;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public string ConsumerKey
        {
            get { return this._ConsumerKey; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("The consumer key can not be null or empty.", "value");
                }

                this._ConsumerKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the consumer secret.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public string ConsumerSecret
        {
            get { return this._ConsumerSecret; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("The consumer secret can not be null or empty", "value");
                }

                this._ConsumerSecret = value;
            }
        }

        #endregion
    }
}