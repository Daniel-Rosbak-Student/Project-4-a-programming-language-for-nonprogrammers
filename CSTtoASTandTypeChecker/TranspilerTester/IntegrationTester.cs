using System.Diagnostics;
using CSTtoASTandTypeChecker;

namespace TranspilerTester;

public class IntegrationTester
{
    public static void testAll()
    {
        testCodeGenerator();
        //call all testmethods in the class
    }

    private static void testCodeGenerator()
    {
        //function, signature, Identifier, addition, return
        CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
        IdentifierNode id = new IdentifierNode();
        id.name = "variables";
        AdditionNode add = new AdditionNode();
        add.left = new NumberNode(5);
        add.right = new NumberNode(14);
        IdentifierNode func = new IdentifierNode();
        func.name = "add";
        Node node = new FunctionNode(new SignatureNode(func, null, new NumberTypeNode()), new GiveNode(add, new NumberTypeNode()));
        node.generate(cgv);
        string expected = "public static Float add()\n{\nreturn 5F + 14F;\n}\npublic static void main(String[] args){\n";
        //Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, function");

        //Niklas
        //List, while, text, break, length of, print to screen
        cgv = new CodeGeneratorVisitor();
        
        
        
        node.generate(cgv);
        expected = "";
        //Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, function");
       
        // if, flag(boolean), and, create, get user input
        //Vaal
        cgv = new CodeGeneratorVisitor();
        
        IdentifierNode i = new IdentifierNode();
        GreaterNode ifGreaterCondition = new GreaterNode();
        ifGreaterCondition.left = i;
        ifGreaterCondition.right = new NumberNode(0);
        LessNode ifLessCondition = new LessNode();
        ifLessCondition.left = i;
        ifLessCondition.right = new NumberNode(10);
        IdentifierNode hej = new IdentifierNode();
        TypeNode variableType = new FlagTypeNode();
        AndNode andCondition = new AndNode();
        andCondition.left = ifGreaterCondition;
        andCondition.right = ifLessCondition;
        IfNode ifStatement = new IfNode(andCondition, new CreateVariableNode(hej,variableType,new ReadNode()), null);
        
        ifStatement.generate(cgv);
        expected = "public static void main(String[] args){\nif( > 0F &&  < 10F){\npublic static boolean  = new Scanner(System.in).nextLine();\n}\n";
        Console.WriteLine(cgv.output);
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, if");
    }
    //IntegrationTests
}