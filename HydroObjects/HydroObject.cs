using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroObject : INotifyPropertyChanged
    {
        protected const string rdictSource = "generic.xaml";

        protected readonly ResourceDictionary rdict;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void TriggerPropertyChangedEvent(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public HydroObject()
        {
            rdict = new ResourceDictionary();
            rdict.Source = new Uri(rdictSource, UriKind.Relative);
        }
    }
}
