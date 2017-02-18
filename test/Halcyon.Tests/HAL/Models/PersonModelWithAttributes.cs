using System.Collections.Generic;
using Halcyon.HAL.Attributes;
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

        [HalEmbedded("pets")]
        public List<Pet> Pets { get; set; } = new List<Pet>();

        [HalEmbedded("favouritePet")]
        public Pet FavouritePet { get; set; } = new Pet { Id = 0, Name = "Benji" };
    }

    [HalLink("self", "pets/{Id}")]
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [HalEmbedded("toys")]
        public List<PetToy> PetToys { get; set; } = new List<PetToy> { new PetToy { Name = "Rubber Bone" } };
    }

    [HalLink("self", "pets/{Id}/toys/{Name}")]
    public class PetToy
    {
        public string Name { get; set; }
    }
}