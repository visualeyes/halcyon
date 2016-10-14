using Halcyon.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Halcyon.HAL {
    public class Link {
        public const string RelForSelf = "self";

        private static readonly Regex isTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);

        private readonly bool replaceParameters;

        public Link(string rel, string href, string title = null, string method = null, bool replaceParameters = true, bool isRelArray = false) {
            this.Rel = rel;
            this.Href = href;
            this.Title = title;
            this.Method = method;
            this.replaceParameters = replaceParameters;
            this.IsRelArray = isRelArray;
        }

        [JsonIgnore]
        public string Rel { get; private set; }

        [JsonIgnore]
        public bool IsRelArray { get; private set; }

        [JsonProperty("href")]
        public string Href { get; private set; }

        [JsonProperty("templated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Templated {
            get {
                return !string.IsNullOrEmpty(Href) && isTemplatedRegex.IsMatch(Href) ? (bool?)true : null;
            }
        }

        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("deprecation", NullValueHandling = NullValueHandling.Ignore)]
        public string Deprecation { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        public string Profile { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("hreflang", NullValueHandling = NullValueHandling.Ignore)]
        public string HrefLang { get; set; }

        internal Link CreateLink(IDictionary<string, object> parameters) {
            var clone = Clone();

            if(replaceParameters && parameters != null) {
                if(!String.IsNullOrWhiteSpace(clone.Href)) {
                    clone.Href = clone.Href.SubstituteParams(parameters);
                }

                if(!String.IsNullOrWhiteSpace(clone.Title)) {
                    clone.Title = clone.Title.SubstituteParams(parameters);
                }
            }

            return clone;
        }

        internal Link RebaseLink(string baseUriString) {
            var clone = Clone();

            if(!String.IsNullOrWhiteSpace(baseUriString)) {
                var hrefUri = GetHrefUri(clone.Href);
                if(!hrefUri.IsAbsoluteUri) {
                    var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);

                    if(baseUri.IsAbsoluteUri) {
                        var rebasedUri = new Uri(baseUri, hrefUri);
                        clone.Href = rebasedUri.ToString();
                    } else {
                        // very simplistic but shoult work
                        clone.Href = String.Format("{0}/{1}", baseUriString.TrimEnd('/'), clone.Href.TrimStart('/'));
                    }
                }
            }

            return clone;
        }

        public Link Clone() {
            return (Link)MemberwiseClone();
        }

        private static Uri GetHrefUri(string href) {
            return new Uri(href, UriKind.RelativeOrAbsolute);
        }
    }
}