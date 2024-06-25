// Defines the namespace for the CST to AST conversion and type checking components
namespace CSTtoASTandTypeChecker;

// TypeChecker class responsible for initiating and managing the type checking process
public class TypeChecker
{
    // Initiates the type checking process on the AST (Abstract Syntax Tree)
    public static void typeCheck(Node AST)
    {
        ScopeNode.typeCheckStart(AST); // Starts type checking from the root of the AST
        Console.WriteLine("The program is properly typed"); // Indicates successful type checking
    }

    // Resets static variables to their default states, preparing for a new type checking session
    public static void resetStaticVariables()
    {
        ScopeNode.reset(); // Resets the scope-related variables
        FunctionNode.reset(); // Resets the function-related variables
    }
}

// Abstract base class for all nodes in the AST
public abstract class Node
{
    // Abstract method for type checking, to be implemented by derived classes
    public abstract TypeNode typeCheck();

    // Abstract method to get the type of the node, to be implemented by derived classes
    public abstract TypeNode Type();

    // Abstract method for code generation, to be implemented by derived classes
    public abstract void generate(CodeGeneratorVisitor cgv);
}

// Abstract class representing type nodes in the AST
public abstract class TypeNode : Node
{
    // Implementation of type checking for type nodes, returns the type itself
    public override TypeNode typeCheck()
    {
        return this;
    }

    // Returns the type of the node, which is the node itself
    public override TypeNode Type()
    {
        return this;
    }
}

// Abstract class for nodes representing infix operations (e.g., addition, subtraction)
public abstract class InFixNode : Node
{
    // Left operand of the infix operation
    public Node left { get; set; }
    // Right operand of the infix operation
    public Node right { get; set; }
}

// Abstract class for nodes representing prefix or suffix operations (e.g., increment, decrement)
public abstract class PreSufFixNode : Node
{
    // The node the operation is applied to
    public Node node { get; set; }

    // Type checks the operation, ensuring the node exists
    public override TypeNode typeCheck()
    {
        if (node != null)
            node.typeCheck(); // Type check the node
        else
        {
            throw new Exception("keyword contents does not exist"); // Error if the node is missing
        }
        return null;
    }
}

// Abstract class for scope nodes, managing variable scopes and type checking within scopes
public abstract class ScopeNode : Node
{
    // Current scope variables
    public static ScopeVariables scope { get; set; }

    // Current function signature, if any
    public static SignatureNode CurrentSignature { get; set; }

    // Flag indicating if a 'give' statement has been encountered
    public static bool hasGive { get; set; }

    // Starts the type checking process from the root of the AST
    public static void typeCheckStart(Node AST)
    {
        scope = new ScopeVariables(); // Initialize a new scope
        CurrentSignature = null; // Reset current function signature
        AST.typeCheck(); // Begin type checking from the root
    }

    // Adds a new scope layer
    public static void addScope()
    {
        scope = new ScopeVariables(scope); // Create a new scope with the current one as its parent
    }

    // Removes the current scope layer, reverting to the parent scope
    public static void removeScope()
    {
        scope = scope.upperScopes; // Revert to the upper scope
    }

    // Resets scope-related static variables to their default states
    internal static void reset()
    {
        scope = null; // Clear current scope
        CurrentSignature = null; // Clear current function signature
        hasGive = false; // Reset 'give' flag
    }
}

// Represents a function node in the AST (Abstract Syntax Tree).
// This node is responsible for defining functions, including their signatures and command sequences.
public class FunctionNode : Node
{
    // Static list to keep track of all functions declared in the program.
    private static List<FunctionNode> functions = new List<FunctionNode>();

    // The signature of the function, containing its name, parameters, and return type.
    public SignatureNode signature { get; set; }

    // The sequence of commands (or statements) that make up the function body.
    public Node cmds { get; set; }

    // Constructor for creating a new function node with a signature and command sequence.
    public FunctionNode(Node x, Node y)
    {
        signature = (SignatureNode)x; // Cast the first node to a SignatureNode for the function's signature.
        cmds = y; // The second node represents the commands within the function.
        addToListOfFunctions(this); // Add this function to the list of declared functions.
    }

    // Adds a function node to the list of functions, ensuring no duplicate function names.
    private void addToListOfFunctions(FunctionNode function)
    {
        if (functionExists(function.signature.id.name)) // Check if a function with the same name already exists.
        {
            throw new Exception("Function with identifier: " + function.signature.id.name + " is declared twice");
        }
        functions.Add(function); // Add the function to the list if it's unique.
    }

    // Checks if a function with the specified name already exists in the list of functions.
    public static bool functionExists(string name)
    {
        foreach (FunctionNode func in functions)
        {
            if (name == func.signature.id.name) // Compare the provided name with each function's name.
            {
                return true; // Return true if a match is found.
            }
        }
        return false; // Return false if no match is found.
    }

    // Retrieves the signature of a function by its name.
    public static SignatureNode getSignature(string name)
    {
        foreach (FunctionNode func in functions)
        {
            if (name == func.signature.id.name) // Find the function by name.
            {
                return func.signature; // Return its signature.
            }
        }
        throw new Exception("Signature does not exist"); // Throw an exception if the function is not found.
    }

