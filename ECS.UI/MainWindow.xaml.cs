using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ECS.Application;
using INNO6.IO;
using INNO6.Core.Manager;
using System.Windows.Media.Animation;
using INNO6.Core;

namespace ECS.UI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        string _AlarmImageSource = "Resources/Images/alert.png";
        string _SystemImageSource = "Resources/Images/keti.png";
        string _EmergencyStopImageSource = "Resources/Images/emergency_stop.png";
        string _LaserCautionImageSource = "Resources/Images/laser_radiation.png";

        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.Dll")]
        static extern int PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);
        private const UInt32 WM_CLOSE = 0x0010;

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.CurrentAlarmButton.Content = MakeImageStoryboard(_AlarmImageSource, 60, 60, 80, 80);
            this.SystemMainButton.Content = MakeImageStoryboard(_SystemImageSource, 220, 140, 250, 150);
            this.EMOButton.Content = MakeImageStoryboard(_EmergencyStopImageSource, 60, 60, 80, 80);
            this.CautionLaserOn.Content = MakeImageStoryboard(_LaserCautionImageSource, 200, 70, 220, 90);
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            
            //_engine.Stop();
        }

        private Image MakeImageStoryboard(string image_source, double from_width, double from_height, double to_width, double to_height)
        {
            Storyboard MenuReset = new Storyboard();

            DoubleAnimation withAnimation = new DoubleAnimation(from_width, new Duration(TimeSpan.Parse("0:0:0.15")));
            DoubleAnimation heightAnimation = new DoubleAnimation(from_height, new Duration(TimeSpan.Parse("0:0:0.15")));

            Storyboard.SetTargetProperty(withAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));

            MenuReset.Children.Add(withAnimation);
            MenuReset.Children.Add(heightAnimation);

            Storyboard ClickMenu = new Storyboard();

            DoubleAnimation _withAnimation = new DoubleAnimation(to_width, new Duration(TimeSpan.Parse("0:0:0.15")));
            DoubleAnimation _heightAnimation = new DoubleAnimation(to_height, new Duration(TimeSpan.Parse("0:0:0.15")));

            Storyboard.SetTargetProperty(_withAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(_heightAnimation, new PropertyPath(HeightProperty));

            ClickMenu.Children.Add(_withAnimation);
            ClickMenu.Children.Add(_heightAnimation);

            BeginStoryboard eventStoryboard1 = new BeginStoryboard();
            BeginStoryboard eventStoryboard2 = new BeginStoryboard();

            eventStoryboard1.Storyboard = ClickMenu;
            eventStoryboard2.Storyboard = MenuReset;

            // 이벤트 트리거
            EventTrigger eventTrigger = new EventTrigger(MouseEnterEvent);
            eventTrigger.Actions.Add(eventStoryboard1);

            EventTrigger eventTrigger2 = new EventTrigger(MouseLeaveEvent);
            eventTrigger2.Actions.Add(eventStoryboard2);


            Uri uri = GetResourceUri(null, image_source);

            Image image = new Image
            {
                Source = new BitmapImage(uri),
                Style = (Style)this.Resources["imageStyle"], // 스타일 적용
                Stretch = Stretch.Uniform,
                Width = from_width,
                Height = from_height

            };

            // 이벤트 트리거 설정
            image.Triggers.Add(eventTrigger);
            image.Triggers.Add(eventTrigger2);


            return image;
        }


        private void AlarmPanelStoryboard()
        {
            Storyboard MenuReset = new Storyboard();

            DoubleAnimation withAnimation = new DoubleAnimation(70, new Duration(TimeSpan.Parse("0:0:0.15")));
            DoubleAnimation heightAnimation = new DoubleAnimation(70, new Duration(TimeSpan.Parse("0:0:0.15")));

            Storyboard.SetTargetProperty(withAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));

            MenuReset.Children.Add(withAnimation);
            MenuReset.Children.Add(heightAnimation);

            Storyboard ClickMenu = new Storyboard();

            DoubleAnimation _withAnimation = new DoubleAnimation(80, new Duration(TimeSpan.Parse("0:0:0.15")));
            DoubleAnimation _heightAnimation = new DoubleAnimation(80, new Duration(TimeSpan.Parse("0:0:0.15")));

            Storyboard.SetTargetProperty(_withAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(_heightAnimation, new PropertyPath(HeightProperty));

            ClickMenu.Children.Add(_withAnimation);
            ClickMenu.Children.Add(_heightAnimation);

            BeginStoryboard eventStoryboard1 = new BeginStoryboard();
            BeginStoryboard eventStoryboard2 = new BeginStoryboard();

            eventStoryboard1.Storyboard = ClickMenu;
            eventStoryboard2.Storyboard = MenuReset;

            // 이벤트 트리거
            EventTrigger eventTrigger = new EventTrigger(MouseEnterEvent);
            eventTrigger.Actions.Add(eventStoryboard1);

            EventTrigger eventTrigger2 = new EventTrigger(MouseLeaveEvent);
            eventTrigger2.Actions.Add(eventStoryboard2);


            Uri uri = GetResourceUri(null, _AlarmImageSource);

            Image image = new Image
            {
                Source = new BitmapImage(uri),
                Style = (Style)this.Resources["imageStyle"], // 스타일 적용
                Stretch = Stretch.Fill
            };

            // 이벤트 트리거 설정
            image.Triggers.Add(eventTrigger);
            image.Triggers.Add(eventTrigger2);

            this.CurrentAlarmButton.Content = image;

        }

        private Uri GetResourceUri(string assemblyName , string resourcePath)
        {
            if(string.IsNullOrEmpty(assemblyName))
            {
                return new Uri(string.Format("pack://application:,,,/{0}", resourcePath));
            }
            else
            {
                return new Uri(string.Format("pack://application:,,,/{0};component/{1}", assemblyName, resourcePath));
            }
        }

    }
}
