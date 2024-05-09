namespace CSTtoASTandTypeChecker;

public class TypeChecker
{
    public static void typeCheck(Node AST)
    {
        ScopeNode.typeCheckStart(AST);
        Console.WriteLine("The program is properly typed");
    }

    public static void resetStaticVariables()
    {
        ScopeNode.reset();
        FunctionNode.reset();
    }
}
public abstract class Node
{
    public abstract TypeNode typeCheck();

    public abstract TypeNode Type();
    public abstract void generate(CodeGeneratorVisitor cgv);
}

public abstract class TypeNode : Node
{
    public override TypeNode typeCheck()
    {
        return this;
    }
    public override TypeNode Type()
    {
        return this;
    }
}

public abstract class InFixNode : Node
{
    public Node left { get; set; }
    public Node right { get; set; }
}

public abstract class PreSufFixNode : Node
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

public abstract class ScopeNode : Node
{
    public static ScopeVariables scope { get; set; }

    public static SignatureNode CurrentSignature { get; set; }
    public static bool hasGive { get; set; }
    public static void typeCheckStart(Node AST)
    {
        scope = new ScopeVariables();
        CurrentSignature = null;
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

    internal static void reset()
    {
        scope = null;
        CurrentSignature = null;
        hasGive = false;
    }
}

public class FunctionNode : Node
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
        if (ScopeNode.CurrentSignature == null)
        {
            ScopeNode.CurrentSignature = signature;
        }
        else
        {
            throw new Exception("Cannot define function within a function");
        }
        ScopeNode.hasGive = false;
        ScopeNode.addScope();

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
        ScopeNode.CurrentSignature = null;
        if (ScopeNode.hasGive)
        {
            return signature.gives;
        }
        throw new Exception("Function must have at least 1 give statement");
    }

    public override TypeNode Type()
    {
        return signature.Type();
    }

    internal static void reset()
    {
        functions = new List<FunctionNode>();
    }

    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "FN" + signature + cmds;
    }
}

public class UseNode : Node
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
            ListOfTypes parameters = signature.takes.GetTypes();
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

    public override TypeNode Type()
    {
        return FunctionNode.getSignature(id.name).Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "U" + id + inputs;
    }
}

public class InputNode : InFixNode
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
        if (right != null)
        {
            ListOfTypes types = (ListOfTypes)right.typeCheck();
            foreach (TypeNode type in types.getList())
            {
                list.Add(type);
            }
        }

        return new ListOfTypes(list);
    }
    public override TypeNode Type()
    {
        return left.Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "IN" + left + right;
    }
}

public class ParameterNode : Node
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

    public ListOfTypes GetTypes()
    {
        List<TypeNode> list = new List<TypeNode>();
        list.Add(type.typeCheck());
        if (next != null)
        {
            ListOfTypes types = next.GetTypes();
            foreach (TypeNode type in types.getList())
            {
                list.Add(type);
            }
        }

        return new ListOfTypes(list);
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
    public override TypeNode Type()
    {
        return type.Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        if (next != null)
        {
            return "P" + type + id + next;
        }
        return "P" + type + id;
    }
}

public class IfNode : Node
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
        bool originalGive = ScopeNode.hasGive;
        ScopeNode.hasGive = false;
        condition.typeCheck();
        ScopeNode.addScope();
        Body.typeCheck();
        ScopeNode.removeScope();
        bool bodyGive = ScopeNode.hasGive;
        ScopeNode.hasGive = false;
        ScopeNode.addScope();
        if (ElseBody != null)
        {
            ElseBody.typeCheck();
        }
        ScopeNode.removeScope();
        ScopeNode.hasGive = (bodyGive && ScopeNode.hasGive) || originalGive;
        return null;
    }
    public override TypeNode Type()
    {
        return null;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "I" + condition + Body + ElseBody;
    }
}

public class RepeatNode : Node
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
    public override TypeNode Type()
    {
        return null;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "RW" + condition + Body;
    }
}

public class ReadNode : Node
{
    public override TypeNode typeCheck()
    {
        return Type();
    }
    public override TypeNode Type()
    {
        return new TextTypeNode();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "R";
    }
}

