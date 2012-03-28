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

namespace Mirai.Twitter.TwitterObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Mirai.Twitter.Core;

    public abstract class TwitterCursorPagedCollection<T> : IList<T> where T : class 
    {
        private List<T> _Elements;


        [TwitterKey("next_cursor_str")]
        public string NextCursor { get; set; }

        [TwitterKey("previous_cursor_str")]
        public string PreviousCursor { get; set; }


        #region Constructors and Destructors

        protected TwitterCursorPagedCollection()
        {
            this._Elements = new List<T>();
        }

        #endregion

        #region Public Methods

        public int IndexOf(T item)
        {
            return this._Elements.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this._Elements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this._Elements.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return this._Elements[index];
            }
            set
            {
                this._Elements[index] = value;
            }
        }

        public void Add(T item)
        {
            this._Elements.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            this._Elements.AddRange(collection);
        }

        public void Clear()
        {
            this._Elements.Clear();
        }

        public bool Contains(T item)
        {
            return this._Elements.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._Elements.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this._Elements.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IList)this._Elements).IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return this._Elements.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList)this._Elements).GetEnumerator();
        }

        public T[] ToArray()
        {
            return this._Elements.ToArray();
        }

        #endregion
    }
}
