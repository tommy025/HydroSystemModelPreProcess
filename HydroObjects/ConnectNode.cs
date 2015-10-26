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
        public static Rectangle GetVisualElement()
        {
            return (Rectangle)rdict["ConnectNodeIcon"];
        }
    }
}
