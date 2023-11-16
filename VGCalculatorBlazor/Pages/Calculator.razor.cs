using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using VGCalculatorBlazor.Data.ViewModel;
using Antlr4.Runtime;
using NCalc.Domain;
using NCalc;
using System.Security.Claims;
using System.Globalization;

namespace VGCalculatorBlazor.Pages
{
    [INotifyPropertyChanged]
    public partial class CalculatorView : ComponentBase
    {
        [ObservableProperty]
        public string inputText = "";

        [ObservableProperty]
        public string calculatedResult = "0";

        public bool isSciOpWaiting = false;
        public long num { get; set; }
        public long den { get; set; }

        public double Result { get; set; }

        public string listItem { get; set; }

        public List<string> calculations = new List<string>();

        public CalculatorView()
        {
        }

        public void Reset()
        {
            CalculatedResult = "0";
            InputText = "";
            isSciOpWaiting = false;
        }

        
        public void Calculate()
        {
            if (InputText.Length == 0)
                return;

            if (isSciOpWaiting)
            {
                InputText += ")";
                isSciOpWaiting = false;
            }

            try
            {
                var inputString = NormalizeInputString();
                var expression = new NCalc.Expression(inputString);
                var result = expression.Evaluate();

                listItem = result.ToString();

                string[] tokens = InputText.Split(")");
                /*foreach (var t in calculations)
                {*/
                    foreach (var k in tokens)
                    {
                        if (k.Equals(k))
                        {
                            if (k == ")")
                            {
                                calculations.Add($"{tokens} = {listItem}");
                            }
                        }
                    }
                /*}*/
                CalculatedResult = result.ToString();
                Result = Convert.ToDouble(CalculatedResult);
                DoubleToNormalFraction(Result);
                calculations.Add($"{InputText} = {listItem} = {num}/{den}");
                StateHasChanged();
                /*calculations.Add($"{InputText} = {listItem} = {num}/{den}");
                CalculatedResult = result.ToString();*/
            }
            catch (Exception ex)
            {
                CalculatedResult = "NaN";
            }
        }

        public string NormalizeInputString()
        {
            Dictionary<string, string> _opMapper = new()
        {
            {"×", "*"},
            {"÷", "/"},
            {"SQRT", "Sqrt"},
        };

            var retString = InputText;

            foreach (var key in _opMapper.Keys)
            {
                retString = retString.Replace(key, _opMapper[key]);
            }

            return retString;
        }

        public void Backspace()
        {
            if (InputText.Length > 0)
                InputText = InputText.Substring(0, InputText.Length - 1);
        }

        public void NumberInput(string key)
        {
            InputText += key;
        }

        public void MathOperator(string op)
        {
            if (isSciOpWaiting)
            {
                InputText += ")";
                isSciOpWaiting = false;
            }

            InputText += $" {op} ";
        }

        public void RegionOperator(string op)
        {
            if (op == ")")
                isSciOpWaiting = false;

            InputText += op;
        }

        public void ScientificOperator(string op)
        {
            InputText += $"{op}(";
            isSciOpWaiting = true;
        }

        public string DoubleToNormalFraction(double numeric)
        {
            
            var numericArray = numeric.ToString().Split(new[] { CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator }, StringSplitOptions.None);
            var wholeStr = numericArray[0];
            var fractionStr = "0";
            if (numericArray.Length > 1)
                fractionStr = numericArray[1];

             
            var power = fractionStr.Length;

            
            long whole = long.Parse(wholeStr) * 10;
            long denominator = 10;
            for (int i = 1; i < power; i++)
            {
                denominator = denominator * 10;
                whole = whole * 10;
            }

            
            var numerator = long.Parse(fractionStr);
            numerator = numerator + whole;


            
            var index = 2;
            /*while (index < denominator / 2) 
            {
                if (numerator % index == 0 && denominator % index == 0)
                {
                    numerator = numerator / index;
                    denominator = denominator / index;
                    index = 1; //При i++ будет увеличен до 2х
                }
                index++;
            }*/
            num = numerator;
            den = denominator;
            return $"{numerator}/{denominator}";
        }
    }
}