    // Performs type checking for the function node.
    public override TypeNode typeCheck()
    {
        if (ScopeNode.CurrentSignature == null) // Ensure no nested function definitions.
        {
            ScopeNode.CurrentSignature = signature; // Set the current function signature in the scope.
        }
        else
        {
            throw new Exception("Cannot define function within a function");
        }
        ScopeNode.hasGive = false; // Reset the 'give' statement flag.
        ScopeNode.addScope(); // Add a new scope for the function body.

        if (signature != null)
        {
            signature.typeCheck(); // Type check the function signature.
        }
        else
        {
            throw new Exception("Signature does not exist");
        }

        if (cmds != null)
        {
            cmds.typeCheck(); // Type check the command sequence.
        }
        else
        {
            throw new Exception("commands does not exist for function: " + signature.id.name);
        }
        ScopeNode.removeScope(); // Remove the function's scope.
        ScopeNode.CurrentSignature = null; // Reset the current function signature in the scope.
        if (ScopeNode.hasGive)
        {
            return signature.gives; // Return the function's return type if a 'give' statement exists.
        }
        throw new Exception("Function must have at least 1 give statement");
    }

    // Returns the type of the function node, which is determined by its signature.
    public override TypeNode Type()
    {
        return signature.Type();
    }

    // Resets the list of functions to an empty list.
    // This is typically used to clear the state between different compilations or runs.
    internal static void reset()
    {
        functions = new List<FunctionNode>();
    }

    // Generates code for the function node using a visitor pattern.
    public override void generate(CodeGeneratorVisitor cgv)
    {
        cgv.visit(this); // Visit this node with the code generator visitor.
    }
    
    // Returns a string representation of the function node, including its signature and commands.
    public override string ToString()
    {
        return "FN" + signature + cmds;
    }
}

// Represents a node for using a function within the AST.
public class UseNode : Node
{
    // Identifier of the function being used.
    public IdentifierNode id { get; set; }
    // Inputs provided to the function.
    public InputNode inputs { get; set; }

    // Constructor for creating a UseNode with a function identifier and inputs.
    public UseNode(Node x, InputNode y)
    {
        id = (IdentifierNode)x; // Cast the first node to an IdentifierNode for the function's identifier.
        inputs = y; // The second node represents the inputs to the function.
    }

