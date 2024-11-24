namespace Monk_Task.Models
{
    public class Coupons 
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public CouponDetails Details { get; set; }
        public DateTime Expiry { get; set; }
    }
    public class CouponDetails
    {
        public int? CouponId { get; set; }
        public decimal? Threshold { get; set; }
        public decimal? Discount { get; set; }
        public int? ProductId { get; set; }
        public List<Items>? BuyProducts { get; set; }
        public List<Items>? GetProducts { get; set; }
        public int? RepitionLimit { get; set; }
    }
    public class ApplicableCoupons
    {
        public int CouponId { get; set; }
        public string Type { get; set; }
        public decimal Discount { get; set; }
    }
}
