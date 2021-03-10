using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Calc
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> preList = new List<string>();
            string[] later = new string[1000];
            int i = 0, k = 0;
            var path = args.FirstOrDefault();
            if (path == null)
            {
                path = "./1.txt";
            }
            var pre = getCalcFormula(preList, path);

            InfixToSuffix(pre, ref later, ref k);

            Console.WriteLine(Calculate(later.ToArray(), ref k));
            Console.ReadKey();
        }

        /// <summary>
        /// 获取计算公式
        /// </summary>
        /// <param name="preList"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] getCalcFormula(List<string> preList, string path)
        {
            var file = File.ReadAllLines(path);

            var parameterList = file.Skip(1).Take(file.Length - 1);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (var item in parameterList)
            {
                var patameter = item.Trim().Split('=');
                parameters.Add(patameter[0], patameter[1]);
            }

            var formula = file.FirstOrDefault();
            //var inputFormula = formula.ToCharArray();
            var inputFormula = Regex.Matches(formula, @"([a-z0-9.])+|(\W)");


            foreach (var item in inputFormula)
            {
                if (parameters.ContainsKey(item.ToString()))
                {
                    preList.Add(parameters.GetValueOrDefault(item.ToString()));
                }
                else
                {
                    preList.Add(item.ToString());
                }

            }
            preList.Add("#");
            return preList.ToArray();
        }

        /// <summary>
        /// 中缀表达式变后缀表达式
        /// </summary>
        /// <param name="pre">暂存中缀表达式</param>
        /// <param name="later">暂存操作数或中间结果</param>
        /// <param name="k"></param>
        public static void InfixToSuffix(string[] pre, ref string[] later, ref int k)
        {
            //暂存操作符优先级数
            Stack<string> stack = new Stack<string>();
            stack.Clear();
            int i = 0, j = 0;
            stack.Push("#");
            while (pre[i] != "#")
            {
                if (pre[i] == "(")
                {
                    stack.Push(pre[i]);
                }
                else if (pre[i] == ")")
                {
                    while (stack.Peek() != "(")
                    {
                        later[j++] = stack.Pop();
                        k++;
                    }
                    stack.Pop();
                }
                else if (IsOperator(pre[i]) > 0)
                {
                    //如果pre[i]是操作符
                    //判断栈顶操作符的优先级是否大于等于该元素的优先级，如果是将栈顶元素弹出加入later中，
                    //循环执行这一操作，直到栈顶元素优先级小于该元素优先级，此时将该元素加入数组later中
                    while (IsOperator(stack.Peek()) >= IsOperator(pre[i]))
                    {
                        later[j++] = stack.Pop();
                        k++;
                    }
                    stack.Push(pre[i]);
                }
                else
                {
                    //如果pre[i]是数字,直接加入数组later中
                    later[j++] = pre[i];
                    k++;
                }
                i++;
            }
            while (stack.Peek() != "#")
            {
                later[j++] = stack.Pop();
                k++;
            }

        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="later"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static double Calculate(string[] later, ref int k)
        {
            Stack<double> stack1 = new Stack<double>();
            stack1.Clear();
            stack1.Push(0);
            int i = 0;
            double x1, x2;
            while (i < k)
            {
                if (later[i] == "+")
                {
                    x2 = stack1.Pop();
                    x1 = stack1.Pop();
                    stack1.Push(x1 + x2);
                    i++;
                }
                else if (later[i] == "-")
                {
                    x2 = stack1.Pop();
                    x1 = stack1.Pop();
                    stack1.Push(x1 - x2);
                    i++;
                }
                else if (later[i] == "*")
                {
                    x2 = stack1.Pop();
                    x1 = stack1.Pop();
                    stack1.Push(x1 * x2);
                    i++;
                }
                else if (later[i] == "/")
                {
                    x2 = stack1.Pop();
                    x1 = stack1.Pop();
                    stack1.Push(x1 / x2);
                    i++;
                }
                else
                {
                    stack1.Push(double.Parse(later[i]));
                    i++;
                }
            }
            return stack1.Peek();
        }

        /// <summary>
        /// 操作符种类
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private static int IsOperator(string op)
        {
            switch (op)
            {
                case "+":
                    return 1;
                case "-":
                    return 1;
                case "*":
                    return 2;
                case "/":
                    return 2;
                case "(":
                    return 0;
                case "#":
                    return -1;
                default:
                    return -1;
            }
        }
    }
}
