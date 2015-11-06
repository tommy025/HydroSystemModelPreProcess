using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class PressurePipe : HydroEdge
    {
        public static Line GetVisualElement()
        {
            return (Line)rdict["PressurePipe"];  
        }

        public static FrameworkElement GetPropertySettingControl()
        {
            return rdict["PressurePipePropertyControl"] as FrameworkElement;
        }

        protected override XElement[] ToXml()
        {
            return new XElement[]
            {
                new XElement("Roughness", Roughness)
            };
        }

        public PressurePipe() : this(DateTime.Now)
        { }

        public PressurePipe(DateTime _creationTime, string _name = "") : base(_creationTime, _name)
        { }

        private double roughness;

        public double Roughness
        {
            get { return roughness; }
            set
            {
                roughness = value;
                TriggerPropertyChangedEvent("Roughness");
            }
        }
    }
}
