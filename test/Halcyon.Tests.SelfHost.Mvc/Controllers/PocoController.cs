using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Tests.SelfHost.Mvc.Controllers {
    [Route("api/[controller]")]
    public class PocoController : Controller {

        [HttpGet("{id:int}")]
        public object Get(int id) {
            // HALResponse is not required
            return new {
                id,
                type = "POCO"
            };
        }
    }
}
