using System.Diagnostics;
using CSTtoASTandTypeChecker;
using Antlr4.Runtime;

namespace TranspilerTester;

public class IntegrationTester
{
    public static void testAll()
    {
        testTypeChecking();
        testCodeGenerator();
    }

    private static void testCodeGenerator()
    {
        CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
        Node node = ASTcase1();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        string expected = "public static boolean IsPalindrome(String s)\n{\npublic static Float i = 1F;;\nwhile(i < (float)s.length() / 2F){\nif(!(s.charAt((int)(i - 1) ) == s.charAt((int)((float)s.length() - i + 1F - 1) ))){\nreturn false;\n};\ni = i + 1F;};\nreturn true;\n}\npublic static void main(String[] args){\nSystem.out.println(\"Please write a word to check if it is a palindrome\");;\npublic static String word = new Scanner(System.in).nextLine();;\n;\nif(IsPalindrome(word)){\nSystem.out.println(\"your word is a palindrome\");\n}else{\nSystem.out.println(\"your word is not a palindrome\");\n}";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 1");
        TypeChecker.resetStaticVariables();
        
        cgv = new CodeGeneratorVisitor();
        node = ASTcase2();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        expected = "public static void main(String[] args){\npublic static List<String> shoppingList = new ArrayList<String>();;\nshoppingList.add(\"Bananas\");;\nshoppingList.add(\"Potatoes\");;\nshoppingList.add(\"Milk\");;\nshoppingList.add(\"Eggs\");;\npublic static Float counter = 1F;;\nSystem.out.println(\"The contents of the shopping list are:\");;\nwhile(counter <= (float)shoppingList.size()){\nSystem.out.println(\" - \" + shoppingList.get((int)(counter - 1)));;\ncounter = counter + 1F;}";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 2");
        TypeChecker.resetStaticVariables();
        
        cgv = new CodeGeneratorVisitor();
        node = ASTcase3();
        TypeChecker.typeCheck(node);
        node.generate(cgv);
        expected = "public static void main(String[] args){\npublic static String Text1 = \"It's \";;\npublic static String Text2 = \"so \";;\npublic static String Text3 = \"fluffy \";;\npublic static String Text4 = \"I'm \";;\npublic static String Text5 = \"gonna \";;\npublic static String Text6 = \"die! \";;\nSystem.out.println(Text1 + Text2 + Text3 + Text4 + Text5 + Text6);";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, case 3");
        TypeChecker.resetStaticVariables();
    }

    private static void testTypeChecking()
    {
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
        TypeChecker.resetStaticVariables();
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
        TypeChecker.resetStaticVariables();
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
        TypeChecker.resetStaticVariables();
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
        string input = File.ReadAllText(path);
        ICharStream stream = CharStreams.fromString(input);
        ITokenSource lexer = new SyntaxLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        SyntaxParser parser = new SyntaxParser(tokens);
        var CST = parser.program();
        return new CSTconverter().VisitProgram(CST);
    }
}