using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Halcyon.HAL {
    public class HALModelConfig : IHALModelConfig {
        public string LinkBase { get; internal set; }
        public bool ForceHAL { get; internal set; }
    }
}