public class PrintNode : PreSufFixNode
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
    public override TypeNode Type()
    {
        return null;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "P" + node;
    }
}

public class LengthOfNode : PreSufFixNode
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
            TypeNode type = Identifier.typeCheck();
            if (type.GetType() == typeof(ListTypeNode) || type.typeCheck().GetType() == typeof(TextTypeNode))
            {
                return type;
            }
        }

        throw new Exception("Lengthof is missing an identifier");
    }
    public override TypeNode Type()
    {
        return Identifier.Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "LO" + Identifier;
    }
}

public class TypeConvertNode : PreSufFixNode
{
    public TypeNode type { get; set; }

    public override TypeNode typeCheck()
    {
        Type val = node.typeCheck().GetType();
        if ((type.GetType() == typeof(TextTypeNode) && val == typeof(NumberTypeNode))||(type.GetType() == typeof(NumberTypeNode) && val == typeof(TextTypeNode)))
        {
            return type;
        }

        throw new Exception("Bad type convert, trying to convert from " + val + " to " + type.GetType());
    }
    public override TypeNode Type()
    {
        return type.Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "TC" + type + node;
    }
}

public class CommandNode : InFixNode
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
    public override TypeNode Type()
    {
        return null;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "C" + left + right;
    }
}

public class CreateVariableNode : Node
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
        foreach (CreateVariableNode variable in ScopeNode.scope.getCurrentVariables())
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
        for (int i = 0; i < ScopeNode.scope.getCurrentVariables().Count; i++)
        {
            if (ScopeNode.scope.getCurrentVariables()[i].name.name == name)
            {
                return ScopeNode.scope.getCurrentVariables()[i];
            }
        }

        throw new Exception("Unable to get variable from scope variables");
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
            throw new Exception("Bad typing in create, attempting to assign a " + variableValue.GetType() + " to a " + type.GetType() + ".");
        }
        return type;
    }
    public override TypeNode Type()
    {
        return type.Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        if (value != null)
        {
            return "CV" + name + value + type;
        }
        return "CV" + name + type;
    }
}

public class ScopeVariables
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

public class AssignNode : InFixNode
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
    public override TypeNode Type()
    {
        return left.Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "A" + left + right;
    }
}
public abstract class NumberInFixNode : InFixNode
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
    public override TypeNode Type()
    {
        return new NumberTypeNode();
    }
}

public class AdditionNode : InFixNode
{
    public AdditionNode(){}

    public AdditionNode(Node left, Node right)
    {
        this.left = left;
        this.right = right;
    }
    
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
    public override TypeNode Type()
    {
        return left.Type();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "+" + right;
    }
}

public class SubtractNode : NumberInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    public override string ToString()
    {
        return left + "-" + right;
    }
}

public class MultiplyNode : NumberInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "*" + right;
    }
}

public class DivideNode : NumberInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "/" + right;
    }
}

public class ModuloNode : NumberInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "%" + right;
    }
}

public abstract class FlagInFixNode : InFixNode
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
    public override TypeNode Type()
    {
        return new FlagTypeNode();
    }
}

public class EqualsNode : FlagInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "=" + right;
    }
}

public class GreaterNode : FlagInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + ">" + right;
    }
}

public class GreaterEqualsNode : FlagInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + ">=" + right;
    }
}

public class LessNode : FlagInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "<" + right;
    }
}

public class LessEqualsNode : FlagInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "<=" + right;
    }
}

public class AndNode : FlagInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "&" + right;
    }
}

public class OrNode : FlagInFixNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return left + "|" + right;
    }
}

public class NotNode : PreSufFixNode
{
    public NotNode(Node x)
    {
        node = x;
    }
    public override TypeNode typeCheck()
    {
        if (node.typeCheck().GetType() == typeof(FlagTypeNode))
        {
            return new FlagTypeNode();
        }
        throw new Exception("Not flag in not");
    }
    public override TypeNode Type()
    {
        return new FlagTypeNode();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "!" + node;
    }
}

