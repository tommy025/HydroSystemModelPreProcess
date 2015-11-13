using System.ComponentModel;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public interface IHydroEdgeInfo : IHydroObjectInfo, INotifyPropertyChanged
    {
        Line Edge
        { get; }
    }
}