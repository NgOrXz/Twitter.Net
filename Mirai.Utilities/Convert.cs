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

namespace Mirai.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    public static class Convert
    {
        /// <summary>
        /// Converts a comma-separated string to an enumerated type(bit flags).
        /// </summary>
        /// <typeparam name="T">Must be a enumerated type.</typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <remarks>This method handles EnumMemberAttribute attribute.</remarks>
        public static T ToEnum<T>(string input) where T : struct
        {
            var flags = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return ToEnum<T>(flags);
        }

        /// <summary>
        /// Converts a collection of strings to an enumerated type(bit flags).
        /// </summary>
        /// <typeparam name="T">Must be a enumerated type.</typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <remarks>This method handles EnumMemberAttribute attribute.</remarks>
        public static T ToEnum<T>(IEnumerable<string> input) where T : struct
        {
            if (!(typeof(T).IsEnum))
                throw new NotSupportedException();

            var enumAttrMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var fieldInfo in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = Attribute.GetCustomAttribute(fieldInfo, typeof(EnumMemberAttribute));
                enumAttrMapping.Add(attr != null ?
                                        ((EnumMemberAttribute)attr).Value :
                                        fieldInfo.Name, fieldInfo.Name);
            }

            var bitFlagBuilder = new StringBuilder();
            input.ToList().ForEach(bitFlagName =>
                {
                    if (!enumAttrMapping.ContainsKey(bitFlagName))
                        return;

                    bitFlagBuilder.AppendFormat("{0}, ", enumAttrMapping[bitFlagName]);
                });
            bitFlagBuilder.Length -= 2; // Trim ending ',' and ' ' characters.

            var result = (T)Enum.Parse(typeof(T), bitFlagBuilder.ToString(), true);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <remarks>This method handles EnumMemberAttribute attribute.</remarks>
        public static string ToString<T>(Enum value, string separator = ", ") where T: struct 
        {
            if (!(typeof(T).IsEnum))
                throw new NotSupportedException();

            var existingValues = value.ToString("F").Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new List<string>();
            foreach (var fieldInfo in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (!existingValues.Contains(fieldInfo.Name))
                    continue;

                var attr = Attribute.GetCustomAttribute(fieldInfo, typeof(EnumMemberAttribute));
                result.Add(attr != null ? ((EnumMemberAttribute)attr).Value : fieldInfo.Name);
            }

            return String.Join(separator, result);
        }
    }
}
