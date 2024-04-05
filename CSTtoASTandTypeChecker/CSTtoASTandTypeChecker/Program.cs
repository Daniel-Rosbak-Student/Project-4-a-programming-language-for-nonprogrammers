namespace CSTtoASTandTypeChecker;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public class Program
{
    static void Main(string[] args)
    {
        string input = File.ReadAllText(@"..\..\..\CFGtest.txt");
        Console.WriteLine(input);
        ICharStream stream = CharStreams.fromString(input);
        ITokenSource lexer = new SyntaxLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        SyntaxParser parser = new SyntaxParser(tokens);
        try
        {
            var CST = parser.program();
            var AST = new CSTconverter().VisitProgram(CST);
            typeChecker.typeCheck(AST);
            
            Console.WriteLine("SUCCESS");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}