using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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

        public HydroVertex[] GetVertexs()
        {
            return (from o in hydroObjects
                    where o is HydroVertex
                    select (HydroVertex)o).ToArray();
        }

        public HydroVertex[] GetVertexs(HydroEdge edge)
        {
            return hydroEdges[edge].GetVertexs();
        }

        public HydroEdge[] GetEdges()
        {
            return (from o in hydroObjects
                    where o is HydroEdge
                    select (HydroEdge)o).ToArray();
        }

        public HydroEdge[] GetEdges(HydroVertex vertex)
        {
            return hydroVertexs[vertex].ToArray();
        }

        public void Add(HydroObject item)
        {
            if (item != null && !hydroObjects.Contains(item))
            { 
                ((ICollection<HydroObject>)hydroObjects).Add(item);
                //RegisterHydroObjectToDictionary(item);
            }
        }

        public void Clear()
        {
            //hydroVertexs.Clear();
            //hydroEdges.Clear();
            ((ICollection<HydroObject>)hydroObjects).Clear();
        }

        public bool Contains(HydroObject item)
        {
            return ((ICollection<HydroObject>)hydroObjects).Contains(item);
        }

        public void CopyTo(HydroObject[] array, int arrayIndex)
        {
            ((ICollection<HydroObject>)hydroObjects).CopyTo(array, arrayIndex);
        }

        public bool Remove(HydroObject item)
        {
            //RemoveHydroObjectFromDictionary(item);
            return ((ICollection<HydroObject>)hydroObjects).Remove(item);
        }

        public IEnumerator<HydroObject> GetEnumerator()
        {
            return ((ICollection<HydroObject>)hydroObjects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<HydroObject>)hydroObjects).GetEnumerator();
        }

        public int Count
        {
            get
            {
                return ((ICollection<HydroObject>)hydroObjects).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<HydroObject>)hydroObjects).IsReadOnly;
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
                var vertexInfo = hydroVertexs[edgeInfo.Vertex1];
                vertexInfo.Remove((HydroEdge)item);
                vertexInfo = hydroVertexs[edgeInfo.Vertex2];
                vertexInfo.Remove((HydroEdge)item);
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
                hydroObjects.Add(edge);

            var hydroEdgeInfo = hydroEdges[edge];
            hydroEdgeInfo.Vertex1 = vertex;

            if (vertex != null)
            {
                if (!hydroObjects.Contains(vertex))
                    hydroObjects.Add(vertex);

                var hydroVertexInfo = hydroVertexs[vertex];
                hydroVertexInfo.Add(edge);
            }
        }

        public void SetVertex2(HydroEdge edge, HydroVertex vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroObjects.Contains(edge))
                hydroObjects.Add(edge);

            

            var hydroEdgeInfo = hydroEdges[edge];
            hydroEdgeInfo.Vertex2 = vertex;


            if (vertex != null)
            {
                if (!hydroObjects.Contains(vertex))
                    hydroObjects.Add(vertex);
                var hydroVertexInfo = hydroVertexs[vertex];
                hydroVertexInfo.Add(edge);
            }
        }

        public int TrySetVertex(HydroEdge edge, HydroVertex vertex)
        {
            return hydroEdges[edge].TryAddVertex(vertex);
        }

        public HydroVertex GetVertex1(HydroEdge edge)
        {
            return hydroEdges[edge].Vertex1;
        }

        public HydroVertex GetVertex2(HydroEdge edge)
        {
            return hydroEdges[edge].Vertex2;
        }

        public HydroVertex GetTheOtherVertex(HydroEdge edge, HydroVertex vertex)
        {
            if (!hydroObjects.Contains(edge) || !hydroObjects.Contains(vertex))
                throw new ArgumentException("Given HydroEdge or HydroVertex is not included in HydroObjectGraph!");

            return hydroEdges[edge].GetTheOtherVertex(vertex);
        }

        public void ConnectVertexs(HydroEdge edge, HydroVertex vertex1, HydroVertex vertex2)
        {
            SetVertex1(edge, vertex1);
            SetVertex2(edge, vertex2);
        }

        public void DisConnectVertexs(HydroEdge edge)
        {
            SetVertex1(edge, null);
            SetVertex2(edge, null);
        }

        public void DisConnectVertexs(HydroVertex vertex1, HydroVertex vertex2)
        {
            var edge = (from e in hydroEdges
                        where e.Value.IsBetween(vertex1, vertex2)
                        select e).First().Key;

            DisConnectVertexs(edge);
        }

        public PressurePipe AddPressurePipe(HydroVertex vertex1, HydroVertex vertex2)
        {
            var pPipe = new PressurePipe();
            Add(pPipe);
            ConnectVertexs(pPipe, vertex1, vertex2);
            return pPipe;
        }

        public ConnectNode AddConnectNode()
        {
            var cNode = new ConnectNode();
            Add(cNode);
            return cNode;
        }
    }
}
