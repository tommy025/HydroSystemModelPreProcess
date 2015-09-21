using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class VertexInfo
    {
        public VertexInfo(IEnumerable<EdgeInfo> edges)
        {
            this.edges = new List<EdgeInfo>(edges);
        }

        private readonly List<EdgeInfo> edges;

        public ICollection<EdgeInfo> Edges
        {
            get { return edges; }
        }

        public EdgeInfo GetEdgeNextTo(EdgeInfo edge)
        {
            var index = (edges as IList<EdgeInfo>).IndexOf(edge);
            if (index == edges.Count - 1)
                return null;
            else
                return edges[index + 1];
        }
    }
}
