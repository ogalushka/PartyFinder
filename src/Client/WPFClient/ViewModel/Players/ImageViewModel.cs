using System;
using System.Net.Cache;
using System.Windows.Media.Imaging;

namespace WPFClient.ViewModel.Players
{
    public class ImageViewModel
    {
        public ImageViewModel(string imageUrl)
        {
            //TODO check chaching
            Image = new BitmapImage(new Uri(imageUrl));
        }

        public BitmapImage Image { get; }
    }
}
