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
    using System.Collections;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The twitter cursor paged collection.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public abstract class TwitterCursorPagedCollection<T> : TwitterObject, IList<T>
        where T : class
    {
        #region Constants and Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly List<T> _Elements;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterCursorPagedCollection{T}"/> class.
        /// </summary>
        protected TwitterCursorPagedCollection()
        {
            this._Elements = new List<T>();
        }

        #endregion


        protected T[] Elements
        {
            get { return this._Elements.ToArray(); }
            set
            {
                this._Elements.Clear();
                this._Elements.AddRange(value);
            }
        }


        #region Public Properties

        /// <summary>
        /// Gets Count.
        /// </summary>
        public int Count
        {
            get { return this._Elements.Count; }
        }

        /// <summary>
        /// Gets or sets NextCursor.
        /// </summary>
        [JsonProperty("next_cursor_str")]
        public string NextCursor { get; set; }

        /// <summary>
        /// Gets or sets PreviousCursor.
        /// </summary>
        [JsonProperty("previous_cursor_str")]
        public string PreviousCursor { get; set; }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        /// Gets a value indicating whether IsReadOnly.
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList)this._Elements).IsReadOnly; }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public T this[int index]
        {
            get { return this._Elements[index]; }
            set { this._Elements[index] = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(T item)
        {
            this._Elements.Add(item);
        }

        /// <summary>
        /// The add range.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void AddRange(IEnumerable<T> collection)
        {
            this._Elements.AddRange(collection);
        }

        /// <summary>
        /// The clear.
        /// </summary>
        public void Clear()
        {
            this._Elements.Clear();
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The contains.
        /// </returns>
        public bool Contains(T item)
        {
            return this._Elements.Contains(item);
        }

        /// <summary>
        /// The copy to.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// The array index.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this._Elements.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._Elements.GetEnumerator();
        }

        /// <summary>
        /// The index of.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The index of.
        /// </returns>
        public int IndexOf(T item)
        {
            return this._Elements.IndexOf(item);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Insert(int index, T item)
        {
            this._Elements.Insert(index, item);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The remove.
        /// </returns>
        public bool Remove(T item)
        {
            return this._Elements.Remove(item);
        }

        /// <summary>
        /// The remove at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void RemoveAt(int index)
        {
            this._Elements.RemoveAt(index);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList)this._Elements).GetEnumerator();
        }

        #endregion
    }
}