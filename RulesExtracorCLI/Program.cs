using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

if (args?.Any() == false)
	throw new ArgumentNullException("No file path specified");

var dllPath = args?[0];
if (!File.Exists(dllPath))
	throw new FileNotFoundException(dllPath);

foreach (var dependencyPath in args?.Skip(1))
	Assembly.LoadFrom(dependencyPath);

var analyzerAssembly = Assembly.LoadFrom(dllPath);

var descriptors = analyzerAssembly.GetTypes()
	.Where(IsInstantiatibleDiagnosticAnalzyer)
	.SelectMany(GetDescriptorFromCreatedAnalyzerInstance)
	.Select(diagnosticDescriptor => (DiagnosticDescriptorEssentials)diagnosticDescriptor)
	.GroupBy(descriptor => descriptor.Id)
	.Select(DiagnosticDescriptorEssentials.Merge)
	.OrderBy(item => item.Id)
	.ToList();

if ((descriptors is not null) && descriptors.Any())
	Serialize(descriptors, Console.OpenStandardOutput());

static void Serialize(object value, Stream s)
{
	using var writer = new StreamWriter(s);
	using var jsonWriter = new JsonTextWriter(writer);
	var ser = new JsonSerializer();
	ser.Serialize(jsonWriter, value);
	jsonWriter.Flush();
}

static bool IsInstantiatibleDiagnosticAnalzyer(Type type)
			=> type.IsSubclassOf(typeof(DiagnosticAnalyzer)) && !type.IsAbstract;

static IEnumerable<DiagnosticDescriptor> GetDescriptorFromCreatedAnalyzerInstance(Type type)
{
	var analyzer = Activator.CreateInstance(type) as DiagnosticAnalyzer;

	return analyzer is not null ?
		(IEnumerable<DiagnosticDescriptor>)analyzer.SupportedDiagnostics
		: throw new Exception($"{nameof(analyzer)} is null");
}
