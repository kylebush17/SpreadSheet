using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEngine
{
    public class ExpTree
    {
        #region fields
        Node m_root;
        private Dictionary<string, double> m_lookup;

        #endregion

        #region Constructor
        //constructor for the ExpTree class. The tree is created within the constructor
        public ExpTree(string expression)
        {
            m_lookup = new Dictionary<string, double>();
            m_root = compile(expression);
        }
        #endregion

        #region classes
        public abstract class Node { public abstract double Eval(); }  //base node class that other Node subclasses will inherit from

        public class ConstNode : Node   //node that represents a constant value in the Expression Tree
        {
            private double m_value;
            public ConstNode(double value) { m_value = value; }
            public override double Eval() { return m_value; }
        }

        //OpNode is a node that inherits from Node and represents an operator in the expression tree
        //as an operand node is the only node with children, it is the only class that needs right and left 
        //connections.
        public class OpNode : Node
        {
            private char m_op;
            private Node m_left, m_right;
            public OpNode(char op, Node left, Node right) { m_op = op; m_left = left; m_right = right; }
            public override double Eval()
            {
                double left = m_left.Eval();
                double right = m_right.Eval();
                switch (m_op)
                {
                    case '+': return left + right;
                    case '-': return left - right;
                    case '*': return left * right;
                    case '/': return left / right;
                    default: return -1;
                }
            }
        }

        //VarNode is a sub class of Node that represents a Variable lookup in the expression tree
        // it will return the value of the variable when it is evaluated.
        public class VarNode : Node
        {
            private string VarName;
            private ExpTree instance;
            public VarNode(string name, ExpTree i) { VarName = name; instance = i; }
            public override double Eval()
            {
                return instance.m_lookup[VarName];
            }
        }
        #endregion

        #region methods
        //compile functions are the functions that actually build the tree.
        //Compile recursively calls itself until the tree is built
        public Node compile(string exp)
        {
            this.m_root = Compile(exp);
            return m_root;
        }

        private Node Compile(string exp)
        {
            exp = exp.Replace(" ", "");
            char op = '\0';
            if (exp[0] == '(')  //remove() if the whole thing is encapsulated
            {
                int Count = 1;

                // For each char in the expression
                for (int i = 1; i < exp.Length; i++)
                {


                    if (exp[i] == ')')
                    {
                        Count--;
                        if (Count == 0)
                        {
                            if (i == exp.Length - 1)
                            {
                                return Compile(exp.Substring(1, exp.Length - 2));
                            }
                        }
                    }
                }
            }
            int index = get_low_opIndex(exp);

            if (index == -1)
            {
                double num;
                if (double.TryParse(exp, out num))
                {
                    return new ConstNode(num);
                }
                else
                {
                    m_lookup[exp] = 0;
                    return new VarNode(exp, this);
                }
            }
            if (index != -1)
            {
                return new OpNode(exp[index], Compile(exp.Substring(0, index)), Compile(exp.Substring(index + 1)));
            }
            else
                return null;




        }

        public string[] getVars()
        {
            return m_lookup.Keys.ToArray();
        }

        private int get_low_opIndex(string exp)
        {
            int parentcounter = 0, index = -1;
            for (int i = exp.Length - 1; i >= 0; i--)
            {
                switch (exp[i])
                {
                    case '(':
                        parentcounter--;
                        break;
                    case ')':
                        parentcounter++;
                        break;
                    case '+':
                    case '-':
                        if (parentcounter == 0)
                        {
                            index = i;
                        }
                        break;
                    case '*':
                    case '/':
                        if (parentcounter == 0 && index == -1)
                            index = i;
                        break;

                }
            }
            return index;

        }
        //helper function for building the tree
        private Node BuildSimple(string term)
        {
            double num;
            if (double.TryParse(term, out num)) { return new ConstNode(num); }

            return new VarNode(term, this);
        }

        //adds a variable to the dictionary
        public void SetVar(string VarName, double VarValue)
        {
            m_lookup[VarName] = VarValue;
        }

        public double Eval()
        {
            if (m_root != null) { return this.m_root.Eval(); }
            else { return double.NaN; }
        }
        #endregion

    }
}
