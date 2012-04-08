

namespace TwitterDemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    using Mirai.Social.Twitter.Core;
    using Mirai.Social.Twitter.TwitterObjects;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal TwitterApi TwitterApi;
        internal TwitterUser AuthenticatedUser;
        internal TwitterTweet[] HomeTimeline;
        internal string SinceId;

        public MainWindow()
        {
            InitializeComponent();

            var keys = XElement.Load(".\\Keys.xml");

            TwitterApi = new TwitterApi(keys.Element("consumerKey").Value,
                                        keys.Element("consumerSecret").Value,
                                        keys.Element("token").Value,
                                        keys.Element("tokenSecret").Value);

            this.RefreshHomeTimeline.Click += (sender, args) =>
                {
                    if (String.IsNullOrWhiteSpace(this.SinceId))
                        return;

                    this.UpdateHomeTimeline();
                };

            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += (sender, args) =>
                {
                    if (!this.TwitterApi.AccountCommand.VerifyCredentials(out this.AuthenticatedUser))
                        return;
                };
            bgWorker.RunWorkerCompleted += (sender, args) =>
                {
                    if (this.AuthenticatedUser == null)
                        return;

                    this.ProfileImage.Source = new BitmapImage(new Uri("https://si0.twimg.com/profile_images/1943624052/1cc1blmkl7yg2ud3b8ke_bigger.jpeg"));
                    this.UserInfoStackPanel.Children.Add(new TextBlock(new Run(this.AuthenticatedUser.Name)));
                    this.UserInfoStackPanel.Children.Add(new TextBlock(new Run(this.AuthenticatedUser.Location)));
                    this.UserInfoStackPanel.Children.Add(new TextBlock(new Run(this.AuthenticatedUser.Description)));

                    this.StatisticInfo.Children.Add(new TextBlock(new Run(this.AuthenticatedUser.StatusesCount.ToString())) { HorizontalAlignment = HorizontalAlignment.Center });
                    this.StatisticInfo.Children.Add(new TextBlock(new Run(this.AuthenticatedUser.FollowersCount.ToString())) { HorizontalAlignment = HorizontalAlignment.Center });
                    this.StatisticInfo.Children.Add(new TextBlock(new Run(this.AuthenticatedUser.FriendsCount.ToString())) { HorizontalAlignment = HorizontalAlignment.Center });
                    this.StatisticInfo.Children.Add(new TextBlock(new Run("Tweets")) { HorizontalAlignment = HorizontalAlignment.Center });
                    this.StatisticInfo.Children.Add(new TextBlock(new Run("Followers")) { HorizontalAlignment = HorizontalAlignment.Center });
                    this.StatisticInfo.Children.Add(new TextBlock(new Run("Following")) { HorizontalAlignment = HorizontalAlignment.Center });

                    this.UpdateHomeTimeline();
                };
            bgWorker.RunWorkerAsync();

            this.SendTweet.Click += (sender, args) =>
                {
                    this.TwitterApi.TweetCommand.Update(this.NewTweetBox.Text); 
                    this.NewTweetBox.Clear();

                    var t = new Timer(state =>
                        {
                            var task = new Task(this.UpdateHomeTimeline);
                            task.Start((TaskScheduler)state);
                            
                        }, TaskScheduler.FromCurrentSynchronizationContext(), 3000, Timeout.Infinite);

                    GC.KeepAlive(t);
                };
        }

        private void UpdateHomeTimeline()
        {
            this.HomeTimeline = this.TwitterApi.TimelineCommand.RetrieveHomeTimeline(sinceId: this.SinceId);

            if (this.SinceId == this.HomeTimeline[0].Id)
                return;

            this.HomeTimeline = this.HomeTimeline.TakeWhile(t => t.Id != this.SinceId).ToArray();

            this.SinceId = this.HomeTimeline[0].Id;

            for (int i = this.HomeTimeline.Length - 1; i >= 0; i--)
            {
                var textblock = new TextBlock(new Run(this.HomeTimeline[i].Text));
                textblock.Margin = new Thickness(5);
                textblock.TextWrapping = TextWrapping.Wrap;

                var border = new Border();
                border.Child = textblock;
                border.BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue);
                border.BorderThickness = new Thickness(2);
                border.Margin = new Thickness(2);
                border.CornerRadius = new CornerRadius(5);

                this.RightPanel.Children.Insert(0, border);
            }
        }
    }
}