    // Performs type checking for the UseNode, ensuring the function exists and input types match.
    public override TypeNode typeCheck()
    {
        SignatureNode signature;
        if (FunctionNode.functionExists(id.name)) // Check if the function exists.
        {
            signature = FunctionNode.getSignature(id.name); // Get the function's signature.
            ListOfTypes parameters = signature.takes.GetTypes(); // Get the expected parameter types.
            ListOfTypes input = (ListOfTypes)inputs.typeCheck(); // Type check and get the provided input types.
            List<TypeNode> paramList = parameters.getList(); // Get the list of expected parameter types.
            List<TypeNode> inputList = input.getList(); // Get the list of provided input types.
            if (paramList.Count == inputList.Count) // Ensure the number of inputs matches the number of parameters.
            {
                for (int i = 0; i < inputList.Count(); i++) // Iterate through each input.
                {
                    if (inputList[i].GetType() != paramList[i].GetType()) // Check if input type matches parameter type.
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
        return signature.gives; // Return the function's return type.
    }

    // Returns the type of the UseNode, which is the return type of the function being used.
    public override TypeNode Type()
    {
        return FunctionNode.getSignature(id.name).Type();
    }

    // Generates code for the UseNode using a visitor pattern.
    public override void generate(CodeGeneratorVisitor cgv)
    {
        cgv.visit(this); // Visit this node with the code generator visitor.
    }
    
    // Returns a string representation of the UseNode, including its identifier and inputs.
    public override string ToString()
    {
        return "U" + id + inputs;
    }
}

// Represents a node for inputs provided to a function or operation within the AST.
public class InputNode : InFixNode
{
    // Constructor for creating an InputNode with left and right child nodes.
    public InputNode(Node x, Node y)
    {
        left = x; // The left child node represents the first input.
        right = y; // The right child node represents additional inputs.
    }

    // Performs type checking for the InputNode, aggregating the types of all inputs.
    public override TypeNode typeCheck()
    {
        List<TypeNode> list = new List<TypeNode>(); // List to hold the types of all inputs.
        list.Add(left.typeCheck()); // Add the type of the first input.
        if (right != null) // Check if there are additional inputs.
        {
            ListOfTypes types = (ListOfTypes)right.typeCheck(); // Type check and get the types of additional inputs.
            foreach (TypeNode type in types.getList()) // Iterate through each additional input type.
            {
                list.Add(type); // Add the type to the list.
            }
        }

        return new ListOfTypes(list); // Return a ListOfTypes containing the types of all inputs.
    }

    // Returns the type of the InputNode, which is the type of the first input.
    public override TypeNode Type()
    {
        return left.Type();
    }

    // Generates code for the InputNode using a visitor pattern.
    public override void generate(CodeGeneratorVisitor cgv)
    {
        cgv.visit(this); // Visit this node with the code generator visitor.
    }
    
    // Returns a string representation of the InputNode, including its left and right child nodes.
    public override string ToString()
    {
        return "IN" + left + right;
    }
}

// Represents a parameter node in the AST, used for function/method parameters.
public class ParameterNode : Node
{
    // Type of the parameter
    public TypeNode type { get; set; }
    // Identifier (name) of the parameter
    public IdentifierNode id { get; set; }
    // Reference to the next parameter in the list (if any)
    public ParameterNode next { get; set; }

    // Constructor: Initializes a new instance of the ParameterNode class with specified type, identifier, and next parameter
    public ParameterNode(Node x, Node y, Node z)
    {
        type = (TypeNode)x; // Casts and sets the type of the parameter
        id = (IdentifierNode)y; // Casts and sets the identifier of the parameter
        next = (ParameterNode)z; // Casts and sets the next parameter (if any)
    }

    // Compiles a list of types from this parameter onwards, useful for function signatures
    public ListOfTypes GetTypes()
    {
        List<TypeNode> list = new List<TypeNode>(); // Initializes a new list of TypeNodes
        list.Add(type.typeCheck()); // Adds the type of this parameter after type checking
        if (next != null) // Checks if there is a next parameter
        {
            ListOfTypes types = next.GetTypes(); // Recursively gets the types of the next parameters
            foreach (TypeNode type in types.getList()) // Iterates over the types
            {
                list.Add(type); // Adds each type to the list
            }
        }

        return new ListOfTypes(list); // Returns a new ListOfTypes containing all parameter types
    }

    // Performs type checking for the parameter and any subsequent parameters
    public override TypeNode typeCheck()
    {
        new CreateVariableNode(id, type).addToVariables(); // Adds this parameter as a variable to the scope

        List<TypeNode> list = new List<TypeNode>(); // Initializes a new list of TypeNodes
        list.Add(type.typeCheck()); // Adds the type of this parameter after type checking
        if (next != null) // Checks if there is a next parameter
        {
            ListOfTypes types = (ListOfTypes)next.typeCheck(); // Recursively type checks the next parameters
            foreach (TypeNode type in types.getList()) // Iterates over the types
            {
                list.Add(type); // Adds each type to the list
            }
        }

        return new ListOfTypes(list); // Returns a new ListOfTypes containing all parameter types after type checking
    }

    // Returns the type of this parameter
    public override TypeNode Type()
    {
        return type.Type();
    }

    // Generates code for this parameter node using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv)
    {
        cgv.visit(this); // Visits this node with the code generator visitor
    }
    
    // Returns a string representation of the parameter node, including its type, identifier, and any subsequent parameters
    public override string ToString()
    {
        if (next != null) // Checks if there is a next parameter
        {
            return "P" + type + id + next; // Returns string representation including the next parameter
        }
        return "P" + type + id; // Returns string representation without the next parameter
    }
}

// Represents an "if" statement node within the abstract syntax tree (AST)
public class IfNode : Node
{
    // Condition for the if statement
    public Node condition { get; set; }
    // Body of the if statement to execute if the condition is true
    public Node Body { get; set; }
    // Optional else body to execute if the condition is false
    public Node ElseBody { get; set; }

    // Constructor to initialize an IfNode with a condition, a body, and an optional else body
    public IfNode(Node x, Node y, Node z)
    {
        condition = x; // Condition node
        Body = y; // Body node for the true branch
        ElseBody = z; // Else body node for the false branch (can be null)
    }

    // Performs type checking for the if statement, including its condition and body/else body
    public override TypeNode typeCheck()
    {
        bool originalGive = ScopeNode.hasGive; // Store the original state of 'hasGive'
        ScopeNode.hasGive = false; // Reset 'hasGive' before type checking
        condition.typeCheck(); // Type check the condition
        ScopeNode.addScope(); // Add a new scope for the body
        Body.typeCheck(); // Type check the body
        ScopeNode.removeScope(); // Remove the body's scope
        bool bodyGive = ScopeNode.hasGive; // Store 'hasGive' after checking the body
        ScopeNode.hasGive = false; // Reset 'hasGive' before checking the else body
        ScopeNode.addScope(); // Add a new scope for the else body
        if (ElseBody != null) // Check if there is an else body
        {
            ElseBody.typeCheck(); // Type check the else body
        }
        ScopeNode.removeScope(); // Remove the else body's scope
        ScopeNode.hasGive = (bodyGive && ScopeNode.hasGive) || originalGive; // Update 'hasGive' based on body and else body checks
        return null; // IfNode does not have a type itself
    }

    // Returns null as IfNode does not have a type
    public override TypeNode Type()
    {
        return null;
    }

    // Generates code for the if statement using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}

    // Returns a string representation of the IfNode, including its condition, body, and else body
    public override string ToString()
    {
        return "I" + condition + Body + ElseBody;
    }
}

// Represents a "repeat" loop node within the abstract syntax tree (AST)
public class RepeatNode : Node
{
    // Condition for the repeat loop
    public Node condition { get; set; }
    // Body of the repeat loop to execute
    public Node Body { get; set; }

    // Constructor to initialize a RepeatNode with a condition and a body
    public RepeatNode(Node x, Node y)
    {
        condition = x; // Condition node
        Body = y; // Body node for the loop
    }

    // Performs type checking for the repeat loop, including its condition and body
    public override TypeNode typeCheck()
    {
        condition.typeCheck(); // Type check the condition
        ScopeNode.addScope(); // Add a new scope for the body
        Body.typeCheck(); // Type check the body
        ScopeNode.removeScope(); // Remove the body's scope
        return null; // RepeatNode does not have a type itself
    }

    // Returns null as RepeatNode does not have a type
    public override TypeNode Type()
    {
        return null;
    }

    // Generates code for the repeat loop using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}

    // Returns a string representation of the RepeatNode, including its condition and body
    public override string ToString()
    {
        return "RW" + condition + Body;
    }
}

// Represents a node for reading input in the abstract syntax tree (AST)
public class ReadNode : Node
{
    // Performs type checking for the read operation and returns its type
    public override TypeNode typeCheck()
    {
        return Type(); // Returns the type of the read node
    }

    // Returns the type of the read operation, which is TextTypeNode indicating text input
    public override TypeNode Type()
    {
        return new TextTypeNode(); // Specifies that the read operation returns text
    }

    // Generates code for the read operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}

    // Returns a string representation of the ReadNode
    public override string ToString()
    {
        return "R"; // Represents the read operation
    }
}

// Represents a node for printing output in the abstract syntax tree (AST)
public class PrintNode : PreSufFixNode
{
    // Constructor to initialize a PrintNode with an input node
    public PrintNode(Node input)
    {
        node = input; // The node to be printed
    }

    // Performs type checking for the print operation, ensuring there is a body to print
    public override TypeNode typeCheck()
    {
        if (node != null) // Checks if there is an input node
        {
            node.typeCheck(); // Performs type checking on the input node
        }
        else
        {
            throw new Exception("Print statement missing body"); // Throws an exception if the input node is missing
        }
        return null; // PrintNode does not have a type itself
    }

    // Returns null as PrintNode does not have a type
    public override TypeNode Type()
    {
        return null;
    }

    // Generates code for the print operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}

    // Returns a string representation of the PrintNode, including its input node
    public override string ToString()
    {
        return "P" + node; // Represents the print operation and its input
    }
}

// Represents a node for obtaining the length of a list or text in the abstract syntax tree (AST)
public class LengthOfNode : PreSufFixNode
{
    // The identifier node whose length is to be determined
    public IdentifierNode Identifier { get; set; }

    // Constructor to initialize a LengthOfNode with an identifier node
    public LengthOfNode(Node x)
    {
        Identifier = (IdentifierNode)x; // Casts and sets the identifier node
    }

    // Performs type checking for the length operation, ensuring the identifier is a list or text
    public override TypeNode typeCheck()
    {
        if (Identifier != null) // Checks if there is an identifier node
        {
            TypeNode type = Identifier.typeCheck(); // Performs type checking on the identifier
            // Checks if the identifier's type is ListTypeNode or TextTypeNode
            if (type.GetType() == typeof(ListTypeNode) || type.typeCheck().GetType() == typeof(TextTypeNode))
            {
                return new NumberTypeNode(); // Returns NumberTypeNode indicating the length is a number
            }
            throw new Exception("Trying to Lengthof on " + type); // Throws an exception if the identifier's type is not list or text
        }
        throw new Exception("Lengthof is missing an identifier"); // Throws an exception if the identifier node is missing
    }

    // Returns the type of the identifier node
    public override TypeNode Type()
    {
        return Identifier.Type();
    }

    // Generates code for the length operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}

    // Returns a string representation of the LengthOfNode, including its identifier
    public override string ToString()
    {
        return "LO" + Identifier; // Represents the length operation and its identifier
    }
}

// Represents a node for type conversion in the abstract syntax tree (AST)
public class TypeConvertNode : PreSufFixNode
{
    // The target type for conversion
    public TypeNode type { get; set; }

    // Performs type checking for the conversion, ensuring valid type conversion rules
    public override TypeNode typeCheck()
    {
        // Get the actual type of the node to be converted
        Type val = node.typeCheck().GetType();
        // Check if conversion is between TextTypeNode and NumberTypeNode in either direction
        if ((type.GetType() == typeof(TextTypeNode) && val == typeof(NumberTypeNode)) ||
            (type.GetType() == typeof(NumberTypeNode) && val == typeof(TextTypeNode)))
        {
            return type; // Return the target type if conversion is valid
        }

        // Throw an exception if attempting an invalid type conversion
        throw new Exception("Bad type convert, trying to convert from " + val + " to " + type.GetType());
    }

    // Returns the target type of the conversion
    public override TypeNode Type()
    {
        return type.Type();
    }

    // Generates code for the type conversion using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the TypeConvertNode, including its target type and node to be converted
    public override string ToString()
    {
        return "TC" + type + node;
    }
}

// Represents a command node in the abstract syntax tree (AST), used for operations that involve two operands
public class CommandNode : InFixNode
{
    // Constructor to initialize a CommandNode with left and right child nodes
    public CommandNode(Node x, Node y)
    {
        left = x; // Left operand
        right = y; // Right operand
    }

    // Performs type checking for the command, ensuring both operands are type-checked
    public override TypeNode typeCheck()
    {
        if (left != null)
        {
            left.typeCheck(); // Type check the left operand
        }

        if (right != null)
        {
            right.typeCheck(); // Type check the right operand
        }

        return null; // CommandNode does not have a type itself
    }

    // Returns null as CommandNode does not have a type
    public override TypeNode Type()
    {
        return null;
    }

    // Generates code for the command using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the CommandNode, including its left and right operands
    public override string ToString()
    {
        return "C" + left + right;
    }
}

// Represents a node for creating variables in the abstract syntax tree (AST)
public class CreateVariableNode : Node
{
    // The name of the variable
    public IdentifierNode name { get; set; }
    // The type of the variable
    public TypeNode type { get; set; }
    // The initial value of the variable
    public Node value { get; set; }

    // Constructor for initializing a CreateVariableNode with a name, type, and initial value
    public CreateVariableNode(Node x, Node y, Node z)
    {
        name = (IdentifierNode)x; // Cast and set the variable name
        type = (TypeNode)y; // Cast and set the variable type
        value = z; // Set the initial value
    }

    // Constructor for initializing a CreateVariableNode with a name and type, but no initial value
    public CreateVariableNode(Node x, Node y)
    {
        name = (IdentifierNode)x; // Cast and set the variable name
        type = (TypeNode)y; // Cast and set the variable type
    }

    // Adds this variable to the current scope if it doesn't already exist
    public void addToVariables()
    {
        if (variableExists(name.name)) // Check if the variable already exists in the current scope
        {
            throw new Exception("variable declared more than once!"); // Throw an exception if it does
        }
        ScopeNode.scope.Add(this); // Add this variable to the current scope
    }

    // Checks if a variable with the given name exists in the current scope
    public static bool variableExists(string name)
    {
        foreach (CreateVariableNode variable in ScopeNode.scope.getCurrentVariables()) // Iterate through current scope variables
        {
            if (variable.name.name == name) // Check if the variable name matches
            {
                return true; // Return true if a match is found
            }
        }
        return false; // Return false if no match is found
    }

    // Retrieves a variable by name from the current scope
    public static CreateVariableNode getVariable(string name)
    {
        for (int i = 0; i < ScopeNode.scope.getCurrentVariables().Count; i++) // Iterate through current scope variables
        {
            if (ScopeNode.scope.getCurrentVariables()[i].name.name == name) // Check if the variable name matches
            {
                return ScopeNode.scope.getCurrentVariables()[i]; // Return the matching variable
            }
        }

        throw new Exception("Unable to get variable from scope variables"); // Throw an exception if the variable is not found
    }

    // Performs type checking for the variable creation, ensuring the initial value matches the declared type
    public override TypeNode typeCheck()
    {
        addToVariables(); // Add this variable to the current scope

        TypeNode variableValue = null;
        if (value != null) // Check if an initial value is provided
        {
            variableValue = value.typeCheck(); // Perform type checking on the initial value

            if (type.GetType() == variableValue.GetType()) // Check if the initial value's type matches the declared type
            {
                return type; // Return the declared type if it matches
            }
            throw new Exception("Bad typing in create, attempting to assign a " + variableValue.GetType() + " to a " + type.GetType() + "."); // Throw an exception if the types do not match
        }
        return type; // Return the declared type if no initial value is provided
    }

    // Returns the declared type of the variable
    public override TypeNode Type()
    {
        return type.Type();
    }

    // Generates code for the variable creation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the CreateVariableNode, including its name, type, and optionally its initial value
    public override string ToString()
    {
        if (value != null) // Check if an initial value is provided
        {
            return "CV" + name + value + type; // Return a string representation including the initial value
        }
        return "CV" + name + type; // Return a string representation without the initial value
    }
}

// Defines a structure for managing variables within scopes, including nested scopes
public class ScopeVariables
{
    // Reference to the parent or upper scope to allow nested scope management
    public ScopeVariables upperScopes { get; set; }
    // List of variables defined in the current scope
    public List<CreateVariableNode> variables { get; set; }

    // Default constructor initializes the variables list for the current scope
    public ScopeVariables()
    {
        variables = new List<CreateVariableNode>();
    }

    // Constructor that initializes the current scope with a reference to an upper scope
    public ScopeVariables(ScopeVariables upperScope)
    {
        upperScopes = upperScope;
        variables = new List<CreateVariableNode>();
    }

    // Adds a variable to the current scope
    public void Add(CreateVariableNode var)
    {
        variables.Add(var);
    }

    // Retrieves all variables available in the current scope, including those from upper scopes
    public List<CreateVariableNode> getCurrentVariables()
    {
        List<CreateVariableNode> vars = new List<CreateVariableNode>();
        vars.AddRange(variables); // Add variables from the current scope
        if (upperScopes != null)
        {
            // Recursively add variables from upper scopes
            vars.AddRange(upperScopes.getCurrentVariables());
        }
        return vars;
    }
}

// Represents an assignment operation in the abstract syntax tree (AST)
public class AssignNode : InFixNode
{
    // Constructor to initialize an AssignNode with left (variable) and right (value) nodes
    public AssignNode(Node x, Node y)
    {
        left = x; // The variable to assign to
        right = y; // The value to assign
    }

    // Performs type checking for the assignment, ensuring the variable and value have compatible types
    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck(); // Type check the variable
        TypeNode rightType = right.typeCheck(); // Type check the value

        if (leftType.GetType() == rightType.GetType())
        {
            return leftType; // Return the variable's type if types are compatible
        }

        // Throw an exception if attempting to assign incompatible types
        throw new Exception("Bad typing in Assignment, attempting to assign a " + rightType + " to a " + leftType);
    }

    // Returns the type of the variable being assigned to
    public override TypeNode Type()
    {
        return left.Type();
    }

    // Generates code for the assignment operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the AssignNode, including its left and right nodes
    public override string ToString()
    {
        return "A" + left + right;
    }
}
// Defines an abstract class for infix operations on numbers in the abstract syntax tree (AST)
public abstract class NumberInFixNode : InFixNode
{
    // Performs type checking for the operation, ensuring both operands are of type NumberTypeNode
    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck(); // Type check the left operand
        TypeNode rightType = right.typeCheck(); // Type check the right operand

        // Check if both operands are of the same type and that type is NumberTypeNode
        if (leftType.GetType() == rightType.GetType() && leftType.GetType() == typeof(NumberTypeNode))
        {
            return leftType; // Return the type of the operands if valid
        }

        // Throw an exception if operands are of different types or not of type NumberTypeNode
        throw new Exception("Bad typing in NumberInfixExpression, attempting to perform invalid operations with a " + leftType.GetType() + " on a " + rightType.GetType() + ".");
    }

    // Returns the type of the node, which is NumberTypeNode for all NumberInFixNode operations
    public override TypeNode Type()
    {
        return new NumberTypeNode();
    }
}

// Represents an addition operation in the abstract syntax tree (AST)
public class AdditionNode : InFixNode
{
    // Default constructor
    public AdditionNode(){}

