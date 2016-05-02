using System.Collections.Generic;
using Halcyon.HAL.Attributes;
using Newtonsoft.Json;

namespace Halcyon.Tests.HAL.Models {
    [HalModel("~/api", true)]
    [HalLink("self", "person?index={PageIndex}")]
    [HalLink("next", "person?index={NextIndex}"), HalLink("prev", "person?index={PrevIndex}")]
    public class PagedPeopleModelWithAttributes {
        [JsonIgnore]
        public int PageIndex { get; set; }

        public IEnumerable<PersonModel> People { get; set; }
    }
}