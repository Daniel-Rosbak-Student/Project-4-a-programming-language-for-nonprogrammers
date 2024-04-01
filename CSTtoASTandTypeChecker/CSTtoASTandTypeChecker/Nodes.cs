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

internal abstract class PreFixNode : Node
{
    public Node right { get; set; }
}

internal abstract class SufFixNode : Node
{
    public Node left { get; set; }
}

internal abstract class ControlNode : Node
{
    public Node condition { get; set; }
    public List<Node> nodes { get; set; }
}

internal class FunctionNode : Node
{
    public SignatureNode signature { get; set; }
    public List<Node> nodes { get; set; }
}

internal class UseNode : Node
{
    public List<Node> parameters { get; set; }
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

internal class LengthOfNode : PreFixNode
{
}

internal class TypeConvertNode : SufFixNode
{
    public Node value { get; set; }
    public TypeNode type { get; set; }
}

internal class CommandNode : InFixNode
{
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

internal class IdentifierNode : Node
{
    public string name { get; set; }
    public Node value { get; set; }
    public TypeNode type { get; set; }
}