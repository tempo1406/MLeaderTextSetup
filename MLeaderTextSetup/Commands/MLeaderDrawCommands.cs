using Autodesk.AutoCAD.Runtime;
using MLeaderTextSetup.Actions;

namespace MLeaderTextSetup.Commands
{
    public class MLeaderDrawCommands
    {
        [CommandMethod("MLEADER_DRAW")]
        public void Draw()
        {
            var settings = SettingsActions.LoadFromDrawing() ?? new Models.MLeaderTextSettings();
            MLeaderActions.CreateNewMLeader(settings);
        }
    }
}