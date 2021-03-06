﻿namespace Mirai.ConsoleUI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml.Linq;

    using Mirai.Social.Twitter;
    using Mirai.Social.Twitter.Commands;
    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    class Program
    {
        static void Main(string[] args)
        {
            var keys = XElement.Load(".\\Keys.xml");

            var twitterApi = new TwitterApi(keys.Element("consumerKey").Value,
                                            keys.Element("consumerSecret").Value,
                                            keys.Element("token").Value,
                                            keys.Element("tokenSecret").Value);
            twitterApi.LogEnabled = true;
            twitterApi.LogStream = Console.Out;

            //var twitterApi = new TwitterApi(keys.Element("consumerKey").Value,
            //                                keys.Element("consumerSecret").Value);

            //var uri = twitterApi.RequestUserAuthorization("oob");
            //Console.WriteLine(uri);
            //var v = Console.ReadLine();
            //twitterApi.ProcessUserAuthorization(v);
            //twitterApi.TweetCommand.Update("Hello World!");

            try
            {
                //var fs = new FileStream(@"C:\Users\Kevin\素材\Avatar\img021.jpg", FileMode.Open, FileAccess.Read);
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

                //twitterApi.TweetCommand.UpdateWithMedia("Gundam Seed", mediaList: files);
                //twitterApi.TweetCommand.RetweetedBy("21947795900469248");
                //twitterApi.TweetCommand.Update("a new test~~~~~~~~....");

                //twitterApi.AccountCommand.UpdateProfileImage(@"C:\Users\Kevin\Pictures\Anime\Anime-029.jpg");
                //int friends, statuses, followers, favorites;
                //twitterApi.AccountCommand.Totals(out friends, out statuses, out followers, out favorites);
                //twitterApi.AccountCommand.RetrieveAccountSettings();

                //twitterApi.GetSupportedLanguages();

                //twitterApi.NotificationCommand.LeaveByScreenName("twitterapi");
                //twitterApi.NotificationCommand.FollowByScreenName("twitterapi");

                //twitterApi.FavoriteCommand.RetrieveFavorites("twitterapi");
                //twitterApi.FavoriteCommand.Destroy("183536265763897345");

                //twitterApi.SuggestedUserCommand.RetrieveSuggestionCategories();
                //twitterApi.SuggestedUserCommand.RetrieveUsersInCategory("ニュース");

                //twitterApi.UserCommand.Lookup(new[] { "twitter", "twittermobile" }, new[] { "6253282" });
                //twitterApi.UserCommand.RetrieveProfileImageUri("twitter");
                //twitterApi.UserCommand.Show("yukinoyume", "11046332");
                //twitterApi.UserCommand.RetrieveContributees("themattharris");
                //twitterApi.UserCommand.RetrieveContributors("twitterapi");

                //twitterApi.FriendshipCommand.RetrieveIdsForFriends("twitterapi");
                //twitterApi.FriendshipCommand.RetrieveIdsForIncomingRequests();
                //twitterApi.FriendshipCommand.RetrieveIdsForOutgoingRequests();
                //twitterApi.FriendshipCommand.Create("episod");
                //twitterApi.FriendshipCommand.Destroy("AncientProverbs");
                //twitterApi.FriendshipCommand.Show("yukinoyume", "FairyLGotay");
                //twitterApi.FriendshipCommand.Lookup(new[] { "FairyLGotay", "twitterapi" });
                //twitterApi.FriendshipCommand.Update("yukinoyume");
                //twitterApi.FriendshipCommand.RetrieveNoRetweetIds();

                //twitterApi.TimelineCommand.RetrieveHomeTimeline();
                //twitterApi.TimelineCommand.RetrieveMentions();
                //twitterApi.TimelineCommand.RetweetedByMe();
                //twitterApi.TimelineCommand.RetweetedToMe();
                //twitterApi.TimelineCommand.RetweetsOfMe();
                //twitterApi.TimelineCommand.RetrieveUserTimeline(screenName: "nokia");
                //twitterApi.TimelineCommand.RetweetedToUser(screenName: "nokia");
                //twitterApi.TimelineCommand.RetweetedByUser(screenName: "nokia");

                //twitterApi.SearchCommand.Search("月亮", new SearchOptions
                //    {
                //        //GeoCode = new TwitterGeoCode(37.781157, -122.398720, 10),
                //        IncludeEntities = true,
                //        Page = 1,
                //        ShowUser = true,
                //        ReusltType = TwitterSearchReusltType.Mixed,
                //        TweetsPerPage = 20,
                //        Until = DateTime.Now
                //    });

                //twitterApi.SavedSearchCommand.RetrieveSavedSearches();
                //twitterApi.SavedSearchCommand.Show("3762040");
                //twitterApi.SavedSearchCommand.Create("@space");
                //twitterApi.SavedSearchCommand.Destroy("86615025");

                //twitterApi.DirectMessageCommand.RetrieveDirectMessages();
                //twitterApi.DirectMessageCommand.Show("1900822745");
                //twitterApi.DirectMessageCommand.Destroy("1900822745");
                //twitterApi.DirectMessageCommand.Sent();

                //twitterApi.ListCommand.RetrieveAllLists("yukinoyume");
                //twitterApi.ListCommand.RetrieveUserCreatedLists("yukinoyume");
                //twitterApi.ListCommand.RetrieveSubscriptions("yukinoyume");
                //twitterApi.ListCommand.RetrieveTweetsOfListMembersBySlug("team", "twitter");
                //twitterApi.ListCommand.RetrieveMemberships("twitter");
                //twitterApi.ListCommand.RetrieveSubscribersBySlug("team", "twitter");
                //twitterApi.ListCommand.RetrieveSubscribersById("8044403");
                //twitterApi.ListCommand.SubscribeToListBySlug("team", "twitter");
                //twitterApi.ListCommand.UnsubscribeFromListBySlug("team", "twitter");
                //twitterApi.ListCommand.AddMembersToListBySlug("tech", "yukinoyume", new[] { "FairyLGotay", "Apigee" });
                //twitterApi.ListCommand.RetrieveMembersBySlug("tech", "yukinoyume");
                //twitterApi.ListCommand.AddMemberToListById("53415010", "twitterapi");
                //twitterApi.ListCommand.RemoveMemberFromListById("53415010", "twitter");
                //twitterApi.ListCommand.RemoveMembersFromListBySlug("tech", "yukinoyume", new[] { "twitterapi" });
                //twitterApi.ListCommand.CreateList("new list");
                //twitterApi.ListCommand.UpdateListBySlug("new-list", "yukinoyume", description: "just a test list");
                //twitterApi.ListCommand.DestroyListBySlug("new-list", "yukinoyume");
                //twitterApi.ListCommand.RetrieveUserCreatedListBySlug("tech1", "yukinoyume");
                //twitterApi.ListCommand.Contains("tech", "yukinoyume", "apige");
                //twitterApi.ListCommand.Subscribes("tech", "yukinoyume", "twitter");

                //twitterApi.BlockCommand.BlockUser("twitter");
                //twitterApi.BlockCommand.RetrieveBlockedUsers();
                //twitterApi.BlockCommand.IsBlocked("twitter");
                //twitterApi.BlockCommand.RetrieveIdsOfBlockedUsers();
                //twitterApi.BlockCommand.UnblockUser("twitter");
                //twitterApi.BlockCommand.IsBlocked("twitter");

                //twitterApi.SpamReportingCommand.ReportSpam("2423423rerfwexfsr2342");

                //foreach (var grp in twitterApi.TrendCommand.RetrieveDailyTrends())
                //{
                //    Console.WriteLine("---{0}---", grp.Key);
                //    foreach (var trend in grp)
                //    {
                //        Console.WriteLine("\tEvents: {0}\n\tName: {1}\n\tQuery: {2}\n\tUrl: {3}", 
                //            trend.Events, trend.Name, trend.Query, trend.Url);
                //    }
                //}
                //twitterApi.TrendCommand.RetrieveWeeklyTrends();
                //twitterApi.TrendCommand.RetrieveTrendLocations();
                //twitterApi.TrendCommand.RetrieveTrendsByWoeId("2442047"); // LA

                //twitterApi.GeoCommand.RetrievePlaceById("df51dec6f4ee2b2c");
                //twitterApi.GeoCommand.RetrieveSimilarPlaces(37, -122, "twitter hq");
                //twitterApi.GeoCommand.ReverseGeoCode("37.76893497", "-122.422848843");
                //twitterApi.GeoCommand.Search(null, null, null, IPAddress.Parse("74.125.19.104"));
            }
            catch (TwitterException e)
            {
                Console.WriteLine(e.Error.Message);
            }

            Console.ReadLine();
        }
    }
}
