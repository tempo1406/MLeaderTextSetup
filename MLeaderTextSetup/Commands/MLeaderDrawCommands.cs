using Autodesk.AutoCAD.Runtime;
using MLeaderTextSetup.Actions;
using MLeaderTextSetup.Models;

namespace MLeaderTextSetup.Commands
{
    public class MLeaderDrawCommands
    {
        private MLeaderTextSettings GetSettingsFromBridge()
        {
            return new MLeaderTextSettings
            {
                TextStyleName = DrawDataBridge.TextStyleName,
                TextHeight = DrawDataBridge.TextHeight,
                ColorIndex = DrawDataBridge.ColorIndex,
                ColorByLayer = DrawDataBridge.ColorByLayer,
                FormatTemplate = DrawDataBridge.FormatTemplate
            };
        }

        [CommandMethod("MLEADER_DRAW")]
        public void Draw()
        {
            var settings = GetSettingsFromBridge();
            MLeaderActions.CreateNewMLeader(settings);
        }

        [CommandMethod("MLEADER_MULTI_POINT")]
        public void MultiPoint()
        {
            var settings = GetSettingsFromBridge();
            MLeaderActions.CreateMLeaderWithMultiplePoints(settings);
        }

        [CommandMethod("MLEADER_MULTI_DRAW")]
        public void MultiDraw()
        {
            var settings = GetSettingsFromBridge();
            MLeaderActions.CreateMultipleMLeaders(settings);
        }
    }
}
