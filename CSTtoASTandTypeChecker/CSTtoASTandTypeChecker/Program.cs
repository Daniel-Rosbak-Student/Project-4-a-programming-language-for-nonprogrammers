using System.Diagnostics;

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
            TypeChecker.typeCheck(AST);
            
            //CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
            //AST.generate(cgv);
            //cgv.finish();

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c cd ..\\..\\..\\&gcc Output.c";
            cmd.Start();
            cmd.WaitForExit();
            //Console.WriteLine(cmd.StandardOutput.ReadToEnd());
            Console.WriteLine("SUCCESS");
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}