using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Diagnostics
{
	/// <summary>
	/// The base type for diagnostic analyzers.
	/// </summary>
	public abstract class DiagnosticAnalyzer
	{
		/// <summary>
		/// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
		/// </summary>
		public abstract ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

		/// <summary>
		/// Called once at session start to register actions in the analysis context.
		/// </summary>
		/// <param name="context"></param>
		public abstract void Initialize(AnalysisContext context);

		public sealed override bool Equals(object? obj)
		{
			return (object?)this == obj;
		}

		public sealed override int GetHashCode() => base.GetHashCode();

		public sealed override string ToString()
		{
			return this.GetType().ToString();
		}
	}
}
