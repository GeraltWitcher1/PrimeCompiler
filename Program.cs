using Prime.Tokens;

namespace Prime;

public class Program
{
    public static void Main()
    {
        string inputCode = ReadCodeFromFile("prime.txt");

        var scanner = new Scanner(inputCode);

        Token? token;
        do
        {
            token = scanner.GetNextToken();
            if (token.Type != TokenType.EndOfFile)
            {
                Console.WriteLine($"Token: {token.Type}, Lexeme: {token.Spelling}");
            }
        } while (token.Type != TokenType.EndOfFile);
    }
    private static string ReadCodeFromFile(string filePath)
    {

        return  File.ReadAllText(filePath);
    }
}