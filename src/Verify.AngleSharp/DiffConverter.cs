static class DiffConverter
{
    public static void Append(IDiff diff, StringBuilder builder)
    {
        switch (diff)
        {
            case NodeDiff(var verified, var received, _):
            {
                builder.AppendLine(
                    $"""
                      * Node Diff
                        Path: {received.Path}
                        Received: {received.Node.NodeValue}
                        Verified: {verified.Node.NodeValue}
                     """);
                return;
            }
            case AttrDiff(var verified, var received, _):
            {
                builder.AppendLine(
                    $"""
                      * Attribute Diff
                        Path: {received.Path}
                        Name: {received.Attribute.Name}
                        Received: {received.Attribute.Value}
                        Verified: {verified.Attribute.Value}
                     """);
                return;
            }
            case UnexpectedAttrDiff unexpectedAttribute:
            {
                var source = unexpectedAttribute.Test;
                builder.AppendLine(
                    $"""
                      * Unexpected Attribute
                        Path: {source.Path}
                        Name: {source.Attribute.Name}
                        Value: {source.Attribute.Value}
                     """);
                return;
            }
            case UnexpectedNodeDiff unexpectedNode:
            {
                var source = unexpectedNode.Test;
                builder.AppendLine(
                    $"""
                      * Unexpected Node
                        Path: {source.Path}
                        Name: {source.Node.NodeName}
                        Value: {source.Node.NodeValue}
                     """);
                return;
            }
            case MissingAttrDiff missingAttribute:
            {
                var source = missingAttribute.Control;
                builder.AppendLine(
                    $"""
                      * Missing Attribute
                        Path: {source.Path}
                        Name: {source.Attribute.Name}
                        Value: {source.Attribute.Value}
                     """);
                return;
            }
            case MissingNodeDiff missingNode:
            {
                var source = missingNode.Control;
                builder.AppendLine(
                    $"""
                      * Missing Node
                        Path: {source.Path}
                        Name: {source.Node.NodeName}
                        Value: {source.Node.NodeValue}
                     """);
                return;
            }
            default:
                throw new($"Unknown diff: {diff.GetType()}");
        }
    }
}