    // Constructor to initialize an AdditionNode with left and right child nodes
    public AdditionNode(Node left, Node right)
    {
        this.left = left; // Set the left operand
        this.right = right; // Set the right operand
    }
    
    // Performs type checking for the addition, ensuring both operands are of compatible types (either NumberTypeNode or TextTypeNode)
    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck(); // Type check the left operand
        TypeNode rightType = right.typeCheck(); // Type check the right operand

        // Check if both operands are of the same type and that type is either NumberTypeNode or TextTypeNode
        if (leftType.GetType() == rightType.GetType() && (leftType.GetType() == typeof(NumberTypeNode) || leftType.GetType() == typeof(TextTypeNode)))
        {
            return leftType; // Return the type of the operands if valid
        }

        // Throw an exception if operands are of incompatible types
        throw new Exception("Type mismatch in addition");
    }

    // Returns the type of the left operand, assuming both operands are of the same type
    public override TypeNode Type()
    {
        return left.Type();
    }

    // Generates code for the addition operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the AdditionNode, including its operands and the addition operator
    public override string ToString()
    {
        return left + "+" + right;
    }
}

// Represents a subtraction operation in the abstract syntax tree (AST) for numbers
public class SubtractNode : NumberInFixNode
{
    // Generates code for the subtraction operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    // Returns a string representation of the SubtractNode, including its operands and the subtraction operator
    public override string ToString()
    {
        return left + "-" + right;
    }
}

