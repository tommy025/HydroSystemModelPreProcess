﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public abstract class HydroObjectInfo : IHydroObjectInfo, INotifyPropertyChanged
    {
        #region Constructors

        protected HydroObjectInfo(Type _hydroObjectType)
        {
            HydroObjectType = _hydroObjectType;
        }

        #endregion

        #region Fields

        private string name = "<NewObject>";

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                TriggerPropertyChanged(nameof(Name));
                TriggerPropertyChanged(nameof(FullName));
            }
        }

        public DateTime CreationTime
        { get; } = DateTime.Now;

        public string FullName
        {
            get { return "_" + CreationTime.Ticks.ToString() + "_" + Name; }
        }

        #endregion

        #region Methods

        protected void TriggerPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IHydroObjectInfo

        public abstract Shape Element
        { get; }

        public Type HydroObjectType
        { get; }

        public FrameworkElement PropertySettingControl
        {
            get { return HydroResourceHelper.GetHydroObjectPropertySettingControl(HydroObjectType); }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}