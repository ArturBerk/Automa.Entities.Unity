namespace Automa.Entities.Unity.Templates
{
    public interface IEntityBuilder
    {
        string Tag { get; set; }
        IGroupBuilder Group { get; }
        Entity Entity { get; }

        void AddComponent<T>(T data);
        bool HasComponent<T>();
        T GetComponent<T>();

        Entity Build(EntityManager entityManager);
    }
}