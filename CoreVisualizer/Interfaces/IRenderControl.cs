using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreVisualizer.Interfaces
{
    public interface IRenderControl
    {
        void DoRender();
        void AlignCamera(ViewPlane plane);
    }
}
