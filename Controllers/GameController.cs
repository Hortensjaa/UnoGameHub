namespace Uno.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Models;


public class GameController : Controller
{
    private static Game? _game;

    public ActionResult Index()
    {
        if (_game == null)
        {
            var players = new List<Player>
            {
                new Player("Alice"),
                new Player("Bob")
            };
            _game = new Game(players);
        }

        return View(_game);
    }

    public ActionResult DrawCard(string playerName)
    {
        if (_game != null)
        {
            _game.DrawCard(playerName);
        }
        return RedirectToAction("Index"); 
    }
    
    public ActionResult PlayCard(string playerName, string type, string number, string color, string? requestedColor = null)
    {
        if (_game != null)
        {
            var card = Card.CardFromStrings(type, number, color);
            _game.PlayCard(playerName, card, requestedColor);
        }
        return RedirectToAction("Index"); 
    }
}

