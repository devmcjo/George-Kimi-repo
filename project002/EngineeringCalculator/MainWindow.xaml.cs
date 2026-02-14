using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace EngineeringCalculator
{
    public partial class MainWindow : Window
    {
        private string currentInput = "0";
        private string currentFormula = "";
        private bool isNewCalculation = true;
        private readonly Queue<HistoryItem> history = new Queue<HistoryItem>(20);
        private bool isHistoryVisible = false;

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        // 숫자 버튼 클릭
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            string number = ((Button)sender).Content.ToString();
            
            if (isNewCalculation)
            {
                currentInput = number;
                isNewCalculation = false;
            }
            else
            {
                currentInput = currentInput == "0" ? number : currentInput + number;
            }
            
            UpdateDisplay();
        }

        // 소수점 버튼
        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (isNewCalculation)
            {
                currentInput = "0.";
                isNewCalculation = false;
            }
            else if (!currentInput.Contains("."))
            {
                currentInput += ".";
            }
            
            UpdateDisplay();
        }

        // 연산자 버튼
        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Tag.ToString();
            
            if (!isNewCalculation)
            {
                currentFormula += currentInput + " " + operation + " ";
                currentInput = "0";
                isNewCalculation = true;
                UpdateFormulaDisplay();
            }
            else if (!string.IsNullOrEmpty(currentFormula))
            {
                // 마지막 연산자 교체
                var parts = currentFormula.Trim().Split(' ');
                if (parts.Length >= 2)
                {
                    currentFormula = string.Join(" ", parts.Take(parts.Length - 1)) + " " + operation + " ";
                    UpdateFormulaDisplay();
                }
            }
            else
            {
                currentFormula = currentInput + " " + operation + " ";
                currentInput = "0";
                isNewCalculation = true;
                UpdateFormulaDisplay();
            }
        }

        // 함수 버튼 (sin, cos, tan, log, ln, sqrt, 등)
        private void FunctionButton_Click(object sender, RoutedEventArgs e)
        {
            string func = ((Button)sender).Tag.ToString();
            double value = double.Parse(currentInput);
            double result = 0;
            string formula = "";

            switch (func)
            {
                case "sin":
                    result = Math.Sin(value * Math.PI / 180);
                    formula = $"sin({value})";
                    break;
                case "cos":
                    result = Math.Cos(value * Math.PI / 180);
                    formula = $"cos({value})";
                    break;
                case "tan":
                    result = Math.Tan(value * Math.PI / 180);
                    formula = $"tan({value})";
                    break;
                case "log":
                    result = Math.Log10(value);
                    formula = $"log({value})";
                    break;
                case "ln":
                    result = Math.Log(value);
                    formula = $"ln({value})";
                    break;
                case "sqrt":
                    result = Math.Sqrt(value);
                    formula = $"√({value})";
                    break;
                case "pow2":
                    result = Math.Pow(value, 2);
                    formula = $"({value})²";
                    break;
                case "inv":
                    result = 1 / value;
                    formula = $"1/({value})";
                    break;
            }

            AddToHistory(formula, result);
            currentInput = FormatResult(result);
            currentFormula = "";
            isNewCalculation = true;
            UpdateDisplay();
            UpdateFormulaDisplay();
        }

        // 상수 버튼 (π, e)
        private void ConstantButton_Click(object sender, RoutedEventArgs e)
        {
            string constant = ((Button)sender).Tag.ToString();
            
            if (constant == "pi")
            {
                currentInput = Math.PI.ToString();
            }
            else if (constant == "e")
            {
                currentInput = Math.E.ToString();
            }
            
            isNewCalculation = false;
            UpdateDisplay();
        }

        // 괄호 버튼
        private void BracketButton_Click(object sender, RoutedEventArgs e)
        {
            string bracket = ((Button)sender).Tag.ToString();
            
            if (isNewCalculation)
            {
                currentFormula += bracket + " ";
            }
            else
            {
                currentFormula += currentInput + " " + bracket + " ";
                currentInput = "0";
                isNewCalculation = true;
            }
            
            UpdateFormulaDisplay();
        }

        // 등호 버튼 (=)
        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFormula) || !isNewCalculation)
            {
                string fullFormula = currentFormula + currentInput;
                
                try
                {
                    double result = EvaluateExpression(fullFormula);
                    AddToHistory(fullFormula, result);
                    currentInput = FormatResult(result);
                    currentFormula = "";
                    isNewCalculation = true;
                    UpdateDisplay();
                    UpdateFormulaDisplay();
                }
                catch (Exception ex)
                {
                    currentInput = "Error";
                    isNewCalculation = true;
                    UpdateDisplay();
                }
            }
        }

        // 지우기 버튼 (C)
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "0";
            currentFormula = "";
            isNewCalculation = true;
            UpdateDisplay();
            UpdateFormulaDisplay();
        }

        // 백스페이스 버튼 (←)
        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentInput.Length > 1)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            else
            {
                currentInput = "0";
                isNewCalculation = true;
            }
            UpdateDisplay();
        }

        // 부호 변경 버튼 (±)
        private void SignButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentInput != "0")
            {
                if (currentInput.StartsWith("-"))
                {
                    currentInput = currentInput.Substring(1);
                }
                else
                {
                    currentInput = "-" + currentInput;
                }
                UpdateDisplay();
            }
        }

        // 히스토리 토글 버튼
        private void HistoryToggleButton_Click(object sender, RoutedEventArgs e)
        {
            isHistoryVisible = !isHistoryVisible;
            
            if (isHistoryVisible)
            {
                HistoryPanel.Visibility = Visibility.Visible;
                var animation = new DoubleAnimation(150, TimeSpan.FromMilliseconds(200));
                HistoryPanel.BeginAnimation(HeightProperty, animation);
            }
            else
            {
                var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
                animation.Completed += (s, args) => HistoryPanel.Visibility = Visibility.Collapsed;
                HistoryPanel.BeginAnimation(HeightProperty, animation);
            }
        }

        // 히스토리 선택
        private void HistoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HistoryListBox.SelectedItem is HistoryItem item)
            {
                currentInput = item.Result.ToString();
                isNewCalculation = true;
                UpdateDisplay();
            }
        }

        // 히스토리에 추가
        private void AddToHistory(string formula, double result)
        {
            if (history.Count >= 20)
            {
                history.Dequeue();
            }
            
            history.Enqueue(new HistoryItem { Formula = formula, Result = result });
            UpdateHistoryDisplay();
        }

        // 히스토리 표시 업데이트
        private void UpdateHistoryDisplay()
        {
            HistoryListBox.Items.Clear();
            foreach (var item in history.Reverse())
            {
                HistoryListBox.Items.Add(item);
            }
        }

        // 수식 계산
        private double EvaluateExpression(string expression)
        {
            // DataTable.Compute를 사용하여 수식 계산
            var table = new DataTable();
            var result = table.Compute(expression.Replace("×", "*").Replace("÷", "/"), "");
            return Convert.ToDouble(result);
        }

        // 결과 포맷팅
        private string FormatResult(double result)
        {
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                return "Error";
            }
            
            // 지수 표기법 또는 소수점 처리
            if (Math.Abs(result) >= 1e10 || (Math.Abs(result) < 1e-10 && result != 0))
            {
                return result.ToString("E6");
            }
            
            string formatted = result.ToString("G15");
            
            // 불필요한 소수점 제거
            if (formatted.Contains("."))
            {
                formatted = formatted.TrimEnd('0').TrimEnd('.');
            }
            
            return formatted;
        }

        // 디스플레이 업데이트
        private void UpdateDisplay()
        {
            DisplayText.Text = currentInput;
        }

        // 수식 표시 업데이트
        private void UpdateFormulaDisplay()
        {
            FormulaText.Text = currentFormula;
        }
    }

    // 히스토리 아이템 클래스
    public class HistoryItem
    {
        public string Formula { get; set; }
        public double Result { get; set; }

        public override string ToString()
        {
            return $"{Formula} = {Result:G10}";
        }
    }
}
