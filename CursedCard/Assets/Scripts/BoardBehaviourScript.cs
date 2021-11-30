using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class BoardBehaviourScript : MonoBehaviour
{
    
    public static BoardBehaviourScript instance;
    public UnityEngine.UI.Text winnertext;
    public Transform MYDeckPos;
    public Transform MyHandPos;
    public Transform MyTablePos;

    public Transform AIDeckPos;
    public Transform AIHandPos;
    public Transform AITablePos;

    public List<GameObject> MyDeckCards = new List<GameObject>();
    public List<GameObject> MyHandCards = new List<GameObject>();
    public List<GameObject> MyTableCards = new List<GameObject>();

    public List<GameObject> AIDeckCards = new List<GameObject>();
    public List<GameObject> AIHandCards = new List<GameObject>();
    public List<GameObject> AITableCards = new List<GameObject>();

    public TextMesh MyManaText;
    public TextMesh AIManaText;

    public HeroBehaviourScript MyHero;
    public HeroBehaviourScript AIHero;

    public enum Turn { MyTurn, AITurn };

    #region SetStartData
    public Turn turn = Turn.MyTurn;

    int maxMana = 1;
    int MyMana = 1;
    int AIMana = 1;

    public bool gameStarted = false;
    int turnNumber = 1;
    #endregion

    public CardBehaviourScript currentCard;
    public CardBehaviourScript targetCard;
    public HeroBehaviourScript currentHero;
    public HeroBehaviourScript targetHero;

    public List<Hashtable> boardHistory = new List<Hashtable>();
    public int AILEVEL = 0;
    public LayerMask layer;
    
    public void AddHistory(CardGameBase a, CardGameBase b)
    {
        Hashtable hash = new Hashtable();

        hash.Add(a, b);

        boardHistory.Add(hash);
        currentCard = null;
        targetCard = null;
        currentHero = null;
        targetHero = null;
    }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        foreach (GameObject CardObject in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardObject.GetComponent<Rigidbody>().isKinematic = true;
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            if (c.team == CardBehaviourScript.Team.My)
                MyDeckCards.Add(CardObject);
            else
                AIDeckCards.Add(CardObject);
        }
        //Update Deck Cards Position
        DecksPositionUpdate();
        
        //Start Game
        StartGame();
    }
    public void StartGame()
    {
        gameStarted = true;
        UpdateGame();

        for (int i = 0; i < 3; i++)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.My);
            DrawCardFromDeck(CardBehaviourScript.Team.AI);
        }
    }
    public void DrawCardFromDeck(CardBehaviourScript.Team team)
    {

        if (team == CardBehaviourScript.Team.My && MyDeckCards.Count != 0 && MyHandCards.Count < 10)
        {
            int random = Random.Range(0, MyDeckCards.Count);
            GameObject tempCard = MyDeckCards[random];

            tempCard.GetComponent<CardBehaviourScript>().newPos = MyHandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);

            MyDeckCards.Remove(tempCard);
            MyHandCards.Add(tempCard);
        }

        if (team == CardBehaviourScript.Team.AI && AIDeckCards.Count != 0 && AIHandCards.Count < 10)
        {
            int random = Random.Range(0, AIDeckCards.Count);
            GameObject tempCard = AIDeckCards[random];

            tempCard.transform.position = AIHandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);

            AIDeckCards.Remove(tempCard);
            AIHandCards.Add(tempCard);
        }

        //Update Hand Cards Position
        HandPositionUpdate();
    }

    void Update()
    {
        if (MyHero.health <= 0)
            EndGame(AIHero);
        if (AIHero.health <= 0)
            EndGame(MyHero);
    }
   
     void UpdateGame()
    {
        MyManaText.text = MyMana.ToString() + "/" + maxMana;
        AIManaText.text = AIMana.ToString() + "/" + maxMana;

        if (MyHero.health <= 0)
            EndGame(AIHero);
        if (AIHero.health <= 0)
            EndGame(MyHero);

    }

    
    void DecksPositionUpdate()
    {
        foreach (GameObject CardObject in MyDeckCards)
        {
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            if (c.cardStatus == CardBehaviourScript.CardStatus.InDeck)
            {
                c.newPos = MYDeckPos.position ;
            }
        }

        foreach (GameObject CardObject in AIDeckCards)
        {
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            if (c.cardStatus == CardBehaviourScript.CardStatus.InDeck)
            {
                c.newPos = AIDeckPos.position;
            }
        }
    }
    //önemli
    public void HandPositionUpdate()
    {
        float space = 0f;
        float space2 = 0f;
        float gap = 1.6f;

        foreach (GameObject card in MyHandCards)
        {
            int numberOfCards = MyHandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = MyHandPos.position + new Vector3(-numberOfCards / 2 + space, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AIHandCards)
        {
            int numberOfCards = AIHandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = AIHandPos.position + new Vector3(-numberOfCards / 2 + space2, 0, 0);
            space2 += gap;
        }
    }
    public void TablePositionUpdate()
    {
        float space = 0f;
        float space2 = 0f;
        float gap = 1.6f;
        float gap2 = 0.8f;

        foreach (GameObject card in MyTableCards)
        {
          
            //card.transform.position = myTablePos.position + new Vector3(-numberOfCards + space - 2,0,0);
            card.GetComponent<CardBehaviourScript>().newPos = MyTablePos.position + new Vector3( space - 2, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AITableCards)
        {
            if (card != null)
            {
                
                card.GetComponent<CardBehaviourScript>().newPos = AITablePos.position + new Vector3(+ space2 - 2, 0, 0);
                space2 += gap2;
            }
        }
    }

    public void PlaceCard(CardBehaviourScript card)
    {
        if (card.team == CardBehaviourScript.Team.My && MyMana - card.mana >= 0 && MyTableCards.Count < 10)
        {
            card.GetComponent<CardBehaviourScript>().newPos = MyTablePos.position;

            MyHandCards.Remove(card.gameObject);
            MyTableCards.Add(card.gameObject);

            card.SetCardStatus(CardBehaviourScript.CardStatus.OnTable);
           
            MyMana -= card.mana;
        }

        if (card.team == CardBehaviourScript.Team.AI && AIMana - card.mana >= 0 && AITableCards.Count < 10)
        {
            card.GetComponent<CardBehaviourScript>().newPos = AITablePos.position;

            AIHandCards.Remove(card.gameObject);
            AITableCards.Add(card.gameObject);

            card.SetCardStatus(CardBehaviourScript.CardStatus.OnTable);        

            AIMana -= card.mana;
        }

        TablePositionUpdate();
        HandPositionUpdate();
        UpdateGame();
    }
    public void PlaceRandomCard(CardBehaviourScript.Team team)
    {
     

        if (team == CardBehaviourScript.Team.AI && AIHandCards.Count != 0)
        {
            int random = Random.Range(0, AIHandCards.Count);
            GameObject tempCard = AIHandCards[random];

            PlaceCard(tempCard.GetComponent<CardBehaviourScript>());
        }

        UpdateGame();
        EndTurn();

        TablePositionUpdate();
        HandPositionUpdate();
    }
    public void EndGame(HeroBehaviourScript winner)
    {
        if (winner == MyHero)
        {
            Debug.Log("MyHero");
            Time.timeScale = 0;
            winnertext.text = "You Won";
            //Destroy(this);
        }

        if (winner == AIHero)
        {
            Time.timeScale = 0;
            Debug.Log("AIHero");
            winnertext.text = "You Lost";
            //Destroy(this);
        }
    }
    void OnGUI()
    {
        if (gameStarted)
        {
            if (turn == Turn.MyTurn)
            {
                if (GUI.Button(new Rect(Screen.width - 200, Screen.height / 2 - 50, 100, 50), "End Turn"))
                {
                    EndTurn();
                }
            }

            foreach (Hashtable history in boardHistory)
            {
                foreach (DictionaryEntry entry in history)
                {
                    CardGameBase card1 = entry.Key as CardGameBase;
                    CardGameBase card2 = entry.Value as CardGameBase;

                    GUILayout.Label(card1._name + " > " + card2._name);
                }
            }
            if (boardHistory.Count > 25)
            {
                Hashtable temp;
                temp = boardHistory[boardHistory.Count - 1];
                boardHistory.Clear();
                boardHistory.Add(temp);
            }
        }
    }
    public void EndTurn()
    {
        maxMana += (turnNumber-1)%2;
        if (maxMana >= 10) maxMana = 10;
        MyMana = maxMana;
        AIMana = maxMana;
        turnNumber += 1;
        currentCard = new CardBehaviourScript() ;
        targetCard = new CardBehaviourScript();
        currentHero = new HeroBehaviourScript();
        targetHero = new HeroBehaviourScript();
        foreach (GameObject card in MyTableCards)
            card.GetComponent<CardBehaviourScript>().canPlay = true;

        foreach (GameObject card in AITableCards)
        {
            if (card != null)
            {
                card.GetComponent<CardBehaviourScript>().canPlay = true;
            }
        }
       

        if (turn == Turn.AITurn)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.My);
            turn = Turn.MyTurn;
        }
        else if (turn == Turn.MyTurn)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.AI);
            turn = Turn.AITurn;
        }

        HandPositionUpdate();
        TablePositionUpdate();

        OnNewTurn();
    }
     public void OnNewTurn()
    {
        UpdateGame();

        if (turn == Turn.AITurn)
            AI_Think();
    }
    public void AI_Think()
    {
        
            if (turn == Turn.AITurn)
            {
                Invoke("RendomActions", 2.0f);
            }
        
       
    }
   
    void RendomActions()
    {
        #region placing cards

        if (AIHandCards.Count == 0)
        {
            EndTurn();
        }
        else
        {
            PlaceRandomCard(CardBehaviourScript.Team.AI);
        }
        #endregion
        #region attacking
        Hashtable attacks = new Hashtable();
        foreach (GameObject Card in AITableCards)
        {
            if (Card != null)
            {
                CardBehaviourScript card = Card.GetComponent<CardBehaviourScript>();

                if (card.canPlay)
                {
                    float changeToAttackhero = Random.value;

                    if (changeToAttackhero < 0.50f)
                    {
                        card.AttackHero(card, MyHero, true, delegate
                        {
                            card.canPlay = false;
                        });
                    }
                    else if (MyTableCards.Count > 0)
                    {
                        int random = Random.Range(0, MyTableCards.Count);
                        GameObject CardToAttack = MyTableCards[random];

                        attacks.Add(card, CardToAttack.GetComponent<CardBehaviourScript>());
                    }
                }
            }
        }

        foreach (DictionaryEntry row in attacks)
        {
            CardBehaviourScript tempCard = row.Key as CardBehaviourScript;
            CardBehaviourScript temp2 = row.Value as CardBehaviourScript;

           
            
                tempCard.AttackCard(tempCard, temp2, true, delegate
                {
                    tempCard.canPlay = false;
                });
            
            

        }
        #endregion
    }
   
  //Replace this!!!!  
    void OnTriggerEnter(Collider Obj)
    {
        CardBehaviourScript card = Obj.GetComponent<CardBehaviourScript>();
            if(card!=null)
            PlaceCard(card);
                        
    }
   
}