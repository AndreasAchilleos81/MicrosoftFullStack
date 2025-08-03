using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id:int}")]
    [Produces("application/json")]
    public ActionResult<User> GetUserById(int id)
    {
        var user = new User { Id = id, Name = "User" + id, Email = "user" + id + "@example.com" };
        return Ok(user);
    }

}