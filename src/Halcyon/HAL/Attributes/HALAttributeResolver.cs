using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Halcyon.HAL.Attributes {
    public class HALAttributeResolver {

        public IHALModelConfig GetConfig(object model) {
            var type = model.GetType();

            // is it worth caching this?
            var classAttributes = type.GetTypeInfo().GetCustomAttributes();

            foreach(var attribute in classAttributes) {
                var modelAttribute = attribute as HalModelAttribute;
                if(modelAttribute != null) {
                    if(modelAttribute.ForceHal.HasValue || modelAttribute.LinkBase != null) {
                        var config = new HALModelConfig();
                        if(modelAttribute.ForceHal.HasValue) {
                            config.ForceHAL = modelAttribute.ForceHal.Value;
                        }
                        if(modelAttribute.LinkBase != null) {
                            config.LinkBase = modelAttribute.LinkBase;
                        }
                        return config;
                    }
                }
            }
            return null;
        }

        public IEnumerable<Link> GetLinks(object model) {
            var type = model.GetType();
            var classAttributes = type.GetTypeInfo().GetCustomAttributes();

            foreach(var attribute in classAttributes) {
                var linkAttribute = attribute as HalLinkAttribute;
                if(linkAttribute != null) {
                    yield return new Link(linkAttribute.Rel, linkAttribute.Href, linkAttribute.Title, linkAttribute.Method);
                }
            }
        }

        private IEnumerable<HALEmbeddedItem> GetEmbeddedCollections(object model, IHALModelConfig config) {
            var type = model.GetType();
            var embeddedModelProperties = type.GetTypeInfo().GetProperties().Where(x => x.IsDefined(typeof(HalEmbeddedAttribute)));

            foreach(var propertyInfo in embeddedModelProperties) {
                var embeddAttribute = propertyInfo.GetCustomAttribute(typeof(HalEmbeddedAttribute)) as HalEmbeddedAttribute;
                if(embeddAttribute == null) continue;

                var modelValue = propertyInfo.GetValue(model);
                var embeddedItems = modelValue as IEnumerable<object>;

                IEnumerable<HALResponse> halResponses = null;
                if (embeddedItems != null)
                {
                    halResponses = embeddedItems.Select(embeddedModel =>
                    {
                        var response = new HALResponse(embeddedModel, config);
                        AddEmbeddedResources(response, embeddedModel, config);
                        return response; 
                    });
                }
                else if (modelValue != null)
                {
                    var response = new HALResponse(modelValue, config);
                    AddEmbeddedResources(response, modelValue, config);
                    halResponses = new[] {response};
                }
                else
                {
                    continue;
                }

                yield return new HALEmbeddedItem(embeddAttribute.CollectionName, halResponses, embeddedItems != null); 
            }
            
        }
        public void AddEmbeddedResources(HALResponse response, object modelValue, IHALModelConfig config)
        {
            response.AddLinks(this.GetLinks(modelValue));
            var embeddedCollections = this.GetEmbeddedCollections(modelValue, config);
            foreach (var embedded in embeddedCollections)
            {
                if (embedded.IsCollection)
                {
                    response.AddEmbeddedCollection(embedded.ResourceName, embedded.HALResponses);
                }
                else
                {
                    response.AddEmbeddedResource(embedded.ResourceName, embedded.HALResponses.Single());
                }
            }
        }
    }
}