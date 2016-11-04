using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Halcyon {
    public static class ObjectExtensions {

        internal static IDictionary<string, object> ToDictionary(this object obj) {
            IDictionary<string, object> vardic;

            if(obj is IDictionary<string, object>) {
                vardic = (IDictionary<string, object>)obj;
            } else if(obj is JObject) {
                vardic = ToDictionary((JObject)obj);
            } else {
                vardic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                var properties = obj.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

                foreach(var prop in properties) {
                    var objValue = prop.GetValue(obj, null);

                    vardic.Add(prop.Name, objValue);
                }
            }

            return vardic;
        }

        internal static IDictionary<string, object> ToDictionary(this JObject obj) {
            var vardic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            var properties = obj.Properties();

            foreach(var prop in properties) {
                var objValue = prop.Value;
                vardic.Add(prop.Name, objValue);
            }

            return vardic;
        }
    }
}
