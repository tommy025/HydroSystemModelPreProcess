using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess
{
    public interface IHydroEdgeInfo : IHydroObjectInfo, INotifyPropertyChanged
    {
        double X1
        { get; set; }

        double Y1
        { get; set; }

        double X2
        { get; set; }

        double Y2
        { get; set; }

        IHydroVertexInfo Vertex1
        { get; set; }

        IHydroVertexInfo Vertex2
        { get; set; }

        IHydroVertexInfo[] GetVertexs();

        bool IsBetween(IHydroVertexInfo v1, IHydroVertexInfo v2);

        bool IsConnectedTo(IHydroVertexInfo vertex);
    }
}
