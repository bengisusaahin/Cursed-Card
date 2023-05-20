using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [Header("Health")]
    public int maxHealth = 30;

    [Header("Mana")]
    public int maxMana = 10;

    [Header("Hand")]
    public int handSize = 7;
    public PlayerHand playerHand;
    public PlayerHand enemyHand;

    [Header("Deck")]
    public int deckSize = 30; 
    public int identicalCardCount = 2; 
    [Header("Battlefield")]
    public PlayerField playerField;
    public PlayerField enemyField;

    [Header("Turn Management")]
    public GameObject endTurnButton;
    [HideInInspector] public bool isOurTurn = false;
    [SyncVar, HideInInspector] public int turnCount = 1;

    
    [HideInInspector] public bool isHovering = false;
    [HideInInspector] public bool isHoveringField = false;
    [HideInInspector] public bool isSpawning = false;

    public SyncListPlayerInfo players = new SyncListPlayerInfo(); // Information of all players online. One is player, other is opponent.

    // Not sent from Player / Object with Authority, so we need to ignoreAuthority. 
    // We could also have this command run on the Player instead
    [Command(ignoreAuthority = true)]
    public void CmdOnCardHover(float moveBy, int index)
    {
      
        if (enemyHand.handContent.transform.childCount > 0 && isServer) RpcCardHover(moveBy, index);
    }

    [ClientRpc]
    public void RpcCardHover(float moveBy, int index)
    {
       
        if (!isHovering)
        {
            HandCard card = enemyHand.handContent.transform.GetChild(index).GetComponent<HandCard>();
            card.transform.localPosition = new Vector2(card.transform.localPosition.x, moveBy);
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdOnFieldCardHover(GameObject cardObject, bool activateShine, bool targeting)
    {
       
        if (isServer) RpcFieldCardHover(cardObject, activateShine, targeting);
    }

    [ClientRpc]
    public void RpcFieldCardHover(GameObject cardObject, bool activateShine, bool targeting)
    {
        if (!isHoveringField)
        {
            FieldCard card = cardObject.GetComponent<FieldCard>();
            Color shine = activateShine ? card.hoverColor : Color.clear;
            card.shine.color = targeting ? card.targetColor : shine;
           
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdEndTurn()
    {
        RpcSetTurn();
    }

    [ClientRpc]
    public void RpcSetTurn()
    {
        // If isOurTurn was true, set it false. If it was false, set it true.
        isOurTurn = !isOurTurn;
        endTurnButton.SetActive(isOurTurn);

       
        if (isOurTurn)
        {
          
            AudioManager.PlaySound("yeniTur");
            playerField.UpdateFieldCards();
            Player.localPlayer.deck.CmdStartNewTurn();
            Player.localPlayer.deck.DrawCard(1);
        }

    }

    public void StartGame()
    {
        Player player = Player.localPlayer;
        endTurnButton.SetActive(true);
        player.mana++;
        player.currentMax++;        
        isOurTurn = true;
        
    }

}
