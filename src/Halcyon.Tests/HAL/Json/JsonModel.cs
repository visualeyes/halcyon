using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.Tests.HAL.Json {
    public class JsonModel {

        public int ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName {
            get { return this.FirstName + " " + this.LastName; }
        }

        [JsonIgnore]
        public string SpecialID {
            get { return this.DisplayName + "(" + this.ID + ")"; }
        }
    }
}
