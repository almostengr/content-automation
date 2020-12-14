using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tweetinvi;

namespace Almostengr.SmAutomation.Models
{
    public class TwitterTweet
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public TwitterTweet(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        public async Task PostTweet(TwitterClient twitterClient, string tweetText)
        {
            if (tweetText.Length > 280)
            {
                _logger.LogWarning("Tweet is too long. Truncating. BEFORE: {tweetText}", tweetText);
                tweetText = tweetText.Substring(0, 280);
            }

            // var tweet = await twitterClient.Tweets.PublishTweetAsync(tweetText);

            _logger.LogInformation("TWEETED: {tweetText}", tweetText);
        }
    }
}