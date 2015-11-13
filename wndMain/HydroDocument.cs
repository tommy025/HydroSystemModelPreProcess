using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using HydroSystemModelPreProcess.HydroObjects;

namespace HydroSystemModelPreProcess
{
    public class HydroDocument : IEnumerable<IHydroProcedure>, INotifyCollectionChanged
    {
        #region Constructors

        public HydroDocument()
        {
            hydroObjectGraph = new HydroObjectGraph();
            hydroObjectInfoList = new List<IHydroObjectInfo>();
            hydroProcedureList = new ObservableCollection<IHydroProcedure>();
        }

        #endregion

        #region Fields

        protected IHydroObjectGraph hydroObjectGraph;

        protected List<IHydroObjectInfo> hydroObjectInfoList;

        protected ObservableCollection<IHydroProcedure> hydroProcedureList;

        #endregion

        #region Methods

        public IHydroProcedure AddHeadLossCalculation()
        {
            var procedure = new HeadLossCalculation(hydroObjectGraph);
            hydroProcedureList.Add(procedure);
            return procedure;
        }

        public IHydroProcedure DuplicatedProcedure(IHydroProcedure procedure)
        {
            var dupProcedure = procedure.CustomizedClone();
            hydroProcedureList.Add(dupProcedure);
            return dupProcedure;
        }

        public bool RemoveProcedure(IHydroProcedure procedure)
        {
            return hydroProcedureList.Remove(procedure);
        }

        public Rectangle AddConnectNode(Point position)
        {
            var cNode = HydroResourceHelper.CreateVisualElement(typeof(ConnectNode)) as Rectangle;
            var cNodeInfo = HydroVertexInfo.CreateHydroVertexInfo(cNode, typeof(ConnectNode));
            cNodeInfo.Left = position.X - cNode.Width / 2;
            cNodeInfo.Top = position.Y - cNode.Height / 2;
            hydroObjectGraph.AddVertex(cNodeInfo);
            hydroObjectInfoList.Add(cNodeInfo); 
            return cNode;
        }

        public Line AddPressurePipe(Point point1, Point point2)
        {
            var pPipe = HydroResourceHelper.CreateVisualElement(typeof(PressurePipe)) as Line;
            var pPipeInfo = HydroEdgeInfo.CreateHydroEdgeInfo(pPipe, typeof(PressurePipe));
            pPipeInfo.X1 = point1.X;
            pPipeInfo.Y1 = point1.Y;
            pPipeInfo.X2 = point2.X;
            pPipeInfo.Y2 = point2.Y;
            hydroObjectGraph.AddEdge(pPipeInfo);
            hydroObjectInfoList.Add(pPipeInfo);
            return pPipe;
        }

        public bool RemoveHydroObject(FrameworkElement element)
        {
            var hydroObjectInfo = hydroObjectInfoList.Single(i => i.Element == element);
            hydroObjectGraph.Remove(hydroObjectInfo);
            return hydroObjectInfoList.Remove(hydroObjectInfo);
        }

        public Line[] GetConnectedEdges(Rectangle vertex)
        {
            if (vertex == null)
                return new Line[] { };

            var hydroVertexInfo = hydroObjectInfoList.Single(i => i.Element == vertex) as IHydroVertexInfo;
            var edgeInfoList = hydroObjectGraph.GetEdges(hydroVertexInfo);
            return edgeInfoList.Select(e => e.Edge).ToArray();
        }

        public Line[] GetConnectedEdges(Rectangle vertex1, Rectangle vertex2)
        {
            if (vertex1 == null || vertex2 == null)
                return new Line[] { };

            var hydroVertexInfo1 = hydroObjectInfoList.Single(i => i.Element == vertex1) as IHydroVertexInfo;
            var hydroVertexInfo2 = hydroObjectInfoList.Single(i => i.Element == vertex2) as IHydroVertexInfo;
            var edgeInfoList = hydroObjectGraph.GetEdges(hydroVertexInfo1, hydroVertexInfo2);
            return edgeInfoList.Select(e => e.Edge).ToArray();
        }

        public Rectangle GetVertex1(Line edge)
        {
            var hydroEdgeInfo = hydroObjectInfoList.Single(i => i.Element == edge) as IHydroEdgeInfo;
            var vertexInfo = hydroObjectGraph.GetVertex1(hydroEdgeInfo);
            return vertexInfo == null ? null : vertexInfo.Vertex;
        }

        public Rectangle GetVertex2(Line edge)
        {
            var hydroEdgeInfo = hydroObjectInfoList.Single(i => i.Element == edge) as IHydroEdgeInfo;
            var vertexInfo = hydroObjectGraph.GetVertex2(hydroEdgeInfo);
            return vertexInfo == null ? null : vertexInfo.Vertex;
        }

        public bool SetVertex1(Line edge, Rectangle vertex)
        {
            var hydroEdgeInfo = hydroObjectInfoList.Single(i => i.Element == edge) as IHydroEdgeInfo;            
            if (vertex == null)
            {
                hydroObjectGraph.SetVertex1(hydroEdgeInfo, null);
                return true;
            }

            var hydroVertexInfo = hydroObjectInfoList.Single(i => i.Element == vertex) as IHydroVertexInfo;
            if (hydroObjectGraph.GetVertex2(hydroEdgeInfo) == hydroVertexInfo)
                return false;

            if (hydroObjectGraph.GetVertex1(hydroEdgeInfo) != hydroVertexInfo)
                hydroObjectGraph.SetVertex1(hydroEdgeInfo, hydroVertexInfo);

            return true;
        }

        public bool SetVertex2(Line edge, Rectangle vertex)
        {
            var hydroEdgeInfo = hydroObjectInfoList.Single(i => i.Element == edge) as IHydroEdgeInfo;           
            if (vertex == null)
            {
                hydroObjectGraph.SetVertex2(hydroEdgeInfo, null);
                return true;
            }

            var hydroVertexInfo = hydroObjectInfoList.Single(i => i.Element == vertex) as IHydroVertexInfo;
            if (hydroObjectGraph.GetVertex1(hydroEdgeInfo) == hydroVertexInfo)
                return false;

            if (hydroObjectGraph.GetVertex2(hydroEdgeInfo) != hydroVertexInfo)
                hydroObjectGraph.SetVertex2(hydroEdgeInfo, hydroVertexInfo);

            return true;
        }

        #endregion

        #region IEnumerable<IHydroProcedure>

        public IEnumerator<IHydroProcedure> GetEnumerator()
        {
            return hydroProcedureList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hydroProcedureList.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { hydroProcedureList.CollectionChanged += value; }
            remove { hydroProcedureList.CollectionChanged -= value; }
        }

        #endregion
    }
}
