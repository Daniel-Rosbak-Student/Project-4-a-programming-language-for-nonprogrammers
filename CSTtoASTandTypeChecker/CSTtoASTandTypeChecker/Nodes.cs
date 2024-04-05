namespace CSTtoASTandTypeChecker;

internal abstract class Node
{
    public abstract TypeNode typeCheck();
}
//----------------------------------------Fatma---------------------------------------------
internal abstract class TypeNode : Node
{
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
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
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
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
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}
//----------------------------------------Daniel---------------------------------------------
internal class InputNode : InFixNode
{
    public InputNode(Node x, Node y)
    {
        left = x;
        right = y;
    }
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
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
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
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
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
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
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}
//----------------------------------------Armin---------------------------------------------
internal class ReadNode : Node
{
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class PrintNode : PreSufFixNode
{
    public PrintNode(Node input)
    {
        node = input;
    }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class LengthOfNode : PreSufFixNode
{
    public IdentifierNode Identifier { get; set; }

    public LengthOfNode(Node x)
    {
        Identifier = (IdentifierNode)x;
    }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class TypeConvertNode : PreSufFixNode
{
    public Node value { get; set; }
    public TypeNode type { get; set; }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}
//----------------------------------------Niklas---------------------------------------------
internal class CommandNode : InFixNode
{
    public CommandNode(Node x, Node y)
    {
        left = x;
        right = y;
    }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class CreateVariableNode : Node
{
    public static List<CreateVariableNode> variables { get; set; }
    public IdentifierNode name { get; set; }
    public TypeNode type { get; set; }
    public Node value { get; set; }
    public CreateVariableNode(Node x, Node y, Node z)
    {
        name = (IdentifierNode)x;
        type = (TypeNode)y;
        value = z;
        addToVariables(this);
    }
    public CreateVariableNode(Node x, Node y)
    {
        name = (IdentifierNode)x;
        type = (TypeNode)y;
        addToVariables(this);
    }

    private void addToVariables(CreateVariableNode variable)
    {
        if (variableExists(variable.name.name))
        {
            throw new Exception("variable declared more than once!");
        }
        variables.Add(this);
    }

    public static bool variableExists(string name)
    {
        foreach (CreateVariableNode variable in variables)
        {
            if (variable.name.name == name)
            {
                return true;
            }
        }
        return false;
    }
    
    public static CreateVariableNode getVariable(string name)
    {
        for (int i = 0; i < variables.Count; i++)
        {
            if (variables[i].name.name == name)
            {
                return variables[i];
            }
        }

        return null;
    }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class AssignNode : InFixNode
{
    public AssignNode(Node x, Node y)
    {
        left = x;
        right = y;
    }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal abstract class NumberInFixNode : InFixNode
{
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}
//----------------------------------------Mathias---------------------------------------------
internal class AdditionNode : InFixNode
{
    //denne gælder også for tekst
    public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? left.typeCheck() : throw new Exception("Type mismatch");
    }
}

internal class SubtractNode : NumberInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? left.typeCheck() : throw new Exception("Type mismatch");
    }
}

internal class MultiplyNode : NumberInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? left.typeCheck() : throw new Exception("Type mismatch");
    }
}

internal class DivideNode : NumberInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? left.typeCheck() : throw new Exception("Type mismatch");
    }
}

internal class ModuloNode : NumberInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? left.typeCheck() : throw new Exception("Type mismatch");
    }
}

internal abstract class FlagInFixNode : InFixNode
{
    public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class EqualsNode : FlagInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class GreaterNode : FlagInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class GreaterEqualsNode : FlagInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class LessNode : FlagInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class LessEqualsNode : FlagInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class AndNode : FlagInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class OrNode : FlagInFixNode
{
        public override TypeNode typeCheck()
    {
        return left.typeCheck() == right.typeCheck() ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class NotNode : FlagInFixNode
{
    public override TypeNode typeCheck()
    {
        return left.typeCheck() is FlagTypeNode ? new FlagTypeNode() : throw new Exception("Type mismatch");
    }
}

internal class NumberTypeNode : TypeNode
{
        public override TypeNode typeCheck()
    {
        return new NumberTypeNode();
    }
}

internal class FlagTypeNode : TypeNode
{
        public override TypeNode typeCheck()
    {
        return new FlagTypeNode();
    }
}

internal class TextTypeNode : TypeNode
{
        public override TypeNode typeCheck()
    {
        return new TextTypeNode();
    }
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

internal class NumberNode : Node
{
    public NumberNode(double x)
    {
        value = x;
    }
    public double value { get; set; }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class FlagNode : Node
{
    public FlagNode(bool x)
    {
        value = x;
    }
    public bool value { get; set; }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
} }
}
//----------------------------------------Vaal---------------------------------------------
internal class TextNode : Node
{
    public TextNode(string x)
    {
        value = x;
    }
    public string value { get; set; }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
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
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class IdentifierNode : Node
{
    public string name { get; set; }
    
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class BreakNode : Node
{
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class GiveNode : Node
{
    public Node value { get; set; }

    public GiveNode(Node x)
    {
        value = x;
    }
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}