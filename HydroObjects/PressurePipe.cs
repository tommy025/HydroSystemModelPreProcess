using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class PressurePipe : HydroEdge
    {
        public static Line GetVisualElement()
        {
            return (Line)rdict["PressurePipe"];  
        }
    }
}
