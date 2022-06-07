using AngleSharp.Dom;

namespace VerifyTests.AngleSharp;

public static class AngleSharpExtensions
{
    public static IEnumerable<TNode> DescendentsAndSelf<TNode>(this INodeList nodes) =>
        nodes.DescendentsAndSelf().OfType<TNode>();

    public static IEnumerable<INode> DescendentsAndSelf(this INodeList nodes)
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

    public static IEnumerable<TNode> Descendents<TNode>(this INodeList nodes) =>
        nodes.Descendents().OfType<TNode>();

    public static IEnumerable<TNode> Descendents<TNode>(this IEnumerable<INode> elements) =>
        elements.SelectMany(x => x.Descendents<TNode>());

    public static IEnumerable<INode> Descendents(this IEnumerable<INode> elements) =>
        elements.SelectMany(x => x.Descendents());

    public static IEnumerable<TNode> DescendentsAndSelf<TNode>(this IEnumerable<INode> elements) =>
        elements.SelectMany(x => x.DescendentsAndSelf<TNode>());

    public static IEnumerable<INode> DescendentsAndSelf(this IEnumerable<INode> elements) =>
        elements.SelectMany(x => x.DescendentsAndSelf());

    public static IEnumerable<INode> Descendents(this INodeList nodes)
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