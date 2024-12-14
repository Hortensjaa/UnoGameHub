namespace Uno.Models;

public class Player
{
    public string Name { get; set; }
    public List<Card> Hand { get; private set; } = new List<Card>();
    public int BlockedTurns { get; set; } = 0; 
    
    public Player(string name)
    {
        Name = name;
    }

    public void BlockPlayer(int turns)
    {
        BlockedTurns = turns;
    }   
    
    public Card PlayCard(Card card)
    {
        Hand.Remove(card);
        return card;
    }
    
    public void DrawCard(Deck deck)
    {
        Hand.Add(deck.Draw());
    }
}