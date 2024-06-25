namespace CSTtoASTandTypeChecker;

// Defines the namespace for the CodeGeneratorVisitor class. This namespace is used to organize classes related to converting CST to AST and type checking within the project.

public class CodeGeneratorVisitor
{
    // This class is responsible for visiting different types of nodes in the AST and generating Java code based on the structure and semantics of those nodes.

    public string output = "public static void main(String[] args){\n";
    // Initializes the output string with the beginning of the main method in Java. This is where the generated Java code will be accumulated as the AST is traversed.

    internal void finish()
    {
        // This method finalizes the Java code generation process.
        string temp = output;
        // Temporarily stores the current state of the output.

        output = "package program;\n";
        output += "import java.util.*;\n";
        output += "public class Program{\n";
        // Resets the output to include the package declaration, necessary imports, and the start of the Program class definition.

        output += temp;
        // Appends the previously generated code (stored in temp) to the output.

        output += "\nnew Scanner(System.in).nextLine();";
        output += "\n}\n}";
        // Adds a line to pause the program execution by waiting for an input, and then closes the class and method definitions.

        string path = @"program\program";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        // Checks if the directory for the generated program exists, and creates it if it doesn't.

        File.WriteAllText(path + @"\Program.java", output);
        // Writes the generated Java code to a file named Program.java in the specified directory.
    }

    // The following visit methods are called for different types of nodes in the AST. Each method generates Java code specific to the type of node it visits by appending to the `output` string.

    // Visits a FunctionNode in the AST. This method is responsible for generating Java code for a function definition.
    internal void visit(FunctionNode node)
    {
        string temp = output; // Temporarily stores the current output.
        output = ""; // Resets the output to start generating the function definition.
        node.signature.generate(this); // Generates the function signature (return type, name, parameters).
        output += "{\n"; // Adds the opening brace for the function body.
        node.cmds.generate(this); // Generates the code for the commands/statements inside the function body.
        output += "\n}\n"; // Adds the closing brace for the function body.
        output += temp; // Appends the temporarily stored output back, preserving any code generated before this function.
    }

    // Visits a UseNode in the AST. This method generates Java code for a function call or variable use.
    internal void visit(UseNode node)
    {
        node.id.generate(this); // Generates the identifier (function name or variable name).
        output += "("; // Adds an opening parenthesis for the function call parameters.
        if (node.inputs != null)
        {
            node.inputs.generate(this); // Generates the code for the function call parameters if any.
        }
        output += ")"; // Adds a closing parenthesis for the function call parameters.
    }

    // Visits an InputNode in the AST. This method is responsible for generating Java code for handling input parameters.
    internal void visit(InputNode node)
    {
        node.left.generate(this); // Generates the code for the left-hand side of the input (could be a variable or another input).
        if (node.right != null)
        {
            output += ","; // Adds a comma to separate multiple inputs.
            node.right.generate(this); // Generates the code for the right-hand side of the input if it exists.
        }
    }

    // Visits a ParameterNode in the AST. This method generates Java code for function/method parameters.
    internal void visit(ParameterNode node)
    {
        node.type.generate(this); // Generates the code for the parameter's type.
        output += " "; // Adds a space between the type and the identifier.
        node.id.generate(this); // Generates the code for the parameter's identifier (name).

        if (node.next != null)
        {
            output += ","; // Adds a comma to separate this parameter from the next one, if any.
            node.next.generate(this); // Recursively generates the code for the next parameter.
        }
    }
        // Visits an IfNode in the AST. Generates Java code for an if statement, including condition, body, and optional else body.
    internal void visit(IfNode node)
    {
        output += "if("; // Starts the if statement with the condition.
        node.condition.generate(this); // Generates the Java code for the condition expression.
        output += "){\n"; // Opens the block for the if body.
        node.Body.generate(this); // Generates the Java code for the statements inside the if body.
        if (node.ElseBody != null) // Checks if there is an else body.
        {
            output += "\n}else{\n"; // Starts the else block.
            node.ElseBody.generate(this); // Generates the Java code for the statements inside the else body.
        }
        output += "\n}"; // Closes the if (and else) block.
    }

