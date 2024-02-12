using Microsoft.AspNetCore.Mvc;
using Application.Features.Roles.Queries.GetAll;

namespace WebAPI.Controllers.v1
{
    /// <summary>
    /// Its Fake Controller, it provides fake roles for authorization.
    /// </summary>
    public class FakeController : ApiControllerBase
    {
        /// <summary>
        /// Get Roles by EmployeeId
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [HttpGet("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetRolesByEmployeeId([FromRoute] string employeeId)
        {
            var roles = await Mediator.Send(new GetRolesByEmployeeIdQuery() { EmployeeId = employeeId });
            if (roles == null)
                return NotFound();

            return Ok(roles);
        }
    }
}
