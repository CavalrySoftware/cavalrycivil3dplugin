using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Windows;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels
{
    public class ZoomPanBehavior : Behavior<FrameworkElement>
    {
        private Point origin;
        private Point start;
        private bool isDragging;

        protected override void OnAttached()
        {
            AssociatedObject.MouseWheel += OnMouseWheel;
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (AssociatedObject.DataContext is LowerPipeMainViewModel vm)
            {
                double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
                vm.Zoom *= zoomFactor;

                var pos = e.GetPosition(AssociatedObject);
                vm.OffsetX = pos.X - zoomFactor * (pos.X - vm.OffsetX);
                vm.OffsetY = pos.Y - zoomFactor * (pos.Y - vm.OffsetY);
            };
            
            return;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.CaptureMouse();
            start = e.GetPosition(null);
            if (AssociatedObject.DataContext is LowerPipeMainViewModel vm)
            {
                origin = new Point(vm.OffsetX, vm.OffsetY);
                isDragging = true;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
            isDragging = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && AssociatedObject.DataContext is LowerPipeMainViewModel vm)
            {
                Vector delta = e.GetPosition(null) - start;
                vm.OffsetX = origin.X + delta.X;
                vm.OffsetY = origin.Y + delta.Y;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseWheel -= OnMouseWheel;
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseMove -= OnMouseMove;
        }
    }
}
