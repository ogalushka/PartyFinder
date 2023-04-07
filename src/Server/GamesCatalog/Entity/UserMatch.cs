
namespace GamesCatalog.Entity
{
    public class UserMatch
    {
        public string PlayerId { get; set; } = "";
        public int GameId { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int CommonStartTime { get; set; }
        public int CommonEndTime { get; set; }
    }
}
