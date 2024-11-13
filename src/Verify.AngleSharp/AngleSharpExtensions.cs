namespace VerifyTests.AngleSharp;

public static class AngleSharpExtensions
{
    public static IEnumerable<TNode> DescendantsAndSelf<TNode>(this INodeList nodes)
        where TNode : INode =>
        nodes.DescendantsAndSelf().OfType<TNode>();

    public static IEnumerable<INode> DescendantsAndSelf(this INodeList nodes)
    {
        foreach (var node in nodes.ToList())
        {
            yield return node;
            foreach (var element in node.DescendantsAndSelf())
            {
                yield return element;
            }
        }
    }

    public static IEnumerable<TNode> Descendants<TNode>(this INodeList nodes)
        where TNode : INode =>
        nodes.Descendants().OfType<TNode>();

    public static IEnumerable<TNode> Descendants<TNode>(this IEnumerable<INode> elements)
        where TNode : INode =>
        elements.SelectMany(_ => _.Descendants<TNode>());

    public static IEnumerable<INode> Descendants(this IEnumerable<INode> elements) =>
        elements.SelectMany(_ => _.Descendants());

    public static IEnumerable<TNode> DescendantsAndSelf<TNode>(this IEnumerable<INode> elements)
        where TNode : INode =>
        elements.SelectMany(_ => _.DescendantsAndSelf<TNode>());

    public static IEnumerable<INode> DescendantsAndSelf(this IEnumerable<INode> elements) =>
        elements.SelectMany(_ => _.DescendantsAndSelf());

    public static IEnumerable<INode> Descendants(this INodeList nodes)
    {
        foreach (var node in nodes.ToList())
        {
            foreach (var element in node.DescendantsAndSelf())
            {
                yield return element;
            }
        }
    }
}