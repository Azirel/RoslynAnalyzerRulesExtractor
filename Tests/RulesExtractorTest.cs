using RoslynAnalyzerRulesExtractor;
using Tests;

namespace RulesExtractorTests
{
	[TestClass]
	public class RulesExtractorTest
	{
		[TestMethod]
		public void CodeQualityAnalyzerTest()
		{
			var codeQualitydllPath = Path.GetFullPath($"{nameof(DllPathsResource.MicrosoftCodeQualityAnalyzers)}.dll");
			File.WriteAllBytes(codeQualitydllPath, DllPathsResource.MicrosoftCodeQualityAnalyzers);

			var codeQualityCsharpdllPath = Path.GetFullPath($"{nameof(DllPathsResource.MicrosoftCodeQualityCSharpAnalyzers)}.dll");
			File.WriteAllBytes(codeQualityCsharpdllPath, DllPathsResource.MicrosoftCodeQualityCSharpAnalyzers);

			var rules = AnalyzerRulesExtractor.GetAnalyzerRules(codeQualitydllPath);

			Assert.IsNotNull(rules);
			Assert.IsTrue(rules.Any());
		}

		[TestMethod]
		public void UnityAnalyzerTest()
		{
			var unityAnalyzersDll = Path.GetFullPath($"{nameof(DllPathsResource.MicrosoftUnityAnalyzers)}.dll");
			File.WriteAllBytes(unityAnalyzersDll, DllPathsResource.MicrosoftUnityAnalyzers);
			var rules = AnalyzerRulesExtractor.GetAnalyzerRules(unityAnalyzersDll);
			Assert.IsNotNull(rules);
			Assert.IsTrue(rules.Any());
		}

		[TestMethod]
		public void MeziantouAnalyzerTest()
		{
			var meziantou = Path.GetFullPath($"{nameof(DllPathsResource.MeziantouAnalyzer)}.dll");
			File.WriteAllBytes(meziantou, DllPathsResource.MeziantouAnalyzer);
			var rules = AnalyzerRulesExtractor.GetAnalyzerRules(meziantou);
			Assert.IsNotNull(rules);
			Assert.IsTrue(rules.Any());
		}
	}
}