// Represents a multiplication operation in the AST for numbers
public class MultiplyNode : NumberInFixNode
{
    // Generates code for the multiplication operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    // Returns a string representation of the MultiplyNode, including its operands and the multiplication operator
    public override string ToString()
    {
        return left + "*" + right;
    }
}

// Represents a division operation in the AST for numbers
public class DivideNode : NumberInFixNode
{
    // Generates code for the division operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    // Returns a string representation of the DivideNode, including its operands and the division operator
    public override string ToString()
    {
        return left + "/" + right;
    }
}

// Represents a modulo operation in the AST for numbers
public class ModuloNode : NumberInFixNode
{
    // Generates code for the modulo operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    // Returns a string representation of the ModuloNode, including its operands and the modulo operator
    public override string ToString()
    {
        return left + "%" + right;
    }
}

// Represents an abstract class for infix operations that result in a flag (boolean) value
public abstract class FlagInFixNode : InFixNode
{
    // Performs type checking for the operation, ensuring both operands are of the same type and returns a FlagTypeNode
    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck(); // Type check the left operand
        TypeNode rightType = right.typeCheck(); // Type check the right operand
        if (leftType.GetType() == rightType.GetType())
        {
            return new FlagTypeNode(); // Return FlagTypeNode if operands are of the same type
        }
        throw new Exception("Type mismatch in flag comparison"); // Throw an exception if operands are of different types
    }
    // Returns a FlagTypeNode indicating the result type of the operation is a flag (boolean)
    public override TypeNode Type()
    {
        return new FlagTypeNode();
    }
}

