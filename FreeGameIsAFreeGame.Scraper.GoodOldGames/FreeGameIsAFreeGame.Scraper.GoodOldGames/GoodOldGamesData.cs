using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FreeGameIsAFreeGame.Scraper.GoodOldGames
{
    using J = JsonPropertyAttribute;

    public partial class GoodOldGamesData
    {
        [J("products")] public List<Product> Products { get; set; }
        [J("ts")] public object Ts { get; set; }
        [J("page")] public int Page { get; set; }
        [J("totalPages")] public int TotalPages { get; set; }

        [J("totalResults")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalResults { get; set; }

        [J("totalGamesFound")] public long TotalGamesFound { get; set; }
        [J("totalMoviesFound")] public long TotalMoviesFound { get; set; }
    }

    public partial class Product
    {
        [J("customAttributes")] public List<object> CustomAttributes { get; set; }
        [J("developer")] public string Developer { get; set; }
        [J("publisher")] public string Publisher { get; set; }
        [J("gallery")] public List<string> Gallery { get; set; }
        [J("video")] public Video Video { get; set; }
        [J("supportedOperatingSystems")] public List<string> SupportedOperatingSystems { get; set; }
        [J("genres")] public List<string> Genres { get; set; }
        [J("globalReleaseDate")] public long? GlobalReleaseDate { get; set; }
        [J("isTBA")] public bool IsTba { get; set; }
        [J("price")] public Price Price { get; set; }
        [J("isDiscounted")] public bool IsDiscounted { get; set; }
        [J("isInDevelopment")] public bool IsInDevelopment { get; set; }
        [J("id")] public long Id { get; set; }
        [J("releaseDate")] public long? ReleaseDate { get; set; }
        [J("availability")] public Availability Availability { get; set; }
        [J("salesVisibility")] public SalesVisibility SalesVisibility { get; set; }
        [J("buyable")] public bool Buyable { get; set; }
        [J("title")] public string Title { get; set; }
        [J("image")] public string Image { get; set; }
        [J("url")] public string Url { get; set; }
        [J("supportUrl")] public string SupportUrl { get; set; }
        [J("forumUrl")] public string ForumUrl { get; set; }
        [J("worksOn")] public WorksOn WorksOn { get; set; }
        [J("category")] public string Category { get; set; }
        [J("originalCategory")] public string OriginalCategory { get; set; }
        [J("rating")] public long Rating { get; set; }
        [J("type")] public long Type { get; set; }
        [J("isComingSoon")] public bool IsComingSoon { get; set; }
        [J("isPriceVisible")] public bool IsPriceVisible { get; set; }
        [J("isMovie")] public bool IsMovie { get; set; }
        [J("isGame")] public bool IsGame { get; set; }
        [J("slug")] public string Slug { get; set; }
        [J("isWishlistable")] public bool IsWishlistable { get; set; }
    }

    public partial class Availability
    {
        [J("isAvailable")] public bool IsAvailable { get; set; }
        [J("isAvailableInAccount")] public bool IsAvailableInAccount { get; set; }
    }

    public partial class Price
    {
        [J("amount")] public string Amount { get; set; }
        [J("baseAmount")] public string BaseAmount { get; set; }
        [J("finalAmount")] public string FinalAmount { get; set; }
        [J("isDiscounted")] public bool IsDiscounted { get; set; }
        [J("discountPercentage")] public int DiscountPercentage { get; set; }
        [J("discountDifference")] public string DiscountDifference { get; set; }
        [J("symbol")] public string Symbol { get; set; }
        [J("isFree")] public bool IsFree { get; set; }
        [J("discount")] public long Discount { get; set; }
        [J("isBonusStoreCreditIncluded")] public bool IsBonusStoreCreditIncluded { get; set; }
        [J("bonusStoreCreditAmount")] public string BonusStoreCreditAmount { get; set; }
        [J("promoId")] public string PromoId { get; set; }
    }

    public partial class SalesVisibility
    {
        [J("isActive")] public bool IsActive { get; set; }
        [J("fromObject")] public Object FromObject { get; set; }
        [J("from")] public long From { get; set; }
        [J("toObject")] public Object ToObject { get; set; }
        [J("to")] public long To { get; set; }
    }

    public partial class Object
    {
        [J("date")] public DateTimeOffset Date { get; set; }
        [J("timezone_type")] public long TimezoneType { get; set; }
        [J("timezone")] public string Timezone { get; set; }
    }

    public partial class Video
    {
        [J("id")] public string Id { get; set; }
        [J("provider")] public string Provider { get; set; }
    }

    public partial class WorksOn
    {
        [J("Windows")] public bool Windows { get; set; }
        [J("Mac")] public bool Mac { get; set; }
        [J("Linux")] public bool Linux { get; set; }
    }

    public partial class GoodOldGamesData
    {
        public static GoodOldGamesData FromJson(string json) =>
            JsonConvert.DeserializeObject<GoodOldGamesData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this GoodOldGamesData self) =>
            JsonConvert.SerializeObject(self, Converter.Settings);
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

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }

            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (long) untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
