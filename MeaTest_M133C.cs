﻿using System;
using System.Globalization;
using System.IO.Ports;

namespace MeaTestCalibratorApp
{
    public sealed class MeaTest_M133C
    {
        private string _meaTestM133C_IdnCode = "MEATEST,M133C,100002,1.22";     // MeaTest M133C Identification Code
        private int[] _validBaudrates = { 1200, 2400, 4800, 9600, 19200, 38400, 76800, 115200 };

        private bool _serialWriteOnProcessFlag;
        private bool _serialReadOnProcessFlag;
        private string _readData;
        private SerialPort _serialPort;

        public MeaTest_M133C(SerialPort serialPort)
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
        
        public enum Conts
        {
            PACK,
            CNT1,
            CNT2,
            TIM1,
            TIM2,
            FR1,
            FR2,
            FR3
        }

        public enum TrigReps
        {
            ONE,
            REP
        }

        public enum TrigInps
        {
            OFF,
            IN3
        }

        public enum ModShaps
        {
            OFF,
            SIN,
            RECT
        }

        public enum Pols
        {
            LEAD,
            LAG
        }

        public enum Confs
        {
            VOLT,
            CURR,
            FREQ
        }

        public enum OutpConfs
        {
            C1 = 1,
            C12 = 12,
            C123 = 123
        }

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

        public enum PhasUnits
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

        public enum EnerUnits
        {
            WS,
            WH
        }

        public enum Channels
        {
            CH1 = 1,
            CH2 = 2,
            CH3 = 3
        }

        public enum OutpLows
        {
            FLO,
            GRO
        }
        
