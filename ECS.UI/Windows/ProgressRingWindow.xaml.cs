using INNO6.Core.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ECS.UI
{
    /// <summary>
    /// ProgressRingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressRingWindow : Window
    {
        private readonly BackgroundWorker _BackgroundWorker;
        private string _ExecuteName;
        private bool _FunctionAbort;
        public PROCESS_RESULT Result { get; set; }
        public ProgressRingWindow(string windowTitle, string executeFunctionName)
        {
            InitializeComponent();

            this.gbTitle.Header = windowTitle;
            _ExecuteName = executeFunctionName;

            _BackgroundWorker = new BackgroundWorker();
            _BackgroundWorker.WorkerReportsProgress = false;
            this._BackgroundWorker.DoWork += DoWork;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_BackgroundWorker.IsBusy)
            {
                this._BackgroundWorker.RunWorkerAsync();
                this._BackgroundWorker.RunWorkerCompleted += _BackgroundWorker_RunWorkerCompleted;
            }

        }

        private void _BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();

            if (Result == PROCESS_RESULT.SUCCESS)
                MessageBoxManager.ShowMessageBox(string.Format("{0} : 실행 완료되었습니다.", this._ExecuteName));
            else if (Result == PROCESS_RESULT.ABORT)
                MessageBoxManager.ShowMessageBox(string.Format("{0} : 실행이 중단되었습니다.", this._ExecuteName));
            else
                MessageBoxManager.ShowMessageBox(string.Format("{0} : 실행이 실패하였습니다.", this._ExecuteName));
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrEmpty(_ExecuteName))
            {
                Result = PROCESS_RESULT.FAIL;
                return;
            }

            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(_ExecuteName, ExecuteResult);


            while (true)
            {
                Thread.Sleep(10);

                if (_FunctionAbort)
                {
                    FunctionManager.Instance.ABORT_FUNCTION(_ExecuteName);
                    Result = PROCESS_RESULT.ABORT;
                    return;
                }
                else if (FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(_ExecuteName) == false)
                {
                    return;
                }
                else
                {
                    string processMessage = FunctionManager.Instance.GET_FUNCTION_PROCESS_MESSAGE(_ExecuteName);
                }
            }
        }

        private void ExecuteResult(string functionName, object state)
        {
            string result = state as string;

            if (result == FunctionManager.FUNC_RESULT_SUCCESS)
                Result = PROCESS_RESULT.SUCCESS;
            else if (result == FunctionManager.FUNC_RESULT_FAIL)
                Result = PROCESS_RESULT.FAIL;
            else if (result == FunctionManager.FUNC_RESULT_TIMEOUT)
                Result = PROCESS_RESULT.FAIL;
            else
                Result = PROCESS_RESULT.FAIL;
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            _FunctionAbort = true;
        }
    }
}
