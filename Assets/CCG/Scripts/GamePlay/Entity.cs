using UnityEngine;
using System;
using Mirror;
using System.Collections.Generic;

[Serializable]
public abstract partial class Entity : NetworkBehaviour //extends networkbehaviour 
{
    [Header("Combat")]
    public Combat combat;

    [Header("Stats")]
    [SyncVar] public int health = 0;
    [SyncVar] public int strength = 0;

    [Header("Targeting Arrow")]
    public Target casterType;
    public TargetingArrow arrow;
    public Transform spawnOffset;
    [HideInInspector] public bool isTargeting = false;
    [HideInInspector] public GameObject arrowObject;

    public bool isTargetable = true; 

    [Header("Special Properties")] // These spawn properties are set by our ScriptableCards, when the card is spawned into the game.
    [SyncVar] public int waitTurn = 1; 
    public bool taunt = false; 

    public bool IsDead() => health <= 0;
    public bool CanAttack() => Player.gameManager.isOurTurn && waitTurn == 0 && casterType == Target.FRIENDLIES;
    public bool CantAttack() => Player.gameManager.isOurTurn && waitTurn > 0 && casterType == Target.FRIENDLIES;

    public virtual void SpawnTargetingArrow(CardInfo card, bool IsAbility = false)
    {
        Player.localPlayer.isTargeting = true;
        isTargeting = true;

        Cursor.visible = false;

        
        Vector3 spawnPos = spawnOffset == null ? transform.position : spawnOffset.position;
        arrowObject = Instantiate(arrow.gameObject, spawnPos, Quaternion.identity);
        arrowObject.GetComponent<TargetingArrow>().DrawLine(this, card, spawnPos, IsAbility);
    }

    public void DestroyTargetingArrow()
    {
        Player.localPlayer.isTargeting = false;
        isTargeting = false;
        Cursor.visible = true;
        Destroy(arrowObject);
    }

    public virtual void Update()
    {
        if (isTargeting && Input.GetMouseButton(1))
        {
            DestroyTargetingArrow();
        }
    }
}
