using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour
{
    [Header("Player")]
    public Player player;
    [HideInInspector] public int deckSize = 30;
    [HideInInspector] public int handSize = 7;

    [Header("Decks")]
    public SyncListCard deckList = new SyncListCard(); // DeckList used during the match. Contains all cards in the deck. This is where we'll be drawing card froms.
    public SyncListCard graveyard = new SyncListCard(); // Cards in player graveyard.
    public SyncListCard hand = new SyncListCard(); // Cards in player's hand during the match.

    [Header("Battlefield")]
    public SyncListCard playerField = new SyncListCard(); 

    [Header("Starting Deck")]
    public CardAndAmount[] startingDeck;

    [HideInInspector] public bool spawnInitialCards = true;

    public void OnDeckListChange(SyncListCard.Operation op, int index, CardInfo oldCard, CardInfo newCard)
    {
        UpdateDeck(index, 1, newCard);
    }

    public void OnHandChange(SyncListCard.Operation op, int index, CardInfo oldCard, CardInfo newCard)
    {
        UpdateDeck(index, 2, newCard);
    }

    public void OnGraveyardChange(SyncListCard.Operation op, int index, CardInfo oldCard, CardInfo newCard)
    {
        UpdateDeck(index, 3, newCard);
    }

    public void UpdateDeck(int index, int type, CardInfo newCard)
    {
        // Deck List
        if (type == 1) deckList[index] = newCard;

        // Hand
        if (type == 2) hand[index] = newCard;

        // Graveyard
        if (type == 3) graveyard[index] = newCard;

    }



    public bool CanPlayCard(int manaCost)
    {
        return player.mana >= manaCost && player.health > 0;
    }

    public void DrawCard(int amount)
    {
        AudioManager.PlaySound("kartCekme");
        PlayerHand playerHand = Player.gameManager.playerHand;
        for (int i = 0; i < amount; ++i)
        {
            int index = i;
            playerHand.AddCard(index);
        }
        spawnInitialCards = false;
    }

    [Command]
    public void CmdPlayCard(CardInfo card, int index)
    {
        CreatureCard creature = (CreatureCard)card.data;
        GameObject boardCard = Instantiate(creature.cardPrefab.gameObject);
        FieldCard newCard = boardCard.GetComponent<FieldCard>();
        newCard.card = new CardInfo(card.data); // Save Card Info so we can re-access it later if we need to.
        newCard.cardName.text = card.name;
        newCard.health = creature.health;
        newCard.strength = creature.strength;
        newCard.image.sprite = card.image;
        newCard.image.color = Color.white;

        // If creature has charge, reduce waitTurn to 0 so they can attack right away.
        if (creature.hasCharge) newCard.waitTurn = 0;

        // Update the Card Info that appears when hovering
        newCard.cardHover.UpdateFieldCardInfo(card);

        // Spawn it
        NetworkServer.Spawn(boardCard);

        // Remove card from hand
        hand.RemoveAt(index);

        if (isServer) RpcPlayCard(boardCard, index);
    }

    [Command]
    public void CmdStartNewTurn()
    {
        
        if (player.mana < player.maxMana)
        {
            player.currentMax++;
            player.mana = player.currentMax;
 
        }
    }

    [ClientRpc]
    public void RpcPlayCard(GameObject boardCard, int index)
    {
        AudioManager.PlaySound("kartAtma");
        if (Player.gameManager.isSpawning)
        {
            // Set our FieldCard as a FRIENDLY creature for our local player, and ENEMY for our opponent.
            boardCard.GetComponent<FieldCard>().casterType = Target.FRIENDLIES;
            boardCard.transform.SetParent(Player.gameManager.playerField.content, false);
            Player.gameManager.playerHand.RemoveCard(index); // Update player's hand
            Player.gameManager.isSpawning = false;
        }
        else if (player.hasEnemy)
        {
            boardCard.GetComponent<FieldCard>().casterType = Target.ENEMIES;
            boardCard.transform.SetParent(Player.gameManager.enemyField.content, false);
            Player.gameManager.enemyHand.RemoveCard(index);
        }
    }
}
