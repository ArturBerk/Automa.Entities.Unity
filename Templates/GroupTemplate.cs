using UnityEngine;

namespace Automa.Entities.Unity.Templates
{
    public class GroupTemplate<T> : MonoBehaviour
    {
        public virtual Entity[] Build(EntityManager entityManager, T parameter)
        {
            IGroupBuilder groupBuilder = new GroupBuilder();
            var entityTemplates = GetComponentsInChildren<EntityTemplate<T>>();
            foreach (var entityTemplate in entityTemplates)
            {
                groupBuilder.AddEntity(new EntityBuilder(groupBuilder)
                {
                    Tag = entityTemplate.Tag
                });
            }
            for (var index = 0; index < entityTemplates.Length; index++)
            {
                var entityTemplate = entityTemplates[index];
                entityTemplate.Build(groupBuilder.EntityBuilders[index], parameter);
            }
            var builded = groupBuilder.Build(entityManager);
            for (var index = 0; index < entityTemplates.Length; index++)
            {
                var entityTemplate = entityTemplates[index];
                entityTemplate.AfterBuild(groupBuilder.EntityBuilders[index], parameter);
            }
            return builded;
        }
    }
}
