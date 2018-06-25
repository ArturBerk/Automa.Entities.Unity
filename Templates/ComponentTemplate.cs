using Automa.Entities.Builders;
using UnityEngine;

namespace Automa.Entities.Unity.Templates
{
    public abstract class ComponentTemplate<TParameter> : MonoBehaviour
    {
        public abstract IComponentBuilder<TParameter> GetBuilder();
    }

    public class ComponentTemplate<TParameter, TComponent> : ComponentTemplate<TParameter>
    {
        [SerializeField]
        private TComponent component;

        public override IComponentBuilder<TParameter> GetBuilder()
        {
            return new ComponentValue<TParameter, TComponent>(component);
        }
    }
}