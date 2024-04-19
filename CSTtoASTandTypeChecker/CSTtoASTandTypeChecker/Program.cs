using System.Diagnostics;

namespace CSTtoASTandTypeChecker;

using Antlr4.Runtime;

public class Program
{
    static void Main(string[] args)
    {
        if (!File.Exists("ProgramInput.txt"))
        {
            File.Create("ProgramInput.txt");
            return;
        }
        string input = File.ReadAllText("ProgramInput.txt");
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

            CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
            AST.generate(cgv);
            cgv.finish();
            
            File.WriteAllText(@"program\Manifest.mf", "Main-Class: program/Program\n");
            
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c cd program&&javac program/Program.java&&jar cmf Manifest.mf Program.jar program/Program.class program/Program.java";
            cmd.Start();
            cmd.WaitForExit();
            File.WriteAllText(@"program\Run.bat", "java -jar Program.jar");
            
            File.Delete(@"program\program\Program.java");
            File.Delete(@"program\program\Program.class");
            File.Delete(@"program\Manifest.mf");
            if (File.Exists("Program.jar"))
            {
                File.Delete("Program.jar");
            }
            File.Move(@"program\Program.jar", "Program.jar");
            if (File.Exists("Run.bat"))
            {
                File.Delete("Run.bat");
            }
            File.Move(@"program\Run.bat", "Run.bat");
            if (!Directory.EnumerateFileSystemEntries("program/program").Any())
            {
                Directory.Delete("program/program");
            }
            if (!Directory.EnumerateFileSystemEntries("program").Any())
            {
                Directory.Delete("program");
            }
            Console.WriteLine("Done");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}