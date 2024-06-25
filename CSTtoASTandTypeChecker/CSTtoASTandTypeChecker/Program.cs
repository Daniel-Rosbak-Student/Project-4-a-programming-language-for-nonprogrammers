using System.Diagnostics;

namespace CSTtoASTandTypeChecker;

using Antlr4.Runtime;

// The main class of the project that orchestrates the compilation process
// of the custom programming language.
public class Program
{
    // The entry point of the program. It reads the input from a file,
    // processes it through various stages (lexical analysis, parsing, AST conversion,
    // type checking, code generation), and finally compiles the generated code.
    static void Main(string[] args)
    {
        // Check if the input file exists, create it if not.
        if (!File.Exists("ProgramInput.txt"))
        {
            File.Create("ProgramInput.txt");
            return;
        }

        // Read the program input from the file.
        string input = File.ReadAllText("ProgramInput.txt");
        Console.WriteLine(input);

        // Lexical analysis: Convert the input string into a stream of tokens.
        ICharStream stream = CharStreams.fromString(input);
        ITokenSource lexer = new SyntaxLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);

        // Parsing: Parse the tokens to produce a parse tree (CST).
        SyntaxParser parser = new SyntaxParser(tokens);
        try
        {
            var CST = parser.program();

            // AST Conversion: Convert the CST to an AST for further processing.
            var AST = new CSTconverter().VisitProgram(CST);

            // Type Checking: Perform type checking on the AST.
            TypeChecker.typeCheck(AST);

            // Code Generation: Visit the AST to generate code.
            CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
            AST.generate(cgv);
            cgv.finish();

            // Compilation: Compile the generated code into an executable jar.
            PrepareAndCompileJavaCode();

            Console.WriteLine("Done");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    // Prepares the environment and compiles the generated Java code into a jar file.
    // It also cleans up the intermediate files and directories.
    private static void PrepareAndCompileJavaCode()
    {
        // Write the manifest file required for jar creation.
        File.WriteAllText(@"program\Manifest.mf", "Main-Class: program/Program\n");

        // Start the compilation process using command line commands.
        Process cmd = new Process();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.Arguments = "/c cd program&&javac program/Program.java&&jar cmf Manifest.mf Program.jar program/Program.class program/Program.java";
        cmd.Start();
        cmd.WaitForExit();

        // Prepare the batch file to run the jar.
        File.WriteAllText(@"program\Run.bat", "java -jar Program.jar");

        // Clean up intermediate files and directories.
        CleanupIntermediateFiles();
    }

    // Cleans up intermediate files and directories created during the compilation process.
    private static void CleanupIntermediateFiles()
    {
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
    }
}