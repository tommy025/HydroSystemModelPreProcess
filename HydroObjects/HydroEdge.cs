using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroEdge : HydroObject, IIsEdge
    {
        private EdgeInfo edgeInfo;

        EdgeInfo IIsEdge.EdgeInfo
        {
            get { return edgeInfo; }
            set { edgeInfo = value; }
        }
    }
}
