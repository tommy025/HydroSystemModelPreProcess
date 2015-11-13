namespace HydroSystemModelPreProcess
{
    public interface IHydroObjectGraph : IReadOnlyHydroObjectGraph
    {
        void AddVertex(IHydroVertexInfo vertex);

        void AddEdge(IHydroEdgeInfo edge);

        void Clear();

        bool Remove(IHydroObjectInfo item);

        void SetVertex1(IHydroEdgeInfo edge, IHydroVertexInfo vertex);

        void SetVertex2(IHydroEdgeInfo edge, IHydroVertexInfo vertex);

        void ConnectVertexs(IHydroEdgeInfo edge, IHydroVertexInfo vertex1, IHydroVertexInfo vertex2);

        void DisConnectVertexs(IHydroEdgeInfo edge);

        void DisConnectVertexs(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2);
    }
}