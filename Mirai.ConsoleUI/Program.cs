using System;

namespace Mirai.ConsoleUI
{
    using Mirai.Twitter;
    using Mirai.Twitter.Commands;
    using Mirai.Twitter.Core;

    class Program
    {
        static void Main(string[] args)
        {
            var twitterObj = new TwitterApi(
                                            "",
                                            "",
                                            "",
                                            "");

            try
            {
                //var fs = new FileStream(@"C:\Users\Kevin\素材\Avatar\3222.jpg", FileMode.Open, FileAccess.Read);
                //var data = new byte[fs.Length];
                //fs.Read(data, 0, data.Length);
                //fs.Close();

                //fs = new FileStream(@"C:\Users\Kevin\素材\Avatar\4543.jpg", FileMode.Open, FileAccess.Read);
                //var data2 = new byte[fs.Length];
                //fs.Read(data2, 0, data2.Length);
                //fs.Close();

                //var files = new List<byte[]>();
                //files.Add(data);
                //files.Add(data2);
                //twitterObj.TweetCommand.UpdateWithMedia("update with two picture for test~~~~~", mediaList: files);
                //Console.WriteLine("Updated!");

                //twitterObj.TweetCommand.RetweetedBy("21947795900469248");
                //twitterObj.TweetCommand.Update("hello theredsfsfasaf againnnnxxxxxnnnnn!!");

                //twitterObj.AccountCommand.UpdateProfileImage(@"C:\Users\Kevin\Pictures\Anime\Anime-029.jpg");

                //int friends, statuses, followers, favorites;
                //twitterObj.AccountCommand.Totals(out friends, out statuses, out followers, out favorites);

                //twitterObj.GetSupportedLanguages();
                //twitterObj.NotificationCommand.LeaveByScreenName("twitterapi");

                //twitterObj.NotificationCommand.FollowByScreenName("twitterapi");

                //twitterObj.FavoriteCommand.RetrieveFavorites("twitterapi");
                //twitterObj.FavoriteCommand.Destroy("183536265763897345");
                //twitterObj.SuggestedUserCommand.RetrieveSuggestionCategories();
                //twitterObj.SuggestedUserCommand.RetrieveUsersInCategory("ニュース");
                //twitterObj.UserCommand.Lookup(new[] { "twitter", "twittermobile" }, new[] { "6253282" });
                //twitterObj.UserCommand.RetrieveProfileImageUri("twitter");
                //twitterObj.UserCommand.Show("yukinoyume", "11046332");
                //twitterObj.UserCommand.RetrieveContributees("themattharris");
                //twitterObj.UserCommand.RetrieveContributors("twitterapi");

                //twitterObj.FriendshipCommand.RetrieveIdsForFollowers("twitterapi");
                //twitterObj.FriendshipCommand.RetrieveIdsForFriends("twitterapi");

                //twitterObj.FriendshipCommand.RetrieveIdsForIncomingRequests();
                //twitterObj.FriendshipCommand.RetrieveIdsForOutgoingRequests();

                //twitterObj.FriendshipCommand.Create("episod");
                //twitterObj.FriendshipCommand.Destroy("AncientProverbs");

                //twitterObj.FriendshipCommand.Show("yukinoyume", "FairyLGotay");
                //twitterObj.FriendshipCommand.Lookup(new[] { "FairyLGotay", "twitterapi" });
                //twitterObj.FriendshipCommand.Update("yukinoyume");

                //twitterObj.FriendshipCommand.RetrieveNoRetweetIds();

                //twitterObj.TimelineCommand.RetrieveHomeTimeline();
                //twitterObj.TimelineCommand.RetrieveMentions();
                //twitterObj.TimelineCommand.RetweetedByMe();
                //twitterObj.TimelineCommand.RetweetedToMe();
                //twitterObj.TimelineCommand.RetweetsOfMe();

                //twitterObj.TimelineCommand.RetrieveUserTimeline(screenName: "nokia");
                //twitterObj.TimelineCommand.RetweetedToUser(screenName: "nokia");
                //twitterObj.TimelineCommand.RetweetedByUser(screenName: "nokia");

                twitterObj.SearchCommand.Search("月亮", new SearchCommandOptions
                    {
                        //GeoCode = new TwitterGeoCode(37.781157, -122.398720, 10),
                        IncludeEntities = true,
                        Page = 1, ShowUser = true, ReusltType = TwitterSearchReusltType.Mixed,
                        TweetsPerPage = 20, Until = DateTime.Now
                    });
            }
            catch (TwitterException e)
            {
                Console.WriteLine(e.Error.Message);
            }

            Console.ReadLine();            
        }
    }
}
