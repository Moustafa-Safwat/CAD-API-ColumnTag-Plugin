using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WPFCADAPI.Ribbon;
using WPFCADAPI.View;

namespace WPFCADAPI.ViewModel
{
    public class CadLayers : ICadCommand
    {

        public static List<string> layernames;

        public override void Execute()
        {
            layername();
        }

        [CommandMethod("coltag")]
        public void layername()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            DocumentLock docd = doc.LockDocument();
            layernames = new List<string>();
            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                try
                {
                    #region Layernames
                    LayerTable lt = tran.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                    foreach (var item in lt)
                    {
                        LayerTableRecord ltr = tran.GetObject(item, OpenMode.ForRead) as LayerTableRecord;
                        string name = ltr.Name;
                        layernames.Add(name);
                    }
                    #endregion

                    tran.Commit();
                    Mainview view = new Mainview();
                    Application.ShowModalWindow(view);
                }
                catch (System.Exception ex)
                {
                    editor.WriteMessage(ex.Message);
                    tran.Abort();
                }
            }
        }
    }
}
