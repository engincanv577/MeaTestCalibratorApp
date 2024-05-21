using Microsoft.Win32;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ExampleApplications
{
    public sealed class MeaTest
    {
        public int[] validBaudrates = { 1200, 2400, 4800, 9600, 19200, 38400, 76800, 115200 };

        private SerialPort _serialPort;
        public MeaTest(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.BaudRate = 9600; //Default
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.ReadTimeout = 1000;
            _serialPort.NewLine = "\r\n";                                   // <CR><LF> Is used for RS232
        }

        private bool _serialWriteOnProcessFlag;
        private bool _serialReadOnProcessFlag;
        private string _readData;

        private string _meaTestM133C_FlagCode = "MEATEST,M133C,100002,1.22";     // MeaTest M133C Flag Code

        /****************************************************************** DEVICE COMMANDS ******************************************************************/
        /* If you want to get detailed information about the commands please follow regarding link: https://www.meatest.com/files/download/man/m133cm_32.pdf */

        // General commands
        private string _getFlag = "*IDN?";                                  //This command returns the identification of the manufacturer, model, serial number and firmware revision.
        private string _setOpc = "*OPC";                                    //This command sets the OPC bit in the ESR (Event Status Register) when all pending operations are complete.
        private string _getOpc = "*OPC?";                                   //This command returns “1” to the output queue after all pending operations are complete.
        private string _getOpt = "*OPT?";                                   //This command return the instrument’s harware fitment.
        private string _setWAI = "*WAI";                                    //Prevents the instrument from executing any further commands or queries until all previous remote commands have been executed.
        private string _setRST = "*RST";                                    //This command resets the calibrator to its initial status.
        private string _getTST = "*TST?";                                   //This command launches an internal self-test. Return the self-test result (“0” for pass or “1” for fail).
        private string _getSTB = "*STB?";                                   //This query returns number in range 0 to 255 with information about content of register STB, which carries the MSS bit status
        private string _setSRE = "*SRE "; //+<value>                        //This command sets condition of the Service Request Enable register. Since bit 6 is not used, the maximum value is 191.
        private string _getSRE = "*SRE?";                                   //This query returns the Service Request Enable Register number.
        private string _getESR = "*ESR?";                                   //This query returns the contents of the Event Status Register and clears the register.
        private string _setESE = "*ESE "; //+<value>                        //This command programs the Event Status Enable register bits. Parameter “value” is number in range 0 – 255.
        private string _getESE = "*ESE?";                                   //This query returns the Event Status Enable register.
        private string _setCLS = "*CLS";                                    //This command clears the Event Status Register and the Status Byte Register except the MAV bit and output queue.Output line is not reset.

        // INPut subsystem commands
        private string _getInpFilt = "INP:FILT?";                           //If query is sent, calibrator returns state of the function { 0 | 1 }
        private string _setInpFilt = "INP:FILT "; //+<value>                //This command connects the internal filter to the IN1. (INP:FILT (?) <CPD> { OFF | ON | 0 | 1 })
        private string _getInpPull = "INP:PULL?";                           //If query is sent, calibrator returns connects resistor { OFF | 150 | 1K }.
        private string _setInpPull = "INP:PULL "; //+<value>                //This command connects the internal pull-up resistor (150 Ω or 1 kΩ) to the IN1 (INP:PULL (?) <CPD> { OFF | 150 | 1K })
        // OUTPut subsystem commands
        private string _getOutpStat = "OUTP?";                              //Gets output status (Device returns "ON" or "OFF")
        private string _setOutpStat = "OUTP ";                              //Sets output to operate mode. (ENERGY ON/OFF)
        private string _getOutpLowv = "OUTP:LOWV?";                         //If query is sent, calibrator returns GRO when the output is grounded or FLO when floating.
        private string _setOutpLowv = "OUTP:LOWV "; //+<value>              //This command connects or disconnects the Lo terminals of all voltage outputs to/from GND terminal (FLO or GRO)
        private string _getOutpLowc = "OUTP:LOWC?";                         //If query is sent, calibrator returns GRO when the output is grounded or FLO when floating
        private string _setOutpLowc = "OUTP:LOWC "; //+<value>              //This command connects or disconnects the Lo terminals of all current outputs to/from GND terminal. (FLO or GRO)
        private string _getOutpPhasUnit = "OUTP:UNIT?";                     //If query is sent, calibrator returns the set unit of measurement { DEG | COS }
        private string _setOutpPhasUnit = "OUTP:UNIT "; //+<value>          //This command sets the method used to specify the phase shift between the output voltage and current.
    /*******Continue here******/
        private string _getOutpEnerUnit = "OUTP:ENER:UNIT?";                //Gets output energy unit
        private string _setOutpEnerUnit = "OUTP:ENER:UNIT "; //+<value>     //Sets output energy unit to Ws or Wh
        

        // SYSTem subsystem commands
        private string _setSystDate = "SYST:DATE ";                         //This command sets system date of the calibrator. (Example format: "SYST:DATE YYYY,MM,DD")
        private string _getSystDate = "SYST:DATE?";                         //This command gets system date of the calibrator. (Example format: "YYYY,MM,DD")
        private string _setSystTime = "SYST:TIME ";                         //This command sets time of the calibrator. (Example format: "SYST:TIME HH,MM,SS")
        private string _getSystTime = "SYST:TIME?";                         //This command gets time of the calibrator. (Example format: "HH,MM,SS")
        private string _getSystErr = "SYST:ERR?";                           //Query the multimeter’s error queue. Detected errors are placed in the queue. This query returns the first error from the queue.
        private string _setSystRem = "SYST:REM";                            //Sets system remote to send RS232 commands (Can change system to local with device physical panel if this command sent)
        private string _setSystRwl = "SYST:RWL";                            //Sets system remote to send RS232 commands (Can not change system to local with device physical panel if this command sent)
        private string _setSystLoc = "SYST:LOC";                            //Sets system local (Can not send RS232 commands if device set to local)

        // STATus subsystem commands
        private string _getStatOperEven = "STAT:OPER:EVEN?";                //This query returns the content of Operational Data Event register. It is a decimal value which corresponds to the binary-weighted sum of all bits set in the register.Register is cleared after this query.
        private string _setStatOperEnab = "STAT:OPER:ENAB "; //+<value>     //This command enables bits in the Operational Data Enable register. Selected bits are summarized at bit 7 (OSS) of the IEEE488.2 Status Byte register.
        private string _getStatOperEnab = "STAT:OPER:ENAB?";                //If query is sent, calibrator returns the value of the registr as a decimal number.
        private string _getStatOperCond = "STAT:OPER:COND?";                //This query returns the content of Operational Condition register.
        private string _getStatQuesEven = "STAT:QUES:EVEN?";                //This query returns the content of Questionable Data Event register.
        private string _setStatQuesEnab = "STAT:QUES:ENAB "; //+<value>     //This command enables bits in the Questionable Data Enable register. Selected bits are summarized at bit 3 (QSS) of the IEEE488.2 Status Byte register.
        private string _getStatQuesEnab = "STAT:QUES:ENAB?";                //If query is sent, calibrator returns the value of the registr as a decimal number. Example: 64 is returned as 64.
        private string _getStatQuesCond = "STAT:QUES:COND?";                //This query returns the content of Questionable Condition register.
        private string _setStatPres = "STAT:PRES";                          //This command clears all bits in the Operation Data Enable register and in the Questionable Data Enable register.



        public enum InpPull
        {
            Off,
            R150,
            R1K
        }
        public enum EnerUnits
        {
            WS,
            WH
        };
        
        private void WriteLineSerialPort(string command)
        {
            if (_serialPort.IsOpen)
            {
                if (_serialWriteOnProcessFlag == false)
                {
                    _serialWriteOnProcessFlag = true;
                    try
                    {
                        _serialPort.WriteLine(command);
                    }
                    catch (Exception)
                    {

                    }
                    _serialWriteOnProcessFlag = false;
                }
            }
        }
        private void WriteLineSerialPortFlagChecked(string command)
        {
            if(CheckFlag() == true)
            {
                WriteLineSerialPort(command);
            }
            else 
            {
                MessageBox.Show("Flag Error");
            }
        }
        private bool WriteReadLineSerialPortFlagChecked(string setValCommand, string getValCommand, string answer)
        {
            WriteLineSerialPortFlagChecked(setValCommand);
            WriteLineSerialPortFlagChecked(getValCommand);
            if (answer == ReadLineSerialPort())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string ReadLineSerialPort()
        {
            if(_serialPort.IsOpen)
            {
                if (_serialReadOnProcessFlag == false)
                {
                    _serialReadOnProcessFlag = true;
                    try
                    {
                        _readData = _serialPort.ReadLine();
                        _serialReadOnProcessFlag = false;
                        return _readData;
                    }
                    catch (Exception)
                    {
                        _serialReadOnProcessFlag = false;
                        return null;
                    }
                }
            }
            return null;
        }
        public bool CheckFlag()
        {
            try
            {
                WriteLineSerialPort(_getFlag);
                if (ReadLineSerialPort() == _meaTestM133C_FlagCode)
                {
                    return true;
                }
                else
                {
                    _serialPort.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                _serialPort.Close();
                return false;
            }
        }
        public void SetSystRem()
        {
            WriteLineSerialPortFlagChecked(_setSystRem);
        }
        public void SetSystRwl()
        {
            WriteLineSerialPortFlagChecked(_setSystRwl);
        }
        public void SetSystLoc()
        {
            WriteLineSerialPortFlagChecked(_setSystLoc);
        }
        public bool SetOutpStat(bool outpStat)
        {
            if(outpStat)
            {
                if(WriteReadLineSerialPortFlagChecked(_setOutpStat + "ON", _getOutpStat, "ON"))
                {
                    return true;
                }
                else { return false; }
            }
            else
            {
                if (WriteReadLineSerialPortFlagChecked(_setOutpStat + "OFF", _getOutpStat, "OFF"))
                {
                    return true;
                }
                else { return false; }
            }
        }
        public string GetOutpStat(bool state)
        {
            WriteLineSerialPortFlagChecked(_getOutpStat);
            return ReadLineSerialPort();
        }
        public bool SetOutpEnerUnit(EnerUnits enerUnit)
        {
            if(enerUnit == EnerUnits.WS)
            {
                if (WriteReadLineSerialPortFlagChecked(_setOutpEnerUnit + "WS", _getOutpEnerUnit, "WS"))
                {
                    return true;  
                }
                else { return false; }
            }
            else
            {
                if (WriteReadLineSerialPortFlagChecked(_setOutpEnerUnit + "WH", _getOutpEnerUnit, "WH"))
                {
                    return true;
                }
                else { return false; }
            }
        }
        public string GetOutpEnerUnit()
        {
            WriteLineSerialPortFlagChecked(_getOutpEnerUnit);
            return ReadLineSerialPort();
        }
    }
}
