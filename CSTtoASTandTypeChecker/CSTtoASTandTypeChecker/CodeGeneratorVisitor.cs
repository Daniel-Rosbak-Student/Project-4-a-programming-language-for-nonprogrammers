namespace CSTtoASTandTypeChecker;

internal class CodeGeneratorVisitor
{
    private string output;

    internal CodeGeneratorVisitor()
    {
        output = "int main(){";
    }

    internal void finish()
    {
        string temp = output;
        output = "#include <stdio.h> \n ";
        output += temp + "}";
    }
    //-------------------------------Daniel----------------------------------
    internal void visit(FunctionNode node)
    {
        string temp = output;
        output = "";
        node.signature.gives.accept(this);
        output += " ";
        node.signature.id.accept(this);
        output += "(";
        if (node.signature.takes != null)
        {
            node.signature.takes.accept(this);
        }
        output += "){";
        node.cmds.accept(this);
        output += "}";
        output += temp;
    }
    internal void visit(UseNode node)
    {
        node.id.accept(this);
        output += "(";
        if (node.inputs != null)
        {
            node.inputs.accept(this);
        }
        output += ")";
    }
    internal void visit(InputNode node)
    {
        node.left.accept(this);
        if (node.right != null)
        {
            output += ",";
            node.right.accept(this);
        }
    }
    internal void visit(ParameterNode node)
    {
        Type list = typeof(ListTypeNode);
        Type param = node.type.GetType();

        if (param == list)
        {
            node.type.accept(this);
            output += " ";
            node.id.accept(this);
            output += "[]";
        }
        else
        {
            node.type.accept(this);
            output += " ";
            node.id.accept(this);
        }

        if (node.next != null)
        {
            output += ",";
            node.next.accept(this);
        }
    }
    internal void visit(IfNode node)
    {
        output += "if(";
        node.condition.accept(this);
        output += "){";
        node.Body.accept(this);
        if (node.ElseBody != null)
        {
            output += "}else{";
            node.ElseBody.accept(this);
        }
        output += "}";
    }
    internal void visit(RepeatNode node)
    {
        output += "while(";
        node.condition.accept(this);
        output += "){";
        node.Body.accept(this);
        output += "}";
    }
    internal void visit(ReadNode node)
    {
        //TODO: somehow get the variable it is assigning to
        output += "scanf(\"%s\", %???)";
    }
    //--------------------------------Armin---------------------------------
    internal void visit(PrintNode node)
    {
        output += "printf(";
        node.text.accept(this);
        output += ")";
    }
    internal void visit(LengthOfNode node)
    {
        output += "sizeof(";
        node.Identifier.accept(this);
        output += ")";
    }
    internal void visit(TypeConvertNode node)
    {
        // Type casting, so you would say: (number)text for example
        output += "(";
        node.type.accept(this);
        output += ")";
    
        if (node.value != null)
        {
            node.value.accept(this);
        }
    }
    internal void visit(CommandNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += ";";
        }
        if (node.right != null)
        {
            node.right.accept(this);
            output += ";";
        }
    }
    internal void visit(CreateVariableNode node)
    {
        node.type.accept(this);
        output += " ";
        node.name.accept(this);
    
        if (node.value != null)
        {
            output += " = ";
            node.value.accept(this);
        }
    }
    internal void visit(AssignNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " = ";
        }

        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(AdditionNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " + ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(SubtractNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " - ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    //--------------------------------Vaalmigi---------------------------------
    internal void visit(MultiplyNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " * ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(DivideNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " / ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(ModuloNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " % ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(EqualsNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " == ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(GreaterNode node)
    {
          if (node.left != null)
        {
            node.left.accept(this);
            output += " > ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }   
    }
    internal void visit(GreaterEqualsNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " >= ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    //--------------------------------Fatma---------------------------------
    internal void visit(LessNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " < ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(LessEqualsNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " <= ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(AndNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " && ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        } 
    }
    internal void visit(OrNode node)
    {
        if (node.left != null)
        {
            node.left.accept(this);
            output += " || ";
        }
        if (node.right != null)
        {
            node.right.accept(this);
        }
    }
    internal void visit(NotNode node)
    {
        output += "!";
        if (node != null && node is Node)
        {
            node.accept(this);
        }
    }
    internal void visit(NumberTypeNode node)
    {
        output += "int";  
    }
    //--------------------------------Mathias---------------------------------
    internal void visit(FlagTypeNode node)
    {
        output += node.value ? "true" : "false";
    }
    internal void visit(TextTypeNode node)
    {
        output += "\"" + node.value + "\"";
    }
    internal void visit(ListTypeNode node)
    {

    }
    internal void visit(NothingNode node)
    {
        output += "null";
    }
    internal void visit(SignatureNode node)
    {

    }
    internal void visit(ListOfTypes node)
    {

    }
    internal void visit(NumberNode node)
    {
        output += node.value;
    }
    //--------------------------------Niklas---------------------------------
    //yes?
    internal void visit(FlagNode node)
    {
        output += node.value ? "1" : "0";    }
    internal void visit(TextNode node)
    {
        output += node.value;
    }
    internal void visit(ListElementNode node)
    {
        output += "[" + node.index + "]";
    }
    internal void visit(IdentifierNode node)
    {
        output += node.name;
    }
    internal void visit(BreakNode node)
    {
        output += "break";
    }
    internal void visit(GiveNode node)
    {
        node.accept(this);
        output += "return " + node.value;
    }
}
