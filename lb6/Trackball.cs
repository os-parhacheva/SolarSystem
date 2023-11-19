using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows;

namespace lb6
{
    public class Trackball
    {
        private FrameworkElement _eventSource;
        private Point _previousPosition2D;
        private Vector3D _previousPosition3D = new Vector3D(0, 0, 1);
        private Transform3DGroup _transform;
        private ScaleTransform3D _scale = new ScaleTransform3D(1, 1, 1);
        private AxisAngleRotation3D _rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);

        public Trackball()
        {
            _transform = new Transform3DGroup();
            _transform.Children.Add(_scale);
            _transform.Children.Add(new RotateTransform3D(_rotation));
        }

        public Transform3D Transform
        {
            get { return _transform; }
        }

        public FrameworkElement EventSource
        {
            get { return _eventSource; }
            set
            {
                if (_eventSource != null)
                {
                    _eventSource.MouseDown -= this.OnMouseDown;
                    _eventSource.MouseUp -= this.OnMouseUp;
                    _eventSource.MouseMove -= this.OnMouseMove;
                    _eventSource.MouseWheel -= this.OnMouseWheel;
                }
                _eventSource = value;
                _eventSource.MouseDown += this.OnMouseDown;
                _eventSource.MouseUp += this.OnMouseUp;
                _eventSource.MouseMove += this.OnMouseMove;
                _eventSource.MouseWheel += this.OnMouseWheel;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scale = e.Delta / 1000.0;
            _scale.ScaleX += scale;
            _scale.ScaleY += scale;
            _scale.ScaleZ += scale;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Mouse.Capture(EventSource, CaptureMode.Element);
            _previousPosition2D = e.GetPosition(EventSource);
            _previousPosition3D = ProjectToTrackball(EventSource.ActualWidth, EventSource.ActualHeight, _previousPosition2D);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(EventSource, CaptureMode.None);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(EventSource);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Track(currentPosition);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Zoom(currentPosition);
            }
            _previousPosition2D = currentPosition;
        }

        public void Track(Point currentPosition)
        {
            try
            {
                Vector3D currentPosition3D = ProjectToTrackball(
                        EventSource.ActualWidth,
                        EventSource.ActualHeight,
                        currentPosition
                    );
                Vector3D axis = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D);
                double angle = Vector3D.AngleBetween(_previousPosition3D, currentPosition3D);
                Quaternion delta = new Quaternion(axis, -angle);
                AxisAngleRotation3D r = _rotation;
                Quaternion q = new Quaternion(_rotation.Axis, _rotation.Angle);
                q *= delta;
                _rotation.Axis = q.Axis;
                _rotation.Angle = q.Angle;
                _previousPosition3D = currentPosition3D;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double x = point.X / (width / 2);
            double y = point.Y / (height / 2);
            x = x - 1;
            y = 1 - y;
            double z2 = 1 - x * x - y * y;
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;
            return new Vector3D(x, y, z);
        }

        private void Zoom(Point currentPosition)
        {
            double yDelta = currentPosition.Y - _previousPosition2D.Y;
            double scale = Math.Exp(yDelta / 100);
            _scale.ScaleX *= scale;
            _scale.ScaleY *= scale;
            _scale.ScaleZ *= scale;
        }
    }
}
