namespace CSTtoASTandTypeChecker;

public class CodeGeneratorVisitor
{
    public string output = "public static void main(String[] args){\n";

    internal void finish()
    {
        string temp = output;
        output = "package program;\n";
        output += "import java.util.*;\n";
        output += "public class Program{\n";
        output += temp;
        output += "\nnew Scanner(System.in).nextLine();";
        output += "\n}\n}";

        string path = @"program\program";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllText(path + @"\Program.java", output);
    }
    internal void visit(FunctionNode node)
    {
        string temp = output;
        output = "";
        node.signature.generate(this);
        output += "{\n";
        node.cmds.generate(this);
        output += "\n}\n";
        output += temp;
    }
    internal void visit(UseNode node)
    {
        node.id.generate(this);
        output += "(";
        if (node.inputs != null)
        {
            node.inputs.generate(this);
        }
        output += ")";
    }
    internal void visit(InputNode node)
    {
        node.left.generate(this);
        if (node.right != null)
        {
            output += ",";
            node.right.generate(this);
        }
    }
    internal void visit(ParameterNode node)
    {
        node.type.generate(this);
        output += " ";
        node.id.generate(this);

        if (node.next != null)
        {
            output += ",";
            node.next.generate(this);
        }
    }
    internal void visit(IfNode node)
    {
        output += "if(";
        node.condition.generate(this);
        output += "){\n";
        node.Body.generate(this);
        if (node.ElseBody != null)
        {
            output += "\n}else{\n";
            node.ElseBody.generate(this);
        }
        output += "\n}";
    }
    internal void visit(RepeatNode node)
    {
        output += "while(";
        node.condition.generate(this);
        output += "){\n";
        node.Body.generate(this);
        output += "}";
    }
    internal void visit(ReadNode node)
    {
        output += "new Scanner(System.in).nextLine()";
    }
    internal void visit(PrintNode node)
    {

        output += "System.out.println(";
        node.node.generate(this);
        output += ");";
    }
    internal void visit(LengthOfNode node)
    {
        output += "(float)";
        node.Identifier.generate(this);
        if (node.Identifier.type != null)
        {
            if (node.Identifier.Type().GetType() == typeof(TextTypeNode))
            {
                output += ".length()";
            }
            else
            {
                output += ".size()";
            }
        }
        else
        {
            output += ".length()";
        }
    }
    internal void visit(TypeConvertNode node)
    {
        if (node.type.GetType() == typeof(NumberTypeNode))
        {
            output += "Float.parseFloat(";
            node.node.generate(this);
            output += ")";
        }
        else if (node.type.GetType() == typeof(TextTypeNode))
        {
            output += "String.valueOf(";
            node.node.generate(this);
            output += ")";
        }
    }
    internal void visit(CommandNode node)
    {
        if (node.left != null)
        {
            node.left.generate(this);
            output += ";\n";
        }
        if (node.right != null)
        {
            node.right.generate(this);
            if (node.right.GetType() != typeof(CommandNode) && output[output.Length - 1] != '}' && output[output.Length - 1] != ';')
            {
                output += ";";
            }
        }
    }
    internal void visit(CreateVariableNode node)
    {
        //output += "public static ";
        node.type.generate(this);
        output += " ";
        node.name.generate(this);

        if (node.type.GetType() == typeof(ListTypeNode))
        {
            output += " = new ArrayList<";
            ListTypeNode list = (ListTypeNode)node.type;
            list.type.generate(this);
            output += ">()";
        }
        else if (node.value != null)
        {
            output += " = ";
            node.value.generate(this);
        }
        output += ";";
    }
    internal void visit(AssignNode node)
    {
        node.left.generate(this);
        output += " = ";
        node.right.generate(this);
        output += ";";
    }
    internal void visit(AdditionNode node)
    {
        node.left.generate(this);
        output += " + ";
        node.right.generate(this);
    }
    internal void visit(SubtractNode node)
    {
        node.left.generate(this);
        output += " - ";
        node.right.generate(this);
    }
    internal void visit(MultiplyNode node)
    {
        node.left.generate(this);
        output += " * ";
        node.right.generate(this);
    }
    internal void visit(DivideNode node)
    {
        node.left.generate(this);
        output += " / ";
        node.right.generate(this);
    }
    internal void visit(ModuloNode node)
    {
        node.left.generate(this);
        output += " % ";
        node.right.generate(this);
    }
    internal void visit(EqualsNode node)
    {
        node.left.generate(this);
        if (node.left.Type().GetType() == typeof(TextTypeNode))
        {
            if (node.left.GetType() != typeof(ListElementNode))
            {
                output += ".equals(";
                node.right.generate(this);
                output += ")";
            }
            else
            {
                output += " == ";
                node.right.generate(this);
            }
        }
        else
        {
            output += " == ";
            node.right.generate(this);
        }
    }
    internal void visit(GreaterNode node)
    {
        node.left.generate(this);
        output += " > ";
        node.right.generate(this);
    }
    internal void visit(GreaterEqualsNode node)
    {
        node.left.generate(this);
        output += " >= ";
        node.right.generate(this);
    }
    internal void visit(LessNode node)
    {
        node.left.generate(this);
        output += " < ";
        node.right.generate(this);
    }
    internal void visit(LessEqualsNode node)
    {
        node.left.generate(this);
        output += " <= ";
        node.right.generate(this);
    }
    internal void visit(AndNode node)
    {
        node.left.generate(this);
        output += " && ";
        node.right.generate(this);
    }
    internal void visit(OrNode node)
    {
        node.left.generate(this);
        output += " || ";
        node.right.generate(this);
    }
    internal void visit(NotNode node)
    {
        output += "!(";
        node.node.generate(this);
        output += ")";
    }
    internal void visit(NumberTypeNode node)
    {
        output += "Float";
    }
    internal void visit(FlagTypeNode node)
    {
        output += "boolean";
    }
    internal void visit(TextTypeNode node)
    {
        output += "String";
    }

