using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroVertex : HydroObject, IIsVertex
    {
        private VertexInfo vertexInfo;

        VertexInfo IIsVertex.VertexInfo
        {
            get { return vertexInfo; }
            set { vertexInfo = value; }
        }

    }
}
