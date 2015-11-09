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
    /// ProcedureSelectorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProcedureSelectorWindow : Window
    {
        public ProcedureSelectorWindow()
        {
            InitializeComponent();
            lbxProcedureSelector.SelectedIndex = 0;
        }

        public string SelectedProcedure
        { get; private set; }

        private void OnButtonConfirmClick(object sender, RoutedEventArgs e)
        {
            SelectedProcedure = lbxProcedureSelector.SelectedItem as string;
            DialogResult = true;
        }

        private void OnButtonCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
