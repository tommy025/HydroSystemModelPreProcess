using System;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public class HydroVertexInfo : HydroObjectInfo, IHydroVertexInfo
    {
        #region Constructors

        public HydroVertexInfo(Shape _element, Type _hydroObjectType) : base(_element, _hydroObjectType)
        { }

        #endregion

        #region Fields

        private double left;

        private double top;

        private double height;

        private double width;

        #endregion

        #region IHydroVertexInfo

        public double Left
        {
            get { return left; }           
            set
            {
                left = value;
                TriggerPropertyChanged(nameof(Left));
            }
        }      

        public double Top
        {
            get { return top; }
            set
            {
                top = value;
                TriggerPropertyChanged(nameof(Top));
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                TriggerPropertyChanged(nameof(Height));
            }
        }

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                TriggerPropertyChanged(nameof(Width));
            }
        }

        #endregion
    }
}
