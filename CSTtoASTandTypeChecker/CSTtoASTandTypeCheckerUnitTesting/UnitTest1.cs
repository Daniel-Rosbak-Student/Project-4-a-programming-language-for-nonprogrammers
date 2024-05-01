using System;
using Xunit;
using CSTtoASTandTypeChecker;

namespace CSTtoASTandTypeCheckerUnitTesting
{
    public class UnitTest1
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
}