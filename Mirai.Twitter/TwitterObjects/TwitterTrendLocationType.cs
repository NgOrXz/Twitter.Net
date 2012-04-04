namespace Mirai.Twitter.TwitterObjects
{
    using Newtonsoft.Json;

    public sealed class TwitterTrendLocationType : TwitterObject
    {
        #region Public Properties

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        #endregion

    }
}