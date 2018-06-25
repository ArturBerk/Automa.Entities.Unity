using System.Linq;
using Automa.Entities.Builders;
using UnityEngine;

namespace Automa.Entities.Unity.Templates
{
    public class EntityTemplate<TParameter> : MonoBehaviour
    {
        public EntityBuilder<TParameter> GetBuilder()
        {
            return new EntityBuilder<TParameter>(gameObject.name, 
                GetComponentsInChildren<ComponentTemplate<TParameter>>()
                .Select(template => template.GetBuilder()));
        }
    }
}