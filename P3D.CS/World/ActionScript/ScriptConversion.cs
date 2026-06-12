using System.Globalization;

namespace P3D;

public static class ScriptConversion
{
    private enum Associativity
    {
        Right,
        Left,
    }

    public static double ToDouble(Object expression)
    {
        String input = expression.ToString() ?? "";
        bool retError = false;
        double retDbl = InternalToDouble(input, ref retError);

        if (retError == false)
        {
            return retDbl;
        }
        else
        {
            if (IsArithmeticExpression(expression) == true)
            {
                String postFix = ToPostfix(expression.ToString() ?? "");
                return EvaluatePostfix(postFix);
            }
            else
            {
                return 0.0;
            }
        }
    }

    public static bool IsArithmeticExpression(Object expression)
    {
        bool retError = false;
        String postfix = ToPostfix(expression.ToString() ?? "", ref retError);
        if (retError == false)
        {
            EvaluatePostfix(postfix, ref retError);
            return retError == false;
        }
        else
        {
            return false;
        }
    }

    private static double EvaluatePostfix(String input, ref bool hasError)
    {
        List<double> stack = [];
        List<char> tokens = [..input.ToCharArray()];

        String cNumber = String.Empty;

        while (tokens.Count > 0)
        {
            char token = tokens[0];
            tokens.RemoveAt(0);

            if (IsNumber(token) == true)
            {
                cNumber += token.ToString();
            }
            else if (cNumber.Length > 0)
            {
                stack.Insert(0, InternalToDouble(cNumber));
                cNumber = String.Empty;
            }

            if (cNumber.Length > 0 && tokens.Count == 0)
            {
                stack.Insert(0, InternalToDouble(cNumber));
                cNumber = String.Empty;
            }

            if (IsOperator(token) == true)
            {
                if (stack.Count >= 2)
                {
                    double v2 = stack[0];
                    double v1 = stack[1];

                    stack.RemoveAt(0);
                    stack.RemoveAt(0);

                    double result = 0;

                    switch (token.ToString())
                    {
                        case "+":
                            result = v1 + v2;
                            break;
                        case "-":
                            result = v1 - v2;
                            break;
                        case "*":
                            result = v1 * v2;
                            break;
                        case "/":
                            if (v2 == 0)
                            {
                                Logger.Log(Logger.LogTypes.Warning, $"ScriptConversion.cs: Cannot evaluate \"{input}\" as an arithmetic expression.");
                                hasError = true;
                                return 0;
                            }
                            else
                            {
                                result = v1 / v2;
                            }
                            break;
                        case "^":
                            result = Math.Pow(v1, v2);
                            break;
                        case "m":
                            result = v1 % v2;
                            break;
                        case "r":
                            result = Math.Pow(v1, 1.0 / v2);
                            break;
                        case "%":
                            result = v1 / 100.0 * v2;
                            break;
                    }

                    stack.Insert(0, result);
                }
                else
                {
                    Logger.Log(Logger.LogTypes.Warning, $"ScriptConversion.cs: Cannot evaluate \"{input}\" as an arithmetic expression.");
                    hasError = true;
                    return 0;
                }
            }
        }

        if (stack.Count == 1)
        {
            return stack[0];
        }
        else
        {
            Logger.Log(Logger.LogTypes.Warning, $"ScriptConversion.cs: Cannot evaluate \"{input}\" as an arithmetic expression.");
            hasError = true;
            return 0;
        }
    }

    private static double EvaluatePostfix(String input)
    {
        bool hasError = false;
        return EvaluatePostfix(input, ref hasError);
    }

