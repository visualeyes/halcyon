using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavis.UriTemplates;

namespace Halcyon.Templates {
    public static class TemplateExtensions {

        public static string SubstituteParams(this string uriTemplateString, IDictionary<string, object> parameters) {
            var uriTemplate = new UriTemplate(uriTemplateString);

            foreach(var parameter in parameters) {
                var name = parameter.Key;
                var value = parameter.Value;

                var substituionValue = value == null ? null : value.ToString();
                uriTemplate.SetParameter(name, substituionValue);
            }

            return uriTemplate.Resolve();
        }
    }
}
