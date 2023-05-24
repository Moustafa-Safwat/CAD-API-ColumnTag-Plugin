using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCADAPI.ViewModel;

namespace WPFCADAPI.Model
{
    public class Foundation
    {
        #region Automatic Property
        public Entity cadElement { get; set; }
        public List<string> CadLayer = CadLayers.layernames;
        public double Fdepth { get; set; }
        public double a { get; set; }
        public double b { get; set; }
        public double r { get; set; }
        #endregion

        public Foundation(double F_Depth)
        {
            //cadElement = Cad_Element;
            Fdepth = F_Depth;
        }



    }
}
