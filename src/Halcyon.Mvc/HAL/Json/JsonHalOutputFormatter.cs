using Halcyon.HAL;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Primitives;

namespace Halcyon.Web.HAL.Json {
    public class JsonHalOutputFormatter : IOutputFormatter, IApiResponseTypeMetadataProvider {
        public const string HalJsonType = "application/hal+json";

        private readonly IEnumerable<string> halJsonMediaTypes;
        private readonly JsonOutputFormatter jsonFormatter;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly IHALConverter[] converters;

        public JsonHalOutputFormatter(IEnumerable<string> halJsonMediaTypes = null, params IHALConverter[] converters) {
            if(halJsonMediaTypes == null) halJsonMediaTypes = new string[] { HalJsonType };

            this.serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();

            this.jsonFormatter = new JsonOutputFormatter(this.serializerSettings, ArrayPool<Char>.Create());

            this.halJsonMediaTypes = halJsonMediaTypes;

            this.converters = converters ?? new IHALConverter[0];
        }

        public JsonHalOutputFormatter(JsonSerializerSettings serializerSettings, IEnumerable<string> halJsonMediaTypes = null, params IHALConverter[] converters) {
            if(halJsonMediaTypes == null) halJsonMediaTypes = new string[] { HalJsonType };

            this.serializerSettings = serializerSettings;

            this.jsonFormatter = new JsonOutputFormatter(this.serializerSettings, ArrayPool<Char>.Create());

            this.halJsonMediaTypes = halJsonMediaTypes;

            this.converters = converters ?? new IHALConverter[0];
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context) {
            return context.ObjectType == typeof(HALResponse) || converters.Any(c => c.CanConvert(context.ObjectType)) ||
                   jsonFormatter.CanWriteResult(context);
        }

        public async Task WriteAsync(OutputFormatterWriteContext context) {
            var halResponse = context.Object as HALResponse;
            if (halResponse == null)
            {
                var converter = converters.FirstOrDefault(c => c.CanConvert(context.ObjectType));
                if (converter == null)
                {
                    await jsonFormatter.WriteAsync(context);
                    return;
                }

                halResponse = converter.Convert(context.Object);
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

            await jsonFormatter.WriteAsync(jsonContext);
        }

        public IReadOnlyList<string> GetSupportedContentTypes(string contentType, Type objectType)
        {
            var jsonTypes = jsonFormatter.GetSupportedContentTypes(contentType, objectType);

            // If we're not being asked about a specific type, send them all including the json types
            if (contentType == null)
            {
                // Add our hal types to the json types
                var allTypes = halJsonMediaTypes.ToList();
                allTypes.AddRange(jsonTypes);
                return allTypes;
            }

            // HAL types can't be subsets of those supported by the json formatter (correct?)
            // So return if supported json types are available
            if (jsonTypes != null)
                return jsonTypes;

            // Finally, return supported HAL types given that requested
            List<string> supportedHalTypes = null;
            var set = new MediaType(contentType);
            foreach (var halType in halJsonMediaTypes)
            {
                if (new MediaType(halType).IsSubsetOf(set))
                {
                    if (supportedHalTypes == null)
                        supportedHalTypes = new List<string>();
                    supportedHalTypes.Add(halType);
                }
            }

            return supportedHalTypes;
        }
    }
}
