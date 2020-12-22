namespace Readible.Models
{
    public class OrderStatistic
    {
        public string Property { get; set; }
        public int TotalOrder { get; set; }
        public int TotalItem { get; set; }
        public double TotalPrice { get; set; }

        public OrderStatistic(string property, int totalOrder, int totalItem, double totalPrice)
        {
            Property = property;
            TotalOrder = totalOrder;
            TotalItem = totalItem;
            TotalPrice = totalPrice;
        }
    }
}
