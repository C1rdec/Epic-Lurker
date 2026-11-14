using Lurker.Epic.Services;

var service = new EpicService();

await service.InitializeAsync();

var games = service.FindGames();
foreach (var game in games)
{
    Console.WriteLine($"Found {game.Name}");
}
