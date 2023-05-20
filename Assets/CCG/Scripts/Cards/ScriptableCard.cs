// Put all our cards in the Resources folder
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct CardAndAmount
{
    public ScriptableCard card;
    public int amount;
}


public partial class ScriptableCard : ScriptableObject
{
    [SerializeField] string id = "";
    public string CardID { get { return id; } }

    [Header("Image")]
    public Sprite image; // Card image

    [Header("Properties")]
    public int cost;
    public string category;

    [Header("Initiative Abilities")]
    public List<CardAbility> intiatives = new List<CardAbility>();

    [HideInInspector] public bool hasInitiative = false;

    [Header("Description")]
    [SerializeField, TextArea(1, 30)] public string description;

    // We can't pass ScriptableCards over the Network, but we can pass uniqueIDs.
    
    static Dictionary<string, ScriptableCard> _cache;
    public static Dictionary<string, ScriptableCard> Cache
    {
        get
        {
            if (_cache == null)
            {
                // Load all ScriptableCards from our Resources folder
                ScriptableCard[] cards = Resources.LoadAll<ScriptableCard>("");

                _cache = cards.ToDictionary(card => card.CardID, card => card);
            }
            return _cache;
        }
    }

    // Called when casting abilities or spells
    public virtual void Cast(Entity caster, Entity target)
    {

    }

    private void OnValidate()
    {
        // Get a unique identifier from the asset's unique 'Asset Path' 
        if (CardID == "")
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(this);
            id = AssetDatabase.AssetPathToGUID(path);
#endif
        }

        if (intiatives.Count > 0) hasInitiative = true;
    }
}

public enum AbilityType : byte { DAMAGE, HEAL, DRAW, DISCARD, BUFF, DEBUFF }

public partial class ScriptableAbility : ScriptableObject
{
    [Header("Targets")]
    public List<Target> targets = new List<Target>();

    [Header("Damage or Heal")]
    public int damage = 0;
    public int heal = 0;

    [Header("Buffs and Debuffs")]
    public int strength = 0;
    public int health = 0;

    [Header("Draw or Discard")]
    public int draw = 0;
    public int discard = 0;

    [Header("Properties")]
    public bool untilEndOfTurn = false; // Whether the buffs/debuffs last until the end of turn or are permanent. Permanent by default.

    public virtual void Cast(Entity target)
    {

    }

    private void OnValidate()
    {
        // By default, all creatures can only attack enemy creatures and our opponent. We set it here so every card get it's automatically.
        if (targets.Count == 0)
        {
            targets.Add(Target.OPPONENT);
        }
    }
}
[Serializable]
public struct CardAbility
{
    public AbilityType abilityType; // Doesn't actually do anything. This is just to help visualize what each ScriptableAbility is doing.
    public List<Target> targets;
    public ScriptableAbility ability;
}