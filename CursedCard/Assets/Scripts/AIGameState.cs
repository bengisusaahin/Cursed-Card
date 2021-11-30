using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class AIGameState
{
    public HeroBehaviourScript PlayerHero;
    public List<CardBehaviourScript> PlayerTableCards;

    public HeroBehaviourScript AIHero;
    public List<CardBehaviourScript> AIHandCards;
    public List<CardBehaviourScript> AITableCards;

    public int maxMana;
    public int PlayerMana;
    public int AIMana;

    public BoardBehaviourScript.Turn turn;

    
    public struct Action
    {
        public string Card1;
        public string Card2;
        public string Hero;
        public int OpCode;
    }
    public Queue<Action> Actions = new Queue<Action>();

   
    public void CardAttackCard(CardBehaviourScript _attacker, CardBehaviourScript _target)
    {
        CardBehaviourScript attacker = AITableCards.Find(item => item._name == _attacker._name);
        CardBehaviourScript target = PlayerTableCards.Find(item => item._name == _target._name);
        {
            Action a;
            a.Card1 = attacker._name;
            a.Card2 = target._name;
            a.Hero = "";
            a.OpCode = 1;
            Actions.Enqueue(a);
            attacker.AttackCard(attacker, target, false, delegate
            {
                attacker.canPlay = false;
            });
        }

    }
    public void CardAttackHero(CardBehaviourScript _attacker, HeroBehaviourScript _target)
    {
        CardBehaviourScript attacker = AITableCards.Find(item => item._name == _attacker._name);
        Action a;
        a.Card1 = attacker._name;
        a.Card2 = "";
        a.Hero = _target._name;
        a.OpCode = 2;
        Actions.Enqueue(a);
        attacker.AttackHero(attacker, PlayerHero, false, delegate
        {
            attacker.canPlay = false;
        });

    }
    public void PlaceCard(CardBehaviourScript temp)
    {
        //Find That Card
        CardBehaviourScript card = AIHandCards.Find(item => item._name == temp._name);
        if (card.team == CardBehaviourScript.Team.AI && AIMana - card.mana >= 0 && AITableCards.Count < 10)
        {
            AIHandCards.Remove(card);
            AITableCards.Add(card);
            Action a;
            a.Card1 = card._name;
            a.Card2 = "";
            a.Hero = "";
            a.OpCode = 0;
            Actions.Enqueue(a);
            card.SetCardStatus(CardBehaviourScript.CardStatus.OnTable);

            AIMana -= card.mana;
        }
    }
    public List<List<CardBehaviourScript>> ProducePlacing(List<CardBehaviourScript> allValues, int maxmana)
    {
        List<List<CardBehaviourScript>> collection = new List<List<CardBehaviourScript>>();
        for (int counter = 0; counter < (1 << allValues.Count); ++counter)
        {
            List<CardBehaviourScript> combination = new List<CardBehaviourScript>();
            for (int i = 0; i < allValues.Count; ++i)
            {
                if ((counter & (1 << i)) == 0)
                    combination.Add(allValues[i]);
            }

            collection.Add(combination);
        }
        return collection;
    }
    public List<List<CardBehaviourScript>> ProduceAllAttackCombinations(List<CardBehaviourScript> allValues)
    {
        List<List<CardBehaviourScript>> collection = new List<List<CardBehaviourScript>>();
        for (int counter = 0; counter < (1 << allValues.Count); ++counter)
        {
            List<CardBehaviourScript> combination = new List<CardBehaviourScript>();
            for (int i = 0; i < allValues.Count; ++i)
            {
                if ((counter & (1 << i)) == 0)
                    combination.Add(allValues[i]);
            }
            collection.Add(combination);
        }
        return collection;
    }
}
public static class CardListCopier
{
    public static List<CardBehaviourScript> DeepCopy(List<CardBehaviourScript> objectToCopy)
    {
        List<CardBehaviourScript> temp = new List<CardBehaviourScript>();
        foreach (CardBehaviourScript Card in objectToCopy)
        {
            temp.Add(Card.Clone() as CardBehaviourScript);
        }

        return temp;
    }

}