using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using HydroSystemModelPreProcess.HydroObjects;
using System.Collections.Specialized;

namespace HydroSystemModelPreProcess
{
    public interface IHydroProcedure : ICustomizedClone<IHydroProcedure>, IDisposable, IEnumerable<HydroObject>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        void ExecuteCalculation();

        void ExecuteCalculationAsync(CancellationTokenSource cts);

        double Progress
        { get; }

        object CalculationResult
        { get; }
    }
}
