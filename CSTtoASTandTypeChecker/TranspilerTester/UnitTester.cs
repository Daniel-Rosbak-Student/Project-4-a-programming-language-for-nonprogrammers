using System;
using CSTtoASTandTypeChecker;

namespace TranspilerTester
{
    public class UnitTester
    {
        public static void TestAll()
        {
            TestAdditionNodeTypeCheck_Success();
            TestAdditionNodeTypeCheck_Failure();
        }

        public static void TestAdditionNodeTypeCheck_Success()
        {
            // Arrange
            AdditionNode additionNode = new AdditionNode();
            additionNode.left = new NumberNode(5);
            additionNode.right = new NumberNode(10);

            // Act
            TypeNode result = additionNode.typeCheck();

            // Assert
            if (!(result is NumberTypeNode))
            {
                throw new Exception("TestAdditionNodeTypeCheck_Success failed");
            }
            
            Console.WriteLine("TestAdditionNodeTypeCheck_Success passed");
        }

        public static void TestAdditionNodeTypeCheck_Failure()
        {
            // Arrange
            AdditionNode additionNode = new AdditionNode();
            additionNode.left = new NumberNode(5);
            additionNode.right = new TextNode("10");

            // Act & Assert
            try
            {
                additionNode.typeCheck();
                throw new Exception("TestAdditionNodeTypeCheck_Failure did not throw exception as expected");
            }
            catch (Exception)
            {
                // Test passed
                Console.WriteLine("TestAdditionNodeTypeCheck_Failure passed");
            }
        }
    }
}