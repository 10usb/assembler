using Assembler.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler {
    public class Parser {//
        static readonly Regex linePattern = new Regex(@"^(?:\s*([a-zA-Z0-9]+):)?\s*(?:(?:(?:(?:(global|local|const)\s+)?([a-zA-Z0-9]+)\s*=|(?:([+\-&!*?$%=~}]+)\s*)?([a-zA-Z0-9]+))\s*(.*?)\s*({)?)|(}))?\s*(;.+)?$", RegexOptions.Compiled);
        static readonly Regex valueRegex = new Regex(@"^\s*(?:([1-9][0-9]*\b|0\b)|(0x[0-9a-fA-F]+\b)|(0[0-7]+\b)|([01]+b\b)|([a-zA-Z][a-zA-Z0-9]*)|""([^""]*(?:""""[^""]*)*)""|(((?<open>\()[^()]*)+([^()]*(?<-open>\)))+(?(open)(?!))))", RegexOptions.Compiled);
        static readonly Regex operatorRegex = new Regex(@"^\s*(<<|>>|>=|<=|!=|is|[+\-*/%|=\^<>&])\s*", RegexOptions.Compiled);
        private IProcessor processor;
        const int GROUP_DECIMAL = 1;
        const int GROUP_HEX = 2;
        const int GROUP_OCTAL = 3;
        const int GROUP_BINARY = 4;
        const int GROUP_SYMBOL = 5;
        const int GROUP_STRING = 6;
        const int GROUP_EXPRESION = 7;

        public Parser(IProcessor processor) {
            this.processor = processor;
        }

        public void Parse(TextReader reader) {
            int lineNr = 0;
            string line;

            while ((line = reader.ReadLine()) != null) {
                Match match = linePattern.Match(line);
                if (!match.Success)
                    throw new AssemblerException("Unexpected syntax on line {0}", lineNr);

                IValue[] arguments = null;
                if (match.Groups[6].Value.Length > 0)
                    arguments = ParseArguments(match.Groups[6].Value).ToArray();

                AssemblyLine assemblyLine = new AssemblyLine(lineNr) {
                    Label = match.Groups[1].Value,
                    Scope = match.Groups[2].Value,
                    Assignment = match.Groups[3].Value,
                    Modifier = match.Groups[4].Value,
                    Instruction = match.Groups[5].Value,
                    Arguments = arguments,
                    IsBlockOpen = match.Groups[7].Value.Length > 0,
                    IsBlockClose = match.Groups[8].Value.Length > 0,
                    Comments = match.Groups[9].Value
                };

                processor.ProcessLine(assemblyLine);

                lineNr++;
            }
        }

        private IEnumerable<IValue> ParseArguments(string value) {
            do {
                Match match = valueRegex.Match(value);
                if (!match.Success)
                    throw new Exception("Failed");

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
                return new Values.String(match.Groups[GROUP_STRING].Value.Replace("\"\"", "\""));

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
            }

            throw new Exception("Unknown operation");
        }
    }
}