// Represents an equality comparison operation in the abstract syntax tree (AST)
public class EqualsNode : FlagInFixNode
{
    // Generates code for the equality comparison operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the EqualsNode, including its operands and the equality operator
    public override string ToString()
    {
        return left + "=" + right;
    }
}

// Represents a greater-than comparison operation in the AST
public class GreaterNode : FlagInFixNode
{
    // Generates code for the greater-than comparison operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the GreaterNode, including its operands and the greater-than operator
    public override string ToString()
    {
        return left + ">" + right;
    }
}

// Represents a greater-than-or-equal-to comparison operation in the AST
public class GreaterEqualsNode : FlagInFixNode
{
    // Generates code for the greater-than-or-equal-to comparison operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the GreaterEqualsNode, including its operands and the greater-than-or-equal-to operator
    public override string ToString()
    {
        return left + ">=" + right;
    }
}

// Represents a less-than comparison operation in the AST
public class LessNode : FlagInFixNode
{
    // Generates code for the less-than comparison operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the LessNode, including its operands and the less-than operator
    public override string ToString()
    {
        return left + "<" + right;
    }
}

// Represents a less-than-or-equal-to comparison operation in the AST
public class LessEqualsNode : FlagInFixNode
{
    // Generates code for the less-than-or-equal-to comparison operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the LessEqualsNode, including its operands and the less-than-or-equal-to operator
    public override string ToString()
    {
        return left + "<=" + right;
    }
}

// Represents a logical AND operation in the AST
public class AndNode : FlagInFixNode
{
    // Generates code for the logical AND operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the AndNode, including its operands and the logical AND operator
    public override string ToString()
    {
        return left + "&" + right;
    }
}

// Represents a logical OR operation in the AST
public class OrNode : FlagInFixNode
{
    // Generates code for the logical OR operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the OrNode, including its operands and the logical OR operator
    public override string ToString()
    {
        return left + "|" + right;
    }
}

// Represents a logical NOT operation node in the abstract syntax tree (AST)
public class NotNode : PreSufFixNode
{
    // Constructor to initialize a NotNode with a child node
    public NotNode(Node x)
    {
        node = x; // Set the child node
    }
    
