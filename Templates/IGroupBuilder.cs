using System;
using System.Collections.Generic;

namespace Automa.Entities.Unity.Templates
{
    public interface IGroupBuilder
    {
        string Name { get; }

        IList<IEntityBuilder> EntityBuilders { get; }

        EntityManager EntityManager { get; }

        void AddEntity(IEntityBuilder entityBuilder);

        IEnumerable<IEntityBuilder> SelectEntities(Func<IEntityBuilder, bool> filter);

        Entity[] Build(EntityManager entityManager);
    }
}