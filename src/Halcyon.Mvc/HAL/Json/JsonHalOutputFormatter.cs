using Halcyon.HAL;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Halcyon.Web.HAL.Json {
    public class JsonHalOutputFormatter : IOutputFormatter {
        public const string HalJsonType = "application/hal+json";

        private readonly IEnumerable<string> halJsonMediaTypes;
        private readonly JsonOutputFormatter jsonFormatter;
        private readonly JsonSerializerSettings serializerSettings;

        public JsonHalOutputFormatter(IEnumerable<string> halJsonMediaTypes = null) {
            if(halJsonMediaTypes == null) halJsonMediaTypes = new string[] { HalJsonType };

            this.serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();

            this.jsonFormatter = new JsonOutputFormatter(this.serializerSettings, ArrayPool<Char>.Create());

            this.halJsonMediaTypes = halJsonMediaTypes;
        }

        public JsonHalOutputFormatter(JsonSerializerSettings serializerSettings, IEnumerable<string> halJsonMediaTypes = null) {
            if(halJsonMediaTypes == null) halJsonMediaTypes = new string[] { HalJsonType };

            this.jsonFormatter = new JsonOutputFormatter(serializerSettings, ArrayPool<Char>.Create());

            this.halJsonMediaTypes = halJsonMediaTypes;
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context) {
            return context.ObjectType == typeof(HALResponse) || jsonFormatter.CanWriteResult(context);
        }

        public Task WriteAsync(OutputFormatterWriteContext context) {
            var halResponse = context.Object as HALResponse;
            if (halResponse == null)
            {
                return jsonFormatter.WriteAsync(context);
            }

            string mediaType = context.ContentType.HasValue ? context.ContentType.Value : null;

            object value = null;

            // If it is a HAL response but set to application/json - convert to a plain response
            var serializer = JsonSerializer.Create(this.serializerSettings);

            if(!halResponse.Config.ForceHAL && !halJsonMediaTypes.Contains(mediaType)) {
                value = halResponse.ToPlainResponse(serializer);
            } else {
                value = halResponse.ToJObject(serializer);
            }

            var jsonContext = new OutputFormatterWriteContext(context.HttpContext, context.WriterFactory, value.GetType(), value);
            jsonContext.ContentType = new StringSegment(mediaType);

            return jsonFormatter.WriteAsync(jsonContext);
        }
    }
}
