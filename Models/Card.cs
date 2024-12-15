using System.Diagnostics;

namespace Uno.Models;

public enum CardColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Wild
}

public enum CardType
{
    Normal,
    Stop,
    Reverse,
    DrawTwo,
    DrawFour,
    ChooseColor
}


public class Card
{
    public CardColor Color { get; set; }
    public int Number { get; }
    public CardType Type { get; }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Card other) return false;
        return Color == other.Color && Number == other.Number && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Color, Number, Type);
    }

    public Card(CardType type, int number, CardColor color)
    {
        Color = color;
        Type = type;
        Number = number;
    }
    
    public string toString()
    {
        if (Type == CardType.Normal)
        {
            return $"{Number}";
        }
        else
        {
            switch (Type)
            {
                case CardType.DrawFour: return "+4";
                case CardType.DrawTwo: return "+2";
                case CardType.ChooseColor: return "Choose";
            }
            return $"{Type}";
        }
        
    }

    public static Card CardFromStrings(string type, string number, string color)
    {
        var cardColor = (CardColor) Enum.Parse(typeof(CardColor), color);
        var cardType = (CardType) Enum.Parse(typeof(CardType), type);
        var cardNumber = int.Parse(number);
        return new Card(cardType, cardNumber, cardColor);
    }

}


