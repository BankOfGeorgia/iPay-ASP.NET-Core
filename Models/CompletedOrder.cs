using System.Collections.Generic;

namespace IpayDemo.Net.Models
{
    public class CompletedOrder
    {
        public OrderStatus Status { get; set; }
        public string OrderId { get; set; }
        public string PaymentHash { get; set; }
        public List<Link> Links { get; set; }
    }

    public enum OrderStatus
    {
        Created
    }

    public class Link
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public Method Method { get; set; }
    }

    public enum Method
    {
        Get,
        Redirect
    }
}