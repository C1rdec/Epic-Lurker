using Lurker.Epic.Services;

var service = new EpicService();

await service.InitializeAsync();

var games = service.FindGames();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
