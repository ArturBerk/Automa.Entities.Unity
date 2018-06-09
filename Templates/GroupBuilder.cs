using System;
using System.Collections.Generic;
using System.Linq;

namespace Automa.Entities.Unity.Templates
{
    internal class GroupBuilder : IGroupBuilder
    {
        private readonly List<IEntityBuilder> entities = new List<IEntityBuilder>();
        private EntityManager entityManager;

        public GroupBuilder(string name = null)
        {
            Name = name;
        }

        public string Name { get; }

        public void AddEntity(IEntityBuilder entityBuilder)
        {
            entities.Add(entityBuilder);
        }

        public IList<IEntityBuilder> EntityBuilders => entities;

        public EntityManager EntityManager
        {
            get
            {
                if (entityManager == null)
                    throw new ApplicationException("Entity manager only available after build completed");
                return entityManager;
            }
            protected set
            {
                entityManager = value;
            }
        }

        public IEnumerable<IEntityBuilder> SelectEntities(Func<IEntityBuilder, bool> filter)
        {
            return entities.Where(filter);
        }

        public virtual Entity[] Build(EntityManager entityManager)
        {
            EntityManager = entityManager;
            var result = new Entity[entities.Count];
            for (var index = 0; index < entities.Count; index++)
            {
                result[index] = entities[index].Build(entityManager);
            }
            return result;
        }
    }
}