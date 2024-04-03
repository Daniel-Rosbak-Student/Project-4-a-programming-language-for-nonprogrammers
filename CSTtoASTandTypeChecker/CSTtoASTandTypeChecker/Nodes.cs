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

internal class FunctionNode : Node
{
    public SignatureNode signature { get; set; }
    public Node cmds { get; set; }

    public FunctionNode(Node x, Node y)
    {
        signature = (SignatureNode)x;
        cmds = y;
    }
}

internal class UseNode : Node
{
    public IdentifierNode id { get; set; }
    public InputNode inputs { get; set; }

    public UseNode(Node x, InputNode y)
    {
        id = (IdentifierNode)x;
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

internal class ParameterNode : Node
{
    public TypeNode type { get; set; }
    public IdentifierNode id { get; set; }
    public ParameterNode next { get; set; }
    public ParameterNode(Node x, Node y, Node z)
    {
        type = (TypeNode)x;
        id = (IdentifierNode)y;
        next = (ParameterNode)z;
    }
}

internal class CommentNode : Node
{
}

internal class IfNode : Node
{
    public Node condition { get; set; }
    public Node Body { get; set; }
    public Node ElseBody { get; set; }

    public IfNode(Node x, Node y, Node z)
    {
        condition = x;
        Body = y;
        ElseBody = z;
    }
}

internal class RepeatNode : Node
{
    public Node condition { get; set; }
    public Node Body { get; set; }

    public RepeatNode(Node x, Node y)
    {
        condition = x;
        Body = y;
    }
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
    public IdentifierNode Identifier { get; set; }

    public LengthOfNode(Node x)
    {
        Identifier = (IdentifierNode)x;
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

internal class CreateVariableNode : Node
{
    public IdentifierNode name { get; set; }
    public TypeNode type { get; set; }
    public Node value { get; set; }
    public CreateVariableNode(Node x, Node y, Node z)
    {
        name = (IdentifierNode)x;
        type = (TypeNode)y;
        value = z;
    }
    public CreateVariableNode(Node x, Node y)
    {
        name = (IdentifierNode)x;
        type = (TypeNode)y;
    }
}

internal class AssignNode : InFixNode
{
    public AssignNode(Node x, Node y)
    {
        left = x;
        right = y;
    }
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

internal class NothingNode : TypeNode
{
}

internal class SignatureNode : TypeNode
{
    public IdentifierNode id { get; set; }
    public ParameterNode takes { get; set; }
    public TypeNode gives { get; set; }

    public SignatureNode (Node x, Node y, Node z)
    {
        id = (IdentifierNode)x;
        takes = (ParameterNode)y;
        gives = (TypeNode)z;
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
    public IdentifierNode id { get; set; }
    public Node index;

    public ListElementNode(Node x, Node y)
    {
        id = (IdentifierNode)x;
        index = y;
    }
}

internal class IdentifierNode : Node
{
    public string name { get; set; }
}

internal class BreakNode : Node
{
}

internal class GiveNode : Node
{
    public Node value { get; set; }

    public GiveNode(Node x)
    {
        value = x;
    }
}