using System;
using System.Windows.Media.Imaging;

namespace WPFClient.ViewModel.Players
{
    public class GameMatchViewModel
    {
        public GameMatchViewModel(string? url, string name)
        {
            Name = name;
            if (string.IsNullOrEmpty(url))
            {
                Cover = new BitmapImage();
            }
            else
            {
                Cover = new BitmapImage(new Uri(url));
            }
        }

        public string Name { get; }
        public BitmapImage Cover { get; }
    }
}
