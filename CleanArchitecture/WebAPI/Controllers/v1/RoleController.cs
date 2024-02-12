using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using AuthorizationHandler.Lib.Genesys.Client;
using AuthorizationHandler.Lib.Configurations;

namespace WebAPI.Controllers.v1
{
    [Authorize]
    public class RoleController : ApiControllerBase
    {
        private readonly IOptions<GenesysAuthOptions> _options;

        public RoleController(IOptions<GenesysAuthOptions> options)
        {
            _options = options;
        }

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
            var client = new GenesysConfigServiceApiClient(_options.Value);
            var roles = await client.GetRolesFromGenesysConfig(employeeId, CancellationToken.None);

            if (roles == null)
                return NotFound();

            return Ok(roles);
        }
    }

}