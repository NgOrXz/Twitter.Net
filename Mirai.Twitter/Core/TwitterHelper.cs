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
    using System.Globalization;
    using System.Text;

    internal static class TwitterHelper
    {
        /// <summary>
        /// Note: if conversion failed, return DateTime.MinValue.
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string dateTimeString)
        {
            DateTime dt;
            DateTime.TryParseExact(dateTimeString,
                                   "ddd MMM dd HH:mm:ss +0000 yyyy",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite,
                                   out dt);

            return dt;
        }

        public static double ToDouble(this string numberString)
        {
            double result;
            Double.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

            return result;
        }

        public static int ToInt32(this string numberString)
        {
            int result;
            Int32.TryParse(numberString, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

            return result;
        }
        
        // Extracted from fastJSON lib.
        internal static string ToJsonString(this string s)
        {
            var output = new StringBuilder();
            output.Append('\"');

            var runIndex = -1;

            for (var index = 0; index < s.Length; ++index)
            {
                var c = s[index];

                if (c >= ' ' && c < 128 && c != '\"' && c != '\\')
                {
                    if (runIndex == -1)
                    {
                        runIndex = index;
                    }

                    continue;
                }

                if (runIndex != -1)
                {
                    output.Append(s, runIndex, index - runIndex);
                    runIndex = -1;
                }

                switch (c)
                {
                    case '\t':
                        output.Append("\\t");
                        break;
                    case '\r':
                        output.Append("\\r");
                        break;
                    case '\n':
                        output.Append("\\n");
                        break;
                    case '"':
                    case '\\':
                        output.Append('\\');
                        output.Append(c);
                        break;
                    default:
                        output.Append("\\u");
                        output.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                }
            }

            if (runIndex != -1)
            {
                output.Append(s, runIndex, s.Length - runIndex);
            }

            output.Append('\"');

            return output.ToString();
        }
    }
}
