using Halcyon.WebApi.HAL.Filters;
using Newtonsoft.Json;

namespace Halcyon.Tests.HAL.Models
{
    [HalModel("~/api", true)]
    [HalLink("self", "person/{ID}")]
    [HalLink("person", "person/{ID}")]
    public class PersonModelWithAttributes
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

        public static PersonModel GetTestModel()
        {
            return new PersonModel()
            {
                ID = 1,
                FirstName = "fname",
                LastName = "lname"
            };
        }
    }
}