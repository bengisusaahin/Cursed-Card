using System;
using UnityEngine;
using Mirror;


public enum PlayerType { PLAYER, ENEMY };

[RequireComponent(typeof(Deck))]
[Serializable]
public class Player : Entity
{
    [Header("Player Info")]
    [SyncVar(hook = nameof(UpdatePlayerName))] public string username; // SyncVar hook to call a command whenever a username changes.

    [Header("Portrait")]
    public Sprite portrait; 

    [Header("Deck")]
    public Deck deck;
    public Sprite cardback;
    [SyncVar, HideInInspector] public int tauntCount = 0; 

    [Header("Stats")]
    [SyncVar] public int maxMana = 10;
    [SyncVar] public int currentMax = 0;
    [SyncVar] public int _mana = 0;
    public int mana
    {
        get { return Mathf.Min(_mana, maxMana); }
        set { _mana = Mathf.Clamp(value, 0, maxMana); }
    }

    
    [HideInInspector] public static Player localPlayer;
    [HideInInspector] public bool hasEnemy = false; 
    [HideInInspector] public PlayerInfo enemyInfo; // We can't pass a Player class through the Network, but we can pass structs. 
    // We store all our enemy's info in a PlayerInfo struct so we can pass it through the network when needed.
    [HideInInspector] public static GameManager gameManager;
    [SyncVar, HideInInspector] public bool firstPlayer = false;

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;

       
        CmdLoadPlayer(PlayerPrefs.GetString("Name"));
        CmdLoadDeck();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        deck.deckList.Callback += deck.OnDeckListChange;
     
        deck.graveyard.Callback += deck.OnGraveyardChange;
    }

    [Command]
    public void CmdLoadPlayer(string user)
    {
      
        username = user;
    }

  
    void UpdatePlayerName(string oldUser, string newUser)
    {
        username = newUser;

   
    }

    [Command] //We use command attribute of mirror to call this from a client to run  this function on the server.
    public void CmdLoadDeck()
    {
       
        for (int i = 0; i < deck.startingDeck.Length; ++i)
        {
            CardAndAmount card = deck.startingDeck[i];
            for (int v = 0; v < card.amount; ++v)
            {
                deck.deckList.Add(card.amount > 0 ? new CardInfo(card.card, 1) : new CardInfo());
                if (deck.hand.Count < 7) deck.hand.Add(new CardInfo(card.card, 1));
            }
        }
        if (deck.hand.Count == 7)
        {
            deck.hand.Shuffle();
        }
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        health = gameManager.maxHealth;
        maxMana = gameManager.maxMana;
        deck.deckSize = gameManager.deckSize;
        deck.handSize = gameManager.handSize;

    }
   



    public override void Update()
    {
        base.Update();
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.G))
        {
            gameManager.StartGame();
        }
        // Get EnemyInfo as soon as another player connects. Only start updating once our Player has been loaded in properly.
        if (!hasEnemy && username != "")
        {
            UpdateEnemyInfo();
        }


    }

    public void UpdateEnemyInfo()
    {
        
        Player[] onlinePlayers = FindObjectsOfType<Player>();

     
        foreach (Player players in onlinePlayers)
        {
          
            if (players.username != "")
            {
              
                if (players != this)//not us
                {
                   
                    PlayerInfo currentPlayer = new PlayerInfo(players.gameObject);
                    enemyInfo = currentPlayer;
                    hasEnemy = true;
                    enemyInfo.data.casterType = Target.OPPONENT;
                 
                }
            }
        }
    }

    public bool IsOurTurn() => gameManager.isOurTurn;
}