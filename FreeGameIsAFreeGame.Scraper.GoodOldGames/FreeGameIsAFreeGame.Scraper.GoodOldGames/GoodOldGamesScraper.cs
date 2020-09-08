using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using FreeGameIsAFreeGame.Core;
using FreeGameIsAFreeGame.Core.Models;
using NLog;

namespace FreeGameIsAFreeGame.Scraper.GoodOldGames
{
    public class GoodOldGamesScraper : IScraper
    {
        private const string URL =
            "https://www.gog.com/games/ajax/filtered?mediaType=game&page=__PAGE__&price=discounted&sort=popularity";

        string IScraper.Identifier => "GoodOldGames";
        string IScraper.DisplayName => "GOG";

        private readonly IBrowsingContext context;
        private readonly ILogger logger;

        public GoodOldGamesScraper()
        {
            context = BrowsingContext.New(Configuration.Default
                .WithDefaultLoader()
                .WithDefaultCookies());
            
            logger = LogManager.GetLogger(GetType().FullName);
        }

        async Task<IEnumerable<IDeal>> IScraper.Scrape(CancellationToken token)
        {
            List<IDeal> deals = new List<IDeal>();
            int pageCount = await GetPageCount(token);
            if (token.IsCancellationRequested)
                return null;

            for (int i = 0; i < pageCount; i++)
            {
                await Task.Delay(1500, token);
                if (token.IsCancellationRequested)
                    return null;

                string content = await GetPageContent(i + 1, token);
                if (token.IsCancellationRequested)
                    return null;

                IEnumerable<Deal> pageDeals = ParseContent(content);
                deals.AddRange(pageDeals);
            }

            return deals;
        }

        private IEnumerable<Deal> ParseContent(string content)
        {
            List<Deal> deals = new List<Deal>();

            GoodOldGamesData data = GoodOldGamesData.FromJson(content);
            foreach (Product product in data.Products)
            {
                if (product.Price.DiscountPercentage != 100)
                    continue;

                Deal deal = new Deal()
                {
                    Discount = product.Price.DiscountPercentage,
                    Image = $"{product.Image}_product_tile_hover_398.jpg",
                    Link = $"https://gog.com{product.Url}",
                    Title = product.Title,
                    Start = DateTimeOffset.FromUnixTimeSeconds(product.SalesVisibility.From).UtcDateTime,
                    End = DateTimeOffset.FromUnixTimeSeconds(product.SalesVisibility.To).UtcDateTime
                };

                if (deal.Image.StartsWith("//"))
                {
                    deal.Image = $"https:{deal.Image}";
                }

                deals.Add(deal);
            }

            return deals;
        }

        private async Task<int> GetPageCount(CancellationToken token)
        {
            string content = await GetPageContent(1, token);
            if (token.IsCancellationRequested)
                return 0;
            GoodOldGamesData data = GoodOldGamesData.FromJson(content);
            return data.TotalPages;
        }

        private async Task<string> GetPageContent(int page, CancellationToken token)
        {
            Url url = Url.Create(GetUrl(page));
            DocumentRequest request = DocumentRequest.Get(url);
            IDocument document = await context.OpenAsync(request, token);
            if (token.IsCancellationRequested)
                return null;
            string content = document.Body.Text();
            return content;
        }

        private string GetUrl(int page)
        {
            return URL.Replace("__PAGE__", page.ToString());
        }
    }
}
