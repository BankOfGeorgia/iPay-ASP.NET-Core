using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IpayDemo.Net.Models
{
    class Order
    {
        private readonly List<Item> items;

        public Order(List<Item> items)
        {
            this.items = items;
        }

        public Intent Intent { get; set; }
        public string RedirectUrl { get; set; }
        public string ShopOrderId { get; set; }
        public string Locale { get; set; }
        public bool ShowShopOrderIdOnExtract { get; set; }

        public ReadOnlyCollection<Item> Items => items.AsReadOnly();

        public ReadOnlyCollection<PurchaseUnit> PurchaseUnits =>
            new List<PurchaseUnit>
            {
                new PurchaseUnit
                {
                    Amount = new Amount
                    {
                        CurrencyCode = Currency.Gel,
                        Value = items.Sum(item => item.Amount)
                    }
                }
            }.AsReadOnly();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    enum Currency
    {
        Gel
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Intent
    {
        [EnumMember(Value = "CAPTURE")]
        Capture,
        [EnumMember(Value = "LOAN")]
        Loan
    }

    class PurchaseUnit
    {
        public Amount Amount { get; set; }
        public string IndustryType { get; } = "ECOMMERCE";
    }

    class Amount
    {
        public decimal Value { get; set; }
        public Currency CurrencyCode { get; set; }
    }
}