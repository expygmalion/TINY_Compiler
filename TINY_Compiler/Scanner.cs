using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; // REGEX
using System.Threading.Tasks;

public enum Token_Class
{
    // Reserved Words
    T_INT, T_FLOAT, T_STRING_KEY, T_READ, T_WRITE, T_REPEAT, T_UNTIL, T_IF, T_ELSEIF, T_ELSE, T_THEN, T_RETURN, T_ENDL, T_MAIN,

    // Operators
    T_ASSIGN, T_NOTEQUAL, T_AND, T_OR,
    T_PLUS, T_MINUS, T_TIMES, T_DIVIDE, T_LESSTHAN, T_GREATERTHAN, T_EQUAL, T_LPAREN, T_RPAREN, T_LCURLY, T_RCURLY, T_COMMA, T_SEMICOLON,

    // Identifiers and Literals
    T_NUMBER, T_IDENTIFIER, T_STRING,

    // Ignored
    COMMENT, WHITESPACE,
    ERROR
}

namespace JASON_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        

        private readonly List<Tuple<Regex, Token_Class>> tokenDefinitions = new List<Tuple<Regex, Token_Class>>();
        public Scanner()
        {

            // Whitespace should be matched first to be ignored
            tokenDefinitions.Add(Tuple.Create(new Regex(@"[ \t\n\r]+"), Token_Class.WHITESPACE));


            tokenDefinitions.Add(Tuple.Create(new Regex(@":="), Token_Class.T_ASSIGN));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"<>"), Token_Class.T_NOTEQUAL));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"&&"), Token_Class.T_AND));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\|\|"), Token_Class.T_OR));


            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bint\b"), Token_Class.T_INT));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bfloat\b"), Token_Class.T_FLOAT));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bstring\b"), Token_Class.T_STRING_KEY));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bread\b"), Token_Class.T_READ));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bwrite\b"), Token_Class.T_WRITE));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\brepeat\b"), Token_Class.T_REPEAT));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\buntil\b"), Token_Class.T_UNTIL));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bif\b"), Token_Class.T_IF));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\belseif\b"), Token_Class.T_ELSEIF));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\belse\b"), Token_Class.T_ELSE));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bthen\b"), Token_Class.T_THEN));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\breturn\b"), Token_Class.T_RETURN));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bendl\b"), Token_Class.T_ENDL));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\bmain\b"), Token_Class.T_MAIN));

            tokenDefinitions.Add(Tuple.Create(new Regex(@"\+"), Token_Class.T_PLUS));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"-"), Token_Class.T_MINUS));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\*"), Token_Class.T_TIMES));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"/"), Token_Class.T_DIVIDE));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"<"), Token_Class.T_LESSTHAN));
            tokenDefinitions.Add(Tuple.Create(new Regex(@">"), Token_Class.T_GREATERTHAN));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"="), Token_Class.T_EQUAL));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\("), Token_Class.T_LPAREN));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\)"), Token_Class.T_RPAREN));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\{"), Token_Class.T_LCURLY));
            tokenDefinitions.Add(Tuple.Create(new Regex(@"\}"), Token_Class.T_RCURLY));
            tokenDefinitions.Add(Tuple.Create(new Regex(@","), Token_Class.T_COMMA));
            tokenDefinitions.Add(Tuple.Create(new Regex(@";"), Token_Class.T_SEMICOLON));


            tokenDefinitions.Add(Tuple.Create(new Regex(@"""[^""]*"""), Token_Class.T_STRING)); 
            tokenDefinitions.Add(Tuple.Create(new Regex(@"[0-9]+(\.[0-9]+)?"), Token_Class.T_NUMBER)); 
            tokenDefinitions.Add(Tuple.Create(new Regex(@"[a-zA-Z][a-zA-Z0-9]*"), Token_Class.T_IDENTIFIER));
        }

        public void StartScanning(string sourceCode)
        {
            Tokens.Clear();
            Errors.Error_List.Clear();
            int currentIndex = 0;

            while (currentIndex < sourceCode.Length)
            {
                
                if (currentIndex + 1 < sourceCode.Length && sourceCode[currentIndex] == '/' && sourceCode[currentIndex + 1] == '*')
                {
                    int commentStartIndex = currentIndex;
                    currentIndex += 2; 
                    bool commentTerminated = false;
                    while (currentIndex + 1 < sourceCode.Length)
                    {
                        if (sourceCode[currentIndex] == '*' && sourceCode[currentIndex + 1] == '/')
                        {
                            currentIndex += 2; 
                            commentTerminated = true;
                            break;
                        }
                        currentIndex++;
                    }

                    if (!commentTerminated)
                    {
                        Errors.Error_List.Add("Lexical Error: Unterminated multi-line comment starting at index " + commentStartIndex);
                    }
                    continue; 
                }



                Match bestMatch = null;
                Token_Class bestTokenType = Token_Class.ERROR;

                foreach (var definition in tokenDefinitions)
                {

                    Match currentMatch = definition.Item1.Match(sourceCode, currentIndex);


                    if (currentMatch.Success && currentMatch.Index == currentIndex)
                    {
                        
                        if (bestMatch == null || currentMatch.Length > bestMatch.Length)
                        {
                            bestMatch = currentMatch;
                            bestTokenType = definition.Item2;
                        }
                    }
                }

                if (bestMatch != null)
                {
                    if (bestTokenType != Token_Class.WHITESPACE)
                    {
                        Tokens.Add(new Token { lex = bestMatch.Value, token_type = bestTokenType });
                    }
                    currentIndex += bestMatch.Length;
                }
                else
                {
                    
                    Errors.Error_List.Add("Lexical Error: Unrecognized token at index " + currentIndex + ": '" + sourceCode[currentIndex] + "'");
                    currentIndex++; 
                }
            }
            TINY_Compiler.TokenStream = Tokens;
        }
    }
}
