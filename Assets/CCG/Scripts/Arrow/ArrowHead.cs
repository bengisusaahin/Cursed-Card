using System.Collections.Generic;
using UnityEngine;

public class ArrowHead : MonoBehaviour
{
    [Header("Arrow Heads")]
    public Sprite defaultHead;
    public Sprite targetHead;

    [Header("Properties")]
    public SpriteRenderer spriteRenderer;
    public LayerMask targetLayer;

    [HideInInspector] public CardInfo card;

    public void FindTargets(Entity caster, Vector2 mousePos, bool IsAbility)
    {
        RaycastHit2D[] hitInfo = Physics2D.CircleCastAll(mousePos, 0.1f, Vector2.zero, 1f, targetLayer);

        // If greater than 0, then we hit s
        if (hitInfo.Length > 0)
        {
            RaycastHit2D hit = hitInfo[0];
            Entity target = hit.collider.gameObject.GetComponent<Entity>();

            bool canTarget = target.casterType.CanTarget(card.acceptableTargets);

            // Check to see if we can attack this target : If entity isn't the one currently targeting, is targetable and isn't friendly
            if (target && !target.isTargeting && target.isTargetable && canTarget)
            {
                spriteRenderer.sprite = targetHead;
                if (Input.GetMouseButtonDown(0))
                {
                    if (!IsAbility) ((CreatureCard)card.data).Attack(caster, target);
                    AudioManager.PlaySound("kartSaldırı");
                    
                }
            }
            else
            {
                spriteRenderer.sprite = defaultHead;
            }
        }
        else if (spriteRenderer.sprite != defaultHead)
        {
            spriteRenderer.sprite = defaultHead;
        }
    }
}
