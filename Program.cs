﻿using Prime.Tokens;

namespace Prime;

public class Program
{
    public static void Main()
    {
        var inputCode = ReadCodeFromFile(@"D:\VIA 6-7 sem\CMC\Compiler\Prime\prime.txt");

        var scanner = new Scanner(inputCode);
        var parser = new Parser(scanner);

        try
        {
            parser.ParseProgram();
            Console.WriteLine("Parsing successful");
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