using System.Linq;
using Automa.Entities.Builders;
using UnityEngine;

namespace Automa.Entities.Unity.Templates
{
    public class EntityTemplate<TParameter> : MonoBehaviour
    {
        public virtual string Name => gameObject.name;

        public EntityBuilder<TParameter> GetBuilder()
        {
            return new EntityBuilder<TParameter>(Name, 
                GetComponentsInChildren<ComponentTemplate<TParameter>>()
                .Select(template => template.GetBuilder()));
        }
    }
}