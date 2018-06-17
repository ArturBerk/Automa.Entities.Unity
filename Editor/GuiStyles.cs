using UnityEngine;

namespace Automa.Entities.Unity
{
    [CreateAssetMenu(fileName = "Styles.asset", menuName = "Styles", order = 600)]
    public class GuiStyles : ScriptableObject
    {

        public GUIStyle includeType;
        public GUIStyle excludeType;
    }
}
