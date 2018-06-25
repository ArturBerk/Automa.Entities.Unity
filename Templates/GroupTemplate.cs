using System.Linq;
using Automa.Entities.Builders;
using UnityEngine;

namespace Automa.Entities.Unity.Templates
{
    public class GroupTemplate<TParameter> : MonoBehaviour
    {
        public Builder<TParameter> GetBuilder()
        {
            return new Builder<TParameter>(GetComponentsInChildren<EntityTemplate<TParameter>>()
                .Select(template => template.GetBuilder()));
        }
    }
}