    // Visits a RepeatNode in the AST. Generates Java code for a while loop.
    internal void visit(RepeatNode node)
    {
        output += "while("; // Starts the while loop with the condition.
        node.condition.generate(this); // Generates the Java code for the loop condition.
        output += "){\n"; // Opens the block for the loop body.
        node.Body.generate(this); // Generates the Java code for the statements inside the loop body.
        output += "}"; // Closes the loop block.
    }

    // Visits a ReadNode in the AST. Generates Java code for reading input from the console.
    internal void visit(ReadNode node)
    {
        output += "new Scanner(System.in).nextLine()"; // Uses Scanner to read a line from the console.
    }

    // Visits a PrintNode in the AST. Generates Java code for printing to the console.
    internal void visit(PrintNode node)
    {
        output += "System.out.println("; // Starts the print statement.
        node.node.generate(this); // Generates the Java code for the expression to be printed.
        output += ");"; // Closes the print statement.
    }

    // Visits a LengthOfNode in the AST. Generates Java code for getting the length or size of a collection or string.
    internal void visit(LengthOfNode node)
    {
        output += "(float)"; // Casts the result to float for consistency in the generated code.
        node.Identifier.generate(this); // Generates the Java code for the identifier whose length/size is being accessed.
        if (node.Identifier.type != null) // Checks if the type of the identifier is known.
        {
            if (node.Identifier.Type().GetType() == typeof(TextTypeNode)) // If the identifier is of type Text.
            {
                output += ".length()"; // Uses .length() for strings.
            }
            else // For other collection types.
            {
                output += ".size()"; // Uses .size() for collections.
            }
        }
        else // If the type is not known, defaults to using .length().
        {
            output += ".length()";
        }
    }
        // Visits a TypeConvertNode in the AST. This method is responsible for generating Java code for type conversion.
    internal void visit(TypeConvertNode node)
    {
        if (node.type.GetType() == typeof(NumberTypeNode)) // Checks if the target type is Number.
        {
            output += "Float.parseFloat("; // Uses Float.parseFloat for converting to a float.
            node.node.generate(this); // Generates the code for the value to be converted.
            output += ")"; // Closes the conversion method call.
        }
        else if (node.type.GetType() == typeof(TextTypeNode)) // Checks if the target type is Text.
        {
            output += "String.valueOf("; // Uses String.valueOf for converting to a string.
            node.node.generate(this); // Generates the code for the value to be converted.
            output += ")"; // Closes the conversion method call.
        }
    }

    // Visits a CommandNode in the AST. This method is responsible for generating Java code for executing commands.
    internal void visit(CommandNode node)
    {
        if (node.left != null) // Checks if there is a left child node (command) to process.
        {
            node.left.generate(this); // Generates the code for the left child node.
            output += ";\n"; // Adds a semicolon and newline to separate commands.
        }
        if (node.right != null) // Checks if there is a right child node (command) to process.
        {
            node.right.generate(this); // Generates the code for the right child node.
            // Ensures the command ends with a semicolon unless it's another command node or ends with a closing brace.
            if (node.right.GetType() != typeof(CommandNode) && output[output.Length - 1] != '}' && output[output.Length - 1] != ';')
            {
                output += ";";
            }
        }
    }

    // Visits a CreateVariableNode in the AST. This method is responsible for generating Java code for variable declaration and initialization.
    internal void visit(CreateVariableNode node)
    {
        // Uncommented to not restrict variable scope to public static.
        //output += "public static ";
        node.type.generate(this); // Generates the code for the variable's type.
        output += " "; // Adds a space between the type and the variable name.
        node.name.generate(this); // Generates the code for the variable name.

        if (node.type.GetType() == typeof(ListTypeNode)) // Checks if the variable type is a List.
        {
            output += " = new ArrayList<"; // Initializes the variable as a new ArrayList.
            ListTypeNode list = (ListTypeNode)node.type;
            list.type.generate(this); // Generates the code for the type of elements in the List.
            output += ">()"; // Closes the ArrayList initialization.
        }
        else if (node.value != null) // Checks if the variable is initialized with a value.
        {
            output += " = "; // Adds the assignment operator.
            node.value.generate(this); // Generates the code for the value to be assigned.
        }
        output += ";"; // Adds a semicolon to end the variable declaration/initialization.
    }
        // Visits an AssignNode in the AST. This method generates Java code for variable assignment.
    internal void visit(AssignNode node)
    {
        node.left.generate(this); // Generates the code for the left-hand side (variable name).
        output += " = "; // Adds the assignment operator.
        node.right.generate(this); // Generates the code for the right-hand side (value to be assigned).
        output += ";"; // Ends the statement with a semicolon.
    }