    internal void visit(ListTypeNode node)
    {
        output += "List<";
        node.type.generate(this);
        output += ">";
    }
    internal void visit(NothingNode node)
    {
        output += "void";
    }
    internal void visit(SignatureNode node)
    {
        output += "public static ";
        node.gives.generate(this);
        output += " ";
        node.id.generate(this);
        output += "(";
        if (node.takes != null)
        {
            node.takes.generate(this);
        }
        output += ")\n";
    }
    internal void visit(NumberNode node)
    {
        output += node.value + "F";
    }

    internal void visit(FlagNode node)
    {
        output += node.value ? "true" : "false";    }
    internal void visit(TextNode node)
    {
        output += node.value;
    }
    internal void visit(ListElementNode node)
    {
        node.id.generate(this);
        if (node.id.Type().GetType() == typeof(TextTypeNode))
        {
            output += ".charAt((int)(";
            node.index.generate(this);
            output += " - 1) )";
        }
        else if (node.id.Type().GetType() == typeof(ListTypeNode))
        {
            output += ".get((int)(";
            node.index.generate(this);
            output += " - 1))";
        }
    }
    internal void visit(IdentifierNode node)
    {
        output += node.name;
    }
    internal void visit(BreakNode node)
    {
        output += "break;";
    }
    internal void visit(GiveNode node)
    {
        output += "return";
        if (node.type.GetType() != typeof(NothingNode))
        {
            output += " ";
            node.value.generate(this);
        }
        output += ";";
    }

    internal void visit(AddToListNode node)
    {
        if (node.index != null)
        {
            node.right.generate(this);
            output += ".add(";
            node.index.generate(this);
            output += ", ";
            node.left.generate(this);
            output += ")";
        }
        else
        {
            node.right.generate(this);
            output += ".add(";
            node.left.generate(this);
            output += ")";
        }
        output += ";";
    }
}