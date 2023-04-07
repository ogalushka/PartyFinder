namespace Common.Mongo
{
    public interface IEntity<Key>
    {
        Key Id { get; set; }
    }
}
