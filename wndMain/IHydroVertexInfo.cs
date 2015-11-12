using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess
{
    public interface IHydroVertexInfo : IHydroObjectInfo, INotifyPropertyChanged
    {
        double Top
        { get; set; }

        double Left
        { get; set; }

        double Height
        { get; }

        double Width
        { get; }
    }
}
