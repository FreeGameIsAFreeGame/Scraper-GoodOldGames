using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FreeGameIsAFreeGame.Core;
using FreeGameIsAFreeGame.Core.Models;
using Newtonsoft.Json;
using NLog;

namespace FreeGameIsAFreeGame.Scraper.GoodOldGames
{
    public class GoodOldGamesScraper : IScraper
    {
        private IBrowsingContext context;
        private ILogger logger;
        string IScraper.Identifier => "GoodOldGames";

        /// <inheritdoc />
        public Task Initialize(CancellationToken token)
        {
            context = BrowsingContext.New(Configuration.Default
                .WithDefaultLoader()
                .WithDefaultCookies());

            logger = LogManager.GetLogger(GetType().FullName);

            return Task.CompletedTask;
        }

        async Task<IEnumerable<IDeal>> IScraper.Scrape(CancellationToken token)
        {
            Url url = Url.Create("https://www.gog.com");
            IDocument document = await context.OpenAsync(url, token);
            token.ThrowIfCancellationRequested();

            IHtmlAnchorElement giveawayAnchor = document.Body.QuerySelector<IHtmlAnchorElement>(".giveaway-banner");
            if (giveawayAnchor == null)
            {
                logger.Info("No giveaway found");
                return new List<IDeal>();
            }

            string onclickContent = giveawayAnchor.GetAttribute("onclick");
            Match match = Regex.Match(onclickContent, "'.+'");
            if (!match.Success)
            {
                logger.Info("No onclick found");
                return new List<IDeal>();
            }

            string encodedJson = match.Value.Trim('\'');
            string json = Regex.Unescape(encodedJson);

            GoodOldGamesData data = JsonConvert.DeserializeObject<GoodOldGamesData>(json);

            return new List<IDeal>
            {
                new Deal
                {
                    Image = $"https://images-1.gog-statics.com/{data.Logo.Image}.png",
                    Link = $"https://www.gog.com{data.GameUrl}",
                    Title = data.Title,
                    End = DateTimeOffset.FromUnixTimeMilliseconds(data.EndTime).UtcDateTime
                }
            };
        }

        /// <inheritdoc />
        public Task Dispose()
        {
            context?.Dispose();
            return Task.CompletedTask;
        }
    }
}
