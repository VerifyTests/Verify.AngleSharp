namespace VerifyTests.AngleSharp;

public static class HtmlPrettyPrint
{
    const StringComparison comparer = StringComparison.OrdinalIgnoreCase;

    const string cacheBusterReplacement = "$1{TAG_HELPER_VERSION}";

    static readonly Regex cacheBusterPattern = new(@"([^""?]+[?&]v=)[\w\-]+");

    /// <summary>
    /// Elements rendered as white-space:pre, where the text is content rather than layout: indenting
    /// inside one rewrites what the document says rather than how it reads. Script and style are not
    /// here — the formatter already emits their raw-text bodies as they stand.
    /// </summary>
    static readonly string[] preformatted =
    [
        "pre",
        "textarea",
        "listing",
        "plaintext"
    ];

    /// <summary>
    /// Stands in for a preformatted element while the document is formatted around it. Letters and
    /// digits only, so it passes through text escaping as written.
    /// </summary>
    const string preformattedPlaceholder = "VerifyAngleSharpPreformatted";

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

        // Lifted out before formatting and put back after: the formatter indents the content of
        // every element it walks, which for these rewrites the content rather than the layout.
        var preserved = Detach(document);

        builder.Clear();
        var formatter = new PrettyMarkupFormatter
        {
            Indentation = "  ",
            NewLine = "\n"
        };
        using (var writer = new StringWriter(builder))
        {
            document.ToHtml(writer, formatter);
        }

        TrimLeadingNewLine(builder);

        for (var index = 0; index < preserved.Count; index++)
        {
            builder.Replace(Placeholder(index), preserved[index]);
        }
    }

    /// <summary>
    /// The formatter breaks the line before every node that sits inside an element, to part it from
    /// whatever came before. A fragment is parsed against a context element that is never itself
    /// written, so its first node takes that break with nothing in front of it and the markup opens on
    /// the second line. A document has no such context, and a fragment opening with text is not broken
    /// before either, so in those cases there is nothing here to remove.
    /// </summary>
    static void TrimLeadingNewLine(StringBuilder builder)
    {
        if (builder.Length > 0 &&
            builder[0] == '\n')
        {
            builder.Remove(0, 1);
        }
    }

    /// <summary>
    /// Swaps the content of each preformatted element for a placeholder, returning the markup each
    /// stood for. The element keeps its place, so the document is laid out exactly as it would have
    /// been; only what is inside is held back, to go in again untouched.
    /// </summary>
    static List<string> Detach(INodeList nodes)
    {
        var preserved = new List<string>();
        foreach (var element in nodes.DescendantsAndSelf<IElement>())
        {
            // The outermost of a nest is enough: its content is taken whole, inner ones included.
            if (!IsPreformatted(element) ||
                InsidePreformatted(element) ||
                !element.HasChildNodes)
            {
                continue;
            }

            preserved.Add(element.InnerHtml);
            element.TextContent = Placeholder(preserved.Count - 1);
        }

        return preserved;
    }

    static string Placeholder(int index) =>
        $"{preformattedPlaceholder}{index}";

    static bool IsPreformatted(IElement element) =>
        preformatted.Contains(element.LocalName, StringComparer.OrdinalIgnoreCase);

    static bool InsidePreformatted(IElement element)
    {
        for (var parent = element.ParentElement; parent is not null; parent = parent.ParentElement)
        {
            if (IsPreformatted(parent))
            {
                return true;
            }
        }

        return false;
    }

    static INodeList Parse(string source)
    {
        var parser = new HtmlParser();
        if (IsDocument(source))
        {
            return parser.ParseDocument(source).ChildNodes;
        }

        // an empty document still yields html, head, and body, so it serves as a
        // fragment context for less work than parsing the markup for them
        var dom = parser.ParseDocument(string.Empty);
        return parser.ParseFragment(source, FragmentContext(dom, source));
    }

    /// <summary>
    /// The element a fragment is parsed against. Body suits almost everything, but the table tags are
    /// only recognised inside a table: against body the tree construction rules discard them and keep
    /// their text, so a fragment of table rows would come back with every tag gone.
    /// </summary>
    static IElement FragmentContext(IHtmlDocument dom, string source)
    {
        var context = FirstTag(source) switch
        {
            "caption" or "colgroup" or "tbody" or "tfoot" or "thead" => "table",
            // A row against a table would gain the tbody the parser implies around it, so the section
            // is the context rather than the table itself.
            "tr" => "tbody",
            "td" or "th" => "tr",
            "col" => "colgroup",
            _ => null
        };

        if (context is null)
        {
            return dom.Body!;
        }

        return dom.CreateElement(context);
    }

    /// <summary>
    /// The name of the first element the source opens, lowercased, or null when it opens none.
    /// Anything that is not an element start is passed over: a comment, a doctype, or a stray angle
    /// bracket in text. A fragment captured from a rendered page routinely leads with a comment — a
    /// framework's component marker, say — and the tag after it is still the one that decides how the
    /// fragment has to be parsed.
    /// </summary>
    static string? FirstTag(string source)
    {
        for (var start = source.IndexOf('<'); start != -1; start = source.IndexOf('<', start + 1))
        {
            var index = start + 1;
            if (index == source.Length ||
                !char.IsLetter(source[index]))
            {
                continue;
            }

            var end = index;
            while (end < source.Length &&
                   char.IsLetterOrDigit(source[end]))
            {
                end++;
            }

            return source
                .Substring(index, end - index)
                .ToLowerInvariant();
        }

        return null;
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
