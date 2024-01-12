using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

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
	.Select(diagnosticDescriptor => (DiagnosticDescriptorEssentials)diagnosticDescriptor)
	.GroupBy(descriptor => descriptor.Id)
	.Select(group => DiagnosticDescriptorEssentials.Merge(group))
	.OrderBy(item => item.Id)
	.ToList();

var resultJson = JsonConvert.SerializeObject(descriptors, Formatting.Indented);
Console.Out.Write(resultJson);

Console.Out.Flush();

static bool IsInstantiatibleDiagnosticAnalzyer(Type type)
			=> type.IsSubclassOf(typeof(DiagnosticAnalyzer)) && !type.IsAbstract;

static IEnumerable<DiagnosticDescriptor> GetDescriptorFromCreatedAnalyzerInstance(Type type)
{
	var analyzer = Activator.CreateInstance(type) as DiagnosticAnalyzer;

	return analyzer is not null ?
		(IEnumerable<DiagnosticDescriptor>)analyzer.SupportedDiagnostics
		: throw new Exception($"{nameof(analyzer)} is null");
}
