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
        public PressurePipe()
        { }

        public PressurePipe(PressurePipe other) : base(other)
        {
            roughness = other.Roughness;
        }

        public override HydroObject DeepClone()
        {
            return new PressurePipe(this);
        }

        protected override XElement[] ToXml()
        {
            return new XElement[]
            {
                new XElement("Roughness", Roughness)
            };
        }

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
