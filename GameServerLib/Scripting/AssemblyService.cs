using LeagueSandbox.GameServer.Logging;
using LeagueSandbox.GameServer;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace GameServerLib.Scripting;

/// <summary>
/// Internal class that loads assemblies at server instance creation.
/// </summary>
internal static class AssemblyService
{
    private static readonly ILog _logger = LoggerProvider.GetLogger();
    private static readonly List<Assembly> Assemblies = [];

    /// <summary>
    /// Attempts to load Assembly files that parts of the server will refrence
    /// </summary>
    /// <param name="assemblyPaths">Array of paths to assembly DLLs</param>
    public static void TryLoadAssemblies(string[] assemblyPaths)
    {
        CreateAssemblyList(assemblyPaths);
        Assemblies.Add(ServerLibAssemblyDefiningType.Assembly);
    }

    /// <returns>An array copy of loaded assemblies</returns>
    public static Assembly[] GetAssemblies()
    {
        return [.. Assemblies];
    }

    private static void CreateAssemblyList(string[] assemblyPaths)
    {
        foreach (var path in assemblyPaths)
        {
            string dll;
            if (!path.EndsWith(".dll"))
            {
                dll = path + ".dll";
            }
            else
            {
                dll = path;
            }

            if (!File.Exists(dll))
            {
                _logger.Error($"Unable to find Assembly {dll}");
                continue;
            }

            try
            {
                Assembly assembly = Assembly.LoadFile(Path.GetFullPath(dll));
                Assemblies.Add(assembly);
            }
            catch (Exception e)
            {
                _logger.Error($"Unable to load assembly: {dll}");
                _logger.Error(e);
                continue;
            }

            _logger.Info($"Successfully loaded assembly: {dll}");
        }
    }
}
