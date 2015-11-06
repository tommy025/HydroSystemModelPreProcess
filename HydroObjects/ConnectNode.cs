using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class ConnectNode : HydroVertex
    {
        public static Rectangle GetVisualElement()
        {
            return (Rectangle)rdict["ConnectNodeIcon"];
        }

        protected override XElement[] ToXml()
        {
            return base.ToXml();
        }

        public ConnectNode() : this(DateTime.Now)
        { }

        public ConnectNode(DateTime _creationTime, string _name = "") : base(_creationTime, _name)
        { }
    }
}
