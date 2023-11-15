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

                var listItem = result.ToString();

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

                calculations.Add($"{InputText} = {listItem}");
                CalculatedResult = result.ToString();
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
    }
}
