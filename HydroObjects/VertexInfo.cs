using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class VertexInfo
    {
        public VertexInfo(IEnumerable<EdgeInfo> edges)
        {
            this.edges = edges.ToArray();
        }

        private readonly EdgeInfo[] edges;

        public EdgeInfo GetEdgeNextTo(EdgeInfo edge)
        {
            var index = (edges as IList<EdgeInfo>).IndexOf(edge);
            if (index == edges.Length - 1)
                return null;
            else
                return edges[index + 1];
        }
    }
}
