using Avaaj.Events;
using Avaaj.Models;
using Avaaj.Services;
using Avaaj.Utils;
using Avaaj.Utils.WPFUtils;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Avaaj.Dialogs
{
    public class EditViewModel : INotifyPropertyChanged
    {
        public enum AddTestResult { Cancel, Generate }
        public AddTestResult Result { get; set; }


        public IEventAggregator EventAggregator { get; set; }
        private string _documentPath;
        private TextViewSelection _selection;
        bool _existingTest = false;
        private string _selectionText;

        public event Action CloseRequest;

        private EditViewModel()
        {
          //  EventAggregator = VisualStudioServices.ComponentModel.GetService<IEventAggregator>();
        }

        public EditViewModel(string documentPath, TextViewSelection selection) : this()
        {
            this._documentPath = documentPath;
            this._selection= selection;
        }

        public string SelectionText => _existingTest ? _selectionText : _selection.Text;
              

        public bool IsExistingDocumentation => _existingTest;

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
          
            if (_existingTest)
            {
                Result = AddTestResult.Generate;
                CloseRequest?.Invoke();
                return;
            }

          //  var newDocFragment = new DocumentationFragment()
            //{
            //    Documentation = DocumentationText,
            //    Selection = this._selection,
            //};
            try
            {
                //string filepath = this._documentPath + Consts.CODY_DOCS_EXTENSION;
                //DocumentationFileHandler.AddDocumentationFragment(newDocFragment, filepath);
                //MessageBox.Show("Documentation added successfully.");
                //EventAggregator.SendMessage<DocumentationAddedEvent>(
                //    new DocumentationAddedEvent()
                //    {
                //        Filepath = filepath,
                //        DocumentationFragment = newDocFragment
                //    });
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