    // Performs type checking for the NOT operation, ensuring the child node is of FlagTypeNode
    public override TypeNode typeCheck()
    {
        // Check if the child node's type is FlagTypeNode
        if (node.typeCheck().GetType() == typeof(FlagTypeNode))
        {
            return new FlagTypeNode(); // Return FlagTypeNode if valid
        }
        // Throw an exception if the child node's type is not FlagTypeNode
        throw new Exception("Not flag in not");
    }
    
    // Returns the type of the node, which is FlagTypeNode for NOT operations
    public override TypeNode Type()
    {
        return new FlagTypeNode();
    }
    
    // Generates code for the NOT operation using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the NotNode, including the NOT operator and the child node
    public override string ToString()
    {
        return "!" + node;
    }
}

// Represents a number type node in the AST
public class NumberTypeNode : TypeNode
{
    // Generates code for the number type using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the NumberTypeNode
    public override string ToString()
    {
        return "NT";
    }
}

// Represents a flag (boolean) type node in the AST
public class FlagTypeNode : TypeNode
{
    // Generates code for the flag type using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the FlagTypeNode
    public override string ToString()
    {
        return "FT";
    }
}

// Represents a text type node in the AST
public class TextTypeNode : TypeNode
{
    // Generates code for the text type using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the TextTypeNode
    public override string ToString()
    {
        return "TT";
    }
}

// Represents a list type node in the AST
public class ListTypeNode : TypeNode
{
    // The type of elements in the list
    public TypeNode type { get; set; }
    
    // Generates code for the list type using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the ListTypeNode
    public override string ToString()
    {
        return "LT";
    }
}

// Represents a node for the absence of a type in the AST
public class NothingNode : TypeNode
{
    // Generates code for the nothing type using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the NothingNode
    public override string ToString()
    {
        return "NT";
    }
}

// Represents a function signature node in the abstract syntax tree (AST)
public class SignatureNode : TypeNode
{
    // Identifier for the function
    public IdentifierNode id { get; set; }
    // Parameters of the function
    public ParameterNode takes { get; set; }
    // Return type of the function
    public TypeNode gives { get; set; }

    // Constructor to initialize a SignatureNode with identifier, parameters, and return type
    public SignatureNode (Node x, Node y, Node z)
    {
        id = (IdentifierNode)x;
        takes = (ParameterNode)y;
        gives = (TypeNode)z;
    }

    // Performs type checking for the function signature
    public override TypeNode typeCheck()
    {
        // Type check the parameters if they exist
        if (takes != null)
        {
            takes.typeCheck();
        }
        // Return the type of the function's return value
        return gives.typeCheck();
    }
    // Generates code for the function signature using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the SignatureNode
    public override string ToString()
    {
        return "S" + id + takes + gives;
    }
}

// Represents a list of type nodes in the AST
public class ListOfTypes : TypeNode
{
    // List of type nodes
    private List<TypeNode> types;

    // Constructor to initialize a ListOfTypes with a list of type nodes
    public ListOfTypes(List<TypeNode> list)
    {
        types = list;
    }

    // Returns the list of type nodes
    public List<TypeNode> getList()
    {
        return types;
    }
    // Generates code for the list of types (currently not implemented)
    public override void generate(CodeGeneratorVisitor cgv){}
    
    // Returns a string representation of the ListOfTypes (not implemented)
    public override string ToString()
    {
        throw new NotImplementedException();
    }
}

// Represents a number node in the AST
public class NumberNode : Node
{
    // Value of the number
    public double value { get; set; }

    // Constructor to initialize a NumberNode with a value
    public NumberNode(double x)
    {
        value = x;
    }

    // Performs type checking for the number node, returning a NumberTypeNode
    public override TypeNode typeCheck()
    {
        return new NumberTypeNode();
    }
    // Returns the type of the node, which is NumberTypeNode
    public override TypeNode Type()
    {
        return new NumberTypeNode();
    }
    // Generates code for the number node using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the NumberNode
    public override string ToString()
    {
        return "N" + value;
    }
}

// Represents a flag (boolean) node in the AST
public class FlagNode : Node
{
    // Value of the flag
    public bool value { get; set; }

    // Constructor to initialize a FlagNode with a value
    public FlagNode(bool x)
    {
        value = x;
    }

    // Performs type checking for the flag node, returning a FlagTypeNode
    public override TypeNode typeCheck()
    {
        return new FlagTypeNode();
    }
    // Returns the type of the node, which is FlagTypeNode
    public override TypeNode Type()
    {
        return new FlagTypeNode();
    }
    // Generates code for the flag node using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the FlagNode
    public override string ToString()
    {
        return "F" + value;
    }
}

// Represents a text node in the abstract syntax tree (AST)
public class TextNode : Node
{
    // Constructor to initialize a TextNode with a string value
    public TextNode(string x)
    {
        value = x; // Assign the string value to the node
    }
    // Property to get or set the text value of the node
    public string value { get; set; }

    // Performs type checking for the text node, returning a TextTypeNode
    public override TypeNode typeCheck()
    {
        return new TextTypeNode(); // Indicates this node represents text
    }
    // Returns the type of the node, which is TextTypeNode
    public override TypeNode Type()
    {
        return new TextTypeNode(); // Indicates this node represents text
    }
    // Generates code for the text node using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the TextNode
    public override string ToString()
    {
        return "T" + value; // Prefixes the value with "T" to indicate a text node
    }
}

// Represents a list element node in the AST
public class ListElementNode : Node
{
    // Identifier for the list
    public IdentifierNode id { get; set; }
    // Index of the element in the list
    public Node index;

