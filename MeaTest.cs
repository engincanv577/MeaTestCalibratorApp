using System;
using System.Globalization;
using System.IO.Ports;
using System.Numerics;
using static ExampleApplications.MeaTest;
using static System.Windows.Forms.AxHost;

namespace ExampleApplications
{
    public sealed class MeaTest
    {
        public int[] validBaudrates = { 1200, 2400, 4800, 9600, 19200, 38400, 76800, 115200 };

        private SerialPort _serialPort;

        private bool _serialWriteOnProcessFlag;
        private bool _serialReadOnProcessFlag;
        private string _readData;

        private string _meaTestM133C_IdnCode = "MEATEST,M133C,100002,1.22";     // MeaTest M133C Identification Code
        /****************************************************************** DEVICE COMMANDS ******************************************************************/
        /* If you want to get detailed information about the commands please follow regarding link: https://www.meatest.com/files/download/man/m133cm_32.pdf */

        // General commands
        private string _getIdn = "*IDN?";
        private string _getOpc = "*OPC?";
        private string _setOpc = "*OPC";
        private string _getOpt = "*OPT?";
        private string _setWAI = "*WAI";
        private string _setRST = "*RST";
        private string _getTST = "*TST?";
        private string _getSTB = "*STB?";
        private string _getSRE = "*SRE?";
        private string _setSRE = "*SRE "; //+<value>
        private string _getESR = "*ESR?";
        private string _getESE = "*ESE?";
        private string _setESE = "*ESE "; //+<value>
        private string _setCLS = "*CLS";


        // INPut subsystem commands
        public bool InpFilt(bool stat) => stat ? WriteReadLnSerialAnsComp($"INP:FILT(?) ON", "1") : WriteReadLnSerialAnsComp("INP:FILT(?) OFF", "0");
        public string InpFilt() => WriteReadLnSerialFlagChecked("INP:FILT?");
        public bool InpPull(InpPulls inpPull)
        {
            if (inpPull == InpPulls.OFF)
            {
                return WriteReadLnSerialAnsComp("INP:PULL(?) OFF", "OFF");
            }
            else if (inpPull == InpPulls.R150)
            {
                return WriteReadLnSerialAnsComp("INP:PULL(?) 150", "150");
            }
            else
            {
                // R1000
                return WriteReadLnSerialAnsComp("INP:PULL(?) 1K", "1K");
            }
        }
        public string InpPull() => WriteReadLnSerialFlagChecked("INP:PULL?");
        // OUTPut subsystem commands
        public bool OutpStat(OutpStats outpStat) => WriteReadLnSerialAnsComp($"OUTP(?) {outpStat}", $"{outpStat}");
        public string OutpStat() => WriteReadLnSerialFlagChecked("OUTP?");
        public bool OutpLowv(OutpLows outpLow) => WriteReadLnSerialAnsComp($"OUTP:LOWV(?) {outpLow}", $"{outpLow}");
        public string OutpLowv() => WriteReadLnSerialFlagChecked("OUTP:LOWV?");
        public bool OutpLowc(OutpLows outpLow) => WriteReadLnSerialAnsComp($"OUTP:LOWC(?) {outpLow}", $"{outpLow}");
        public string OutpLowc() => WriteReadLnSerialFlagChecked("OUTP:LOWC?");
        public bool OutpPhasUnit(OutpPhasUnits outpPhasUnit) => WriteReadLnSerialAnsComp($"OUTP:UNIT(?) {outpPhasUnit}", $"{outpPhasUnit}");
        public string OutpPhasUnit() => WriteReadLnSerialFlagChecked("OUTP:UNIT?");
        public bool OutpCurc(OutpCurcs outpCurc) => WriteReadLnSerialAnsComp($"OUTP:CURC(?) {outpCurc}", $"{outpCurc}");
        public string OutpCurc() => WriteReadLnSerialFlagChecked("OUTP:CURC?");
        public bool OutpSync(OutpSyncs outpSync) => WriteReadLnSerialAnsComp($"OUTP:SYNC(?) {outpSync}", $"{outpSync}");
        public string OutpSync() => WriteReadLnSerialFlagChecked("OUTP:SYNC?");
        public string OutpSyncLock() => WriteReadLnSerialFlagChecked("OUTP:SYNC:LOCK?");
        public bool OutpEnerUnit(OutpEnerUnits outpEnerUnit) => WriteReadLnSerialAnsComp($"OUTP:ENER:UNIT(?) {outpEnerUnit}", $"{outpEnerUnit}");
        public string OutpEnerUnit() => WriteReadLnSerialFlagChecked("OUTP:ENER:UNIT?");
        public bool OutpEnerMVol(bool stat) => stat ? WriteReadLnSerialAnsComp("OUTP:ENER:MVOL(?) 1", "1") : WriteReadLnSerialAnsComp("OUTP:ENER:MVOL(?) 0", "0");
        public string OutpEnerMVol() => WriteReadLnSerialFlagChecked("OUTP:ENER:MVOL?");
        public bool OutpRefPull(bool R150) => R150 ? WriteReadLnSerialAnsComp("OUTP:REF:PULL(?) 1", "1") : WriteReadLnSerialAnsComp("OUTP:REF:PULL(?) 0", "0");
        public string OutpRefPull() => WriteReadLnSerialFlagChecked("OUTP:REF:PULL?");
        // OutpRefCons default value is 1000.0i/kWh
        public bool OutpRefCons(string val) => WriteReadLnSerialAnsComp($"OUTP:REF:CONS(?) {val}", $"{val}");
        public string OutpRefCons() => ExpToIntStr(WriteReadLnSerialFlagChecked("OUTP:REF:CONS?"));
        public bool OutpRefUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"OUTP:REF:UNIT(?) {powUnit}", $"{powUnit}");
        public string OutpRefUnit() => WriteReadLnSerialFlagChecked("OUTP:REF:UNIT?");
        public bool OutpMHarUnit(OutpMHarUnits outpMHarUnit) => WriteReadLnSerialAnsComp($"OUTP:MHAR:UNIT(?) {outpMHarUnit}", $"{outpMHarUnit}");
        public string OutpMHarUnit() => WriteReadLnSerialFlagChecked("OUTP:MHAR:UNIT?");

