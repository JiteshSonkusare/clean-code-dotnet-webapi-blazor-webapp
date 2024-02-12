using MediatR;
using AutoMapper;
using Domain.Entities;
using Application.Interfaces.Repositories;

namespace Application.Features.Roles.Queries.GetAll
{
    public class GetRolesByEmployeeIdQuery : IRequest<List<string>>
    {
        public string EmployeeId { get; set; } = null!;
    }

    internal class GetAllRoleQueryHandler : IRequestHandler<GetRolesByEmployeeIdQuery, List<string>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;

        public GetAllRoleQueryHandler(IUnitOfWork<Guid> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<string>> Handle(GetRolesByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.Repository<Role>().Entities();
                var roles = result.Where(r => r.EmployeeId.Equals(request.EmployeeId));
                return roles.Select(r => r.Name).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get User data! Error: {ex.Message}");
            }
        }
    }
}
