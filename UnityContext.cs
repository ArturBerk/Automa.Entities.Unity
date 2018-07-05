using Automa.Entities.Events;
using Automa.Entities.Systems;
using UnityEngine;

namespace Automa.Entities.Unity
{
    public class UnityContext : MonoBehaviour
    {
        public bool Debug;
        internal IContext context;

        public IContext Context => context;

        public EntityManager EntityManager { get; private set; }
        public EventManager EventManager { get; private set; }
        public SystemManager SystemManager { get; private set; }

        private void Awake()
        {
            context = ContextFactory.CreateEntitiesContext(Debug);
            EntityManager = context.GetManager<EntityManager>();
            SystemManager = context.GetManager<SystemManager>();
            EventManager = context.GetManager<EventManager>();
            Setup();
        }

        protected virtual void Setup() { }

        private void Update()
        {
            context.Update();
        }

        void OnDestroy()
        {
            context.Dispose();
        }
    }
}