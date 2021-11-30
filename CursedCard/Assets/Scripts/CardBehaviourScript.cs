using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class CardBehaviourScript : CardGameBase, System.ICloneable
{

    public string description;
    public Texture2D image;
    public int health;
    public int _Attack;
    public int mana;

    public TextMesh nameText;
    public TextMesh healthText;
    public TextMesh AttackText;
    public TextMesh manaText;
    public TextMesh DescriptionText;
    public TextMesh DebugText;

    public bool GenerateRandomeData = false;
    public bool canPlay = false;
    public enum CardStatus { InDeck, InHand, OnTable, Destroyed };
    public CardStatus cardStatus = CardStatus.InDeck;
    
    
    public enum CardEffect { ToAll, ToEnemies, ToSpecific };
    public CardEffect cardeffect;
    public int AddedHealth;
    public int AddedAttack;
    public enum Team { My, AI };
    public Team team = Team.My;

    public Vector3 newPos;

    float distance_to_screen;
    bool Selected = false;
    public delegate void CustomAction();
    void Start()
    {
        distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z - 1;
        DescriptionText.text = description.ToString();

    }
    void FixedUpdate()
    {
        if (!Selected)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 3);
           
                if (cardStatus != CardStatus.InDeck)
                {
                    if(cardStatus == CardStatus.InHand && team == Team.AI)
                    {
                        //Do nothing.
                    }
                    else
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);

            }
            
            
        }
        
            if (health <= 0)
            {
                Destroy(this);
            }
        
        //Update Visuals
        nameText.text = _name.ToString();
        healthText.text = health.ToString();
        AttackText.text = _Attack.ToString();
        manaText.text = mana.ToString();
        DebugText.text = canPlay ? "Ready to attack" : "Not Ready";
    }
    
    void OnMouseDown()
    {
        if (cardStatus == CardStatus.InHand)
        {
            Selected = true;
        }

        
        if (!BoardBehaviourScript.instance.currentCard && cardStatus==CardStatus.OnTable && team == Team.My)
        {
            BoardBehaviourScript.instance.currentCard = this;
            print("Selected card: " + _Attack + ":" + health);
        }
       
        else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && cardStatus == CardStatus.OnTable && BoardBehaviourScript.instance.currentCard!=this)//Card VS Card
        {
            //clicked opponent card on table on your turn
            if (BoardBehaviourScript.instance.currentCard.canPlay)
            {
                BoardBehaviourScript.instance.targetCard = this;
                print("Target card: " + _Attack + ":" + health);


                AttackCard(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard, true, delegate
                 {
                     BoardBehaviourScript.instance.currentCard.canPlay = false;
                 });

            }
            else
                 print("Card cannot attack");
            
           
        }
       
        else
        {
            BoardBehaviourScript.instance.currentCard = null;
            BoardBehaviourScript.instance.currentHero = null;
            BoardBehaviourScript.instance.targetCard = null;
            BoardBehaviourScript.instance.targetHero = null;
            Debug.Log("Action Reset");
        }

    }
    void OnMouseUp()
    {
        //Debug.Log("On Mouse Up Event");
        Selected = false;
    }
  
    void OnMouseDrag()
    {
        //Debug.Log("On Mouse Drag Event");
        if(team == Team.My)
            GetComponent<Rigidbody>().MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen)));
    }
    public void SetCardStatus(CardStatus status)
    {
        cardStatus = status;
    }
    public void AttackCard(CardBehaviourScript attacker, CardBehaviourScript target,bool addhistory, CustomAction action)
    {
       
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;
        
            if (target.health <= 0)
            {
                    Destroy(target);
            

            }

            if (attacker.health <= 0)
            {
                    attacker.Destroy(attacker);
            }

            action();
            if(addhistory)
            BoardBehaviourScript.instance.AddHistory(attacker, target);
        
    }//Attack
    public void AttackHero(CardBehaviourScript attacker, HeroBehaviourScript target, bool addhistory, CustomAction action)
    {
        if (attacker.canPlay)
        {
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;

            action();
            if (addhistory)
                BoardBehaviourScript.instance.AddHistory(attacker, target);
        }
    }//Attack
    public void Destroy(CardBehaviourScript card)
    {
        
            if (card.gameObject != null)
            {
                if (card.team == CardBehaviourScript.Team.My)
                    BoardBehaviourScript.instance.MyTableCards.Remove(card.gameObject);
                else if (card.team == CardBehaviourScript.Team.AI)
                    BoardBehaviourScript.instance.AITableCards.Remove(card.gameObject);


                Destroy(card.gameObject);

            BoardBehaviourScript.instance.TablePositionUpdate();
        }

                  

    }

    public object Clone()
    {
        CardBehaviourScript temp = new CardBehaviourScript();
        temp._name = _name;
        temp.description = this.description;
        temp.health = this.health;
        temp._Attack = this._Attack;
        temp.mana = this.mana;
        temp.canPlay = this.canPlay;
        temp.cardStatus = this.cardStatus;
        
        temp.cardeffect = this.cardeffect;
        temp.AddedHealth = this.AddedHealth;
        temp.AddedAttack = this.AddedAttack;
        temp.team = this.team;
        temp.newPos = this.newPos;
        temp.distance_to_screen = this.distance_to_screen;
        temp.Selected = this.Selected;
        return temp;
    }
}