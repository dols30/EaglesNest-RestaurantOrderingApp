namespace backendKiosk.Project.Models;

// Only the MAUI storage location is adapted for the console host. Tests execute
// the real menu models, controllers, and SQLite implementation against a temp DB.
internal static class FileSystem
{
    public static string AppDataDirectory => Path.GetTempPath();
}
