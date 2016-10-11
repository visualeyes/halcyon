using Halcyon.HAL.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Halcyon.HAL {
    [JsonConverter(typeof(JsonHALModelConverter))]
    public class HALResponse {
        public const string LinksKey = "_links";
        public const string EmbeddedKey = "_embedded";

        private readonly IHALModelConfig config;

        private readonly object model;
        private readonly List<Link> links = new List<Link>();
        private readonly Dictionary<string, object> embedded = new Dictionary<string, object>();

        public HALResponse(IHALModelConfig config) {
            this.config = config ?? new HALModelConfig();
        }

        public HALResponse(object model, IHALModelConfig config = null)
            : this(config) {
            if(!(model is JObject) && (model is IEnumerable)) {
                throw new ArgumentException("The HAL model should not be Enumerable. You should use an embedded collection instead", nameof(model));
            }
            this.model = model;
        }

        public object Model { get { return model; } }

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

        public HALResponse AddEmbeddedResource(string name, HALResponse resource) {
            embedded.Add(name, resource);
            return this;
        }

        public HALResponse AddEmbeddedCollection(string name, IEnumerable<HALResponse> objects) {
            embedded.Add(name, objects);
            return this;
        }

        public JObject ToPlainResponse(JsonSerializer serializer, bool attachEmbedded = true) {
            var output = GetBaseJObject(serializer);

            if(this.embedded.Any()) {
                var embeddedOutput = EmbeddedToJObject((m) => m.ToPlainResponse(serializer));
                output.Merge(embeddedOutput);
            }

            return output;
        }

        public JObject ToJObject(JsonSerializer serializer) {
            var output = GetBaseJObject(serializer);

            if(this.links.Any()) {
                var linksOutput = new JObject();

                var dtoProps = this.model?.ToDictionary() ?? new Dictionary<string, object>();
                var resolvedLinks = GetResolvedLinks(this.links, dtoProps, this.config.LinkBase);

                foreach(var link in resolvedLinks) {
                    if(link.Value is IEnumerable) {
                        var linksOuput = JArray.FromObject(link.Value);
                        linksOutput.Add(link.Key, linksOuput);
                    } else {
                        var linkOuput = JObject.FromObject(link.Value);
                        linksOutput.Add(link.Key, linkOuput);
                    }
                }

                output.Add(LinksKey, linksOutput);
            }

            if(this.embedded.Any()) {
                var embeddedOutput = EmbeddedToJObject((m) => m.ToJObject(serializer));
                output.Add(EmbeddedKey, embeddedOutput);
            }

            return output;
        }

        private JObject EmbeddedToJObject(Func<HALResponse, JObject> converter) {
            var embeddedOutput = new JObject();
            foreach(var embedPair in this.embedded) {

                if(embedPair.Value is IEnumerable<HALResponse>) {
                    embeddedOutput.Add(embedPair.Key, JArray.FromObject(((IEnumerable<HALResponse>)embedPair.Value).Select(m => converter(m))));
                } else if(embedPair.Value is HALResponse) {
                    embeddedOutput.Add(embedPair.Key, JObject.FromObject(converter((HALResponse)embedPair.Value)));
                } else {
                    throw new NotImplementedException();
                }
            }

            return embeddedOutput;
        }

        private JObject GetBaseJObject(JsonSerializer serializer) {
            JObject output;

            if(this.model != null) {
                output = JObject.FromObject(this.model, serializer);
            } else {
                output = new JObject();
            }

            return output;
        }

        private static Dictionary<string, object> GetResolvedLinks(IEnumerable<Link> links, IDictionary<string, object> properties, string linkBase) {
            var subsituted = links;

            if(properties.Any()) {
                subsituted = links.Select(l => l.CreateLink(properties)).ToList();
            }

            var resolved = subsituted;

            if(!String.IsNullOrWhiteSpace(linkBase)) {
                resolved = subsituted.Select(l => l.RebaseLink(linkBase)).ToList();
            }

            var grouped = resolved.GroupBy(r => r.Rel);

            var singles = grouped.Where(g => g.Count() <= 1 && g.All(l => !l.IsRelArray)).ToDictionary(k => k.Key, v => v.SingleOrDefault() as object);
            var lists = grouped.Where(g => g.Count() > 1 || g.Any(l => l.IsRelArray)).ToDictionary(k => k.Key, v => v.AsEnumerable() as object);

            var allLinks = singles.Concat(lists).ToDictionary(k => k.Key, v => v.Value);

            return allLinks;
        }
    }
}
