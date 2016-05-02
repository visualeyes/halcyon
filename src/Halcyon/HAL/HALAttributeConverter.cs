using System;
using System.Linq;
using Halcyon.HAL.Attributes;

namespace Halcyon.HAL
{
    public class HALAttributeConverter : IHALConverter
    {
        public bool CanConvert(Type type, object model)
        {
            var response = model as HALResponse;
            if (response == null)
            {
                return Attribute.GetCustomAttributes(type).Any(x => x is HalModelAttribute) && model != null;
            }
            return Attribute.GetCustomAttributes(response.Model.GetType()).Any(x => x is HalModelAttribute);
        }

        public HALResponse Convert(object model)
        {
            HALAttributeResolver resolver;
            var response = model as HALResponse;
            if (response == null)
            {
                resolver = new HALAttributeResolver(model);
                var config = resolver.ResolveConfig();
                response = new HALResponse(model, config);
            }
            else
            {
                resolver = new HALAttributeResolver(response.Model);
            }
            resolver.ResolveAttributes(response);
            return response;
        }
    }
}