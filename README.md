# Halcyon
A HAL implementation for ASP.NET

## What is HAL?
> HAL is a simple format that gives a consistent and easy way to hyperlink between resources in your API.

For more information please see https://github.com/mikekelly/hal_specification and http://stateless.co/hal_specification.html  


## Getting Started with Halcyon
Halcyon is simple to drop into an exising project as it does not require changes to existing models.

#### ApiController

    [HttpGet]
    [Route("{int:id}")]
    public async Task<IHttpActionResult> Get(int id) {
        // Any plain old object will do
        var model = new {
            id = id,
            foo = "bar",
            complex = new {
                baz = "qux"
            }
        }

        return this.HAL(model, new Link(Link.RelForSelf, "/api/path/to/route/{id}"));
    }

#### Result

    {
      "_links": {
        "self": {
          "href": "/api/path/to/route/1"
        }
      },
      "id": 1,
      "foo": "bar",
      "complex": {
        "baz": "qux"
      }
    }

## Compatability with HALON
Halon (https://github.com/LeanKit-Labs/halon) is a HAL Javascript client that adds non-standard properties.

When creating a link you can optionally set the Method property to provide compatability with this client


## Credits
This project was inspiried by https://github.com/JakeGinnivan/WebApi.Hal. 
The fundamental difference is that Halcyon does not require large changes 
to your models to function.
