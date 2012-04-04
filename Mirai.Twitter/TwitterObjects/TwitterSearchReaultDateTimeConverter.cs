namespace Mirai.Twitter.TwitterObjects
{
    internal sealed class TwitterSearchReaultDateTimeConverter : TwitterDateTimeConverter
    {
        public TwitterSearchReaultDateTimeConverter()
        {
            this.DateTimeFormat = "ddd, dd MMM yyyy HH:mm:ss +0000";
        }
    }
}