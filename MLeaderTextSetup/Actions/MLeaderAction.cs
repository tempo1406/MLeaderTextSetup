using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using MLeaderTextSetup.Models;
using System;
using System.Collections.Generic;

namespace MLeaderTextSetup.Actions
{
    public static class MLeaderAction
    {
        #region Vẽ Mleader
        public static void CreateNewMLeader(MLeaderTextSettingModel settings)
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

                    Color finalColor;
                    if (settings.ColorByLayer)
                        finalColor = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                    else
                        finalColor = Color.FromColorIndex(ColorMethod.ByAci, settings.ColorIndex);

                    var mt = new MText();
                    mt.SetDatabaseDefaults();
                    var demoPreview = new PreviewDataModel();
                    mt.Contents = PreviewAction.BuildText(settings, demoPreview);
                    mt.TextStyleId = textStyleId;
                    mt.TextHeight = settings.TextHeight;
                    mt.Location = ppr2.Value;
                    mt.Color = finalColor;

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

                    mleader.Color = finalColor;

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
        #endregion

        #region Vẽ multiple vertex leader
        public static void CreateSingleLeaderMultiVertex(MLeaderTextSettingModel settings)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var ed = doc.Editor;
            var db = doc.Database;

            try
            {
                var vertices = new List<Point3d>();

                ed.WriteMessage("\nChọn các điểm của leader (điểm đầu là mũi tên). Enter để kết thúc:");
                while (true)
                {
                    var opt = new PromptPointOptions("\nChọn điểm leader: ")
                    {
                        AllowNone = true
                    };

                    if (vertices.Count > 0)
                    {
                        opt.UseBasePoint = true;
                        opt.BasePoint = vertices[vertices.Count - 1];
                    }

                    var res = ed.GetPoint(opt);

                    if (res.Status == PromptStatus.OK)
                    {
                        vertices.Add(res.Value);
                    }
                    else if (res.Status == PromptStatus.None || res.Status == PromptStatus.Cancel)
                    {
                        break;
                    }
                    else
                    {
                        return;
                    }
                }

                if (vertices.Count < 2)
                {
                    ed.WriteMessage("\nCần ít nhất 2 điểm (mũi tên + ít nhất 1 điểm bẻ).");
                    return;
                }

                var landingRes = ed.GetPoint("\nChọn điểm đặt Text: ");
                if (landingRes.Status != PromptStatus.OK) return;

                var landingPoint = landingRes.Value;

                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                    ObjectId textStyleId = db.Textstyle;
                    var tst = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    if (!string.IsNullOrWhiteSpace(settings.TextStyleName) && tst.Has(settings.TextStyleName))
                        textStyleId = tst[settings.TextStyleName];

                    var finalColor = settings.ColorByLayer
                        ? Color.FromColorIndex(ColorMethod.ByLayer, 256)
                        : Color.FromColorIndex(ColorMethod.ByAci, settings.ColorIndex);

                    var previewData = new PreviewDataModel();
                    string contents = PreviewAction.BuildText(settings, previewData);

                    var mt = new MText();
                    mt.SetDatabaseDefaults();
                    mt.Contents = contents;
                    mt.TextStyleId = textStyleId;
                    mt.TextHeight = settings.TextHeight;
                    mt.Location = landingPoint;
                    mt.Color = finalColor;

                    var ml = new MLeader();
                    ml.SetDatabaseDefaults();
                    ml.ContentType = ContentType.MTextContent;
                    ml.MText = mt;

                    ml.TextLocation = landingPoint;
                    ml.LeaderLineType = LeaderType.StraightLeader;
                    ml.Color = finalColor;

                    int leaderIndex = ml.AddLeader();
                    int lineIndex = ml.AddLeaderLine(leaderIndex);

                    ml.AddFirstVertex(lineIndex, vertices[0]);

                    for (int i = 1; i < vertices.Count; i++)
                    {
                        ml.AddLastVertex(lineIndex, vertices[i]);
                    }

                    ml.AddLastVertex(lineIndex, landingPoint);

                    btr.AppendEntity(ml);
                    tr.AddNewlyCreatedDBObject(ml, true);

                    tr.Commit();
                }

                ed.WriteMessage("\nĐã vẽ MLeader (1 leader, nhiều điểm gấp khúc) thành công!");
            }
            catch (Exception ex)
            {
                ed.WriteMessage($"\nLỗi: {ex.Message}");
            }
        }
        #endregion
    }
}
