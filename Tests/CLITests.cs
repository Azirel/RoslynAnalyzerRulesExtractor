using System;
using System.Diagnostics;
using System.Reflection.Metadata;

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
			var outputText = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputText));
		}

		[TestMethod]
		public void CodeQualityCSharpTest()
		{
			var exePath = Path.GetFullPath(Resources.ExtractorExePath);
			var parameter = Path.GetFullPath(Resources.CodeQualityCSharpDllPath);
			var outputText = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputText));
		}

		[TestMethod]
		public void UnityAnalyzersTest()
		{
			var exePath = Path.GetFullPath(Resources.ExtractorExePath);
			var parameter = Path.GetFullPath(Resources.UnityAnalyzersPath);
			var outputText = StartProcessAndGetOutput(exePath, parameter);
			Assert.IsFalse(String.IsNullOrEmpty(outputText));
		}
	}
}
