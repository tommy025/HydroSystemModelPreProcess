using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class ConnectNode : HydroVertex
    {
        public override Rectangle VertexVisualElement
        {
            get
            {
                return (Rectangle)rdict["ConnectNodeIcon"];
            }
        }
    }
}
