using System.Diagnostics;
using CSTtoASTandTypeChecker;
using Antlr4.Runtime;

namespace TranspilerTester;

public class IntegrationTester
{
    public static void testAll()
    {
        testCSTconverter();
        testTypeChecking();
        testCodeGenerator();
    }

    private static void testCSTconverter()
    {
        Node node = ASTcase3();
        Node print = new PrintNode(new AdditionNode(new AdditionNode(new AdditionNode(new AdditionNode(new AdditionNode(new IdentifierNode("Text1"), new IdentifierNode("Text2")),new IdentifierNode("Text3")),new IdentifierNode("Text4")),new IdentifierNode("Text5")),new IdentifierNode("Text6")));
        Node create1 = new CreateVariableNode(new IdentifierNode("Text1"), new TextTypeNode(), new TextNode("\"It's \""));
        Node create2 = new CreateVariableNode(new IdentifierNode("Text2"), new TextTypeNode(), new TextNode("\"so \""));
        Node create3 = new CreateVariableNode(new IdentifierNode("Text3"), new TextTypeNode(), new TextNode("\"fluffy \""));
        Node create4 = new CreateVariableNode(new IdentifierNode("Text4"), new TextTypeNode(), new TextNode("\"I'm \""));
        Node create5 = new CreateVariableNode(new IdentifierNode("Text5"), new TextTypeNode(), new TextNode("\"gonna \""));
        Node create6 = new CreateVariableNode(new IdentifierNode("Text6"), new TextTypeNode(), new TextNode("\"die! \""));
        Node expected = new CommandNode(new CommandNode(create1,new CommandNode(create2, new CommandNode(create3, new CommandNode(create4, new CommandNode(create5, create6))))), print);
        Debug.Assert(ASTcompare(node, expected),"Integration test failure: CST converter, case 3");
    }
    
    private static void testTypeChecking()
    {
        //case 1
        bool success = true;
        try
        {
            TypeChecker.typeCheck(ASTcase1());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            success = false;
        }
        Debug.Assert(success, "Integration test failure: type checking, case 1");
        //case 2
        success = true;
        try
        {
            TypeChecker.typeCheck(ASTcase2());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            success = false;
        }
        Debug.Assert(success, "Integration test failure: type checking, case 2");
        //case 3
        success = true;
        try
        {
            TypeChecker.typeCheck(ASTcase3());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            success = false;
        }
        Debug.Assert(success, "Integration test failure: type checking, case 3");
    }
    
    private static void testCodeGenerator()
    {
        //case 1
        CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
        Node node = ASTcase1();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        string expected = "public static boolean IsPalindrome(String s)\n{\n Float i = 1F;;\nwhile(i < (float)s.length() / 2F){\nif(!(s.charAt((int)(i - 1) ) == s.charAt((int)((float)s.length() - i + 1F - 1) ))){\nreturn false;\n};\ni = i + 1F;};\nreturn true;\n}\n void main(String[] args){\nSystem.out.println(\"Please write a word to check if it is a palindrome\");;\n String word = new Scanner(System.in).nextLine();;\n;\nif(IsPalindrome(word)){\nSystem.out.println(\"your word is a palindrome\");\n}else{\nSystem.out.println(\"your word is not a palindrome\");\n}";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 1");
        //case 2
        cgv = new CodeGeneratorVisitor();
        node = ASTcase2();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        expected = "public static void main(String[] args){\n List<String> shoppingList = new ArrayList<String>();;\nshoppingList.add(\"Bananas\");;\nshoppingList.add(\"Potatoes\");;\nshoppingList.add(\"Milk\");;\nshoppingList.add(\"Eggs\");;\n Float counter = 1F;;\nSystem.out.println(\"The contents of the shopping list are:\");;\nwhile(counter <= (float)shoppingList.size()){\nSystem.out.println(\" - \" + shoppingList.get((int)(counter - 1)));;\ncounter = counter + 1F;}";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 2");
        //case 3
        cgv = new CodeGeneratorVisitor();
        node = ASTcase3();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        expected = "public static void main(String[] args){\n String Text1 = \"It's \";;\n String Text2 = \"so \";;\n String Text3 = \"fluffy \";;\n String Text4 = \"I'm \";;\n String Text5 = \"gonna \";;\n String Text6 = \"die! \";;\nSystem.out.println(Text1 + Text2 + Text3 + Text4 + Text5 + Text6);";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 3");
    }

    private static Node ASTcase1()
    {
        return getAST(@"..\..\..\..\CSTtoASTandTypeChecker\Palindrome.txt");
    }
    
    private static Node ASTcase2()
    {
        return getAST(@"..\..\..\..\CSTtoASTandTypeChecker\ShoppingList.txt");

    }
    
    private static Node ASTcase3()
    {
        return getAST(@"..\..\..\..\CSTtoASTandTypeChecker\AddStringProgram.txt");
    }

    private static Node getAST(string path)
    {
        TypeChecker.resetStaticVariables();
        string input = File.ReadAllText(path);
        ICharStream stream = CharStreams.fromString(input);
        ITokenSource lexer = new SyntaxLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        SyntaxParser parser = new SyntaxParser(tokens);
        var CST = parser.program();
        return new CSTconverter().VisitProgram(CST);
    }

    private static bool ASTcompare(Node tree1, Node tree2)
    {
        return tree1.ToString().Equals(tree2.ToString());
    }
}