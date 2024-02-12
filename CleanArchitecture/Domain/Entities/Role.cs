using Domain.Contracts;

namespace Domain.Entities
{
    public class Role : AuditableEntity<Guid>
    {
        public string EmployeeId { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}