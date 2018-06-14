using Microsoft.VisualStudio.PlatformUI;

namespace Avaaj.Dialogs
{
    public class BaseDialogWindow : DialogWindow
    {

        public BaseDialogWindow()
        {
            this.HasMaximizeButton = true;
            this.HasMinimizeButton = true;
        }
    }
}
