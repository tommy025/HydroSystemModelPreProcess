using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class EdgeInfo
    {
        private VertexInfo vertex1;

        public VertexInfo Vertex1
        {
            get { return vertex1; }
            set
            {
                vertex1.Edges.Remove(this);
                vertex1 = value;

                if (vertex1 != null)
                    vertex1.Edges.Add(this);
            }
        }

        private VertexInfo vertex2;

        public VertexInfo Vertex2
        {
            get { return vertex2; }
            set
            {
                vertex2.Edges.Remove(this);
                vertex2 = value;

                if (vertex1 != null)
                    vertex2.Edges.Add(this);
            }
        }

        public EdgeInfo NextEdgeOfV1
        {
            get { return Vertex1 != null ? Vertex1.GetEdgeNextTo(this) : null; }
        }

        public EdgeInfo NextEdgeOfV2
        {
            get { return Vertex2 != null ? Vertex2.GetEdgeNextTo(this) : null; }
        }
    }
}
