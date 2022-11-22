using System;
using System.Collections.Generic;
using System.IO;
using Lurker.Common.Extensions;
using Lurker.Common.Services;
using Lurker.Epic.Models;

namespace Lurker.Epic.Services
{
    public class EpicService : ServiceBase<EpicGame>
    {
        protected override string ProcessName => "EpicGamesLauncher";

        protected override string OpenLink => @".\epic.lurker\OpenEpicLink.url";

        public override List<EpicGame> FindGames()
        {
            if (string.IsNullOrEmpty(ExecutablePath))
            {
                throw new InvalidOperationException("Must be initialize");
            }

            var games = new List<EpicGame>();

            var localAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var configurationFile = Path.Combine(localAppdata, "EpicGamesLauncher\\Saved\\Config\\Windows\\GameUserSettings.ini");
            var text = File.ReadAllText(configurationFile);
            var installationFolder = text.GetLineAfter("DefaultAppInstallLocation=");

            foreach (var folder in Directory.GetDirectories(installationFolder))
            {
                var epicGame = new EpicGame(folder, ExecutablePath);
                epicGame.Initialize();

                games.Add(epicGame);
            }

            return games;
        }
    }
}
