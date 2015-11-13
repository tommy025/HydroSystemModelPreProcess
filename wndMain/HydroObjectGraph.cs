using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace HydroSystemModelPreProcess
{
    public class HydroObjectGraph : IHydroObjectGraph
    {
        #region Constructors

        public HydroObjectGraph()
        {
            hydroObjects = new ObservableCollection<IHydroObjectInfo>();
            hydroObjectGraphEdgeInfo = new Dictionary<IHydroEdgeInfo, HydroObjectGraphEdgeInfo>();
        }

        #endregion

        #region Fields

        protected ObservableCollection<IHydroObjectInfo> hydroObjects;

        protected Dictionary<IHydroEdgeInfo, HydroObjectGraphEdgeInfo> hydroObjectGraphEdgeInfo;

        #endregion

        #region IHydroObjectGraph

        public bool Contains(IHydroObjectInfo item)
        {
            return hydroObjects.Contains(item);
        }

        public int Count
        {
            get
            {
                return hydroObjects.Count;
            }
        }

        public IEnumerator<IHydroObjectInfo> GetEnumerator()
        {
            return hydroObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hydroObjects.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { hydroObjects.CollectionChanged += value; }
            remove { hydroObjects.CollectionChanged -= value; }
        }

        public IHydroVertexInfo GetVertex1(IHydroEdgeInfo edge)
        {
            return hydroObjectGraphEdgeInfo[edge].Vertex1;
        }

        public IHydroVertexInfo GetVertex2(IHydroEdgeInfo edge)
        {
            return hydroObjectGraphEdgeInfo[edge].Vertex2;
        }
        
        public bool IsConnected(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            return GetEdges(vertex1, vertex2).Length != 0;
        }

        public bool IsConnectedTo(IHydroEdgeInfo edge, IHydroVertexInfo vertex)
        {
            return hydroObjectGraphEdgeInfo[edge].IsConnectedTo(vertex);
        }

        public bool IsBetween(IHydroEdgeInfo edge, IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            return hydroObjectGraphEdgeInfo[edge].IsBetween(vertex1, vertex2);
        }   

        public IHydroVertexInfo[] GetAllVertexs()
        {
            return hydroObjects.Except(hydroObjectGraphEdgeInfo.Keys).Select(o => o as IHydroVertexInfo).ToArray();
        }

        public IHydroVertexInfo[] GetVertexs(IHydroEdgeInfo edge)
        {
            return hydroObjectGraphEdgeInfo[edge].GetVertexs();
        }

        public IHydroEdgeInfo[] GetAllEdges()
        {
            return hydroObjectGraphEdgeInfo.Keys.ToArray();
        }

        public IHydroEdgeInfo[] GetEdges(IHydroVertexInfo vertex)
        {
            return (from kvp in hydroObjectGraphEdgeInfo
                    where kvp.Value.IsConnectedTo(vertex)
                    select kvp.Key).ToArray();
        }

        public IHydroEdgeInfo[] GetEdges(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            return (from kvp in hydroObjectGraphEdgeInfo
                    where kvp.Value.IsBetween(vertex1, vertex2)
                    select kvp.Key).ToArray();
        }     

        public void AddVertex(IHydroVertexInfo vertex)
        {
            if (hydroObjects.Contains(vertex))
                throw new ArgumentException("Given vertex already exists!");

            hydroObjects.Add(vertex);
        }

        public void AddEdge(IHydroEdgeInfo edge)
        {
            if (hydroObjects.Contains(edge))
                throw new ArgumentException("Given edge already exists!");

            hydroObjectGraphEdgeInfo.Add(edge, new HydroObjectGraphEdgeInfo());
            hydroObjects.Add(edge);
        }

        public void Clear()
        {
            hydroObjects.Clear();
            hydroObjectGraphEdgeInfo.Clear();
        }

        public bool Remove(IHydroObjectInfo item)
        {
            if (item is IHydroEdgeInfo)
                hydroObjectGraphEdgeInfo.Remove(item as IHydroEdgeInfo);

            return hydroObjects.Remove(item);
        }

        public void SetVertex1(IHydroEdgeInfo edge, IHydroVertexInfo vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroObjects.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroObjects.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            hydroObjectGraphEdgeInfo[edge].Vertex1 = vertex;
        }

        public void SetVertex2(IHydroEdgeInfo edge, IHydroVertexInfo vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroObjects.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroObjects.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            hydroObjectGraphEdgeInfo[edge].Vertex2 = vertex;
        }

        public void ConnectVertexs(IHydroEdgeInfo edge, IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            SetVertex1(edge, vertex1);
            SetVertex2(edge, vertex2);
        }

        public void DisConnectVertexs(IHydroEdgeInfo edge)
        {
            SetVertex1(edge, null);
            SetVertex2(edge, null);
        }

        public void DisConnectVertexs(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            var edges = from kvp in hydroObjectGraphEdgeInfo
                        where kvp.Value.IsBetween(vertex1, vertex2)
                        select kvp.Key;

            foreach (var e in edges)
                DisConnectVertexs(e);
        }

        #endregion

        #region HydroObjectGraphEdgeInfo

        protected class HydroObjectGraphEdgeInfo
        {
            private IHydroVertexInfo vertex1;

            public IHydroVertexInfo Vertex1
            {
                get { return vertex1; }
                set
                {
                    if (vertex2 == value && value != null)
                        throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                    vertex1 = value;
                }
            }

            private IHydroVertexInfo vertex2;

            public IHydroVertexInfo Vertex2
            {
                get { return vertex2; }
                set
                {
                    if (vertex1 == value && value != null)
                        throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                    vertex2 = value;
                }
            }

            public IHydroVertexInfo[] GetVertexs()
            {
                return new IHydroVertexInfo[] { Vertex1, Vertex2 };
            }

            public bool IsBetween(IHydroVertexInfo v1, IHydroVertexInfo v2)
            {
                if (v1 == null || v2 == null)
                    return false;

                if (Vertex1 == v1 && Vertex2 == v2 || Vertex2 == v1 && Vertex1 == v2)
                    return true;
                else
                    return false;
            }

            public bool IsConnectedTo(IHydroVertexInfo vertex)
            {
                if (vertex == Vertex1 || vertex == Vertex2)
                    return true;
                else
                    return false;
            }
        }

        #endregion
    }
}
