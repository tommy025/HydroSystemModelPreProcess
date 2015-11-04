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

namespace HydroSystemModelPreProcess
{
    public class HydroDocument
    {
        public static HydroDocument Load(Stream stream)
        {
            var doc = new HydroDocument();
            var xdoc = XDocument.Load(stream);
            var version = xdoc.Root.Attribute("Version").Value;
            if (version != "1.0.0.0")
                throw new FileFormatException("Unsupported file version!");

            var xroot = xdoc.Element("HydroObjectFile");
            doc.hydroObjectGraph.LoadFromXmlFile(xroot);
            foreach (var xelement in xroot.Element("ElementInfo").Elements("FrameworkElement"))
            {
                doc.XmlDeserializeElement(xelement);
            }

            return doc;
        }

        public HydroDocument()
        {
            hydroObjectGraph = new HydroObjectGraph();
            elementDictionary = new Dictionary<FrameworkElement, HydroObject>();
        }

        protected HydroObjectGraph hydroObjectGraph;

        protected Dictionary<FrameworkElement, HydroObject> elementDictionary;

        public FrameworkElement[] GetElements()
        {
            return elementDictionary.Keys.ToArray(); 
        }

        public HydroObject GetHydroObject(FrameworkElement element)
        {
            return elementDictionary[element];
        }

        public Rectangle AddConnectNode(Point position)
        {
            var cNode = new ConnectNode();
            hydroObjectGraph.Add(cNode);
            return RegisterConnectNode(position, cNode);
        }

        private Rectangle RegisterConnectNode(Point position, ConnectNode cNode)
        {
            var element = ConnectNode.GetVisualElement();
            elementDictionary.Add(element, cNode);

            Canvas.SetLeft(element, position.X - element.Width / 2);
            Canvas.SetTop(element, position.Y - element.Height / 2);
            return element;
        }

        public Line AddPressurePipe(Visibility visibility)
        {
            var pPipe = new PressurePipe();
            hydroObjectGraph.Add(pPipe);
            return RegisterPressurePipe(visibility, pPipe);     
        }

        private Line RegisterPressurePipe(Visibility visibility, PressurePipe pPipe)
        {
            var element = PressurePipe.GetVisualElement();
            elementDictionary.Add(element, pPipe);

            Canvas.SetZIndex(element, -1);
            element.Visibility = visibility;
            return element;
        }

        public void MoveNode(Rectangle node, Point position)
        {
            var selectedObject = elementDictionary[node];
            Canvas.SetLeft(node, position.X);
            Canvas.SetTop(node, position.Y);

            var connectEdges = hydroObjectGraph.GetEdges(selectedObject as HydroVertex);
            foreach (var edge in connectEdges)
            {
                var element = elementDictionary.First(kvp => { return kvp.Value == edge; }).Key as Line;
                if (hydroObjectGraph.GetVertex1(edge as HydroEdge) == selectedObject)
                {
                    element.X1 = position.X + node.Width / 2;
                    element.Y1 = position.Y + node.Height / 2;
                }
                else
                {
                    element.X2 = position.X + node.Width / 2;
                    element.Y2 = position.Y + node.Height / 2;
                }
            }
        }

        public void SetPipeFirstPoint(Line pPipe, Point position)
        {
            pPipe.X1 = position.X;
            pPipe.Y1 = position.Y;
        }

        public void SetPipeSecondPoint(Line pPipe, Point position)
        {
            pPipe.X2 = position.X;
            pPipe.Y2 = position.Y;            
        }

        public bool SetPipeFirstNode(Line pPipe, Rectangle node)
        {
            var hEdge = elementDictionary[pPipe] as HydroEdge;
            if (node == null)
            {
                hydroObjectGraph.SetVertex1(hEdge, null);
                return true;
            }

            var hVertex = elementDictionary[node] as HydroVertex;
            if (hydroObjectGraph.GetVertex2(hEdge) == hVertex)
                return false;

            if (hydroObjectGraph.GetVertex1(hEdge) != hVertex)
                hydroObjectGraph.SetVertex1(hEdge, hVertex);

            SetPipeFirstPoint(pPipe, new Point(
                Canvas.GetLeft(node) + node.Width / 2,
                Canvas.GetTop(node) + node.Height / 2));

            return true;
        }

