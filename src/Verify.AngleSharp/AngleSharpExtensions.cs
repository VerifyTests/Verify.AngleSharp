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