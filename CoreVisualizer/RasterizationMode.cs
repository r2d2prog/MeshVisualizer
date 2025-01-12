using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreVisualizer
{
    [Flags]
    public enum RasterizationMode
    { 
        None = 0,
        Shaded = 1,
        Wireframe = 2,
        Points = 4,
        Full = Shaded | Wireframe | Points
    }
}
