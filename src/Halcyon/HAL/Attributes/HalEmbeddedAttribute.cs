using System;

namespace Halcyon.HAL.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HalEmbeddedAttribute : HalPropertyAttribute {
        public string CollectionName { get; }

        public HalEmbeddedAttribute(string collectionName) {
            CollectionName = collectionName;
        }
    }
}