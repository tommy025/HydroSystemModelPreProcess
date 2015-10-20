using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroVertex : HydroObject
    {
        public abstract Rectangle VertexVisualElement
        { get; }

        public sealed override FrameworkElement VisualElement
        {
            get
            {
                return VertexVisualElement;
            }
        }
    }
}