    // Visits an AdditionNode in the AST. This method generates Java code for addition operation.
    internal void visit(AdditionNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " + "; // Adds the addition operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a SubtractNode in the AST. This method generates Java code for subtraction operation.
    internal void visit(SubtractNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " - "; // Adds the subtraction operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a MultiplyNode in the AST. This method generates Java code for multiplication operation.
    internal void visit(MultiplyNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " * "; // Adds the multiplication operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a DivideNode in the AST. This method generates Java code for division operation.
    internal void visit(DivideNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " / "; // Adds the division operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a ModuloNode in the AST. This method generates Java code for modulo operation.
    internal void visit(ModuloNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " % "; // Adds the modulo operator.
        node.right.generate(this); // Generates the code for the right operand.
    }
        // Visits an EqualsNode in the AST. This method generates Java code for equality comparison.
    internal void visit(EqualsNode node)
    {
        node.left.generate(this); // Generates the code for the left operand of the equality comparison.
        if (node.left.Type().GetType() == typeof(TextTypeNode)) // Checks if the left operand is of type Text.
        {
            if (node.left.GetType() != typeof(ListElementNode)) // Checks if the left operand is not a ListElementNode.
            {
                output += ".equals("; // Uses .equals method for string comparison in Java.
                node.right.generate(this); // Generates the code for the right operand.
                output += ")"; // Closes the .equals method call.
            }
            else // If the left operand is a ListElementNode.
            {
                output += " == "; // Uses == for comparison, which is not typical for strings but may apply for list elements comparison.
                node.right.generate(this); // Generates the code for the right operand.
            }
        }
        else // If the left operand is not of type Text.
        {
            output += " == "; // Uses == for equality comparison of non-string types.
            node.right.generate(this); // Generates the code for the right operand.
        }
    }
        // Visits a GreaterNode in the AST. Generates Java code for the '>' operation.
    internal void visit(GreaterNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " > "; // Adds the greater than operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a GreaterEqualsNode in the AST. Generates Java code for the '>=' operation.
    internal void visit(GreaterEqualsNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " >= "; // Adds the greater than or equal to operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a LessNode in the AST. Generates Java code for the '<' operation.
    internal void visit(LessNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " < "; // Adds the less than operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a LessEqualsNode in the AST. Generates Java code for the '<=' operation.
    internal void visit(LessEqualsNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " <= "; // Adds the less than or equal to operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits an AndNode in the AST. Generates Java code for the logical '&&' operation.
    internal void visit(AndNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " && "; // Adds the logical AND operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits an OrNode in the AST. Generates Java code for the logical '||' operation.
    internal void visit(OrNode node)
    {
        node.left.generate(this); // Generates the code for the left operand.
        output += " || "; // Adds the logical OR operator.
        node.right.generate(this); // Generates the code for the right operand.
    }

    // Visits a NotNode in the AST. Generates Java code for the logical '!' operation.
    internal void visit(NotNode node)
    {
        output += "!"; // Adds the logical NOT operator.
        output += "("; // Opens a parenthesis for the NOT operation.
        node.node.generate(this); // Generates the code for the operand.
        output += ")"; // Closes the parenthesis.
    }

    // Visits a NumberTypeNode in the AST. Specifies the Java type 'Float' for number variables.
    internal void visit(NumberTypeNode node)
    {
        output += "Float"; // Specifies the Java Float type.
    }

    // Visits a FlagTypeNode in the AST. Specifies the Java type 'boolean' for flag variables.
    internal void visit(FlagTypeNode node)
    {
        output += "boolean"; // Specifies the Java boolean type.
    }

    // Visits a TextTypeNode in the AST. Specifies the Java type 'String' for text variables.
    internal void visit(TextTypeNode node)
    {
        output += "String"; // Specifies the Java String type.
    }

        // Visits a ListTypeNode in the AST. Generates Java code for list type declaration.
    internal void visit(ListTypeNode node)
    {
        output += "List<"; // Begins list type declaration with generic type.
        node.type.generate(this); // Generates the type of elements in the list.
        output += ">"; // Closes the generic type declaration.
    }

    // Visits a NothingNode in the AST. Generates Java code for a method that returns nothing (void).
    internal void visit(NothingNode node)
    {
        output += "void"; // Specifies that the method returns nothing.
    }

    // Visits a SignatureNode in the AST. Generates Java code for method signatures.
    internal void visit(SignatureNode node)
    {
        output += "public static "; // Specifies the method as public and static.
        node.gives.generate(this); // Generates the return type of the method.
        output += " "; // Adds a space between the return type and the method name.
        node.id.generate(this); // Generates the method name.
        output += "("; // Opens the parameter list.
        if (node.takes != null) // Checks if there are parameters.
        {
            node.takes.generate(this); // Generates the parameter list.
        }
        output += ")\n"; // Closes the parameter list and adds a newline.
    }

    // Visits a NumberNode in the AST. Generates Java code for number literals.
    internal void visit(NumberNode node)
    {
        output += node.value + "F"; // Appends 'F' to specify a float literal in Java.
    }

    // Visits a FlagNode in the AST. Generates Java code for boolean values.
    internal void visit(FlagNode node)
    {
        output += node.value ? "true" : "false"; // Outputs 'true' or 'false' based on the boolean value.
    }

    // Visits a TextNode in the AST. Generates Java code for text literals.
    internal void visit(TextNode node)
    {
        output += node.value; // Outputs the text value.
    }

    // Visits a ListElementNode in the AST. Generates Java code for accessing list elements.
    internal void visit(ListElementNode node)
    {
        node.id.generate(this); // Generates the code for the list identifier.
        if (node.id.Type().GetType() == typeof(TextTypeNode)) // Checks if the list elements are of type Text.
        {
            output += ".charAt((int)("; // Uses charAt for accessing characters in a string.
            node.index.generate(this); // Generates the code for the index (1-based).
            output += " - 1) )"; // Adjusts the index to 0-based and closes the expression.
        }
        else if (node.id.Type().GetType() == typeof(ListTypeNode)) // Checks if it's a list of elements.
        {
            output += ".get((int)("; // Uses get for accessing elements in a list.
            node.index.generate(this); // Generates the code for the index (1-based).
            output += " - 1))"; // Adjusts the index to 0-based and closes the expression.
        }
    }
        // Visits an IdentifierNode in the AST. Generates Java code for identifiers (variable names, method names, etc.).
    internal void visit(IdentifierNode node)
    {
        output += node.name; // Appends the identifier's name to the output.
    }

    // Visits a BreakNode in the AST. Generates Java code for a break statement.
    internal void visit(BreakNode node)
    {
        output += "break;"; // Appends the break statement to the output.
    }

    // Visits a GiveNode in the AST. Generates Java code for return statements.
    internal void visit(GiveNode node)
    {
        output += "return"; // Begins the return statement.
        if (node.type.GetType() != typeof(NothingNode)) // Checks if the return type is not void.
        {
            output += " "; // Adds a space before the return value.
            node.value.generate(this); // Generates the return value.
        }
        output += ";"; // Ends the return statement with a semicolon.
    }

    // Visits an AddToListNode in the AST. Generates Java code for adding elements to a list.
    internal void visit(AddToListNode node)
    {
        if (node.index != null) // Checks if an index is specified for insertion.
        {
            node.right.generate(this); // Generates the code for the list identifier.
            output += ".add("; // Begins the add method call.
            node.index.generate(this); // Generates the index at which to insert.
            output += ", "; // Separates the index and the element to be added.
            node.left.generate(this); // Generates the element to be added.
            output += ")"; // Closes the add method call.
        }
        else // If no index is specified, add to the end of the list.
        {
            node.right.generate(this); // Generates the code for the list identifier.
            output += ".add("; // Begins the add method call.
            node.left.generate(this); // Generates the element to be added.
            output += ")"; // Closes the add method call.
        }
        output += ";"; // Ends the statement with a semicolon.
    }
}