        public bool SetPipeSecondNode(Line pPipe, Rectangle node)
        {
            var hEdge = elementDictionary[pPipe] as HydroEdge;
            if (node == null)
            {
                hydroObjectGraph.SetVertex2(hEdge, null);
                return true;
            }

            var hVertex = elementDictionary[node] as HydroVertex;
            if (hydroObjectGraph.GetVertex1(hEdge) == hVertex)
                return false;

            var hOtherVertex = hydroObjectGraph.GetVertex1(hEdge);
            if (hydroObjectGraph.IsConnected(hOtherVertex, hVertex) &&
                !hydroObjectGraph.IsBetween(hEdge, hVertex, hOtherVertex))
                return false;

            if (hydroObjectGraph.GetVertex2(hEdge) != hVertex)
                hydroObjectGraph.SetVertex2(hEdge, hVertex);

            SetPipeSecondPoint(pPipe, new Point(
                Canvas.GetLeft(node) + node.Width / 2,
                Canvas.GetTop(node) + node.Height / 2));

            return true;
        }

        public void Remove(FrameworkElement element)
        {
            hydroObjectGraph.Remove(elementDictionary[element]);
            elementDictionary.Remove(element);
        }

        public void Save(Stream stream)
        {
            var xdoc = new XDocument();
            xdoc.Add(new XElement("HydroObjectFile", new XAttribute("Version", "1.0.0.0"),
                hydroObjectGraph.SaveToXmlFile(),
                new XElement("ElementInfo",
                    (from e in elementDictionary.Keys
                     select XmlSerializeElement(e)).ToArray())));

            xdoc.Save(stream);
        }

        private XElement XmlSerializeElement(FrameworkElement element)
        {
            var hObject = elementDictionary[element];
            if (element is Rectangle)
            {
                return new XElement("FrameworkElement",
                    new XAttribute("ElementType", element.GetType().Name),
                    new XAttribute("HydroObjectFullName", hObject.FullName),
                    new XElement("Left", Canvas.GetLeft(element) + element.Width / 2),
                    new XElement("Top", Canvas.GetTop(element) + element.Height / 2));
            }
            else if (element is Line)
            {
                var line = element as Line;
                return new XElement("FrameworkElement",
                    new XAttribute("ElementType", element.GetType().Name),
                    new XAttribute("HydroObjectFullName", hObject.FullName),
                    new XElement("X1", line.X1),
                    new XElement("Y1", line.Y1),
                    new XElement("X2", line.X2),
                    new XElement("Y2", line.Y2));
            }
            else
                throw new ArgumentException("Unsupported type " + element.GetType().FullName + " when serializing!");
        }

        private void XmlDeserializeElement(XElement xelement)
        {
            var fullName = xelement.Attribute("HydroObjectFullName").Value;
            switch (xelement.Attribute("ElementType").Value)
            {
                case "Line":
                    var pPipe = hydroObjectGraph.GetObject(fullName) as PressurePipe;
                    var element = RegisterPressurePipe(Visibility.Hidden, pPipe);
                    SetPipeFirstPoint(element,
                        new Point(double.Parse(xelement.Element("X1").Value), double.Parse(xelement.Element("Y1").Value)));

                    SetPipeSecondPoint(element,
                        new Point(double.Parse(xelement.Element("X2").Value), double.Parse(xelement.Element("Y2").Value)));
                    element.Visibility = Visibility.Visible;

                    return;
                case "Rectangle":
                    var cNode = hydroObjectGraph.GetObject(fullName) as ConnectNode;
                    RegisterConnectNode(new Point(double.Parse(xelement.Element("Left").Value), double.Parse(xelement.Element("Top").Value)),
                        cNode);

                    return;
            }
        }
    }
}
