using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using HydroSystemModelPreProcess.HydroObjects;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

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
            var cNodeInfo = new HydroVertexInfo(cNode, typeof(ConnectNode));
            hydroObjectGraph.AddVertex(cNodeInfo);
            hydroObjectInfoList.Add(cNodeInfo);
            MoveVertex(cNode, position);
            return cNode;
        }

        public Line AddPressurePipe()
        {
            var pPipe = HydroResourceHelper.CreateVisualElement(typeof(PressurePipe)) as Line;
            var pPipeInfo = new HydroEdgeInfo(pPipe, typeof(PressurePipe));
            hydroObjectGraph.AddEdge(pPipeInfo);
            hydroObjectInfoList.Add(pPipeInfo);
            Canvas.SetZIndex(pPipe, -1);
            return pPipe;
        }

        public bool RemoveHydroObject(FrameworkElement element)
        {
            var hydroObjectInfo = hydroObjectInfoList.Single(i => i.Element == element);
            hydroObjectGraph.Remove(hydroObjectInfo);
            return hydroObjectInfoList.Remove(hydroObjectInfo);
        }

        public void MoveVertex(Rectangle vertex, Point position)
        {
            Canvas.SetLeft(vertex, position.X - vertex.Width / 2);
            Canvas.SetTop(vertex, position.Y - vertex.Height / 2);

            var vertexInfo = hydroObjectInfoList.Single(i => i.Element == vertex) as IHydroVertexInfo;
            var connectEdges = hydroObjectGraph.GetEdges(vertexInfo);
            foreach (var edge in connectEdges)
            {
                if (hydroObjectGraph.GetVertex1(edge) == vertexInfo)
                {
                    edge.X1 = position.X + vertex.Width / 2;
                    edge.Y1 = position.Y + vertex.Height / 2;
                }
                else
                {
                    edge.X2 = position.X + vertex.Width / 2;
                    edge.Y2 = position.Y + vertex.Height / 2;
                }
            }
        }

        public void SetPipeFirstPoint(Line edge, Point position)
        {
            edge.X1 = position.X;
            edge.Y1 = position.Y;
        }

        public void SetPipeSecondPoint(Line edge, Point position)
        {
            edge.X2 = position.X;
            edge.Y2 = position.Y;            
        }

        public bool SetPipeFirstVertex(Line edge, Rectangle vertex)
        {
            if (vertex == null)
            {
                hydroObjectGraph.SetVertex1(edge, null);
                return true;
            }

            if (hydroObjectGraph.GetVertex2(edge) == vertex)
                return false;

            if (hydroObjectGraph.GetVertex1(edge) != vertex)
                hydroObjectGraph.SetVertex1(edge, vertex);

            SetPipeFirstPoint(edge, new Point(
                Canvas.GetLeft(vertex) + vertex.Width / 2,
                Canvas.GetTop(vertex) + vertex.Height / 2));

            return true;
        }

        public bool SetPipeSecondVertex(Line edge, Rectangle vertex)
        {
            if (vertex == null)
            {
                hydroObjectGraph.SetVertex2(edge, null);
                return true;
            }

            if (hydroObjectGraph.GetVertex1(edge) == vertex)
                return false;

            var otherVertex = hydroObjectGraph.GetVertex1(edge);
            if (hydroObjectGraph.IsConnected(otherVertex, vertex) &&
                !hydroObjectGraph.IsBetween(edge, vertex, otherVertex))
                return false;

            if (hydroObjectGraph.GetVertex2(edge) != vertex)
                hydroObjectGraph.SetVertex2(edge, vertex);

            SetPipeSecondPoint(edge, new Point(
                Canvas.GetLeft(vertex) + vertex.Width / 2,
                Canvas.GetTop(vertex) + vertex.Height / 2));

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
