using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroEdge : HydroObject
    {
        public abstract FrameworkElement EdgeVisualElement
        { get; }

        public sealed override FrameworkElement VisualElement
        {
            get
            {
                return EdgeVisualElement;
            }
        }
    }
}
