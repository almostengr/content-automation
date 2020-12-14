using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.SmAutomation.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;

namespace Almostengr.SmAutomation
{
    public class TwitterWorker : BackgroundService
    {
        private readonly ILogger<TwitterWorker> _logger;
        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;
        private TwitterClient twitterClient;
        private Random _random;

        public TwitterWorker(ILogger<TwitterWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _appSettings = new AppSettings();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _random = new Random();
            ConfigurationBinder.Bind(_config, _appSettings);

            twitterClient = new TwitterClient(
                _appSettings.Twitter.ConsumerKey,
                _appSettings.Twitter.ConsumerSecret,
                _appSettings.Twitter.AccessToken,
                _appSettings.Twitter.AccessSecret
            );

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await FavoriteTweets(stoppingToken);
                }
                catch (TwitterException ex)
                {
                    _logger.LogError(string.Concat(ex.GetType(), " ", ex.StatusCode, " ", ex.Message));

                    if (ex.StatusCode == 429)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(_random.Next(20, 50)), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(string.Concat(ex.GetType(), " ", ex.Message));
                }

                await Task.Delay(TimeSpan.FromHours(_random.Next(1, 8)), stoppingToken);
            }
        }

        private async Task FavoriteTweets(CancellationToken stopToken)
        {
            IList<string> hashTags = _appSettings.Twitter.FavoriteHashTags;
            int tweetCounter = 0;

            ITweet[] tweets = await twitterClient.Search.SearchTweetsAsync(hashTags[_random.Next(hashTags.Count)]);

            foreach (var tweet in tweets)
            {
                bool containsExcludeWord = false;
                foreach (var excludeWord in _appSettings.Twitter.ExcludeWords)
                {
                    if (tweet.Text.ToLower().Contains(excludeWord))
                    {
                        containsExcludeWord = true;
                    }
                }

                if (tweet.Favorited == false && tweet.PossiblySensitive == false &&
                    tweet.IsRetweet == false && tweet.CreatedBy.Name.Contains("almostengr") == false &&
                    containsExcludeWord == false)
                {
                    if (_appSettings.Testing == false)
                    {
                        await twitterClient.Tweets.FavoriteTweetAsync(tweet);
                    }

                    _logger.LogInformation(string.Concat("Favorited Tweet: ", tweet.Text));
                    tweetCounter++;

                    if (tweetCounter >= 3)
                    {
                        break;
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(_random.Next(15, 70)), stopToken);
        }
    }
}
