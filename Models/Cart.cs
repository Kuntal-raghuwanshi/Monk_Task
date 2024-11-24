namespace Monk_Task.Models
{
    public class Cart
    {
        public List<Items> Items { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal? FinalPrice { get; set; }

    }
}
