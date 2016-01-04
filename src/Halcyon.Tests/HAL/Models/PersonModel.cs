using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.Tests.HAL.Models {
    public class PersonModel {
        public const string TestModelJson = "\"ID\":1,\"FirstName\":\"fname\",\"LastName\":\"lname\",\"display_name\":\"fname lname\"";

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

        public static PersonModel GetTestModel() {
            return new PersonModel() {
                ID = 1,
                FirstName = "fname",
                LastName = "lname"
            };
        }
    }
}
