using RemoteDeviceControllerForWindows.FileAccessServiceReference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RemoteDeviceControllerForWindows
{
    /// <summary>
    /// Interaction logic for ImageViewerWindow.xaml
    /// </summary>
    public partial class ImageViewerWindow : Window
    {

        private string _currentDirectory;
        private List<string> _images;
        private int _currentImageIndex = -1;
        private bool _mouseDown;
        private Point _mouseXY;
        private FileAccessServiceClient _client;

        public int CurrentImageIndex
        {
            get
            {
                return _currentImageIndex;
            }

            private set
            {
                if (value >= _images.Count && _images.Count > 1)
                {
                    value = 0;
                }
                if (value < 0 && _images.Count > 1)
                {
                    value = _images.Count - 1;
                }
                if (_currentImageIndex != value)
                {
                    _currentImageIndex = value;
                    SetImage(_client.GetFile(_currentDirectory + "\\" + _images[_currentImageIndex]));
                }
            }
        }

        public ImageViewerWindow(string currentDirectory, List<string> images, int index)
        {
            InitializeComponent();
            if (index < 0)
            {
                Close();
            }
            _client = ClientCreator.fileAccessClient;
            _currentDirectory = currentDirectory;
            _images = images;
            CurrentImageIndex = index;
        }

        public ImageViewerWindow(string currentDirectory, List<string> images, string imageName)
            : this(currentDirectory, images, images.FindIndex(n => n.Equals(imageName)))
        {
        }


        private void SetImage(byte[] bs)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(bs);
            bitmap.EndInit();
            image.Source = bitmap;
        }



        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentImageIndex += 1;
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentImageIndex -= 1;
        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            var group = image.FindResource("Imageview") as TransformGroup;
            Point point = image.TransformToAncestor(window).Transform(new Point(0, 0));
            double delta = -0.1;
            ChangeScale(group, new Point(window.Width / 2, window.Height / 2), delta);
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            var group = image.FindResource("Imageview") as TransformGroup;
            Point point = image.TransformToAncestor(window).Transform(new Point(0, 0));
            double delta = 0.1;
            ChangeScale(group, new Point(window.Width / 2, window.Height / 2), delta);
        }

        private void contentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cc = sender as ContentControl;
            if (cc == null)
            {
                return;
            }
            cc.CaptureMouse();
            _mouseDown = true;
            _mouseXY = e.GetPosition(cc);
        }

        private void contentControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cc = sender as ContentControl;
            if (cc == null)
            {
                return;
            }
            cc.ReleaseMouseCapture();
            _mouseDown = false;
        }

        private void contentControl_MouseMove(object sender, MouseEventArgs e)
        {
            var cc = sender as ContentControl;
            if (cc == null)
            {
                return;
            }
            if (_mouseDown)
            {
                MoveMouse(cc, e);
            }
        }

        private void MoveMouse(ContentControl img, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var group = image.FindResource("Imageview") as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            var position = e.GetPosition(img);
            transform.X -= _mouseXY.X - position.X;
            transform.Y -= _mouseXY.Y - position.Y;
            _mouseXY = position;
        }

        private void contentControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var cc = sender as ContentControl;
            if (cc == null)
            {
                return;
            }
            var point = e.GetPosition(cc);
            var group = mainGrid.FindResource("Imageview") as TransformGroup;
            var delta = e.Delta * 0.001;
            ChangeScale(group, point, delta);
        }

        private void ChangeScale(TransformGroup group, Point point, double delta)
        {
            var pointToContent = group.Inverse.Transform(point);
            var transform = group.Children[0] as ScaleTransform;
            if (transform.ScaleX + delta < 0.1) return;
            transform.ScaleX += delta;
            transform.ScaleY += delta;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);
        }
    }
}
