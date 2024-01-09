using System;
using System.Collections.Generic;
using System.IO;
using Lurker.Common.Extensions;
using Lurker.Common.Services;
using Lurker.Epic.Models;

namespace Lurker.Epic.Services;

public class EpicService : ServiceBase<EpicGame>
{
    protected override string ProcessName => "EpicGamesLauncher";

    protected override string OpenUrl => "com.epicgames.launcher://open";

    public override List<EpicGame> FindGames()
    {
        var games = new List<EpicGame>();
        if (string.IsNullOrEmpty(ExecutablePath))
        {
            return games;
        }

        var localAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var configurationFile = Path.Combine(localAppdata, "EpicGamesLauncher\\Saved\\Config\\Windows\\GameUserSettings.ini");
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
