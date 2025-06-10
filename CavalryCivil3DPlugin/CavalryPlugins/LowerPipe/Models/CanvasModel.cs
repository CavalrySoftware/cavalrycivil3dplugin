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
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class CanvasModel
    {
        public ObservableCollection<ShapeViewModel> Shapes { get; }
        private LowerPipeMainViewModel _ViewModel;


        public CanvasModel(LowerPipeMainViewModel _viewModel)
        {
            _ViewModel = _viewModel;
            Shapes = new ObservableCollection<ShapeViewModel>();
        }


        public void Update()
        {
            Shapes.Clear();
            if (_ViewModel.LowerAnalysisModel.ValidRequirements)
            {
                UpdateGraph();
            }
        }


        private void UpdateGraph()
        {

            #region << Setting Properties >>
            List<(double, double)> profilePoints = _ViewModel.LowerAnalysisModel.ProfileData;
            double bottomElevation = _ViewModel.LowerAnalysisModel.LowerElevation;
            double factor = (600 / (profilePoints[5].Item1 - profilePoints[0].Item1)) * 0.9;
            double lateralOffset = _ViewModel.LowerAnalysisModel.LateralOfsset;

            double totalXStart = profilePoints[2].Item1 - profilePoints[1].Item1;
            double totalXEnd = profilePoints[5].Item1 - profilePoints[3].Item1;

            double totalDifferenceX = (totalXEnd - totalXStart) * factor;

            var originX = 350 - (totalDifferenceX / 2);
            var originY = 120;

            
            double p4x = originX + (factor * lateralOffset);
            double p4y = originY;

            double p5x = p4x + (factor * (profilePoints[4].Item1 - profilePoints[3].Item1));
            double p5y = originY - (factor * (profilePoints[4].Item2 - bottomElevation));

            double p6x = p5x + (factor * (profilePoints[5].Item1 - profilePoints[4].Item1));
            double p6y = originY - (factor * (profilePoints[5].Item2 - bottomElevation));

            double p3x = originX - (factor *  lateralOffset);
            double p3y = originY;

            double p2x = p3x - (factor * (profilePoints[2].Item1 - profilePoints[1].Item1));
            double p2y = originY - (factor * (profilePoints[1].Item2 - bottomElevation));

            double p1x = p2x - (factor * ( profilePoints[1].Item1 - profilePoints[0].Item1));
            double p1y = originY - (factor * (profilePoints[0].Item2 - bottomElevation));

            double dimYOffset = 35.0;
            double dimXOffset = 25;

            string horizontalRunStart = $"{profilePoints[2].Item1 - profilePoints[1].Item1 : 0.00}";
            string crossingLength = $"{lateralOffset * 2 : 0.00}";
            string horizontalRunEnd = $"{profilePoints[4].Item1 - profilePoints[3].Item1: 0.00}";
            string verticalOfssetStart = $"{profilePoints[1].Item2 - bottomElevation: 0.00}";
            string verticalOfssetEnd = $"{profilePoints[4].Item2 - bottomElevation: 0.00}";
            string d1 = $"{_ViewModel.LowerAnalysisModel.ActualDeflections[0]: 0.00}°";
            string d2 = $"{_ViewModel.LowerAnalysisModel.ActualDeflections[1]: 0.00}°";
            string d3 = $"{_ViewModel.LowerAnalysisModel.ActualDeflections[2]: 0.00}°";
            string d4 = $"{_ViewModel.LowerAnalysisModel.ActualDeflections[3]: 0.00}°";
            string clearanceStr = $"{_ViewModel.LowerAnalysisModel.VerticalClearance: 0.00}";
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
                Text = crossingLength,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX - 13,
                Y = originY + dimXOffset + 2,
            });


            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = horizontalRunEnd,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + ((p5x - originX) / 2) - 13,
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
                Y = p2y + ((originY - p2y)/2) - 10,
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


            #region << CROSSING PIPE >>
            double dia = _ViewModel.UpperPipe.OuterDiameter * factor;
            double radius = dia / 2;
            double clearance = _ViewModel.LowerAnalysisModel.VerticalClearance * factor;
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Circle,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Cyan,
                Diameter = dia,
                X = originX - radius,
                Y = originY - radius - clearance,
                StrokeThickness = 0.5,
            });
            #endregion


            #region << VERTICAL CROSSING PIPE DIMENSION >>
            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + radius + 5,
                Y1 = originY,
                X2 = originX + radius + 5,
                Y2 = originY - clearance - 4,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Line,
                X1 = originX + 1,
                Y1 = originY - clearance + radius,
                X2 = originX + radius + 10,
                Y2 = originY - clearance + radius,
                StrokeThickness = 0.5,
                Fill = Brushes.LightGreen
            });

            Shapes.Add(new ShapeViewModel
            {
                Type = ShapeType.Text,
                Text = clearanceStr,
                FontSize = 11,
                Fill = Brushes.LightPink,
                X = originX + radius + 5,
                Y = originY - clearance + radius - 15,
            });

            #endregion


        }
    }
}
