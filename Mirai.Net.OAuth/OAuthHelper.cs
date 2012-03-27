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
    using System.Text;
    using System.Globalization;
    using System.Text.RegularExpressions;

    internal static class OAuthHelper
    {
        internal static string EnumToString(this SignatureMethod signatureMethod)
        {
            var result = String.Empty;
            switch (signatureMethod)
            {
                case SignatureMethod.HmacSha1:
                    result = "HMAC-SHA1";
                    break;
                case SignatureMethod.RsaSha1:
                    result = "RSA-SHA1";
                    break;
                case SignatureMethod.Plaintext:
                    result = "PLAINTEXT";
                    break;
            }

            return result;
        }

        internal static string EnumToString(this ProtocolVersion protocolVersion)
        {
            var result = String.Empty;
            switch (protocolVersion)
            {
                case ProtocolVersion.V10:
                    result = "1.0";
                    break;
                case ProtocolVersion.V10A:
                    result = "1.0a";
                    break;
                case ProtocolVersion.V20:
                    result = "2.0";
                    break;
            }

            return result;
        }

        internal static Uri ToBaseUri(this Uri uri)
        {
            var uriBuilder = new UriBuilder(uri.Scheme + Uri.SchemeDelimiter + uri.Authority + uri.LocalPath);

            return uriBuilder.Uri;
        }

        internal static string ToPercentEncode(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;

            // RFC 3986 unreserved characters
            // a-z A-Z 0-9 - _ . ~

            // RFC 3986 reserved characters
            // ":", "/", "?", "#", "[", "]", "@"
            // "!", "$", "&", "'", "(", ")"
            // "*", "+", ",", ";", "="

            var encoded     = new StringBuilder();
            var charsToSkip = "[0-9a-zA-Z-_.~%]";

            foreach (char c in Uri.EscapeDataString(input))
            {
                if (Regex.IsMatch(c.ToString(CultureInfo.InvariantCulture), charsToSkip))
                    encoded.Append(c);
                else
                    encoded.Append(Uri.HexEscape(c));
            }

            return encoded.ToString();
        }
    }
}
