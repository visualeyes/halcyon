# Halcyon 
![Build Status](https://ci.appveyor.com/api/projects/status/github/visualeyes/halcyon?branch=master&svg=true) 
[![Coverage Status](https://coveralls.io/repos/visualeyes/halcyon/badge.svg?branch=master&service=github)](https://coveralls.io/github/visualeyes/halcyon?branch=master)

**Halcyon** - 
[![Halcyon Nuget Version](https://img.shields.io/nuget/v/Halcyon.svg)](https://www.nuget.org/packages/Halcyon/) 
[![Halcyon Nuget Version](https://img.shields.io/nuget/vpre/Halcyon.svg)](https://www.nuget.org/packages/Halcyon/) 

**Halcyon.WebApi** *(ASP.NET Web API)* - 
[![Halcyon.WebApi Nuget Version](https://img.shields.io/nuget/v/Halcyon.WebApi.svg)](https://www.nuget.org/packages/Halcyon.WebApi/)
[![Halcyon.WebApi Nuget Version](https://img.shields.io/nuget/vpre/Halcyon.WebApi.svg)](https://www.nuget.org/packages/Halcyon.WebApi/)

**Halycon.Mvc** *(ASP.NET Core)* -
[![Halcyon.WebApi Nuget Version](https://img.shields.io/nuget/v/Halcyon.Mvc.svg)](https://www.nuget.org/packages/Halcyon.Mvc/)
[![Halcyon.WebApi Nuget Version](https://img.shields.io/nuget/vpre/Halcyon.Mvc.svg)](https://www.nuget.org/packages/Halcyon.Mvc/)


A HAL implementation for ASP.NET. Halcyon builds a HAL Model and lets ASP.NET take care of formatting the model into JSON.

## What is HAL?
> HAL is a simple format that gives a consistent and easy way to hyperlink between resources in your API.

For more information please see [the guide](https://github.com/mikekelly/hal_specification) and [the formal sepcification](http://stateless.co/hal_specification.html)

## Why Halcyon?

Halcyon was created with to meet two core requirements:

* The client should be able to chose whether or not to use HAL. The controller should return HAL or plain JSON based on the accept header. 
* The developer should not have to significantly modify Models to return HAL formatted JSON



For more info you can read the [blog post](https://medium.com/@johncmckim/halcyon-hal-for-net-ebc416844152)

## Getting Started with Halcyon

**Getting Halcyon**

For ASP.NET 4.5 - `Install-Package Halcyon.WebApi`

If you are using ASP.NET Core - `Install-Package Halcyon.Mvc -Pre`

**Using Halcyon**

Halcyon is simple to drop into an exising project as it does not require changes to existing models.
To start returing HAL from you API you simply need to call the HAL method in your Controller.
```c#
return HAL(model, new Link[] {
    new Link("self", "/api/foo/{id}"),
    new Link("foo:bar", "/api/foo/{id}/bar")
});
```
If you want more flexibility you can build a HALResponse using the Fluent API
```c#
var response = new HALResponse(model)
                .AddLinks(linkOne)
                .AddLinks(linkTwo)
                .AddEmbeddedCollection(embeddedName, embedded);
    
return this.Ok(response);
```
## Upgrading from 1.0 to 1.1

## Full Example
Links add discoverability by directing consumers to other Resources in the API.
The self link is a special link that references the current resource.

#### Controller 
```c#
[RoutePrefix("api/foo")]
public class FooController : Controller {
    
    [HttpGet, Route("{id:int}")]
    public IActionResult Get(int id) {
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
    public IActionResult GetBar(int fooId) {
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
```
#### Results

##### /api/foo/1
```json
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
```
##### /api/foo/1/bar
```json
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
      "id": 2,
      "fooId": 1, 
      "type": "bar"
    }]
  },
  "fooId": 1,
  "count": "2"
}
```
## Accept Header

By default HAL method returns HAL formatted responses regardless of the Accept Header.
If you need to return both HAL and standard JSON you can configure this using a ``JsonHALMediaTypeFormatter`` or ``JsonHalOutputFormatter```
depending on your version of MVC.

**MVC 5 and earlier**

```c#
config.Formatters.RemoveAt(0);
config.Formatters.Insert(0, new JsonHALMediaTypeFormatter(
    halJsonMediaTypes: new string[] { "application/hal+json", "application/vnd.example.hal+json", "application/vnd.example.hal.v1+json" },
    jsonMediaTypes: new string[] { "application/vnd.example+json", "application/vnd.example.v1+json" }
));
```

** MVC 6 and later**

```c#
public IServiceProvider ConfigureServices(IServiceCollection services) {
    services
      .AddMvc()
      .AddMvcOptions(c => {
          var jsonOutputFormatter = new JsonOutputFormatter();
          c.OutputFormatters.Add(new JsonHalOutputFormatter(
              jsonOutputFormatter,
              halJsonMediaTypes: new string[] { "application/hal+json", "application/vnd.example.hal+json", "application/vnd.example.hal.v1+json" }
          ));
      })
}
```
This will return responses without the HAL properties for requests that send an Accept Type included in the ``jsonMediaTypes`` array.
Embedded collections will be attached at the root of the JSON Object.

```json
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
```

## Compatability with HALON
[Halon](https://github.com/LeanKit-Labs/halon) is a HAL Javascript client that adds non-standard properties.

When creating a link you can optionally set the Method property to provide compatability with this client


## Credits
This project was inspiried by https://github.com/JakeGinnivan/WebApi.Hal. 
The fundamental difference is that Halcyon does not require large changes 
to your models to function.
