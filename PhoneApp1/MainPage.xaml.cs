using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Linq.Expressions;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LLS.DataContext = new List<Item>(new[] { "1", "2", "3", "4", "5", "6", "7", "8" }.Select(z => new Item() { Value = z }));
        }

        private void Grid_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var ct = (sender as Grid).RenderTransform as CompositeTransform;
            var sum = ct.TranslateX + e.DeltaManipulation.Translation.X;
            if (sum < -200)
                ct.TranslateX = -200;
            else if (sum > 0)
                ct.TranslateX = 0;
            else
                ct.TranslateX = sum;

            var bo = VisualTreeHelper.GetChild(VisualTreeHelper.GetParent(sender as Grid), 0) as Border;
            bo.Opacity = sum / -200;
        }

        private void Grid_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            
        }

        private void Grid_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            var senderContainer = sender as Grid;
            var bo = VisualTreeHelper.GetChild(VisualTreeHelper.GetParent(senderContainer), 0) as Border;
            
            Storyboard sb = null;
            var item = senderContainer.DataContext as Item;

            if (e.FinalVelocities.LinearVelocity.X > 650)
            {
                if (e.TotalManipulation.Translation.X < -20)
                    item.IsShow = true;
                else if (e.TotalManipulation.Translation.X > 20)
                    item.IsShow = false;
            }
            else if (e.TotalManipulation.Translation.X < -70)
                item.IsShow = true;
            else if (e.TotalManipulation.Translation.X >  70)
                item.IsShow = false;

            sb = MakeButtonStoryboard(item.IsShow);

            if (sb != null)
            {
                sb.Stop();

                foreach (var tl in sb.Children.OfType<DoubleAnimation>())
                {
                    if (Storyboard.GetTargetProperty(tl).Path.Contains("Opacity"))
                        Storyboard.SetTarget(tl, bo);
                    else
                        Storyboard.SetTarget(tl, senderContainer);
                }

                foreach (var tl in sb.Children.OfType<ObjectAnimationUsingKeyFrames>())
                {
                    Storyboard.SetTarget(tl, bo);
                }

                sb.Begin();
            }
        }

        Storyboard MakeButtonStoryboard(bool isShowOrHide)
        {
            var time = TimeSpan.FromSeconds(0.2);
            
            var a1 = new DoubleAnimation()
            {
                Duration = new Duration(time),
                To = isShowOrHide ? 1 : 0
            };
            Storyboard.SetTargetProperty(a1, new PropertyPath("(UIElement.Opacity)"));
            var a2 = new DoubleAnimation()
            {
                Duration = new Duration(time),
                To = isShowOrHide ? - 200 : 0
            };
            Storyboard.SetTargetProperty(a2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            var a3 = new ObjectAnimationUsingKeyFrames();
            a3.KeyFrames.Add(new DiscreteObjectKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(time),
                    Value = isShowOrHide
                });
            Storyboard.SetTargetProperty(a3, new PropertyPath("(UIElement.IsHitTestVisible)"));

            var sb = new Storyboard();
            sb.Children.Add(a1);
            sb.Children.Add(a2);
            sb.Children.Add(a3);

            return sb;
        }
    }



    public class Item
    {
        public string Value { get; set; }

        public bool IsShow { get; set; }
    }
}