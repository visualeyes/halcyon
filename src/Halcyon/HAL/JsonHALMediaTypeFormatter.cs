using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Halcyon.HAL {
    public class JsonHALMediaTypeFormatter : JsonMediaTypeFormatter {
        private const string HalJsonType = "application/hal+json";

        private readonly string[] jsonMediaTypes;

        public JsonHALMediaTypeFormatter(string[] halJsonMedaiTypes = null, string[] jsonMediaTypes = null)
            : base() {
            if (halJsonMedaiTypes == null) halJsonMedaiTypes = new string[] { HalJsonType };
            if (jsonMediaTypes == null) jsonMediaTypes = new string[] { };

            this.jsonMediaTypes = jsonMediaTypes;

            foreach (var mediaType in halJsonMedaiTypes) {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            foreach (var mediaType in jsonMediaTypes.Where(t => t != JsonMediaTypeFormatter.DefaultMediaType.MediaType)) {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }
        }

        public override bool CanReadType(Type type) {
            return base.CanReadType(type);
        }

        public override bool CanWriteType(Type type) {
            return type == typeof(HALModel) || base.CanWriteType(type);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {
            return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext) {
            
            // If it is a HAL response but set to application/json - convert to a plain response
            if(type == typeof(HALModel) && value != null) {
                var halResponse = ((HALModel)value);

                string mediaType = content.Headers.ContentType.MediaType;
                if (!halResponse.Config.ForceHAL && (jsonMediaTypes.Contains(mediaType) || mediaType == JsonMediaTypeFormatter.DefaultMediaType.MediaType)) {
                    value = halResponse.ToPlainResponse();
                }
            }

            return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
        }
    }
}