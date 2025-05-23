using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Autodesk.Aec.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using CavalryCivil3DPlugin.ACADLibrary.Annotation;
using CavalryCivil3DPlugin.ACADLibrary.AutoCADTable;
using CavalryCivil3DPlugin.ACADLibrary.Elements;
using CavalryCivil3DPlugin.C3DLibrary.Elements;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models.CADObject;

using CADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands
{
    public class ExtractCoordinatesOk
    {
        private ExtractCoordinatesViewModel ViewModel_;

        private List<string> headersBasic = new List<string>() { "REF", "EASTINGS", "NORTHINGS" };
        private List<string> headersElevation = new List<string>() { "REF", "EASTINGS", "NORTHINGS", "ELEVATIONS" };
        private List<string> headers;
        private string title { get { return ViewModel_.TableName; } }
        private List<string> _PrefixList { get { return ViewModel_.PrefixList; } }
        private string _PointPrefix { get { return ViewModel_.Prefix_; } }
        private string _TableStyleName { get { return ViewModel_.SelectedTableStyle; } }

        
        public ExtractCoordinatesOk(ExtractCoordinatesViewModel viewModel)
        {
            ViewModel_ = viewModel;
            
        }

        public List<List<Point3d>> GetCoordinatesFromSelectedObjects()
        {
            try
            {
                var selectedObjects = ViewModel_.SelectedEntities;

                //selectedObjects = selectedObjects.OrderBy(x => x.Name).ToList();

                List<List<Point3d>> coordinates = new List<List<Point3d>>();

                foreach (var _object in selectedObjects)
                {
                    coordinates.Add(_object.GetAllVertices());
                }

                return coordinates;
            }
            
            catch (Exception ex) { _Console.ShowConsole(ex.ToString()); return new List<List<Point3d>>(); }
        }


        public void CreateCoordinatesTableExecute()
        {
            var allCoordinates = GetCoordinatesFromSelectedObjects();
            int totalPolylines = allCoordinates.Count;
            headers = ViewModel_.IncludeElevation ? headersElevation : headersBasic;

            ViewModel_.HideAction.Invoke();
            if (ViewModel_.IsCombined)
            {
                var combinedCooridnates = new List<Point3d>();

                foreach (List<Point3d> plinePoints in allCoordinates)
                {
                    combinedCooridnates.AddRange(plinePoints);
                }

                CreateTable(combinedCooridnates, _TableStyleName, headers, title, _PointPrefix, ViewModel_.IncludeElevation);
            }

            else
            {
                try
                {
                    int index = 0;

                    foreach (var plineCoordinates in allCoordinates)
                    {
                        string prefix = _PrefixList.Count == totalPolylines ? _PrefixList[index] : _PointPrefix;
                        bool created = CreateTable(plineCoordinates, _TableStyleName, headers, title, prefix, ViewModel_.IncludeElevation);

                        if (!created) break;

                        if(ViewModel_.WillCreatePointGroups)
                        {
                            string groupName = string.IsNullOrEmpty(ViewModel_.PointGroupsName) ? prefix : ViewModel_.PointGroupsName;
                            CreatePointGroup(plineCoordinates, prefix, groupName);
                        }

                        index++;
                    }
                }
                
                catch (Exception ex) {_Console.ShowConsole(ex.ToString());} 
            }
            
            ViewModel_.CloseAction.Invoke();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_objectIds"></param>
        public void CreateCoordinatesTableFromPickedElements(List<CADObjectId> _objectIds=null)
        {
            headers = ViewModel_.IncludeElevation ? headersElevation : headersBasic;
            List<Point3d> coordinates;
            

            if (_objectIds != null)
            {
                int index = 0;
                foreach (CADObjectId objectId in _objectIds)
                {

                    coordinates = GetCoordinatesFromId(objectId);

                    string prefix = _PrefixList.Count == coordinates.Count ? _PrefixList[index] : _PointPrefix;

                    bool created =  CreateTable(coordinates, _TableStyleName, headers, title, prefix, ViewModel_.IncludeElevation);
                    if (!created) break;

                    if (ViewModel_.WillCreatePointGroups)
                    {
                        string groupName = string.IsNullOrEmpty(ViewModel_.PointGroupsName) ? prefix : ViewModel_.PointGroupsName;
                        CreatePointGroup(coordinates, prefix, groupName);
                    }

                    index ++;
                }
            }
        }


        public List<Point3d> GetCoordinatesFromId(CADObjectId _objectId)
        {
            if (ViewModel_.SelectedObjectType.ObjectName == "Polyline")
            {
                return _Polyline.GetAllVertices(ViewModel_.AutocadDocument, _objectId);
            }
            else
            {
                return _FeatureLines.GetPIPoints(ViewModel_.AutocadDocument, _objectId);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="_polylineCoordinates"></param>
        /// <param name="_prefix"></param>
        /// <param name="_includeElevation"></param>
        /// <returns></returns>
        private List<List<string>> GetSortedCoordinates(List<Point3d> _polylineCoordinates, string _prefix, bool _includeElevation)
        {
            List<List<string>> sortedCoordinates = new List<List<string>>();

            int index = 1;
            foreach(Point3d point in _polylineCoordinates)
            {
                List<string> row = new List<string>();

                string prefix = $"{_prefix}{index}";
                string x = Math.Round((double)point.X, 3).ToString("F3");
                string y = Math.Round((double)point.Y, 3).ToString("F3");
                string z = Math.Round((double)point.Z, 3).ToString("F3");

                row.Add(prefix);
                row.Add(x);
                row.Add(y);

                if (_includeElevation ) row.Add(z);

                sortedCoordinates.Add(row);
                index++;    
            }

            return sortedCoordinates;
        }


        private (Point3d _insertionPoint, bool _valid) GetInsertionPoint()
        {
            PromptPointOptions prompt = new PromptPointOptions("\nSelect insertion point: ");
            PromptPointResult promptResult = ViewModel_._Editor.GetPoint(prompt);

            Point3d insertPoint;
            bool valid = false; 

            if (promptResult.Status == PromptStatus.OK)
            {
                insertPoint = promptResult.Value;
                valid = true;
            }
            else
            {
                insertPoint = new Point3d(0, 0, 0);
            }

            return (insertPoint, valid);
        }


        private bool CreateTable(List<Point3d> points, string _tableStyle, List<string> _headers, string _title, string _prefix, bool _includeElevation)
        {
            (Point3d insertionPoint, bool validPoint) = GetInsertionPoint();
            if (validPoint)
            {
                List<List<string>> sortedData = GetSortedCoordinates(points, _prefix, _includeElevation);
                CADTable.CreateTable(ViewModel_.AutocadDocument, _data: sortedData, _tableStyle: _tableStyle, _headers: _headers, _title: _title, _insertionPoint: insertionPoint);

                if (ViewModel_.IsAnnotate)
                {
                    CADText.AnnotateAtPoints
                        (
                        _points: points, 
                        _prefix: _prefix, 
                        _autocadDocument: ViewModel_.AutocadDocument, 
                        _xOffset: ViewModel_.xOffset,
                        _yOffset: ViewModel_.yOffset
                        );
                }

                return true;    
            }

            else { return false; }
        }


        private void CreatePointGroup(List<Point3d> _points, string _prefix, string _groupName)
        {
            _Points.CreatePointGroup(
                _autocadDocument: ViewModel_.AutocadDocument,
                _civilDocument: ViewModel_.Civil3DDocument,
                _points: _points,
                _prefix: _prefix,
                _groupName: _groupName
                );
        }
    }
}
