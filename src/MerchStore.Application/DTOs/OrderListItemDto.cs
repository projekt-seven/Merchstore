namespace MerchStore.Application.DTOs
{
    public class OrderListItemDto
    {
        public Guid Id { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
