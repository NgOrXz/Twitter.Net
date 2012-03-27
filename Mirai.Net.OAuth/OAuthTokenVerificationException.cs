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
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class OAuthTokenVerificationException : Exception
    {
        private readonly string _CurrentToken;
        private readonly string _ReceivedToken;

        public string CurrentToken
        {
            get { return this._CurrentToken; }
        }

        public string ReceivedToken
        {
            get { return this._ReceivedToken; }
        }

        #region Constructors and Destructors

        public OAuthTokenVerificationException(string message, string currentToken, string receivedToken) 
            : base(message)
        {
            if (String.IsNullOrEmpty(currentToken))
                throw new ArgumentNullException("currentToken");
            if (String.IsNullOrEmpty(receivedToken))
                throw new ArgumentNullException("receivedToken");

            this._CurrentToken  = currentToken;
            this._ReceivedToken = receivedToken;
        }

        private OAuthTokenVerificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._CurrentToken  = info.GetString("CurrentToken");
            this._ReceivedToken = info.GetString("ReceivedToken");
        }

        #endregion

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("CurrentToken", this._CurrentToken);
            info.AddValue("ReceivedToken", this._ReceivedToken);
        }
    }
}
