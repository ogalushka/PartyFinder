namespace WPFClient.Model
{
    public class GameModel
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Url;

        public GameModel(int id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url;
        }
    }
}
