using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL.Json {
    public class JsonHALModelConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(HALResponse);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var model = value as HALResponse;
            if(model == null) return;

            var output = model.ToJObject(serializer);
            output.WriteTo(writer);
        }
    }
}