        private string ExpToFloatStr(string expStr)
        {
            if (float.TryParse(expStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
            {
                return floatValue.ToString();
            }
            else
            {
                throw new FormatException("The provided string is not a valid exponential format.");
            }
        }
        private bool WriteLnSerial(string command)
        {
            if (_serialWriteOnProcessFlag)
            {
                Console.WriteLine("Serial write on process!");
                return false;
            }

            if (_serialPort.IsOpen)
            {
                Console.WriteLine("Failed to open serial port.");
                return false;
            }

            try
            {
                _serialPort.Open();
            }
            catch 
            {
                return false;
            }

            _serialWriteOnProcessFlag = true;

            try
            {
                _serialPort.WriteLine(command);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during serial write: {ex.Message}");
                return false;
            }
            finally
            {
                _serialWriteOnProcessFlag = false;
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
        }
        private void WriteLnSerialIdnChecked(string command)
        {
            if (CheckIdn())
            {
                WriteLnSerial(command);
            }
        }

        private bool WriteReadLnSerialAnsComp(string setValCmd, string ans, bool ansExp)
        {
            WriteLnSerialIdnChecked(setValCmd);
            if (ansExp)
            {
                if(ans == ExpToFloatStr(ReadLnSerial()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ans == ReadLnSerial())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private string WriteReadLnSerial(string setValCmd)
        {
            WriteLnSerial(setValCmd);
            return ReadLnSerial();
        }

        private string WriteReadLnSerialIdnChecked(string setValCmd, bool ansExp)
        {
            WriteLnSerialIdnChecked(setValCmd);
            if (ansExp)
            {
                return ExpToFloatStr(ReadLnSerial());
            }
            else
            {
                return ReadLnSerial();
            }
        }

        private string ReadLnSerial()
        {
            if (_serialReadOnProcessFlag)
            {
                Console.WriteLine("Serial read on process!");
                return null;
            }
            if (_serialPort.IsOpen)
            {
                return null;
            }

            try
            {
                _serialPort.Open();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to open serial port.");
                return null;
            }

            _serialReadOnProcessFlag = true;

            try
            {
                _readData = _serialPort.ReadLine();
                return _readData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during serial read: {ex.Message}");
                return null;
            }
            finally
            {
                _serialReadOnProcessFlag = false;
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
        }

        public bool CheckIdn()
        {
            if (Idn() == _meaTestM133C_IdnCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Identification Error!");
                return false;
            }
        }
        /****************************************************************** DEVICE COMMANDS ******************************************************************/
        /* If you want to get detailed information about the commands please follow regarding link: https://www.meatest.com/files/download/man/m133cm_32.pdf */

        // General commands
        public string Idn() => WriteReadLnSerial("*IDN?");
        public void SetOpc() => WriteLnSerialIdnChecked("*OPC");
        public string GetOpc() => WriteReadLnSerialIdnChecked("*OPC?", false);
        public string GetOpt() => WriteReadLnSerialIdnChecked("*OPT?", false);
        public void Wai() => WriteLnSerialIdnChecked("*WAI");
        public void Rst() => WriteLnSerialIdnChecked("*RST");
        public string GetTst() => WriteReadLnSerialIdnChecked("*TST?", false);
        public string GetStb() => WriteReadLnSerialIdnChecked("*STB?", false);
        public void SetSre(byte val) => WriteLnSerialIdnChecked($"*SRE {val}");
        public string GetSre() => WriteReadLnSerialIdnChecked("*SRE?", false);
        public string GetEsr() => WriteReadLnSerialIdnChecked("*ESR?", false);
        public void SetEse(byte val) => WriteLnSerialIdnChecked($"*ESE {val}");
        public string GetEse() => WriteReadLnSerialIdnChecked("*ESE?", false);
        public void SetCls() => WriteLnSerialIdnChecked("*CLS");


        // INPut subsystem commands
        public bool InpFilt(bool stat) => stat ? WriteReadLnSerialAnsComp($"INP:FILT(?) ON", "1", false) : WriteReadLnSerialAnsComp("INP:FILT(?) OFF", "0", false);
        public string InpFilt() => WriteReadLnSerialIdnChecked("INP:FILT?", false);
        public bool InpPull(InpPulls inpPull)
        {
            if (inpPull == InpPulls.OFF)
            {
                return WriteReadLnSerialAnsComp("INP:PULL(?) OFF", "OFF", false);
            }
            else if (inpPull == InpPulls.R150)
            {
                return WriteReadLnSerialAnsComp("INP:PULL(?) 150", "150", false);
            }
            else
            {
                // R1000
                return WriteReadLnSerialAnsComp("INP:PULL(?) 1K", "1K", false);
            }
        }
        public string InpPull() => WriteReadLnSerialIdnChecked("INP:PULL?", false);


        // OUTPut subsystem commands
        public bool OutpStat(OutpStats outpStat) => WriteReadLnSerialAnsComp($"OUTP(?) {outpStat}", $"{outpStat}", false);
        public string OutpStat() => WriteReadLnSerialIdnChecked("OUTP?", false);
        public bool OutpLowv(OutpLows outpLow) => WriteReadLnSerialAnsComp($"OUTP:LOWV(?) {outpLow}", $"{outpLow}", false);
        public string OutpLowv() => WriteReadLnSerialIdnChecked("OUTP:LOWV?", false);
        public bool OutpLowc(OutpLows outpLow) => WriteReadLnSerialAnsComp($"OUTP:LOWC(?) {outpLow}", $"{outpLow}", false);
        public string OutpLowc() => WriteReadLnSerialIdnChecked("OUTP:LOWC?", false);
        public bool OutpPhasUnit(PhasUnits outpPhasUnit) => WriteReadLnSerialAnsComp($"OUTP:UNIT(?) {outpPhasUnit}", $"{outpPhasUnit}", false);
        public string OutpPhasUnit() => WriteReadLnSerialIdnChecked("OUTP:UNIT?", false);
        public bool OutpCurc(OutpCurcs outpCurc) => WriteReadLnSerialAnsComp($"OUTP:CURC(?) {outpCurc}", $"{outpCurc}", false);
        public string OutpCurc() => WriteReadLnSerialIdnChecked("OUTP:CURC?", false);
        public bool OutpSync(OutpSyncs outpSync) => WriteReadLnSerialAnsComp($"OUTP:SYNC(?) {outpSync}", $"{outpSync}", false);
        public string OutpSync() => WriteReadLnSerialIdnChecked("OUTP:SYNC?", false);
        public string OutpSyncLock() => WriteReadLnSerialIdnChecked("OUTP:SYNC:LOCK?", false);
        public bool OutpEnerUnit(EnerUnits outpEnerUnit) => WriteReadLnSerialAnsComp($"OUTP:ENER:UNIT(?) {outpEnerUnit}", $"{outpEnerUnit}", false);
        public string OutpEnerUnit() => WriteReadLnSerialIdnChecked("OUTP:ENER:UNIT?", false);
        public bool OutpEnerMVol(bool stat) => stat ? WriteReadLnSerialAnsComp("OUTP:ENER:MVOL(?) 1", "1", false) : WriteReadLnSerialAnsComp("OUTP:ENER:MVOL(?) 0", "0", false);
        public string OutpEnerMVol() => WriteReadLnSerialIdnChecked("OUTP:ENER:MVOL?", false);
        public bool OutpRefPull(bool R150) => R150 ? WriteReadLnSerialAnsComp("OUTP:REF:PULL(?) 1", "1", false) : WriteReadLnSerialAnsComp("OUTP:REF:PULL(?) 0", "0", false);
        public string OutpRefPull() => WriteReadLnSerialIdnChecked("OUTP:REF:PULL?", false);
        public bool OutpRefCons(string val) => WriteReadLnSerialAnsComp($"OUTP:REF:CONS(?) {val}", $"{val}", true);
        public string OutpRefCons() => WriteReadLnSerialIdnChecked("OUTP:REF:CONS?", true);
        public bool OutpRefUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"OUTP:REF:UNIT(?) {powUnit}", $"{powUnit}", false);
        public string OutpRefUnit() => WriteReadLnSerialIdnChecked("OUTP:REF:UNIT?", false);
        public bool OutpMHarUnit(OutpMHarUnits outpMHarUnit) => WriteReadLnSerialAnsComp($"OUTP:MHAR:UNIT(?) {outpMHarUnit}", $"{outpMHarUnit}", false);
        public string OutpMHarUnit() => WriteReadLnSerialIdnChecked("OUTP:MHAR:UNIT?", false);
        public bool OutpConf(OutpConfs outpConf) => WriteReadLnSerialAnsComp($"OUTP:CONF(?) {(uint)outpConf}", $"{(uint)outpConf}", false);
        public string OutpConf() => WriteReadLnSerialIdnChecked("OUTP:CONF?", false);
        public bool OutpL280(bool stat) => stat ? WriteReadLnSerialAnsComp("OUTP:L280(?) 1", "1", false) : WriteReadLnSerialAnsComp("OUTP:L280(?) 0", "0", false);
        public string OutpL280() => WriteReadLnSerialIdnChecked("OUTP:L280?", false);

        // MEASure a CONFigure subsystem
        public string Meas() => WriteReadLnSerialIdnChecked("MEAS?", true);
        public bool Conf(Confs conf) => WriteReadLnSerialAnsComp($"OUTP:CONF(?) {conf}", $"{conf}", false);
        public string Conf() => WriteReadLnSerialIdnChecked("CONF?", false);


        /**************************************************************************** SOURce subsystem ****************************************************************************/
        public string Mode() => WriteReadLnSerialIdnChecked("MODE?", false);
        // SOUR PAC (Power AC Mode)
        // The commands below will switch the calibrator to Power AC Mode
        public bool PacPow(float val) => WriteReadLnSerialAnsComp($"PAC:POW(?) {val}", $"{val}", true);
        public string PacPow() => WriteReadLnSerialIdnChecked("PAC:POW?", true);
        public bool PacPowUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"PAC:UNIT(?) {powUnit}", $"{powUnit}", false);
        public string PacPowUnit() => WriteReadLnSerialIdnChecked("PAC:UNIT?", false);
        public bool PacVolt(float val) => WriteReadLnSerialAnsComp($"PAC:VOLT(?) {val}", $"{val}", true);
        public string PacVolt() => WriteReadLnSerialIdnChecked("PAC:VOLT?", true);
        public bool PacCurr(float val) => WriteReadLnSerialAnsComp($"PAC:CURR(?) {val}", $"{val}", true);
        public string PacCurr() => WriteReadLnSerialIdnChecked("PAC:CURR?", true);
        public bool PacPhas(float val) => WriteReadLnSerialAnsComp($"PAC:PHAS(?) {val}", $"{val}", true);
        public string PacPhas() => WriteReadLnSerialIdnChecked("PAC:PHAS?", true); // Check this function from document, it has a parameter to execute
        public bool PacPol(Pols acPol) => WriteReadLnSerialAnsComp($"PAC:POL(?) {acPol}", $"{acPol}", true); // Check this function from document, it has a parameter to execute
        public string PacPol() => WriteReadLnSerialIdnChecked("PAC:POL?", true);
        public bool PacFreq(uint val) => WriteReadLnSerialAnsComp($"PAC:FREQ(?) {val}", $"{val}", true);
        public string PacFreq() => WriteReadLnSerialIdnChecked("PAC:FREQ?", true);

        // SOUR PDC (Power DC Mode)
        // The commands below will switch the calibrator to Power DC Mode
        public bool PdcPow(float val) => WriteReadLnSerialAnsComp($"PDC:POW(?) {val}", $"{val}", true);
        public string PdcPow() => WriteReadLnSerialIdnChecked("PDC:POW?", true);
        public bool PdcVolt(float val) => WriteReadLnSerialAnsComp($"PDC:VOLT(?) {val}", $"{val}", true);
        public string PdcVolt() => WriteReadLnSerialIdnChecked("PDC:VOLT?", true);
        public bool PdcCurr(float val) => WriteReadLnSerialAnsComp($"PDC:CURR(?) {val}", $"{val}", true);
        public string PdcCurr() => WriteReadLnSerialIdnChecked("PDC:CURR?", true);

        // SOUR PACI (Power AC High Current Mode)
        // The commands below will switch the calibrator to Power AC High Current Mode
        public bool PaciPow(float val) => WriteReadLnSerialAnsComp($"PACI:POW(?) {val}", $"{val}", true);
        public string PaciPow() => WriteReadLnSerialIdnChecked("PACI:POW?", true);
        public bool PaciPowUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"PACI:UNIT(?) {powUnit}", $"{powUnit}", false);
        public string PaciPowUnit() => WriteReadLnSerialIdnChecked("PACI:UNIT?", false);
        public bool PaciVolt(float val) => WriteReadLnSerialAnsComp($"PACI:VOLT(?) {val}", $"{val}", true);
        public string PaciVolt() => WriteReadLnSerialIdnChecked("PACI:VOLT?", true);
        public bool PaciCurr(float val) => WriteReadLnSerialAnsComp($"PACI:CURR(?) {val}", $"{val}", true);
        public string PaciCurr() => WriteReadLnSerialIdnChecked("PACI:CURR?", true);
        public bool PaciPhas(Pols acPol) => WriteReadLnSerialAnsComp($"PACI:PHAS(?) {acPol}", $"{acPol}", true);
        public string PaciPhas() => WriteReadLnSerialIdnChecked("PACI:PHAS?", true); // Check this function from document, it has a parameter to execute
        public bool PaciPol(Pols acPol) => WriteReadLnSerialAnsComp($"PACI:POL(?) {acPol}", $"{acPol}", true); // Check this function from document, it has a parameter to execute
        public string PaciPol() => WriteReadLnSerialIdnChecked("PACI:POL?", true);
        public bool PaciFreq(uint val) => WriteReadLnSerialAnsComp($"PACI:FREQ(?) {val}", $"{val}", true);
        public string PaciFreq() => WriteReadLnSerialIdnChecked("PACI:FREQ?", true);

        // SOUR PDCI (Power DC High Current Mode)
        // The commands below will switch the calibrator to Power DC High Current Mode
        public bool PdciPow(float val) => WriteReadLnSerialAnsComp($"PDCI:POW(?) {val}", $"{val}", true);
        public string PdciPow() => WriteReadLnSerialIdnChecked("PDCI:POW?", true);
        public bool PdciVolt(float val) => WriteReadLnSerialAnsComp($"PDCI:VOLT(?) {val}", $"{val}", true);
        public string PdciVolt() => WriteReadLnSerialIdnChecked("PDCI:VOLT?", true);
        public bool PdciCurr(float val) => WriteReadLnSerialAnsComp($"PDCI:CURR(?) {val}", $"{val}", true);
        public string PdciCurr() => WriteReadLnSerialIdnChecked("PDCI:CURR?", true);

        // SOUR PACE (Power AC Extended Mode)
        // The commands below will switch the calibrator to Power AC Extended Mode
        public string PacePow() => WriteReadLnSerialIdnChecked("PACE:POW?", true);
        public bool PacePowUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"PACE:UNIT(?) {powUnit}", $"{powUnit}", false);
        public string PacePowUnit() => WriteReadLnSerialIdnChecked("PACE:UNIT?", false);
        public bool PaceVolt(Channels ch, float val) => WriteReadLnSerialAnsComp($"PACE:VOLT{(uint)ch}(?) {val}", $"{val}", true);
        public string PaceVolt(Channels ch) => WriteReadLnSerialIdnChecked($"PACE:VOLT{(uint)ch}?", true);
        public bool PaceVoltPhas(Channels ch, float val) => WriteReadLnSerialAnsComp($"PACE:VOLT{(uint)ch}(?):PHAS(?) {val}", $"{val}", true);
        public string PaceVoltPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PACE:VOLT{(uint)ch}:PHAS?", true);
        public bool PaceVoltEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PACE:VOLT{(uint)ch}(?):ENAB(?) {stat}", $"{stat}", false);
        public string PaceVoltEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PACE:VOLT{(uint)ch}:ENAB?", false);
        public bool PaceCurr(Channels ch, float val) => WriteReadLnSerialAnsComp($"PACE:CURR{(uint)ch}(?) {val}", $"{val}", true);
        public string PaceCurr(Channels ch) => WriteReadLnSerialIdnChecked($"PACE:VOLT{(uint)ch}?", true);
        public bool PaceCurrPhas(Channels ch, uint val) => WriteReadLnSerialAnsComp($"PACE:CURR{(uint)ch}:PHAS(?) {val}", $"{val}", true);
        public string PaceCurrPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PACE:CURR{(uint)ch}:PHAS?", true);
        public bool PaceCurrEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PACE:CURR{(uint)ch}:ENAB(?) {stat}", $"{stat}", false);
        public string PaceCurrEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PACE:CURR{(uint)ch}:ENAB?", true);
        public bool PaceFreq(uint val) => WriteReadLnSerialAnsComp($"PACE:FREQ(?) {val}", $"{val}", true);
        public string PaceFreq() => WriteReadLnSerialIdnChecked($"PACE:FREQ?", true);

        // SOUR PDCE (Power DC Extended Mode)
        // The commands below will switch the calibrator to Power DC Extended Mode
        public string PdcePow() => WriteReadLnSerialIdnChecked("PDCE:POW?", true);
        public bool PdceVolt(Channels ch, float val) => WriteReadLnSerialAnsComp($"PDCE:VOLT{(uint)ch}(?) {val}", $"{val}", true);
        public string PdceVolt(Channels ch) => WriteReadLnSerialIdnChecked($"PDCE:VOLT{(uint)ch}?", true);
        public bool PdceVoltEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PDCE:VOLT{(uint)ch}(?) {stat}", $"{stat}", false);
        public string PdceVoltEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PDCE:VOLT{(uint)ch}:ENAB?", false);
        public bool PdceCurr(Channels ch, float val) => WriteReadLnSerialAnsComp($"PDCE:CURR{(uint)ch}(?) {val}", $"{val}", true);
        public string PdceCurr(Channels ch) => WriteReadLnSerialIdnChecked($"PDCE:CURR{(uint)ch}?", true);
        public bool PdceCurrEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PDCE:CURR{(uint)ch}(?) {stat}", $"{stat}", false);
        public string PdceCurrEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PDCE:CURR{(uint)ch}:ENAB?", false);

        // SOUR PHAR (Power Harmonic Mode)
        // The commands below will switch the calibrator to Power Harmonic Mode
        public string PharPow() => WriteReadLnSerialIdnChecked("PHAR:POW?", true);
        public bool PharVolt(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}(?) {val}", $"{val}", true);
        public string PharVolt(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}?", true);
        public bool PharVoltPhas(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}(?):PHAS(?) {val}", $"{val}", true);
        public string PharVoltPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}:PHAS?", true);
        public bool PharVoltEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}:ENAB(?) {stat}", $"{stat}", false);
        public string PharVoltEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}:ENAB?", false);
        public bool PharVoltHarm(Channels ch, uint harmOrder, float val) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}:HARM{harmOrder}(?) {val}", $"{val}", true);     // Harmonic order range (2 to 50)
        public string PharVoltHarm(Channels ch, uint harmOrder) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}:HARM{harmOrder}?", true);             // Harmonic order range (1 to 50)
        public bool PharVoltHarmPhas(Channels ch, uint harmOrder, float val) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}:HARM{harmOrder}:PHAS(?) {val}", $"{val}", true);
        public string PharVoltHarmPhas(Channels ch, uint harmOrder) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}:HARM{harmOrder}:PHAS?", true);
        public bool PharVoltMod(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}:MOD(?) {val}", $"{val}", true);
        public string PharVoltMod(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}:MOD?", true);
        public bool PharVoltModShap(Channels ch, ModShaps modShap) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}:MOD:SHAP(?) {modShap}", $"{modShap}", false);
        public string PharVoltModShap(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}:MOD?", true);
        public bool PharVoltModDuty(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:VOLT{(uint)ch}:MOD:DUTY(?) {val}", $"{val}", true);
        public string PharVoltModDuty(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:VOLT{(uint)ch}:MOD:DUTY?", true);
        public bool PharCurr(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}(?) {val}", $"{val}", true);
        public string PharCurr(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}?", true);
        public bool PharCurrPhas(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}:PHAS(?) {val}", $"{val}", true);
        public string PharCurrPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}:PHAS?", true);
        public bool PharCurrEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}:ENAB(?) {stat}", $"{stat}", false);
        public string PharCurrEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}:ENAB?", false);
        public bool PharCurrHarm(Channels ch, uint harmOrder, float val) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}:HARM{harmOrder}(?) {val}", $"{val}", true);     // Harmonic order range (2 to 50)
        public string PharCurrHarm(Channels ch, uint harmOrder) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}:HARM{harmOrder}?", true);             // Harmonic order range (1 to 50)
        public bool PharCurrHarmPhas(Channels ch, uint harmOrder, float val) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}:HARM{harmOrder}:PHAS(?) {val}", $"{val}", true);
        public string PharCurrHarmPhas(Channels ch, uint harmOrder) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}:HARM{harmOrder}:PHAS?", true);
        public bool PharCurrMod(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}:MOD(?) {val}", $"{val}", true);
        public string PharCurrMod(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}:MOD?", true);
        public bool PharCurrModShap(Channels ch, ModShaps modShap) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}:MOD:SHAP(?) {modShap}", $"{modShap}", false);
        public string PharCurrModShap(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}:MOD?", false);
        public bool PharCurrModDuty(Channels ch, float val) => WriteReadLnSerialAnsComp($"PHAR:CURR{(uint)ch}:MOD:DUTY(?) {val}", $"{val}", true);
        public string PharCurrModDuty(Channels ch) => WriteReadLnSerialIdnChecked($"PHAR:CURR{(uint)ch}:MOD:DUTY?", true);
        public bool PharFreq(uint val) => WriteReadLnSerialAnsComp($"PHAR:FREQ(?) {val}", $"{val}", true);
        public string PharFreq() => WriteReadLnSerialIdnChecked($"PHAR:FREQ?", true);
        public bool PharFreqMod(float val) => WriteReadLnSerialAnsComp($"PHAR:FREQ:MOD(?) {val}", $"{val}", true);
        public string PharFreqMod() => WriteReadLnSerialIdnChecked($"PHAR:FREQ:MOD?", true);

        // SOUR PIH (Power Interharmonic Mode)
        // The commands below will switch the calibrator to Power Interharmonic Mode
        public void Pih() => WriteLnSerialIdnChecked("PIH");
        public bool PihVolt(Channels ch, float val) => WriteReadLnSerialAnsComp($"PIH:VOLT{(uint)ch}(?) {val}", $"{val}", true);
        public string PihVolt(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:VOLT{(uint)ch}?", true);
        public bool PihVoltPhas(Channels ch, uint val) => WriteReadLnSerialAnsComp($"PIH:VOLT{(uint)ch}:PHAS(?) {val}", $"{val}", true);
        public string PihVoltPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:VOLT{(uint)ch}:PHAS?", true);
        public bool PihVoltEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PIH:VOLT{(uint)ch}:ENAB(?) {stat}", $"{stat}", false);
        public string PihVoltEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:VOLT{(uint)ch}:ENAB?", false);
        public bool PihVoltIhar(Channels ch, float val) => WriteReadLnSerialAnsComp($"PIH:VOLT{(uint)ch}:IHAR(?) {val}", $"{val}", true);
        public string PihVoltIhar(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:VOLT{(uint)ch}:IHAR?", true);
        public bool PihCurr(Channels ch, float val) => WriteReadLnSerialAnsComp($"PIH:CURR{(uint)ch}(?) {val}", $"{val}", true);
        public string PihCurr(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:CURR{(uint)ch}?", true);
        public bool PihCurrPhas(Channels ch, uint val) => WriteReadLnSerialAnsComp($"PIH:CURR{(uint)ch}:PHAS(?) {val}", $"{val}", true);
        public string PihCurrPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:CURR{(uint)ch}:PHAS?", true);
        public bool PihCurrEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PIH:CURR{(uint)ch}:ENAB(?) {stat}", $"{stat}", false);
        public string PihCurrEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:CURR{(uint)ch}:ENAB?", true);
        public bool PihCurrIhar(Channels ch, float val) => WriteReadLnSerialAnsComp($"PIH:CURR{(uint)ch}:IHAR(?) {val}", $"{val}", true);
        public string PihCurrIhar(Channels ch) => WriteReadLnSerialIdnChecked($"PIH:CURR{(uint)ch}:IHAR?", true);
        public bool PihFreq(uint val) => WriteReadLnSerialAnsComp($"PIH:FREQ(?) {val}", $"{val}", true);
        public string PihFreq() => WriteReadLnSerialIdnChecked($"PIH:FREQ?", true);
        public bool PihFreqIhar(uint val) => WriteReadLnSerialAnsComp($"PIH:FREQ:IHAR(?) {val}", $"{val}", true);
        public string PihFreqIhar() => WriteReadLnSerialIdnChecked($"PIH:FREQ:IHAR?", true);

        // SOUR PDIP (Power Dip/Swell Mode)
        // The commands below will switch the calibrator to Power Dip/Swell Mode
        public void Pdip() => WriteLnSerialIdnChecked("PDIP");
        public bool PdipVolt(Channels ch, float val) => WriteReadLnSerialAnsComp($"PDIP:VOLT{(uint)ch}(?) {val}", $"{val}", true);
        public string PdipVolt(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:VOLT{(uint)ch}?", true);
        public bool PdipVoltPhas(Channels ch, uint val) => WriteReadLnSerialAnsComp($"PDIP:VOLT{(uint)ch}:PHAS(?) {val}", $"{val}", true);
        public string PdipVoltPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:VOLT{(uint)ch}:PHAS?", true);
        public bool PdipVoltEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PDIP:VOLT{(uint)ch}:ENAB(?) {stat}", $"{stat}", false);
        public string PdipVoltEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:VOLT{(uint)ch}:ENAB?", false);
        public bool PdipVoltChan(Channels ch, float val) => WriteReadLnSerialAnsComp($"PDIP:VOLT{(uint)ch}:CHAN(?) {val}", $"{val}", true);
        public string PdipVoltChan(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:VOLT{(uint)ch}:CHAN?", true);
        public bool PdipCurr(Channels ch, float val) => WriteReadLnSerialAnsComp($"PDIP:CURR{(uint)ch}(?) {val}", $"{val}", true);
        public string PdipCurr(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:CURR{(uint)ch}?", true);
        public bool PdipCurrPhas(Channels ch, uint val) => WriteReadLnSerialAnsComp($"PDIP:CURR{(uint)ch}:PHAS(?) {val}", $"{val}", true);
        public string PdipCurrPhas(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:CURR{(uint)ch}:PHAS?", true);
        public bool PdipCurrEnab(Channels ch, OutpStats stat) => WriteReadLnSerialAnsComp($"PDIP:CURR{(uint)ch}:ENAB(?) {stat}", $"{stat}", false);
        public string PdipCurrEnab(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:CURR{(uint)ch}:ENAB?", false);
        public bool PdipCurrChan(Channels ch, float val) => WriteReadLnSerialAnsComp($"PDIP:CURR{(uint)ch}:CHAN(?) {val}", $"{val}", true);
        public string PdipCurrChan(Channels ch) => WriteReadLnSerialIdnChecked($"PDIP:CURR{(uint)ch}:CHAN?", true);
        public bool PdipFreq(uint val) => WriteReadLnSerialAnsComp($"PDIP:FREQ(?) {val}", $"{val}", true);
        public string PdipFreq() => WriteReadLnSerialIdnChecked($"PDIP:FREQ?", true);
        public bool PdipTime(string time) => WriteReadLnSerialAnsComp($"PDIP:TIME(?) {time}", $"{time}", true);
        public string PdipTime() => WriteReadLnSerialIdnChecked($"PDIP:TIME?", true);
        public bool PdipTrigInp(TrigInps inp) => WriteReadLnSerialAnsComp($"PDIP:TRIG:INP(?) {inp}", $"{inp}", false);
        public string PdipTrigInp() => WriteReadLnSerialIdnChecked($"PDIP:TRIG:INP?", true);
        public bool PdipTrigSync(OutpStats stat) => WriteReadLnSerialAnsComp($"PDIP:TRIG:SYNC(?) {stat}", $"{stat}", false);
        public string PdipTrigSync() => WriteReadLnSerialIdnChecked($"PDIP:TRIG:SYNC?", true);
        public bool PdipTrigRep(TrigReps rep) => WriteReadLnSerialAnsComp($"PDIP:TRIG:REP(?) {rep}", $"{rep}", false);
        public string PdipTrigRep() => WriteReadLnSerialIdnChecked($"PDIP:TRIG:REP?", false);
        public void PdipTrigStar() => WriteLnSerialIdnChecked("PDIP:TRIG:STAR");

        // SOUR EAC (Energy AC Mode)
        // The commands below will switch the calibrator to Energy AC Mode
        public string EacEner() => WriteReadLnSerialIdnChecked("EAC:ENER?", true);
        public string EacDev() => WriteReadLnSerialIdnChecked("EAC:DEV?", true);
        public string EacPow() => WriteReadLnSerialIdnChecked("EAC:POW?", true);
        public bool EacPowUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"EAC:UNIT(?) {powUnit}", $"{powUnit}", false);
        public string EacPowUnit() => WriteReadLnSerialIdnChecked("EAC:POW:UNIT?", false);
        public bool EacVolt(float val) => WriteReadLnSerialAnsComp($"EAC:VOLT(?) {val}", $"{val}", true);
        public string EacVolt() => WriteReadLnSerialIdnChecked($"EAC:VOLT?", true);
        public bool EacCurr(float val) => WriteReadLnSerialAnsComp($"EAC:CURR(?) {val}", $"{val}", true);
        public string EacCurr() => WriteReadLnSerialIdnChecked($"EAC:CURR?", true);
        public bool EacCurrPhas(float val) => WriteReadLnSerialAnsComp($"EAC:CURR:PHAS(?) {val}", $"{val}", true); // This command will return different format if (PHAS:UNIT == COS)
        public string EacCurrPhas() => WriteReadLnSerialIdnChecked($"EAC:CURR:PHAS?", true);
        public bool EacCurrPol(Pols pols) => WriteReadLnSerialAnsComp($"EAC:POL(?) {pols}", $"{pols}", true);
        public string EacCurrPol() => WriteReadLnSerialIdnChecked($"EAC:POL?", true);
        public bool EacFreq(uint val) => WriteReadLnSerialAnsComp($"EAC:FREQ(?) {val}", $"{val}", true);
        public string EacFreq() => WriteReadLnSerialIdnChecked($"EAC:FREQ?", true);
        public bool EacCont(Conts cont) => WriteReadLnSerialAnsComp($"EAC:CONT(?) {cont}", $"{cont}", false);
        public string EacCont() => WriteReadLnSerialIdnChecked($"EAC:CONT?", false);
        public bool EacTime(uint val) => WriteReadLnSerialAnsComp($"EAC:TIME(?) {val}", $"{val}", true);
        public string EacTime() => WriteReadLnSerialIdnChecked($"EAC:TIME?", true);
        public bool EacCons(uint val) => WriteReadLnSerialAnsComp($"EAC:CONS(?) {val}", $"{val}", true);
        public string EacCons() => WriteReadLnSerialIdnChecked($"EAC:CONS?", true);
        public bool EacTestTime(uint val) => WriteReadLnSerialAnsComp($"EAC:TEST:TIME(?) {val}", $"{val}", true);
        public string EacTestTime() => WriteReadLnSerialIdnChecked($"EAC:TEST:TIME?", true);
        public bool EacTestCoun(uint val) => WriteReadLnSerialAnsComp($"EAC:TEST:COUN(?) {val}", $"{val}", true);
        public string EacTestCoun() => WriteReadLnSerialIdnChecked($"EAC:TEST:COUN?", true);
        public string EacTestFreq() => WriteReadLnSerialIdnChecked($"EAC:TEST:FREQ?", true);
        public bool EacWupTime(uint val) => WriteReadLnSerialAnsComp($"EAC:WUP:TIME(?) {val}", $"{val}", true);
        public string EacWupTime() => WriteReadLnSerialIdnChecked($"EAC:WUP:TIME?", true);
        public bool EacWupCoun(uint val) => WriteReadLnSerialAnsComp($"EAC:WUP:COUN(?) {val}", $"{val}", true);
        public string EacWupCoun() => WriteReadLnSerialIdnChecked($"EAC:WUP:COUN?", true);

        // SOUR EAC (Energy DC Mode)
        // The commands below will switch the calibrator to Energy DC Mode
        public string EdcEner() => WriteReadLnSerialIdnChecked($"EDC:ENER?", true);
        public string EdcDev() => WriteReadLnSerialIdnChecked($"EDC:DEV?", true);
        public string EdcPow() => WriteReadLnSerialIdnChecked($"EDC:POW?", true);
        public bool EdcVolt(float val) => WriteReadLnSerialAnsComp($"EDC:VOLT(?) {val}", $"{val}", true);
        public string EdcVolt() => WriteReadLnSerialIdnChecked($"EDC:VOLT?", true);
        public bool EdcCurr(float val) => WriteReadLnSerialAnsComp($"EDC:CURR(?) {val}", $"{val}", true);
        public string EdcCurr() => WriteReadLnSerialIdnChecked($"EDC:CURR?", true);
        public bool EdcCont(Conts cont) => WriteReadLnSerialAnsComp($"EDC:CONT(?) {cont}", $"{cont}", false);
        public string EdcCont() => WriteReadLnSerialIdnChecked($"EDC:CONT?", false);
        public bool EdcTime(uint val) => WriteReadLnSerialAnsComp($"EDC:TIME(?) {val}", $"{val}", true);
        public string EdcTime() => WriteReadLnSerialIdnChecked($"EDC:TIME?", true);
        public bool EdcCons(uint val) => WriteReadLnSerialAnsComp($"EDC:CONS(?) {val}", $"{val}", true);
        public string EdcCons() => WriteReadLnSerialIdnChecked($"EDC:CONS?", true);
        public bool EdcTestTime(uint val) => WriteReadLnSerialAnsComp($"EDC:TEST:TIME(?) {val}", $"{val}", true);
        public string EdcTestTime() => WriteReadLnSerialIdnChecked($"EDC:TEST:TIME?", true);
        public bool EdcTestCoun(uint val) => WriteReadLnSerialAnsComp($"EDC:TEST:COUN(?) {val}", $"{val}", true);
        public string EdcTestCoun() => WriteReadLnSerialIdnChecked($"EDC:TEST:COUN?", true);
        public string EdcTestFreq() => WriteReadLnSerialIdnChecked($"EDC:TEST:FREQ?", true);
        public bool EdcWupTime(uint val) => WriteReadLnSerialAnsComp($"EDC:WUP:TIME(?) {val}", $"{val}", true);
        public string EdcWupTime() => WriteReadLnSerialIdnChecked($"EDC:WUP:TIME?", true);
        public bool EdcWupCoun(uint val) => WriteReadLnSerialAnsComp($"EDC:WUP:COUN(?) {val}", $"{val}", true);
        public string EdcWupCoun() => WriteReadLnSerialIdnChecked($"EDC:WUP:COUN?", true);

        // SOUR EACI (Energy AC High Current Mode)
        // The commands below will switch the calibrator to Energy AC High Current Mode
        public string EaciEner() => WriteReadLnSerialIdnChecked($"EACI:ENER?", true);
        public string EaciDev() => WriteReadLnSerialIdnChecked($"EACI:DEV?", true);
        public string EaciPow() => WriteReadLnSerialIdnChecked($"EACI:POW?", true);
        public bool EaciPowUnit(PowUnits powUnit) => WriteReadLnSerialAnsComp($"EACI:UNIT(?) {powUnit}", $"{powUnit}", false);
        public string EaciPowUnit() => WriteReadLnSerialIdnChecked($"EACI:UNIT?", false);
        public bool EaciVolt(float val) => WriteReadLnSerialAnsComp($"EACI:VOLT(?) {val}", $"{val}", true);
        public string EaciVolt() => WriteReadLnSerialIdnChecked($"EACI:VOLT?", true);
        public bool EaciCurr(float val) => WriteReadLnSerialAnsComp($"EACI:CURR(?) {val}", $"{val}", true);
        public string EaciCurr() => WriteReadLnSerialIdnChecked($"EACI:CURR?", true);
        public bool EaciCurrPhas(float val) => WriteReadLnSerialAnsComp($"EACI:CURR:PHAS(?) {val}", $"{val}", true); // This command will return different format if (PHAS:UNIT == COS)
        public string EaciCurrPhas() => WriteReadLnSerialIdnChecked($"EACI:CURR:PHAS?", true);
        public bool EaciCurrPol(Pols pols) => WriteReadLnSerialAnsComp($"EACI:POL(?) {pols}", $"{pols}", true);
        public string EaciCurrPol() => WriteReadLnSerialIdnChecked($"EACI:POL?", true);
        public bool EaciFreq(uint val) => WriteReadLnSerialAnsComp($"EACI:FREQ(?) {val}", $"{val}", true);
        public string EaciFreq() => WriteReadLnSerialIdnChecked($"EACI:FREQ?", true);
        public bool EaciCont(Conts cont) => WriteReadLnSerialAnsComp($"EACI:CONT(?) {cont}", $"{cont}", false);
        public string EaciCont() => WriteReadLnSerialIdnChecked($"EACI:CONT?", false);
        public bool EaciTime(uint val) => WriteReadLnSerialAnsComp($"EACI:TIME(?) {val}", $"{val}", true);
        public string EaciTime() => WriteReadLnSerialIdnChecked($"EACI:TIME?", true);
        public bool EaciCons(uint val) => WriteReadLnSerialAnsComp($"EACI:CONS(?) {val}", $"{val}", true);
        public string EaciCons() => WriteReadLnSerialIdnChecked($"EACI:CONS?", true);
        public bool EaciTestTime(uint val) => WriteReadLnSerialAnsComp($"EACI:TEST:TIME(?) {val}", $"{val}", true);
        public string EaciTestTime() => WriteReadLnSerialIdnChecked($"EACI:TEST:TIME?", true);
        public bool EaciTestCoun(uint val) => WriteReadLnSerialAnsComp($"EACI:TEST:COUN(?) {val}", $"{val}", true);
        public string EaciTestCoun() => WriteReadLnSerialIdnChecked($"EACI:TEST:COUN?", true);
        public string EaciTestFreq() => WriteReadLnSerialIdnChecked($"EACI:TEST:FREQ?", true);
        public bool EaciWupTime(uint val) => WriteReadLnSerialAnsComp($"EACI:WUP:TIME(?) {val}", $"{val}", true);
        public string EaciWupTime() => WriteReadLnSerialIdnChecked($"EACI:WUP:TIME?", true);
        public bool EaciWupCoun(uint val) => WriteReadLnSerialAnsComp($"EACI:WUP:COUN(?) {val}", $"{val}", true);
        public string EaciWupCoun() => WriteReadLnSerialIdnChecked($"EACI:WUP:COUN?", true);

        // SOUR EDCI (Energy DC High Current Mode)
        // The commands below will switch the calibrator to Energy AC High Current Mode
        public string EdciEner() => WriteReadLnSerialIdnChecked($"EDCI:ENER?", true);
        public string EdciDev() => WriteReadLnSerialIdnChecked($"EDCI:DEV?", true);
        public string EdciPow() => WriteReadLnSerialIdnChecked($"EDCI:POW?", true);
        public bool EdciVolt(float val) => WriteReadLnSerialAnsComp($"EDCI:VOLT(?) {val}", $"{val}", true);
        public string EdciVolt() => WriteReadLnSerialIdnChecked($"EDCI:VOLT?", true);
        public bool EdciCurr(float val) => WriteReadLnSerialAnsComp($"EDCI:CURR(?) {val}", $"{val}", true);
        public string EdciCurr() => WriteReadLnSerialIdnChecked($"EDCI:CURR?", true);
        public bool EdciCont(Conts cont) => WriteReadLnSerialAnsComp($"EDCI:CONT(?) {cont}", $"{cont}", false);
        public string EdciCont() => WriteReadLnSerialIdnChecked($"EDCI:CONT?", false);
        public bool EdciTime(uint val) => WriteReadLnSerialAnsComp($"EDCI:TIME(?) {val}", $"{val}", true);
        public string EdciTime() => WriteReadLnSerialIdnChecked($"EDCI:TIME?", true);
        public bool EdciCons(uint val) => WriteReadLnSerialAnsComp($"EDCI:CONS(?) {val}", $"{val}", true);
        public string EdciCons() => WriteReadLnSerialIdnChecked($"EDCI:CONS?", true);
        public bool EdciTestTime(uint val) => WriteReadLnSerialAnsComp($"EDCI:TEST:TIME(?) {val}", $"{val}", true);
        public string EdciTestTime() => WriteReadLnSerialIdnChecked($"EDCI:TEST:TIME?", true);
        public bool EdciTestCoun(uint val) => WriteReadLnSerialAnsComp($"EDCI:TEST:COUN(?) {val}", $"{val}", true);
        public string EdciTestCoun() => WriteReadLnSerialIdnChecked($"EDCI:TEST:COUN?", true);
        public string EdciTestFreq() => WriteReadLnSerialIdnChecked($"EDCI:TEST:FREQ?", true);
        public bool EdciWupTime(uint val) => WriteReadLnSerialAnsComp($"EDCI:WUP:TIME(?) {val}", $"{val}", true);
        public string EdciWupTime() => WriteReadLnSerialIdnChecked($"EDCI:WUP:TIME?", true);
        public bool EdciWupCoun(uint val) => WriteReadLnSerialAnsComp($"EDCI:WUP:COUN(?) {val}", $"{val}", true);
        public string EdciWupCoun() => WriteReadLnSerialIdnChecked($"EDCI:WUP:COUN?", true);

        // SOUR VAC (Voltage AC Mode)
        // The commands below will switch the calibrator to Voltage AC Mode
        public bool VacVolt(float val) => WriteReadLnSerialAnsComp($"VAC:VOLT(?) {val}", $"{val}", true);
        public string VacVolt() => WriteReadLnSerialIdnChecked($"VAC:VOLT?", true);
        public bool VacFreq(uint val) => WriteReadLnSerialAnsComp($"VAC:FREQ(?) {val}", $"{val}", true);
        public string VacFreq() => WriteReadLnSerialIdnChecked($"VAC:FREQ?", true);

        // SOUR VDC (Voltage DC Mode)
        // The commands below will switch the calibrator to Voltage DC Mode
        public bool VdcVolt(float val) => WriteReadLnSerialAnsComp($"VDC:VOLT(?) {val}", $"{val}", true);
        public string VdcVolt() => WriteReadLnSerialIdnChecked($"VDC:VOLT?", true);

        // SOUR CAC (Current AC Mode)
        // The commands below will switch the calibrator to Current AC Mode
        public bool CacCurr(float val) => WriteReadLnSerialAnsComp($"CAC:CURR(?) {val}", $"{val}", true);
        public string CacCurr() => WriteReadLnSerialIdnChecked($"CAC:CURR?", true);
        public bool CacFreq(uint val) => WriteReadLnSerialAnsComp($"CAC:FREQ(?) {val}", $"{val}", true);
        public string CacFreq() => WriteReadLnSerialIdnChecked($"CAC:FREQ?", true);

        // SOUR CDC (Current DC Mode)
        // The commands below will switch the calibrator to Current DC Mode
        public bool CdcCurr(float val) => WriteReadLnSerialAnsComp($"CDC:CURR(?) {val}", $"{val}", true);
        public string CdcCurr() => WriteReadLnSerialIdnChecked($"CDC:CURR?", true);

        // SOUR CACI (Current AC High Current Mode)
        // The commands below will switch the calibrator to Current AC High Current Mode
        public bool CaciCurr(float val) => WriteReadLnSerialAnsComp($"CACI:CURR(?) {val}", $"{val}", true);
        public string CaciCurr() => WriteReadLnSerialIdnChecked($"CACI:CURR?", true);
        public bool CaciFreq(uint val) => WriteReadLnSerialAnsComp($"CACI:FREQ(?) {val}", $"{val}", true);
        public string CaciFreq() => WriteReadLnSerialIdnChecked($"CACI:FREQ?", true);

        // SOUR CDCI (Current DC High Current Mode)
        // The commands below will switch the calibrator to Current DC High Current Mode
        public bool CdciCurr(float val) => WriteReadLnSerialAnsComp($"CDCI:CURR(?) {val}", $"{val}", true);
        public string CdciCurr() => WriteReadLnSerialIdnChecked($"CDCI:CURR?", true);

        /**************************************************************************************************************************************************************************/


        // SYSTem subsystem commands
        public bool SystDate(string val) => WriteReadLnSerialAnsComp($"SYST:DATE(?) {val}", $"{val}", false);      // val => (YYYY,MM,DD) (YYYY = year (2000-2099)) (MM = month (01-12)) (DD = day (01-31))
        public string SystDate() => WriteReadLnSerialIdnChecked($"SYST:DATE?", false);
        public bool SystTime(string val) => WriteReadLnSerialAnsComp($"SYST:TIME(?) {val}", $"{val}", false);      // val => (HH,MM,SS) (HH = hour (00-23)) (MM = minute (00-59)) (SS = second (00-59))
        public string SystTime() => WriteReadLnSerialIdnChecked($"SYST:TIME?", false);
        public string SystErr() => WriteReadLnSerialIdnChecked($"SYST:ERR?", false);
        public void SystRem() => WriteLnSerial("SYST:REM");
        public void SystLoc() => WriteLnSerial("SYST:LOC");
        public void SystRwl() => WriteLnSerial("SYST:RWL");


        // STATus subsystem commands
        public string StatOperEven() => WriteReadLnSerialIdnChecked($"STAT:OPER:EVEN?", false);
        public void StatOperEnab(byte val) => WriteLnSerial($"STAT:OPER:ENAB {val}");
        public string StatOperEnab() => WriteReadLnSerialIdnChecked($"STAT:OPER:ENAB?", false);
        public string StatOperCond() => WriteReadLnSerialIdnChecked($"STAT:OPER:COND?", false);
        public string StatQuesEven() => WriteReadLnSerialIdnChecked($"STAT:QUES:EVEN?", false);
        public void StatQuesEnab(byte val) => WriteLnSerial($"STAT:QUES:ENAB {val}");
        public string StatQuesEnab() => WriteReadLnSerialIdnChecked($"STAT:QUES:ENAB?", false);
        public string StatQuesCond() => WriteReadLnSerialIdnChecked($"STAT:QUES:COND?", false);
        public void StatPres() => WriteLnSerial("STAT:PRES");

    }
}
