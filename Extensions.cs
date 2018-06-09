namespace Automa.Entities.Unity
{
    public static class Extensions
    {
        public static void SetComponent<T>(this EntityManager entityManager,
            Entity[] entityGroup, T data, bool onlyFirst = true)
        {
            foreach (var entity in entityGroup)
            {
                if (entityManager.HasComponent<T>(entity))
                {
                    entityManager.SetComponent(entity, data);
                    if (onlyFirst) return;
                }
            }
        }
    }
}
