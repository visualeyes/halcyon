# Halcyon 
![Build Status](https://ci.appveyor.com/api/projects/status/github/visualeyes/halcyon?branch=master&svg=true) 
[![Coverage Status](https://coveralls.io/repos/visualeyes/halcyon/badge.svg?branch=master&service=github)](https://coveralls.io/github/visualeyes/halcyon?branch=master)
[![Halcyon Nuget Version](https://img.shields.io/nuget/v/Halcyon.svg)](https://www.nuget.org/packages/Halcyon/)
[![Halcyon Nuget Downloads](https://img.shields.io/nuget/dt/Halcyon.svg)](https://www.nuget.org/packages/Halcyon/)

A HAL implementation for ASP.NET. Halcyon builds a HAL Model and lets ASP.NET take care of formatting the model into JSON or XML.

## What is HAL?
> HAL is a simple format that gives a consistent and easy way to hyperlink between resources in your API.

For more information please see [the guide](https://github.com/mikekelly/hal_specification) and [the formal sepcification](http://stateless.co/hal_specification.html)

## Getting Started with Halcyon
Halcyon is simple to drop into an exising project as it does not require changes to existing models.
To start returing HAL from you API you simply need to call the HAL method in your ApiController.

    return HAL(model, new Link[] {
        new Link("self", "/api/foo/{id}"),
        new Link("foo:bar", "/api/foo/{id}/bar")
    });

## Full Example
Links add discoverability by directing consumers to other Resources in the API.
The self link is a special link that references the current resource.

#### ApiController 

    [RoutePrefix("api/foo")]
    public class FooController : ApiController {
    
        [HttpGet, Route("{id:int}")]
        public IHttpActionResult Get(int id) {
            // Any plain old object will do
            var fooModel = new {
                id = id,
                type = "foo"
            };
            
            // Return a simple resource with links to related resources
            return this.HAL(fooModel, new Link[] {
                new Link("self", "/api/foo/{id}"),
                new Link("foo:bar", "/api/foo/{id}/bar")
            });
        }
    
        [HttpGet, Route("{fooId:int}/bars")]
        public IHttpActionResult GetBar(int fooId) {
            // A collection of bars related to foo
            var bars = new List<object> {
                new { id = 1, fooId = fooId, type = "bar" },
                new { id = 2, fooId = fooId, type = "bar" }
            };
    
            // data about the bars related to foo
            var fooBarModel = new {
                fooId = fooId,
                count = bars.Count
            };
    
            // Return a fooBar resource with embedded bars
            return this.HAL(
                fooBarModel,
                new Link[] {
                    new Link("self", "/api/foo/{fooId}/bar")
                },
                "bars",
                bars,
                new Link[] {
                    new Link("self", "/api/bar/{id}")
                }
            );
        }
    }

#### Results

##### /api/foo/1

    {
      "_links": {
        "self": {
          "href": "/api/foo/1"
        },
        "foo:bar": {
          "href": "/api/foo/1/bar"
        }
      },
      "id": 1,
      "type": "foo"
    }

##### /api/foo/1/bar

    {
      "_links": {
        "self": {
          "href": "/api/foo/1/bar"
        }
      },
      "_embedded": {
        "bars": [{
          "_links": {
            "self": {
              "href": "/api/foo/1"
            }
          },
          "id": 1,
          "fooId": 1, 
          "type": "bar"
        },{
          "_links": {
            "self": {
              "href": "/api/foo/2"
            }
          },
          "id": 2
          "fooId": 1, 
          "type": "bar"
        }]
      }
      "fooId": 1,
      "count": "2"
    }

## Accept Header

By default HAL method returns HAL formatted responses regardless of the Accept Header.
If you need to return both HAL and standard JSON you can configure this using the ``JsonHALMediaTypeFormatter``.

    config.Formatters.RemoveAt(0);
    config.Formatters.Insert(0, new JsonHALMediaTypeFormatter(
        halJsonMediaTypes: new string[] { "application/hal+json", "application/vnd.example.hal+json", "application/vnd.example.hal.v1+json" },
        jsonMediaTypes: new string[] { "application/vnd.example+json", "application/vnd.example.v1+json" }
    ));

This will return responses without the HAL properties for requests that send an Accept Type included in the ``jsonMediaTypes`` array.
Embedded collections will be attached at the root of the JSON Object.


    {
      "fooId": 1,
      "count": 2,
      "bars": [
        {
          "id": 1,
          "fooId": 1,
          "type": "bar"
        },
        {
          "id": 2,
          "fooId": 1,
          "type": "bar"
        }
      ]
    }


## Compatability with HALON
[Halon](https://github.com/LeanKit-Labs/halon) is a HAL Javascript client that adds non-standard properties.

When creating a link you can optionally set the Method property to provide compatability with this client


## Credits
This project was inspiried by https://github.com/JakeGinnivan/WebApi.Hal. 
The fundamental difference is that Halcyon does not require large changes 
to your models to function.
