using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    public interface IReadOnlyHydroObjectGraph : IEnumerable<Shape>, INotifyCollectionChanged
    {
        bool Contains(Shape item);

        int Count
        { get; }

        Rectangle GetVertex1(Line edge);

        Rectangle GetVertex2(Line edge);

        bool IsConnected(Rectangle vertex1, Rectangle vertex2);

        bool IsConnectedTo(Line edge, Rectangle vertex);

        bool IsBetween(Line edge, Rectangle vertex1, Rectangle vertex2);

        Rectangle[] GetAllVertexs();

        Rectangle[] GetVertexs(Line edge);

        Line[] GetAllEdges();

        Line[] GetEdges(Rectangle vertex);

        Line[] GetEdges(Rectangle vertex1, Rectangle vertex2);

        Shape GetObject(string name);
    }
}
