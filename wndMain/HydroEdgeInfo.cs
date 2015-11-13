using System;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public class HydroEdgeInfo : HydroObjectInfo, IHydroEdgeInfo
    {
        #region StaticMethods

        public static HydroEdgeInfo CreateHydroEdgeInfo(Line edge, Type hydroObjectType)
        {
            var info = new HydroEdgeInfo(edge, hydroObjectType);
            edge.DataContext = info;
            return info;
        }

        #endregion

        #region Constructors

        protected HydroEdgeInfo(Line _edge, Type _hydroObjectType) : base(_hydroObjectType)
        {
            edge = _edge;
        }

        #endregion

        #region Fields

        private double x1;

        private double y1;

        private double x2;

        private double y2;

        private readonly Line edge;

        #endregion

        #region Properties

        public double X1
        {
            get { return x1; }
            set
            {
                x1 = value;
                TriggerPropertyChanged(nameof(X1));
            }
        }
            
        public double Y1
        {
            get { return y1; }
            set
            {
                y1 = value;
                TriggerPropertyChanged(nameof(Y1));
            }
        }

        public double X2
        {
            get { return x2; }
            set
            {
                x2 = value;
                TriggerPropertyChanged(nameof(X2));
            }
        }

        public double Y2
        {
            get { return y2; }
            set
            {
                y2 = value;
                TriggerPropertyChanged(nameof(Y2));
            }
        }

        public override Shape Element
        {
            get { return edge; }
        }

        #endregion

        #region IHydroEdgeInfo

        public Line Edge
        {
            get { return edge; }
        }

        #endregion
    }
}
