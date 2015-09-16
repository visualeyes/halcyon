using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Halcyon.HAL {
    public class HALModel : DynamicObject {
        public const string LinksKey = "_links";
        public const string EmbeddedKey = "_embedded";

        private readonly IHALModelConfig config;
        
        private readonly IDictionary<string, object> properties = new Dictionary<string, object>();
        private readonly List<Link> links = new List<Link>();
        private readonly Dictionary<string, IEnumerable<HALModel>> embedded = new Dictionary<string, IEnumerable<HALModel>>();

        public HALModel(IHALModelConfig config) {
            this.config = config ?? new HALModelConfig();
        }

        public HALModel(IDictionary<string, object> model, IHALModelConfig config = null) 
            : this(config) {
            this.properties = model;
        }

        public HALModel(object model, IHALModelConfig config = null)
            : this(model.ToDictionary(), config) {
        }

        public IHALModelConfig Config {
            get { return config; }
        }

        public override IEnumerable<string> GetDynamicMemberNames() {
            var keys = properties.Keys.ToList();

            if (links.Any()) {
                keys.Add(LinksKey);
            }

            if (embedded.Any()) {
                keys.Add(EmbeddedKey);
            }

            return keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            var key = binder.Name;

            if(key == LinksKey) {
                var subsituted = links;

                if (properties.Any()) {
                    subsituted = links.Select(l => l.CreateLink(properties)).ToList();
                }

                var resolved = subsituted;

                if (!String.IsNullOrWhiteSpace(this.config.LinkBase)) {
                    resolved = subsituted.Select(l => l.RebaseLink(this.config.LinkBase)).ToList();
                }

                result = resolved.ToDictionary(l => l.Rel);
                return true;
            } else if(key == EmbeddedKey) {
                result = embedded;
                return true;
            }

            return properties.TryGetValue(key, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            var key = binder.Name;

            if(key == LinksKey || key == EmbeddedKey) {
                throw new ArgumentException(LinksKey + " and " + EmbeddedKey + " are protected");
            }

            properties[binder.Name] = value;

            return true;
        }

        public HALModel AddLinks(IEnumerable<Link> links) {
            this.links.AddRange(links);
            return this;
        }

        public HALModel AddEmbeddedCollection(string name, IEnumerable<HALModel> objects) {
            embedded.Add(name, objects);
            return this;
        }

        public IDictionary<string, object> ToPlainResponse(bool attachEmbedded = true) {
            var plainResponse = new Dictionary<string, object>(properties); 

            if(attachEmbedded) {
                foreach (var item in embedded) {
                    plainResponse.Add(item.Key, item.Value.Select(i => i.ToPlainResponse(attachEmbedded: attachEmbedded)));
                }
            }

            return plainResponse;
        }

    }
}