namespace JwtAuth.Models.ProductDTO
{
    public class PlaceOrderRequestDTO
    {
        public Guid UserId { get; set; }
        public List<OrderItemRequestDTO> Items { get; set; } = new();
    }
    public class OrderItemRequestDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
