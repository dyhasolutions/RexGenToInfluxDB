﻿using System;
using System.Collections.Generic;

namespace InfluxShared.FileObjects
{
    public enum ConversionType { Formula, TableNumeric, TableVerbal, FormulaAndTableNumeric, FormulaAndTableVerbal }


    public class ItemConversion
    {
        public ItemConversion()
        {
            Type = ConversionType.Formula;
        }

        private ConversionType type { get; set; }
        public ConversionType Type {
            get => type;
            set
            {
                type = value;
                if ((value == ConversionType.Formula) || (value == ConversionType.FormulaAndTableNumeric) || (value == ConversionType.FormulaAndTableVerbal))
                {
                    if (Formula is null)
                        Formula = new FormulaConversion();
                }
                if ((value == ConversionType.TableNumeric) || (value == ConversionType.FormulaAndTableNumeric))
                {
                    if (TableNumeric is null)
                        TableNumeric = new TableNumericConversion();
                }
                if ((value == ConversionType.TableVerbal) || (value == ConversionType.FormulaAndTableVerbal))
                {
                    if (TableVerbal is null)
                        TableVerbal = new TableVerbalConversion();
                }
            } }
        public FormulaConversion Formula { get; set; }
        public TableNumericConversion TableNumeric { get; set; }
        public TableVerbalConversion TableVerbal { get; set; }

        public override string ToString()
        {
            switch (type)
            {
                case ConversionType.Formula: return Formula.ToString();
                case ConversionType.FormulaAndTableNumeric:
                    string fx = Formula.ToString();
                    return (fx == "x" ? "" : $"{fx} with ") + "numeric table";
                case ConversionType.FormulaAndTableVerbal:
                    fx = Formula.ToString();
                    return (fx == "x" ? "" : $"{fx} with ") + "verbal table";
                case ConversionType.TableNumeric: return "numeric table";
                case ConversionType.TableVerbal: return "verbal table";
                default: return "unknown";
            }
        }
    }

    /// <summary>
    /// Formula of type (x*a^2 + b*x + c) / (x*d^2 + e*x + f)
    /// </summary>
    public class FormulaConversion
    {
        public string FormulaText { get; set; }
        public double CoeffA { get; set; }
        public double CoeffB { get; set; }
        public double CoeffC { get; set; }
        public double CoeffD { get; set; }
        public double CoeffE { get; set; }
        public double CoeffF { get; set; }

        public override string ToString()
        {
            string coeffStr(double coeff, int xpow) =>
                coeff == 0 ? "" : ((coeff == 1 ? "" : ((coeff > 0 ? "+" : "") + coeff.ToString() + "*")) + (xpow == 0 ? "" : xpow == 1 ? "x" : $"x^{xpow}"));

            string up = (coeffStr(CoeffA, 2).Trim('+', '*') + coeffStr(CoeffB, 1) + coeffStr(CoeffC, 0)).Trim('+', '*');
            string dn = (coeffStr(CoeffD, 2).Trim('+', '*') + coeffStr(CoeffE, 1) + coeffStr(CoeffF, 0)).Trim('+', '*');
            if (dn.Contains("+") || dn.Contains("-"))
                dn = $"({dn})";
            if (dn == "1")
                dn = "";

            return  (dn == "") ? (up == "")? "x": up : (up == "") ? $"1/{dn}": $"{up}/{dn}";
        }
    }

    public class TableNumericConversion 
    {
        public int Count;
        public Dictionary<double, double> Pairs;

        public TableNumericConversion()
        {
            Pairs = new Dictionary<double, double>();
        }
    }

    public class TableVerbalConversion 
    {
        public int Count;
        public Dictionary<double, string> Pairs;

        public TableVerbalConversion()
        {
            Pairs = new Dictionary<double, string>();
        }
    }
}
