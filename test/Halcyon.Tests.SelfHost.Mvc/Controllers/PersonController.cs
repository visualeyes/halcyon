using Microsoft.AspNetCore.Mvc;
using Halcyon.Tests.HAL.Models;

namespace Halcyon.Tests.SelfHost.Mvc.Controllers {
    [Route("api/[controller]")]
    public class PersonController : Controller {

        [HttpGet("{id:int}")]
        public PersonModelWithAttributes Get(int id) {
            return new PersonModelWithAttributes {
                ID = id,
                FirstName = "Bob",
                LastName = "Smith"
            };
        }
    }
}
