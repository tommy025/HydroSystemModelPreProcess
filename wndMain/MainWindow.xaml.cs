using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Xml;
using System.Xml.Linq;
using System.IO;
using HydroSystemModelPreProcess.HydroObjects;
using Microsoft.Win32;

namespace HydroSystemModelPreProcess
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static RoutedUICommand changeState = new RoutedUICommand(nameof(ChangeState), nameof(ChangeState), typeof(MainWindow));

        public static RoutedUICommand ChangeState
        {
            get { return changeState; }
        }

        private MainWindowState mainWindowState;

        private Dictionary<FrameworkElement, ElementData> elementDataDic; 

        private FrameworkElement selectedElement;

        public FrameworkElement SelectedElement
        {
            get { return selectedElement; }           
            set
            {
                if (selectedElement == value ||
                    selectedElement != null && 
                    !elementDataDic.ContainsKey(selectedElement))
                    return;

                if (selectedElement != null)
                    selectedElement.Tag = "";

                selectedElement = value;
                if (selectedElement != null)
                {
                    selectedElement.Tag = "Selected";
                }
            }
        }

        public TranslateTransform Transform
        { get; set; }

        public CommandBinding ChangeStateCommandBinding
        { get; private set; }
            
        public CommandBinding SaveAsCommandBinding
        { get; private set; }

        public CommandBinding OpenCommandBinding
        { get; private set; }

        public HydroDocument HydroDocument
        { get; private set; }      

        public MainWindow()
        {
            InitializeComponent();

            ChangeStateCommandBinding = new CommandBinding(ChangeState);
            SaveAsCommandBinding = new CommandBinding(ApplicationCommands.SaveAs);
            SaveAsCommandBinding.Executed += SaveAsCommandBindingExecuted;
            OpenCommandBinding = new CommandBinding(ApplicationCommands.Open);
            OpenCommandBinding.Executed += OpenCommandBindingExecuted;
            CommandBindings.Add(ChangeStateCommandBinding);
            CommandBindings.Add(SaveAsCommandBinding);
            CommandBindings.Add(OpenCommandBinding);

            mainWindowState = new MainWindowSelecting(this);
            Transform = new TranslateTransform();
            HydroDocument = new HydroDocument();
            elementDataDic = new Dictionary<FrameworkElement, ElementData>();
        }

        private void OpenCommandBindingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //var dlg = new OpenFileDialog();
            //dlg.DefaultExt = ".sblx";
            //dlg.Filter = "SBLX File (.sblx)|*.sblx";
            //dlg.Multiselect = false;

            //if (dlg.ShowDialog(this) == true)
            //{
            //    using (var stream = dlg.OpenFile())
            //    {
            //        ReLoadHydroDocument(stream);
            //    }
            //}
        }

        private void SaveAsCommandBindingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //var dlg = new SaveFileDialog();
            //dlg.DefaultExt = ".sblx";
            //dlg.Filter = "SBLX File (.sblx)|*.sblx";

            //if (dlg.ShowDialog(this) == true)
            //{
            //    using (var stream = dlg.OpenFile())
            //    {
            //        HydroDocument.Save(stream);
            //    }
            //}
        }

        protected void RegisterHydroElement(FrameworkElement element)
        {
            var elementData = new ElementData();
            elementDataDic.Add(element, elementData);
            element.RenderTransform = Transform;
            drawingCanvas.Children.Add(element);
        }

        protected void UnregisterHydroElement(FrameworkElement element)
        {
            if (SelectedElement == element)
                SelectedElement = null;

            elementDataDic.Remove(element);
            drawingCanvas.Children.Remove(element);
        }

        //protected void ReLoadHydroDocument(Stream stream)
        //{
        //    ClearHydroElements();
        //    HydroDocument = HydroDocument.Load(stream);
        //    foreach (var element in HydroDocument.GetElements())
        //        RegisterHydroElement(element);
        //}

        protected void ClearHydroElements()
        {
            foreach (var kvp in elementDataDic.ToArray())
                RemoveHydroElement(kvp.Key);
        }

        private void MoveScreen(Vector v)
        {
            Transform.X += v.X;
            Transform.Y += v.Y;
        }

        private Point TransformToCanvasCoSys(Point p)
        {
            return Transform.Inverse.Transform(p);
        }

        public Rectangle AddConnectNode(Point position)
        {
            var element = HydroDocument.AddConnectNode(position);
            RegisterHydroElement(element);
            return element;
        }

        public Line AddPressurePipe(Point point1, Point point2)
        {
            var element = HydroDocument.AddPressurePipe(point1, point2);
            RegisterHydroElement(element);
            return element;
        }

        public void RemoveHydroElement(FrameworkElement element)
        {
            HydroDocument.RemoveHydroObject(element);
            UnregisterHydroElement(element);
        }

        public void MoveVertexCenter(Rectangle vertex, Point position)
        {
            Canvas.SetLeft(vertex, position.X - vertex.Width / 2);
            Canvas.SetTop(vertex, position.Y - vertex.Height / 2);

            var connectedEdges = HydroDocument.GetConnectedEdges(vertex);
            foreach (var edge in connectedEdges)
            {
                if (HydroDocument.GetVertex1(edge) == vertex)
                {
                    edge.X1 = position.X;
                    edge.Y1 = position.Y;
                }
                else
                {
                    edge.X2 = position.X;
                    edge.Y2 = position.Y;
                }
            }
        }

        public void SetEdgeFirstPoint(Line edge, Point position)
        {
            edge.X1 = position.X;
            edge.Y1 = position.Y;
        }

        public void SetEdgeSecondPoint(Line edge, Point position)
        {
            edge.X2 = position.X;
            edge.Y2 = position.Y;
        }

        public bool SetEdgeFirstNode(Line edge, Rectangle vertex)
        {
            if (HydroDocument.SetVertex1(edge, vertex))
            {
                if (vertex != null)
                    SetEdgeFirstPoint(edge, new Point(
                        Canvas.GetLeft(vertex) + vertex.Width / 2,
                        Canvas.GetTop(vertex) + vertex.Height / 2));

                return true;
            }
            else
                return false;
        }

        public bool SetEdgeSecondNode(Line edge, Rectangle vertex)
        {
            if (HydroDocument.SetVertex2(edge, vertex))
            {
                if (vertex != null)
                    SetEdgeSecondPoint(edge, new Point(
                        Canvas.GetLeft(vertex) + vertex.Width / 2,
                        Canvas.GetTop(vertex) + vertex.Height / 2));

                return true;
            }
            else
                return false;
        }

        private class ElementData
        {
            public ElementData()
            {
                var textBlock = new TextBlock();
                var textBinding = new Binding();
                textBinding.Source = this;
                textBinding.Path = new PropertyPath("DataObject.Name");
                textBlock.SetBinding(TextBlock.TextProperty, textBinding);
                LabelElement = textBlock;
            }

            public FrameworkElement LabelElement
            { get; set; }
        }

        private abstract class MainWindowState
        {
            protected MainWindow container;

            public MainWindowState(MainWindow _container)
            {
                container = _container;
                DrawingCanvas.MouseMove += OnDrawingCanvasMouseMove;
                DrawingCanvas.MouseLeftButtonDown += OnDrawingCanvasMouseLeftButtonDown;
                DrawingCanvas.MouseLeftButtonUp += OnDrawingCanvasMouseLeftButtonUp;

                container.ChangeStateCommandBinding.Executed += OnChangeState;
                container.ChangeStateCommandBinding.CanExecute += CanChangeState;
                container.SaveAsCommandBinding.CanExecute += CanChangeState;
                container.OpenCommandBinding.CanExecute += CanChangeState;
            }

            protected FrameworkElement SelectedElement
            {
                get { return container.SelectedElement; }
                set { container.SelectedElement = value; }
            }

            protected Canvas DrawingCanvas
            {
                get{ return container.drawingCanvas; }
            }

            protected virtual void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            { }

            protected virtual void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            { }

            protected virtual void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            { }

            protected void OnChangeState(object sender, RoutedEventArgs e)
            {
                var newState = null as MainWindowState;
                if (sender == this)
                {
                    newState = (e as RoutedPropertyChangedEventArgs<MainWindowState>).NewValue;
                }
                else if (e.OriginalSource == container.rbtnPointer)
                {
                    newState = new MainWindowSelecting(container);
                }
                else if (e.OriginalSource == container.rbtnDelete)
                {
                    newState = new MainWindowDeleting(container);
                }
                else if (e.OriginalSource == container.rbtnReConn)
                {
                    newState = new MainWindowReconnecting(container);
                }
                else if (e.OriginalSource == container.rbtnShiftScr)
                {
                    newState = new MainWindowShiftingScreen(container);
                }
                else if (e.OriginalSource == container.rbtnCNode)
                {
                    newState = new MainWindowAddingCNode(container);
                }
                else if (e.OriginalSource == container.rbtnPPipe)
                {
                    newState = new MainWindowSettingFirstPPipeNode(container, null);
                }

                LeaveState(newState);
                container.mainWindowState = newState;
            }

            /// <summary>
            /// Overrided method should not remove 'base.LeaveState' unless you know what you're doing.
            /// </summary>
            /// <param name="newState"></param>
            protected virtual void LeaveState(MainWindowState newState)
            {
                DrawingCanvas.MouseMove -= OnDrawingCanvasMouseMove;
                DrawingCanvas.MouseLeftButtonDown -= OnDrawingCanvasMouseLeftButtonDown;
                DrawingCanvas.MouseLeftButtonUp -= OnDrawingCanvasMouseLeftButtonUp;

                container.ChangeStateCommandBinding.Executed -= OnChangeState;
                container.ChangeStateCommandBinding.CanExecute -= CanChangeState;
                container.SaveAsCommandBinding.CanExecute -= CanChangeState;
                container.OpenCommandBinding.CanExecute -= CanChangeState;
            }

            protected virtual void CanChangeState(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = true;
            }
        }

        private class MainWindowSelecting : MainWindowState
        {
            public MainWindowSelecting(MainWindow _container) : base(_container)
            { }

            protected bool isDragging;

            protected Vector clickOffset;

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                if (SelectedElement != null)
                {
                    if (SelectedElement is Rectangle && isDragging == true)
                    { 
                        var drawPosition = e.GetPosition(DrawingCanvas) - clickOffset;
                        container.MoveVertexCenter(SelectedElement as Rectangle, drawPosition);
                    }
                }
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(DrawingCanvas);
                var hitElement = VisualTreeHelper.HitTest(DrawingCanvas, hitPosition).VisualHit;
                if (hitElement == null || hitElement == DrawingCanvas)
                    return;

                SelectedElement = hitElement as FrameworkElement;
                clickOffset = new Vector(hitPosition.X - Canvas.GetLeft(SelectedElement) - SelectedElement.Width / 2,
                    hitPosition.Y - Canvas.GetTop(SelectedElement) - SelectedElement.Height / 2);

                isDragging = true;
            }

            protected override void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isDragging = false;
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);
                SelectedElement = null;
            }
        }

        private class MainWindowDeleting : MainWindowState
        {
            public MainWindowDeleting(MainWindow _container) : base(_container)
            { }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(DrawingCanvas);
                var hitResult = VisualTreeHelper.HitTest(DrawingCanvas, hitPosition);
                var hitElement = hitResult.VisualHit as FrameworkElement;

                if (hitElement != DrawingCanvas)
                {
                    container.RemoveHydroElement(hitElement);
                }
            }
        }

        private class MainWindowReconnecting : MainWindowState
        {
            public MainWindowReconnecting(MainWindow _container) : base(_container)
            {

            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(DrawingCanvas);
                var hitElement = VisualTreeHelper.HitTest(DrawingCanvas, hitPosition).VisualHit as FrameworkElement;

                if (hitElement is Line)
                {
                    var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this,
                        new MainWindowSettingFirstPPipeNode(container, hitElement as Line));
                    OnChangeState(this, re);
                }                  
            }
        }

        private class MainWindowShiftingScreen : MainWindowState
        {
            public MainWindowShiftingScreen(MainWindow _container) : base(_container)
            {

            }

            protected bool isDragging;

            protected Point lastPos;

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                if (isDragging == true)
                {
                    var vector = e.GetPosition(DrawingCanvas) - lastPos;                 
                    container.MoveScreen(vector);
                    lastPos = e.GetPosition(DrawingCanvas);
                }              
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {               
                isDragging = true;
                lastPos = e.GetPosition(DrawingCanvas);
            }

            protected override void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {              
                isDragging = false;
                lastPos = e.GetPosition(DrawingCanvas);
            }
        }

        private class MainWindowAddingCNode : MainWindowState
        {
            public MainWindowAddingCNode(MainWindow _container) : base(_container)
            { }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(DrawingCanvas);
                var transPosition = container.TransformToCanvasCoSys(hitPosition);
                var element = container.AddConnectNode(transPosition);
            }
        }

        private class MainWindowSettingFirstPPipeNode : MainWindowState
        {
            public MainWindowSettingFirstPPipeNode(MainWindow _container, Line _originalElement) : base(_container)
            {              
                originalElement = _originalElement;
                if (originalElement != null)
                {
                    originalElement.Visibility = Visibility.Hidden;
                    element = container.AddPressurePipe(
                        new Point(originalElement.X1, originalElement.Y1),
                        new Point(originalElement.X2, originalElement.Y2));         
                }
                else
                    element = container.AddPressurePipe(new Point(), new Point()); 
            }

            private bool IsCreating
            {
                get { return originalElement == null; }
            }

            private Line element
            { get; set; }

            private Line originalElement;

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                if(!IsCreating)
                {
                    var mousePos = e.GetPosition(DrawingCanvas);
                    var transPosition = container.TransformToCanvasCoSys(mousePos);
                    container.SetEdgeFirstPoint(element, transPosition);
                }
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(DrawingCanvas);
                var hitElement = VisualTreeHelper.HitTest(DrawingCanvas, hitPosition).VisualHit;
                if (hitElement == DrawingCanvas || hitElement == element)
                {
                    var transPosition = container.TransformToCanvasCoSys(hitPosition);
                    container.SetEdgeFirstPoint(element, transPosition);
                }
                else if (hitElement is Rectangle)
                {
                    if (container.SetEdgeFirstNode(element, hitElement as Rectangle) == false)
                        return;
                }
                else
                    return;

                var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this, 
                    new MainWindowSettingSecondPPipeNode(container, element, originalElement));              
                OnChangeState(this, re);
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);
                if(!IsCreating)
                {
                    if(!(newState is MainWindowSettingSecondPPipeNode))
                    {
                        container.RemoveHydroElement(element);
                        originalElement.Visibility = Visibility.Visible;
                    }
                }    
                else
                {
                    if (!(newState is MainWindowSettingSecondPPipeNode))
                    {
                        container.RemoveHydroElement(element);
                    }
                }         
            }
        }

        private class MainWindowSettingSecondPPipeNode : MainWindowState
        {
            public MainWindowSettingSecondPPipeNode(MainWindow _container, Line _line, Line _originalElement) : base(_container)
            {
                if (_line == null)
                    throw new ArgumentNullException("Input line element must not be null!");

                element = _line;
                originalElement = _originalElement;       
                container.SetEdgeSecondPoint(element, new Point(_line.X1, _line.Y1));
                element.Visibility = Visibility.Visible;
            }

            private bool IsCreating
            {
                get { return originalElement == null; }
            }
        
            private Line element
            { get; set; }

            private Line originalElement;

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                var mousePos = e.GetPosition(DrawingCanvas);
                var transPosition = container.TransformToCanvasCoSys(mousePos);
                container.SetEdgeSecondPoint(element, transPosition);
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(DrawingCanvas);            
                var hitElement = VisualTreeHelper.HitTest(DrawingCanvas, hitPosition).VisualHit as FrameworkElement;
                var transPosition = container.TransformToCanvasCoSys(hitPosition);
                if (hitElement == DrawingCanvas || hitElement == element)
                {                   
                    container.SetEdgeSecondPoint(element, transPosition);
                }
                else if (hitElement is Rectangle)
                {
                    if (container.SetEdgeSecondNode(element, hitElement as Rectangle) == false)
                        return;

                    var vertex1 = container.HydroDocument.GetVertex1(element);
                    var vertex2 = container.HydroDocument.GetVertex2(element);
                    if (IsCreating)
                    {
                        if (container.HydroDocument.GetConnectedEdges(vertex1, vertex2).Length > 1)
                        {
                            container.SetEdgeSecondNode(element, null);
                            container.SetEdgeSecondPoint(element, transPosition);
                            return;
                        }
                    }
                    else
                    {
                        if (vertex1 == container.HydroDocument.GetVertex1(originalElement) &&
                            vertex2 == container.HydroDocument.GetVertex2(originalElement))
                        {
                            container.RemoveHydroElement(element);
                            originalElement.Visibility = Visibility.Visible;
                        }
                        else if (vertex2 == container.HydroDocument.GetVertex1(originalElement) &&
                            vertex1 == container.HydroDocument.GetVertex2(originalElement))
                        {
                            container.RemoveHydroElement(originalElement);
                        }
                        else
                        {
                            if (container.HydroDocument.GetConnectedEdges(vertex1, vertex2).Length > 1)
                            {
                                container.SetEdgeSecondNode(element, null);
                                container.SetEdgeSecondPoint(element, transPosition);
                                return;
                            }
                            else
                                container.RemoveHydroElement(originalElement);
                        }
                    }
                }
                else
                    return;

                var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this, (IsCreating ?
                    new MainWindowSettingFirstPPipeNode(container, null) as MainWindowState :
                    new MainWindowReconnecting(container) as MainWindowState));
                OnChangeState(this, re);
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);
                if (!IsCreating)
                {
                    if (!(newState is MainWindowReconnecting))
                    {
                        container.RemoveHydroElement(element);
                        originalElement.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (!(newState is MainWindowSettingFirstPPipeNode))
                        container.RemoveHydroElement(element);
                }
            }

            protected override void CanChangeState(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = false;
            }
        }

        private void OnMenuItemOpenProcedureManagerClick(object sender, RoutedEventArgs e)
        {
            var wndProcedureManager = new ProcedureManagerWindow();
            wndProcedureManager.DataContext = HydroDocument;
            wndProcedureManager.ShowDialog();
        }
    }
}
