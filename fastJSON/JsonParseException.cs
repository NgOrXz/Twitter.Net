namespace fastJSON
{
    using System;

    [Serializable]
    public sealed class JsonParseException : Exception
    {
        public JsonParseException(string message)
            : this(message, null)
        {
            
        }

        public JsonParseException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}
