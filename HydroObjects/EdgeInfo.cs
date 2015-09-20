using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class EdgeInfo
    {
        private readonly VertexInfo vertex1;

        public VertexInfo Vertex1
        {
            get { return vertex1; }
        }

        private readonly VertexInfo vertex2;

        public VertexInfo Vertex2
        {
            get { return vertex2; }
        }

        public EdgeInfo NextEdgeOfV1
        {
            get { return Vertex1.GetEdgeNextTo(this); }
        }

        public EdgeInfo NextEdgeOfV2
        {
            get { return Vertex2.GetEdgeNextTo(this); }
        }
    }
}
