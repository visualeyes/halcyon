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

        public IEnumerable<KeyValuePair<string, IEnumerable<HALResponse>>> GetEmbeddedCollections(object model, IHALModelConfig config) {
            var type = model.GetType();
            var embeddedModelProperties = type.GetTypeInfo().GetProperties().Where(x => x.IsDefined(typeof(HalEmbeddedAttribute)));

            foreach(var propertyInfo in embeddedModelProperties) {
                var embeddAttribute = propertyInfo.GetCustomAttribute(typeof(HalEmbeddedAttribute)) as HalEmbeddedAttribute;
                if(embeddAttribute == null) continue;

                var modelValue = propertyInfo.GetValue(model);

                var embeddedItems = modelValue as IEnumerable<object> ?? new List<object> { modelValue };

                var halResponses = embeddedItems.Select(embeddedModel => {
                    var response = new HALResponse(embeddedModel, config);
                    response.AddLinks(this.GetLinks(embeddedModel));
                    response.AddEmbeddedCollections(this.GetEmbeddedCollections(embeddedModel, config));

                    return response;
                });

                yield return new KeyValuePair<string, IEnumerable<HALResponse>>(embeddAttribute.CollectionName, halResponses);
            }
        }
    }
}