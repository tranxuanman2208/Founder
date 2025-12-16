using System.Windows;

namespace MapShortestPath
{
    public partial class DetailWindow : Window
    {
        public DetailWindow(string id)
        {
            InitializeComponent();

            PlaceInfo info = LocationData.Get(id);

            TxtName.Text = info.Name;
            TxtAddress.Text = "Địa chỉ: " + info.Address;
            TxtContent.Text = info.Content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}