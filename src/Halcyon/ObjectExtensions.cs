using System;
using System.Collections.Generic;
using System.Reflection;

namespace Halcyon {
    public static class ObjectExtensions {

        internal static IDictionary<string, object> ToDictionary(this object obj) {
            IDictionary<string, object> vardic;

            if (obj is IDictionary<string, object>) {
                vardic = (IDictionary<string, object>)obj;
            } else {
                vardic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                PropertyInfo[] properties = obj.GetType().GetProperties();

                foreach (var prop in properties) {
                    var objValue = prop.GetValue(obj, null);

                    vardic.Add(prop.Name, objValue);
                }
            }

            return vardic;
        }
    }
}
