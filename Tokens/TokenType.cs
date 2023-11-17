namespace Prime.Tokens;


public enum TokenType
{
    Identifier,
    Type,
    Array,
    Func,
    Return,
    If,
    Else,
    For,
    Integer,
    Char,
    Bool,
    Operator,

    LeftCurly,
    RightCurly,
    LeftParen,
    RightParen,
    RightBracket,
    LeftBracket,
    Semicolon,
    Arrow,
    Comma,
    
    EndOfFile,
    Error,
    // Add more token types as needed
    AssignOperator,
    BitwiseAnd,
    BitwiseOr,
    Equal,
    NotEqual,
    GreaterEqual,
    LessEqual,
    LessThan,
    GreaterThan,
    Plus,
    Minus,
    Multiply,
    Divide,
    Modulo,
    OperatorL1,
    OperatorL2,
    OperatorL3,
    OperatorL4
}


