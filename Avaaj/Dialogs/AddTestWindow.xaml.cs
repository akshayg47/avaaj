﻿using System;
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

            var candidates = (DataContext as EditViewModel).Candidates;           

            InnerGrid.ShowGridLines = true;

            // Create Columns
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();
            ColumnDefinition gridCol4 = new ColumnDefinition();
            InnerGrid.ColumnDefinitions.Add(gridCol1);
            InnerGrid.ColumnDefinitions.Add(gridCol2);
            InnerGrid.ColumnDefinitions.Add(gridCol3);
            InnerGrid.ColumnDefinitions.Add(gridCol4);

            // Create Rows
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new GridLength(45);
            InnerGrid.RowDefinitions.Add(gridRow1);

            // Add first column header
            TextBlock txtBlock1 = new TextBlock();
            txtBlock1.Text = "#";
            txtBlock1.FontSize = 14;
            txtBlock1.FontWeight = FontWeights.Bold;
            txtBlock1.Foreground = new SolidColorBrush(Colors.Green);
            txtBlock1.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(txtBlock1, 0);
            Grid.SetColumn(txtBlock1, 0);

            // Add second column header
            TextBlock txtBlock2 = new TextBlock();
            txtBlock2.Text = "Interface";
            txtBlock2.FontSize = 14;
            txtBlock2.FontWeight = FontWeights.Bold;
            txtBlock2.Foreground = new SolidColorBrush(Colors.Green);
            txtBlock2.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(txtBlock2, 0);
            Grid.SetColumn(txtBlock2, 1);

            // Add third column header
            TextBlock txtBlock3 = new TextBlock();
            txtBlock3.Text = "Method";
            txtBlock3.FontSize = 14;
            txtBlock3.FontWeight = FontWeights.Bold;
            txtBlock3.Foreground = new SolidColorBrush(Colors.Green);
            txtBlock3.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(txtBlock3, 0);
            Grid.SetColumn(txtBlock3, 2);

            // Add forth column header
            TextBlock txtBlock4 = new TextBlock();
            txtBlock4.Text = "Include?";
            txtBlock4.FontSize = 14;
            txtBlock4.FontWeight = FontWeights.Bold;
            txtBlock4.Foreground = new SolidColorBrush(Colors.Green);
            txtBlock4.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(txtBlock4, 0);
            Grid.SetColumn(txtBlock4, 3);

            //// Add column headers to the Grid
            InnerGrid.Children.Add(txtBlock1);
            InnerGrid.Children.Add(txtBlock2);
            InnerGrid.Children.Add(txtBlock3);
            InnerGrid.Children.Add(txtBlock4);
            int i = 0;

            foreach (var candidate in candidates)
            {
                i++;

                // Create Grid raw
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(45);
                InnerGrid.RowDefinitions.Add(gridRow);

                TextBlock candidateNumber = new TextBlock();
                candidateNumber.Text = i.ToString();
                candidateNumber.FontSize = 12;
                candidateNumber.FontWeight = FontWeights.Bold;
                Grid.SetRow(candidateNumber, i);
                Grid.SetColumn(candidateNumber, 0);

                TextBlock candidateInterface = new TextBlock();
                candidateInterface.Text = candidate.InterfaceName;
                candidateInterface.FontSize = 12;
                candidateInterface.FontWeight = FontWeights.Bold;
                Grid.SetRow(candidateInterface, i);
                Grid.SetColumn(candidateInterface, 1);

                TextBlock candidateMethod = new TextBlock();
                candidateMethod.Text = candidate.MethodName;
                candidateMethod.FontSize = 12;
                candidateMethod.FontWeight = FontWeights.Bold;
                Grid.SetRow(candidateMethod, i);
                Grid.SetColumn(candidateMethod, 2);

                CheckBox isIncluded = new CheckBox();
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
    }
}
