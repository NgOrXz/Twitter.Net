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

namespace Mirai.Social.Twitter.TwitterObjects
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The twitter color.
    /// </summary>
    public struct TwitterColor
    {
        #region Constants and Fields

        /// <summary>
        /// The a.
        /// </summary>
        public byte A;

        /// <summary>
        /// The b.
        /// </summary>
        public byte B;

        /// <summary>
        /// The g.
        /// </summary>
        public byte G;

        /// <summary>
        /// The r.
        /// </summary>
        public byte R;

        #endregion

        #region Public Methods

        /// <summary>
        /// The from argb.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// </returns>
        public static TwitterColor FromArgb(byte a, byte r, byte g, byte b)
        {
            TwitterColor result = default(TwitterColor);
            result.A            = a;
            result.B            = b;
            result.G            = g;
            result.R            = r;

            return result;
        }

        /// <summary>
        /// The from rgb.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// </returns>
        public static TwitterColor FromRgb(byte r, byte g, byte b)
        {
            return FromArgb(255, r, g, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">
        /// 
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="FormatException">
        /// </exception>
        public static TwitterColor FromString(string input)
        {
            if (input.Length < 6)
                input = input.PadLeft(6, '0');

            var pattern = @"^#?(?<r>[0-9A-Fa-f]{2})(?<g>[0-9A-Fa-f]{2})(?<b>[0-9A-Fa-f]{2})$";
            var match   = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            var isRgb   = match.Success;

            if (!isRgb)
            {
                pattern = @"^#?(?<a>[0-9A-Fa-f]{2})(?<r>[0-9A-Fa-f]{2})(?<g>[0-9A-Fa-f]{2})(?<b>[0-9A-Fa-f]{2})$";
                match   = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    throw new FormatException(
                        "The format is incorrect, expected format: #RRGGBB, RRGGBB, " +
                        "#AARRGGBB or AARRGGBB.");
                }
            }

            TwitterColor result = default(TwitterColor);
            result.A = isRgb ? (byte)255 : 
                               byte.Parse(match.Groups["a"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            result.B = Byte.Parse(match.Groups["b"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            result.G = Byte.Parse(match.Groups["g"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            result.R = Byte.Parse(match.Groups["r"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// RRGGBB
        /// </returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendFormat("{0:X2}{1:X2}{2:X2}", this.R, this.G, this.B);

            return result.ToString();
        }

        #endregion
    }
}