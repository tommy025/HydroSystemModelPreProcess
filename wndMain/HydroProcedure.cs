using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using HydroSystemModelPreProcess.HydroObjects;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading;

namespace HydroSystemModelPreProcess
{
    public abstract class HydroProcedure : IHydroProcedure
    {
        #region Constructors

        protected HydroProcedure(HydroProcedure other)
        {
            Name = other.name;
            PropertyChanged += other.PropertyChanged;

            hydroObjectGraph = other.hydroObjectGraph;
            hydroObjectGraph.CollectionChanged += OnHydroObjectGraphCollectionChanged;

            hydroObjects = new ObservableCollection<HydroObject>();
            foreach (var o in other.hydroObjects)
            {
                hydroObjects.Add(o.DeepClone());
            }
        }

        public HydroProcedure(IReadOnlyHydroObjectGraph _hydroObjectGraph)
        {
            hydroObjectGraph = _hydroObjectGraph;
            hydroObjectGraph.CollectionChanged += OnHydroObjectGraphCollectionChanged;
            hydroObjects = new ObservableCollection<HydroObject>();
            AddItems(hydroObjectGraph);
        }

        #endregion

        #region Fields

        private string name = "<NewProcedure>";

        protected IReadOnlyHydroObjectGraph hydroObjectGraph;

        protected ObservableCollection<HydroObject> hydroObjects;

        private double progress;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set
            {
                if(value != null)
                {
                    name = value;
                    TriggerPropertyChanged(nameof(Name));
                }         
            }
        }

        public FrameworkElement PropertySettingControl
        {
            get { return HydroResourceHelper.GetProcedurePropertySettingControl(GetType()); }
        }

        #endregion

        #region Methods

        protected void TriggerPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private HydroObject CreateHydroObject(IHydroObjectInfo info)
        {
            var hydroObject = HydroResourceHelper.CreateHydroObject(info.HydroObjectType);
            hydroObject.Tag = info;
            return hydroObject;
        }

        private void OnHydroObjectGraphCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    hydroObjects.Clear();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveItems(e.OldItems);
                    AddItems(e.NewItems);
                    break;
                default:
                    break;
            }
        }

        private void AddItems(IEnumerable items)
        {
            foreach (var item in items)
            {
                var info = item as IHydroObjectInfo;
                hydroObjects.Add(CreateHydroObject(info));
            }
        }

        private void RemoveItems(IEnumerable items)
        {
            var query = from o in hydroObjects
                        join IHydroObjectInfo i in items on o.Tag equals i
                        select o;

            foreach (var o in query)
            {
                hydroObjects.Remove(o);
            }
        }

        #endregion

        #region IHydroProcedure    

        public string Note
        {
            get { return nameof(HydroObject) + "s are recreated but they all point to the same " + nameof(IHydroObjectInfo) + "s."; }
        }

        public abstract IHydroProcedure CustomizedClone();

        public void Dispose()
        {
            hydroObjectGraph.CollectionChanged -= OnHydroObjectGraphCollectionChanged;
        }

        public IEnumerator<HydroObject> GetEnumerator()
        {
            return hydroObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hydroObjects.GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { hydroObjects.CollectionChanged += value; }
            remove { hydroObjects.CollectionChanged -= value; }
        }

        public virtual void ExecuteCalculation()
        { }

        public async void ExecuteCalculationAsync(CancellationTokenSource cts)
        {
            await Task.Run(() => ExecuteCalculation());
        }

        public double Progress
        {
            get { return progress; }
            protected set
            {
                if (value > 0.0 && value < 100.0)
                {
                    progress = value;
                    TriggerPropertyChanged(nameof(Progress));
                }
            }
            
        }

        public abstract object CalculationResult
        { get; protected set; }

        #endregion
    }
}
