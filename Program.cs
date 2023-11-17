using Prime.AST;
using Prime.Tokens;

namespace Prime;

public class Program
{
    public static void Main()
    {




        var inputCode = ReadCodeFromFile(@"C:\Users\klavs\Source\Repos\GeraltWitcher1\PrimeCompiler\prime.txt");

        var scanner = new Scanner(inputCode);
        var parser = new ParserAST(scanner);

        try
        {
            ProgramNode astRoot = parser.ParseProgram();
            Console.WriteLine("Parsing successful");
            PrintAstVisitor printVisitor = new PrintAstVisitor();
            astRoot.Accept(printVisitor);
            Checker checker = new Checker();
            astRoot.Accept(checker);
        }
        catch (ParseException ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        // Token? token;
        // do
        // {
        //     token = scanner.GetNextToken();
        //     if (token.Type != TokenType.EndOfFile)
        //     {
        //         Console.WriteLine($"Token: {token.Type}, Lexeme: {token.Spelling}");
        //     }
        // } while (token.Type != TokenType.EndOfFile);
    }
    private static string ReadCodeFromFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }
}