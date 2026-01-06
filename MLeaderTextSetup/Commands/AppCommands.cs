using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using MLeaderTextSetup.Actions;
using MLeaderTextSetup.Models;
using MLeaderTextSetup.Views;

namespace MLeaderTextSetup.Commands
{
    public class AppCommands
    {
        #region UI Commands
        [CommandMethod("MLEADER_TEXT_SETUP")]
        public void OpenSetup()
        {
            var win = new TextSetupWindow();
            Application.ShowModelessWindow(win);
        }
        #endregion

        #region Drawing Commands
        [CommandMethod("MLEADER_DRAW")]
        public void Draw()
        {
            var settings = SettingsAction.LoadFromDrawing() ?? new MLeaderTextSettingModel();
            MLeaderAction.CreateNewMLeader(settings);
        }
        #endregion
    }
}
