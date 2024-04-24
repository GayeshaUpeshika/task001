using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class GreetingController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get(string name)
    {
        return $"Hello, my name is: {name}";
    }
}