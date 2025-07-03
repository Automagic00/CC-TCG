using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;
using UnityEditor;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Data", menuName = "CCTCG/Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public string cardName;
    public Sprite cardImage;
    public int health;
    public int damage;
    public string description;
    public List<Action> actions;
    //public targetType targetType;

    public CostDictionary costs = new CostDictionary() {
        {costType.Fire,0 },
        {costType.Earth,0 },
        {costType.Water,0 },
        {costType.Air,0 }
    };


 
}

public enum targetType
{ 
    Standard, //Hits 1 space ahead if in front
    Ranged, //Hits 2 spaces ahead
    Bi_Split, //Hits to the left and right
    Tri_Split, //Hits left right and ahead
    Partner, //Targets ally in same lane
    Adjacent, //Targets allies to the left and right
    Allies, //Targets all allies
    Enemies, //Targets all opponents
    Self //Targets self
}

[Serializable]
public class Action
{
    public targetType target;
    public actionType action;

    [HideIf("HideValue")]
    [AllowNesting]
    public int value;

    public bool HideValue()
    {
        List<actionType> validActions = new List<actionType>() { actionType.Heal};
        return !validActions.Contains(action);
    }
}
/*
#if UNITY_EDITOR
[CustomEditor(typeof(CardScriptableObject))]
public class MyEditorClass : Editor
{
    public override void OnInspectorGUI()
    {

        // If we call base the default inspector will get drawn too.
        // Remove this line if you don't want that to happen.
        //base.OnInspectorGUI();
        List<string> disables = new List<string>();
        CardScriptableObject self = target as CardScriptableObject;
        serializedObject.Update();

        foreach (Action action in self.actions)
        {
            if (action.action == actionType.Heal)
            {
                disables.Add(nameof(action.value));
            }
        }
        if (disables.Count <= 0)
        {
            DrawDefaultInspector();
        }
        else
        {
            DrawPropertiesExcluding(serializedObject, disables.ToArray());
        }
        
        serializedObject.ApplyModifiedProperties();
        
    }
}
#endif
*/

public enum actionType
{
    Attack,
    Heal,
    BuffAttack
}
public enum costType
{
    Fire,
    Water,
    Earth,
    Air,
    Any
}
public enum eventType
{
    DamageRecieved,
    DamageDealt,
    ThisCardPlayed,
    ThisCardDied,

}

[Serializable]
public class CostDictionary : SerializableDictionary<costType, int> { }

