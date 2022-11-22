using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FuzzySharp;
using Lurker.Common.Models;

namespace Lurker.Epic.Models
{
    public class EpicGame : GameBase
    {
        #region Fields

        private string _installationFolder;
        private string _epicExe;
        private string _mancpnFilepath;
        private CatalogInformations _informations;

        #endregion

        #region Methods

        public EpicGame(string installationFolder,  string epicExe)
        {
            _installationFolder = installationFolder;
            _epicExe = epicExe;
            Name = FirstCharToUpper(Path.GetFileName(installationFolder));

            var storeFolder = Path.Combine(installationFolder, ".egstore");
            _mancpnFilepath = Directory.GetFiles(storeFolder, "*.mancpn").FirstOrDefault();
            if (_mancpnFilepath == null)
            {
                throw new InvalidOperationException("Not an Epic Game");
            }
        }

        #endregion

        #region Properties

        public override LauncherType Launcher => LauncherType.Epic;

        #endregion

        #region Methods

        public override void Initialize()
        {
            _informations = System.Text.Json.JsonSerializer.Deserialize<CatalogInformations>(File.ReadAllText(_mancpnFilepath));

            var exeFiles = new DirectoryInfo(_installationFolder).GetFiles($"*.exe", SearchOption.AllDirectories);
            var matches = exeFiles.Select(e => new
            {
                FilePath = e.FullName,
                Ratio = Fuzz.Ratio(e.Name.Replace(".exe", string.Empty).ToLower(), Name.ToLower())
            });

            var bestmatch = matches.MaxBy(r => r.Ratio);
            ExeFilePath = bestmatch.FilePath;
        }

        public override Task Open()
            => CliWrap.Cli.Wrap(_epicExe).WithArguments(_informations.GetArguments()).ExecuteAsync();



        public static string FirstCharToUpper(string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        #endregion
    }
}
