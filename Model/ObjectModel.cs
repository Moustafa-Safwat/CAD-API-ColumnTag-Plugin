using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCADAPI.Model
{
    public class ObjectModel
    {
        #region Automatic Property
        public string ColumnModel { set; get; }
        public string ColumnDimension { set; get; }
        public Entity ElementCad { get; set; }
        #endregion
        public ObjectModel(string columnModel, string columnDimension,Entity elementCad)
        {
            ColumnModel = columnModel;
            ColumnDimension = columnDimension;
            ElementCad = elementCad;
        }
       
    }
}
