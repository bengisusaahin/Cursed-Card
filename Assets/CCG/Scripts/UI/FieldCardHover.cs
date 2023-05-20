using UnityEngine.EventSystems;
using UnityEngine;

public class FieldCardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public FieldCard card;
    public float hoverDelay = 0.4f;
    private bool isHovering = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Make sure our Player isn't already targetting something
        if (!Player.localPlayer.isTargeting && Player.gameManager.isOurTurn && card.casterType == Target.FRIENDLIES && card.CanAttack())
        {
            card.SpawnTargetingArrow(card.card);
            HideCardInfo();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        Invoke("ShowCardInfo", hoverDelay); // Reveal card info after a slight delay, so it doesn't appear instantly when we play a card.
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // Turn off hover
        card.cardHover.gameObject.SetActive(false);

        // Turn off Shine/Glow border
        Player.gameManager.CmdOnFieldCardHover(this.gameObject, false, false);

        Player.gameManager.isHoveringField = false;
    }

    public void ShowCardInfo()
    {
        if (isHovering)
        {
            // Turn on hover if player isn't targeting
            if (!Player.localPlayer.isTargeting) card.cardHover.gameObject.SetActive(true);

            // Turn on Shine/Glow border so our opponents can see us hovering over cards during our turn.
            if (Player.gameManager.isOurTurn)
            {
                Player.gameManager.isHoveringField = true;
                Player.gameManager.CmdOnFieldCardHover(this.gameObject, true, Player.localPlayer.isTargeting);
            }
        }
    }

    public void HideCardInfo()
    {
        card.cardHover.gameObject.SetActive(false);
    }
}
