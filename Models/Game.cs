namespace Uno.Models;

public class Game
{
    public Dictionary<string, Player> Players { get; private set; }
    public Deck Deck { get; private set; }
    public string ActivePlayer { get; set; }
    public bool ClockwiseTurn { get; set; } = true;
    private int _drawCount = 0;
    private int _blockCount = 0;
    public string? ErrorMessage { get; set; }

    
    public Game(List<Player> players)
    {
        Deck = new Deck();
        Players = players
            .Select(p => new KeyValuePair<string, Player>(p.Name, p))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        for (var i = 0; i < 7; i++)
        {
            foreach (var player in Players.Values)
            {
                player.DrawCard(Deck);
            }
        }
        ActivePlayer = players[0].Name;
    }
    
    private string GetNextPlayerName()
    {
        var playerNames = Players.Keys.ToList();
        var currentIndex = playerNames.IndexOf(ActivePlayer);
        var nextIndex = ClockwiseTurn
            ? (currentIndex + 1) % playerNames.Count
            : (currentIndex - 1 + playerNames.Count) % playerNames.Count;
        return playerNames[nextIndex];
    }

    private bool AddCardOnTop(string playerName, Card card)
    {
        if (Players[ActivePlayer].Name != playerName)
        {
            ErrorMessage = "It is not your turn.";
            return false;
        }
        if (!Players[ActivePlayer].Hand.Contains(card))
        {
            ErrorMessage = "You do not have that card.";
            return false;
        }
        if (!Deck.IsCardValid(card, _drawCount > 0, _blockCount > 0))
        {
            ErrorMessage = $"You cannot play {card.toString()} on {Deck.TopCard.toString()}.";
            return false;
        }

        ErrorMessage = null; 
        Players[ActivePlayer].Hand.Remove(card);
        return true;
    }


    private void SpecialCardAction()
    {
        if (Deck.TopCard.Type == CardType.Reverse)
        {
            ClockwiseTurn = !ClockwiseTurn;
            return;
        }

        if (Deck.TopCard.Type == CardType.Stop)
        {
            _blockCount += 1;
            return;
        }

        if (Deck.TopCard.Type == CardType.DrawTwo)
        {
            _drawCount += 2;
            return;
        }

        if (Deck.TopCard.Type == CardType.DrawFour)
        {
            _drawCount += 4;
            return;
        }
        
        if (Deck.TopCard.Type == CardType.ChooseColor)
        {
            return;
        }
    }
    
    private void NextTurn()
    {
        foreach (var player in Players.Values)
        {
            if (player.Hand.Count == 0)
            {
                ErrorMessage = $"{player.Name} has won!";
                return;
            }
        }        
        foreach (var player in Players.Values)
        {
            if (player.Hand.Count == 1)
            {
                ErrorMessage = $"{player.Name}: Uno!";
            }
        }
        ActivePlayer = GetNextPlayerName();
        if (Players[ActivePlayer].BlockedTurns > 0)
        {
            Players[ActivePlayer].BlockedTurns -= 1;
            NextTurn();
        }
    }
    
    public void PlayCard(string playerName, Card card, string? requestedColor = null)
    {
        if (AddCardOnTop(playerName, card))
        {
              Deck.SetTopCard(card, requestedColor);
              SpecialCardAction();
              NextTurn();  
        }
    }
    
    public void DrawCard(string playerName)
    {
        if (Players[ActivePlayer].Name != playerName)
        {
            ErrorMessage = "It is not your turn.";
            return;
        }
        ErrorMessage = null;

        if (_blockCount > 0)
        {
            Players[ActivePlayer].BlockPlayer(_blockCount);
            _blockCount = 0;
            NextTurn();
            return;
        }
        
        for (var i = 0; i < Math.Max(1, _drawCount); i++)
        {
            Players[ActivePlayer].DrawCard(Deck);
        }
        _drawCount = 0;


        NextTurn();
    }


}