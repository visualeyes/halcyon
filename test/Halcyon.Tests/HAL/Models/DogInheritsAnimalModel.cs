namespace Halcyon.Tests.HAL.Models
{
    public class DogInheritsAnimalModel : Animal
    {
        public string Name { get; set; }
    }
    public class Animal
    {
        public string Species { get; set; }
    }
}