using System;
using UnityEngine;

public class TargetingArrow : MonoBehaviour
{
    [Header("Arrow Head")]
    public GameObject headPrefab;
    private ArrowHead arrowHead;
    private GameObject tempArrow;

    bool isDragging = false;
    bool isAbility = false;

    [HideInInspector] public Entity caster;

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            arrowHead.transform.position = mousePos;
            arrowHead.FindTargets(caster, mousePos, isAbility);
        }
    }

    public void DrawLine(Entity entity, CardInfo card, Vector2 startPosition, bool IsAbility)
    {
        caster = entity;
        isDragging = true;
        isAbility = IsAbility;

        tempArrow = Instantiate(headPrefab);
        arrowHead = tempArrow.GetComponent<ArrowHead>();
        tempArrow.transform.SetParent(transform, false);
        arrowHead.card = new CardInfo(card.data);
    }
}
[Serializable]
public enum Target : byte { OWNER, OPPONENT, FRIENDLIES, ENEMIES, RANDOM, ALL }