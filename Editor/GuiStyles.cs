using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Styles.asset", menuName = "Styles", order = 600)]
public class GuiStyles : ScriptableObject
{

    public GUIStyle includeType;
    public GUIStyle excludeType;
}
