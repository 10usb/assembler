using Assembler.Interpreters;
using Assembler.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler {
    public class Parser {
        private static readonly Regex linePattern = new Regex(@"^(?:\s*([a-zA-Z0-9_]+):)?\s*(?:(?:(?:(?:(global|local|const)\s+)?([a-zA-Z0-9]+)\s*=|(?:([+\-&!*?$%=~}]+)\s*)?([a-zA-Z0-9]+))\s*(.*?)\s*({)?)|(}))?\s*(;.+)?$", RegexOptions.Compiled);
        private static readonly Regex valueRegex = new Regex(@"^\s*(?:([1-9][0-9]*\b|0\b)|(0x[0-9a-fA-F]+\b)|(0[0-7]+\b)|([01]+b\b)|([a-zA-Z$_][a-zA-Z0-9$_]*)|""([^""]*(?:""""[^""]*)*)""|(((?<open>\()[^()]*)+([^()]*(?<-open>\)))+(?(open)(?!))))", RegexOptions.Compiled);
        private static readonly Regex operatorRegex = new Regex(@"^\s*(<<|>>|>=|<=|!=|is not|is|as|[+\-*/%|=\^<>&])\s*", RegexOptions.Compiled);

        private const int GROUP_DECIMAL = 1;
        private const int GROUP_HEX = 2;
        private const int GROUP_OCTAL = 3;
        private const int GROUP_BINARY = 4;
        private const int GROUP_SYMBOL = 5;
        private const int GROUP_STRING = 6;
        private const int GROUP_EXPRESION = 7;

        private const int GROUP_LABEL = 1;
        private const int GROUP_SCOPE = 2;
        private const int GROUP_ASSIGNMENT = 3;
        private const int GROUP_MODIFIER = 4;
        private const int GROUP_INSTRUCTION = 5;
        private const int GROUP_ARGUMENTS = 6;
        private const int GROUP_BLOCKOPEN = 7;
        private const int GROUP_BLOCKCLOSE = 8;
        private const int GROUP_COMMENTS = 9;

        private readonly FileInfo source;
        private readonly IInterpreter interpreter;

        public Parser(FileInfo source, IInterpreter interpreter) {
            this.source = source;
            this.interpreter = interpreter;
        }

        public void Parse(TextReader reader) {
            int lineNr = 0;
            string line;

            while ((line = reader.ReadLine()) != null) {
                Match match = linePattern.Match(line);
                if (!match.Success)
                    throw new AssemblerException("Unexpected syntax on line {0}", lineNr);

                IValue[] arguments = null;
                if (match.Groups[GROUP_ARGUMENTS].Value.Length > 0)
                    arguments = ParseArguments(match.Groups[GROUP_ARGUMENTS].Value, lineNr).ToArray();

                AssemblyLine assemblyLine = new AssemblyLine(source, lineNr) {
                    Label = match.Groups[GROUP_LABEL].Value,
                    Scope = GetScopeType(match.Groups[GROUP_SCOPE].Value, lineNr),
                    Assignment = match.Groups[GROUP_ASSIGNMENT].Value,
                    Modifier = match.Groups[GROUP_MODIFIER].Value,
                    Instruction = match.Groups[GROUP_INSTRUCTION].Value,
                    Arguments = arguments,
                    IsBlockOpen = match.Groups[GROUP_BLOCKOPEN].Value.Length > 0,
                    IsBlockClose = match.Groups[GROUP_BLOCKCLOSE].Value.Length > 0,
                    Comments = match.Groups[GROUP_COMMENTS].Value
                };

                interpreter.Process(assemblyLine);

                lineNr++;
            }
        }

        private ScopeType GetScopeType(string value, int lineNr) {
            switch (value) {
                case "global": return ScopeType.Global;
                case "const": return ScopeType.Constant;
                case "local": return ScopeType.Local;
                case "": return ScopeType.None;
                default: throw new AssemblerException("Unexpected syntax on line {0}", lineNr, value);
            }
        }

        private IEnumerable<IValue> ParseArguments(string value, int lineNr) {
            do {
                Match match = valueRegex.Match(value);
                if (!match.Success)
                    throw new AssemblerException("Failed to match argument '{0}'", lineNr, value);

                yield return ParseValue(match);

                value = value.Substring(match.Length);
            } while (value.Length > 0);
        }

        private IValue ParseValue(Match match) {
            if (match.Groups[GROUP_SYMBOL].Success)
                return new Symbol(match.Groups[GROUP_SYMBOL].Value);

            if (match.Groups[GROUP_DECIMAL].Success)
                return new Number(Convert.ToInt64(match.Groups[GROUP_DECIMAL].Value), NumberFormat.Decimal);

            if (match.Groups[GROUP_HEX].Success)
                return new Number(Convert.ToInt64(match.Groups[GROUP_HEX].Value.Substring(2), 16), NumberFormat.Hex);

            if (match.Groups[GROUP_OCTAL].Success)
                return new Number(Convert.ToInt64(match.Groups[GROUP_OCTAL].Value.Substring(1), 8), NumberFormat.Octal);

            if (match.Groups[GROUP_BINARY].Success) {
                string value = match.Groups[GROUP_BINARY].Value;
                return new Number(Convert.ToInt64(value.Substring(0, value.Length - 1), 2), NumberFormat.Binary);
            }

            if (match.Groups[GROUP_STRING].Success)
                return new Text(match.Groups[GROUP_STRING].Value.Replace("\"\"", "\""));

            if (match.Groups[GROUP_EXPRESION].Success) {
                string value = match.Groups[GROUP_EXPRESION].Value;
                value = value.Substring(1, value.Length - 2);

                Match leftMatch = valueRegex.Match(value);
                IValue left = ParseValue(leftMatch);

                value = value.Substring(leftMatch.Length);

                Match operatorMatch = operatorRegex.Match(value);
                if (!operatorMatch.Success)
                    throw new Exception("Failed");

                value = value.Substring(operatorMatch.Length);

                Match rightMatch = valueRegex.Match(value);
                IValue right = ParseValue(rightMatch);
                return new Expression(GetOperator(operatorMatch.Groups[1].Value), left, right);
            }


            Console.WriteLine("/---------------------------------------------\\");
            for (int i = 0; i < match.Groups.Count; i++) {
                Console.WriteLine("{0}: {1}", i, match.Groups[i].Value);
            }
            Console.WriteLine("\\---------------------------------------------/");

            return null;
        }

        private Operation GetOperator(string value) {
            switch (value) {
                case "+": return Operation.Add;
                case "-": return Operation.Substract;
                case "*": return Operation.Muliply;
                case "/": return Operation.Divide;
                case "%": return Operation.Modulo;
                case "&": return Operation.And;
                case "|": return Operation.Or;
                case "^": return Operation.Xor;
                case "=": return Operation.Equal;
                case "<": return Operation.Less;
                case ">": return Operation.Greater;
                case "!=": return Operation.NotEqual;
                case "<=": return Operation.LessOrEqual;
                case ">=": return Operation.GreaterOrEqual;
                case "<<": return Operation.ShiftLeft;
                case ">>": return Operation.ShiftRight;
                case "is": return Operation.Is;
                case "is not": return Operation.IsNot;
                case "as": return Operation.Cast;
            }

            throw new Exception("Unknown operation");
        }
    }
}
