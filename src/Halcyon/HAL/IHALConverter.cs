using System;

namespace Halcyon.HAL
{
    public interface IHALConverter
    {
        bool CanConvert(Type type, object model);
        HALResponse Convert(object model);
    }
}