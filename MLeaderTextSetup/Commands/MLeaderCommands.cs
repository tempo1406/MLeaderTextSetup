using Autodesk.AutoCAD.Runtime;
using MLeaderTextSetup.Views;
using Autodesk.AutoCAD.ApplicationServices;


namespace MLeaderTextSetup.Commands
{
    public class MLeaderCommands
    {
        [CommandMethod("MLEADER_TEXT_SETUP")]
        public void OpenSetup()
        {
            var win = new TextSetupWindow();
            Application.ShowModelessWindow(win);
        }
    }
}
