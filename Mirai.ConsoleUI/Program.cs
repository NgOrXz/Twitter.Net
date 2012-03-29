namespace Mirai.ConsoleUI
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    using Mirai.Twitter;
    using Mirai.Twitter.Commands;
    using Mirai.Twitter.Core;

    class Program
    {
        static void Main(string[] args)
        {
            var keys = XElement.Load(".\\Keys.xml");

            var twitterObj = new TwitterApi(keys.Element("consumerKey").Value,
                                            keys.Element("consumerSecret").Value,
                                            keys.Element("token").Value,
                                            keys.Element("tokenSecret").Value);

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

                //twitterObj.SearchCommand.Search("月亮", new SearchOptions
                //    {
                //        //GeoCode = new TwitterGeoCode(37.781157, -122.398720, 10),
                //        IncludeEntities = true,
                //        Page = 1, ShowUser = true, ReusltType = TwitterSearchReusltType.Mixed,
                //        TweetsPerPage = 20, Until = DateTime.Now
                //    });

                //twitterObj.SavedSearchCommand.RetrieveSavedSearches();
                //twitterObj.SavedSearchCommand.Show("86615025");
                //twitterObj.SavedSearchCommand.Create("@space");
                //twitterObj.SavedSearchCommand.Destroy("86615025");

                //twitterObj.DirectMessageCommand.RetrieveDirectMessages();
                //twitterObj.DirectMessageCommand.Show("1900822745");
                //twitterObj.DirectMessageCommand.Destroy("1900822745");
                //twitterObj.DirectMessageCommand.Sent();

                //twitterObj.ListCommand.RetrieveAllLists("yukinoyume");
                //twitterObj.ListCommand.RetrieveUserLists("yukinoyume");
                //twitterObj.ListCommand.RetrieveSubscriptions("yukinoyume");
                //twitterObj.ListCommand.RetrieveTweetsOfListMembersBySlug("team", "twitter");
                //twitterObj.ListCommand.RetrieveMemberships("twitter");
                //twitterObj.ListCommand.RetrieveSubscribersBySlug("team", "twitter");
                //twitterObj.ListCommand.RetrieveSubscribersById("8044403");
                //twitterObj.ListCommand.SubscribeToListBySlug("team", "twitter");
                //twitterObj.ListCommand.UnsubscribeFromListBySlug("team", "twitter");
                //twitterObj.ListCommand.AddMembersToListBySlug("tech", "yukinoyume", new[] { "FairyLGotay", "Apigee" });
                //twitterObj.ListCommand.RetrieveMembersBySlug("tech", "yukinoyume");
                //twitterObj.ListCommand.AddMemberToListById("53415010", "twitterapi");
                //twitterObj.ListCommand.RemoveMemberFromListById("53415010", "twitter");
                //twitterObj.ListCommand.RemoveMembersFromListBySlug("tech", "yukinoyume", new[] { "twitterapi" });
                //twitterObj.ListCommand.CreateList("new list");
                //twitterObj.ListCommand.UpdateListBySlug("new-list", "yukinoyume", description: "just a test list");
                //twitterObj.ListCommand.DestroyListBySlug("new-list", "yukinoyume");
                //twitterObj.ListCommand.RetrieveUserCreatedListBySlug("tech1", "yukinoyume");
                //twitterObj.ListCommand.Contains("tech", "yukinoyume", "apige");
                //twitterObj.ListCommand.Subscribes("tech", "yukinoyume", "twitter");
            }
            catch (TwitterException e)
            {
                Console.WriteLine(e.Error.Message);
            }

            Console.ReadLine();
        }
    }
}
