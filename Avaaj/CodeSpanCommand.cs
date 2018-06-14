using System;
using System.ComponentModel.Design;
using System.Globalization;
using Avaaj.Dialogs;
using Avaaj.Models;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System.IO;
using System.Collections.Generic;

namespace Avaaj
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CodeSpanCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("187c712d-ea69-463c-8013-ed2b1ba89cf8");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeSpanCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CodeSpanCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CodeSpanCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CodeSpanCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            TextViewSelection selection = GetSelection(ServiceProvider);
            string activeDocumentName = GetActiveDocumentFileName(ServiceProvider);
            var activeDllPath = GetActiveDocumentAssemblyPath(ServiceProvider);
            var methodInspector = new MethodsInspector(activeDocumentName, selection.Text, activeDllPath);
            var candidates = methodInspector.GetAllMethods();

            ShowAddTestWindow(candidates, selection);
        }

        private void ShowAddTestWindow(List<CandidatesModel> candidates, TextViewSelection selection)
        {
            var documentationControl = new AddTestWindow();
            documentationControl.DataContext = new EditViewModel(candidates, selection);
            documentationControl.ShowDialog();
        }

        private string GetActiveDocumentAssemblyPath(IServiceProvider serviceProvider)
        {
            EnvDTE80.DTE2 applicationObject = serviceProvider.GetService(typeof(DTE)) as EnvDTE80.DTE2;
            var projFileName = Path.GetFileName(applicationObject.ActiveDocument.ProjectItem.ContainingProject.FileName);

            return Path.GetDirectoryName(applicationObject.ActiveDocument.Path) + "\\bin\\debug\\"
                + projFileName.Substring(0, projFileName.LastIndexOf('.')) + ".dll";
        }

        private string GetActiveDocumentFileName(IServiceProvider serviceProvider)
        {
            EnvDTE80.DTE2 applicationObject = serviceProvider.GetService(typeof(DTE)) as EnvDTE80.DTE2;
            return applicationObject.ActiveDocument.Name.Split('.')[0];

        }

        private TextViewSelection GetSelection(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof(SVsTextManager));
            var textManager = service as IVsTextManager2;
            IVsTextView view;
            int result = textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out view);

            view.GetSelection(out int startLine, out int startColumn, out int endLine, out int endColumn);//end could be before beginning

            int ok = view.GetNearestPosition(startLine, startColumn, out int position1, out int piVirtualSpaces);
            ok = view.GetNearestPosition(endLine, endColumn, out int position2, out piVirtualSpaces);

            var startPosition = Math.Min(position1, position2);
            var endPosition = Math.Max(position1, position2);

            view.GetSelectedText(out string selectedText);

            TextViewSelection selection = new TextViewSelection(startPosition, endPosition, selectedText);
            return selection;
        }
    }
}
