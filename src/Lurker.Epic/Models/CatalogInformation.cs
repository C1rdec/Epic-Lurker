namespace Lurker.Epic.Models
{
    internal class CatalogInformations
    {
        public string CatalogNamespace { get; set; }

        public string CatalogItemId { get; set; }

        public string AppName { get; set; }

        public string GetArguments()
            => $"com.epicgames.launcher://apps/{CatalogNamespace}%3A{CatalogItemId}%3A{AppName}?action=launch&silent=true";
    }
}
