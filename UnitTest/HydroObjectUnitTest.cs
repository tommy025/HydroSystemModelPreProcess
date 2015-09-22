using System;
using HydroSystemModelPreProcess.HydroObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroSystemModelPreProcess.UnitTest
{
    [TestClass]
    public class HydroObjectUnitTest
    {
        private HydroObjectGraph graph = new HydroObjectGraph();
        private HydroVertex vertexV1 = new ConnectNode();
        private HydroVertex vertexV2 = new ConnectNode();
        private HydroVertex vertexV3 = new ConnectNode();
        private HydroVertex vertexV4 = new ConnectNode();
        private HydroVertex vertexV5 = new ConnectNode();
        private HydroVertex vertexV6 = new ConnectNode();
        private HydroEdge edgeE1 = new PressurePipe();
        private HydroEdge edgeE2 = new PressurePipe();
        private HydroEdge edgeE3 = new PressurePipe();
        private HydroEdge edgeE4 = new PressurePipe();
        private HydroEdge edgeE5 = new PressurePipe();
        private HydroEdge edgeE6 = new PressurePipe();
        private HydroEdge edgeE7 = new PressurePipe();

        [TestMethod]
        public void TestHydroObjectGrap_Build()
        {
            graph.Add(vertexV1);
            graph.Add(vertexV2);
            graph.Add(edgeE1);
            graph.SetVertex1(edgeE1, vertexV1);
            graph.SetVertex2(edgeE1, vertexV2);

            graph.ConnectVertexs(edgeE2, vertexV2, vertexV5);
            graph.ConnectVertexs(edgeE3, vertexV2, vertexV3);
            graph.ConnectVertexs(edgeE4, vertexV3, vertexV5);
            graph.ConnectVertexs(edgeE5, vertexV3, vertexV4);
            graph.ConnectVertexs(edgeE6, vertexV1, vertexV4);
            graph.ConnectVertexs(edgeE7, vertexV4, vertexV6);          
        }

        [TestMethod]
        public void TestHydroObjectGrap_Modify()
        {
            TestHydroObjectGrap_Build();

            graph.Remove(vertexV6);

            graph.TrySetVertex(edgeE7, vertexV5);

            graph.Remove(edgeE7);
        }
    }
}
