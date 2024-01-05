using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RoslynAnalyzerRulesExtractor
{
	public static class AnalyzerRulesExtractor
	{
		public static void Main(string[] _) { }

		private static IReadOnlyList<string>? potentialDependencies;

		public static ISet<DiagnosticDescriptor> GetAnalyzerRules(string analyzerAssemblyPath, params string[] dependencies)
		{
			potentialDependencies = dependencies;
			AppDomain.CurrentDomain.AssemblyResolve += ResolveDependency;
			var analyzerAssembly = Assembly.LoadFrom(analyzerAssemblyPath);
			AppDomain.CurrentDomain.AssemblyResolve -= ResolveDependency;
			return analyzerAssembly.GetTypes()
				.Where(IsInstantiatibleDiagnosticAnalzyer)
				.SelectMany(GetDescriptorFromCreatedAnalyzerInstance)
				.ToHashSet();
		}

		private static Assembly? ResolveDependency(object? sender, ResolveEventArgs args)
		{
			var matchingDependency = potentialDependencies!
				.FirstOrDefault(dep => string.Equals(AssemblyName.GetAssemblyName(dep).FullName, args.Name, StringComparison.Ordinal));
			return matchingDependency != null ? Assembly.LoadFrom(matchingDependency) : null;
		}

		private static bool IsInstantiatibleDiagnosticAnalzyer(Type type)
			=> type.IsSubclassOf(typeof(DiagnosticAnalyzer)) && !type.IsAbstract;

		private static IEnumerable<DiagnosticDescriptor> GetDescriptorFromCreatedAnalyzerInstance(Type type)
		{
			var analyzer = Activator.CreateInstance(type) as DiagnosticAnalyzer;
			return analyzer is not null ?
				(IEnumerable<DiagnosticDescriptor>)analyzer.SupportedDiagnostics
				: throw new Exception($"{nameof(analyzer)} is null");
		}
	}
}
