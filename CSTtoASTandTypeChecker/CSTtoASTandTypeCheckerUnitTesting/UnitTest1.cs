using System;
using Xunit;
using Antlr4.Runtime;
using CSTtoASTandTypeChecker;


namespace CSTtoASTandTypeCheckerUnitTesting
{
    public class TypeCheckingTests
    {
        [Fact]
        public void TestAdditionNodeTypeCheck_Success()
        {
            // Arrange
            AdditionNode additionNode = new AdditionNode();
            additionNode.left = new NumberNode(5);
            additionNode.right = new NumberNode(10);

            // Act
            TypeNode result = additionNode.typeCheck();

            // Assert
            Assert.IsType<NumberTypeNode>(result);
        }

        [Fact]
        public void TestAdditionNodeTypeCheck_Failure()
        {
            // Arrange
            AdditionNode additionNode = new AdditionNode();
            additionNode.left = new NumberNode(5);
            additionNode.right = new TextNode("10");

            // Act & Assert
            Exception ex = Assert.Throws<Exception>(() => additionNode.typeCheck());
            Assert.Contains("Type mismatch in addition", ex.Message);
        }
    }

    public class CodeGenerationTests
    {
        [Fact]
        public void TestPrintCodegeneration_Success() 
        {
            //Arrange
            CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
            TextNode TextNode = new TextNode("\"Hello\"");
            PrintNode PrintNode = new PrintNode(TextNode);
            var expected = "public static void main(String[] args){\nSystem.out.println(\"Hello\");";

            //Act
            PrintNode.generate(cgv);
            
            //Assert
            Assert.Equal(expected, cgv.output);
        }

        [Fact]
        public void TestCreateNumberVariable_Success()
        {
            //Arrange
            CodeGeneratorVisitor cgv = new CodeGeneratorVisitor();
            IdentifierNode name = new IdentifierNode();
            name.name = "variable";
            name.type = new NumberTypeNode();
            NumberNode value = new NumberNode(10);
         
            CreateVariableNode createVariableNode = new CreateVariableNode(name, new NumberTypeNode(), value);

            var expected = "public static void main(String[] args){\npublic static Float variable = 10F;";

            //Act
            createVariableNode.generate(cgv);

            //Assert
            Assert.Equal(expected, cgv.output);
        }
    }
}