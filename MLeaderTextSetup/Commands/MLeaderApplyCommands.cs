using Autodesk.AutoCAD.Runtime;
using MLeaderTextSetup.Actions;


namespace MLeaderTextSetup.Commands
{
    public class MLeaderApplyCommands
    {
        [CommandMethod("MLEADER_TEXT_APPLY")]
        public void Apply()
        {
            var settings = SettingsActions.LoadFromDrawing() ?? new Models.MLeaderTextSettings();
        }
    }
}
