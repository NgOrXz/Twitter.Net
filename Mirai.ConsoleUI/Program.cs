using System;

namespace Mirai.ConsoleUI
{

    using Mirai.Twitter.Core;

    class Program
    {
        static void Main(string[] args)
        {
            //var consumerCredential = new ConsumerCredential(
            //                                                "H1eRltWyujV54wV5DpVQQ",
            //                                                "Njg4YH4hKboYiO5f1haZKH4ODljhwcPGkqWX5BCovck");
            //consumerCredential.SetToken(
            //                            "11046312-egjx9U7pUHq4UJpoGb76K4TsdhWMPS9IgdzwTvYiz",
            //                            "1EFqdonQDmtbS98Dj8GNeH8xCnnTkum6xH2xWBwGU",
            //                            TokenType.AccessToken);

            //var serviceProviderDescription = new ServiceProviderDescription(ProtocolVersion.V10A);
            //serviceProviderDescription.RequestTokenEndPoint =
            //    new OAuthEndPoint("https://api.twitter.com/oauth/request_token");
            //serviceProviderDescription.AuthorizationEndPoint =
            //    new OAuthEndPoint("https://api.twitter.com/oauth/authorize", HttpMethod.Get);
            //serviceProviderDescription.AccessTokenEndPoint =
            //    new OAuthEndPoint("https://api.twitter.com/oauth/access_token");
            //OAuth oAuth = new OAuth(consumerCredential, serviceProviderDescription);
            //oAuth.Realm = "https://api.twitter.com";

            //oAuth.AcquireRequestToken("oob");
            //Console.WriteLine("Request Token: {0}\nToken Secret: {1}", oAuth.Token, oAuth.TokenSecret);
            //Console.WriteLine(oAuth.AuthorizationEndPoint.ResourceUri + "?oauth_token=" + oAuth.Token);

            //Console.WriteLine("Enter verifier: ");
            //string verifier = Console.ReadLine();
            //oAuth.AcquireAccessToken(verifier);
            //Console.WriteLine("Access Token: {0}\nToken Secret: {1}", oAuth.Token, oAuth.TokenSecret);

            //var statusUpdateUrl = "https://api.twitter.com/1/statuses/update.json?{0}={1}";
            //var updateWithMedia = "https://upload.twitter.com/1/statuses/update_with_media.json";
            //var homeTimelineUri = "http://api.twitter.com/1/statuses/home_timeline.json";

            //while (true)
            //{
            //    Console.WriteLine("Enter q to exit.\nSend a user: ");
            //    var user = Console.ReadLine();
            //    if (user == "q")
            //        break;

            //    var postData = new Dictionary<string, string>();
            //    postData.Add("trim_user", "true");
            //    oAuth.ExecuteAuthenticatedRequest(new Uri(homeTimelineUri),
            //        HttpMethod.Get, postData);

            //    //var postData = new Dictionary<string, string>();
            //    //postData.Add("status", user);
            //    //postData.Add("include_entities", "true");
            //    //oAuth.ExecuteAuthenticatedRequest(new Uri(String.Format(statusUpdateUrl, "trim_user", "true")), HttpMethod.Post, postData);
            //    Console.WriteLine("Done.");
            //}

            //var fs = new FileStream(@"C:\Users\Kevin\素材\Avatar\002.jpg", FileMode.Open, FileAccess.Read);
            //var data = new byte[fs.Length];
            //fs.Read(data, 0, data.Length);
            //fs.Close();

            //var dict = new Dictionary<string, object>();
            //dict.Add("status", "update with pic");
            //dict.Add("media[]", data);
            //oAuth.ExecuteAuthenticatedRequestForMultipartFormData(new Uri(updateWithMedia), dict);
            //Console.WriteLine("Updated!");

            var twitterObj = new TwitterApi(
                                            "H1eRltWyujV54wV5DpVQQ",
                                            "Njg4YH4hKboYiO5f1haZKH4ODljhwcPGkqWX5BCovck",
                                            "11046312-egjx9U7pUHq4UJpoGb76K4TsdhWMPS9IgdzwTvYiz",
                                            "1EFqdonQDmtbS98Dj8GNeH8xCnnTkum6xH2xWBwGU");

            //var uri = twitterObj.AcquireRequestToken("oob", null);
            //Console.WriteLine(uri.ToString());

            //Console.WriteLine("Enter verifier: ");
            //string verifier = Console.ReadLine();
            //twitterObj.AcquireAccessToken(verifier);

            //Console.WriteLine("Enter a tweet: ");
            //var tweet = Console.ReadLine();
            //var result = twitterObj.TweetCommand.Update(tweet, responseFormat: ResponseFormat.Json);
            //Console.WriteLine(result);

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

            //var s = "{\"coordinate\":null,\"created_at\":\"Fri Jun 24 17:43:26 +0000 2011\",\"truncated\":false,\"favorited\":false,\"id_str\":\"84315710834212866\",\"entities\":{\"urls\":[],\"hashtags\":[{\"text\":\"peterfalk\",\"indices\":[35,45]}],\"user_mentions\":[]},\"in_reply_to_user_id_str\":null,\"contributors\":null,\"text\":\"Maybe he'll finally find his keys. #peterfalk\",\"retweet_count\":0,\"id\":84315710834212866,\"in_reply_to_status_id_str\":null,\"geo\":null,\"retweeted\":false,\"in_reply_to_user_id\":null,\"source\":\"<a href=\"http://sites.google.com/site/yorufukurou/\" rel=\"nofollow\">YoruFukurou</a>\",\"in_reply_to_screen_name\":null,\"user\":{\"id_str\":\"819797\",\"id\":819797},\"place\":null,\"in_reply_to_status_id\":null}";
            //TwitterTweet.FromJson(s);

            //twitterObj.TweetCommand.RetweetedBy("21947795900469248");
            //twitterObj.TweetCommand.Update("hello theredsfsfasaf againnnnxxxxxnnnnn!!");

            int friends, statuses, followers, favorites;
            try
            {
                //twitterObj.AccountCommand.UpdateProfileImage(@"C:\Users\Kevin\Pictures\Anime\Anime-029.jpg");
                
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
                twitterObj.FriendshipCommand.Update("yukinoyume");

                //twitterObj.FriendshipCommand.RetrieveNoRetweetIds();

                //twitterObj.TimelineCommand.RetrieveHomeTimeline();
                //twitterObj.TimelineCommand.RetrieveMentions();
                //twitterObj.TimelineCommand.RetweetedByMe();
                //twitterObj.TimelineCommand.RetweetedToMe();
                //twitterObj.TimelineCommand.RetweetsOfMe();

                //twitterObj.TimelineCommand.RetrieveUserTimeline(screenName: "nokia");
                //twitterObj.TimelineCommand.RetweetedToUser(screenName: "nokia");
                //twitterObj.TimelineCommand.RetweetedByUser(screenName: "nokia");
            }
            catch (TwitterException e)
            {
                Console.WriteLine(e.Error.Message);
            }

            Console.ReadLine();            
        }
    }
}
