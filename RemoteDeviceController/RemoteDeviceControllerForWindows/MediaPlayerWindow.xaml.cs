using System;
using System.Collections.Generic;
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
using RemoteDeviceController.Util;
using System.Windows.Threading;

namespace RemoteDeviceControllerForWindows
{
    public partial class MediaPlayerWindow : Window
    {
        //要在别的线程对UI元素进行操作，只能用这种方式开线程
        private DispatcherTimer _timer = new DispatcherTimer();

        public MediaPlayerWindow()
        {
            InitializeComponent();
            //不要把MediaOpend放在Loaded里，那时视频的一些属性还未初始化，会出错
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaFailed += MediaElement_MediaFailed;
            //如果设置autoplay的话，那play(),pause()之类的方法就没法用了，报exception
            mediaElement.LoadedBehavior = MediaState.Manual;
            startButton.Visibility = Visibility.Visible;
            pauseButton.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// 播放指定路径下的视频文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SetMedia(string filePath)
        {
            filePath = TextEncryptor.Encrypt_Hex(filePath);
            //通过HTTP协议从服务器获取视频流来边下边放
            //不是所有格式都可以这样，但至少H264可以
            mediaElement.Source = new Uri(
                string.Format("http://{0}:{1}/?filepath={2}",
                    ClientCreator.currentServerIP,
                    8444,
                    filePath));
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            progressSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            //启动一个线程，每一秒改变一次进度条和显示的时间
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(ticktock);
            _timer.Start();
        }
        
        private void ticktock(object sender, EventArgs e)
        {
            //如果此时并没有用鼠标拖着进度条
            if (!progressSlider.IsMouseCaptureWithin)
            {
                progressSlider.Value = mediaElement.Position.TotalSeconds;
                if (mediaElement.NaturalDuration.TimeSpan.TotalHours < 1)
                {
                    // 12:03/14:02
                    positionTextBlock.Text = string.Format(
                        "{0}:{1:D2}/{2}:{3:D2}",
                        mediaElement.Position.Minutes,
                        mediaElement.Position.Seconds,
                        mediaElement.NaturalDuration.TimeSpan.Minutes,
                        mediaElement.NaturalDuration.TimeSpan.Seconds);
                }
                else
                {
                    // 1:23:04/1:40:00
                    positionTextBlock.Text = string.Format(
                        "{0}:{1}:{2:D2}/{3}:{4}:{5:D2}",
                        mediaElement.Position.Hours,
                        mediaElement.Position.Minutes,
                        mediaElement.Position.Seconds,
                        mediaElement.NaturalDuration.TimeSpan.Hours,
                        mediaElement.NaturalDuration.TimeSpan.Minutes,
                        mediaElement.NaturalDuration.TimeSpan.Seconds);
                }
            }
        }

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
            startButton.Visibility = Visibility.Hidden;
            pauseButton.Visibility = Visibility.Visible;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
            startButton.Visibility = Visibility.Visible;
            pauseButton.Visibility = Visibility.Hidden;
        }
        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //如果正用鼠标拖着进度条
            if (progressSlider.IsMouseCaptureWithin)
            {
                int pos = Convert.ToInt32(progressSlider.Value);
                mediaElement.Position = new TimeSpan(0, 0, 0, pos, 0);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _timer.Stop();
        }
    }
}