    private static String ToPostfix(String input, ref bool hasError)
    {
        if (input.TrimStart().StartsWith("-") == true)
        {
            input = "0" + input;
        }

        List<char> tokens = [..input.ToCharArray()];
        List<char> stack = [];

        String output = String.Empty;
        String cNumber = String.Empty;

        while (tokens.Count > 0)
        {
            char token = tokens[0];
            tokens.RemoveAt(0);

            if (IsNumber(token) == true)
            {
                cNumber += token.ToString();
            }
            else if (cNumber.Length > 0)
            {
                output += cNumber + " ";
                cNumber = String.Empty;
            }

            if (cNumber.Length > 0 && tokens.Count == 0)
            {
                output += cNumber + " ";
                cNumber = String.Empty;
            }

            if (IsOperator(token) == true)
            {
                char o1 = token;

                while (stack.Count > 0 &&
                       IsOperator(stack[0]) == true &&
                       ((GetAssociativity(o1) == Associativity.Left && GetPrecedence(o1) <= GetPrecedence(stack[0])) ||
                        (GetAssociativity(o1) == Associativity.Right && GetPrecedence(o1) < GetPrecedence(stack[0]))))
                {
                    output += stack[0].ToString() + " ";
                    stack.RemoveAt(0);
                }

                stack.Insert(0, o1);
            }

            if (token == '(')
            {
                stack.Insert(0, token);
            }

            if (token == ')')
            {
                if (stack.Count > 0)
                {
                    while (stack.Count > 0)
                    {
                        if (stack[0] == '(')
                        {
                            stack.RemoveAt(0);
                            break;
                        }
                        else
                        {
                            output += stack[0].ToString() + " ";
                            stack.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    Logger.Log(Logger.LogTypes.Warning, $"ScriptConversion.cs: Cannot convert \"{input}\" to an arithmetic expression.");
                    hasError = true;
                    return "0";
                }
            }
        }

        while (stack.Count > 0)
        {
            if (stack[0] == '(' || stack[0] == ')')
            {
                Logger.Log(Logger.LogTypes.Warning, $"ScriptConversion.cs: Cannot convert \"{input}\" to an arithmetic expression.");
                hasError = true;
                return "0";
            }
            else
            {
                output += stack[0].ToString() + " ";
                stack.RemoveAt(0);
            }
        }

        return output;
    }

    private static String ToPostfix(String input)
    {
        bool hasError = false;
        return ToPostfix(input, ref hasError);
    }

    private static bool IsNumber(char token)
    {
        return "0123456789.,".ToCharArray().Contains(token);
    }

    private static bool IsOperator(char token)
    {
        return "+-*/^%mr".ToCharArray().Contains(token);
    }

    private static int GetPrecedence(char op)
    {
        switch (op)
        {
            case '+':
            case '-':
                return 2;
            case '*':
            case '/':
            case '%':
            case 'm':
                return 3;
            case '^':
            case 'r':
                return 4;
        }
        return -1;
    }

    private static Associativity GetAssociativity(char op)
    {
        switch (op)
        {
            case '^':
            case 'r':
                return Associativity.Right;
            default:
                return Associativity.Left;
        }
    }

    private static double InternalToDouble(String expression, ref bool hasError)
    {
        expression = expression.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

        if (StringHelper.IsNumeric(expression) == true)
        {
            return Convert.ToDouble(expression);
        }
        else
        {
            if (expression.ToLower().Equals("false") == true)
            {
                return 0;
            }
            else if (expression.ToLower().Equals("true") == true)
            {
                return 1;
            }
            else
            {
                hasError = true;
                return 0;
            }
        }
    }

    private static double InternalToDouble(String expression)
    {
        bool hasError = false;
        return InternalToDouble(expression, ref hasError);
    }

    public static int ToInteger(Object expression)
    {
        return (int)Math.Round(ToDouble(expression));
    }

    public static float ToSingle(Object expression)
    {
        return (float)ToDouble(expression);
    }

    public static bool ToBoolean(Object expression)
    {
        switch (expression.ToString()?.ToLower())
        {
            case "true":
            case "1":
                return true;
            default:
                return false;
        }
    }

    public static bool IsBoolean(Object expression)
    {
        String s = expression.ToString() ?? "";
        String[] validBools = ["0", "1", "true", "false"];
        return validBools.Contains(s.ToLower());
    }
}
