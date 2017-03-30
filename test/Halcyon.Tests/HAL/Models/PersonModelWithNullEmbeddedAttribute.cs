using Halcyon.HAL.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Halcyon.Tests.HAL.Models
{
    [HalModel("~/api", true)]
    [HalLink("self", "person/{ID}")]
    [HalLink("person", "person/{ID}")]
    public class PersonModelWithNullEmbeddedAttribute
    {
        public const string TestModelJson = "\"ID\":1,\"FirstName\":\"fname\",\"LastName\":\"lname\",\"display_name\":\"fname lname\"";

        public int ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName
        {
            get { return this.FirstName + " " + this.LastName; }
        }

        [JsonIgnore]
        public string SpecialID
        {
            get { return this.DisplayName + "(" + this.ID + ")"; }
        }

        [HalEmbedded("pets")]
        public List<Pet> Pets { get; set; }

        [HalEmbedded("favouritePet")]
        public Pet FavouritePet { get; set; }
    }
}
