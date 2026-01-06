using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using MLeaderTextSetup.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLeaderTextSetup.Actions
{
    public static class SettingsActions
    {
        private const string DictName = "MLEADER_TEXT_SETUP_DICT";
        private const string RecordKey = "SETTINGS_JSON";

        public static MLeaderTextSettings LoadFromDrawing()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return null;
            var db = doc.Database;

            DocumentLock docLock = null;
            try
            {
                docLock = doc.LockDocument();
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                if (ex.ErrorStatus != Autodesk.AutoCAD.Runtime.ErrorStatus.LockViolation)
                    throw;
            }

            try
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var nod = (DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForRead);

                    if (!nod.Contains(DictName)) return null;
                    var dict = (DBDictionary)tr.GetObject(nod.GetAt(DictName), OpenMode.ForRead);

                    if (!dict.Contains(RecordKey)) return null;
                    var xr = (Xrecord)tr.GetObject(dict.GetAt(RecordKey), OpenMode.ForRead);

                    var rb = xr.Data;
                    if (rb == null) return null;

                    var arr = rb.AsArray();
                    if (arr.Length == 0) return null;

                    var json = arr[0].Value as string;
                    tr.Commit();

                    if (string.IsNullOrWhiteSpace(json)) return null;
                    return JsonConvert.DeserializeObject<MLeaderTextSettings>(json);
                }
            }
            finally
            {
                if (docLock != null)
                    docLock.Dispose();
            }
        }

        public static void SaveToDrawing(MLeaderTextSettings settings)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var db = doc.Database;

            string json = JsonConvert.SerializeObject(settings);

            DocumentLock docLock = null;
            try
            {
                docLock = doc.LockDocument();
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                if (ex.ErrorStatus != ErrorStatus.LockViolation)
                {
                    throw;
                }
            }

            try
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var nod = (DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite);

                    if (!nod.Contains(DictName))
                    {
                        var newDict = new DBDictionary();
                        nod.SetAt(DictName, newDict);
                        tr.AddNewlyCreatedDBObject(newDict, true);
                    }

                    var dict = (DBDictionary)tr.GetObject(nod.GetAt(DictName), OpenMode.ForWrite);

                    Xrecord xr;
                    if (!dict.Contains(RecordKey))
                    {
                        xr = new Xrecord();
                        dict.SetAt(RecordKey, xr);
                        tr.AddNewlyCreatedDBObject(xr, true);
                    }
                    else
                    {
                        xr = (Xrecord)tr.GetObject(dict.GetAt(RecordKey), OpenMode.ForWrite);
                    }

                    xr.Data = new ResultBuffer(new TypedValue((int)DxfCode.Text, json));

                    tr.Commit();
                }
            }
            finally
            {
                docLock?.Dispose();
            }
        }
    }
}
