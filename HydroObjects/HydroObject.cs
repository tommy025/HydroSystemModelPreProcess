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

        public HydroObject() : this(DateTime.Now)
        { }

        public HydroObject(DateTime _creationTime, string _name = "")
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
            var name = xelement.Attributes().Single(a => { return a.Name.LocalName == "HydroObjectName"; });
            var creationTime = xelement.Attributes().Single(a => { return a.Name.LocalName == "CreationTime"; });
            switch (xelement.Name.LocalName)
            {
                case "ConnectNode":
                    return new ConnectNode(DateTime.Parse(creationTime.Value), name.Value);
                case "PressurePipe":
                    return new PressurePipe(DateTime.Parse(creationTime.Value), name.Value);
                default:
                    throw new ArgumentException("Unsupported type " + xelement.Name.LocalName + " when loading from file!");
            }
        }
    }
}
