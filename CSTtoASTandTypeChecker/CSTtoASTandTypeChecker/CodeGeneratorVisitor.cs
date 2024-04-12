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
        string temp = "#include <stdio.h> \n ";
        temp += output + "}";
        output = temp;
    }
    //-------------------------------Daniel----------------------------------
    internal void visit(FunctionNode node)
    {
        
    }
    internal void visit(UseNode node)
    {
        
    }
    internal void visit(InputNode node)
    {
        
    }
    internal void visit(ParameterNode node)
    {
        
    }
    internal void visit(IfNode node)
    {
        
    }
    internal void visit(RepeatNode node)
    {
        
    }
    internal void visit(ReadNode node)
    {
        
    }
    //--------------------------------Armin---------------------------------
    internal void visit(PrintNode node)
    {
        
    }
    internal void visit(LengthOfNode node)
    {
        
    }
    internal void visit(TypeConvertNode node)
    {
        
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
        
    }
    internal void visit(AssignNode node)
    {
        
    }
    internal void visit(AdditionNode node)
    {
        
    }
    internal void visit(SubtractNode node)
    {
        
    }
    //--------------------------------Vaalmigi---------------------------------
    internal void visit(MultiplyNode node)
    {
        
    }
    internal void visit(DivideNode node)
    {
        
    }
    internal void visit(ModuloNode node)
    {
        
    }
    internal void visit(EqualsNode node)
    {
        
    }
    internal void visit(GreaterNode node)
    {
        
    }
    internal void visit(GreaterEqualsNode node)
    {
        
    }
    //--------------------------------Fatma---------------------------------
    internal void visit(LessNode node)
    {
        
    }
    internal void visit(LessEqualsNode node)
    {
        
    }
    internal void visit(AndNode node)
    {
        
    }
    internal void visit(OrNode node)
    {
        
    }
    internal void visit(NotNode node)
    {
        
    }
    internal void visit(NumberTypeNode node)
    {
        
    }
    //--------------------------------Mathias---------------------------------
    internal void visit(FlagTypeNode node)
    {
        
    }
    internal void visit(TextTypeNode node)
    {
        
    }
    internal void visit(ListTypeNode node)
    {
        
    }
    internal void visit(NothingNode node)
    {
        
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
    internal void visit(FlagNode node)
    {
        
    }
    internal void visit(TextNode node)
    {
        
    }
    internal void visit(ListElementNode node)
    {
        
    }
    internal void visit(IdentifierNode node)
    {
        
    }
    internal void visit(BreakNode node)
    {
        
    }
    internal void visit(GiveNode node)
    {
        
    }
}