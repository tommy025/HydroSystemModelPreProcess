using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Collections.Specialized;
using HydroSystemModelPreProcess.HydroObjects;
using System.Collections;

namespace HydroSystemModelPreProcess
{
    public abstract class HydroProcedure : IDisposable
    {
        protected HydroProcedure(HydroProcedure other)
        {
            hydroObjectGraph = other.hydroObjectGraph;
            hydroObjectGraph.CollectionChanged += OnHydroObjectGraphCollectionChanged;
            elementDataDictionary = new Dictionary<Shape, HydroObject>();
            foreach (var kvp in other.elementDataDictionary)
            {
                elementDataDictionary.Add(kvp.Key, kvp.Value.DeepClone());
            }
        }

        public HydroProcedure(IReadOnlyHydroObjectGraph _hydroObjectGraph)
        {
            hydroObjectGraph = _hydroObjectGraph;
            hydroObjectGraph.CollectionChanged += OnHydroObjectGraphCollectionChanged;
            elementDataDictionary = new Dictionary<Shape, HydroObject>();
            AddItems(hydroObjectGraph);            
        }

        private HydroObject CreateHydroObject(Shape shape)
        {
            var hydroObjectInfo = shape.DataContext as IHydroObjectInfo;
            return HydroResourceHelper.CreateHydroObject(hydroObjectInfo.HydroObjectType);
        }

        private void AddItems(IEnumerable items)
        {
            foreach (var element in items)
            {
                var shape = element as Shape;
                elementDataDictionary.Add(shape, CreateHydroObject(shape));
            }
        }

        private void RemoveItems(IEnumerable items)
        {
            foreach (var element in items)
            {
                elementDataDictionary.Remove(element as Shape);
            }
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
                    elementDataDictionary.Clear();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveItems(e.OldItems);
                    AddItems(e.NewItems);
                    break;
                default:
                    break;
            }
        }

        protected IReadOnlyHydroObjectGraph hydroObjectGraph;

        protected Dictionary<Shape, HydroObject> elementDataDictionary;

        public virtual void ExecuteCalculation()
        { }

        public async void ExecuteCalculationAsync()
        {
            await Task.Run(() => ExecuteCalculation());
        }

        public void Dispose()
        {
            hydroObjectGraph.CollectionChanged -= OnHydroObjectGraphCollectionChanged;
        }

        public abstract HydroProcedure DeepClone();
    }
}
