using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Windows.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPFCADAPI.Command;
using WPFCADAPI.Model;
using WPFCADAPI.View;

namespace WPFCADAPI.ViewModel
{
    public class Viewmodel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Fields
        List<string> layersnames = CadLayers.layernames;
        string textstylename; //Binding From WPF
        string selectedlayer;
        string columnModel;
        string selectedtextlayer;
        bool annotativecheck = true;
        bool coldim = true;
        bool colmodel = true;
        bool round;
        bool run;
        double scaleFactor;
        double distancefactor;
        ObservableCollection<ObjectModel> columnDimensions;
        ObservableCollection<string> columnDimensionsOutput;
        ObservableCollection<string> columnModelOutput;
        ObservableCollection<ColumnModel> columnModelInternal;

        ObservableCollection<Foundation> foundationelements;

        SelectionSet sel;
        #endregion

        #region Property
        public List<string> Layersnames
        {
            get
            {
                return layersnames;
            }
            set
            {
                layersnames = value;
            }

        }
        public string Textstylename
        {
            get
            {
                return textstylename;
            }
            set
            {
                textstylename = value;
                OnPropertyChanged();
            }
        }
        public string Selectedlayer
        {
            get
            {
                return selectedlayer;
            }
            set
            {
                selectedlayer = value;
                OnPropertyChanged();
            }
        }
        public string Selectedtextlayer
        {
            get
            {
                return selectedtextlayer;
            }
            set
            {
                selectedtextlayer = value;
                OnPropertyChanged();
            }
        }
        public bool Annotativecheck
        {
            get
            {
                return annotativecheck;
            }
            set
            {
                annotativecheck = value;
                OnPropertyChanged();
            }
        }
        public int Roundofftxt { set; get; }
        public bool Coldim
        {
            get
            {
                return coldim;
            }
            set
            {
                coldim = value;
                OnPropertyChanged();
            }
        }
        public bool Colmodel
        {
            get { return colmodel; }
            set
            {
                colmodel = value;
                OnPropertyChanged();
            }
        }
        public bool Round
        {
            get
            {
                return round;
            }
            set
            {
                round = value;
                OnPropertyChanged();
            }
        }
        public bool Run
        {
            set
            {
                run = value;
                OnPropertyChanged();
            }
            get
            {
                return run;
            }
        }
        public double ScaleFactor
        {
            set
            {
                scaleFactor = value;
                OnPropertyChanged();
            }
            get
            {
                return scaleFactor;
            }
        }
        public double Distancefactor
        {
            set
            {
                distancefactor = value;
                OnPropertyChanged();
            }
            get
            {
                return distancefactor;
            }
        }


