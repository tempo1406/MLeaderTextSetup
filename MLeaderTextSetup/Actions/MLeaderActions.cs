using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using MLeaderTextSetup.Models;

namespace MLeaderTextSetup.Actions
{
    public static class MLeaderActions
    {
        public static void CreateNewMLeader(MLeaderTextSettings settings)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var ed = doc.Editor;
            var db = doc.Database;

            try
            {
                var ppr1 = ed.GetPoint("\nChọn điểm đầu mũi tên: ");
                if (ppr1.Status != PromptStatus.OK) return;

                var ppr2 = ed.GetPoint(new PromptPointOptions("\nChọn điểm vị trí text: ")
                {
                    UseBasePoint = true,
                    BasePoint = ppr1.Value
                });
                if (ppr2.Status != PromptStatus.OK) return;

                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    ObjectId textStyleId = db.Textstyle;
                    var tst = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    if (!string.IsNullOrWhiteSpace(settings.TextStyleName) && tst.Has(settings.TextStyleName))
                        textStyleId = tst[settings.TextStyleName];

                    var mt = new MText();
                    mt.SetDatabaseDefaults();
                    var demoPreview = new PreviewData();
                    mt.Contents = PreviewActions.BuildText(settings, demoPreview);
                    mt.TextStyleId = textStyleId;
                    mt.TextHeight = settings.TextHeight;
                    mt.Location = ppr2.Value;

                    var mleader = new MLeader();
                    mleader.SetDatabaseDefaults();
                    mleader.ContentType = ContentType.MTextContent;
                    mleader.MText = mt;
                    mleader.TextLocation = ppr2.Value;
                    mleader.LeaderLineType = LeaderType.StraightLeader;

                    int leaderIndex = mleader.AddLeader();
                    int lineIndex = mleader.AddLeaderLine(leaderIndex);
                    mleader.AddFirstVertex(lineIndex, ppr1.Value);
                    mleader.AddLastVertex(lineIndex, ppr2.Value);

                    if (settings.ColorByLayer)
                        mleader.Color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                    else
                        mleader.Color = Color.FromColorIndex(ColorMethod.ByAci, settings.ColorIndex);

                    btr.AppendEntity(mleader);
                    tr.AddNewlyCreatedDBObject(mleader, true);

                    tr.Commit();
                }

                ed.WriteMessage("\nĐã tạo MLeader thành công!");
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nLỗi: {ex.Message}");
            }
        }
    }
}
