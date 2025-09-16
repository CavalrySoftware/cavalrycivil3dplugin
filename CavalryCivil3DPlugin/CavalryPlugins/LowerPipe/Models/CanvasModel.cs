using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CavalryCivil3DPlugin._C3DLibrary.Custom;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class CanvasModel
    {
        public ObservableCollection<ShapeViewModel> Shapes { get; }
        private LowerPipeMainViewModel _ViewModel;


        public CanvasModel(LowerPipeMainModel _mainModel)
        {
            _ViewModel = _mainModel.ViewModel;
            Shapes = new ObservableCollection<ShapeViewModel>();
        }


        public void Update(bool _startOnly = false, bool _endOnly = false)
        {
            try
            {
                Shapes.Clear();
                if (!_startOnly && !_endOnly)
                {
                    UpdateGraph();
                }

                else if (_startOnly)
                {
                    UpdateGraphStartOnly();
                }

                else if (_endOnly)
                {
                    UpdateGraphEndOnly();
                }
            }

            catch (Exception ex) { _Console.ShowConsole(ex.ToString()); }

        }


        public void Erase()
        {
            Shapes.Clear();
        }


        private void UpdateGraph()
        {


            #region << Setting Properties >>
            PipeLowering pipeLoweringObject = _ViewModel.LowerPipeMainModel_.PipeLowering_;
            List<(double, double)> profilePoints = pipeLoweringObject.ProfileDataContainer;

            double bottomElevation1 = profilePoints[2].Item2;
            double bottomElevation2 = profilePoints[3].Item2;
            double factor = (600 / (profilePoints[5].Item1 - profilePoints[0].Item1)) * 0.9;
            double crossLengthStart = pipeLoweringObject.CrossLengthStart;
            double crossLengthEnd = pipeLoweringObject.CrossLengthEnd;

            double refCover= pipeLoweringObject.RefCover;
            double refObjectDepth = pipeLoweringObject.RefDepth;
            double radius= refObjectDepth / 2;
            double clearance = pipeLoweringObject.VerticalClearance;

            double refCoverF = refCover * factor;
            double refObjectDepthF = refObjectDepth * factor;
            double radiusF = radius * factor;
            double clearanceF = clearance * factor;

            double totalXStart = profilePoints[2].Item1 - profilePoints[1].Item1;
            double totalXEnd = profilePoints[5].Item1 - profilePoints[3].Item1;

            double totalDifferenceX = (totalXEnd - totalXStart) * factor;

            var originX = 350 - (totalDifferenceX / 2);
            var originY = 120;

            double p4x = originX + (crossLengthEnd * factor);
            double p4y = originY + (factor * (bottomElevation1 - bottomElevation2));

            double p5x = p4x + (factor * (profilePoints[4].Item1 - profilePoints[3].Item1));
            double p5y = originY - (factor * (profilePoints[4].Item2 - bottomElevation2));
             
            double p6x = p5x + (factor * (profilePoints[5].Item1 - profilePoints[4].Item1));
            double p6y = originY - (factor * (profilePoints[5].Item2 - bottomElevation2));

            double p3x = originX - (crossLengthStart * factor);
            double p3y = originY;

            double p2x = p3x - (factor * (profilePoints[2].Item1 - profilePoints[1].Item1));
            double p2y = originY - (factor * (profilePoints[1].Item2 - bottomElevation1));

            double p1x = p2x - (factor * (profilePoints[1].Item1 - profilePoints[0].Item1));
            double p1y = originY - (factor * (profilePoints[0].Item2 - bottomElevation1));

            double dimYOffset = 35.0;
            double dimXOffset = 25;

            string horizontalRunStart = $"{profilePoints[2].Item1 - profilePoints[1].Item1: 0.00}";
            string crossLengthStart_str = $"{crossLengthStart: 0.00}";
            string crossLengthEnd_str = $"{crossLengthEnd: 0.00}";
            string horizontalRunEnd = $"{profilePoints[4].Item1 - profilePoints[3].Item1: 0.00}";
            string verticalOfssetStart = $"{profilePoints[1].Item2 - bottomElevation1: 0.00}";
            string verticalOfssetEnd = $"{profilePoints[4].Item2 - bottomElevation1: 0.00}";
            string d1 = $"{pipeLoweringObject.ActualDeflections[0]: 0.00}°";
            string d2 = $"{pipeLoweringObject.ActualDeflections[1]: 0.00}°";
            string d3 = $"{pipeLoweringObject.ActualDeflections[2]: 0.00}°";
            string d4 = $"{pipeLoweringObject.ActualDeflections[3]: 0.00}°";
            string clearanceStr = $"{_ViewModel.LowerPipeMainModel_.VerticalClearance: 0.00}";
            string refObjectDepthStr = $"{refObjectDepth: 0.00}";
            string refCoverStr = $"{refCover: 0.00}";
            #endregion




            #region << PROFILE SEGMENTS >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x,
                Y1 = p3y,
                X2 = p4x,
                Y2 = p4y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p4x,
                Y1 = p4y,
                X2 = p5x,
                Y2 = p5y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x,
                Y1 = p5y,
                X2 = p6x,
                Y2 = p6y,
                Fill = Brushes.Yellow
            });

            //Shapes.Add(new ShapeViewModel
            //{
            //    Type = ShapeType.Line,
            //    X1 = originX,
            //    Y1 = originY,
            //    X2 = p3x,
            //    Y2 = p3y,
            //    Fill = Brushes.Yellow
            //});

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x,
                Y1 = p3y,
                X2 = p2x,
                Y2 = p2y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x,
                Y1 = p2y,
                X2 = p1x,
                Y2 = p1y,
                Fill = Brushes.Yellow
            });
            #endregion


            #region << HORIZONTAL DIMENSIONS >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x,
                Y1 = p2y + 5,
                X2 = p2x,
                Y2 = originY + dimYOffset,
                Fill = Brushes.LightGreen,
                StrokeThickness = 0.5
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x,
                Y1 = p3y + 5,
                X2 = p3x,
                Y2 = originY + dimYOffset,
                Fill = Brushes.LightGreen,
                StrokeThickness = 0.5
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,
                Y1 = p4y + 5,
                X2 = originX,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p4x,
                Y1 = p4y + 5,
                X2 = p4x,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x,
                Y1 = p5y + 5,
                X2 = p5x,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            // Horizontal
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x - 10,
                Y1 = originY + dimXOffset,
                X2 = p5x + 10,
                Y2 = originY + dimXOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });
            #endregion


            #region << VERTICAL DIMENSIONS LEFT >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x - 20,
                Y1 = p2y - 5,
                X2 = p2x - 20,
                Y2 = originY + 5,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x - 10,
                Y1 = p2y,
                X2 = p2x - 25,
                Y2 = p2y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x - 10,
                Y1 = p3y,
                X2 = p2x - 25,
                Y2 = p3y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });
            #endregion


            #region << VERTICAL DIMENSIONS RIGHT >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x + 20,
                Y1 = p5y - 5,
                X2 = p5x + 20,
                Y2 = originY + 5,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x + 10,
                Y1 = p5y,
                X2 = p5x + 25,
                Y2 = p5y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p4x + 10,
                Y1 = p4y,
                X2 = p5x + 25,
                Y2 = p4y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });
            #endregion


            #region << DIMENSION VALUES HORIZONTAL >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = horizontalRunStart,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p2x + ((p3x - p2x) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = crossLengthStart_str,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p3x + ((originX - p3x) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = crossLengthEnd_str,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + ((p4x - originX) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = horizontalRunEnd,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p4x + ((p5x - p4x) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });
            #endregion


            #region << DIMENSION VALUES VERTICAL >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = verticalOfssetStart,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p2x - 50,
                Y = p2y + ((originY - p2y) / 2) - 10,
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = verticalOfssetEnd,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p5x + 20,
                Y = p5y + ((originY - p5y) / 2) - 10,
            });
            #endregion


            #region << ANGLE VALUES >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d2,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p3x - 35,
                Y = p3y + 3,
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d3,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p4x + 3,
                Y = p3y + 3,
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d1,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p2x,
                Y = p2y - 15,
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d4,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p5x,
                Y = p5y - 15,
            });
            #endregion


            #region << INTERSECTION >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Circle,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Cyan,
                Diameter = refObjectDepthF,
                X = originX - radiusF,
                Y = originY - radiusF - clearanceF - radiusF,
                StrokeThickness = 0.5,
            });
            #endregion


            #region << INTERSECTION VERTICAL DIMENSION >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + radiusF + 5,
                Y1 = originY,
                X2 = originX + radiusF + 5,
                Y2 = originY - clearanceF - refObjectDepthF - refCoverF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + 1,
                Y1 = originY - clearanceF,
                X2 = originX + radiusF + 10,
                Y2 = originY - clearanceF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + 1,
                Y1 = originY - clearanceF - refObjectDepthF,
                X2 = originX + radiusF + 10,
                Y2 = originY - clearanceF - refObjectDepthF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + 1,
                Y1 = originY - clearanceF - refObjectDepthF - refCoverF,
                X2 = originX + radiusF + 10,
                Y2 = originY - clearanceF - refObjectDepthF - refCoverF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = clearance != 0 ? clearanceStr : "",
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + radiusF + 5,
                Y = originY - (clearanceF / 2) - 5,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = refObjectDepth != 0 ? refObjectDepthStr : "",
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + radiusF + 5,
                Y = originY - clearanceF - radiusF - 7,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = refCover != 0 ? refCoverStr : "",
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + radiusF + 5,
                Y = originY - clearanceF - refObjectDepthF - (refCoverF / 2) - 7,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = "GROUND LEVEL",
                FontSize = 11,
                Fill = Brushes.LightGreen,
                X = originX - 35,
                Y = originY - clearanceF - refObjectDepthF - refCoverF - 15,
            });


            #endregion


        }


        private void UpdateGraphStartOnly()
        {

            #region << Setting Properties >>
            PipeLowering pipeLoweringObject = _ViewModel.LowerPipeMainModel_.PipeLowering_;
            List<(double, double)> profilePoints = pipeLoweringObject.ProfileDataContainer;

            double bottomElevation = profilePoints[3].Item2;
            double factor = (600 / (profilePoints[5].Item1 - profilePoints[2].Item1)) * 0.9;
            double crossLengthStart = pipeLoweringObject.CrossLengthStart;
            double crossLengthEnd = pipeLoweringObject.CrossLengthEnd;

            double refCover = pipeLoweringObject.RefCover;
            double refObjectDepth = pipeLoweringObject.RefDepth;
            double radius = refObjectDepth / 2;
            double clearance = pipeLoweringObject.VerticalClearance;

            double refCoverF = refCover * factor;
            double refObjectDepthF = refObjectDepth * factor;
            double radiusF = radius * factor;
            double clearanceF = clearance * factor;

            double totalXEnd = profilePoints[5].Item1 - profilePoints[3].Item1;

            double totalDifferenceX = totalXEnd* factor;

            var originX = 40;
            var originY = 200;

            double p4x = originX + (crossLengthEnd * factor);
            double p4y = originY;

            double p5x = p4x + (factor * (profilePoints[4].Item1 - profilePoints[3].Item1));
            double p5y = originY - (factor * (profilePoints[4].Item2 - bottomElevation));

            double p6x = p5x + (factor * (profilePoints[5].Item1 - profilePoints[4].Item1));
            double p6y = originY - (factor * (profilePoints[5].Item2 - bottomElevation));

            double p3x = originX;
            double p3y = originY;

            double dimYOffset = 35.0;
            double dimXOffset = 25;

            string crossLengthEnd_str = $"{crossLengthEnd: 0.00}";
            string horizontalRunEnd = $"{profilePoints[4].Item1 - profilePoints[3].Item1: 0.00}";
            string verticalOfssetStart = $"{profilePoints[1].Item2 - bottomElevation: 0.00}";
            string verticalOfssetEnd = $"{profilePoints[4].Item2 - bottomElevation: 0.00}";
            string d1 = $"{pipeLoweringObject.ActualDeflections[0]: 0.00}°";
            string d2 = $"{pipeLoweringObject.ActualDeflections[1]: 0.00}°";
            string d3 = $"{pipeLoweringObject.ActualDeflections[2]: 0.00}°";
            string d4 = $"{pipeLoweringObject.ActualDeflections[3]: 0.00}°";
            string refCoverStr = $"{refCover: 0.00}";
            #endregion




            #region << PROFILE SEGMENTS >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,
                Y1 = originY,
                X2 = p4x,
                Y2 = p4y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p4x,
                Y1 = p4y,
                X2 = p5x,
                Y2 = p5y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x,
                Y1 = p5y,
                X2 = p6x,
                Y2 = p6y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,
                Y1 = originY,
                X2 = p3x,
                Y2 = p3y,
                Fill = Brushes.Yellow
            });

            #endregion


            #region << HORIZONTAL DIMENSIONS >>

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x,
                Y1 = p3y + 5,
                X2 = p3x,
                Y2 = originY + dimYOffset,
                Fill = Brushes.LightGreen,
                StrokeThickness = 0.5
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,
                Y1 = p4y + 5,
                X2 = originX,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p4x,
                Y1 = p4y + 5,
                X2 = p4x,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x,
                Y1 = p5y + 5,
                X2 = p5x,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            // Horizontal
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x - 10,
                Y1 = originY + dimXOffset,
                X2 = p5x + 10,
                Y2 = originY + dimXOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            #endregion


            #region << VERTICAL DIMENSIONS RIGHT >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x + 20,
                Y1 = p5y - 5,
                X2 = p5x + 20,
                Y2 = originY + 5,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p5x + 10,
                Y1 = p5y,
                X2 = p5x + 25,
                Y2 = p5y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p4x + 10,
                Y1 = p4y,
                X2 = p5x + 25,
                Y2 = p4y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });
            #endregion


            #region << DIMENSION VALUES HORIZONTAL >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = crossLengthEnd_str,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + ((p4x - originX) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = horizontalRunEnd,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p4x + ((p5x - p4x) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });
            #endregion


            #region << DIMENSION VALUES VERTICAL >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = verticalOfssetEnd,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p5x + 20,
                Y = p5y + ((originY - p5y) / 2) - 10,
            });
            #endregion


            #region << ANGLE VALUES >>

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d3,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p4x + 3,
                Y = p3y + 3,
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d4,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p5x,
                Y = p5y - 15,
            });
            #endregion


            #region << INTERSECTION >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Circle,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Cyan,
                Diameter = refObjectDepthF,
                X = originX - radiusF,
                Y = originY - radiusF - clearanceF - radiusF,
                StrokeThickness = 0.5,
            });
            #endregion


            #region << INTERSECTION VERTICAL DIMENSION >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,// + radiusF + 5,
                Y1 = originY,
                X2 = originX,// + radiusF + 5,
                Y2 = originY - clearanceF - refObjectDepthF - refCoverF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX - 10,
                Y1 = originY - clearanceF,
                X2 = originX + 10,
                Y2 = originY - clearanceF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX - 10,
                Y1 = originY - clearanceF - refObjectDepthF,
                X2 = originX + 10,
                Y2 = originY - clearanceF - refObjectDepthF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX - 10,
                Y1 = originY - clearanceF - refObjectDepthF - refCoverF,
                X2 = originX + 10,
                Y2 = originY - clearanceF - refObjectDepthF - refCoverF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = "GROUND LEVEL",
                FontSize = 11,
                Fill = Brushes.LightGreen,
                X = originX - 35,
                Y = originY - clearanceF - refObjectDepthF - refCoverF - 15,
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = refCover != 0 ? refCoverStr : "",
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + radiusF + 5,
                Y = originY - clearanceF - refObjectDepthF - (refCoverF / 2) - 7,
            });

            #endregion


        }



        private void UpdateGraphEndOnly()
        {
            #region << Setting Properties >>
            PipeLowering pipeLoweringObject = _ViewModel.LowerPipeMainModel_.PipeLowering_;
            List<(double, double)> profilePoints = pipeLoweringObject.ProfileDataContainer;

            double bottomElevation = profilePoints[2].Item2;
            double factor = (600 / (profilePoints[3].Item1 - profilePoints[0].Item1)) * 0.9;
            double crossLengthStart = pipeLoweringObject.CrossLengthStart;
            double crossLengthEnd = pipeLoweringObject.CrossLengthEnd;

            double refCover = pipeLoweringObject.RefCover;
            double refObjectDepth = pipeLoweringObject.RefDepth;
            double radius = refObjectDepth / 2;
            double clearance = pipeLoweringObject.VerticalClearance;

            double refCoverF = refCover * factor;
            double refObjectDepthF = refObjectDepth * factor;
            double radiusF = radius * factor;
            double clearanceF = clearance * factor;

            double totalXStart = profilePoints[2].Item1 - profilePoints[1].Item1;

            double totalDifferenceX = totalXStart * factor;

            var originX = 600;// - (totalDifferenceX / 2);
            var originY = 160;

            double p4x = originX;
            double p4y = originY;

            double p3x = originX - (crossLengthStart * factor);
            double p3y = originY;

            double p2x = p3x - (factor * (profilePoints[2].Item1 - profilePoints[1].Item1));
            double p2y = originY - (factor * (profilePoints[1].Item2 - bottomElevation));

            double p1x = p2x - (factor * (profilePoints[1].Item1 - profilePoints[0].Item1));
            double p1y = originY - (factor * (profilePoints[0].Item2 - bottomElevation));

            double dimYOffset = 35.0;
            double dimXOffset = 25;

            string horizontalRunStart = $"{profilePoints[2].Item1 - profilePoints[1].Item1: 0.00}";
            string crossLengthStart_str = $"{crossLengthStart: 0.00}";
            string verticalOfssetStart = $"{profilePoints[1].Item2 - bottomElevation: 0.00}";
            string d1 = $"{pipeLoweringObject.ActualDeflections[0]: 0.00}°";
            string d2 = $"{pipeLoweringObject.ActualDeflections[1]: 0.00}°";
            string refObjectDepthStr = $"{refObjectDepth: 0.00}";
            string refCoverStr = $"{refCover: 0.00}";
            #endregion


            #region << PROFILE SEGMENTS >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,
                Y1 = originY,
                X2 = p4x,
                Y2 = p4y,
                Fill = Brushes.Yellow
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,
                Y1 = originY,
                X2 = p3x,
                Y2 = p3y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x,
                Y1 = p3y,
                X2 = p2x,
                Y2 = p2y,
                Fill = Brushes.Yellow
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x,
                Y1 = p2y,
                X2 = p1x,
                Y2 = p1y,
                Fill = Brushes.Yellow
            });
            #endregion


            #region << HORIZONTAL DIMENSIONS >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x,
                Y1 = p2y + 5,
                X2 = p2x,
                Y2 = originY + dimYOffset,
                Fill = Brushes.LightGreen,
                StrokeThickness = 0.5
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x,
                Y1 = p3y + 5,
                X2 = p3x,
                Y2 = originY + dimYOffset,
                Fill = Brushes.LightGreen,
                StrokeThickness = 0.5
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX,
                Y1 = p4y + 5,
                X2 = originX,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p4x,
                Y1 = p4y + 5,
                X2 = p4x,
                Y2 = originY + dimYOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            // Horizontal
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x - 10,
                Y1 = originY + dimXOffset,
                X2 = p4x + 10,
                Y2 = originY + dimXOffset,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });
            #endregion


            #region << VERTICAL DIMENSIONS LEFT >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x - 20,
                Y1 = p2y - 5,
                X2 = p2x - 20,
                Y2 = originY + 5,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p2x - 10,
                Y1 = p2y,
                X2 = p2x - 25,
                Y2 = p2y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = p3x - 10,
                Y1 = p3y,
                X2 = p2x - 25,
                Y2 = p3y,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });
            #endregion


 
            #region << DIMENSION VALUES HORIZONTAL >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = horizontalRunStart,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p2x + ((p3x - p2x) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = crossLengthStart_str,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p3x + ((originX - p3x) / 2) - 13,
                Y = originY + dimXOffset + 2,
            });

            #endregion


            #region << DIMENSION VALUES VERTICAL >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = verticalOfssetStart,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = p2x - 50,
                Y = p2y + ((originY - p2y) / 2) - 10,
            });

            #endregion


            #region << ANGLE VALUES >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d2,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p3x - 35,
                Y = p3y + 3,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = d1,
                FontSize = 11,
                Fill = Brushes.Orange,
                X = p2x,
                Y = p2y - 15,
            });

            #endregion


            #region << INTERSECTION >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Circle,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Cyan,
                Diameter = refObjectDepthF,
                X = originX - radiusF,
                Y = originY - radiusF - clearanceF - radiusF,
                StrokeThickness = 0.5,
            });
            #endregion


            #region << INTERSECTION VERTICAL DIMENSION >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + radiusF + 5,
                Y1 = originY,
                X2 = originX + radiusF + 5,
                Y2 = originY - clearanceF - refObjectDepthF - refCoverF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + 1,
                Y1 = originY - clearanceF,
                X2 = originX + radiusF + 10,
                Y2 = originY - clearanceF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + 1,
                Y1 = originY - clearanceF - refObjectDepthF,
                X2 = originX + radiusF + 10,
                Y2 = originY - clearanceF - refObjectDepthF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + 1,
                Y1 = originY - clearanceF - refObjectDepthF - refCoverF,
                X2 = originX + radiusF + 10,
                Y2 = originY - clearanceF - refObjectDepthF - refCoverF,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = refObjectDepth != 0 ? refObjectDepthStr : "",
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + radiusF + 5,
                Y = originY - clearanceF - radiusF - 7,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = refCover != 0 ? refCoverStr : "",
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + radiusF + 5,
                Y = originY - clearanceF - refObjectDepthF - (refCoverF / 2) - 7,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = "GROUND LEVEL",
                FontSize = 11,
                Fill = Brushes.LightGreen,
                X = originX - 35,
                Y = originY - clearanceF - refObjectDepthF - refCoverF - 15,
            });


            #endregion


        }

    }
}
