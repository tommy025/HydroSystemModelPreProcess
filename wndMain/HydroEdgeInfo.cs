using System;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public class HydroEdgeInfo : HydroObjectInfo, IHydroEdgeInfo
    {
        #region Constructors

        public HydroEdgeInfo(Shape _element, Type _hydroObjectType) : base(_element, _hydroObjectType)
        { }

        #endregion

        #region Fields

        private double x1;

        private double y1;

        private double x2;

        private double y2;

        #endregion

        #region IHydroEdgeInfo

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

        private IHydroVertexInfo vertex1;   

        public IHydroVertexInfo Vertex1
        {
            get { return vertex1; }
            set
            {
                if (vertex2 == value && value != null)
                    throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                vertex1 = value;
                TriggerPropertyChanged(nameof(Vertex1));
            }
        }

        private IHydroVertexInfo vertex2;

        public IHydroVertexInfo Vertex2
        {
            get { return vertex2; }
            set
            {
                if (vertex1 == value && value != null)
                    throw new ArgumentException("Vertexs of HydroEdgeInfo must be different!");

                vertex2 = value;
                TriggerPropertyChanged(nameof(Vertex2));
            }
        }

        public IHydroVertexInfo[] GetVertexs()
        {
            return new IHydroVertexInfo[] { Vertex1, Vertex2 };
        }

        public bool IsBetween(IHydroVertexInfo v1, IHydroVertexInfo v2)
        {
            if (v1 == null || v2 == null)
                return false;

            if (Vertex1 == v1 && Vertex2 == v2 || Vertex2 == v1 && Vertex1 == v2)
                return true;
            else
                return false;
        }

        public bool IsConnectedTo(IHydroVertexInfo vertex)
        {
            if (vertex == Vertex1 || vertex == Vertex2)
                return true;
            else
                return false;
        }    

        #endregion      
    }
}
