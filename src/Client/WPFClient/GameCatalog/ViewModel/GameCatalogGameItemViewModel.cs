using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFClient.Factory;
using WPFClient.GameCatalog.Command;
using WPFClient.Model;

namespace WPFClient.GameCatalog.ViewModel
{
    public class GameCatalogGameItemViewModel : ViewModelBase
    {
        private readonly CommandFactory commandFactory;

        public GameCatalogGameItemViewModel(CommandFactory commandFactory,
                                    GameModel gameModel,
                                    bool added)
            : this(commandFactory, gameModel.Id, gameModel.Name, gameModel.Url, added)
        {
        }

        public GameCatalogGameItemViewModel(CommandFactory commandFactory, int id, string name, string? coverUrl, bool added)
        {
            this.commandFactory = commandFactory;
            Id = id;
            Name = name;
            Added = added;
            CoverUrl = coverUrl;
            if (string.IsNullOrEmpty(coverUrl))
            {
                Cover = null;
            }
            else
            {
                Cover = new BitmapImage(new Uri(coverUrl));
            }
        }

        private bool added;
        public bool Added {
            get { return added; }
            set { SetField(ref added, value); }
        }
        public string? CoverUrl { get; }
        public string Name { get; }
        public BitmapImage? Cover { get; } 
        public int Id { get; }

        // TODO bind to parent
        public ICommand ToggleGameCommand => commandFactory.Get<ToggleGameCommand>(this);
    }
}
