using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class HydroObjectGraph : ICollection<HydroObject>, INotifyCollectionChanged
    { 
        public HydroObjectGraph()
        {
            hydroObjects = new ObservableCollection<HydroObject>();
            hydroEdges = new Dictionary<HydroEdge, HydroEdgeInfo>();
            CollectionChanged += OnHydroObjectGraphModified;
        }

        private void OnHydroObjectGraphModified(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = (ObservableCollection<HydroObject>)sender;
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            var hydroObject = (HydroObject)item;
                            RegisterHydroObjectToDictionary(hydroObject);
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            var hydroObject = (HydroObject)item;
                            RemoveHydroObjectFromDictionary(hydroObject);
                        }

                        break;
                    }               
                case NotifyCollectionChangedAction.Reset:
                    {
                        hydroEdges.Clear();
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Replacing or sorting HydroObject in HydroObjectGraph is not supported!");
                    }
            }
        }

        public void Add(HydroObject item)
        {
            if (item == null)
                throw new ArgumentNullException("Cannot add null object to HydroObjectGraph!");

            if (hydroObjects.Contains(item))
                throw new ArgumentException("Given HydroObject already in HydroObjectGraph!");

            hydroObjects.Add(item);
        }     

        public void Clear()
        {
            hydroObjects.Clear();
        }

        public bool Contains(HydroObject item)
        {
            return hydroObjects.Contains(item);
        }

        public void CopyTo(HydroObject[] array, int arrayIndex)
        {
            hydroObjects.CopyTo(array, arrayIndex);
        }

        public bool Remove(HydroObject item)
        {
            return hydroObjects.Remove(item);
        }

        public IEnumerator<HydroObject> GetEnumerator()
        {
            return hydroObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hydroObjects.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return hydroObjects.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return (hydroObjects as ICollection<HydroObject>).IsReadOnly;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { hydroObjects.CollectionChanged += value; }
            remove { hydroObjects.CollectionChanged -= value; }
        }

        private ObservableCollection<HydroObject> hydroObjects;

        private Dictionary<HydroEdge, HydroEdgeInfo> hydroEdges;

        private void RegisterHydroObjectToDictionary(HydroObject item)
        {
            if (item is HydroEdge)
                hydroEdges.Add((HydroEdge)item, new HydroEdgeInfo());
            else if (!(item is HydroVertex))
                throw new ArgumentException("Unsupported type '" + item.GetType().ToString() +
                    "' when adding to HydroObjectGraph!");
        }

        private void RemoveHydroObjectFromDictionary(HydroObject item)
        {
            if (item is HydroEdge)
            {
                hydroEdges.Remove((HydroEdge)item);
            }
            else if (!(item is HydroVertex))
                throw new ArgumentException("Unsupported type '" + item.GetType().ToString() +
                    "' when removing from HydroObjectGraph!");
        }

        public void SetVertex1(HydroEdge edge, HydroVertex vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroObjects.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroObjects.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            hydroEdges[edge].Vertex1 = vertex;
        }

        public void SetVertex2(HydroEdge edge, HydroVertex vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroObjects.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroObjects.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            hydroEdges[edge].Vertex2 = vertex;
        }

        public HydroVertex GetVertex1(HydroEdge edge)
        {
            return hydroEdges[edge].Vertex1;
        }

        public HydroVertex GetVertex2(HydroEdge edge)
        {
            return hydroEdges[edge].Vertex2;
        }

        public void ConnectVertexs(HydroEdge edge, HydroVertex vertex1, HydroVertex vertex2)
        {
            SetVertex1(edge, vertex1);
            SetVertex2(edge, vertex2);
        }

        public bool IsConnected(HydroVertex vertex1, HydroVertex vertex2)
        {
            var result = (from e in hydroEdges.Values
                          where e.IsBetween(vertex1, vertex2)
                          select e).ToArray();

            return result.Length != 0;
        }

        public bool IsConnectedTo(HydroEdge edge, HydroVertex vertex)
        {
            return hydroEdges[edge].IsConnectedTo(vertex);
        }

        public bool IsBetween(HydroEdge edge, HydroVertex vertex1, HydroVertex vertex2)
        {
            return hydroEdges[edge].IsBetween(vertex1, vertex2);
        }

        public void DisConnectVertexs(HydroEdge edge)
        {
            SetVertex1(edge, null);
            SetVertex2(edge, null);
        }

        public void DisConnectVertexs(HydroVertex vertex1, HydroVertex vertex2)
        {
            var edges = from e in hydroEdges
                        where e.Value.IsBetween(vertex1, vertex2)
                        select e.Key;

            foreach (var e in edges)
                DisConnectVertexs(e);
        }

        public HydroVertex[] GetAllVertexs()
        {
            return (from v in hydroObjects
                    where v is HydroVertex
                    select v as HydroVertex).ToArray();
        }

        public HydroVertex[] GetVertexs(HydroEdge edge)
        {
            return hydroEdges[edge].GetVertexs();
        }

        public HydroEdge[] GetAllEdges()
        {
            return hydroEdges.Keys.ToArray();
        }

        public HydroEdge[] GetEdges(HydroVertex vertex)
        {
            return (from kvp in hydroEdges
                    where kvp.Value.IsConnectedTo(vertex)
                    select kvp.Key).ToArray();
        }

        public HydroEdge[] GetEdges(HydroVertex vertex1, HydroVertex vertex2)
        {
            return (from kvp in hydroEdges
                    where kvp.Value.IsBetween(vertex1, vertex2)
                    select kvp.Key).ToArray();
        }

        public HydroObject GetObject(string fullName)
        {
            return hydroObjects.Single(n => { return n.FullName == fullName; });
        }

        public void LoadFromXmlFile(XElement xroot)
        {
            var xHydroObjects = xroot.Element("HydroObjects");
            var xHydroEdgeInfos = xroot.Element("HydroEdgeInfos");
            foreach (var xHydroObject in xHydroObjects.Elements())
            {
                Add(HydroObject.XmlDeserialize(xHydroObject));
            }

            foreach(var xHydroEdgeInfo in xHydroEdgeInfos.Elements())
            {
                var associatedEdgeName = xHydroEdgeInfo.Attribute("HydroEdgeFullName").Value;
                var hydroEdge = hydroObjects.Single(hydroObject => { return hydroObject.FullName == associatedEdgeName; }) as HydroEdge;
                var associatedVertexName1 = xHydroEdgeInfo.Element("Vertex1").Value;
                var hydroVertex1 = hydroObjects.SingleOrDefault(hydroObject => { return hydroObject.FullName == associatedVertexName1; }) as HydroVertex;
                SetVertex1(hydroEdge, hydroVertex1);
                var associatedVertexName2 = xHydroEdgeInfo.Element("Vertex2").Value;
                var hydroVertex2 = hydroObjects.SingleOrDefault(hydroObject => { return hydroObject.FullName == associatedVertexName2; }) as HydroVertex;
                SetVertex2(hydroEdge, hydroVertex2);
            }
        }

        public XElement[] SaveToXmlFile()
        {
            return new XElement[] {
                new XElement("HydroObjects",
                    (from o in hydroObjects
                     select o.XmlSerialize()).ToArray()),
                new XElement("HydroEdgeInfos",
                    (from kvp in hydroEdges
                     select kvp.Value.XmlSerialize(kvp.Key.FullName)).ToArray())};
        }

        private class HydroEdgeInfo
        {
            private HydroVertex vertex1;

            public HydroVertex Vertex1
            {
                get { return vertex1; }
                set
                {
                    if (vertex2 == value && value != null)
                        throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                    vertex1 = value;
                }
            }

            private HydroVertex vertex2;

            public HydroVertex Vertex2
            {
                get { return vertex2; }
                set
                {
                    if (vertex1 == value && value != null)
                        throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                    vertex2 = value;
                }
            }

            public HydroVertex[] GetVertexs()
            {
                return new HydroVertex[] { Vertex1, Vertex2 };
            }

            public bool RemoveVertex(HydroVertex vertex)
            {
                if (vertex == Vertex1)
                {
                    Vertex1 = null;
                    return true;
                }
                else if (vertex == Vertex2)
                {
                    Vertex2 = null;
                    return true;
                }
                else
                    return false;
            }

            public bool IsBetween(HydroVertex v1, HydroVertex v2)
            {
                if (v1 == null || v2 == null)
                    return false;

                if (Vertex1 == v1 && Vertex2 == v2 || Vertex2 == v1 && Vertex1 == v2)
                    return true;
                else
                    return false;
            }

            public bool IsConnectedTo(HydroVertex vertex)
            {
                if (vertex == Vertex1 || vertex == Vertex2)
                    return true;
                else
                    return false;
            }

            public XElement XmlSerialize(string hObjectFullName)
            {
                return new XElement(GetType().Name, new XAttribute("HydroEdgeFullName", hObjectFullName),
                    new XElement("Vertex1", Vertex1 != null ? Vertex1.FullName : "null"),
                    new XElement("Vertex2", Vertex2 != null ? Vertex2.FullName : "null"));
            }
        }
    }
}
