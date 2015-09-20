using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class HydroObjectGraph
    {
        private List<HydroVertex> vertexs;

        public ICollection<HydroVertex> Vertexs
        {
            get { return vertexs; }
        }

        private List<HydroEdge> edges;

        public ICollection<HydroEdge> Edges
        {
            get { return edges; }
        }
    }
}
