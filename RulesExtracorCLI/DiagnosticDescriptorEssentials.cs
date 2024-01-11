using Microsoft.CodeAnalysis;

[Serializable]
public struct DiagnosticDescriptorEssentials
{
	public readonly string Id;
	public readonly string Title;
	public readonly string Description;
	public readonly string HelpLinkUri;
	public readonly string Category;

	public DiagnosticDescriptorEssentials(string id, string title, string description, string helpLinkUri, string category)
	{
		Id = id;
		Title = title;
		Description = description;
		HelpLinkUri = helpLinkUri;
		Category = category;
	}

	public static implicit operator DiagnosticDescriptorEssentials(DiagnosticDescriptor source)
		=> new(source.Id, source.Title.ToString(), source.Description.ToString(), source.HelpLinkUri, source.Category);

	public static DiagnosticDescriptorEssentials Merge(IGrouping<string, DiagnosticDescriptorEssentials> descriptors)
	{
		if (descriptors is null)
			throw new ArgumentNullException(nameof(descriptors));
		if (!descriptors.Any())
			throw new ArgumentException("");
		var first = descriptors.First();
		return new(descriptors.Key, first.Title, JoinDescriptions(descriptors), first.HelpLinkUri, first.Category);
	}

	private static string JoinDescriptions(IEnumerable<DiagnosticDescriptorEssentials> descriptions)
		=> String.Join('\n', descriptions.Select(descriptor => descriptor.Description));
}
