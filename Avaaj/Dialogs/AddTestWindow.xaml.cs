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
using static Avaaj.Dialogs.EditViewModel;

namespace Avaaj.Dialogs
{
    /// <summary>
    /// Interaction logic for AddTestWindow.xaml
    /// </summary>
    public partial class AddTestWindow
    {
        public AddTestWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        public AddTestResult Result => (DataContext as EditViewModel).Result;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
          //  DocumentationTextBox.Focus();
            RegisterToViewModelEvents();
        }

        private void RegisterToViewModelEvents()
        {
            var vm = (DataContext as EditViewModel);
            vm.CloseRequest += () =>
            {
                this.Close();
            };
        }
    }
}
