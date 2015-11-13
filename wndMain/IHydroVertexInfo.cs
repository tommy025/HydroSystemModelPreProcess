using System.ComponentModel;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public interface IHydroVertexInfo : IHydroObjectInfo, INotifyPropertyChanged
    {
        Rectangle Vertex
        { get; }
    }
}