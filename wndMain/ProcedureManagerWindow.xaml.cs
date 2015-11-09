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
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    /// <summary>
    /// ProcedureManagerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProcedureManagerWindow : Window
    {
        private static RoutedUICommand removeProcedure = new RoutedUICommand(nameof(RemoveProcedure), nameof(RemoveProcedure), typeof(ProcedureManagerWindow));

        public static RoutedUICommand RemoveProcedure
        {
            get { return removeProcedure; }
        }

        private static RoutedUICommand duplicateProcedure = new RoutedUICommand(nameof(DuplicateProcedure), nameof(DuplicateProcedure), typeof(ProcedureManagerWindow));

        public static RoutedUICommand DuplicateProcedure
        {
            get { return duplicateProcedure; }
        }

        public CommandBinding RemoveProcedureCommandBinding
        { get; private set; }

        public CommandBinding DuplicateProcedureCommandBinding
        { get; private set; }

        public ProcedureManagerWindow()
        {
            InitializeComponent();

            RemoveProcedureCommandBinding = new CommandBinding(RemoveProcedure);
            RemoveProcedureCommandBinding.Executed += OnRemoveProcedureCommandBindingExecuted;
            RemoveProcedureCommandBinding.CanExecute += RemoveOrDuplicateProcedureCommandBindingCanExecute;
            CommandBindings.Add(RemoveProcedureCommandBinding);
            DuplicateProcedureCommandBinding = new CommandBinding(DuplicateProcedure);
            DuplicateProcedureCommandBinding.Executed += OnDuplicateProcedureCommandBindingExecuted;
            DuplicateProcedureCommandBinding.CanExecute += RemoveOrDuplicateProcedureCommandBindingCanExecute;
            CommandBindings.Add(DuplicateProcedureCommandBinding);
        }

        private HydroDocument HydroDocument
        {
            get { return DataContext as HydroDocument; }
        }

        private void OnButtonAddProcedureClick(object sender, RoutedEventArgs e)
        {
            var selector = new ProcedureSelectorWindow();
            if (selector.ShowDialog() == true)
            {
                switch (selector.SelectedProcedure)
                {
                    case "恒定流水头损失计算":
                        HydroDocument.AddHeadLossCalculation();
                        break;
                }
            }
        }

        private void RemoveOrDuplicateProcedureCommandBindingCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lbxProcedures.SelectedItem != null;
        }

        private void OnDuplicateProcedureCommandBindingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HydroDocument.DuplicatedProcedure(lbxProcedures.SelectedItem as HydroProcedure);
        }

        private void OnRemoveProcedureCommandBindingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HydroDocument.RemoveProcedure(lbxProcedures.SelectedItem as HydroProcedure);
        }
    }
}
