using System;
using System.Windows;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public interface IHydroObjectInfo
    {
        Shape Element
        { get; }

        Type HydroObjectType
        { get; }
    }
}
