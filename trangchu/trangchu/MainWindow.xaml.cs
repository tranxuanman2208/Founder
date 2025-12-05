using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using trangchu.Views;
using WpfMapDijkstra.Views;

namespace trangchu
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowPage(btnHome);   
        }
        private void LoadHome()
        {
            ContentArea.Content = new HomePage();
        }

        private void LoadBasic()
        {
            ContentArea.Content = new BasicPage();
        }

        private void LoadFunc1()
        {
            ContentArea.Content = new Func1Page();
        }

        private void LoadFunc2()
        {
            ContentArea.Content = new Func2Page();
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            ShowPage(clicked);
        }

        private void ShowPage(Button btn)
        {
            ResetButtons();

            btn.Background = new SolidColorBrush(Color.FromRgb(33, 150, 243));
            btn.Foreground = Brushes.White;

            if (btn == btnHome)
                LoadHome();
            else if (btn == btnBasic)
                LoadBasic();
            else if (btn == btnFunc1)
                LoadFunc1();
            else if (btn == btnFunc2)
                LoadFunc2();
        }

        private void ResetButtons()
        {
            Brush normal = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF505050"));

            btnHome.Background = normal;
            btnBasic.Background = normal;
            btnFunc1.Background = normal;
            btnFunc2.Background = normal;

            btnHome.Foreground = Brushes.White;
            btnBasic.Foreground = Brushes.White;
            btnFunc1.Foreground = Brushes.White;
            btnFunc2.Foreground = Brushes.White;
        }
    }
}
