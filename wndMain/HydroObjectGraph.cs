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
    public class HydroObjectGraph : IHydroObjectGraph
    {
        #region Constructors

        public HydroObjectGraph()
        {
            hydroObjects = new ObservableCollection<IHydroObjectInfo>();
        }

        #endregion

        #region Fields

        protected ObservableCollection<IHydroObjectInfo> hydroObjects;

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
            return edge.Vertex1;
        }

        public IHydroVertexInfo GetVertex2(IHydroEdgeInfo edge)
        {
            return edge.Vertex2;
        }
        
        public bool IsConnected(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            return GetEdges(vertex1, vertex2).Length != 0;
        }

        public bool IsConnectedTo(IHydroEdgeInfo edge, IHydroVertexInfo vertex)
        {
            return edge.IsConnectedTo(vertex);
        }

        public bool IsBetween(IHydroEdgeInfo edge, IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            return edge.IsBetween(vertex1, vertex2);
        }   

        public IHydroVertexInfo[] GetAllVertexs()
        {
            return (from o in hydroObjects
                    where o is IHydroVertexInfo
                    select o as IHydroVertexInfo).ToArray();
        }

        public IHydroVertexInfo[] GetVertexs(IHydroEdgeInfo edge)
        {
            return edge.GetVertexs();
        }

        public IHydroEdgeInfo[] GetAllEdges()
        {
            return (from o in hydroObjects
                    where o is IHydroEdgeInfo
                    select o as IHydroEdgeInfo).ToArray();
        }

        public IHydroEdgeInfo[] GetEdges(IHydroVertexInfo vertex)
        {
            return (from o in hydroObjects
                    where o is IHydroEdgeInfo && (o as IHydroEdgeInfo).IsConnectedTo(vertex)
                    select o as IHydroEdgeInfo).ToArray();
        }

        public IHydroEdgeInfo[] GetEdges(IHydroVertexInfo vertex1, IHydroVertexInfo vertex2)
        {
            return (from o in hydroObjects
                    where o is IHydroEdgeInfo && (o as IHydroEdgeInfo).IsBetween(vertex1, vertex2)
                    select o as IHydroEdgeInfo).ToArray();
        }     

        public void AddVertex(IHydroVertexInfo vertex)
        {
            hydroObjects.Add(vertex);
        }

        public void AddEdge(IHydroEdgeInfo edge)
        {
            hydroObjects.Add(edge);
        }

        public void Clear()
        {
            hydroObjects.Clear();
        }

        public bool Remove(IHydroObjectInfo item)
        {
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

            edge.Vertex1 = vertex;
        }

        public void SetVertex2(IHydroEdgeInfo edge, IHydroVertexInfo vertex)
        {
            if (edge == null)
                throw new ArgumentNullException("HydroEdge reference should not be null!");

            if (!hydroObjects.Contains(edge))
                throw new ArgumentException("Given HydroEdge not contained in HydroObjectGraph!");

            if (vertex != null && !hydroObjects.Contains(vertex))
                throw new ArgumentException("Given HydroVertex not contained in HydroObjectGraph!");

            edge.Vertex2 = vertex;
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
            var edges = from o in hydroObjects
                        where o is IHydroEdgeInfo && (o as IHydroEdgeInfo).IsBetween(vertex1, vertex2)
                        select o as IHydroEdgeInfo;

            foreach (var e in edges)
                DisConnectVertexs(e);
        }

        #endregion
    }
}
