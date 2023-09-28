using Prime.Tokens;

namespace Prime;

public class Program
{
    public static void Main()
    {
        const string inputCode = "int x = 42; bool y = true; for (int i = 0; i < 10; i++) { x = x + 1; }";

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
}