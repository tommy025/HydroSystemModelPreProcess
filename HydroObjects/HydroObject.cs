using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroObject : INotifyPropertyChanged
    {
        //This lead to UnitTest not working since pack URI requires System.Windows.Application while UnitTest didn't create one.
        protected const string rdictSource = "pack://application:,,,/HydroObjects;component/generic.xaml";

        protected static readonly ResourceDictionary rdict;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void TriggerPropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        static HydroObject()
        {
            rdict = new ResourceDictionary();
            rdict.Source = new Uri(rdictSource, UriKind.RelativeOrAbsolute);
        }

        public HydroObject(DateTime _creationTime, string _name)
        {
            Name = _name;
            creationTime = _creationTime;
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                TriggerPropertyChangedEvent("Name");
            }
        }

        private readonly DateTime creationTime;

        public DateTime CreationTime
        {
            get { return creationTime; }
        }

        public string FullName
        {
            get { return CreationTime.Ticks.ToString() + "_" + Name; }
        }

        public XElement XmlSerialize()
        {
            return new XElement(GetType().Name, 
                new XAttribute("HydroObjectFullName", FullName),
                new XAttribute("HydroObjectName", Name), 
                new XAttribute("CreationTime", CreationTime),
                ToXml());                    
        }

        protected virtual XElement[] ToXml()
        {
            return new XElement[] { };
        }

        public static HydroObject XmlDeserialize(XElement xelement)
        {
            var name = xelement.Attribute("HydroObjectName").Value;
            var creationTime = xelement.Attribute("CreationTime").Value;
            switch (xelement.Name.LocalName)
            {
                case "ConnectNode":
                    return new ConnectNode(DateTime.Parse(creationTime), name);
                case "PressurePipe":
                    return new PressurePipe(DateTime.Parse(creationTime), name)
                    {
                        Roughness = double.Parse(xelement.Element("Roughness").Value)
                    };
                default:
                    throw new ArgumentException("Unsupported type " + xelement.Name.LocalName + " when loading from file!");
            }
        }
    }
}