        private string _setOutpConf = "OUTP:CONF(?) "; //+<value>          
        private string _getOutpConf = "OUTP:CONF?";
        private string _setOutpL280 = "OUTP:L280(?) "; //+<value>          
        private string _getOutpL280 = "OUTP:L280?";


        // MEASure a CONFigure subsystem
        private string _getMeas = "MEAS?";
        private string _getConf = "CONF?";
        private string _setConf = "CONF(?) "; //+<value> 


        /****************** SOURce subsystem ******************/

        // SOUR PAC (Power AC Mode)
        // The commands below will switch the calibrator to Power AC Mode
        private string _setPacPow = "PAC:POW(?) "; //+<value>      
        private string _getPacPow = "PAC:POW?";
        private string _setPacPowUnit = "PAC:UNIT(?) "; //+<value> 
        private string _getPacPowUnit = "PAC:UNIT?";
        private string _setPacVolt = "PAC:VOLT(?) "; //+<value>    
        private string _getPacVolt = "PAC:VOLT?";
        private string _setPacCurr = "PAC:CURR(?) "; //+<value>    
        private string _getPacCurr = "PAC:CURR?";
        private string _setPacPhas = "PAC:PHAS(?) "; //+<value>    
        private string _getPacPhas = "PAC:PHAS?";
        private string _setPacPol = "PAC:POL(?) "; //+<value>      
        private string _getPacPol = "PAC:POL?";
        private string _setPacFreq = "PAC:FREQ(?) "; //+<value>    
        private string _getPacFreq = "PAC:FREQ?";

        // SOUR PDC (Power DC Mode)
        // The commands below will switch the calibrator to Power DC Mode
        private string _setPdcPow = "PDC:POW(?) "; //+<value>   
        private string _getPdcPow = "PDC:POW?";
        private string _setPdcVolt = "PDC:VOLT(?) "; //+<value> 
        private string _getPdcVolt = "PDC:VOLT?";
        private string _setPdcCurr = "PDC:CURR(?) "; //+<value> 
        private string _getPdcCurr = "PDC:CURR?";

        // SOUR PACI (Power AC High Current Mode)
        // The commands below will switch the calibrator to Power AC High Current Mode
        private string _setPaciPow = "PACI:POW(?) "; //+<value>        
        private string _getPaciPow = "PACI:POW?";
        private string _setPaciPowUnit = "PACI:UNIT(?) "; //+<value>   
        private string _getPaciPowUnit = "PACI:UNIT?";
        private string _setPaciVolt = "PACI:VOLT(?) "; //+<value>      
        private string _getPaciVolt = "PACI:VOLT?";
        private string _setPaciCurr = "PACI:CURR(?) "; //+<value>      
        private string _getPaciCurr = "PACI:CURR?";
        private string _setPaciPhas = "PACI:PHAS(?) "; //+<value>      
        private string _getPaciPhas = "PACI:PHAS?";
        private string _setPaciPol = "PACI:POL(?) "; //+<value>        
        private string _getPaciPol = "PACI:POL?";
        private string _setPaciFreq = "PACI:FREQ(?)"; //+<value>       
        private string _getPaciFreq = "PACI:FREQ?";

        // SOUR PDCI (Power DC High Current Mode)
        // The commands below will switch the calibrator to Power DC High Current Mode
        private string _setPdciPow = "PDCI:POW(?) "; //+<value>               
        private string _getPdciPow = "PDCI:POW?";
        private string _setPdciVolt = "PDCI:VOLT(?) "; //+<value>             
        private string _getPdciVolt = "PDCI:VOLT?";
        private string _setPdciCurr = "PDCI:CURR(?) "; //+<value>             
        private string _getPdciCurr = "PDCI:CURR?";

