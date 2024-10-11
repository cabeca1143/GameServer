using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using LeagueSandbox.GameServer.Logging;
using Microsoft.CodeAnalysis;
using GameServerLib.Scripting;

namespace LeagueSandbox.GameServer.Scripting.CSharp;

public class CSharpScriptEngine
{
    private static ILog _logger = LoggerProvider.GetLogger();
    private readonly Dictionary<string, Type> _types = [];

    /// <summary>
    /// Loads scripts from a list of assemblies, to avoid project circular dependeces
    /// </summary>
    /// <param name="assemblies"></param>
    public CSharpScriptEngine()
    {
        foreach (Assembly assembly in AssemblyService.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.FullName != null)
                {
                    _types[type.FullName] = type;
                }
            }
        }
    }

    /// <summary>
    /// Creates a script object given a script namespace and class name.
    /// </summary>
    public T? CreateObject<T>(string scriptNamespace, string scriptClass, bool suppressWarning = false, T sourceObject = default)
    {
        if (sourceObject != null)
        {
            return CopyObject(sourceObject, suppressWarning);
        }

        if (AssemblyService.GetAssemblies().Length <= 0 || string.IsNullOrEmpty(scriptClass) || string.IsNullOrEmpty(scriptNamespace))
        {
            return default;
        }

        scriptNamespace = scriptNamespace.Replace(" ", "_");
        scriptClass = scriptClass.Replace(" ", "_");
        string fullClassName = scriptNamespace + "." + scriptClass;

        Type? classType = _types.GetValueOrDefault(fullClassName);
        if (classType != null)
        {
            return (T)Activator.CreateInstance(classType)!;
        }
        if (!suppressWarning)
        {
            _logger.Warn($"Could not find script: {fullClassName}");
        }

        return default;
    }

    public T? CopyObject<T>(T sourceObject, bool suppressWarning = false)
    {
        if (AssemblyService.GetAssemblies().Length <= 0 || sourceObject == null)
        {
            return default;
        }

        Type sourceType = sourceObject.GetType();
        Type? classType = _types.GetValueOrDefault(sourceObject.GetType().FullName!);
        if (classType != null)
        {
            ConstructorInfo? constructor = classType.GetConstructors().FirstOrDefault();
            object[]? defaultParameterValues = [.. constructor?.GetParameters()
                .Select(p => p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null)];

            T instance = (T)Activator.CreateInstance(classType, defaultParameterValues)!;

            FieldInfo[] fields = sourceType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                FieldInfo? correspondingField = classType.GetField(field.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (correspondingField != null && field.FieldType == correspondingField.FieldType && field.FieldType.IsValueType)
                {
                    correspondingField.SetValue(instance, field.GetValue(sourceObject));
                }
            }

            return instance;

        }
        if (!suppressWarning)
        {
            _logger.Warn($"Could not find script: {sourceObject.GetType().FullName}");
        }

        return default;
    }
}