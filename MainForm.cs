using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using Telerik.WinControls.Enumerations;
using static ExampleApplications.MeaTest;

namespace ExampleApplications
{
    public partial class MainForm : Form
    {
        public enum ButtonState
        {
            Started = 0,
            Stopped = 1,
            Stopped2 = 2,
            TryingToConnect = 3,
        }
        public ButtonState buttonState = ButtonState.Stopped;
        public bool receivedAns;
        public string receivedMessage;
        public Thread threadReadSerialMeaTest;
        public MeaTest meaTest;
        public MainForm()
        {
            InitializeComponent();
            meaTest = new MeaTest(serialPortMeaTest);
            Initialize_COM_Ports();
            Initialize_Baudrates();
            string a = meaTest.PaceVolt(Channel.Ch1, "24");
            Console.WriteLine(a);
        }
        public void Initialize_COM_Ports()
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (string port in ports)
            {
                DropDownList_COM_Ports.Items.Add(port);
            }
        }

        public void Initialize_Baudrates()
        {
            foreach (int baud in meaTest.validBaudrates)
            {
                DropDownList_Baudrate.Items.Add(baud.ToString());
            }
        }
        
        private void DropDownList_COM_Ports_Click(object sender, EventArgs e)
        {
            DropDownList_COM_Ports.Items.Clear();
            Initialize_COM_Ports();
        }
        public string selectedCOMPortMeaTest;
        private void DropDownList_COM_Ports_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            selectedCOMPortMeaTest = null;
            if (DropDownList_COM_Ports.SelectedItem != null)
            {
                selectedCOMPortMeaTest = DropDownList_COM_Ports.SelectedItem.ToString();
                serialPortMeaTest.PortName = selectedCOMPortMeaTest;
            }
        }

        private void DropDownList_Baudrate_Click(object sender, EventArgs e)
        {
            DropDownList_Baudrate.Items.Clear();
            Initialize_Baudrates();
        }
        public Int32 selectedBaudrateMeaTest;
        private void DropDownList_Baudrate_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            selectedBaudrateMeaTest = 0;
            if (DropDownList_Baudrate.SelectedItem != null) 
            {
                selectedBaudrateMeaTest = Convert.ToInt32(DropDownList_Baudrate.SelectedItem.ToString());
                serialPortMeaTest.BaudRate = selectedBaudrateMeaTest;
            }
        }
        public void StartCommunicationButton_Click(object sender, EventArgs e)
        {
            if ((!String.IsNullOrEmpty(selectedCOMPortMeaTest)) && (selectedBaudrateMeaTest != 0))
            {
                StartCommunicationButtonState(ButtonState.Stopped);
                if (buttonState == ButtonState.Stopped)
                {
                    if (serialPortMeaTest.IsOpen)
                    {
                        serialPortMeaTest.Close();
                        StartCommunicationButtonState(ButtonState.Stopped2);
                    }
                    else
                    {
                        try
                        {
                            serialPortMeaTest.Open();
                            threadReadSerialMeaTest = new Thread(CheckFlagMeaTest);
                            threadReadSerialMeaTest.Start();
                        }

                        catch (Exception)
                        {
                            serialPortMeaTest.Close();
                            StartCommunicationButtonState(ButtonState.Stopped);
                            StartCommunicationButton.ToggleState = ToggleState.Off;
                            MessageBox.Show("Selected port could not opened.");
                        }
                    }
                }
                else if(buttonState == ButtonState.Started)
                {
                    serialPortMeaTest.Close();
                    StartCommunicationButtonState(ButtonState.Stopped);
                }
            }
            else
            {
                StartCommunicationButtonState(ButtonState.Stopped2);
            }
        }
        public void StartCommunicationButtonState(ButtonState newState)
        {
            buttonState = newState;
            switch (buttonState)
            {
                case ButtonState.Stopped:
                    StartCommunicationButton.ToggleState = ToggleState.Off;
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Text = "Connect"; }));
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Enabled = true; }));
                    break;
                case ButtonState.Stopped2:
                    StartCommunicationButton.ToggleState = ToggleState.On;
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Text = "Connect"; }));
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Enabled = true; }));
                    break;
                case ButtonState.TryingToConnect:
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Text = "Connecting"; }));
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Enabled = false; }));
                    break;
                case ButtonState.Started:
                    StartCommunicationButton.ToggleState = ToggleState.On;
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Text = "Connected"; }));
                    StartCommunicationButton.Invoke(new MethodInvoker(() => { StartCommunicationButton.Enabled = true; }));
                    break;
            }
        }
        public void CheckFlagMeaTest()
        {
            StartCommunicationButtonState(ButtonState.TryingToConnect);
            if (meaTest.CheckIdn() == true)
            {
                StartCommunicationButtonState(ButtonState.Started);
                meaTest.SetSystRem();
            }
            else
            {
                MessageBox.Show("Identification isn't correct");
                StartCommunicationButtonState(ButtonState.Stopped);
            }    
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPortMeaTest.IsOpen)
            {
                serialPortMeaTest.Close();
            }
        }

        private void MeaTestOperStateSwitch_ValueChanged(object sender, EventArgs e)
        {
            if(MeaTestOperStateSwitch.Value == true)
            {
                //meaTest.SetOutpStat(true);
                labelOperationCaution.TextAlign = ContentAlignment.MiddleCenter;
                labelOperationCaution.Text = "CAUTION!\nENERGY ON";
            }
            else
            {
                labelOperationCaution.Text = "";
            }
        }
    }
}
