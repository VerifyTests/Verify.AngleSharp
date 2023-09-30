namespace VerifyTests.AngleSharp;

public static class AngleSharpExtensions
{
    public static IEnumerable<TNode> DescendantsAndSelf<TNode>(this INodeList nodes) =>
        nodes.DescendantsAndSelf().OfType<TNode>();

    public static IEnumerable<INode> DescendantsAndSelf(this INodeList nodes)
    {
        foreach (var node in nodes.ToList())
        {
            yield return node;
            foreach (var element in node.DescendentsAndSelf())
            {
                yield return element;
            }
        }
    }

    public static IEnumerable<TNode> Descendants<TNode>(this INodeList nodes) =>
        nodes.Descendants().OfType<TNode>();

    public static IEnumerable<TNode> Descendants<TNode>(this IEnumerable<INode> elements) =>
        elements.SelectMany(_ => _.Descendents<TNode>());

    public static IEnumerable<INode> Descendants(this IEnumerable<INode> elements) =>
        elements.SelectMany(_ => _.Descendents());

    public static IEnumerable<TNode> DescendantsAndSelf<TNode>(this IEnumerable<INode> elements) =>
        elements.SelectMany(_ => _.DescendentsAndSelf<TNode>());

    public static IEnumerable<INode> DescendantsAndSelf(this IEnumerable<INode> elements) =>
        elements.SelectMany(_ => _.DescendentsAndSelf());

    public static IEnumerable<INode> Descendants(this INodeList nodes)
    {
        foreach (var node in nodes.ToList())
        {
            foreach (var element in node.DescendentsAndSelf())
            {
                yield return element;
            }
        }
    }
}