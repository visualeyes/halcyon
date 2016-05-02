using Halcyon.HAL;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Halcyon.Web.HAL.Json {
    public class JsonHalOutputFormatter : IOutputFormatter {
        public const string HalJsonType = "application/hal+json";

        private readonly IEnumerable<string> halJsonMediaTypes;
        private readonly JsonOutputFormatter jsonFormatter;


        public JsonHalOutputFormatter(JsonOutputFormatter jsonFormatter, IEnumerable<string> halJsonMediaTypes = null) {
            if(halJsonMediaTypes == null) halJsonMediaTypes = new string[] { HalJsonType };

            this.jsonFormatter = jsonFormatter;

            this.halJsonMediaTypes = halJsonMediaTypes;
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context) {
            return context.ObjectType == typeof(HALResponse);
        }

        public async Task WriteAsync(OutputFormatterWriteContext context) {
            string mediaType = context.ContentType.MediaType;

            object value = null;
            var halResponse = ((HALResponse)context.Object);

            // If it is a HAL response but set to application/json - convert to a plain response
            var serializer = Newtonsoft.Json.JsonSerializer.Create(jsonFormatter.SerializerSettings);

            if(!halResponse.Config.ForceHAL && !halJsonMediaTypes.Contains(mediaType)) {
                value = halResponse.ToPlainResponse(serializer);
            } else {
                value = halResponse.ToJObject(serializer);
            }

            var jsonContext = new OutputFormatterWriteContext(context.HttpContext, context.WriterFactory, value.GetType(), value);

            await jsonFormatter.WriteAsync(jsonContext);
        }
    }
}
