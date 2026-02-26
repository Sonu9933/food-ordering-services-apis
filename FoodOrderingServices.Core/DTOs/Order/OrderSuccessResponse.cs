using Customer.Core.Enum;

namespace FoodOrderingServices.Core.DTOs.Order
{
    public class OrderSuccessResponse
    {
        public Guid OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string ETA { get; set; }
    }
}
