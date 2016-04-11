using Halcyon.HAL.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Halcyon.HAL.Filters;

namespace Halcyon.HAL {
    [JsonConverter(typeof(JsonHALModelConverter))]
    public class HALResponse {
        public const string LinksKey = "_links";
        public const string EmbeddedKey = "_embedded";

        private readonly IHALModelConfig config;

        private readonly object dto;
        private readonly List<Link> links = new List<Link>();
        private readonly Dictionary<string, IEnumerable<HALResponse>> embedded = new Dictionary<string, IEnumerable<HALResponse>>();

        public HALResponse(IHALModelConfig config) {
            this.config = config ?? new HALModelConfig();
        }

        public HALResponse(object model, IHALModelConfig config = null)
            : this(config) {
            if (!(model is JObject) && (model is IEnumerable)) {
                throw new ArgumentException("The HAL model should be Enumerable. You should use an embedded collection instead", "model");
            }

            var attributes = Attribute.GetCustomAttributes(model.GetType());
            foreach (var attribute in attributes)
            {
                if (attribute is IHalModelAttribute)
                {
                    var modelConfig = (IHalModelAttribute) attribute;
                    this.config = new HALModelConfig {ForceHAL = modelConfig.ForceHal, LinkBase = modelConfig.LinkBase};
                }
                if (attribute is IHalLinkAttribute)
                {
                    var link = (IHalLinkAttribute)attribute;
                    this.links.Add(new Link(link.Rel, link.Href, link.Title, link.Method));
                }
            }

            this.dto = model;
        }

        public IHALModelConfig Config {
            get { return config; }
        }

        public bool HasLink(string rel) {
            return links.Any(l => l.Rel == rel);
        }

        public HALResponse AddLinks(IEnumerable<Link> links) {
            this.links.AddRange(links);
            return this;
        }

        public HALResponse AddEmbeddedCollection(string name, IEnumerable<HALResponse> objects) {
            embedded.Add(name, objects);
            return this;
        }

        public object ToPlainResponse(JsonSerializer serializer, bool attachEmbedded = true) {
            var output = GetBaseJObject(serializer);

            if (this.embedded.Any()) {
                var plainEmbedded = this.embedded.ToDictionary(
                        e => e.Key,
                        e => e.Value.Select(m => m.ToPlainResponse(serializer))
                );

                var embeddedObject = JObject.FromObject(plainEmbedded);

                output.Merge(embeddedObject);
            }

            return output;
        }

        public JObject ToJObject(JsonSerializer serializer) {
            var output = GetBaseJObject(serializer);

            if (this.links.Any()) {
                var linksOutput = new JObject();

                var dtoProps = this.dto?.ToDictionary() ?? new Dictionary<string, object>();
                var resolvedLinks = GetResolvedLinks(this.links, dtoProps, this.config.LinkBase);

                foreach (var link in resolvedLinks) {
                    if (link.Value is IEnumerable) {
                        var linksOuput = JArray.FromObject(link.Value);
                        linksOutput.Add(link.Key, linksOuput);
                    } else {
                        var linkOuput = JObject.FromObject(link.Value);
                        linksOutput.Add(link.Key, linkOuput);
                    }
                }

                output.Add(LinksKey, linksOutput);
            }

            if (this.embedded.Any()) {
                var embeddedOutput = new JObject();
                foreach (var embedPair in this.embedded) {
                    embeddedOutput.Add(embedPair.Key, new JArray(embedPair.Value.Select(m => m.ToJObject(serializer))));
                }

                output.Add(EmbeddedKey, embeddedOutput);
            }

            return output;
        }

        private JObject GetBaseJObject(JsonSerializer serializer) {
            JObject output;

            if (this.dto != null) {
                output = JObject.FromObject(this.dto, serializer);
            } else {
                output = new JObject();
            }

            return output;
        }

        private static Dictionary<string, object> GetResolvedLinks(IEnumerable<Link> links, IDictionary<string, object> properties, string linkBase) {
            var subsituted = links;

            if (properties.Any()) {
                subsituted = links.Select(l => l.CreateLink(properties)).ToList();
            }

            var resolved = subsituted;

            if (!String.IsNullOrWhiteSpace(linkBase)) {
                resolved = subsituted.Select(l => l.RebaseLink(linkBase)).ToList();
            }

            var grouped = resolved.GroupBy(r => r.Rel);

            var singles = grouped.Where(g => g.Count() <= 1).ToDictionary(k => k.Key, v => v.SingleOrDefault() as object);
            var lists = grouped.Where(g => g.Count() > 1).ToDictionary(k => k.Key, v => v.AsEnumerable() as object);

            var allLinks = singles.Concat(lists).ToDictionary(k => k.Key, v => v.Value);

            return allLinks;
        }
    }
}