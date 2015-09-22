using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    internal class HydroVertexInfo : ICollection<HydroEdge>
    {
        public HydroVertexInfo()
        {
            edges = new List<HydroEdge>();
        }       

        private readonly List<HydroEdge> edges;

        public HydroEdge GetEdgePrevTo(HydroEdge edge)
        {
            var index = (edges as IList<HydroEdge>).IndexOf(edge);
            if (index == 0)
                return null;
            else
                return edges[index - 1];
        }

        public HydroEdge GetEdgeNextTo(HydroEdge edge)
        {
            var index = (edges as IList<HydroEdge>).IndexOf(edge);
            if (index == edges.Count - 1)
                return null;
            else
                return edges[index + 1];
        }

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