    // Constructor to initialize a ListElementNode with an identifier and an index
    public ListElementNode(Node x, Node y)
    {
        id = (IdentifierNode)x; // Assign the identifier node
        index = y; // Assign the index node
    }
    // Performs type checking for the list element node
    public override TypeNode typeCheck()
    {
        TypeNode type = id.typeCheck(); // Check the type of the identifier
        if (type.GetType() == typeof(ListTypeNode)) // If the identifier is a list
        {
            ListTypeNode list = (ListTypeNode)type; // Cast the type to ListTypeNode
            return list.type; // Return the type of elements in the list
        }
        return type; // Return the type of the identifier if it's not a list
    }
    // Returns the type of the list element node
    public override TypeNode Type()
    {
        TypeNode type = id.Type(); // Get the type of the identifier
        if (type.GetType() == typeof(ListTypeNode)) // If the identifier is a list
        {
            ListTypeNode list = (ListTypeNode)type; // Cast the type to ListTypeNode
            return list.type; // Return the type of elements in the list
        }
        return type; // Return the type of the identifier if it's not a list
    }
    // Generates code for the list element node using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the ListElementNode
    public override string ToString()
    {
        return "LE" + id + index; // Prefixes the identifier and index with "LE" to indicate a list element node
    }
}

// Represents a node for adding an element to a list in the AST
public class AddToListNode : InFixNode
{
    // Index where the element should be added in the list
    public Node index;

    // Constructor to initialize an AddToListNode with left, right, and index nodes
    public AddToListNode(Node x, Node y, Node z)
    {
        left = x; // The element to be added
        right = y; // The list to which the element will be added
        index = z; // The position at which to add the element
    }

    // Performs type checking for adding an element to a list
    public override TypeNode typeCheck()
    {
        TypeNode leftType = left.typeCheck(); // Check the type of the element to be added
        TypeNode rightType = right.typeCheck(); // Check the type of the list
        // Ensure the list is of type ListTypeNode and the element's type matches the list's element type
        if (rightType.GetType() == typeof(ListTypeNode))
        {
            ListTypeNode list = (ListTypeNode)rightType;
            if (leftType.GetType() == list.type.GetType())
            {
                return leftType; // Return the type of the element being added
            }
        }
        throw new Exception("Cannot add a " + leftType.GetType() + " to a " + rightType.GetType());
    }

    // Returns null as this node does not have a specific type
    public override TypeNode Type()
    {
        return null;
    }

    // Generates code for adding an element to a list using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the AddToListNode
    public override string ToString()
    {
        return "ATL" + index; // Prefixes the index with "ATL" to indicate an AddToList operation
    }
}

// Represents an identifier node in the AST. Identifiers are used to name variables, functions, etc.
public class IdentifierNode : Node
{
    // The type of the identifier
    public TypeNode type { get; set; }
    // The name of the identifier
    public string name { get; set; }
    
    // Default constructor
    public IdentifierNode(){}

    // Constructor to initialize an IdentifierNode with a name
    public IdentifierNode(string name)
    {
        this.name = name; // Assign the name to the identifier
    }

    // Performs type checking for the identifier
    public override TypeNode typeCheck()
    {
        // Check if the variable exists and assign its type to this identifier
        if (CreateVariableNode.variableExists(name))
        {
            type = CreateVariableNode.getVariable(name).type;
            return type;
        }
        throw new Exception("Variable with name: " + name + " does not exist");
    }

    // Returns the type of the identifier
    public override TypeNode Type()
    {
        return type;
    }

    // Generates code for the identifier using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the IdentifierNode
    public override string ToString()
    {
        return "I" + name; // Prefixes the name with "I" to indicate an Identifier
    }
}

// Represents a break statement node in the AST. Used to exit loops or switch cases.
public class BreakNode : Node
{
    // Performs type checking for the break node
    public override TypeNode typeCheck()
    {
        return null; // Break does not have a type
    }

    // Returns null as break does not have a specific type
    public override TypeNode Type()
    {
        return null;
    }

    // Generates code for the break statement using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}
    
    // Returns a string representation of the BreakNode
    public override string ToString()
    {
        return "B"; // Indicates a Break operation
    }
}

// Represents a "give" statement node in the AST, used for returning values from functions or methods.
public class GiveNode : Node
{
    // The value to be returned by the give statement
    public Node value { get; set; }
    // The type of the value to be returned
    public TypeNode type { get; set; }

    // Constructor to initialize a GiveNode with a value
    public GiveNode(Node x)
    {
        value = x; // Assign the value to be returned
    }
    
    // Performs type checking for the give statement
    public override TypeNode typeCheck()
    {
        SignatureNode sign = ScopeNode.CurrentSignature; // Get the current function signature
        if (sign != null)
        {
            // Check if the type of the value matches the return type of the function
            if (value.typeCheck().GetType() == sign.gives.typeCheck().GetType())
            {
                ScopeNode.hasGive = true; // Indicate that a give statement has been used
                type = sign.gives; // Assign the return type
                return sign.gives; // Return the return type
            }

            throw new Exception("Bad typing in give, trying to give " + value.typeCheck() + " in a function that gives " + sign.gives);
        }
        throw new Exception("cannot use give outside function declaration");
    }

    // Returns null as this node does not have a specific type
    public override TypeNode Type()
    {
        return null;
    }

    // Generates code for the give statement using a visitor pattern
    public override void generate(CodeGeneratorVisitor cgv){cgv.visit(this);}

    // Returns a string representation of the GiveNode
    public override string ToString()
    {
        return "G" + value; // Prefixes the value with "G" to indicate a Give operation
    }
}