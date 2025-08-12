using ECommerceAPI.Domain.Entities.Comman;

namespace ECommerceAPI.Domain.Entities
{
    public class CompletedOrder : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}
