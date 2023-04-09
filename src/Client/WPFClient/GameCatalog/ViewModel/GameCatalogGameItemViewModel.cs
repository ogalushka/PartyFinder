using System;
using System.Windows.Media.Imaging;

namespace WPFClient.GameCatalog.ViewModel
{
    public class GameCatalogGameItemViewModel
    {
        public GameCatalogGameItemViewModel(string name, string coverUrl, bool added)
        {
            Name = name;
            Cover = new BitmapImage(new Uri(coverUrl));
            ButtonText = added ? "Remove" : "Add";
        }

        public string Name { get; }
        public string ButtonText { get; }
        public BitmapImage Cover { get; } 
        public int Id { get; }
    }
}
