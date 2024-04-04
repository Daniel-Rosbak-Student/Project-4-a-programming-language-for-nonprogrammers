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
        return left.typeCheck();
        
        //Hvad skal der gøres her, vi skal returne typen fra alle inputs
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
        return type;
        
        //Hvad skal der gøres her, vi skal returne typen fra alle parametre
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
        condition.typeCheck();
        Body.typeCheck();
        ElseBody.typeCheck();
        return null;
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
        condition.typeCheck();
        Body.typeCheck();
        return null;
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
        left.typeCheck();
        right.typeCheck();

        return null;
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
        TypeNode variableValue = value.typeCheck();

        if (type.GetType() == value.GetType())
        {
            return type;
        }

        throw new Exception("Bad typing in create, attempting to assign a " + variableValue.GetType() + " to a " + type.GetType() + ".");
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
        TypeNode leftType = left.typeCheck();
        TypeNode rightType = right.typeCheck();

        if (leftType.GetType() == rightType.GetType())
        {
            return leftType;
        }

        throw new Exception("Bad typing in Assignment, attempting to assign a " + rightType + " to a " + leftType);
    }
}
internal abstract class NumberInFixNode : InFixNode
{
    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck();
        TypeNode rightType = right.typeCheck();

        if (leftType.GetType() == rightType.GetType() && leftType.GetType() == typeof(NumberTypeNode))
        {
            return leftType;
        }

            throw new Exception("Bad typing in NumberInfixExpression, attempting to perform invalid operations with a " + leftType.GetType() + " on a " + rightType.GetType() + ".");
    }
}
//----------------------------------------Mathias---------------------------------------------
internal class AdditionNode : InFixNode
{
    //denne gælder også for tekst
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class SubtractNode : NumberInFixNode
{
}

internal class MultiplyNode : NumberInFixNode
{
}

internal class DivideNode : NumberInFixNode
{
}

internal class ModuloNode : NumberInFixNode
{
}

internal abstract class FlagInFixNode : InFixNode
{
    public override TypeNode typeCheck()
    {
        throw new NotImplementedException();
    }
}

internal class EqualsNode : FlagInFixNode
{
}

internal class GreaterNode : FlagInFixNode
{
}

internal class GreaterEqualsNode : FlagInFixNode
{
}

internal class LessNode : FlagInFixNode
{
}

internal class LessEqualsNode : FlagInFixNode
{
}

internal class AndNode : FlagInFixNode
{
}

internal class OrNode : FlagInFixNode
{
}

internal class NotNode : FlagInFixNode
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