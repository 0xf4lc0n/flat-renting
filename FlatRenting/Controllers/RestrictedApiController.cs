using FlatRenting.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlatRenting.Controllers;

[ApiController]
[Authorize]
public class RestrictedApiController : ControllerBase {
    protected User GetAuthorizedUserFromCtx() => (User)HttpContext.Items["User"];
}
