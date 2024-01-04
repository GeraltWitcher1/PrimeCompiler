using Prime.AST;

namespace Prime;

public class Compiler
{
    public static void Main()
    {
        var inputCode = ReadCodeFromFile(@"D:\VIA 6-7 sem\CMC\Compiler\Prime\prime.txt");

        var scanner = new Scanner(inputCode);
        var parser = new ParserAST(scanner);

        try
        {
            var astRoot = parser.ParseProgram();
            Console.WriteLine("Parsing successful");
            var printVisitor = new PrintAstVisitor();
            astRoot.Accept(printVisitor);
            var checker = new Checker();
            astRoot.Accept(checker);
            var encoder = new Encoder();
            astRoot.Accept(encoder);
            
            const string targetName = "prime.tam";
            encoder.SaveTargetProgram( targetName );
            
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