        // SOUR PACE (Power AC Extended Mode)
        // The commands below will switch the calibrator to Power AC Extended Mode
        public string PacePow = "PACE:POW?";
        public string _pacePowUnit(OperType oper) => oper == OperType.Set ? "PACE:UNIT(?) " : "PACE:UNIT?";
        public string PacePowUnit() => "PACE:UNIT?";
        public bool PacePowUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"PACE:UNIT(?) {powUnit}", ExpToIntStr(powUnit.ToString()));
        public string PaceVolt(Channel ch) => WriteReadLnSerialFlagChecked($"PACE:VOLT{(int)ch}?");
        public string PaceVolt(Channel ch, string value) => $"PACE:VOLT{(int)ch}(?) {value}";
        public string PaceVoltPhas(Channel ch) => $"PACE:VOLT{(int)ch}:PHAS?";
        public string PaceVoltPhas(Channel ch, string value) => $"PACE:VOLT{(int)ch}:PHAS(?) {value}";
        public string PaceVoltEnab(Channel ch) => $"PACE:VOLT{(int)ch}:ENAB?";
        public string PaceVoltEnab(Channel ch, OutpStats stat) => $"PACE:VOLT{(int)ch}:ENAB(?) {stat}";



        // SYSTem subsystem commands
        private string _setSystDate = "SYST:DATE(?) "; //+<value>              
        private string _getSystDate = "SYST:DATE?";
        private string _setSystTime = "SYST:TIME(?) "; //+<value>              
        private string _getSystTime = "SYST:TIME?";
        private string _getSystErr = "SYST:ERR?";
        private string _setSystRem = "SYST:REM";
        private string _setSystLoc = "SYST:LOC";
        private string _setSystRwl = "SYST:RWL";

        // STATus subsystem commands
        private string _getStatOperEven = "STAT:OPER:EVEN?";
        private string _setStatOperEnab = "STAT:OPER:ENAB(?) "; //+<value>
        private string _getStatOperEnab = "STAT:OPER:ENAB?";
        private string _getStatOperCond = "STAT:OPER:COND?";
        private string _getStatQuesEven = "STAT:QUES:EVEN?";
        private string _setStatQuesEnab = "STAT:QUES:ENAB "; //+<value>   
        private string _getStatQuesEnab = "STAT:QUES:ENAB?";
        private string _getStatQuesCond = "STAT:QUES:COND?";
        private string _setStatPres = "STAT:PRES";
        public enum OutpMHarUnits
        {
            PRMS,
            PFUN
        }
        public enum OutpSyncs
        {
            INT,
            LINE,
            IN1,
            IN2,
            IN3
        }
        public enum OutpCurcs
        {
            OFF,
            X25,
            X50
        }
        public enum OutpPhasUnits
        {
            DEG,
            COS
        }
        public enum PowUnits
        {
            W,
            VA,
            VAR
        }
        public enum OperType
        {
            Get,
            Set
        }
        public enum OutpStats
        {
            ON,
            OFF
        }
        public enum InpPulls
        {
            OFF,
            R150,
            R1K
        }
        public enum OutpEnerUnits
        {
            WS,
            WH
        };
        public enum Channel
        {
            Ch1 = 1,
            Ch2 = 2,
            Ch3 = 3
        }
        public enum OutpLows
        {
            FLO,
            GRO
        }
        public MeaTest(SerialPort serialPort)
        {
            _serialPort = serialPort;
            /* Use a COM Port Name depends on your port connection
            _serialPort.PortName = "COMx"; */
            _serialPort.BaudRate = 9600;                                    // Default Baudrate (If you want to use a different baudrate you have to change calibrator's baudrate from physical panel firstly.)
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.ReadTimeout = 1000;
            _serialPort.NewLine = "\r\n";                                   // <CR><LF> Is used for RS232
        }
        private string ExpToIntStr(string expStr)
        {
            if (decimal.TryParse(expStr, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue))
            {
                BigInteger bigIntValue = new BigInteger(decimalValue);

                return bigIntValue.ToString();
            }
            else
            {
                throw new FormatException("The provided string is not a valid exponential format.");
            }
        }
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
                else
                {
                    Console.WriteLine("Serial write on process!");
                }
            }
        }
        private void WriteLineSerialPortFlagChecked(string command)
        {
            if (CheckIdn() == true)
            {
                WriteLineSerialPort(command);
            }
        }

        private bool WriteReadLnSerialAnsComp(string setValCmd, string ans)
        {
            WriteLineSerialPortFlagChecked(setValCmd);
            if (ans == ReadLineSerialPort())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string WriteReadLnSerialFlagChecked(string setValCmd)
        {
            WriteLineSerialPortFlagChecked(setValCmd);
            return ReadLineSerialPort();
        }
        private string ReadLineSerialPort()
        {
            if (_serialPort.IsOpen)
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
                else
                {
                    Console.WriteLine("Serial read on process!");
                }
            }
            return null;
        }
        public bool CheckIdn()
        {
            try
            {
                WriteLineSerialPort(_getIdn);
                if (ReadLineSerialPort() == _meaTestM133C_IdnCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Flag Error!");
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
    }
}
