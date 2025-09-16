using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models.CADObject;
using CSurface = Autodesk.Civil.DatabaseServices.Surface;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Models
{
    public class SurfaceModelCollection
    {

        private List<SurfaceModel> _SurfaceModels = new List<SurfaceModel>();
        public List<SurfaceModel> SurfaceModels => _SurfaceModels;

        private Document AutocadDocument;
        private CivilDocument C3DDocument;


        public SurfaceModelCollection(Document _autocadDocument, CivilDocument civilDocument)
        {
            try
            {
                AutocadDocument = _autocadDocument;
                C3DDocument = civilDocument;
                GetAllSurfaces();
            }

            catch (System.Exception ex)
            {
                _Console.ShowConsole(ex.ToString());    
            }
            
        }


        private void GetAllSurfaces()
        {
            ObjectIdCollection surfaceIds = C3DDocument.GetSurfaceIds();

            using(Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                foreach(ObjectId surfaceId in surfaceIds)
                {
                    CSurface surface = tr.GetObject(surfaceId, OpenMode.ForRead) as CSurface;

                    SurfaceModel surfaceModel = new SurfaceModel()
                    {
                        Id = surfaceId,
                        Name = surface.Name,
                    };

                    SurfaceModels.Add(surfaceModel);
                }

                SurfaceModel noneSurfaceModel = new SurfaceModel()
                {
                    Id = ObjectId.Null,
                    Name = "None",
                };

                SurfaceModels.Add(noneSurfaceModel);
            }
        }
    }
}
