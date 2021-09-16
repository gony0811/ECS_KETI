using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

namespace INNO6.Core.Communication
{
    public class XSerialComm
    {

        private ConcurrentQueue<string> receivedQueue = new ConcurrentQueue<string>();
        private static object critical_section = new object();
        private string _Buffer;
        private char _Seperator;

        private SerialPort _serialPort { get; set; }


        public bool IsOpen { get { return _serialPort.IsOpen; } }

        public XSerialComm(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, char seperator = char.MinValue)
        {
            _serialPort = new SerialPort(portName: portName, baudRate: baudRate, parity: parity, dataBits: dataBits, stopBits: stopBits);
            _serialPort.DataReceived += SerialPort_DataReceived;
            _Seperator = seperator;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            if (_serialPort.IsOpen && _serialPort.BytesToRead > 0)
            {
                string readData = _serialPort.ReadExisting();

                DataProcessing(readData, _Seperator);
            }

        }

        private bool DataProcessing(string read, char seperator = char.MinValue)
        {
            if (string.IsNullOrEmpty(read) || read.StartsWith("\0")) return false;

            _Buffer += read;

            if (seperator == char.MinValue || _Buffer.Contains<char>(seperator))
            {              
                receivedQueue.Enqueue(_Buffer);
                _Buffer = string.Empty;
                return true;
            }
            else
            {
                
                return false;
            }
        }

        public virtual void Open()
        {
            try
            {
                _serialPort.Open();
            }
            catch(Exception ex)
            {
                LogHelper.Instance.DeviceLog.WarnFormat("Fail SerialCommunication open... retry to conn. after 1 seconds.");
            }           
         }

        public virtual void Close()
        {
            _serialPort.Close();
        }

        public bool ReadBuffer(out string data)
        {
            return receivedQueue.TryDequeue(out data);
        }

        public virtual void FlushBuffer()
        { 
            while(!receivedQueue.IsEmpty)
            {
                receivedQueue.TryDequeue(out string _);
            }
        }

        public void SendMessage(string message)
        {           
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(message);
            }               
        } 

        public void SendMessage(byte[] message)
        {
            if(_serialPort.IsOpen) 
                _serialPort.Write(message, 0, message.Length);
        }


        private void WriteData(string data)
        {
            lock (critical_section)
            {
                FlushBuffer();
                if (_serialPort.IsOpen)
                {
                    try
                    {
                        // Send the binary data out the port
                        byte[] hexstring = Encoding.ASCII.GetBytes(data);
                        //There is a intermitant problem that I came across
                        //If I write more than one byte in succesion without a 
                        //delay the PIC i'm communicating with will Crash
                        //I expect this id due to PC timing issues ad they are
                        //not directley connected to the COM port the solution
                        //Is a ver small 1 millisecound delay between chracters

                        foreach (byte hexval in hexstring)
                        {
                            byte[] _hexval = new byte[] { hexval }; // need to convert byte to byte[] to write
                            _serialPort.Write(_hexval, 0, 1);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
