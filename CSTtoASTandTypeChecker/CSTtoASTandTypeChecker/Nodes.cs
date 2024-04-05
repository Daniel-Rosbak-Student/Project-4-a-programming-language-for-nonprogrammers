namespace CSTtoASTandTypeChecker;

internal class typeChecker
{
    public static void typeCheck(Node AST)
    {
        ScopeNode.typeCheckStart(AST);
        Console.WriteLine("The program is properly typed");
    }
}
internal abstract class Node
{
    public abstract TypeNode typeCheck();
}
//----------------------------------------Fatma---------------------------------------------
internal abstract class TypeNode : Node
{
    public override TypeNode typeCheck()
    {
        return this;
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

    public override TypeNode typeCheck()
    {
        if (node != null)
            node.typeCheck();
        else
        {
            throw new Exception("keyword contents does not exist");
        }
        return null;
    }
}

internal abstract class ScopeNode : Node
{
    public static ScopeVariables scope { get; set; }
    public static void typeCheckStart(Node AST)
    {
        scope = new ScopeVariables();
        AST.typeCheck();
    }

    public static void addScope()
    {
        scope = new ScopeVariables(scope);
    }

    public static void removeScope()
    {
        scope = scope.upperScopes;
    }
}

internal class FunctionNode : Node
{
    private static List<FunctionNode> functions = new List<FunctionNode>();
    public SignatureNode signature { get; set; }
    public Node cmds { get; set; }

    public FunctionNode(Node x, Node y)
    {
        signature = (SignatureNode)x;
        cmds = y;
        addToListOfFunctions(this);
    }

    private void addToListOfFunctions(FunctionNode function)
    {
        if (functionExists(function.signature.id.name))
        {
            throw new Exception("Function with identifier: " + function.signature.id.name + " is declared twice");
        }
        functions.Add(function);
    }

    public static bool functionExists(string name)
    {
        foreach (FunctionNode func in functions)
        {
            if (name == func.signature.id.name)
            {
                return true;
            }
        }

        return false;
    }

    public static SignatureNode getSignature(string name)
    {
        foreach (FunctionNode func in functions)
        {
            if (name == func.signature.id.name)
            {
                return func.signature;
            }
        }
        throw new Exception("Signature does not exist");
    }
    
