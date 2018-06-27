using System.Collections.Generic;
using Automa.Entities.Builders;
using UnityEngine;

#pragma warning disable 649

namespace Automa.Entities.Unity.Templates
{
    public abstract class ComponentTemplate<TParameter> : MonoBehaviour
    {
        public abstract IComponentBuilder<TParameter> GetBuilder();
    }

    public class ComponentTemplate<TParameter, TComponent> : ComponentTemplate<TParameter>
    {
        [SerializeField] private TComponent component;

        public override IComponentBuilder<TParameter> GetBuilder()
        {
            return new ComponentValue<TParameter, TComponent>(component);
        }
    }

    public abstract class ComponentTemplateWithBuilder<TParameter> : ComponentTemplate<TParameter>,
        IComponentBuilder<TParameter>
    {
        public abstract void Build(ref EntityReference entity, ref TParameter parameter,
            Dictionary<string, EntityReference> templateEntities);

        public abstract ComponentType[] Types { get; }

        public override IComponentBuilder<TParameter> GetBuilder()
        {
            return this;
        }
    }
}