using System;
using System.Collections.Generic;
using System.Globalization;

namespace server.Services
{
    public class Calculator
    {
        static string ToCount(string expression)
{
    Stack<object> st = new Stack<object>();  
    List<object> nstr = new List<object>();  
    List<object> pln = new List<object>();  
    string subS = "";  
    //////////////////////////////////////////////////////////////////////

    for (int i = 0; i < expression.Length; i++)
    {
        if (Char.IsDigit(expression[i]))
        {
            subS += expression[i].ToString();  
        }
        else
        {
            if (subS != "")
            {
                nstr.Add(int.Parse(subS)); 
                subS = ""; 
            }            
            nstr.Add(expression[i]);
        }
    }
    
    if (subS != "")
    {
        nstr.Add(int.Parse(subS));
    }

  /////////////////////////////////////////////////////////////////////
    for (int i = 0; i < nstr.Count; i++)
    {
        if (nstr[i].GetType() == typeof(int))
        {
            pln.Add(nstr[i]);
        }
        else
        {
            char op = (char)nstr[i];
            if (op == '(')
            {
                st.Push(op); 
            }
            else if (op == ')')
            {
                while (st.Count > 0 && (char)st.Peek() != '(')
                {
                    pln.Add(st.Pop());
                }
                st.Pop(); 
            }
            else 
            {
                while (st.Count > 0 && Precedence(op) <= Precedence((char)st.Peek()))
                {
                    pln.Add(st.Pop());
                }
                st.Push(op); 
            }
        }
    }    
    while (st.Count > 0)
    {
        pln.Add(st.Pop());
    }

///////////////////////////////////////////////
    Stack<int> count = new Stack<int>();  

    foreach (var token in pln)
    {
        if (token.GetType() == typeof(int)) 
        {
            count.Push((int)token);  
        }
        else if (token is char operatorChar) 
        {
            
            int b = count.Pop();  
            int a = count.Pop();  
            
            int result = 0;
            switch (operatorChar)
            {
                case '+':
                    result = a + b;
                    break;
                case '-':
                    result = a - b;
                    break;
                case '*':
                    result = a * b;
                    break;
                case '/':
                    if (b == 0)
                        throw new InvalidOperationException("Delenie nulou.");
                    result = a / b;
                    break;            
            }

            count.Push(result);
        }
    }
    
    return count.Pop().ToString();
}

static int Precedence(char op)
{
    if (op == '+' || op == '-')
        return 1;
    if (op == '*' || op == '/')
        return 2;
    return 0;
}
    }
}