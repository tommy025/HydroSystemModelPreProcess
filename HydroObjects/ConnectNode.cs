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
        public ConnectNode()
        { }

        public ConnectNode(ConnectNode other) : base(other)
        { }

        public override HydroObject DeepClone()
        {
            return new ConnectNode(this);
        }

        protected override XElement[] ToXml()
        {
            return base.ToXml();
        }
    }
}
