using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess
{
    public interface IHydroObjectGraph : IReadOnlyHydroObjectGraph
    {
        void AddVertex(IHydroVertexInfo vertex);

        void AddEdge(IHydroEdgeInfo edge);

        void Clear();

        bool Remove(IHydroObjectInfo item);

        IHydroVertexInfo SetVertex1(IHydroEdgeInfo edge);

        IHydroVertexInfo SetVertex2(IHydroEdgeInfo edge);

        void ConnectVertexs(IHydroEdgeInfo edge, IHydroVertexInfo vertex1, IHydroVertexInfo vertex2);

        void DisConnectVertexs(IHydroEdgeInfo edge);

        void DisConnectVertexs(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2);
    }
}
