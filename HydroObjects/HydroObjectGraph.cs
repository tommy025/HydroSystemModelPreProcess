using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class HydroObjectGraph : ICollection<HydroObject>
    { 
        public HydroObjectGraph()
        {
            hydroObjects = new ObservableCollection<HydroObject>();
        }

        public HydroVertex[] GetVertexs()
        {
            return (from o in hydroObjects
                    where o is HydroVertex
                    select (HydroVertex)o).ToArray();
        }

        public HydroEdge[] GetEdges()
        {
            return (from o in hydroObjects
                    where o is HydroEdge
                    select (HydroEdge)o).ToArray();
        }

        public void Add(HydroObject item)
        {
            if (item != null && !hydroObjects.Contains(item))
                ((ICollection<HydroObject>)hydroObjects).Add(item);
        }

        public void Clear()
        {
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

        private ObservableCollection<HydroObject> hydroObjects;

        
    }
}
