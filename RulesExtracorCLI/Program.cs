﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Reflection;

if (args is null || args.Length == 0)
	throw new ArgumentNullException("No file path specified");

var dllPath = args[0];
if (!File.Exists(dllPath))
	throw new FileNotFoundException(dllPath);

foreach (var dependencyPath in args.Skip(1))
	Assembly.LoadFrom(dependencyPath);

var analyzerAssembly = Assembly.LoadFrom(dllPath);

var assemblies = AppDomain.CurrentDomain.GetAssemblies();
var descriptors = analyzerAssembly.GetTypes()
				.Where(IsInstantiatibleDiagnosticAnalzyer)
				.SelectMany(GetDescriptorFromCreatedAnalyzerInstance)
				.ToHashSet();

foreach (var group in descriptors.Where(descriptor => !String.IsNullOrEmpty(descriptor.Description.ToString())).GroupBy(descriptor => descriptor))
	Console.WriteLine($"{group.Key.Id} : {String.Concat(group.Select(descriptor => descriptor.Description))}");

static bool IsInstantiatibleDiagnosticAnalzyer(Type type)
			=> type.IsSubclassOf(typeof(DiagnosticAnalyzer)) && !type.IsAbstract;

static IEnumerable<DiagnosticDescriptor> GetDescriptorFromCreatedAnalyzerInstance(Type type)
{
	var analyzer = Activator.CreateInstance(type) as DiagnosticAnalyzer;

	return analyzer is not null ?
		(IEnumerable<DiagnosticDescriptor>)analyzer.SupportedDiagnostics
		: throw new Exception($"{nameof(analyzer)} is null");
}