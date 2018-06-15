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

        private List<CandidatesModel> _candidates;
        private MethodsInspector _methodsInspector;

        public AddTestWindow(MethodsInspector methodsInspector)
        {
            _methodsInspector = methodsInspector;
            _candidates = methodsInspector.GetAllMethods();
            InitializeComponent();
            this.Loaded += OnLoaded; 

            // Create Rows
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new GridLength(45);
            InnerGrid.RowDefinitions.Add(gridRow1);

            // Add first column header
            TextBlock txtBlock1 = new TextBlock();
            txtBlock1.Text = "#";
            txtBlock1.FontSize = 14;
            txtBlock1.FontWeight = FontWeights.Bold;           
            txtBlock1.VerticalAlignment = VerticalAlignment.Center;
            txtBlock1.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetRow(txtBlock1, 0);
            Grid.SetColumn(txtBlock1, 0);

            // Add second column header
            TextBlock txtBlock2 = new TextBlock();
            txtBlock2.Text = "Interface";
            txtBlock2.FontSize = 14;
            txtBlock2.FontWeight = FontWeights.Bold;         
            txtBlock2.VerticalAlignment = VerticalAlignment.Center;
            txtBlock2.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(txtBlock2, 0);
            Grid.SetColumn(txtBlock2, 1);

            // Add third column header
            TextBlock txtBlock3 = new TextBlock();
            txtBlock3.Text = "Method";
            txtBlock3.FontSize = 14;
            txtBlock3.FontWeight = FontWeights.Bold;           
            txtBlock3.VerticalAlignment = VerticalAlignment.Center;
            txtBlock3.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(txtBlock3, 0);
            Grid.SetColumn(txtBlock3, 2);

            // Add forth column header
            TextBlock txtBlock4 = new TextBlock();
            txtBlock4.Text = "Include?";
            txtBlock4.FontSize = 14;
            txtBlock4.FontWeight = FontWeights.Bold;          
            txtBlock4.VerticalAlignment = VerticalAlignment.Center;
            txtBlock4.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(txtBlock4, 0);
            Grid.SetColumn(txtBlock4, 3);

            //// Add column headers to the Grid
            InnerGrid.Children.Add(txtBlock1);
            InnerGrid.Children.Add(txtBlock2);
            InnerGrid.Children.Add(txtBlock3);
            InnerGrid.Children.Add(txtBlock4);
            InnerGrid.Background = new SolidColorBrush(Colors.White);

            int i = 0;

            foreach (var candidate in _candidates)
            {
                i++;

                // Create Grid raw
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(30);
               
                InnerGrid.RowDefinitions.Add(gridRow);               

                TextBlock candidateNumber = new TextBlock();
                candidateNumber.Text = i.ToString();
                candidateNumber.FontSize = 12;             
                candidateNumber.VerticalAlignment = VerticalAlignment.Center;
                candidateNumber.HorizontalAlignment = HorizontalAlignment.Center;      
                Grid.SetRow(candidateNumber, i);
                Grid.SetColumn(candidateNumber, 0);

                TextBlock candidateInterface = new TextBlock();
                candidateInterface.Text = candidate.InterfaceName;
                candidateInterface.FontSize = 12;          
                candidateInterface.VerticalAlignment = VerticalAlignment.Center;
                candidateInterface.HorizontalAlignment = HorizontalAlignment.Center;      
                Grid.SetRow(candidateInterface, i);
                Grid.SetColumn(candidateInterface, 1);

                TextBlock candidateMethod = new TextBlock();
                candidateMethod.Text = candidate.MethodName;
                candidateMethod.FontSize = 12; 
                candidateMethod.VerticalAlignment = VerticalAlignment.Center;
                candidateMethod.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(candidateMethod, i);
                Grid.SetColumn(candidateMethod, 2);

                CheckBox isIncluded = new CheckBox();
                isIncluded.Name = "Checkbox_" + i.ToString();
                isIncluded.VerticalAlignment = VerticalAlignment.Center;
                isIncluded.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(isIncluded, i);
                Grid.SetColumn(isIncluded, 3);

                // Add first row to Grid
                InnerGrid.Children.Add(candidateNumber);
                InnerGrid.Children.Add(candidateInterface);
                InnerGrid.Children.Add(candidateMethod);
                InnerGrid.Children.Add(isIncluded);     
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var checkboxes = InnerGrid.
                 Children
                 .Cast<UIElement>() 
                 .Where(x => x is CheckBox).Where(y => ((CheckBox)y).IsChecked.Value).ToList();
            var selectedCandidates = new List<CandidatesModel>();
            foreach (CheckBox checkbox in checkboxes)
            {
                var index = Convert.ToInt16(checkbox.Name.Split('_')[1])-1;
                selectedCandidates.Add(_candidates[index]);
            }

            _methodsInspector.GetElementsForScaffolding(selectedCandidates);
            this.Close();
        }
    }
}
