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
        public HydroObject()
        { }

        public HydroObject(HydroObject other)
        {
            PropertyChanged += other.PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void TriggerPropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Tag
        { get; set; }

        public abstract HydroObject DeepClone();

        //public XElement XmlSerialize()
        //{
        //    return new XElement(GetType().Name, 
        //        new XAttribute("HydroObjectFullName", FullName),
        //        new XAttribute("HydroObjectName", Name), 
        //        new XAttribute("CreationTime", CreationTime),
        //        ToXml());                    
        //}

        protected virtual XElement[] ToXml()
        {
            return new XElement[] { };
        }

        //public static HydroObject XmlDeserialize(XElement xelement)
        //{
        //    var name = xelement.Attribute("HydroObjectName").Value;
        //    var creationTime = xelement.Attribute("CreationTime").Value;
        //    switch (xelement.Name.LocalName)
        //    {
        //        case "ConnectNode":
        //            return new ConnectNode(DateTime.Parse(creationTime), name);
        //        case "PressurePipe":
        //            return new PressurePipe(DateTime.Parse(creationTime), name)
        //            {
        //                Roughness = double.Parse(xelement.Element("Roughness").Value)
        //            };
        //        default:
        //            throw new ArgumentException("Unsupported type " + xelement.Name.LocalName + " when loading from file!");
        //    }
        //}
    }
}
