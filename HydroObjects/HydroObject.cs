using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroObject : INotifyPropertyChanged
    {
        protected const string rdictSource = "HydroObjects;component/generic.xaml";

        protected static readonly ResourceDictionary rdict;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void TriggerPropertyChangedEvent(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        static HydroObject()
        {
            rdict = new ResourceDictionary();
            rdict.Source = new Uri(rdictSource, UriKind.RelativeOrAbsolute);
        }

        public HydroObject() : this("", DateTime.Now)
        { }

        public HydroObject(string _name) : this(_name, DateTime.Now)
        { }

        public HydroObject(DateTime _creationTime) : this("", _creationTime)
        { }

        public HydroObject(string _name, DateTime _creationTime)
        {
            Name = _name;
            creationTime = _creationTime;
        }

        public string Name
        { get; set; }

        private readonly DateTime creationTime;

        public DateTime CreationTime
        {
            get { return creationTime; }
        }
    }
}
