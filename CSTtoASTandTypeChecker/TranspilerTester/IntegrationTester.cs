﻿using System.Diagnostics;
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
        string expected = "public static Float add()\n{\nreturn 5F + 14F;\n}\npublic static void main(String[] args){\n";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, function");

        //Niklas
        //List, while, text, break, length of, print to screen
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


        // if, flag(boolean), and, create, get user input
        //Vaal
        cgv = new CodeGeneratorVisitor();



        node.generate(cgv);
        expected = "";
        Debug.Assert(cgv.output.Equals(expected), "Integration test failure: code generation, function");
    }
    //IntegrationTests
}