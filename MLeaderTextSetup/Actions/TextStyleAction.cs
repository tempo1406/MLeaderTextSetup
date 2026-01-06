using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace MLeaderTextSetup.Actions
{
    public class TextStyleAction
    {
        public static List<string> GetTextStyleNames()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return new List<string>();
            var db = doc.Database;

            DocumentLock docLock = null;
            try
            {
                docLock = doc.LockDocument();
            }
            catch (Exception ex)
            {
                if (ex.ErrorStatus != ErrorStatus.LockViolation)
                    throw;
            }

            try
            {
                var result = new List<string>();
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var tst = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    foreach (ObjectId id in tst)
                    {
                        var rec = (TextStyleTableRecord)tr.GetObject(id, OpenMode.ForRead);
                        result.Add(rec.Name);
                    }
                    tr.Commit();
                }
                return result;
            }
            finally
            {
                docLock?.Dispose();
            }
        }
    }
}
