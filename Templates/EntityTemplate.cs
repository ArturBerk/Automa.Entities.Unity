using UnityEngine;

namespace Automa.Entities.Unity.Templates
{
    public class EntityTemplate<T> : MonoBehaviour
    {
        public string Tag => gameObject.name;

        public virtual void Build(IEntityBuilder entityBuilder, T parameter)
        {
            Build(entityBuilder, transform, parameter);
        }

        public virtual void AfterBuild(IEntityBuilder entityBuilder, T parameter)
        {
            AfterBuild(entityBuilder, transform, parameter);
        }

        protected static void Build(IEntityBuilder entityBuilder, Transform transform, T parameter)
        {
            foreach (var componentTemplate in transform.GetComponents<ComponentTemplate<T>>())
            {
                componentTemplate.Build(entityBuilder, parameter);
            }
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.GetComponent<EntityTemplate<T>>() == null)
                {
                    Build(entityBuilder, child, parameter);
                }
            }
        }

        protected static void AfterBuild(IEntityBuilder entityBuilder, Transform transform, T parameter)
        {
            foreach (var componentTemplate in transform.GetComponents<ComponentTemplate<T>>())
            {
                componentTemplate.AfterBuild(entityBuilder, parameter);
            }
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.GetComponent<EntityTemplate<T>>() == null)
                {
                    Build(entityBuilder, child, parameter);
                }
            }
        }
    }
}