using Basler.Pylon;
using ECS.UI.Model;
using GalaSoft.MvvmLight;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using INNO6.IO;
using INNO6.IO.Service;
using System.Threading;
using INNO6.Core.Manager;
using GalaSoft.MvvmLight.CommandWpf;
using System.Text.RegularExpressions;

namespace ECS.UI.ViewModel
{
    public class MainSystemViewModel : ViewModelBase
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        #region IO DEFINE
        private const string IO_NAME_X_POSITION_INPUT = "oPMAC.dAxisX.SetPostion";
        private const string IO_NAME_Y_POSITION_INPUT = "oPMAC.dAxisY.SetPostion";

        private const string IO_NAME_X_VELOCITY_INPUT = "oPMAC.dAxisX.SetVelocity";
        private const string IO_NAME_Y_VELOCITY_INPUT = "oPMAC.dAxisY.SetVelocity";

        private const string IO_NAME_CH1_LED_OUTPUT = "oLed.iDataSet.Ch1";
        private const string IO_NAME_CH1_LED_OUTPUT_STATUS = "iLed.iData.Ch1";
        #endregion


        private RelayCommand<TextCompositionEventArgs> previewTextInputCommand;
        private RelayCommand<object> xJogPlusPreviewMouseLeftButtonDownCommand;
        private RelayCommand<object> xJogPlusPreviewMouseLeftButtonUpCommand;
        private RelayCommand<object> xJogMinusPreviewMouseLeftButtonDownCommand;
        private RelayCommand<object> xJogMinusPreviewMouseLeftButtonUpCommand;

        private RelayCommand<object> yJogPlusPreviewMouseLeftButtonDownCommand;
        private RelayCommand<object> yJogPlusPreviewMouseLeftButtonUpCommand;
        private RelayCommand<object> yJogMinusPreviewMouseLeftButtonDownCommand;
        private RelayCommand<object> yJogMinusPreviewMouseLeftButtonUpCommand;

        private RelayCommand<object> ch1LedOutputValueChangedCommand;

        private ICommand ch1_LedOn_Command;
        private ICommand ch1_LedOff_Command;


        private double ch1LedOutputValue;


        public MainSystemViewModel()
        {
            Ch1LedOutputValue = DataManager.Instance.GET_INT_DATA(IO_NAME_CH1_LED_OUTPUT_STATUS, out bool _);
        }

        public double Ch1LedOutputValue
        {
            get { return ch1LedOutputValue; }
            set
            {
                ch1LedOutputValue = value;
                RaisePropertyChanged("Ch1LedOutputValue");
            }
        }

        public ICommand Ch1_LedOn_Command
        {
            get
            {
                if (ch1_LedOn_Command == null)
                {
                    ch1_LedOn_Command = new DelegateCommand(Execute_Ch1_LedOn);
                }

                return ch1_LedOn_Command;
            }
        }

        public ICommand Ch1_LedOff_Command
        {
            get
            {
                if (ch1_LedOff_Command == null)
                {
                    ch1_LedOff_Command = new DelegateCommand(Execute_Ch1_LedOff);
                }

                return ch1_LedOff_Command;
            }
        }

