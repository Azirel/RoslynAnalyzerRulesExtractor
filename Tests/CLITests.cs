using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using Newtonsoft.Json;

namespace Tests
{
	[TestClass]
	public class CLITests
	{
		private static string StartProcessAndGetOutput(string exePath, string arguments)
		{
			using var extractorProcess = new Process();
			extractorProcess.StartInfo.UseShellExecute = false;
			extractorProcess.StartInfo.RedirectStandardOutput = true;
			extractorProcess.StartInfo.FileName = exePath;
			extractorProcess.StartInfo.Arguments = arguments;
			extractorProcess.Start();
			using var output = extractorProcess.StandardOutput;
			var outputText = output.ReadToEnd();
			extractorProcess.WaitForExit();
			return outputText;
		}

		[TestMethod]
		public void CodeQualityTest()
		{
			var exePath = Path.GetFullPath(Resources.ExtractorExePath);
			var parameter = Path.GetFullPath(Resources.CodeQualityDllPath);
			var outputJson = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputJson));
			var descriptors = JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(outputJson);
			Assert.IsTrue(descriptors?.Any());
			Assert.IsTrue(descriptors?.Count() == 91);
		}

		[TestMethod]
		public void CodeQualityCSharpTest()
		{
			var exePath = Path.GetFullPath(Resources.ExtractorExePath);
			var parameter = Path.GetFullPath(Resources.CodeQualityCSharpDllPath);
			var outputJson = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputJson));
			var descriptors = JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(outputJson);
			Assert.IsTrue(descriptors?.Any());
			Assert.IsTrue(descriptors?.Count() == 8);
		}

		[TestMethod]
		public void UnityAnalyzersTest()
		{
			var exePath = Path.GetFullPath(Resources.ExtractorExePath);
			var parameter = Path.GetFullPath(Resources.UnityAnalyzersPath);
			var outputJson = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputJson));
			var descriptors = JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(outputJson);
			Assert.IsTrue(descriptors?.Any());
			Assert.IsTrue(descriptors?.Count() == 35);
		}

		//[TestMethod]
		//public void PublishedExeTest()
		//{
		//	var exePath = Path.GetFullPath(Resources.ReleasePath);
		//	var parameter = Path.GetFullPath(Resources.UnityAnalyzersPath);
		//	var outputJson = StartProcessAndGetOutput(exePath, parameter);
		//	Assert.IsFalse(String.IsNullOrEmpty(outputJson));
		//	var descriptors = JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(outputJson);
		//	Assert.IsTrue(descriptors?.Any());
		//	Assert.IsTrue(descriptors?.Count() == 35);
		//}

		[TestMethod]
		public void MaroonTest()
		{
			var exePath = Path.GetFullPath(Resources.ExtractorExePath);
			var parameter = Path.GetFullPath(Resources.MaroontressOxbind);
			var outputJson = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputJson));
			var descriptors = JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(outputJson);
			Assert.IsTrue(descriptors?.Any());
		}

		[TestMethod]
		public void CodeStyle()
		{
			var exePath = Path.GetFullPath(Resources.ExtractorExePath);
			var parameter = Path.GetFullPath(Resources.CodeStyle);
			var outputJson = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputJson));
			var descriptors = JsonConvert.DeserializeObject<IEnumerable<DiagnosticDescriptorEssentials>>(outputJson);
			Assert.IsTrue(descriptors?.Any());
		}
	}
}
