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
        Node node = new CommandNode(new FunctionNode(new SignatureNode(func, null, new NumberTypeNode()), new GiveNode(add, new NumberTypeNode())), new UseNode(func, null));
        node.generate(cgv);
        Console.WriteLine(cgv.output);
        string expected = "public static Float add()\n{\nreturn 5F + 14F;\n}\npublic static void main(String[] args){\n;\nadd();";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, function");

        
        cgv = new CodeGeneratorVisitor();
        IdentifierNode list = new IdentifierNode();
        list.name = "list";
        list.type = new ListTypeNode();
        AddToListNode addToList = new AddToListNode(new TextNode("\"Hello\""), list, null);
        ListElementNode listElement = new ListElementNode(list, new NumberNode(1));
        LessNode condition = new LessNode();
        condition.left = new NumberNode(0);
        condition.right = new LengthOfNode(list);
        CommandNode print = new CommandNode(new PrintNode(listElement), new BreakNode());
        RepeatNode repeat = new RepeatNode(condition, print);
        CommandNode commands = new CommandNode(addToList, repeat);
        commands.generate(cgv);
        expected = "public static void main(String[] args){\nlist.add(\"Hello\");;\n while(0F < (float)list.size()){\n System.out.println(list.get((int)(1F - 1)));;\n break;}";
        Console.WriteLine(cgv.output);
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, repeat");

        
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
}