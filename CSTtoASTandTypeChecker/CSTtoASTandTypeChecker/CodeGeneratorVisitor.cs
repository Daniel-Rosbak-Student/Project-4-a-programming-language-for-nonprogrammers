﻿namespace CSTtoASTandTypeChecker;

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
        File.WriteAllText(@"..\\..\\..\\Output.txt", output);
    }
    //-------------------------------Daniel----------------------------------
    internal void visit(FunctionNode node)
    {
        string temp = output;
        output = "";
        node.signature.generate(this);
        output += "{";
        node.cmds.generate(this);
        output += "}";
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
        Type list = typeof(ListTypeNode);
        Type param = node.type.GetType();
        
        node.type.generate(this);
        output += " ";
        node.id.generate(this);
        if (param == list)
        {
            output += "[]";
        }

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
        output += "){";
        node.Body.generate(this);
        if (node.ElseBody != null)
        {
            output += "}else{";
            node.ElseBody.generate(this);
        }
        output += "}";
    }
    internal void visit(RepeatNode node)
    {
        output += "while(";
        node.condition.generate(this);
        output += "){";
        node.Body.generate(this);
        output += "}";
    }
    internal void visit(ReadNode node)
    {
        //string temp;
        //scanf("%s",temp);
        //
        //mogens = "mogens" + temp;
        //
        //TODO: somehow get the variable it is assigning to
        output += "scanf(\"%s\", %???)";
    }
    //--------------------------------Armin---------------------------------
    internal void visit(PrintNode node)
    {
        output += "printf(";
        node.node.generate(this);
        output += ")";
    }
    internal void visit(LengthOfNode node)
    {
        output += "sizeof(";
        node.Identifier.generate(this);
        output += ")";
    }
    internal void visit(TypeConvertNode node)
    {
        // Type casting, so you would say: (number)text for example
        output += "(";
        node.type.generate(this);
        output += ")";
        
        node.value.generate(this);
    }
    internal void visit(CommandNode node)
    {
        if (node.left != null)
        {
            node.left.generate(this);
            output += ";";
        }
        if (node.right != null)
        {
            node.right.generate(this);
            output += ";";
        }
    }
    internal void visit(CreateVariableNode node)
    {
        node.type.generate(this);
        output += " ";
        node.name.generate(this);
    
        if (node.value != null)
        {
            output += " = ";
            node.value.generate(this);
        }
    }
    internal void visit(AssignNode node)
    {
        node.left.generate(this);
        output += " = ";
        node.right.generate(this);
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
    //--------------------------------Vaalmigi---------------------------------
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
        output += " == ";
        node.right.generate(this);
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
    //--------------------------------Fatma---------------------------------
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
        output += "!";
        node.node.generate(this);
    }
    internal void visit(NumberTypeNode node)
    {
        output += "int";  
    }
    //--------------------------------Mathias---------------------------------
    internal void visit(FlagTypeNode node)
    {
        output += "int";
    }
    internal void visit(TextTypeNode node)
    {
        output += "char*";
    }
    internal void visit(ListTypeNode node)
    {
        node.type.generate(this);
        output += "*";
    }
    internal void visit(NothingNode node)
    {
        output += "void";
    }
    internal void visit(SignatureNode node)
    {
        node.gives.generate(this);
        output += " ";
        node.id.generate(this);
        output += "(";
        if (node.takes != null)
        {
            node.takes.generate(this);
        }
        output += ")";
    }
    internal void visit(NumberNode node)
    {
        output += node.value;
    }
    //--------------------------------Niklas---------------------------------
    
    internal void visit(FlagNode node)
    {
        output += node.value ? "1" : "0";    }
    internal void visit(TextNode node)
    {
        output += "\"" + node.value + "\"";
    }
    internal void visit(ListElementNode node)
    {
        node.id.generate(this);
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
        output += "return";
        if (node.value != null)
        {
            output += " ";
            node.value.generate(this);
        }
    }
}
