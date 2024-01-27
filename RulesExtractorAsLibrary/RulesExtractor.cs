using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RulesExtractorAsLibrary
{
	public static class RulesExtractor
	{
		private static Type diagnosticType;
		private const string DiagnosticTypeName = "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer";
		private static string dependenciesPath;

		public static IEnumerable<DiagnosticDescriptorEssentials> Extract(string mainAnalyzerAssemblyPath, string dependenciesPath)
		{
			RulesExtractor.dependenciesPath = Path.GetFullPath(dependenciesPath);
			AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
			var mainAssembly = Assembly.LoadFrom(mainAnalyzerAssemblyPath);
			var types = mainAssembly.GetTypes();
			diagnosticType = Array
				.Find(types, type => IsSubClassOf(type, DiagnosticTypeName))?
				.BaseType;
			return (!(diagnosticType is null))
				? mainAssembly.GetTypes()
						.Where(IsDiagnosticAnalyzer)
						.SelectMany(GetDescriptorsFromAnalyzerType)
						.Select(descriptor => new DiagnosticDescriptorEssentials(descriptor))
						.GroupBy(descriptor => descriptor.Id)
						.Select(group => DiagnosticDescriptorEssentials.Merge(group))
						.OrderBy(item => item.Id)
						.ToList()
				: Enumerable.Empty<DiagnosticDescriptorEssentials>();
		}

		public static IEnumerable<dynamic> GetDescriptorsFromAnalyzerType(Type type)
		{
			dynamic analyzerInstance = Activator.CreateInstance(type);
			return !(analyzerInstance is null) ? (analyzerInstance.SupportedDiagnostics as IEnumerable<dynamic>) : throw new Exception($"{nameof(analyzerInstance)} is null");
		}

		private static bool IsDiagnosticAnalyzer(Type type)
			=> type?.IsSubclassOf(diagnosticType) == true && !type.IsAbstract;

		private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		{
			const string dependencyNamePattern = "^([^,]+)";
			var dependencyName = Regex.Match(args.Name, dependencyNamePattern).Value;
			var assemblyPath = Array.Find(Directory.GetFiles(dependenciesPath), path => Path.GetFileNameWithoutExtension(path) == dependencyName);
			return !String.IsNullOrEmpty(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
		}

		private static bool IsSubClassOf(Type mainType, string targetClassFullName) => !(mainType?.BaseType is null)
		&& ((mainType.BaseType.FullName == targetClassFullName
						&& !mainType.IsAbstract)
			|| IsSubClassOf(mainType.BaseType, targetClassFullName));
	}
}