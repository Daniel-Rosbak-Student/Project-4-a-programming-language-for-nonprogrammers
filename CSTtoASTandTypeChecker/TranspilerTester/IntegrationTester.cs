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
        Console.WriteLine(cgv.output);
        Debug.Assert(cgv.output == "", "Integration test failure: code genration, function");

        //Niklas



        //Vaal
        
        

    }
    //IntegrationTests
}