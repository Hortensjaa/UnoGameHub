namespace Uno.Models;
using System;
using System.Collections.Generic;


public class Deck
{
    private readonly Random _random = new Random();
    public List<Card> Cards { get; }
    public Card TopCard { get; set; }
    
    public Deck()
    {
        Cards = GenerateFullDeck();  
        TopCard = Draw();
        while (TopCard.Type != CardType.Normal) 
        {
           TopCard = Draw();
        }
    }

    private List<Card> GenerateFullDeck()
    {
        var deck = new List<Card>();
        
        foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
        {
            if (color == CardColor.Wild)
            {
                continue;
            }
            deck.Add(new Card(CardType.Normal, 0, color)); 
            for (var i = 1; i <= 9; i++)
            {
                deck.Add(new Card(CardType.Normal, i, color));
                deck.Add(new Card(CardType.Normal, i, color)); 
            }
        }

        foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
        {
            if (color == CardColor.Wild)
            {
                continue;
            }
            for (var i = 0; i < 2; i++) 
            {
                deck.Add(new Card(CardType.Stop, -1, color));
                deck.Add(new Card(CardType.Reverse, -2, color));
                deck.Add(new Card(CardType.DrawTwo, -3, color));
            }
        }

        for (var i = 0; i < 4; i++)
        {
            deck.Add(new Card(CardType.ChooseColor, -4, CardColor.Wild));
            deck.Add(new Card(CardType.DrawFour, -5, CardColor.Wild));
        }

        return deck;
    }
    
    public Card Draw()
    {
        if (Cards.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty!");
        }

        var index = _random.Next(Cards.Count);
        Card drawnCard = Cards[index];
        Cards.RemoveAt(index);
        return drawnCard;
    }
    
    public bool IsCardValid(Card card, bool draw_fight, bool stop_fight)
    {
        // match two cards of the same type (always true, for normal cards we have to check color and number)
        if (card.Type == TopCard.Type)
        {
            if (card.Type == CardType.Normal)
            {
                return card.Color == TopCard.Color || card.Number == TopCard.Number;
            }
            return true;
        }
        
        // put on top of +4 (you can only put +2 or +4)
        if (TopCard.Type == CardType.DrawFour)
        {
            if (draw_fight)
            {
                return card.Type == CardType.DrawTwo && card.Color == TopCard.Color;
            }
            return card.Color == TopCard.Color || card.Color == CardColor.Wild;
        }
        
        // put on top of choose color (you can put any card unless it matches active color)
        if (TopCard.Type == CardType.ChooseColor)
        {
            return card.Color == TopCard.Color || card.Color == CardColor.Wild;
        }
        
        // put on top of +2 (you can only put +2 or +4)
        if (TopCard.Type == CardType.DrawTwo)
        {
            if (draw_fight)
            {
                return card.Type == CardType.DrawFour;
            }
            return card.Color == TopCard.Color || card.Color == CardColor.Wild;
        }  
        
        // put on top of stop (you can only put stop if active)
        if (TopCard.Type == CardType.Stop)
        {
            if (stop_fight)
            {
                return card.Type == CardType.Stop;
            }
            return card.Color == TopCard.Color || card.Color == CardColor.Wild;
        }

        // wild card on anything is fine (excluding +4 and +2)
        if (card.Color == CardColor.Wild)
        {
            return true;
        }

        return card.Color == TopCard.Color;
    }
    
    public void SetTopCard(Card card, string? requestedColor = null)
    {
        if (card.Type == CardType.ChooseColor || card.Type == CardType.DrawFour)
        {
            if (requestedColor == null)
            {
                throw new InvalidOperationException("You must specify a color.");
            }
            card.Color = Enum.Parse<CardColor>(requestedColor, true);
        }
        TopCard = card;
    }
}

