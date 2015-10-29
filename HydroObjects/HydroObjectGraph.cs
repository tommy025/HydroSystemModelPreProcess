using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class HydroObjectGraph : ICollection<HydroObject>, INotifyCollectionChanged
    { 
        public HydroObjectGraph()
        {
            hydroObjects = new ObservableCollection<HydroObject>();
            hydroVertexs = new Dictionary<HydroVertex, HydroVertexInfo>();
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
                        hydroVertexs.Clear();
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

        private Dictionary<HydroVertex, HydroVertexInfo> hydroVertexs;

        private Dictionary<HydroEdge, HydroEdgeInfo> hydroEdges;

        private void RegisterHydroObjectToDictionary(HydroObject item)
        {
            if (item is HydroEdge)
                hydroEdges.Add((HydroEdge)item, new HydroEdgeInfo());
            else if (item is HydroVertex)
                hydroVertexs.Add((HydroVertex)item, new HydroVertexInfo());
            else
                throw new ArgumentException("Unsupported type '" + item.GetType().ToString() +
                    "' when adding to HydroObjectGraph!");
        }

        private void RemoveHydroObjectFromDictionary(HydroObject item)
        {
            if (item is HydroEdge)
            {
                var edgeInfo = hydroEdges[(HydroEdge)item];
                if (edgeInfo.Vertex1 != null)
                {
                    var vertexInfo = hydroVertexs[edgeInfo.Vertex1];
                    vertexInfo.Remove((HydroEdge)item);
                }

                if (edgeInfo.Vertex2 != null)
                {
                    var vertexInfo = hydroVertexs[edgeInfo.Vertex2];
                    vertexInfo.Remove((HydroEdge)item);
                }

                hydroEdges.Remove((HydroEdge)item);
            }
            else if (item is HydroVertex)
            {
                var vertexInfo = hydroVertexs[(HydroVertex)item];
                foreach(var edge in vertexInfo)
                {
                    var edgeInfo = hydroEdges[edge];
                    edgeInfo.RemoveVertex((HydroVertex)item);
                }

                hydroVertexs.Remove((HydroVertex)item);
            }
            else
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

            var hydroEdgeInfo = hydroEdges[edge];
            if (hydroEdgeInfo.Vertex1 != null)
                hydroVertexs[hydroEdgeInfo.Vertex1].Remove(edge);
            
            hydroEdgeInfo.Vertex1 = vertex;
            if (vertex != null)
                hydroVertexs[vertex].Add(edge);
        }

        public void SetVertex2(HydroEdge edge, HydroVertex vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroObjects.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroObjects.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            var hydroEdgeInfo = hydroEdges[edge];
            if (hydroEdgeInfo.Vertex2 != null)
                hydroVertexs[hydroEdgeInfo.Vertex1].Remove(edge);

            hydroEdgeInfo.Vertex2 = vertex;
            if (vertex != null)
                hydroVertexs[vertex].Add(edge);  
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
            var result = (from e1 in hydroVertexs[vertex1]
                          join e2 in hydroVertexs[vertex2]
                          on e1 equals e2
                          select e1).ToArray();

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
            return hydroVertexs.Keys.ToArray();
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
            return hydroVertexs[vertex].ToArray();
        }

        private class HydroEdgeInfo : HydroObjectInfo
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
        }

        private class HydroVertexInfo : HydroObjectInfo, ICollection<HydroEdge>
        {
            public HydroVertexInfo()
            {
                edges = new List<HydroEdge>();
            }

            private readonly List<HydroEdge> edges;

            public void Add(HydroEdge item)
            {
                if (item != null && !edges.Contains(item))
                    edges.Add(item);
            }

            public void Clear()
            {
                edges.Clear();
            }

            public bool Contains(HydroEdge item)
            {
                return edges.Contains(item);
            }

            public void CopyTo(HydroEdge[] array, int arrayIndex)
            {
                edges.CopyTo(array, arrayIndex);
            }

            public bool Remove(HydroEdge item)
            {
                return edges.Remove(item);
            }

            public IEnumerator<HydroEdge> GetEnumerator()
            {
                return ((ICollection<HydroEdge>)edges).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((ICollection<HydroEdge>)edges).GetEnumerator();
            }

            public int Count
            {
                get
                {
                    return edges.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return ((ICollection<HydroEdge>)edges).IsReadOnly;
                }
            }
        }
    }
}
