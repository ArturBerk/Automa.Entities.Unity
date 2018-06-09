using UnityEngine;

namespace Automa.Entities.Unity.Templates
{
    public abstract class ComponentTemplate<T> : MonoBehaviour
    {
        public abstract void Build(IEntityBuilder entityBuilder, T parameter);
        public virtual void AfterBuild(IEntityBuilder entityBuilder, T parameter) { }
    }
}