using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Gis.Map.Project;
using Autodesk.Gis.Map;
using CavalryCivil3DPlugin.Consoles;
using Autodesk.Gis.Map.ObjectData;

namespace CavalryCivil3DPlugin.CavalryPlugins.MTO
{
    public class MTOMain
    {
        private StaticConsole Console = new StaticConsole();
        private Document _AutocadDocument = Application.DocumentManager.MdiActiveDocument;
        private CivilDocument _CivilDocument = CivilApplication.ActiveDocument;
        private static ProjectModel _ProjectModel = HostMapApplicationServices.Application.ActiveProject;

        private PipelineData _PipelineData;
        public PipelineData PipelineData => _PipelineData;

        private FeatureData _FeatureData;
        public FeatureData FeatureData => _FeatureData;

        private CorridorData _CorridorData;
        public CorridorData CorridorData => _CorridorData;


        public MTOMain()
        {
            InitializePipelineData();
            InitializeFeautreData();
            InitializeCorridorData();
        }


        public void InitializePipelineData()
        {
            Console.Print("Pipeline Entry...");
            List<string> pressureNetworks = new List<string>() { "SantosGas", "SantosWater" };
            string payItemPath = @"C:\Users\Harvey\Desktop\Cavalry\04. PLUGIN DEVELOPMENT\Notes\SANTOS PAY ITEM.csv";
            string layer = "SANTOS-TRENCHES";
            string propertySetName = "PIPE GIS DATA";

            _PipelineData = new PipelineData( 
                _layerName: layer, 
                _autocadDocument: _AutocadDocument, 
                _civilDocument: _CivilDocument, 
                _pipeNetworkNames: pressureNetworks,
                _propertySetName: propertySetName, 
                _payItemPath: payItemPath);

            Console.Print(_PipelineData.PipeRunDataCollection.Count);

            List<string> types = _PipelineData.PipeRunDataCollection.Select(x => x.Network).ToList();
            Console.Print(types);
        }


        public void InitializeFeautreData()
        {
            Console.Print("Feature Entry..");

            List<string> layerNames = new List<string>()
            {
                "SANTOS-APPT",
                "SANTOS-FITTING"
            };

            string propertySetName = "SANTOS_ENG_DATA";
            string payItemPath = @"C:\Users\Harvey\Desktop\Cavalry\04. PLUGIN DEVELOPMENT\Notes\SANTOS PAY ITEM.csv";


            _FeatureData = new FeatureData(
                _layerNames: layerNames,
                _autocadDocument: _AutocadDocument,
                _civilDocument: _CivilDocument,
                _projectModel: _ProjectModel,
                _propertySetName: propertySetName,
                _payItemPath: payItemPath
                );

            Console.Print(_FeatureData.ComponentDataCollection.Count);
        }


        public void InitializeCorridorData()
        {
            Console.Print("Corridor Entry...");

            string layerName = "SANTOS-TRENCHES";
            string propertySetName = "CORRIDOR DETAILS";

            _CorridorData = new CorridorData(
                _layerName: layerName,
                _autocadDocument: _AutocadDocument,
                _civilDocument: _CivilDocument,
                _propertySetName: propertySetName
                );

            Console.Print(_CorridorData.AlignmentDataCollection.Count); 
        }
    }
}
