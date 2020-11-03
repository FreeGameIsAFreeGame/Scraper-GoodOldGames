using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FreeGameIsAFreeGame.Scraper.GoodOldGames
{
    using J = JsonPropertyAttribute;

    public partial class GoodOldGamesData
    {
        [J("id")] public Guid Id { get; set; }
        [J("endTime")] public long EndTime { get; set; }
        [J("background")] public string Background { get; set; }
        [J("mobileBackground")] public string MobileBackground { get; set; }
        [J("title")] public string Title { get; set; }
        [J("logo")] public Logo Logo { get; set; }
        [J("gameUrl")] public string GameUrl { get; set; }
        [J("backgroundColour")] public string BackgroundColour { get; set; }
    }

    public partial class Logo
    {
        [J("image")] public string Image { get; set; }
        [J("styles")] public Styles Styles { get; set; }
    }

    public partial class Styles
    {
        [J("mobile")] public Desktop Mobile { get; set; }
        [J("tablet")] public Desktop Tablet { get; set; }
        [J("desktop")] public Desktop Desktop { get; set; }
    }

    public partial class Desktop
    {
        [J("top")] public string Top { get; set; }
        [J("left")] public string Left { get; set; }
        [J("width")] public string Width { get; set; }
    }

    public partial class GoodOldGamesData
    {
        public static GoodOldGamesData FromJson(string json) => JsonConvert.DeserializeObject<GoodOldGamesData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this GoodOldGamesData self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }
}
