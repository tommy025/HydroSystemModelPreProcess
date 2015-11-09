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
    public class HydroDocument : IEnumerable<HydroProcedure>, INotifyCollectionChanged
    {
        //public static HydroDocument Load(Stream stream)
        //{
        //    var doc = new HydroDocument();
        //    var xdoc = XDocument.Load(stream);
        //    var version = xdoc.Root.Attribute("Version").Value;
        //    if (version != "1.0.0.0")
        //        throw new FileFormatException("Unsupported file version!");

        //    var xroot = xdoc.Element("HydroObjectFile");
        //    doc.hydroObjectGraph.LoadFromXmlFile(xroot);
        //    foreach (var xelement in xroot.Element("ElementInfo").Elements("FrameworkElement"))
        //    {
        //        doc.XmlDeserializeElement(xelement);
        //    }

        //    return doc;
        //}

        public HydroDocument()
        {
            hydroObjectGraph = new HydroObjectGraph();
            hydroProcedureList = new ObservableCollection<HydroProcedure>();
        }

        public IReadOnlyHydroObjectGraph HydroObjectGraph
        {
            get { return hydroObjectGraph; }
        }

        protected HydroObjectGraph hydroObjectGraph;

        protected ObservableCollection<HydroProcedure> hydroProcedureList;

        public HydroProcedure AddHeadLossCalculation()
        {
            var procedure = new HeadLossCalculation(HydroObjectGraph);
            hydroProcedureList.Add(procedure);
            return procedure;
        }

        public HydroProcedure DuplicatedProcedure(HydroProcedure procedure)
        {
            var dupProcedure = procedure.DeepClone();
            hydroProcedureList.Add(dupProcedure);
            return dupProcedure;
        }

        public bool RemoveProcedure(HydroProcedure procedure)
        {
            return hydroProcedureList.Remove(procedure);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { hydroProcedureList.CollectionChanged += value; }
            remove { hydroProcedureList.CollectionChanged -= value; }
        }

        public FrameworkElement[] GetElements()
        {
            return hydroObjectGraph.ToArray();
        }

        public Rectangle AddConnectNode(Point position)
        {
            var cNode = HydroResourceHelper.CreateVisualElement(typeof(ConnectNode)) as Rectangle;
            hydroObjectGraph.AddVertex(cNode, typeof(ConnectNode));
            MoveVertex(cNode, position - new Vector(cNode.Width / 2, cNode.Height / 2));
            return cNode;
        }

        public Line AddPressurePipe()
        {
            var pPipe = HydroResourceHelper.CreateVisualElement(typeof(PressurePipe)) as Line;
            hydroObjectGraph.AddEdge(pPipe, typeof(PressurePipe));
            Canvas.SetZIndex(pPipe, -1);
            return pPipe;
        }

        public void MoveVertex(Rectangle vertex, Point position)
        {
            Canvas.SetLeft(vertex, position.X);
            Canvas.SetTop(vertex, position.Y);

            var connectEdges = hydroObjectGraph.GetEdges(vertex);
            foreach (var edge in connectEdges)
            {
                if (hydroObjectGraph.GetVertex1(edge) == vertex)
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

        public FrameworkElement GetPropertySettingControl(FrameworkElement element)
        {
            var hydroObjectInfo = element.DataContext as IHydroObjectInfo;
            return HydroResourceHelper.GetHydroObjectPropertySettingControl(hydroObjectInfo.HydroObjectType);
        }

        public bool Remove(FrameworkElement element)
        {
            return hydroObjectGraph.Remove(element as Shape);
        }

        public IEnumerator<HydroProcedure> GetEnumerator()
        {
            return hydroProcedureList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hydroProcedureList.GetEnumerator();
        }

        //public void Save(Stream stream)
        //{
        //    var xdoc = new XDocument();
        //    xdoc.Add(new XElement("HydroObjectFile", new XAttribute("Version", "1.0.0.0"),
        //        hydroObjectGraph.SaveToXmlFile(),
        //        new XElement("ElementInfo",
        //            (from e in elementDictionary.Keys
        //             select XmlSerializeElement(e)).ToArray())));

        //    xdoc.Save(stream);
        //}

        //private XElement XmlSerializeElement(FrameworkElement element)
        //{
        //    var hObject = elementDictionary[element];
        //    if (element is Rectangle)
        //    {
        //        return new XElement("FrameworkElement",
        //            new XAttribute("ElementType", element.GetType().Name),
        //            new XAttribute("HydroObjectFullName", hObject.FullName),
        //            new XElement("Left", Canvas.GetLeft(element) + element.Width / 2),
        //            new XElement("Top", Canvas.GetTop(element) + element.Height / 2));
        //    }
        //    else if (element is Line)
        //    {
        //        var line = element as Line;
        //        return new XElement("FrameworkElement",
        //            new XAttribute("ElementType", element.GetType().Name),
        //            new XAttribute("HydroObjectFullName", hObject.FullName),
        //            new XElement("X1", line.X1),
        //            new XElement("Y1", line.Y1),
        //            new XElement("X2", line.X2),
        //            new XElement("Y2", line.Y2));
        //    }
        //    else
        //        throw new ArgumentException("Unsupported type " + element.GetType().FullName + " when serializing!");
        //}

        //private void XmlDeserializeElement(XElement xelement)
        //{
        //    var fullName = xelement.Attribute("HydroObjectFullName").Value;
        //    switch (xelement.Attribute("ElementType").Value)
        //    {
        //        case "Line":
        //            var pPipe = hydroObjectGraph.GetObject(fullName) as PressurePipe;
        //            var element = RegisterPressurePipe(Visibility.Hidden, pPipe);
        //            SetPipeFirstPoint(element,
        //                new Point(double.Parse(xelement.Element("X1").Value), double.Parse(xelement.Element("Y1").Value)));

        //            SetPipeSecondPoint(element,
        //                new Point(double.Parse(xelement.Element("X2").Value), double.Parse(xelement.Element("Y2").Value)));
        //            element.Visibility = Visibility.Visible;

        //            return;
        //        case "Rectangle":
        //            var cNode = hydroObjectGraph.GetObject(fullName) as ConnectNode;
        //            RegisterConnectNode(new Point(double.Parse(xelement.Element("Left").Value), double.Parse(xelement.Element("Top").Value)),
        //                cNode);

        //            return;
        //    }
        //}
    }
}
