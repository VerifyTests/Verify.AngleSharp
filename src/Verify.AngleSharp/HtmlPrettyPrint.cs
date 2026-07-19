namespace VerifyTests.AngleSharp;

public static class HtmlPrettyPrint
{
    const StringComparison comparer = StringComparison.OrdinalIgnoreCase;

    const string cacheBusterReplacement = "$1{TAG_HELPER_VERSION}";

    static readonly Regex cacheBusterPattern = new(@"([^""?]+[?&]v=)[\w\-]+");

    public static void All(Action<INodeList>? action = null)
    {
        VerifierSettings.AddScrubber("html", builder => CleanSource(builder, action));
        VerifierSettings.AddScrubber("htm", builder => CleanSource(builder, action));
    }

    public static void ScrubEmptyDivs(this INodeList nodes) =>
        ScrubEmptyDivs(nodes.OfType<IElement>());

    public static void ScrubEmptyDivs(this IEnumerable<IElement> elements)
    {
        // materialized since scrubbing removes nodes from the tree being walked
        foreach (var element in elements.DescendantsAndSelf<IElement>().ToList())
        {
            TryScrubDiv(element);
        }
    }

    /// <summary>
    /// Removes <paramref name="element" /> when it is a div with no attributes and no
    /// content, or unwraps it when it wraps a single element.
    /// </summary>
    /// <returns>True when the div was removed or unwrapped.</returns>
    public static bool TryScrubDiv(this IElement element)
    {
        if (element is not IHtmlDivElement div)
        {
            return false;
        }

        TrimTrailingWhitespace(div);
        if (element.HasAttributes())
        {
            return false;
        }

        if (!element.HasChildNodes)
        {
            element.RemoveFromParent();
            return true;
        }

        if (element.Parent is { } parent &&
            TryGetOnlyElement(element, out var child))
        {
            parent.InsertBefore(child, element);
            element.RemoveFromParent();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Equivalent to trimming the end of InnerHtml, but without serializing and re-parsing
    /// the subtree. The round trip replaced every descendant node, which both invalidated
    /// nodes already collected for scrubbing and cost a full parse per div.
    /// </summary>
    static void TrimTrailingWhitespace(IElement element)
    {
        while (element.LastChild is IText text)
        {
            var trimmed = text.Data.TrimEnd();
            if (trimmed.Length != 0)
            {
                text.Data = trimmed;
                return;
            }

            element.RemoveChild(text);
        }
    }

    /// <summary>
    /// Gets the single child element of <paramref name="element" />, but only when every
    /// other child node is whitespace. Returns false when there is no element, more than
    /// one element, or any text that unwrapping would discard.
    /// </summary>
    static bool TryGetOnlyElement(IElement element, [NotNullWhen(true)] out IElement? child)
    {
        child = null;
        foreach (var node in element.ChildNodes)
        {
            if (node is IElement candidate)
            {
                if (child != null)
                {
                    child = null;
                    return false;
                }

                child = candidate;
                continue;
            }

            if (node is not IText text ||
                !string.IsNullOrWhiteSpace(text.Data))
            {
                child = null;
                return false;
            }
        }

        return child != null;
    }

    public static void ScrubAttributes(this INodeList nodes, string name) =>
        ScrubAttributes(nodes.OfType<IElement>(), name);

    public static void ScrubAttributes(this INodeList nodes, Func<IAttr, bool> match) =>
        ScrubAttributes(nodes.OfType<IElement>(), match);

    public static void ScrubAttributes(this INodeList nodes, Func<IAttr, string?> match) =>
        ScrubAttributes(nodes.OfType<IElement>(), match);

    public static void ScrubAttributes(this IEnumerable<IElement> elements, string name) =>
        elements.ScrubAttributes(_ => _.Name == name);

    public static void ScrubAttributes(this IEnumerable<IElement> elements, Func<IAttr, string?> tryGetValue)
    {
        foreach (var element in elements.DescendantsAndSelf<IElement>())
        {
            foreach (var attribute in element.Attributes)
            {
                var value = tryGetValue(attribute);
                if (value != null &&
                    !string.Equals(value, attribute.Value, StringComparison.Ordinal))
                {
                    attribute.Value = value;
                }
            }
        }
    }

    public static void ScrubAttributes(this IEnumerable<IElement> elements, Func<IAttr, bool> match)
    {
        foreach (var element in elements.DescendantsAndSelf<IElement>())
        {
            foreach (var attribute in element.Attributes.ToList())
            {
                if (match(attribute))
                {
                    element.RemoveAttribute(attribute.Name);
                }
            }
        }
    }

    public static void PrettyPrintHtml(
        this VerifySettings settings,
        Action<INodeList>? action = null)
    {
        settings.AddScrubber("html", builder => CleanSource(builder, action));
        settings.AddScrubber("htm", builder => CleanSource(builder, action));
    }

    public static SettingsTask PrettyPrintHtml(
        this SettingsTask settings,
        Action<INodeList>? action = null)
    {
        settings.AddScrubber("html", builder => CleanSource(builder, action));
        settings.AddScrubber("htm", builder => CleanSource(builder, action));
        return settings;
    }

    /// <summary>
    /// Replaces the automatic cache-busting tag generated by the asp-append-version
    /// tag helper with constant string "{TAG_HELPER_VERSION}".
    /// </summary>
    /// <param name="nodes">
    /// The <see cref="INodeList"/> containing the DOM elements.
    /// </param>
    public static void ScrubAspCacheBusterTagHelper(this INodeList nodes) =>
        nodes.OfType<IElement>().ScrubAspCacheBusterTagHelper();

    /// <summary>
    /// Replaces the automatic cache-busting tag generated by the asp-append-version
    /// tag helper with constant string "{TAG_HELPER_VERSION}".
    /// </summary>
    /// <param name="elements">
    /// The collection of  <see cref="IElement"/> containing the DOM elements.
    /// </param>
    public static void ScrubAspCacheBusterTagHelper(this IEnumerable<IElement> elements) =>
        elements.ScrubAttributes(static attr =>
        {
            if (!attr.Name.Equals("href", comparer) &&
                !attr.Name.Equals("src", comparer))
            {
                return null;
            }

            var value = attr.Value;

            // the pattern cannot match without a literal v=
            if (value.IndexOf("v=", StringComparison.Ordinal) == -1)
            {
                return null;
            }

            return cacheBusterPattern.Replace(value, cacheBusterReplacement);
        });

    /// <summary>
    /// Removes elements injected by Browser Link.
    /// </summary>
    /// <param name="nodes">
    /// The <see cref="INodeList"/> containing the DOM elements.
    /// </param>
    public static void ScrubBrowserLink(this INodeList nodes)
    {
        List<INode> nodesToRemove = [];

        foreach (var comment in nodes.DescendantsAndSelf<IComment>())
        {
            var content = comment.TextContent;
            if (content.Contains("Visual Studio Browser Link", comparer)
                || content.Contains("End Browser Link", comparer))
            {
                nodesToRemove.Add(comment);
                nodesToRemove.AddRange(CollectAdjacentWhitespace(comment));
            }
        }

        foreach (var element in nodes.DescendantsAndSelf<IElement>())
        {
            if (element.TagName.Equals("script", comparer))
            {
                var src = element.GetAttribute("src") ?? string.Empty;
                if (src.Equals("/_vs/browserLink", comparer) || src.Equals("/_framework/aspnetcore-browser-refresh.js", comparer))
                {
                    nodesToRemove.Add(element);
                    nodesToRemove.AddRange(CollectAdjacentWhitespace(element));
                }
            }
        }

        foreach (var node in nodesToRemove)
        {
            node.Parent?.RemoveChild(node);
        }
    }

    static IEnumerable<INode> CollectAdjacentWhitespace(INode node)
    {
        return Collect(node, _ => _.PreviousSibling)
            .Union(Collect(node, _ => _.NextSibling));

        static IEnumerable<INode> Collect(INode n, Func<INode, INode?> iterator)
        {
            var current = iterator(n);
            while (current is not null)
            {
                if (current is IText text && string.IsNullOrWhiteSpace(text.TextContent))
                {
                    yield return current;
                    current = iterator(current);
                }
                else
                {
                    break;
                }
            }
        }
    }

    static void CleanSource(StringBuilder builder, Action<INodeList>? action)
    {
        var source = builder.ToString();
        var document = Parse(source);
        action?.Invoke(document);

        builder.Clear();
        var formatter = new PrettyMarkupFormatter
        {
            Indentation = "  ",
            NewLine = "\n"
        };
        using var writer = new StringWriter(builder);
        document.ToHtml(writer, formatter);
    }

    static INodeList Parse(string source)
    {
        var parser = new HtmlParser();
        if (IsDocument(source))
        {
            return parser.ParseDocument(source).ChildNodes;
        }

        var dom = parser.ParseDocument("<html><body></body></html>");
        return parser.ParseFragment(source, dom.Body!);
    }

    /// <summary>
    /// Detects a full document so it is parsed as one. Parsing a document as a body
    /// fragment silently discards the html, head, and body elements.
    /// </summary>
    static bool IsDocument(string source)
    {
        var index = 0;
        while (index < source.Length &&
               char.IsWhiteSpace(source[index]))
        {
            index++;
        }

        if (Matches("<!doctype"))
        {
            return true;
        }

        if (!Matches("<html"))
        {
            return false;
        }

        // guard against matching elements like <htmlfoo>
        var next = index + "<html".Length;
        return next >= source.Length ||
               source[next] is '>' or '/' ||
               char.IsWhiteSpace(source[next]);

        bool Matches(string tag) =>
            index + tag.Length <= source.Length &&
            string.Compare(source, index, tag, 0, tag.Length, comparer) == 0;
    }
}
