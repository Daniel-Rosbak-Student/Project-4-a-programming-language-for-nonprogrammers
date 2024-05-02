namespace TranspilerTester;

public class Tester
{
    static void Main(string[] args)
    {
        UnitTester.TestAll();
        IntegrationTester.testAll();
    }
}