namespace fastJSON
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class JsonPropertyAttribute : Attribute
    {
        private readonly string _Name;

        public string Name
        {
            get { return this._Name; }
        }

        public JsonPropertyAttribute(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("The key cannot be null or empty.");

            this._Name = key;
        }
    }
}
