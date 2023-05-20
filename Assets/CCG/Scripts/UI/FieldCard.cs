using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FieldCard : Entity
{
    [SyncVar, HideInInspector] public CardInfo card; // Get card info

    [Header("Card Properties")]
    public Image image; // card image on field
    public Text cardName; // Text of the card name
    public Text healthText; // Text of the health
    public Text strengthText; // Text of the strength

    [Header("Shine")]
    public Image shine;
    public Color hoverColor;
    public Color readyColor; // Shine color when ready to attack
    public Color targetColor; // Shine color when ready to attack

    [Header("Card Hover")]
    public HandCard cardHover;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        // If we have a card but no sprite, make sure the sprite is up to date since we can't SyncVar the sprite.
        // Useful to avoid bugs when a player was offline when the card spawned, or if they reconnected.
        if (image.sprite == null && (card.name != null || cardName.text == ""))
        {
            // Update Stats
            image.color = Color.white;
            image.sprite = card.image;
            cardName.text = card.name;

            // Update card hover info
            cardHover.UpdateFieldCardInfo(card);
        }

        healthText.text = health.ToString();
        strengthText.text = strength.ToString();

        if (CanAttack()) shine.color = readyColor;
        else if (CantAttack()) shine.color = Color.clear;
    }

    [Command(ignoreAuthority = true)]
    public void CmdUpdateWaitTurn()
    {
        if (waitTurn > 0) waitTurn--;
    }
}