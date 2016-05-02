using Halcyon.HAL;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Halcyon.WebApi.HAL.Json {
    public class JsonHALMediaTypeFormatter : JsonMediaTypeFormatter {
        private const string HalJsonType = "application/hal+json";

        private readonly string[] jsonMediaTypes;
        private readonly IHALConverter[] converters;

        public JsonHALMediaTypeFormatter(string[] halJsonMediaTypes = null, string[] jsonMediaTypes = null, params IHALConverter[] converters) {
            if (halJsonMediaTypes == null) halJsonMediaTypes = new string[] { HalJsonType };
            if (jsonMediaTypes == null) jsonMediaTypes = new string[] { };

            this.jsonMediaTypes = jsonMediaTypes;
            this.converters = converters ?? new IHALConverter[0];

            foreach (var mediaType in halJsonMediaTypes) {
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
            return type == typeof(HALResponse) || base.CanWriteType(type);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {
            return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext) {
            // If it is a HAL response but set to application/json - convert to a plain response
            HALResponse halResponse = null;

            if (TryGetHalResponse(type, value, out halResponse)) {
                var serializer = this.CreateJsonSerializer();

                string mediaType = content.Headers.ContentType.MediaType;
                if (!halResponse.Config.ForceHAL && (jsonMediaTypes.Contains(mediaType) || mediaType == JsonMediaTypeFormatter.DefaultMediaType.MediaType)) {
                    value = halResponse.ToPlainResponse(serializer);
                } else {
                    value = halResponse.ToJObject(serializer);
                }
            }

            return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
        }

        private bool TryGetHalResponse(Type type, object value, out HALResponse response) {
            foreach (var converter in converters.Where(c => c.CanConvert(type))) {
                response = converter.Convert(value);
                return true;
            }

            if (type == typeof(HALResponse) && value != null) {
                response = (HALResponse)value;
                return true;
            }

            response = null;
            return false;
        }
    }
}