    public override TypeNode typeCheck()
    {
        ScopeNode.addScope();
        // Type check  signature og commands hvis der er behov
        if (signature != null)
        {
            signature.typeCheck();
        }
        else
        {
            throw new Exception("Signature does not exist");
        }

        if (cmds != null)
        {
            cmds.typeCheck();
        }
        else
        {
            throw new Exception("commands does not exist for function: " + signature.id.name);
        }
        ScopeNode.removeScope();
        return signature.gives;
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
        SignatureNode signature;
        if (FunctionNode.functionExists(id.name))
        {
            signature = FunctionNode.getSignature(id.name);
            ListOfTypes parameters = (ListOfTypes)signature.takes.typeCheck();
            ListOfTypes input = (ListOfTypes)inputs.typeCheck();
            List<TypeNode> paramList = parameters.getList();
            List<TypeNode> inputList = input.getList();
            if (paramList.Count == inputList.Count)
            {
                for (int i = 0; i < inputList.Count(); i++)
                {
                    if (inputList[i].GetType() != paramList[i].GetType())
                    {
                        throw new Exception("Invalid type of input in use call for function named: " + id.name);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid amount of inputs in use call for function named: " + id.name);
            }
        }
        else
        {
            throw new Exception("Use call: " + id.name + " unsuccessful as no such function exists");
        }
        return signature.gives;
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
        List<TypeNode> list = new List<TypeNode>();
        list.Add(left.typeCheck());
        if (right.GetType() == typeof(ListOfTypes))
        {
            ListOfTypes types = (ListOfTypes)right.typeCheck();
            foreach (TypeNode type in types.getList())
            {
                list.Add(type);
            }
        }
        else
        {
            list.Add(right.typeCheck());
        }
        
        return new ListOfTypes(list);
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
        new CreateVariableNode(id, type).addToVariables();
        
        List<TypeNode> list = new List<TypeNode>();
        list.Add(type.typeCheck());
        if (next != null)
        {
            ListOfTypes types = (ListOfTypes)next.typeCheck();
            foreach (TypeNode type in types.getList())
            {
                list.Add(type);
            }
        }
        
        return new ListOfTypes(list);
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
        ScopeNode.addScope();
        Body.typeCheck();
        ScopeNode.removeScope();
        ScopeNode.addScope();
        if (ElseBody != null)
        {
            ElseBody.typeCheck();
        }
        ScopeNode.removeScope();
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
        ScopeNode.addScope();
        Body.typeCheck();
        ScopeNode.removeScope();
        return null;
    }
}
//----------------------------------------Armin---------------------------------------------
internal class ReadNode : Node
{
    public override TypeNode typeCheck()
    {
        return new TextTypeNode();
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
        if (node != null)
        {
            node.typeCheck();
        }
        else
        {
            throw new Exception("Print statement missing body");
        }
        return null;
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
        if (Identifier != null)
        {
            return Identifier.typeCheck();
        }

        return new NumberTypeNode();
    }
}

internal class TypeConvertNode : PreSufFixNode
{
    public Node value { get; set; } // The value the user wants to convert to
    public TypeNode type { get; set; }

    public override TypeNode typeCheck()
    {
        TypeNode typeValue = value.typeCheck(); // Conversition happens here

        return type;
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
        if (left != null)
        {
            left.typeCheck();
        }

        if (right != null)
        {
            right.typeCheck();
        }
        
        return null;
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

    public void addToVariables()
    {
        if (variableExists(name.name))
        {
            throw new Exception("variable declared more than once!");
        }
        ScopeNode.scope.Add(this);
    }

    public static bool variableExists(string name)
    {
        foreach (CreateVariableNode variable in ScopeNode.scope.variables)
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
        for (int i = 0; i < ScopeNode.scope.variables.Count; i++)
        {
            if (ScopeNode.scope.variables[i].name.name == name)
            {
                return ScopeNode.scope.variables[i];
            }
        }

        return null;
    }

    public override TypeNode typeCheck()
    {
        addToVariables();

        TypeNode variableValue = null;
        if (value != null)
        {
            variableValue = value.typeCheck();
            
            if (type.GetType() == variableValue.GetType())
            {
                return type;
            }
        }

        throw new Exception("Bad typing in create, attempting to assign a " + variableValue.GetType() + " to a " + type.GetType() + ".");
    }
}

internal class ScopeVariables
{
    public ScopeVariables upperScopes { get; set; }
    public List<CreateVariableNode> variables { get; set; }
    public ScopeVariables()
    {
        variables = new List<CreateVariableNode>();
    }
    public ScopeVariables(ScopeVariables upperScope)
    {
        upperScopes = upperScope;
        variables = new List<CreateVariableNode>();
    }

    public void Add(CreateVariableNode var)
    {
        variables.Add(var);
    }

    public List<CreateVariableNode> getCurrentVariables()
    {
        List<CreateVariableNode> vars = new List<CreateVariableNode>();
        vars.AddRange(variables);
        if (upperScopes != null)
        {
            vars.AddRange(upperScopes.getCurrentVariables());
        }
        return vars;
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
        TypeNode leftType = left.typeCheck();
        TypeNode rightType = right.typeCheck();
        if (leftType.GetType() == rightType.GetType() && (leftType.GetType() == typeof(NumberTypeNode) || leftType.GetType() == typeof(TextTypeNode)))
        {
            return leftType;
        }
        throw new Exception("Type mismatch in addition");
    }
}

internal class SubtractNode : NumberInFixNode
{
}

internal class MultiplyNode : NumberInFixNode
{
    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck();
        TypeNode rightType = right.typeCheck();
        if ((leftType.GetType() == typeof(NumberTypeNode) && rightType.GetType() == typeof(TextTypeNode)) || ( leftType.GetType() == typeof(TextTypeNode) && rightType.GetType() == typeof(NumberTypeNode)))
        {
            return new TextTypeNode();
        }
        if (leftType.GetType() == rightType.GetType() && leftType.GetType() == typeof(NumberTypeNode))
        {
            return new NumberTypeNode();
        }
        throw new Exception("Type mismatch in multiplication");
    }
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
        TypeNode leftType = left.typeCheck();
        TypeNode rightType = right.typeCheck();
        if (leftType.GetType() == rightType.GetType())
        {
            return new FlagTypeNode();
        }
        throw new Exception("Type mismatch in flag comparison");
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

    public override TypeNode typeCheck()
    {
        takes.typeCheck();
        return gives.typeCheck();
    }
}

internal class ListOfTypes : TypeNode
{
    private List<TypeNode> types = new List<TypeNode>();

    public ListOfTypes(List<TypeNode> list)
    {
        types = list;
    }

    public List<TypeNode> getList()
    {
        return types;
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
        return new NumberTypeNode();
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
        return new FlagTypeNode();
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
        return new TextTypeNode();
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
        return id.typeCheck();
    }
}

internal class IdentifierNode : Node
{
    public string name { get; set; }

    public override TypeNode typeCheck()
    {
        if (CreateVariableNode.variableExists(name))
        {
            return CreateVariableNode.getVariable(name).type;
        }
        throw new Exception("Variable with name: " + name + " does not exist");
    }
}

internal class BreakNode : Node
{
    public override TypeNode typeCheck()
    {
        return null;
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
        return null;
    }
}

//TODO: add typechecking for function variables
//TODO: add scope to variables
//TODO: add typechecking to give nodes, so they give the proper type
//