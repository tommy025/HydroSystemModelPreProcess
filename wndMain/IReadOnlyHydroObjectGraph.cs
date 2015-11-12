using System.Collections.Generic;
using System.Collections.Specialized;

namespace HydroSystemModelPreProcess
{
    public interface IReadOnlyHydroObjectGraph : IEnumerable<IHydroObjectInfo>, INotifyCollectionChanged
    {
        bool Contains(IHydroObjectInfo item);

        int Count
        { get; }

        IHydroVertexInfo GetVertex1(IHydroEdgeInfo edge);

        IHydroVertexInfo GetVertex2(IHydroEdgeInfo edge);

        bool IsConnected(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2);

        bool IsConnectedTo(IHydroEdgeInfo edge, IHydroVertexInfo vertex);

        bool IsBetween(IHydroEdgeInfo edge, IHydroVertexInfo vertex1, IHydroVertexInfo vertex2);

        IHydroVertexInfo[] GetAllVertexs();

        IHydroVertexInfo[] GetVertexs(IHydroEdgeInfo edge);

        IHydroEdgeInfo[] GetAllEdges();

        IHydroEdgeInfo[] GetEdges(IHydroVertexInfo vertex);

        IHydroEdgeInfo[] GetEdges(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2);
    }
}
