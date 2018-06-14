using Avaaj.Events;
using Avaaj.Models;
using Avaaj.Utils.WPFUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Avaaj.Dialogs
{
    public class EditViewModel : INotifyPropertyChanged
    {
        public enum AddTestResult { Cancel, Generate }
        public AddTestResult Result { get; set; }
        public IEventAggregator EventAggregator { get; set; }
        private List<CandidatesModel> candidates;
        private TextViewSelection _selection;
        public event Action CloseRequest;

        private EditViewModel()
        {
          //  EventAggregator = VisualStudioServices.ComponentModel.GetService<IEventAggregator>();
        }

        public EditViewModel(List<CandidatesModel> candidates, TextViewSelection selection) : this()
        {
            this.candidates = candidates;
            this._selection= selection;
        }

        public string SelectionText => _selection.Text;             

        public ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(_ =>
                    {
                        Result = AddTestResult.Cancel;
                        CloseRequest?.Invoke();
                    });
                }
                return _cancelCommand;
            }
        }

        public ICommand _generateCommand;
        public ICommand GenerateCommand
        {
            get
            {
                if (_generateCommand == null)
                {
                    _generateCommand = new RelayCommand(GenerateTest);
                }
                return _generateCommand;
            }
        }

        private void GenerateTest(object obj)
        {                      
            try
            {                
                Result = AddTestResult.Generate;
                CloseRequest?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Test generation failed. Exception: " + ex.ToString());
            }
        }   


        #region NOTIFY PROPERTY CHANGE
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertChange([CallerMemberName]string propertyName = null)
        {
            if (propertyName == null)
                return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion NOTIFY PROPERTY CHANGE
    }
}
