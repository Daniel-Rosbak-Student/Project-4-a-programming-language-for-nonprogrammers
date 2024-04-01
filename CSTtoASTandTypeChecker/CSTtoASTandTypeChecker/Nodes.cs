namespace CSTtoASTandTypeChecker;

internal abstract class Node
{
}

internal abstract class TypeNode : Node
{
}

internal abstract class InFixNode : Node
{
    public Node left { get; set; }
    public Node right { get; set; }
}

internal abstract class PreSufFixNode : Node
{
    public Node node { get; set; }
}

internal abstract class ControlNode : Node
{
    public Node condition { get; set; }
    public CommandNode nodes { get; set; }
}

internal class FunctionNode : Node
{
    public SignatureNode signature { get; set; }
    public CommandNode nodes { get; set; }
}

internal class UseNode : Node
{
    public string id { get; set; }
    public InputNode inputs { get; set; }

    public UseNode(string x, InputNode y)
    {
        id = x;
        inputs = y;
    }
}

internal class InputNode : InFixNode
{
    public InputNode(Node x, Node y)
    {
        left = x;
        right = y;
    }
}

internal class CommentNode : Node
{
}

internal class IfNode : ControlNode
{
}

internal class ElseNode : ControlNode
{
}

internal class RepeatNode : ControlNode
{
}

internal class ReadNode : Node
{
}

internal class PrintNode : PreSufFixNode
{
    public PrintNode(Node input)
    {
        node = input;
    }
}

internal class LengthOfNode : PreSufFixNode
{
    public Node Identifier { get; set; }

    public LengthOfNode(Node identifier)
    {
        Identifier = identifier;
    }
}

internal class TypeConvertNode : PreSufFixNode
{
    public Node value { get; set; }
    public TypeNode type { get; set; }
}

internal class CommandNode : InFixNode
{
    public CommandNode(Node x, Node y)
    {
        left = x;
        right = y;
    }
}

internal class CreateVariableNode : InFixNode
{
}

internal class AssignNode : InFixNode
{
}

internal class AdditionNode : InFixNode
{
}

internal class SubtractNode : InFixNode
{
}

internal class MultiplyNode : InFixNode
{
}

internal class DivideNode : InFixNode
{
}

internal class ModuloNode : InFixNode
{
}

internal class TextAdditionNode : InFixNode
{
}

internal class EqualsNode : InFixNode
{
}

internal class GreaterNode : InFixNode
{
}

internal class GreaterEqualsNode : InFixNode
{
}

internal class LessNode : InFixNode
{
}

internal class LessEqualsNode : InFixNode
{
}

internal class AndNode : InFixNode
{
}

internal class OrNode : InFixNode
{
}

internal class NotNode : InFixNode
{
}

internal class NumberTypeNode : TypeNode
{
}

internal class FlagTypeNode : TypeNode
{
}

internal class TextTypeNode : TypeNode
{
}

internal class ListTypeNode : TypeNode
{
    public TypeNode type { get; set; }
}

internal class SignatureNode : TypeNode
{
    public static List<SignatureNode> signatures = new List<SignatureNode>();
    public List<TypeNode> takes { get; set; }
    public TypeNode gives { get; set; }

    SignatureNode ()
    {
        signatures.Add(this);
    }
}

internal class ListNode : Node
{
    public ListTypeNode type { get; set; }
    public List<Node> values { get; set; }
}

internal class NumberNode : Node
{
    public NumberNode(double x)
    {
        value = x;
    }
    public double value { get; set; }
}

internal class FlagNode : Node
{
    public FlagNode(bool x)
    {
        value = x;
    }
    public bool value { get; set; }
}

internal class TextNode : Node
{
    public TextNode(string x)
    {
        value = x;
    }
    public string value { get; set; }
}

internal class ListElementNode : Node
{
    public string id { get; set; }
    public Node index;

    public ListElementNode(string x, Node y)
    {
        id = x;
        index = y;
    }
}

internal class IdentifierNode : Node
{
    public string name { get; set; }
    public Node value { get; set; }
    public TypeNode type { get; set; }
}

internal class BreakNode : Node
{
}