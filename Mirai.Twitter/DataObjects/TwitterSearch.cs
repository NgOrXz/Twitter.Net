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


namespace Mirai.Twitter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Utilities.Text;

    using fastJSON;

    public sealed class TwitterSearch
    {
        [TwitterKey("completed_in")]
        public double CompletedIn { get; set; }

        [TwitterKey("max_id_str")]
        public string MaxId { get; set; }

        [TwitterKey("next_page")]
        public string NextPage { get; set; }

        [TwitterKey("page")]
        public int Page { get; set; }

        [TwitterKey("query")]
        public string Query { get; set; }

        [TwitterKey("refresh_url")]
        public string RefreshUrl { get; set; }

        [TwitterKey("results")]
        public TwitterSearchResult[] Results { get; set; }

        [TwitterKey("results_per_page")]
        public int ResultsPerPage { get; set; }

        [TwitterKey("since_id")]
        public string SinceId { get; set; }
    }
}
