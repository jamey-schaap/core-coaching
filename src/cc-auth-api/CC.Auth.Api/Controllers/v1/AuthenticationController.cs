using Asp.Versioning;

using CC.Auth.Api.Persistence.v1;

using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CC.Auth.Api.Controllers.v1;

[ApiVersion(1)]
[ApiController]
[Route("api/auth/v{apiVersion:apiVersion}")]
public class AuthenticationController(UserRepository userRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await userRepository.TryGetUses(request.Email, request.Password);
        return Ok();
    }
}
