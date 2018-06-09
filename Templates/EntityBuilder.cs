using System;
using System.Collections.Generic;
using System.Linq;

namespace Automa.Entities.Unity.Templates
{
    internal class EntityBuilder : IEntityBuilder
    {
        private readonly Dictionary<ComponentType, IComponentBuilder> components =
            new Dictionary<ComponentType, IComponentBuilder>();

        public EntityBuilder(IGroupBuilder @group, string name = null)
        {
            Group = @group;
            Tag = name ?? string.Empty;
        }

        public string Tag { get; set; }
        public IGroupBuilder Group { get; }

        private Entity entity;

        public Entity Entity
        {
            get
            {
                if (entity == Entity.Null)
                    throw new ApplicationException("Entity is available only after build complete");
                return entity;
            }
            set
            {
                entity = value;
            }
        }

        public void AddComponent<T>(T data)
        {
            AddComponentBuilder(new ComponentBuilder<T>(data));
        }

        public bool HasComponent<T>()
        {
            IComponentBuilder builder;
            return components.TryGetValue(ComponentType.Create<T>(), out builder)
                   && builder is ComponentBuilder<T>;
        }

        public T GetComponent<T>()
        {
            IComponentBuilder builder;
            if (components.TryGetValue(ComponentType.Create<T>(), out builder)
                && builder is ComponentBuilder<T>)
            {
                return ((ComponentBuilder<T>)builder).data;
            }
            throw new ArgumentException("Entity not contains component data of type " + typeof(T).Name);
        }

        private void AddComponentBuilder(IComponentBuilder builder)
        {
            components.Add(builder.Type, builder);
        }

        public Entity Build(EntityManager entityManager)
        {
            // Build entity
            var entity = entityManager.CreateEntity(components.Keys.ToArray());
            // Build components
            foreach (var componentBuilder in components.Values)
            {
                componentBuilder.Build(entityManager, entity);
            }
            Entity = entity;
            return entity;
        }

        private interface IComponentBuilder
        {
            ComponentType Type { get; }
            void Build(EntityManager entityManager, Entity entity);
        }

        private class ComponentBuilder<T> : IComponentBuilder
        {
            public readonly T data;

            public ComponentBuilder(T data)
            {
                this.data = data;
            }

            public ComponentType Type => ComponentType.Create<T>();

            public void Build(EntityManager entityManager, Entity entity)
            {
                entityManager.SetComponent(entity, data);
            }
        }

    }
}