using System;
using UnityEngine;
using Mirror;

[Serializable]
public partial struct PlayerInfo
{
    public GameObject player;
    

    public PlayerInfo(GameObject player)
    {
        this.player = player;
    }
    
    public Player data
    {
        get
        {
            
            return player.GetComponentInChildren<Player>();
        }
    }

   
    public string username => data.username;
    public Sprite portrait => data.portrait;

    
    public int health => data.health;
    public int mana => data.mana;
    

   
    public Sprite cardback => data.cardback;

  
    public int handCount => data.deck.hand.Count;
    public int deckCount => data.deck.deckList.Count;
    public int graveCount => data.deck.graveyard.Count;
}

public class SyncListPlayerInfo : SyncList<PlayerInfo> { }
