using System.Diagnostics;
using CSTtoASTandTypeChecker;
using Antlr4.Runtime;

// Defines the namespace for the integration tester within the transpiler testing suite.
namespace TranspilerTester;

// IntegrationTester class encapsulates methods for testing the integration of various components of the transpiler.
public class IntegrationTester
{
    // testAll method serves as the main entry point for running all integration tests.
    public static void testAll()
    {
        testCSTconverter(); // Tests the CST to AST conversion process.
        testTypeChecking(); // Tests the type checking functionality.
        testCodeGenerator(); // Tests the code generation from AST.
    }

    // Tests the conversion of Concrete Syntax Tree (CST) to Abstract Syntax Tree (AST).
    private static void testCSTconverter()
    {
        // Constructs an AST for a specific test case.
        Node node = ASTcase3();
        // Constructs the expected AST manually for comparison.
        Node print = new PrintNode(new AdditionNode(new AdditionNode(new AdditionNode(new AdditionNode(new AdditionNode(new IdentifierNode("Text1"), new IdentifierNode("Text2")),new IdentifierNode("Text3")),new IdentifierNode("Text4")),new IdentifierNode("Text5")),new IdentifierNode("Text6")));
        Node create1 = new CreateVariableNode(new IdentifierNode("Text1"), new TextTypeNode(), new TextNode("\"It's \""));
        Node create2 = new CreateVariableNode(new IdentifierNode("Text2"), new TextTypeNode(), new TextNode("\"so \""));
        Node create3 = new CreateVariableNode(new IdentifierNode("Text3"), new TextTypeNode(), new TextNode("\"fluffy \""));
        Node create4 = new CreateVariableNode(new IdentifierNode("Text4"), new TextTypeNode(), new TextNode("\"I'm \""));
        Node create5 = new CreateVariableNode(new IdentifierNode("Text5"), new TextTypeNode(), new TextNode("\"gonna \""));
        Node create6 = new CreateVariableNode(new IdentifierNode("Text6"), new TextTypeNode(), new TextNode("\"die! \""));
        Node expected = new CommandNode(new CommandNode(create1,new CommandNode(create2, new CommandNode(create3, new CommandNode(create4, new CommandNode(create5, create6))))), print);
        // Asserts that the generated AST matches the expected AST.
        Debug.Assert(ASTcompare(node, expected),"Integration test failure: CST converter, case 3");
    }
    
    // Tests the type checking functionality of the transpiler.
    private static void testTypeChecking()
    {
        // Case 1: Tests type checking on a specific AST.
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

        // Repeats the process for additional test cases.
        // Case 2
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

        // Case 3
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
    
    // Tests the code generation functionality, ensuring the AST is correctly translated into target language code.
    private static void testCodeGenerator()
    {
        // Case 1: Generates code for a specific AST and compares it to the expected output.
        CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
        Node node = ASTcase1();
        TypeChecker.typeCheck(node); // Ensures the AST passes type checking before code generation.
        node.generate(cgv);
        string expected = "public static boolean IsPalindrome(String s)\n{\n Float i = 1F;;\nwhile(i < (float)s.length() / 2F){\nif(!(s.charAt((int)(i - 1) ) == s.charAt((int)((float)s.length() - i + 1F - 1) ))){\nreturn false;\n};\ni = i + 1F;};\nreturn true;\n}\n void main(String[] args){\nSystem.out.println(\"Please write a word to check if it is a palindrome\");;\n String word = new Scanner(System.in).nextLine();;\n;\nif(IsPalindrome(word)){\nSystem.out.println(\"your word is a palindrome\");\n}else{\nSystem.out.println(\"your word is not a palindrome\");\n}";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 1");

        // Repeats the process for additional test cases.
        // Case 2
        cgv = new CodeGeneratorVisitor();
        node = ASTcase2();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        expected = "public static void main(String[] args){\n List<String> shoppingList = new ArrayList<String>();;\nshoppingList.add(\"Bananas\");;\nshoppingList.add(\"Potatoes\");;\nshoppingList.add(\"Milk\");;\nshoppingList.add(\"Eggs\");;\n Float counter = 1F;;\nSystem.out.println(\"The contents of the shopping list are:\");;\nwhile(counter <= (float)shoppingList.size()){\nSystem.out.println(\" - \" + shoppingList.get((int)(counter - 1)));;\ncounter = counter + 1F;}";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 2");

        // Case 3
        cgv = new CodeGeneratorVisitor();
        node = ASTcase3();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        expected = "public static void main(String[] args){\n String Text1 = \"It's \";;\n String Text2 = \"so \";;\n String Text3 = \"fluffy \";;\n String Text4 = \"I'm \";;\n String Text5 = \"gonna \";;\n String Text6 = \"die! \";;\nSystem.out.println(Text1 + Text2 + Text3 + Text4 + Text5 + Text6);";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 3");
    }

    // Helper methods for constructing specific ASTs from source files.
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

    // Reads a source file and converts its content into an AST.
    private static Node getAST(string path)
    {
        TypeChecker.resetStaticVariables(); // Resets static variables to ensure a clean state.
        string input = File.ReadAllText(path); // Reads the file content.
        ICharStream stream = CharStreams.fromString(input); // Converts the string into a char stream.
        ITokenSource lexer = new SyntaxLexer(stream); // Lexical analysis.
        ITokenStream tokens = new CommonTokenStream(lexer); // Token stream.
        SyntaxParser parser = new SyntaxParser(tokens); // Syntax parsing.
        var CST = parser.program(); // Parses the program to produce a CST.
        return new CSTconverter().VisitProgram(CST); // Converts CST to AST.
    }

    // Compares two ASTs for equality based on their string representations.
    private static bool ASTcompare(Node tree1, Node tree2)
    {
        return tree1.ToString().Equals(tree2.ToString());
    }
}