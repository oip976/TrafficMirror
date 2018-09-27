/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
/*                                                                                                                  */
/*    (c) Cooperative Media Lab, Bamberg, Germany                                                                   */
/*                                                                                                                  */
/*     This file is part of the Master Thesis "TrafficMirror: An Ambient Zoomable Display for Traffic Information." */
/*                                                                                                                  */
/*     Chair for Human-Computer Interaction                                                                         */
/*                                                                                                                  */
/*     Friedemann Dürrbeck                                                                                          */
/*     1766526                                                                                                      */
/*     friedemann-thomas.duerrbeck@stud.uni-bamberg.de                                                              */
/*                                                                                                                  */
/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;

namespace TrafficMirror
{
    /// <summary>
    /// This class is responsible for handling the business logic
    /// of the remote controlled car
    /// </summary>
class TMController
    {
        private static SerialPort serialPort;

        //Bits to represent different directions.
        public readonly byte FORWARD_BIT = 1;
        public readonly byte BACKWARD_BIT = 2;
        public readonly byte LEFT_BIT = 4;
        public readonly byte RIGHT_BIT = 8;

        //Bits to indicate RED_LED, AMBER_LED and GREEN_LED
        public readonly byte RED_LED_BIT = 11;
        public readonly byte AMBER_LED_BIT = 12;
        public readonly byte GREEN_LED_BIT = 13;

        //Enum value for the drive command
        private static readonly byte DRIVE_CMD = 10;

        //speed of the car, PWM value from 0-255 => 0 = STOP; 255 = full speed
        private readonly byte car_speed = 255;

        //traffic_severity for manual drive mode
        private readonly byte traffic_sev_def = 0;

        private void SendCommand(byte commandID, byte key_data, byte speed_data, byte traffic_severity)
        {
            byte[] byteArray = new byte[5];
            byteArray[0] = commandID;
            byteArray[1] = key_data;
            byteArray[2] = speed_data;
            byteArray[3] = traffic_severity;
            byteArray[4] = (byte)(commandID + key_data + speed_data);
            SendData(byteArray);
        }

        public void SendDirectionCommand(byte keyState, byte c_speed, byte traffic_severity)
        {
            SendCommand(DRIVE_CMD, keyState, c_speed, traffic_severity);
            Console.WriteLine("Sending Command: DRIVE_CMD = {0}/keyState = {1}/car_speed = {2}", DRIVE_CMD, keyState, c_speed);
        }

        public void SendDirectionCommand(byte keyState)
        {
            SendCommand(DRIVE_CMD, keyState, car_speed, traffic_sev_def);
            Console.WriteLine("Sending Command: DRIVE_CMD = {0}/keyState = {1}/car_speed = {2}", DRIVE_CMD, keyState, car_speed);
        }

        public void SendData(byte[] array)
        {
            if (array != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(array, 0, array.Length);

                } catch (IOException e)
                {
                    throw new Exception("Sending data to Serial Port not possible: {0}", e);
                }
            }
        }

        public void ConnectSerialPort(string portName)
        {
            IContainer components = new Container();
            serialPort = new SerialPort(components)
            {
                PortName = portName, // Setting what port number.
                BaudRate = 9600, // Setting baudrate.
                DtrEnable = true // Enable the Data Terminal Ready 
            };

            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            serialPort.Open(); // Open the port for use.
        }

        public void DisconnectSerialPort()
        {
            if(serialPort != null)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.Dispose();
                serialPort.Close();
                serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                serialPort = null;
            }
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.Write(indata);
        }

        public bool CheckSerialPort()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return false;
            }
            else if (serialPort.IsOpen)
            {
                return true;
            }
            return false;
        }
    }
}
