using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Halcyon {
    public static class ObjectExtensions {

        internal static IDictionary<string, object> ToDictionary(this object obj) {
            IDictionary<string, object> vardic;

            if (obj is IDictionary<string, object>) {
                vardic = (IDictionary<string, object>)obj;
            } else if (obj is JObject) {
                var jObj = ((JObject)obj);
                vardic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                var properties = jObj.Properties();

                foreach (var prop in properties) {
                    var objValue = prop.Value;

                    vardic.Add(prop.Name, objValue);
                }
            } else {
                vardic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                var properties = obj.GetType().GetProperties();

                foreach (var prop in properties) {
                    var objValue = prop.GetValue(obj, null);

                    vardic.Add(prop.Name, objValue);
                }
            }

            return vardic;
        }
    }
}
