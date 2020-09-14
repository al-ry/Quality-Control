using System;
using System.IO;
using TriangleType;

namespace Tests
{
    class TestTriangleType
    {
        static private void VerifyResult(string expected, string actual, StreamWriter sw, ref int testCounter)
        {
            string result = actual.Replace("\n", "").Replace("\r", "");
            if (result == expected)
            {
                sw.WriteLine($"Test {testCounter}: success");
            }
            else
            {
                sw.WriteLine($"Test {testCounter}: failed. Expected: {expected}, Actual: {actual}");
            }
            testCounter++;
        }
        static void Main()
        {
            string outputFileName = "../../../output.txt";
            File.WriteAllText(outputFileName, "Tests file initialized\n");
            using StreamWriter outputFile = new StreamWriter(outputFileName, true);

            int testCounter = 1;

            using (var inputFile = new StreamReader("../../../tests.txt"))
            {
                string testCase;
                while ((testCase = inputFile.ReadLine()) != null)
                {
                    string[] args = testCase.Split(' ');
                    string expectedResult = inputFile.ReadLine();

                    StringWriter sw = new StringWriter();
                    Console.SetOut(sw);
                    Console.SetError(sw);
                    try
                    {
                        TriangleType.Program.Main(args);
                        VerifyResult(expectedResult, sw.ToString(), outputFile, ref testCounter);
                    }
                    catch(Exception)
                    {
                        VerifyResult(expectedResult, sw.ToString(), outputFile, ref testCounter);
                    }
                }
            }
            
        }
    }
}
