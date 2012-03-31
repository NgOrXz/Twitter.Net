namespace Mirai.Twitter.TwitterObjects
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Mirai.Twitter.Core;

    public sealed class TwitterTrendLocationType
    {
        [TwitterKey("name")]
        public string Name { get; set; }

        [TwitterKey("code")]
        public string Code { get; set; }


        public static TwitterTrendLocationType FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var locationType = new TwitterTrendLocationType();
            if (dictionary.Count == 0)
                return locationType;

            var pis = locationType.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                    typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false
                    || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(String))
                {
                    propertyInfo.SetValue(locationType, value, null);
                }
            }

            return locationType;
        }
    }
}