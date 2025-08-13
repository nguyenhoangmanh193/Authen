namespace JwtAuth.Models.ProductDTO
{
    public class AddToCartRequestDTO
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
