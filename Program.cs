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
        IParseTree tree = parser.program();
    }
}