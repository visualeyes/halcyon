using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Halcyon.HAL.Attributes;

namespace Halcyon.HAL
{
    public class HALAttributeResolver
    {
        private readonly object _model;
        private readonly Attribute[] _classAttributes;

        public HALAttributeResolver(object model)
        {
            _model = model;
            _classAttributes = Attribute.GetCustomAttributes(model.GetType());
        }

        public IHALModelConfig ResolveConfig()
        {
            foreach (var attribute in _classAttributes)
            {
                var modelAttribute = attribute as HalModelAttribute;
                if (modelAttribute != null)
                {
                    if (modelAttribute.ForceHal.HasValue || modelAttribute.LinkBase != null)
                    {
                        var config = new HALModelConfig();
                        if (modelAttribute.ForceHal.HasValue)
                        {
                            config.ForceHAL = modelAttribute.ForceHal.Value;
                        }
                        if (modelAttribute.LinkBase != null)
                        {
                            config.LinkBase = modelAttribute.LinkBase;
                        }
                        return config;
                    }
                }
            }
            return null;
        }

        public void ResolveAttributes(HALResponse halResponse)
        {
            foreach (var attribute in _classAttributes)
            {
                var linkAttribute = attribute as HalLinkAttribute;
                if (linkAttribute != null)
                {
                    halResponse.AddLinks(new Link(linkAttribute.Rel, linkAttribute.Href, linkAttribute.Title, linkAttribute.Method));
                }
            }

            var modelProperties = _model.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(HalPropertyAttribute)));
            foreach (var propertyInfo in modelProperties)
            {
                var modelValue = propertyInfo.GetValue(_model);
                if (modelValue == null) continue;
                var embeddAttribute = propertyInfo.GetCustomAttribute(typeof(HalEmbeddedAttribute)) as HalEmbeddedAttribute;
                if (embeddAttribute == null) continue;

                var embeddedItems = modelValue as IEnumerable<object> ?? new List<object> { modelValue };
                embeddedItems = embeddedItems.ToList();

                if (embeddedItems.Any())
                {
                    var embeddedResolver = new HALAttributeResolver(embeddedItems.First());
                    var halResponses = embeddedItems.Select(embeddedModel => new HALResponse(embeddedModel, halResponse.Config)).ToArray();
                    foreach (var response in halResponses)
                    {
                        embeddedResolver.ResolveAttributes(response);
                    }

                    halResponse.AddEmbeddedCollection(embeddAttribute.CollectionName, halResponses);
                }
            }
        }
    }
}