public class NumberTypeNode : TypeNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "NT";
    }
}

public class FlagTypeNode : TypeNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "FT";
    }
}

public class TextTypeNode : TypeNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "TT";
    }
}

public class ListTypeNode : TypeNode
{
    public TypeNode type { get; set; }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "LT";
    }
}

public class NothingNode : TypeNode
{
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "NT";
    }
}

public class SignatureNode : TypeNode
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
        if (takes != null)
        {
            takes.typeCheck();
        }
        return gives.typeCheck();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "S" + id + takes + gives;
    }
}

public class ListOfTypes : TypeNode
{
    private List<TypeNode> types;

    public ListOfTypes(List<TypeNode> list)
    {
        types = list;
    }

    public List<TypeNode> getList()
    {
        return types;
    }
    public override void generate(CodeGeneratorVisitor cgv){}
    
    public override string ToString()
    {
        throw new NotImplementedException();
    }
}

public class NumberNode : Node
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
    public override TypeNode Type()
    {
        return new NumberTypeNode();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "N" + value;
    }
}

public class FlagNode : Node
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
    public override TypeNode Type()
    {
        return new FlagTypeNode();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "F" + value;
    }
}

public class TextNode : Node
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
    public override TypeNode Type()
    {
        return new TextTypeNode();
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "T" + value;
    }
}

public class ListElementNode : Node
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
        TypeNode type = id.typeCheck();
        if (type.GetType() == typeof(ListTypeNode))
        {
            ListTypeNode list = (ListTypeNode)type;
            return list.type;
        }
        return type;
    }
    public override TypeNode Type()
    {
        TypeNode type = id.Type();
        if (type.GetType() == typeof(ListTypeNode))
        {
            ListTypeNode list = (ListTypeNode)type;
            return list.type;
        }
        return type;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "LE" + id + index;
    }
}

public class AddToListNode : InFixNode
{
    public Node index;
    public AddToListNode(Node x, Node y, Node z)
    {
        left = x;
        right = y;
        index = z;
    }

    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck();
        TypeNode rightType = right.typeCheck();
        if (rightType.GetType() == typeof(ListTypeNode))
        {
            ListTypeNode list = (ListTypeNode)rightType;
            if (leftType.GetType() == list.type.GetType())
            {
                return leftType;
            }
        }
        throw new Exception("Cannot add a " + leftType.GetType() + " to a " + rightType.GetType);
    }
    public override TypeNode Type()
    {
        return null;
    }

    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "ATL" + index;
    }
}

public class IdentifierNode : Node
{
    public TypeNode type { get; set; }
    public string name { get; set; }
    
    public IdentifierNode(){}

    public IdentifierNode(string name)
    {
        this.name = name;
    }

    public override TypeNode typeCheck()
    {
        if (CreateVariableNode.variableExists(name))
        {
            type = CreateVariableNode.getVariable(name).type;
            return type;
        }
        throw new Exception("Variable with name: " + name + " does not exist");
    }
    public override TypeNode Type()
    {
        return type;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "I" + name;
    }
}

public class BreakNode : Node
{
    public override TypeNode typeCheck()
    {
        return null;
    }
    public override TypeNode Type()
    {
        return null;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    public override string ToString()
    {
        return "B";
    }
}

public class GiveNode : Node
{
    public Node value { get; set; }
    public TypeNode type { get; set; }

    public GiveNode(Node x)
    {
        value = x;
    }
    
    public override TypeNode typeCheck()
    {
        SignatureNode sign = ScopeNode.CurrentSignature;
        if (sign != null)
        {
            if (value.typeCheck().GetType() == sign.gives.typeCheck().GetType())
            {
                ScopeNode.hasGive = true;
                type = sign.gives;
                return sign.gives;
            }

            throw new Exception("Bad typing in give, trying to give " + value.typeCheck() + " in a function that gives " + sign.gives);
        }
        throw new Exception("cannot use give outside function declaration");
    }
    public override TypeNode Type()
    {
        return null;
    }
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}

    public override string ToString()
    {
        return "G" + value;
    }
}