using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ECS.UI.Behaviors
{
    public class TextBoxInputBehavior : Behavior<TextBox>
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Dependency Property
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Public

        #region 양수만 십진수 입력 여부 속성 - JustPositivDecimalInputProperty

        /// <summary>
        /// 양수만 십진수 입력 여부 속성
        /// </summary>
        public static readonly DependencyProperty JustPositivDecimalInputProperty = DependencyProperty.Register
        (
            "JustPositivDecimalInput",
            typeof(bool),
            typeof(TextBoxInputBehavior),
            new FrameworkPropertyMetadata(false)
        );

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// 유효 숫자 스타일
        /// </summary>
        private const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 입력 모드 - InputMode

        /// <summary>
        /// 입력 모드
        /// </summary>
        public TextBoxInputMode InputMode { get; set; }

        #endregion
        #region 양수만 십진수 입력 여부 - JustPositivDecimalInput

        /// <summary>
        /// 양수만 십진수 입력 여부
        /// </summary>
        public bool JustPositivDecimalInput
        {
            get
            {
                return (bool)GetValue(JustPositivDecimalInputProperty);
            }
            set
            {
                SetValue(JustPositivDecimalInputProperty, value);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 생성자 - TextBoxInputBehavior()

        /// <summary>
        /// 생성자
        /// </summary>
        public TextBoxInputBehavior()
        {
            InputMode = TextBoxInputMode.None;
            JustPositivDecimalInput = false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Protected

        #region 부착시 처리하기 - OnAttached()

        /// <summary>
        /// 부착시 처리하기
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;

            DataObject.AddPastingHandler(AssociatedObject, AssociatedObject_Pasting);
        }

        #endregion
        #region 탈착시 처리하기 - OnDetaching()

        /// <summary>
        /// 탈착시 처리하기
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
            AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;

            DataObject.RemovePastingHandler(AssociatedObject, AssociatedObject_Pasting);
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////// Private
        //////////////////////////////////////////////////////////////////////////////// Event

        #region 관련 객체 프리뷰 키 DOWN 처리하기 - AssociatedObject_PreviewKeyDown(sender, e)

        /// <summary>
        /// 관련 객체 프리뷰 키 DOWN 처리하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!IsValidInput(GetText(" ")))
                {
                    SystemSounds.Beep.Play();

                    e.Handled = true;
                }
            }
        }

        #endregion
        #region 관련 객체 프리뷰 텍스트 입력시 처리하기 - AssociatedObject_PreviewTextInput(sender, e)

        /// <summary>
        /// 관련 객체 프리뷰 텍스트 입력시 처리하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsValidInput(GetText(e.Text)))
            {
                SystemSounds.Beep.Play();

                e.Handled = true;
            }
        }

        #endregion
        #region 관련 객체 붙여넣기시 처리하기 - AssociatedObject_Pasting(sender, e)

        /// <summary>
        /// 관련 객체 붙여넣기시 처리하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void AssociatedObject_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));

                if (!IsValidInput(GetText(text)))
                {
                    SystemSounds.Beep.Play();

                    e.CancelCommand();
                }
            }
            else
            {
                SystemSounds.Beep.Play();

                e.CancelCommand();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////// Function

        #region 텍스트 구하기 - GetText(source)

        /// <summary>
        /// 텍스트 구하기
        /// </summary>
        /// <param name="source">소스 문자열</param>
        /// <returns>텍스트</returns>
        private string GetText(string source)
        {
            TextBox textBox = AssociatedObject;

            int selectionStart = textBox.SelectionStart;

            if (textBox.Text.Length < selectionStart)
            {
                selectionStart = textBox.Text.Length;
            }

            int selectionLength = textBox.SelectionLength;

            if (textBox.Text.Length < selectionStart + selectionLength)
            {
                selectionLength = textBox.Text.Length - selectionStart;
            }

            string actualText = textBox.Text.Remove(selectionStart, selectionLength);

            int caretIndex = textBox.CaretIndex;

            if (actualText.Length < caretIndex)
            {
                caretIndex = actualText.Length;
            }

            string target = actualText.Insert(caretIndex, source);

            return target;
        }

        #endregion
        #region 숫자 여부 구하기 - IsDigit(source)

        /// <summary>
        /// 숫자 여부 구하기
        /// </summary>
        /// <param name="source">소스 문자열</param>
        /// <returns>숫자 여부</returns>
        private bool IsDigit(string source)
        {
            return source.ToCharArray().All(Char.IsDigit);
        }

        #endregion
        #region 유효 입력 여부 구하기 - IsValidInput(source)

        /// <summary>
        /// 유효 입력 여부 구하기
        /// </summary>
        /// <param name="source">소스 문자열</param>
        /// <returns>유효 입력 여부</returns>
        private bool IsValidInput(string source)
        {
            switch (InputMode)
            {
                case TextBoxInputMode.None:

                    return true;

                case TextBoxInputMode.DigitInput:

                    return IsDigit(source);

                case TextBoxInputMode.DecimalInput:

                    decimal decimalValue;

                    if (source.ToCharArray().Where(x => x == '.').Count() > 1)
                    {
                        return false;
                    }

                    if (source.Contains("-"))
                    {
                        if (JustPositivDecimalInput)
                        {
                            return false;
                        }

                        if (source.IndexOf("-", StringComparison.Ordinal) > 0)
                        {
                            return false;
                        }

                        if (source.ToCharArray().Count(x => x == '-') > 1)
                        {
                            return false;
                        }

                        if (source.Length == 1)
                        {
                            return true;
                        }
                    }

                    bool result = decimal.TryParse(source, validNumberStyles, CultureInfo.CurrentCulture, out decimalValue);

                    return result;

                default:

                    throw new ArgumentException("Unknown TextBoxInputMode");
            }
        }

        #endregion
    }
}
