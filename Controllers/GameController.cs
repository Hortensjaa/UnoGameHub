using Microsoft.AspNetCore.SignalR;
using Uno.Hubs;

namespace Uno.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Models;


public class GameController : Controller
{
    private static Game? _game;
    private readonly IHubContext<GameHub> _hubContext;

    public GameController(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task<IActionResult> PlayCard(string playerName, Card card)
    {
        // Perform game logic here
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", playerName, $"played {card}");
        return RedirectToAction("Index"); 
    }


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