        public ObservableCollection<ObjectModel> ColumnDimensions
        {
            get
            {
                return columnDimensions;
            }
            set
            {
                columnDimensions = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> ColumnDimensionsOutput
        {
            get
            {
                return columnDimensionsOutput;
            }
            set
            {
                columnDimensionsOutput = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> ColumnModelOutput
        {
            get
            {
                return columnModelOutput;
            }
            set
            {
                columnModelOutput = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ColumnModel> ColumnModelInternal
        {
            get
            {
                return columnModelInternal;
            }
            set
            {
                columnModelInternal = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Foundation> Foundationelements
        {
            get
            {
                return foundationelements;
            }
            set
            {
                foundationelements = value;
                OnPropertyChanged();
            }
        }

        public string ColumnModel
        {
            get
            {
                return columnModel;
            }
            set
            {
                columnModel = value;
                OnPropertyChanged();
            }
        }
        public Mycommand TagColumnLayer { get; set; }
        public Mycommand Closeprogram { set; get; }
        public Mycommand TagColumnPreDefined { set; get; }
        public Mycommand RunTagColumnPreDefined { get; set; }
        public Mainview v;
        #endregion

        #region Constructor
        public Viewmodel(Mainview v)
        {
            try
            {
                TagColumnLayer = new Mycommand(ExcuteColumnTagLayer, CanExcuteColumnTagLayer);
                TagColumnPreDefined = new Mycommand(ExcuteTagColumnPreDefined, CanExcuteTagColumnPreDefined);
                Closeprogram = new Mycommand(Closepg, CanClosepg);
                RunTagColumnPreDefined = new Mycommand(ExcuteRunTagColumnPreDefined, CanExcuteRunTagColumnPreDefined);
                this.v = v;

                Foundationelements=new ObservableCollection<Foundation>( );
                Foundationelements.Add(new Foundation(20));

            }
            catch (Exception ex)
            {

                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Editor ed = doc.Editor;
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.Message);
            }
        }
        #endregion

        #region Methods
        public void TagColumnWithLayers()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                try
                {
                    //make the filter by layer name and polyline(column)
                    TypedValue[] tv = new TypedValue[2];
                    tv.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE,CIRCLE"), 0);
                    tv.SetValue(new TypedValue((int)DxfCode.LayerName, selectedlayer), 1);
                    SelectionFilter filter = new SelectionFilter(tv);

                    //make the selection of elements
                    PromptSelectionResult psr = editor.GetSelection(filter);
                    SelectionSet selectionset = psr.Value;
                    if (psr.Status == PromptStatus.Cancel)
                    {
                        return;
                    }

                    editor.WriteMessage($"{selectionset.Count} items are selected");


                    //make the list of circle and list of the rectangle
                    List<Entity> rectangle = new List<Entity>();
                    List<Entity> circle = new List<Entity>();
                    if (psr.Status == PromptStatus.OK)
                    {
                        foreach (SelectedObject selectedEntity in selectionset)
                        {
                            var ent = tran.GetObject(selectedEntity.ObjectId, OpenMode.ForRead) as Entity;
                            if (ent.Drawable.GetType().Name == "Polyline")
                            {
                                rectangle.Add(ent);
                            }
                            else
                            {
                                circle.Add(ent);
                            }
                        }
                    }
                    else { return; }

                    BlockTable acBlkTbl = tran.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acBlkTblRec = tran.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    foreach (Entity item in circle)
                    {
                        MText text = new MText();
                        string colmdl1;
                        string coldim2;
                        if (Coldim == true)
                        {
                            coldim2 = "(D=" + System.Math.Round((item as Circle).Diameter, (int)Roundofftxt).ToString() + ")";
                        }
                        else
                        {
                            coldim2 = "";
                        }
                        if (Colmodel == true)
                        {
                            colmdl1 = columnModel;
                        }
                        else { colmdl1 = ""; }
                        text.Contents = $"{colmdl1}\n{coldim2}";
                        double x = ((item as Circle).Center.X + ((item as Circle).Diameter / 2)) + Distancefactor;
                        double y = ((item as Circle).Center.Y + ((item as Circle).Diameter / 2)) + Distancefactor;
                        text.Layer = Selectedtextlayer;
                        if (Annotativecheck == true)
                        {
                            text.Annotative = AnnotativeStates.True;
                        }
                        else
                        {
                            text.Annotative = AnnotativeStates.False;
                        }
                        text.Attachment = AttachmentPoint.BottomCenter;
                        text.Location = new Point3d(x, y, 0);
                        acBlkTblRec.AppendEntity(text);
                        tran.AddNewlyCreatedDBObject(text, true);
                    }
                    foreach (Entity item in rectangle)
                    {
                        MText text = new MText();

                        double area = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Area;
                        double prem = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Length;

                        double a = 0.5 * ((prem / 2) + Math.Sqrt((prem * prem / 4) - 4 * area));
                        double b = 0.5 * ((prem / 2) - Math.Sqrt((prem * prem / 4) - 4 * area));


                        string colmdl1;
                        string coldima;
                        string coldimb;

                        if (Coldim == true)
                        {
                            coldima = "(" + System.Math.Round(b, (int)Roundofftxt).ToString() + " x";
                            coldimb = " " + System.Math.Round(a, (int)Roundofftxt).ToString() + ")";
                        }
                        else
                        {
                            coldima = "";
                            coldimb = "";
                        }
                        if (Colmodel == true)
                        {
                            colmdl1 = columnModel;
                        }
                        else { colmdl1 = ""; }

                        text.Contents = $"{colmdl1}\n{coldima}{coldimb}";
                        text.Layer = Selectedtextlayer;
                        if (Annotativecheck == true)
                        {
                            text.Annotative = AnnotativeStates.True;
                        }
                        else
                        {
                            text.Annotative = AnnotativeStates.False;
                        }
                        text.Attachment = AttachmentPoint.BottomCenter;
                        text.Location = new Point3d((item as Autodesk.AutoCAD.DatabaseServices.Polyline).Bounds.Value.MaxPoint.X + Distancefactor, (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Bounds.Value.MaxPoint.Y + Distancefactor, 0);
                        acBlkTblRec.AppendEntity(text);
                        tran.AddNewlyCreatedDBObject(text, true);
                    }
                    tran.Commit();
                }
                catch (System.Exception ex)
                {
                    editor.WriteMessage("Error : " + ex.Message);
                    tran.Abort();
                }
            }
        }

        public SelectionSet PreDefined()
        {

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable acBlkTbl = tran.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acBlkTblRec = tran.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    //make the filter by layer name and polyline(column)
                    TypedValue[] tv = new TypedValue[2];
                    tv.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE,CIRCLE"), 0);
                    tv.SetValue(new TypedValue((int)DxfCode.LayerName, Selectedlayer), 1);
                    SelectionFilter filter = new SelectionFilter(tv);

                    //make the selection of elements
                    PromptSelectionResult prompt = editor.GetSelection(filter);
                    SelectionSet selectionSet = prompt.Value;
                    if (prompt.Status == PromptStatus.Cancel)
                    {
                        return selectionSet;
                    }
                    editor.WriteMessage($"\n{selectionSet.Count} items are selected\n");

                    //make the list of circle and list of the rectangle
                    List<Entity> rectangle = new List<Entity>();
                    List<Entity> circle = new List<Entity>();
                    if (prompt.Status == PromptStatus.OK)
                    {
                        foreach (SelectedObject selectedEntity in selectionSet)
                        {
                            var ent = tran.GetObject(selectedEntity.ObjectId, OpenMode.ForRead) as Entity;
                            if (ent.Drawable.GetType().Name == "Polyline")
                            {
                                rectangle.Add(ent);
                            }
                            else
                            {
                                circle.Add(ent);
                            }
                        }
                    }
                    else { return selectionSet; }


                    #region Rectangular Column Dimensions

                    //Get a For Column
                    List<double> a = new List<double>();
                    List<double> b = new List<double>();
                    ColumnDimensions = new ObservableCollection<ObjectModel>();
                    double area, prem, ac, bc = 0;
                    foreach (var item in rectangle)
                    {
                        area = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Area;
                        prem = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Length;
                        ac = System.Math.Round(0.5 * ((prem / 2) - Math.Sqrt((prem * prem / 4) - 4 * area)), Roundofftxt);
                        bc = System.Math.Round(0.5 * ((prem / 2) + Math.Sqrt((prem * prem / 4) - 4 * area)), Roundofftxt);
                        a.Add(ac);
                        b.Add(bc);
                        ColumnDimensions.Add(new ObjectModel("", $"{ac} x {bc}", item));
                    }

                    #region Unique Column Cimension List
                    List<string> ColumnDimensionsOutputdistinct = new List<string>();
                    foreach (var item in ColumnDimensions)
                    {
                        ColumnDimensionsOutputdistinct.Add(item.ColumnDimension);
                    }
                    List<string> ColumnDimensionsOutputdis = ColumnDimensionsOutputdistinct.Distinct<string>().ToList();
                    ColumnDimensionsOutput = new ObservableCollection<string>();
                    foreach (var item in ColumnDimensionsOutputdis)
                    {
                        ColumnDimensionsOutput.Add(item);
                    }
                    #endregion

                    #region Unique Column Model List
                    List<string> ColumnModelOutputdistinct = new List<string>();
                    foreach (var item in ColumnDimensions)
                    {
                        ColumnModelOutputdistinct.Add(item.ColumnDimension);
                    }
                    List<string> ColumnModelsOutputdis = ColumnModelOutputdistinct.Distinct<string>().ToList();
                    ColumnModelInternal = new ObservableCollection<ColumnModel>();
                    foreach (var item in ColumnModelsOutputdis)
                    {
                        ColumnModelInternal.Add(new ColumnModel(""));
                    }
                    #endregion

                    #endregion


                    #region Circular Column Dimensions
                    List<double> r = new List<double>();
                    double d = 0;
                    foreach (var item in circle)
                    {
                        area = (item as Autodesk.AutoCAD.DatabaseServices.Circle).Area;
                        d = System.Math.Round(0.5 * (Math.Sqrt(area / Math.PI)), Roundofftxt);
                        ColumnDimensions.Add(new ObjectModel("", $"D={d}", item));

                        #region Unique Column Cimension List
                        List<string> ColumnDimensionsOutputdistinct2 = new List<string>();
                        foreach (var item2 in ColumnDimensions)
                        {
                            ColumnDimensionsOutputdistinct2.Add(item2.ColumnDimension);
                        }
                        List<string> ColumnDimensionsOutputdis2 = ColumnDimensionsOutputdistinct2.Distinct<string>().ToList();
                        ColumnDimensionsOutput = new ObservableCollection<string>();
                        foreach (var item2 in ColumnDimensionsOutputdis2)
                        {
                            ColumnDimensionsOutput.Add(item2);
                        }
                        #endregion

                        #region Unique Column Model List
                        List<string> ColumnModelOutputdistinct2 = new List<string>();
                        foreach (var item2 in ColumnDimensions)
                        {
                            ColumnModelOutputdistinct2.Add(item2.ColumnDimension);
                        }
                        List<string> ColumnModelsOutputdis2 = ColumnModelOutputdistinct2.Distinct<string>().ToList();
                        ColumnModelInternal = new ObservableCollection<ColumnModel>();
                        foreach (var item2 in ColumnModelsOutputdis2)
                        {
                            ColumnModelInternal.Add(new ColumnModel(""));
                        }
                        #endregion
                    }




                    #endregion

                    tran.Commit();
                    return selectionSet;
                }
                catch (System.Exception ex)
                {
                    editor.WriteMessage(ex.Message);
                    tran.Abort();
                }
            }
            SelectionSet ff = null;
            return ff;
        }

        public void RunPreDefined(SelectionSet sel)
        {
            if (Selectedtextlayer == null)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Select Layer for Text");
            }

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            using (Transaction tran = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable acBlkTbl = tran.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acBlkTblRec = tran.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    ObservableCollection<ColumnModel> fndjk = ColumnModelInternal;
                    List<string> ColumnDim = new List<string>();
                    foreach (var item in fndjk)
                    {
                        ColumnDim.Add(item.ColModl);
                    }

                    foreach (var item in ColumnDimensions)
                    {
                        MText text = new MText();
                        #region Rectangle Column

                        if (item.ElementCad.Drawable.GetType().Name == "Polyline")
                        {
                            double area = (item.ElementCad as Autodesk.AutoCAD.DatabaseServices.Polyline).Area;
                            double prem = (item.ElementCad as Autodesk.AutoCAD.DatabaseServices.Polyline).Length;

                            double a = 0.5 * ((prem / 2) + Math.Sqrt((prem * prem / 4) - 4 * area));
                            double b = 0.5 * ((prem / 2) - Math.Sqrt((prem * prem / 4) - 4 * area));

                            string colmdl1 = "";
                            string coldima;
                            string coldimb;

                            if (Coldim == true)
                            {
                                coldima = "(" + System.Math.Round(b, (int)Roundofftxt).ToString() + " x";
                                coldimb = " " + System.Math.Round(a, (int)Roundofftxt).ToString() + ")";
                            }
                            else
                            {
                                coldima = "";
                                coldimb = "";
                            }
                            if (Colmodel == true)
                            {
                                int index = 0;
                                foreach (var item2 in ColumnDimensionsOutput)
                                {
                                    if (item.ColumnDimension == item2)
                                    {

                                        colmdl1 = ColumnDim[index];

                                    }
                                    index++;
                                }
                            }
                            else { colmdl1 = ""; }

                            text.Contents = $"{colmdl1}\n{coldima}{coldimb}";
                            text.Layer = Selectedtextlayer;
                            if (Annotativecheck == true)
                            {
                                text.Annotative = AnnotativeStates.True;
                            }
                            else
                            {
                                text.Annotative = AnnotativeStates.False;
                            }
                            text.Attachment = AttachmentPoint.BottomCenter;
                            text.Location = new Point3d((item.ElementCad as Autodesk.AutoCAD.DatabaseServices.Polyline).Bounds.Value.MaxPoint.X + Distancefactor, (item.ElementCad as Autodesk.AutoCAD.DatabaseServices.Polyline).Bounds.Value.MaxPoint.Y + Distancefactor, 0);
                            acBlkTblRec.AppendEntity(text);
                            tran.AddNewlyCreatedDBObject(text, true);
                        }
                        #endregion


                        //double area = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Area;
                        //double prem = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Length;
                        //double ac = System.Math.Round(0.5 * ((prem / 2) - Math.Sqrt((prem * prem / 4) - 4 * area)), Roundofftxt);
                        //double bc = System.Math.Round(0.5 * ((prem / 2) + Math.Sqrt((prem * prem / 4) - 4 * area)), Roundofftxt);
                        //a.Add(ac);
                        //b.Add(bc);
                        //ColumnDimensions.Add(new RecColumn("", $"{ac} x {bc}", item));




                        #region Circular Column
                        if (item.ElementCad.Drawable.GetType().Name != "Polyline")
                        {
                            string colmdl1 = "";
                            string coldim2;
                            if (Coldim == true)
                            {
                                coldim2 = "(D=" + System.Math.Round((item.ElementCad as Circle).Diameter, (int)Roundofftxt).ToString() + ")";
                            }
                            else
                            {
                                coldim2 = "";
                            }
                            if (Colmodel == true)
                            {
                                int index = 0;
                                foreach (var item2 in ColumnDimensionsOutput)
                                {
                                    if (item.ColumnDimension == item2)
                                    {

                                        colmdl1 = ColumnDim[index];

                                    }
                                    index++;
                                }



                            }
                            else { colmdl1 = ""; }
                            text.Contents = $"{colmdl1}\n{coldim2}";
                            double x = ((item.ElementCad as Circle).Center.X + ((item.ElementCad as Circle).Diameter / 2)) + Distancefactor;
                            double y = ((item.ElementCad as Circle).Center.Y + ((item.ElementCad as Circle).Diameter / 2)) + Distancefactor;
                            text.Layer = Selectedtextlayer;
                            if (Annotativecheck == true)
                            {
                                text.Annotative = AnnotativeStates.True;
                            }
                            else
                            {
                                text.Annotative = AnnotativeStates.False;
                            }
                            text.Attachment = AttachmentPoint.BottomCenter;
                            text.Location = new Point3d(x, y, 0);
                            acBlkTblRec.AppendEntity(text);
                            tran.AddNewlyCreatedDBObject(text, true);
                        }



                        #endregion
                    }


                    #region try

                    //List<Entity> rectangle = new List<Entity>();
                    //List<Entity> circle = new List<Entity>();

                    //foreach (SelectedObject selectedEntity in sel)
                    //{
                    //    var ent = tran.GetObject(selectedEntity.ObjectId, OpenMode.ForRead) as Entity;
                    //    if (ent.Drawable.GetType().Name == "Polyline")
                    //    {
                    //        rectangle.Add(ent);
                    //    }
                    //    else
                    //    {
                    //        circle.Add(ent);
                    //    }
                    //}



                    //#region Circle

                    ////foreach (Entity item in circle)
                    ////{
                    ////    MText text = new MText();
                    ////    string colmdl1;
                    ////    string coldim2;
                    ////    if (Coldim == true)
                    ////    {
                    ////        coldim2 = "(D=" + System.Math.Round((item as Circle).Diameter, (int)Roundofftxt).ToString() + ")";
                    ////    }
                    ////    else
                    ////    {
                    ////        coldim2 = "";
                    ////    }
                    ////    if (Colmodel == true)
                    ////    {
                    ////        colmdl1 = columnModel;
                    ////    }
                    ////    else { colmdl1 = ""; }
                    ////    text.Contents = $"{colmdl1}\n{coldim2}";
                    ////    double x = ((item as Circle).Center.X + ((item as Circle).Diameter / 2)) * 1.05;
                    ////    double y = ((item as Circle).Center.Y + ((item as Circle).Diameter / 2)) * 1.05;
                    ////    text.Layer = Selectedtextlayer;
                    ////    if (Annotativecheck == true)
                    ////    {
                    ////        text.Annotative = AnnotativeStates.True;
                    ////    }
                    ////    else
                    ////    {
                    ////        text.Annotative = AnnotativeStates.False;
                    ////    }
                    ////    text.Attachment = AttachmentPoint.BottomCenter;
                    ////    text.Location = new Point3d(x, y, 0);
                    ////    acBlkTblRec.AppendEntity(text);
                    ////    tran.AddNewlyCreatedDBObject(text, true);
                    ////}
                    //#endregion
                    //foreach (Entity item in rectangle)
                    //{
                    //    MText text = new MText();

                    //    double area = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Area;
                    //    double prem = (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Length;

                    //    double a = 0.5 * ((prem / 2) + Math.Sqrt((prem * prem / 4) - 4 * area));
                    //    double b = 0.5 * ((prem / 2) - Math.Sqrt((prem * prem / 4) - 4 * area));


                    //    string colmdl1;
                    //    string coldima;
                    //    string coldimb;

                    //    if (Coldim == true)
                    //    {
                    //        coldima = "(" + System.Math.Round(b, (int)Roundofftxt).ToString() + " x";
                    //        coldimb = " " + System.Math.Round(a, (int)Roundofftxt).ToString() + ")";
                    //    }
                    //    else
                    //    {
                    //        coldima = "";
                    //        coldimb = "";
                    //    }
                    //    if (Colmodel == true)
                    //    {


                    //        colmdl1 = columnModel;
                    //    }
                    //    else { colmdl1 = ""; }

                    //    text.Contents = $"{colmdl1}\n{coldima}{coldimb}";
                    //    text.Layer = Selectedtextlayer;
                    //    if (Annotativecheck == true)
                    //    {
                    //        text.Annotative = AnnotativeStates.True;
                    //    }
                    //    else
                    //    {
                    //        text.Annotative = AnnotativeStates.False;
                    //    }
                    //    text.Attachment = AttachmentPoint.BottomCenter;
                    //    text.Location = new Point3d((item as Autodesk.AutoCAD.DatabaseServices.Polyline).Bounds.Value.MaxPoint.X * 1.1, (item as Autodesk.AutoCAD.DatabaseServices.Polyline).Bounds.Value.MaxPoint.Y * 1.1, 0);
                    //    acBlkTblRec.AppendEntity(text);
                    //    tran.AddNewlyCreatedDBObject(text, true);
                    //}
                    #endregion
                    tran.Commit();
                }
                catch (System.Exception ex)
                {
                    editor.WriteMessage("Error : " + ex.Message);
                    tran.Abort();
                }
            }
        }

        public void ExcuteColumnTagLayer(object par)
        {
            TagColumnWithLayers();
            v.Close();
        }
        public bool CanExcuteColumnTagLayer(object par)
        {
            return true;
        }

        public void ExcuteTagColumnPreDefined(object par)
        {

            sel = PreDefined();
            Run = true;
        }
        public bool CanExcuteTagColumnPreDefined(object par)
        {
            return true;
        }

        public void ExcuteRunTagColumnPreDefined(object par)
        {
            if (Run == true)
            {

                RunPreDefined(sel);
                v.Close();
            }
            else
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Select The Elements First !");
            }
        }
        public bool CanExcuteRunTagColumnPreDefined(object par)
        {
            return true;
        }


        public void Closepg(object par)
        {
            v.Close();
        }
        public bool CanClosepg(object par)
        {
            return true;
        }

        #endregion
    }
}