        private void Execute_Ch1_LedOn()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_LED_CH1_ON");
        }

        private void Execute_Ch1_LedOff()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_LED_CH1_OFF");
        }      
        private void XAxisStopPositionMove()
        {

        }

        private void YAxisStopPositionMove()
        {

        }  
        public ICommand Ch1LedOutputValueChangedCommand
        {
            get
            {
                return this.ch1LedOutputValueChangedCommand ?? (this.ch1LedOutputValueChangedCommand = new RelayCommand<object>(ExecuteCh1LedOutputValueChangedCommand));
            }
        }

        private void ExecuteCh1LedOutputValueChangedCommand(object obj)
        {
            if (obj is RoutedPropertyChangedEventArgs<double>)
            {
                RoutedPropertyChangedEventArgs<double> eventArgs = obj as RoutedPropertyChangedEventArgs<double>;
                int value = Convert.ToInt32(eventArgs.NewValue);
                DataManager.Instance.SET_INT_DATA(IO_NAME_CH1_LED_OUTPUT, value);
            }         
        }

        public ICommand PreviewTextInputCommand
        {
            get
            {
                return this.previewTextInputCommand ?? (this.previewTextInputCommand = new RelayCommand<TextCompositionEventArgs>(ExecutePreviewTextInputCommand));
            }
        }

        public ICommand XJogPlusPreviewMouseLeftButtonDownCommand
        {
            get
            {
                return this.xJogPlusPreviewMouseLeftButtonDownCommand ?? (this.xJogPlusPreviewMouseLeftButtonDownCommand = new RelayCommand<object>(XJogPlusExecuteMouseLeftButtonDownCommand));
            }
        }

        public ICommand XJogPlusPreviewMouseLeftButtonUpCommand
        {
            get
            {
                return this.xJogPlusPreviewMouseLeftButtonUpCommand ?? (this.xJogPlusPreviewMouseLeftButtonUpCommand = new RelayCommand<object>(XJogPlusExecuteMouseLeftButtonUpCommand));
            }
        }

        public ICommand XJogMinusPreviewMouseLeftButtonDownCommand
        {
            get
            {
                return this.xJogMinusPreviewMouseLeftButtonDownCommand ?? (this.xJogMinusPreviewMouseLeftButtonDownCommand = new RelayCommand<object>(XJogMinusExecuteMouseLeftButtonDownCommand));
            }
        }


        public ICommand XJogMinusPreviewMouseLeftButtonUpCommand
        {
            get
            {
                return this.xJogMinusPreviewMouseLeftButtonUpCommand ?? (this.xJogMinusPreviewMouseLeftButtonUpCommand = new RelayCommand<object>(XJogMinusExecuteMouseLeftButtonUpCommand));
            }
        }

        public ICommand YJogPlusPreviewMouseLeftButtonDownCommand
        {
            get
            {
                return this.yJogPlusPreviewMouseLeftButtonDownCommand ?? (this.yJogPlusPreviewMouseLeftButtonDownCommand = new RelayCommand<object>(YJogPlusExecuteMouseLeftButtonDownCommand));
            }
        }



        public ICommand YJogPlusPreviewMouseLeftButtonUpCommand
        {
            get
            {
                return this.yJogPlusPreviewMouseLeftButtonUpCommand ?? (this.yJogPlusPreviewMouseLeftButtonUpCommand = new RelayCommand<object>(YJogPlusExecuteMouseLeftButtonUpCommand));
            }
        }

        public ICommand YJogMinusPreviewMouseLeftButtonDownCommand
        {
            get
            {
                return this.yJogMinusPreviewMouseLeftButtonDownCommand ?? (this.yJogMinusPreviewMouseLeftButtonDownCommand = new RelayCommand<object>(YJogMinusExecuteMouseLeftButtonDownCommand));
            }
        }

        public ICommand YJogMinusPreviewMouseLeftButtonUpCommand
        {
            get
            {
                return this.yJogMinusPreviewMouseLeftButtonUpCommand ?? (this.yJogMinusPreviewMouseLeftButtonUpCommand = new RelayCommand<object>(YJogMinusExecuteMouseLeftButtonUpCommand));
            }
        }


        private void XJogMinusExecuteMouseLeftButtonDownCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_X_AXIS_JOG_MINUS");
        }

        private void XJogMinusExecuteMouseLeftButtonUpCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_X_AXIS_JOG_STOP");
        }

        private void XJogPlusExecuteMouseLeftButtonDownCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_X_AXIS_JOG_PLUS");
        }

        private void XJogPlusExecuteMouseLeftButtonUpCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_X_AXIS_JOG_STOP");
        }

        private void YJogPlusExecuteMouseLeftButtonDownCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_Y_AXIS_JOG_PLUS");
        }

        private void YJogPlusExecuteMouseLeftButtonUpCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_Y_AXIS_JOG_STOP");
        }
        private void YJogMinusExecuteMouseLeftButtonDownCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_Y_AXIS_JOG_MINUS");
        }

        private void YJogMinusExecuteMouseLeftButtonUpCommand(object obj)
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_Y_AXIS_JOG_STOP");
        }

        private void ExecutePreviewTextInputCommand(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        } 


        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
    }
}