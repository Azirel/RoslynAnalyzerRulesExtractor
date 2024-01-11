using Microsoft.CodeAnalysis;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ObjectiveC;
using System.Collections.Immutable;
using System.Reflection.Metadata;
//using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynAnalyzerRulesExtractor
{
	public static class AnalyzerRulesExtractor
	{
		private static IReadOnlyList<string>? potentialDependencies;

		public static ISet<Type> GetAnalyzerRules(string analyzerAssemblyPath, params string[] dependencies)
		{
			potentialDependencies = dependencies;
			AppDomain.CurrentDomain.AssemblyResolve += ResolveDependency;
			//LoadMainDependencies();
			//Assembly.LoadFile(@"C:\Temp\microsoft.codeanalysis.common.3.3.0\lib\netstandard2.0\Microsoft.CodeAnalysis.dll");
			var analyzerAssembly = Assembly.LoadFile(analyzerAssemblyPath);
			AppDomain.CurrentDomain.AssemblyResolve -= ResolveDependency;
			Type[] types;
			try
			{
				types = analyzerAssembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				types = ex.Types.Where(t => t != null).ToArray();
			}
			//var types = analyzerAssembly.GetTypes()
			//	.Where(IsInstantiatibleDiagnosticAnalzyerByName)
			//	.SelectMany(GetFieldAsIEnumerable)
			//	.ToList();

			var loadedTypes = types?.Where(IsInstantiatibleDiagnosticAnalzyerByName)
				//.SelectMany(GetFieldAsIEnumerable)
				.ToList();

			return analyzerAssembly.GetTypes()
				.Where(IsInstantiatibleDiagnosticAnalzyer)
				//.SelectMany(GetDescriptorFromCreatedAnalyzerInstance)
				.ToHashSet();
		}

		private static void LoadMainDependencies()
		{
			//var dependenciesDirectoryPath = Path.GetFullPath(@"..\..\Dependencies");
			var dependenciesDirectoryPath = Path.GetFullPath(@"C:\Repos\RulesExtractor\RulesExtracotrFormRoslynDll\Dependencies");
			var dllPaths = Directory.GetFiles(dependenciesDirectoryPath, @"*.dll");
			foreach (var dllPath in dllPaths)
				try { Assembly.LoadFile(dllPath); }
				catch (Exception ex)
				{
					_ = dllPath;
				}
		}

		private static Assembly? ResolveDependency(object? sender, ResolveEventArgs args)
		{
			var matchingDependency = potentialDependencies!
				.FirstOrDefault(dep => string.Equals(AssemblyName.GetAssemblyName(dep).FullName, args.Name, StringComparison.Ordinal));
			return matchingDependency != null ? Assembly.LoadFrom(matchingDependency) : null;
		}

		private static bool IsInstantiatibleDiagnosticAnalzyer(Type type)
			=> type.IsSubclassOf(typeof(DiagnosticAnalyzer)) && !type.IsAbstract;

		private static bool IsInstantiatibleDiagnosticAnalzyerByName(Type type)
			=> InheritsFromClass(type, "DiagnosticAnalyzer") && !type.IsAbstract;

		public static bool InheritsFromClass(Type type, string className)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (string.IsNullOrEmpty(className))
				throw new ArgumentException("Class name cannot be null or empty.", nameof(className));

			var currentType = type.BaseType;
			while (currentType != null)
			{
				if (currentType.Name == className || currentType.FullName == className)
					return true;
				currentType = currentType.BaseType;
			}

			return false;
		}

		private static IEnumerable<object> GetDescriptorFromCreatedAnalyzerInstance(Type type)
		{
			var analyzer = Activator.CreateInstance(type)/* as DiagnosticAnalyzer*/;

			return null;
			//return analyzer is not null ?
			//	(IEnumerable<DiagnosticDescriptor>)analyzer.SupportedDiagnostics
			//	: throw new Exception($"{nameof(analyzer)} is null");
		}

		public static IEnumerable<object> GetFieldAsIEnumerable(Type type)
		{
			var fieldName = "SupportedDiagnostics";
			if (type == null)
			throw new ArgumentNullException(nameof(type));

			if (string.IsNullOrEmpty(fieldName))
				throw new ArgumentException("Field name cannot be null or empty.", nameof(fieldName));

			// Create an instance of the specified type
			var instance = Activator.CreateInstance(type);

			// Get the field using reflection
			var field = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
				throw new ArgumentException($"Field '{fieldName}' not found in type '{type}'.", nameof(fieldName));

			// Extract the value of the field
			var fieldValue = field.GetValue(instance);

			// Check if the field value is IEnumerable
			if (fieldValue is IEnumerable<object> enumerable)
				return enumerable;

			else
			{
				throw new InvalidOperationException($"The field '{fieldName}' is not of type IEnumerable.");
			}
		}
	}
}
