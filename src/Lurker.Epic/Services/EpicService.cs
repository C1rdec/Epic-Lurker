using System;
using System.Collections.Generic;
using System.IO;
using Lurker.Common.Extensions;
using Lurker.Common.Services;
using Lurker.Epic.Models;

namespace Lurker.Epic.Services;

public class EpicService : ServiceBase<EpicGame>
{
    private readonly List<string> GameUserSettingsLocations =
    [
        "EpicGamesLauncher\\Saved\\Config\\Windows\\GameUserSettings.ini",
        "EpicGamesLauncher\\Saved\\Config\\WindowsEditor\\GameUserSettings.ini"
    ];

    protected override string ProcessName => "EpicGamesLauncher";

    protected override string OpenUrl => "com.epicgames.launcher://open";

    public override List<EpicGame> FindGames()
    {
        var games = new List<EpicGame>();
        if (string.IsNullOrEmpty(ExecutablePath))
        {
            return games;
        }

        string configurationFile = null;
        foreach (var location in GameUserSettingsLocations)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), location);
            if (!File.Exists(path))
            {
                continue;
            }

            configurationFile = path;
            break;
        }

        if (configurationFile == null)
        {
            throw new FileNotFoundException("Could not find Epic Games configuration file");
        }

        var text = File.ReadAllText(configurationFile);
        var installationFolder = text.GetLineAfter("DefaultAppInstallLocation=");

        if (string.IsNullOrEmpty(installationFolder))
        {
            return games;
        }

        foreach (var folder in Directory.GetDirectories(installationFolder))
        {
            if (!Directory.Exists(Path.Combine(folder, ".egstore")))
            {
                continue;
            }

            var epicGame = new EpicGame(folder, ExecutablePath);
            epicGame.Initialize();

            games.Add(epicGame);
        }

        return games;
    }
}
