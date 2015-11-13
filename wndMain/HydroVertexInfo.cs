using System;
using System.Windows.Data;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public class HydroVertexInfo : HydroObjectInfo, IHydroVertexInfo
    {
        #region StaticMethods

        public static HydroVertexInfo CreateHydroVertexInfo(Rectangle vertex, Type hydroObjectType)
        {
            var info = new HydroVertexInfo(vertex, hydroObjectType);
            vertex.DataContext = info;
            return info;
        }

        #endregion

        #region Constructors

        protected HydroVertexInfo(Rectangle _vertex, Type _hydroObjectType) : base(_hydroObjectType)
        {
            vertex = _vertex;
        }

        #endregion

        #region Fields

        private double left;

        private double top;

        private readonly Rectangle vertex;

        #endregion

        #region Properties

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

        public override Shape Element
        {
            get { return vertex; }
        }

        #endregion

        #region IHydroVertexInfo

        public Rectangle Vertex
        {
            get { return vertex; }
        }

        #endregion
    }
}
