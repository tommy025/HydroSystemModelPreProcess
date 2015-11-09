using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HydroSystemModelPreProcess
{
    public class HydroObjectGraph : IReadOnlyHydroObjectGraph
    {
        public HydroObjectGraph()
        {
            hydroElements = new ObservableCollection<Shape>();
            hydroEdges = new Dictionary<Line, HydroEdgeInfo>();
        }

        public void AddVertex(Rectangle vertex, Type type)
        {
            hydroElements.Add(vertex);
            vertex.DataContext = new HydroObjectInfo(vertex, type);
        }

        public void AddEdge(Line edge, Type type)
        {
            hydroElements.Add(edge);
            edge.DataContext = new HydroObjectInfo(edge, type);
            hydroEdges.Add(edge, new HydroEdgeInfo());
        }

        public void Clear()
        {
            hydroEdges.Clear();
            hydroElements.Clear();
        }

        public bool Contains(Shape item)
        {
            return hydroElements.Contains(item);
        }

        public bool Remove(Shape item)
        {
            if (item is Line)
                hydroEdges.Remove(item as Line);

            return hydroElements.Remove(item);
        }

        public int Count
        {
            get
            {
                return hydroElements.Count;
            }
        }

        private ObservableCollection<Shape> hydroElements;

        private Dictionary<Line, HydroEdgeInfo> hydroEdges;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { hydroElements.CollectionChanged += value; }
            remove { hydroElements.CollectionChanged -= value; }
        }

        public void SetVertex1(Line edge, Rectangle vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroElements.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroElements.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            hydroEdges[edge].Vertex1 = vertex;
        }

        public void SetVertex2(Line edge, Rectangle vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroElements.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroElements.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            hydroEdges[edge].Vertex2 = vertex;
        }

        public Rectangle GetVertex1(Line edge)
        {
            return hydroEdges[edge].Vertex1;
        }

        public Rectangle GetVertex2(Line edge)
        {
            return hydroEdges[edge].Vertex2;
        }

        public void ConnectVertexs(Line edge, Rectangle vertex1, Rectangle vertex2)
        {
            SetVertex1(edge, vertex1);
            SetVertex2(edge, vertex2);
        }

        public bool IsConnected(Rectangle vertex1, Rectangle vertex2)
        {
            var result = (from e in hydroEdges.Values
                          where e.IsBetween(vertex1, vertex2)
                          select e).ToArray();

            return result.Length != 0;
        }

        public bool IsConnectedTo(Line edge, Rectangle vertex)
        {
            return hydroEdges[edge].IsConnectedTo(vertex);
        }

        public bool IsBetween(Line edge, Rectangle vertex1, Rectangle vertex2)
        {
            return hydroEdges[edge].IsBetween(vertex1, vertex2);
        }

        public void DisConnectVertexs(Line edge)
        {
            SetVertex1(edge, null);
            SetVertex2(edge, null);
        }

        public void DisConnectVertexs(Rectangle vertex1, Rectangle vertex2)
        {
            var edges = from e in hydroEdges
                        where e.Value.IsBetween(vertex1, vertex2)
                        select e.Key;

            foreach (var e in edges)
                DisConnectVertexs(e);
        }

        public Rectangle[] GetAllVertexs()
        {
            return (from v in hydroElements
                    where v is Rectangle
                    select v as Rectangle).ToArray();
        }

        public Rectangle[] GetVertexs(Line edge)
        {
            return hydroEdges[edge].GetVertexs();
        }

        public Line[] GetAllEdges()
        {
            return hydroEdges.Keys.ToArray();
        }

        public Line[] GetEdges(Rectangle vertex)
        {
            return (from kvp in hydroEdges
                    where kvp.Value.IsConnectedTo(vertex)
                    select kvp.Key).ToArray();
        }

        public Line[] GetEdges(Rectangle vertex1, Rectangle vertex2)
        {
            return (from kvp in hydroEdges
                    where kvp.Value.IsBetween(vertex1, vertex2)
                    select kvp.Key).ToArray();
        }

        public Shape GetObject(string name)
        {
            return null;
            //return hydroElementInfo.Where(kvp => kvp.Value.Name == name)
        }

        public IEnumerator<Shape> GetEnumerator()
        {
            return hydroElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hydroElements.GetEnumerator();
        }

        //public void LoadFromXmlFile(XElement xroot)
        //{
        //    var xHydroObjects = xroot.Element("HydroObjects");
        //    var xHydroEdgeInfos = xroot.Element("HydroEdgeInfos");
        //    foreach (var xHydroObject in xHydroObjects.Elements())
        //    {
        //        Add(HydroObject.XmlDeserialize(xHydroObject));
        //    }

        //    foreach(var xHydroEdgeInfo in xHydroEdgeInfos.Elements())
        //    {
        //        var associatedEdgeName = xHydroEdgeInfo.Attribute("HydroEdgeFullName").Value;
        //        var hydroEdge = hydroElements.Single(hydroObject => { return hydroObject.FullName == associatedEdgeName; }) as HydroEdge;
        //        var associatedVertexName1 = xHydroEdgeInfo.Element("Vertex1").Value;
        //        var hydroVertex1 = hydroElements.SingleOrDefault(hydroObject => { return hydroObject.FullName == associatedVertexName1; }) as HydroVertex;
        //        SetVertex1(hydroEdge, hydroVertex1);
        //        var associatedVertexName2 = xHydroEdgeInfo.Element("Vertex2").Value;
        //        var hydroVertex2 = hydroElements.SingleOrDefault(hydroObject => { return hydroObject.FullName == associatedVertexName2; }) as HydroVertex;
        //        SetVertex2(hydroEdge, hydroVertex2);
        //    }
        //}

        //public XElement[] SaveToXmlFile()
        //{
        //    return new XElement[] {
        //        new XElement("HydroObjects",
        //            (from o in hydroElements
        //             select o.XmlSerialize()).ToArray()),
        //        new XElement("HydroEdgeInfos",
        //            (from kvp in hydroEdges
        //             select kvp.Value.XmlSerialize(kvp.Key.FullName)).ToArray())};
        //}

        private class HydroObjectInfo : IHydroObjectInfo
        {
            public HydroObjectInfo(Shape element, Type type)
            {
                Element = element;
                HydroObjectType = type;
            }

            public Shape Element
            { get; set; }

            public string Name
            { get; set; } = "";

            public string FullName
            {
                get { return Element.Name + "_" + Name; }
            }

            public Type HydroObjectType
            { get; set; }

            public string HydroObjectTypeName
            {
                get { return HydroObjectType.Name; }              
            }
        }

        private class HydroEdgeInfo
        {
            private Rectangle vertex1;

            public Rectangle Vertex1
            {
                get { return vertex1; }
                set
                {
                    if (vertex2 == value && value != null)
                        throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                    vertex1 = value;
                }
            }

            private Rectangle vertex2;

            public Rectangle Vertex2
            {
                get { return vertex2; }
                set
                {
                    if (vertex1 == value && value != null)
                        throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                    vertex2 = value;
                }
            }

            public Rectangle[] GetVertexs()
            {
                return new Rectangle[] { Vertex1, Vertex2 };
            }

            public bool IsBetween(Rectangle v1, Rectangle v2)
            {
                if (v1 == null || v2 == null)
                    return false;

                if (Vertex1 == v1 && Vertex2 == v2 || Vertex2 == v1 && Vertex1 == v2)
                    return true;
                else
                    return false;
            }

            public bool IsConnectedTo(Rectangle vertex)
            {
                if (vertex == Vertex1 || vertex == Vertex2)
                    return true;
                else
                    return false;
            }

            //public XElement XmlSerialize(string hObjectFullName)
            //{
            //    return new XElement(GetType().Name, new XAttribute("HydroEdgeFullName", hObjectFullName),
            //        new XElement("Vertex1", Vertex1 != null ? Vertex1.Name : "null"),
            //        new XElement("Vertex2", Vertex2 != null ? Vertex2.Name : "null"));
            //}
        }
    } 

    public interface IHydroObjectInfo
    {
        Shape Element
        { get; }

        string Name
        { get; }

        string FullName
        { get; }

        Type HydroObjectType
        { get; }

        string HydroObjectTypeName
        { get; }
    }
}
