using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace server.Services
{
    public class MathExpressionValidator
    {
        public bool Validate(string input, out List<string> errors, out string cleanedExpression)
        {
            errors = new List<string>();
            cleanedExpression = "";

            if (string.IsNullOrWhiteSpace(input))
            {
                errors.Add("Expression is empty.");
                return false;
            }

            // Run basic structure validation
            if (!IsValidStructure(input, out var structureErrors))
                errors.AddRange(structureErrors);

            // Check for consecutive operators (e.g. ++, **, etc.)
            if (Regex.IsMatch(input, @"[\+\-\*/]{2,}"))
                errors.Add("Expression contains consecutive operators.");

            // Check if expression ends with an operator
            if (Regex.IsMatch(input.Trim(), @"[\+\-\*/]$"))
                errors.Add("Expression ends with an operator.");

            // Remove all whitespace characters
            cleanedExpression = Regex.Replace(input ?? "", @"\s+", "");

            return errors.Count == 0;
        }

        /// <summary>
        /// Checks if the expression has only allowed characters and balanced parentheses.
        /// </summary>
        public bool IsValidStructure(string input, out List<string> errors)
        {
            errors = new List<string>();

            if (!Regex.IsMatch(input, @"^[0-9+\-*/()\s]+$"))
                errors.Add("Expression contains invalid characters. Allowed: digits, +, -, *, /, (, ), [, ].");

            if (!AreParenthesesBalanced(input))
                errors.Add("Unbalanced parentheses.");

            return errors.Count == 0;
        }

        private bool AreParenthesesBalanced(string expression)
        {
            int balance = 0;
            foreach (char c in expression)
            {
                if (c == '[' || c == '(') balance++;
                else if (c == ']'|| c == ')') balance--;

                if (balance < 0) return false;
            }
            return balance == 0;
        }
    }
}
