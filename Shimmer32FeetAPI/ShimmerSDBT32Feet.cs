﻿/* * Rev 0.6
 * 
 * Changes since Rev 0.5
 * - Removed startstreamandlog when Shimmer button pressed
 * - Removed computer driver/directory search
 * - Removed sd card config file writing (firmware handles this)
 * 
 * Changes since Rev 0.4
 * - Add read pressure sensor coefficients to initializeshimmer3sdbt
 * - fixed problem with initializeshimmer2
 * - Removed serial port functions
 * 
 *  Changes since Rev 0.3
 * - include sleep before sending start streaming cmd
 * - update writesdconfig which was writing wrong sampling rate
 * - include get status command in stop streaming to get dock status, so the ui can update between Stream and Log / Stream
 * 
 * Changes since Rev 0.2
 * - Removed MessageBox
 * - Added ShimmerSDBTMinorIdentifier, for easier identification of message
 * - Included TempDrivePath, to set the drivePath when there more than one Shimmer device
 * - Removed SDConfigWriteFinish
 * 
 * Changes since Rev 0.1
 * - Removed delegates, switched to virtual and override methods
 * - Removed Backgroundworker
 * - Remove UICallBackSDBT, now using OnMyPropertyChanged inherited method
 * 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel;

namespace ShimmerAPI
{
    [System.Obsolete] //Moving the serial port controls (Shimmer32Feet.cs) to the highest implementing class so code is more concise and extendible for use with 32 feet/xamarin/etc
    public class ShimmerSDBT32Feet : Shimmer32Feet
    {

        public enum ShimmerSDBTMinorIdentifier
        {
            MSG_WARNING = 0,
            MSG_EXTRA_REMOVABLE_DEVICES_DETECTED = 1,
            MSG_ERROR = 2,
        }

        public ShimmerSDBT32Feet(String devID, String bComPort)
            : base(devID, bComPort)
        {
            //PControlForm = controlForm;
            
            isLogging = false;
            CurrentSensingStatus = false;
        }

        //Shimmer3 Constructor
        public ShimmerSDBT32Feet(String devName, String bluetoothAddress, double samplingRate, int accelRange, int gsrRange, int setEnabledSensors, bool enableLowPowerAccel, bool enableLowPowerGyro, bool enableLowPowerMag, int gyroRange, int magRange, byte[] exg1configuration, byte[] exg2configuration, bool internalexppower)
            :base(devName, bluetoothAddress, samplingRate, accelRange, gsrRange, setEnabledSensors, enableLowPowerAccel, enableLowPowerGyro, enableLowPowerMag, gyroRange, magRange, exg1configuration, exg2configuration, internalexppower)
        {

        }

        //Shimmer2R
        public ShimmerSDBT32Feet(String devID, String bluetoothAddress, double samplingRate, int AccelRange, int GyroRange, int gsrRange, int setEnabledSensors)
            : base(devID, bluetoothAddress, samplingRate, AccelRange, GyroRange, gsrRange, setEnabledSensors)
        {
        }
        //===================================================================
        //===================================================================
        //===================================================================
        //==================== btsd changes from Shimmer.cs =================
        //===================================================================
        //===================================================================
        //===================================================================


        // btsd changes
        public enum TrialConfigBitmap
        {
            //byte 0
            Reg5v = 0x80,
            PMUX = 0x40,
            UserButton = 0x20,
            GyroButton = 0x10,
            Sync = 0x04,
            IAmMaster = 0x02,
            //byte 1
            SingleTouch = 0x80,
            TCXO = 0x10,
            ExpPower = 0x08

        }

        public enum SensorBitmapShimmer3_unused
        {
            SensorMpuAccel = 0x400000,
            SensorMpuMag = 0x200000
        }

        public enum ChannelContents
        {
            XLNAccel = 0x00,
            YLNAccel = 0x01,
            ZLNAccel = 0x02,
            VBatt = 0x03,
            XWRAccel = 0x04,
            YWRAccel = 0x05,
            ZWRAccel = 0x06,
            XMag = 0x07,
            YMag = 0x08,
            ZMag = 0x09,
            XGyro = 0x0A,
            YGyro = 0x0B,
            ZGyro = 0x0C,
            ExternalAdc7 = 0x0D,
            ExternalAdc6 = 0x0E,
            ExternalAdc15 = 0x0F,
            InternalAdc1 = 0x10,
            InternalAdc12 = 0x11,
            InternalAdc13 = 0x12,
            InternalAdc14 = 0x13,
            AlternativeXAccel = 0x14, //Unsupported
            AlternativeYAccel = 0x15, //Unsupported
            AlternativeZAccel = 0x16, //Unsupported
            AlternativeXMag = 0x17, //Unsupported
            AlternativeYMag = 0x18, //Unsupported
            AlternativeZMag = 0x19, //Unsupported
            Temperature = 0x1A,
            Pressure = 0x1B,
            Exg1_Status = 0x1D,
            Exg1_CH1 = 0x1E,
            Exg1_CH2 = 0x1F,
            Exg2_Status = 0x20,
            Exg2_CH1 = 0x21,
            Exg2_CH2 = 0x22,
            Exg1_CH1_16Bit = 0x23,
            Exg1_CH2_16Bit = 0x24,
            Exg2_CH1_16Bit = 0x25,
            Exg2_CH2_16Bit = 0x26,
            STRAIN_HIGH = 0x27,
            STRAIN_LOW = 0x28,
            GsrRaw = 0x1C
        }
        // btsd changes
        private int trialConfig;
        private int interval;

        // btsd changes
        /*private int accelSamplingRate;//byte0(7-4)
        private int accelRange;//(3-2)
        private int accelLP;//(1)
        private int accelHR;//(0)
        private int mpu9150SamplingRate;//byte1(7-0)
        private int magGain;//byte2(7-5)
        private int magSamplingRate;//(4-2)
        private int gyroRange;//(1-0)
        private int mpu9150AccelRange;//byte3(7-6)
        private int pressureResolution;//(5-4)
        private int gsrRange;//(3-1)
        private int cfgExpPower;//(0)
        private int configSetupByte0;*/

        public bool isLogging = false;
        // btsd changes 2
        //public Control PControlForm;
        public const string AppNameCapture = "ShimmerCapture";

        // btsd changes
        public bool changeExgCfgVals;
        public bool changeTrial;
        public bool changeCenter;
        public bool changeMyID;
        public bool changeNshimmer;
        public bool changeShimmerName;
        public bool changeExpID;
        public bool changeConfigTime;

        // btsd changes
        private bool sync;
        private bool userButton;
        private bool iAmMaster;
        private bool singleTouch;
        private bool tcxo;
        private bool expPower;
        private bool monitor;
        private int clock_freq;

        // btsd changes
        private string center = null;
        private int myid = 0;
        private int Nshimmer = 0;
        private string shimmername = null;
        private string SdDir = "";
        private string experimentid = null;
        private long configtime = 0;
        public byte[] storedConfig = new byte[118];

        // btsd changes
        private bool dataReceived;
        // btsd changes
        public bool CurrentDockStatus = false;
        public bool CurrentSensingStatus = false;

        String TempDrivePath;

        // btsd changes
        public void fillTrialShimmer3(List<byte> packet)
        {
            SplitTrialConfig(packet[0] + (packet[1] << 8));
            SetInterval((int)packet[2]);
        }

        // btsd changes
        public override bool GetSync() { return sync; }
        public override bool GetUserButton() { return userButton; }
        public override bool GetIAmMaster() { return iAmMaster; }
        public override bool GetSingleTouch() { return singleTouch; }
        public override bool GetTcxo() { return tcxo; }
        public override bool GetExpPower() { return expPower; }

        public override void SetSync(bool val) { sync = val; }
        public override void SetUserButton(bool val) { userButton = val; }
        public override void SetIAmMaster(bool val) { iAmMaster = val; }
        public override void SetSingleTouch(bool val) { singleTouch = val; }
        public override void SetTcxo(bool val) { tcxo = val; }
        public override void SetExpPower(bool val) { expPower = val; }
        public override void SetMonitor(bool val) { monitor = val; }

        public override void SetDataReceived(bool val) { dataReceived = val; }
        public override bool GetDataReceived() { return dataReceived; }
        public override int GetClockTCXO() { return clock_freq; }
        public override void SetClockTCXO(int tcxo)
        {
            if (tcxo == 1)
                clock_freq = 255765;
            else
                clock_freq = 32768;
        }

        // btsd changes
        public void SplitTrialConfig(int val)
        {
            trialConfig = val;
            sync = ((val >> 2) & 0x01) == 1;
            userButton = ((val >> 5) & 0x01) == 1;
            iAmMaster = ((val >> 1) & 0x01) == 1;
            singleTouch = ((val >> 15) & 0x01) == 1;
            tcxo = ((val >> 12) & 0x01) == 1;
            expPower = ((val >> 11) & 0x01) == 1;
            monitor = ((val >> 10) & 0x01) == 1;
        }

        // btsd changes
        public void CombineTrialConfig()
        {
            trialConfig = (((sync ? 1 : 0) & 0x01) << 2) +
                          (((userButton ? 1 : 0) & 0x01) << 5) +
                          (((iAmMaster ? 1 : 0) & 0x01) << 1) +
                          (((singleTouch ? 1 : 0) & 0x01) << 15) +
                          (((tcxo ? 1 : 0) & 0x01) << 12) +
                          (((expPower ? 1 : 0) & 0x01) << 11) +
                          (((monitor ? 1 : 0) & 0x01) << 10);
        }

        public int GetTrialConfig()
        {
            CombineTrialConfig();
            return trialConfig;
        }

        public override void SetInterval(int val)
        {
            if ((val >= 54) && (val <= 255))
                interval = val;
            else
                interval = 120;
            interval = val;
        }

        public override int GetInterval() { return interval; }
        public override void SetCenter(string val) { center = val; }
        public override string GetCenter() { return center; }
        public override void SetMyID(int val) { myid = val; }
        public override int GetMyID() { return myid; }
        public override void SetNshimmer(int val) { Nshimmer = val; }
        public override int GetNshimmer() { return Nshimmer; }
        public override void SetShimmerName(string val) { shimmername = val; }
        public override string GetShimmerName() { return shimmername; }
        public override void SetSdDir(string val) { SdDir = val; }
        public override string GetSdDir() { return SdDir; }
        public override void SetExperimentID(string val) { experimentid = val; }
        public override string GetExperimentID() { return experimentid; }
        public override void SetConfigTime(long val) { configtime = val; }
        public override long GetConfigTime() { return configtime; }

        public override string SystemTime2Config()
        {
            DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime currentDate = DateTime.Now;
            int lTotalSecondsElapsed = 0;

            TimeSpan tsDiff = currentDate.Subtract(startDate);
            lTotalSecondsElapsed = lTotalSecondsElapsed + tsDiff.Days * (24 * 60 * 60);
            lTotalSecondsElapsed = lTotalSecondsElapsed + (tsDiff.Hours * 60 * 60);
            lTotalSecondsElapsed = lTotalSecondsElapsed + (tsDiff.Minutes * 60);
            lTotalSecondsElapsed = lTotalSecondsElapsed + tsDiff.Seconds;

            return Convert.ToString(lTotalSecondsElapsed);
        }

        public override string ConfigTimeToShowString(long cfgtime_in)
        {
            string configtime_text = null;
            if (cfgtime_in > 0)
            {
                int dif_secs = (int)(cfgtime_in % 60);
                int dif_mins = ((int)(cfgtime_in / 60)) % 60;
                int dif_hours = ((int)(cfgtime_in / (60 * 60))) % 24;
                int dif_days = (int)(cfgtime_in / (24 * 60 * 60));
                TimeSpan tsDiff = new TimeSpan(dif_days, dif_hours, dif_mins, dif_secs);

                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0);
                DateTime configDate = startDate + tsDiff;

                configtime_text = String.Format("Last Config Time: {0,2:00}/{1,2:00}/{2,4:0000}, {3,2:00}:{4,2:00}:{5,2:00}",
                                         configDate.Day, configDate.Month, configDate.Year,
                                         configDate.Hour, configDate.Minute, configDate.Second);
            }
            else
            {
                configtime_text = "Last Config Time not found.";
            }

            return configtime_text;
        }

        public override void StopStreaming()
        {
            base.StopStreaming();
            // btsd changes 2
            if (GetFirmwareIdentifier() == 3)
            {
                System.Threading.Thread.Sleep(300);
                WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_STATUS_COMMAND }, 0, 1);
                System.Threading.Thread.Sleep(500);
                String message = "Stopped.";
                if (isLogging)
                {
                    message = "Stopped. Last SDLog Directory : " + GetSdDir();
                }

                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                OnNewEvent(newEventArgs);
                //PControlForm.ChangeStatusLabel("Stopped, Last SDLog Directory : " + GetSdDir());
                SetSdDir("");
            }

        }

        public override void StartStreamingandLog()
        {
            KeepObjectCluster = null;
            UnalignedBytesReceived = new List<Byte>();
            if (IsConnectionOpen())
            {
                if (ShimmerState == SHIMMER_STATE_CONNECTED)
                {
                    if (IsFilled)
                    {
                        StreamingACKReceived = false;
                        LastReceivedTimeStamp = 0;
                        CurrentTimeStampCycle = 0;
                        LastReceivedCalibratedTimeStamp = -1;
                        FirstTimeCalTime = true;
                        PacketLossCount = 0;
                        PacketReceptionRate = 100;
                        KeepObjectCluster = null; //This is important and is required!
                        OrientationAlgo = null;

                        SetState(SHIMMER_STATE_STREAMING);
                        if (GetFirmwareIdentifier() == 3)
                        {
                            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_STATUS_COMMAND }, 0, 1);
                            System.Threading.Thread.Sleep(500);
                        }
                        SetSdDir("");
                        if (CurrentDockStatus)
                        {
                            String message = "Streaming only mode. \nShimmer on dock, SD mode not accessible.";
                            CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_WARNING);
                            OnNewEvent(newEventArgs);
                            isLogging = false;
                        }
                        else
                        {
                            isLogging = true;
                        }

                        // btsd changes 2
                        if (GetFirmwareIdentifier() == 3)
                        {
                            //System.Console.WriteLine("ShimmerLOG: Send START SDBT CMD");
                            System.Threading.Thread.Sleep(300);
                            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.START_SDBT_COMMAND }, 0, 1);
                            System.Threading.Thread.Sleep(1500);


                            if (isLogging)
                            {
                                WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_DIR_COMMAND }, 0, 1);
                                int i = 11;
                                while (((i--) > 1) && (GetSdDir() == ""))
                                {
                                    System.Threading.Thread.Sleep(200);
                                }
                                if (i == 0)
                                {
                                    // btsd changes 2
                                    /*EventHandler handler = UICallbackSDBT;
                                    if (handler != null)
                                    {
                                        handler(this, newEventArgs);
                                    }*/
                                    String message = "BTStream + SDLog, Current SDLog Directory : Oops, could not read the directory name. Pos1.";
                                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                    OnNewEvent(newEventArgs);
                                    //PControlForm.ChangeStatusLabel("BTStream + SDLog, Current SDLog Directory : Oops, could not read the directory name.");

                                }
                            }

                        }
                        else
                        {
                            WriteBytes(new byte[1] { (byte)PacketTypeShimmer2.START_STREAMING_COMMAND }, 0, 1);
                        }

                    }
                    else
                    {
                        String message = "Failed to read configuration information from shimmer. Please ensure correct shimmer is connected";
                        CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_WARNING);
                        OnNewEvent(newEventArgs);
                    }
                }
                else
                {
                    String message = "No serial port is open";
                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_WARNING);
                    OnNewEvent(newEventArgs);
                }
            }
        }


        protected override void InitializeShimmer3SDBT()
        {
            if (SetupDevice == true)
            {
                WriteAccelRange(AccelRange);
                WriteGSRRange(GSRRange);
                WriteGyroRange(GyroRange);
                WriteMagRange(MagGain);
                WriteSamplingRate(SamplingRate);
                WriteInternalExpPower(InternalExpPower);
                WriteEXGConfigurations(Exg1RegArray, Exg2RegArray);
                WriteSensors(SetEnabledSensors); //this should always be the last command
                SetLowPowerAccel(LowPowerAccelEnabled);
                SetLowPowerMag(LowPowerMagEnabled);
                SetLowPowerGyro(LowPowerGyroEnabled);
            }


            //BackgroundWorker worker;
            SetDataReceived(false);

            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_STATUS_COMMAND }, 0, 1);

            waitTilTimeOut();
            /*
            if (CurrentDockStatus)
            {
                // btsd changes 2
                //string status_text = "Reading from SD card...";
                //PControlForm.status_text = "Reading from SD card...";
                //worker.ReportProgress(10, status_text);

                String message = "Reading from SD card...";
                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                OnNewEvent(newEventArgs);
                System.Console.WriteLine(message);
                int read_sd = SdConfigRead();
                // 0    if successful
                // 1    if need InfoMemRead() + SdConfigWrite()
                // -1   if unexpected error happens
                if (read_sd == 1)
                {
                    readInfoMem();
                    SdConfigWrite();
                }
                else if (read_sd == 0)
                {
                    // btsd changes 2
                    //status_text = "Successfully read from SD card.";
                    //PControlForm.status_text = "Successfully read from SD card.";
                    //worker.ReportProgress(80, status_text);
                    message = "Successfully read from SD card.";
                    newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                    System.Console.WriteLine(message);
                    OnNewEvent(newEventArgs);

                    SetState(SHIMMER_STATE_CONNECTED);
                }
                else
                {
                    SetState(SHIMMER_STATE_NONE);
                    return;
                    //PControlForm.status_text = "Failed to read from SD card.";
                    //worker.ReportProgress(80);
                }
            }
            else
            {
                readInfoMem();
            }
            */

            readInfoMem();
            ReadBaudRate();
            ReadPressureCalibrationCoefficients();

            if (CurrentSensingStatus)
            {
                /*
                // btsd changes 2
                //PControlForm.status_text = "Preparing for BTStream...";
                String message = "Preparing for BTStream...";
                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                OnNewEvent(newEventArgs);
                //worker.ReportProgress(90, status_text);
                //PControlForm.buttonStart_Click1();
                //StartStreamingandLog();
                */ 
            }
            else
            {
            }

        }

        public override void StartStreaming()
        {
            base.StartStreaming();
            isLogging = false;
        }

        // btsd changes
        private void readInfoMem()
        {
            //string status_text = "";
            //PChangeStatusLabel("Inquiring Shimmer Info"); // need to be in a UI thread for update
            SetDataReceived(false);

            // btsd changes 2
            string status_text = "Acquiring Accelerometer Range...";
            CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring Accelerometer Range...";
            //worker.ReportProgress(15, status_text);
            ReadAccelRange();

            status_text = "Acquiring ADC Sampling Rate...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring ADC Sampling Rate...";
            //worker.ReportProgress(20, status_text);
            ReadSamplingRate();

            status_text = "Acquiring Magnetometer Range...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring Magnetometer Range...";
            //worker.ReportProgress(25, status_text);
            ReadMagRange();

            status_text = "Acquiring Gyro Range...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring Gyro Range...";
            //worker.ReportProgress(30, status_text);
            ReadGyroRange();

            status_text = "Acquiring Accel Sampling Rate...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring Accel Sampling Rate...";
            //worker.ReportProgress(35, status_text);
            ReadAccelSamplingRate();

            status_text = "Acquiring Calibration settings...";
            //PControlForm.status_text = "Acquiring Calibration settings...";
            //worker.ReportProgress(40, status_text);
            ReadCalibrationParameters("All");

            status_text = "Acquiring EXG1 configure settings...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring EXG1 configure settings...";
            //worker.ReportProgress(45, status_text);
            ReadEXGConfigurations(1);

            status_text = "Acquiring EXG2 configure settings...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring EXG2 configure settings...";
            //worker.ReportProgress(50, status_text);
            ReadEXGConfigurations(2);

            //There is an inquiry below so no need for this
            status_text = "Acquiring Internal Exp Power setting...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            ReadInternalExpPower();

            status_text = "Acquiring trial configure settings...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring trial configure settings...";
            //worker.ReportProgress(55, status_text);
            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_TRIAL_CONFIG_COMMAND }, 0, 1);
            waitTilTimeOut();

            status_text = "Acquiring center name...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring center name...";
            //worker.ReportProgress(60, status_text);
            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_CENTER_COMMAND }, 0, 1);
            waitTilTimeOut();

            status_text = "Acquiring shimmer name...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring shimmer name...";
            //worker.ReportProgress(65, status_text);
            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_SHIMMERNAME_COMMAND }, 0, 1);
            waitTilTimeOut();

            status_text = "Acquiring experiment ID...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring experiment ID...";
            //worker.ReportProgress(70, status_text);
            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_EXPID_COMMAND }, 0, 1);
            waitTilTimeOut();

            status_text = "Acquiring Multi Shimmer settings...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring Multi Shimmer settings...";
            //worker.ReportProgress(75, status_text);
            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_MYID_COMMAND }, 0, 1);
            waitTilTimeOut();

            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_NSHIMMER_COMMAND }, 0, 1);
            waitTilTimeOut();

            status_text = "Acquiring configure time...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring configure time...";
            //worker.ReportProgress(80, status_text);
            WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_CONFIGTIME_COMMAND }, 0, 1);
            waitTilTimeOut();

            status_text = "Acquiring general settings...";
            newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)status_text);
            OnNewEvent(newEventArgs);
            //PControlForm.status_text = "Acquiring general settings...";
            //worker.ReportProgress(85, status_text);
            Inquiry();
        }


        public void SDBT_Get()
        {
            WriteBytes(new byte[1] { (byte)PacketTypeShimmer2.INQUIRY_COMMAND }, 0, 1);
            System.Threading.Thread.Sleep(200);
        }


        public override void WriteTrial()
        {
            byte[] trial_config_byte = BitConverter.GetBytes((Int16)GetTrialConfig());
            byte[] tosend = new byte[4];
            tosend[0] = (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SET_TRIAL_CONFIG_COMMAND;
            tosend[1] = trial_config_byte[0];
            tosend[2] = trial_config_byte[1];
            tosend[3] = (byte)GetInterval();
            WriteBytes(tosend, 0, tosend.Length);
            System.Threading.Thread.Sleep(200);
        }

        public override void WriteCenter()
        {
            byte[] center_byte = System.Text.Encoding.Default.GetBytes(GetCenter());

            byte[] tosend = new byte[center_byte.Length + 2];
            tosend[0] = (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SET_CENTER_COMMAND;
            tosend[1] = (byte)center_byte.Length;
            System.Buffer.BlockCopy(center_byte, 0, tosend, 2, center_byte.Length);
            WriteBytes(tosend, 0, tosend.Length);
            System.Threading.Thread.Sleep(200);
        }

        public override void WriteShimmerName()
        {
            byte[] shimmername_byte = System.Text.Encoding.Default.GetBytes(GetShimmerName());

            byte[] tosend = new byte[shimmername_byte.Length + 2];
            tosend[0] = (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SET_SHIMMERNAME_COMMAND;
            tosend[1] = (byte)shimmername_byte.Length;
            System.Buffer.BlockCopy(shimmername_byte, 0, tosend, 2, shimmername_byte.Length);
            WriteBytes(tosend, 0, tosend.Length);
            System.Threading.Thread.Sleep(200);
        }

        public override void WriteExpID()
        {
            byte[] expid_byte = System.Text.Encoding.Default.GetBytes(GetExperimentID());

            byte[] tosend = new byte[expid_byte.Length + 2];
            tosend[0] = (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SET_EXPID_COMMAND;
            tosend[1] = (byte)expid_byte.Length;
            System.Buffer.BlockCopy(expid_byte, 0, tosend, 2, expid_byte.Length);
            WriteBytes(tosend, 0, tosend.Length);
            System.Threading.Thread.Sleep(200);
        }

        public override void WriteNshimmer()
        {
            byte[] tosend = new byte[] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SET_NSHIMMER_COMMAND, (byte)GetNshimmer() };
            WriteBytes(tosend, 0, 2);
            System.Threading.Thread.Sleep(200);
        }

        public override void WriteMyID()
        {
            byte[] tosend = new byte[] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SET_MYID_COMMAND, (byte)GetMyID() };
            WriteBytes(tosend, 0, 2);
            System.Threading.Thread.Sleep(200);
        }

        public override void WriteConfigTime()
        {
            byte[] configtime_byte = System.Text.Encoding.Default.GetBytes(Convert.ToString(GetConfigTime()));

            byte[] tosend = new byte[configtime_byte.Length + 2];
            tosend[0] = (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SET_CONFIGTIME_COMMAND;
            tosend[1] = (byte)configtime_byte.Length;
            System.Buffer.BlockCopy(configtime_byte, 0, tosend, 2, configtime_byte.Length);
            WriteBytes(tosend, 0, tosend.Length);
            System.Threading.Thread.Sleep(200);
        }

        public override void WriteSdConfigFile()
        {
            // btsd changes
            if (GetFirmwareIdentifier() == 3)
            {
                WriteBytes(new byte[1] { (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.GET_STATUS_COMMAND }, 0, 1);
                //System.Threading.Thread.Sleep(200);

                waitTilTimeOut();
                if (CurrentDockStatus)
                {
                    //SdConfigWrite();
                }
            }
        }



        // btsd changes
        private void waitTilTimeOut()
        {
            int timeout = 150;
            while (timeout > 0)
            {
                if (GetDataReceived())
                {
                    SetDataReceived(false);
                    break;
                }
                System.Threading.Thread.Sleep(10);
                timeout--;
            }
            SetDataReceived(false);
        }
        // btsd changes

        public override void SetDrivePath(String path)
        {
            TempDrivePath = path;
        }




        public void SdConfigWrite()
        {

            string drivePath = "";
            string filePath = "";
            DriveInfo[] drives = DriveInfo.GetDrives();
            int i = 0, sdcardSeq = 0, totalRemovable = 0;

            foreach (DriveInfo drive in drives)
            {
                bool dready = false;
                DriveType dtype;
                dready = drive.IsReady;
                dtype = drive.DriveType;

                if (dready && (dtype == (DriveType)2))
                {
                    sdcardSeq = i;
                    totalRemovable++;
                }
                i++;
            }

            if (totalRemovable == 1)
            {
                drivePath = drives[sdcardSeq].Name;
            }
            else if (totalRemovable > 1)
            {
                String message = totalRemovable + "Two or more removable devices are detected. Please choose the intended Shimmer.";
                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_EXTRA_REMOVABLE_DEVICES_DETECTED);
                OnNewEvent(newEventArgs);
                drivePath = TempDrivePath;
            }
            else
            {
                String message = "No removable storage device is found.";
                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_ERROR);
                OnNewEvent(newEventArgs);
                return;
            }
            filePath = drivePath + "sdlog.cfg";
            // now file path is found. Read from existing sdlog.cfg file.
            System.IO.StreamWriter file = null;

            file = new System.IO.StreamWriter(filePath, false);


            file.WriteLine("accel=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_A_ACCEL) == 0 ? 0 : 1));//SensorBitmapShimmer3.SensorAAccel
            file.WriteLine("gyro=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_MPU9150_GYRO) == 0 ? 0 : 1));
            file.WriteLine("mag=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_LSM303DLHC_MAG) == 0 ? 0 : 1));
            file.WriteLine("exg1_24bit=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_EXG1_24BIT) == 0 ? 0 : 1));
            file.WriteLine("exg2_24bit=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_EXG2_24BIT) == 0 ? 0 : 1));
            file.WriteLine("gsr=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_GSR) == 0 ? 0 : 1));
            file.WriteLine("extch7=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_EXT_A7) == 0 ? 0 : 1));
            file.WriteLine("extch6=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_EXT_A6) == 0 ? 0 : 1));
            file.WriteLine("br_amp=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_BRIDGE_AMP) == 0 ? 0 : 1));
            file.WriteLine("vbat=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_VBATT) == 0 ? 0 : 1));
            file.WriteLine("accel_d=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_D_ACCEL) == 0 ? 0 : 1));
            file.WriteLine("extch15=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_EXT_A15) == 0 ? 0 : 1));
            file.WriteLine("intch1=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_INT_A1) == 0 ? 0 : 1));
            file.WriteLine("intch12=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_INT_A12) == 0 ? 0 : 1));
            file.WriteLine("intch13=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_INT_A13) == 0 ? 0 : 1));
            file.WriteLine("intch14=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_INT_A14) == 0 ? 0 : 1));
            file.WriteLine("accel_mpu=" + ((GetEnabledSensors() & (int)ShimmerSDBT.SensorBitmapShimmer3_unused.SensorMpuAccel) == 0 ? 0 : 1));
            file.WriteLine("mag_mpu=" + ((GetEnabledSensors() & (int)ShimmerSDBT.SensorBitmapShimmer3_unused.SensorMpuMag) == 0 ? 0 : 1));
            file.WriteLine("exg1_16bit=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_EXG1_16BIT) == 0 ? 0 : 1));
            file.WriteLine("exg2_16bit=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_EXG2_16BIT) == 0 ? 0 : 1));
            file.WriteLine("pres_bmp180=" + ((GetEnabledSensors() & (int)SensorBitmapShimmer3.SENSOR_BMP180_PRESSURE) == 0 ? 0 : 1));
            double file_sample_rate = (GetSamplingRate());
            file.WriteLine("sample_rate=" + string.Format("{0:N2}", file_sample_rate));
            file.WriteLine("mg_internal_rate=" + GetMagSamplingRate());
            file.WriteLine("mg_range=" + GetMagRange());
            file.WriteLine("acc_internal_rate=" + GetAccelSamplingRate());
            file.WriteLine("accel_mpu_range=" + GetMpu9150AccelRange());
            file.WriteLine("pres_bmp180_prec=" + GetPressureResolution());
            file.WriteLine("gs_range=" + GetGSRRange());
            file.WriteLine("exp_power=" + (GetExpPower() ? 1 : 0));
            file.WriteLine("gyro_range=" + GetGyroRange());
            file.WriteLine("gyro_samplingrate=" + GetMpu9150SamplingRate());
            file.WriteLine("acc_range=" + GetAccelRange());
            file.WriteLine("acc_lpm=" + GetAccelLPBit());
            file.WriteLine("acc_hrm=" + GetAccelHRBit());
            file.WriteLine("user_button_enable=" + (GetUserButton() ? 1 : 0));
            file.WriteLine("iammaster=" + (GetIAmMaster() ? 1 : 0));
            file.WriteLine("sync=" + (GetSync() ? 1 : 0));
            file.WriteLine("singletouch=" + (GetSingleTouch() ? 1 : 0));
            file.WriteLine("tcxo=" + (GetTcxo() ? 1 : 0));
            file.WriteLine("interval=" + GetInterval());
            file.WriteLine("center=" + GetCenter());
            file.WriteLine("myid=" + GetMyID());
            file.WriteLine("Nshimmer=" + GetNshimmer());
            file.WriteLine("shimmername=" + GetShimmerName());
            file.WriteLine("experimentid=" + GetExperimentID());
            file.WriteLine("configtime=" + GetConfigTime());
            file.WriteLine("EXG_ADS1292R_1_CONFIG1=" + Exg1RegArray[0].ToString());
            file.WriteLine("EXG_ADS1292R_1_CONFIG2=" + Exg1RegArray[1].ToString());
            file.WriteLine("EXG_ADS1292R_1_LOFF=" + Exg1RegArray[2].ToString());
            file.WriteLine("EXG_ADS1292R_1_CH1SET=" + Exg1RegArray[3].ToString());
            file.WriteLine("EXG_ADS1292R_1_CH2SET=" + Exg1RegArray[4].ToString());
            file.WriteLine("EXG_ADS1292R_1_RLD_SENS=" + Exg1RegArray[5].ToString());
            file.WriteLine("EXG_ADS1292R_1_LOFF_SENS=" + Exg1RegArray[6].ToString());
            file.WriteLine("EXG_ADS1292R_1_LOFF_STAT=" + Exg1RegArray[7].ToString());
            file.WriteLine("EXG_ADS1292R_1_RESP1=" + Exg1RegArray[8].ToString());
            file.WriteLine("EXG_ADS1292R_1_RESP2=" + Exg1RegArray[9].ToString());
            file.WriteLine("EXG_ADS1292R_2_CONFIG1=" + Exg2RegArray[0].ToString());
            file.WriteLine("EXG_ADS1292R_2_CONFIG2=" + Exg2RegArray[1].ToString());
            file.WriteLine("EXG_ADS1292R_2_LOFF=" + Exg2RegArray[2].ToString());
            file.WriteLine("EXG_ADS1292R_2_CH1SET=" + Exg2RegArray[3].ToString());
            file.WriteLine("EXG_ADS1292R_2_CH2SET=" + Exg2RegArray[4].ToString());
            file.WriteLine("EXG_ADS1292R_2_RLD_SENS=" + Exg2RegArray[5].ToString());
            file.WriteLine("EXG_ADS1292R_2_LOFF_SENS=" + Exg2RegArray[6].ToString());
            file.WriteLine("EXG_ADS1292R_2_LOFF_STAT=" + Exg2RegArray[7].ToString());
            file.WriteLine("EXG_ADS1292R_2_RESP1=" + Exg2RegArray[8].ToString());
            file.WriteLine("EXG_ADS1292R_2_RESP2=" + Exg2RegArray[9].ToString());

            file.Close();
            return;
        }


        // returns 
        // 0    if successful
        // 1    if need InfoMemRead() + SdConfigWrite()
        // -1   if unexpected error happens
        private int SdConfigRead()
        {

            string drivePath = "";
            string filePath = "";
            DriveInfo[] drives = DriveInfo.GetDrives();
            int i = 0, sdcardSeq = 0, totalRemovable = 0;

            foreach (DriveInfo drive in drives)
            {
                bool dready = false;
                DriveType dtype;
                dready = drive.IsReady;
                dtype = drive.DriveType;

                if (dready && (dtype == (DriveType)2))
                {
                    sdcardSeq = i;
                    totalRemovable++;
                }
                i++;
            }

            if (totalRemovable == 1)
            {
                drivePath = drives[sdcardSeq].Name;
            }
            else if (totalRemovable > 1)
            {
                //System.Windows.Forms.
                //Important Change Needed
                //MessageBox.Show(totalRemovable + " removable devices are detected. Please choose the intended Shimmer.", "Message");
                //FolderBrowserDialog fbd = new FolderBrowserDialog();
                //DialogResult result = fbd.ShowDialog();

                //Users should always ensure TempDrivePath is taken care of on the outer class/ui which is called through the method OnMyPropertyChanged
                String message = totalRemovable + "Two or more removable devices are detected. Please choose the intended Shimmer.";
                CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_EXTRA_REMOVABLE_DEVICES_DETECTED);
                OnNewEvent(newEventArgs);
                drivePath = TempDrivePath;
            }
            else
            {
                //MessageBox.Show("No SD card was found.", Shimmer.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            filePath = drivePath + "sdlog.cfg";

            // now file path is found. Read from existing sdlog.cfg file.
            System.IO.StreamReader file = null;
            int counter = 0;
            string line;

            if (!File.Exists(filePath))
            {
                //MessageBox.Show("Configure file not found. Please connect the intended Shimmer through BlueTooth before further operation.", Shimmer.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;//file not exist, need to read InfoMem
            }
            else//file exists, read here and update app's stored vars
            {
                file = new System.IO.StreamReader(filePath);
                int equals = 0;
                double freq = 0.0;
                byte[] file_sample_rate = new byte[2] { 0, 0 };
                byte[] file_sensors = new byte[3] { 0, 0, 0 };
                byte[] file_trialConfig = new byte[3] { 0, 0, 0 };
                byte[] file_configSetup = new byte[4] { 0, 0, 0, 0 };
                bool centername_set = false;
                SetClockTCXO(0);

                while ((line = file.ReadLine()) != null)
                {
                    counter++;
                    if ((equals = line.IndexOf('=')) == -1)
                        continue;
                    equals++;

                    if (line.Contains("accel="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL;//Shimmer3.SensorBitmap.SensorAAccel;
                    }
                    else if (line.Contains("gyro="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_MPU9150_GYRO;
                    }
                    else if (line.Contains("mag="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_LSM303DLHC_MAG;
                    }
                    else if (line.Contains("exg1_24bit="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG1_24BIT;
                    }
                    else if (line.Contains("exg2_24bit="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG2_24BIT;
                    }
                    else if (line.Contains("gsr="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR;
                    }
                    else if (line.Contains("extch7="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXT_A7;
                    }
                    else if (line.Contains("extch6="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXT_A6;
                    }
                    else if (line.Contains("vbat="))
                    {
                        if (line[equals] == '1')
                            file_sensors[1] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_VBATT >> 8) & 0xff);
                    }
                    else if (line.Contains("accel_d="))
                    {
                        if (line[equals] == '1')
                            file_sensors[1] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_D_ACCEL >> 8) & 0xff);
                    }
                    else if (line.Contains("extch15="))
                    {
                        if (line[equals] == '1')
                            file_sensors[1] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXT_A15 >> 8) & 0xff);
                    }
                    else if (line.Contains("intch1="))
                    {
                        if (line[equals] == '1')
                            file_sensors[1] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A1 >> 8) & 0xff);
                    }
                    else if (line.Contains("intch12="))
                    {
                        if (line[equals] == '1')
                            file_sensors[1] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A12 >> 8) & 0xff);
                    }
                    else if (line.Contains("intch13="))
                    {
                        if (line[equals] == '1')
                            file_sensors[1] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13 >> 8) & 0xff);
                    }
                    else if (line.Contains("intch14="))
                    {
                        if (line[equals] == '1')
                            file_sensors[2] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A14 >> 16) & 0xff);
                    }
                    else if (line.Contains("accel_mpu="))
                    {
                        if (line[equals] == '1')
                            file_sensors[2] |= (byte)(((int)ShimmerSDBT.SensorBitmapShimmer3_unused.SensorMpuAccel >> 16) & 0xff);//sdbt only
                    }
                    else if (line.Contains("mag_mpu="))
                    {
                        if (line[equals] == '1')
                            file_sensors[2] |= (byte)(((int)ShimmerSDBT.SensorBitmapShimmer3_unused.SensorMpuMag >> 16) & 0xff);//sdbt only
                    }
                    else if (line.Contains("exg1_16bit="))
                    {
                        if (line[equals] == '1')
                            file_sensors[2] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG1_16BIT >> 16) & 0xff);
                    }
                    else if (line.Contains("exg2_16bit="))
                    {
                        if (line[equals] == '1')
                            file_sensors[2] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG2_16BIT >> 16) & 0xff);
                    }
                    else if (line.Contains("pres_bmp180="))
                    {
                        if (line[equals] == '1')
                            file_sensors[2] |= (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_BMP180_PRESSURE >> 16) & 0xff);
                    }
                    else if (line.Contains("mag="))
                    {
                        if (line[equals] == '1')
                            file_sensors[0] |= (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_LSM303DLHC_MAG;
                    }
                    else if (line.Contains("sample_rate="))
                        freq = Convert.ToDouble(line.Substring(equals, line.Length - equals));
                    else if (line.Contains("mg_internal_rate="))
                        file_configSetup[2] |= (byte)((Convert.ToInt16(line[equals]) & 0x07) << 2);
                    else if (line.Contains("mg_range="))
                        file_configSetup[2] |= (byte)((Convert.ToInt16(line[equals]) & 0x07) << 5);
                    else if (line.Contains("acc_internal_rate="))
                        file_configSetup[0] |= (byte)((Convert.ToInt16(line[equals]) & 0x0f) << 4);
                    else if (line.Contains("accel_mpu_range="))
                        file_configSetup[3] |= (byte)((Convert.ToInt16(line[equals]) & 0x03) << 6);
                    else if (line.Contains("acc_range="))
                        file_configSetup[0] |= (byte)((Convert.ToInt16(line[equals]) & 0x03) << 2);
                    else if (line.Contains("acc_lpm="))
                        file_configSetup[0] |= (byte)((Convert.ToInt16(line[equals]) & 0x01) << 1);
                    else if (line.Contains("acc_hrm="))
                        file_configSetup[0] |= (byte)((Convert.ToInt16(line[equals]) & 0x01));
                    else if (line.Contains("gs_range="))
                        file_configSetup[3] |= (byte)((Convert.ToInt16(line[equals]) & 0x07) << 1);
                    else if (line.Contains("gyro_samplingrate="))
                        file_configSetup[1] |= (byte)(Convert.ToInt16(line[equals]));
                    else if (line.Contains("gyro_range="))
                        file_configSetup[2] |= (byte)((Convert.ToInt16(line[equals]) & 0x03));
                    else if (line.Contains("pres_bmp180_prec="))
                        file_configSetup[3] |= (byte)((Convert.ToInt16(line[equals]) & 0x03) << 4);
                    else if (line.Contains("user_button_enable="))
                    {
                        if (line[equals] == '1')
                            file_trialConfig[0] |= (byte)ShimmerSDBT.TrialConfigBitmap.UserButton;
                    }
                    else if (line.Contains("iammaster="))
                    {
                        if (line[equals] == '1')
                            file_trialConfig[0] |= (byte)ShimmerSDBT.TrialConfigBitmap.IAmMaster;
                    }
                    else if (line.Contains("sync="))
                    {
                        if (line[equals] == '1')
                            file_trialConfig[0] |= (byte)ShimmerSDBT.TrialConfigBitmap.Sync;
                    }
                    else if (line.Contains("singletouch="))
                    {
                        if (line[equals] == '1')
                            file_trialConfig[1] |= (byte)ShimmerSDBT.TrialConfigBitmap.SingleTouch;
                    }
                    else if (line.Contains("exp_power="))
                    {
                        //if (line.Contains("True"))
                        if (line[equals] == '1')
                            file_trialConfig[1] |= (byte)ShimmerSDBT.TrialConfigBitmap.ExpPower;
                    }
                    else if (line.Contains("tcxo="))
                    {
                        if (line[equals] == '1')
                        {
                            file_trialConfig[1] |= (byte)ShimmerSDBT.TrialConfigBitmap.TCXO;
                            SetClockTCXO(1);
                        }
                    }
                    else if (line.Contains("user_button_enable="))
                    {
                        if (line[equals] == '1')
                            file_trialConfig[0] |= (byte)ShimmerSDBT.TrialConfigBitmap.UserButton;
                    }
                    else if (line.Contains("interval="))
                    {
                        try
                        {
                            SetInterval(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                        }
                        catch (FormatException)
                        {
                            SetInterval(0);
                        }
                    }
                    else if (line.Contains("center="))
                    {
                        SetCenter(line.Substring(equals, line.Length - equals));
                        centername_set = true;
                    }
                    else if (line.Contains("myid="))
                    {
                        try
                        {
                            SetMyID(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                        }
                        catch (FormatException)
                        {
                            SetMyID(0);
                        }
                    }
                    else if (line.Contains("Nshimmer="))
                    {
                        try
                        {
                            SetNshimmer(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                        }
                        catch (FormatException)
                        {
                            SetNshimmer(0);
                        }
                    }
                    else if (line.Contains("shimmername="))
                        SetShimmerName(line.Substring(equals, line.Length - equals));
                    else if (line.Contains("experimentid="))
                        SetExperimentID(line.Substring(equals, line.Length - equals));
                    else if (line.Contains("configtime="))
                    {
                        try
                        {
                            SetConfigTime(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                        }
                        catch (FormatException)
                        {
                            SetConfigTime(0);
                        }
                    }
                    else if (line.Contains("EXG_ADS1292R_1_CONFIG1="))
                        Exg1RegArray[0] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_CONFIG2="))
                        Exg1RegArray[1] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_LOFF="))
                        Exg1RegArray[2] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_CH1SET="))
                        Exg1RegArray[3] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_CH2SET="))
                        Exg1RegArray[4] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_RLD_SENS="))
                        Exg1RegArray[5] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_LOFF_SENS="))
                        Exg1RegArray[6] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_LOFF_STAT="))
                        Exg1RegArray[7] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_RESP1="))
                        Exg1RegArray[8] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_1_RESP2="))
                        Exg1RegArray[9] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_CONFIG1="))
                        Exg2RegArray[0] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_CONFIG2="))
                        Exg2RegArray[1] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_LOFF="))
                        Exg2RegArray[2] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_CH1SET="))
                        Exg2RegArray[3] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_CH2SET="))
                        Exg2RegArray[4] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_RLD_SENS="))
                        Exg2RegArray[5] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_LOFF_SENS="))
                        Exg2RegArray[6] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_LOFF_STAT="))
                        Exg2RegArray[7] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_RESP1="))
                        Exg2RegArray[8] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                    else if (line.Contains("EXG_ADS1292R_2_RESP2="))
                        Exg2RegArray[9] = (byte)(Convert.ToInt32(line.Substring(equals, line.Length - equals)));
                }

                file.Close();

                int a = (int)(32768 / freq);
                file_sample_rate[0] = (byte)(a % 256);
                file_sample_rate[1] = (byte)(a / 256);

                // they are sharing adc13,14, so ban them when strain is on
                if ((byte)((byte)file_sensors[1] & (byte)(((int)Shimmer.SensorBitmapShimmer3.SENSOR_BRIDGE_AMP >> 8) & 0xff)) != 0)
                {
                    file_sensors[1] &= (byte)(0xff - (byte)(((int)Shimmer.SensorBitmapShimmer3.SENSOR_INT_A13 >> 8) & 0xff));
                    file_sensors[2] &= (byte)(0xff - (byte)(((int)Shimmer.SensorBitmapShimmer3.SENSOR_INT_A14 >> 16) & 0xff));
                }

                // they are sharing adc1, so ban intch1 when gsr is on
                if ((byte)((byte)file_sensors[0] & (byte)Shimmer.SensorBitmapShimmer3.SENSOR_GSR) != 0)
                    file_sensors[1] &= (byte)(0xff - (byte)(((int)Shimmer.SensorBitmapShimmer3.SENSOR_INT_A1 >> 8) & 0xff));

                if (GetInterval() < 54)
                {
                    SetInterval(54);
                }
                if ((byte)((byte)file_trialConfig[1] & (byte)ShimmerSDBT.TrialConfigBitmap.SingleTouch) != 0)
                {
                    file_trialConfig[0] |= (byte)ShimmerSDBT.TrialConfigBitmap.UserButton;
                    file_trialConfig[0] |= (byte)ShimmerSDBT.TrialConfigBitmap.Sync;
                }

                if (!centername_set) // if no center is appointed, let this guy be the center
                    SetCenter("00066643b48e");

                //debug

                List<byte> buffer_channelContents = new List<byte>();
                int nbrAdcChans = 0, nbrDigiChans = 0;

                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.XLNAccel);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.YLNAccel);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.ZLNAccel);
                    nbrAdcChans += 3;
                }
                if ((byte)((byte)file_sensors[1] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_VBATT >> 8) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.VBatt);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXT_A7) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.ExternalAdc7);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXT_A6) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.ExternalAdc6);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[1] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXT_A15 >> 8) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.ExternalAdc15);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[1] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A12 >> 8) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.InternalAdc12);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[1] & (byte)(((int)Shimmer.SensorBitmapShimmer3.SENSOR_BRIDGE_AMP >> 8) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.STRAIN_HIGH);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.STRAIN_LOW);
                    nbrAdcChans += 2;
                }
                if ((byte)((byte)file_sensors[1] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13 >> 8) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.InternalAdc13);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[2] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A14 >> 16) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.InternalAdc14);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.GsrRaw);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[1] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A1 >> 8) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.InternalAdc1);
                    nbrAdcChans++;
                }
                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_MPU9150_GYRO) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.XGyro);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.YGyro);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.ZGyro);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[1] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_D_ACCEL >> 8) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.XWRAccel);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.YWRAccel);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.ZWRAccel);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_LSM303DLHC_MAG) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.XMag);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.YMag);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.ZMag);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[2] & (byte)(((int)ShimmerSDBT.SensorBitmapShimmer3_unused.SensorMpuAccel >> 16) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.AlternativeXAccel);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.AlternativeYAccel);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.AlternativeZAccel);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[2] & (byte)(((int)ShimmerSDBT.SensorBitmapShimmer3_unused.SensorMpuMag >> 16) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.AlternativeXMag);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.AlternativeYMag);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.AlternativeZMag);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[2] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_BMP180_PRESSURE >> 16) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Temperature);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Pressure);
                    nbrDigiChans += 2;
                }
                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG1_24BIT) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg1_Status);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg1_CH1);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg1_CH2);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[2] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG1_16BIT >> 16) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg1_Status);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg1_CH1_16Bit);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg1_CH2_16Bit);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[0] & (byte)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG2_24BIT) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg2_Status);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg2_CH1);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg2_CH2);
                    nbrDigiChans += 3;
                }
                if ((byte)((byte)file_sensors[2] & (byte)(((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG2_16BIT >> 16) & 0xff)) != 0)
                {
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg2_Status);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg2_CH1_16Bit);
                    buffer_channelContents.Add((byte)ShimmerSDBT.ChannelContents.Exg2_CH2_16Bit);
                    nbrDigiChans += 3;
                }

                // get Sampling rate(0-1), config setup byte0(2-5), num chans(6) and buffer size(7)
                List<byte> buffer_inq = new List<byte>();
                // get trial_config(0-1), interval(2)
                List<byte> buffer_trial = new List<byte>();

                buffer_inq.Add((byte)file_sample_rate[0]);
                buffer_inq.Add((byte)file_sample_rate[1]);
                buffer_inq.Add((byte)file_configSetup[0]);
                buffer_inq.Add((byte)file_configSetup[1]);
                buffer_inq.Add((byte)file_configSetup[2]);
                buffer_inq.Add((byte)file_configSetup[3]);
                buffer_inq.Add((byte)(nbrAdcChans + nbrDigiChans));
                buffer_inq.Add((byte)1);
                for (int channelx = 0; channelx < nbrAdcChans + nbrDigiChans; channelx++)
                {
                    buffer_inq.Add(buffer_channelContents[channelx]);
                }
                InterpretInquiryResponseShimmer3(buffer_inq);

                //byte[] file_sensors = new byte[3] { 0, 0, 0 };

                buffer_trial.Add((byte)file_trialConfig[0]);
                buffer_trial.Add((byte)file_trialConfig[1]);
                buffer_trial.Add((byte)GetInterval());
                fillTrialShimmer3(buffer_trial);
            }
            return 0;
        }
        public override void SDBT_switch(byte b)
        {
            List<byte> buffer = new List<byte>();
            int i;
            if (GetState() == ShimmerBluetooth.SHIMMER_STATE_STREAMING)
            {
                switch (b)
                {
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.INSTREAM_CMD_RESPONSE: int inStreamCMD = ReadByte();
                        if (inStreamCMD == (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.STATUS_RESPONSE)
                        {
                            //STATUS: 0|0|0|0|0|SELFCMD|SENSING|DOCKED
                            int bufferint = ReadByte();
                            bool docked = (bufferint & 0x01) == 1;
                            bool sensing = ((bufferint >> 1) & 0x01) == 1;
                            bool selfcmd = ((bufferint >> 2) & 0x01) == 1;
                            if (CurrentDockStatus != docked)
                            {
                                CurrentDockStatus = docked;
                                if (docked && isLogging)
                                {
                                    String message = "Shimmer docking detected.\nStop writing to SD card.";
                                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_WARNING);
                                    OnNewEvent(newEventArgs);

                                    /*if (GetSdDir() != "")
                                        this.Invoke(new Action(() => { PChangeStatusLabel("BT Streaming. Last SDLog Directory : " + pProfile.GetSdDir()); }));
                                    else
                                        this.Invoke(new Action(() => { PChangeStatusLabel("BT Streaming."); }));*/
                                    isLogging = false;
                                }
                            }
                            if (CurrentSensingStatus != sensing)
                            {
                                CurrentSensingStatus = sensing;
                            }
                            if (selfcmd) //fw does this, and sw is forced to do the same
                            {
                                if (CurrentSensingStatus && (GetState() != ShimmerBluetooth.SHIMMER_STATE_STREAMING))// to start sensing
                                {
                                    // btsd changes 2
                                    //PControlForm.buttonStart_Click1(); 
                                    //System.Console.WriteLine("Shimmer Self CMD");
                                    //StartStreamingandLog();
                                }
                                else if (!CurrentSensingStatus && (GetState() == ShimmerBluetooth.SHIMMER_STATE_STREAMING))// to stop sensing
                                {
                                    if (isLogging && (GetSdDir() != ""))
                                    {
                                        // btsd changes 2
                                        String message = "Stopped. Last SDLog Directory : " + GetSdDir();
                                        CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                        OnNewEvent(newEventArgs);


                                    }
                                    else
                                    {

                                        String message = "Stopped.";
                                        CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                        OnNewEvent(newEventArgs);
                                        //PControlForm.ChangeStatusLabel("Stopped.");
                                        //this.Invoke(new Action(() => { PChangeStatusLabel("Stopped."); }));
                                    }
                                    SetSdDir("");
                                    if (IsConnectionOpen())
                                    {
                                        SetState(SHIMMER_STATE_CONNECTED);// = false;
                                        isLogging = false;
                                        //PChangeStatusLabel("Stopped");
                                        /*
                                        while (SerialPort.BytesToRead != 0)
                                        {
                                            ReadByte();
                                        }*/
                                        FlushInputConnection();
                                    }
                                }
                            }

                            SetDataReceived(true);
                        }
                        else if (inStreamCMD == (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.DIR_RESPONSE)
                        {
                            buffer.Clear();
                            int len = ReadByte();
                            for (i = 0; i < len; i++)
                            {
                                buffer.Add((byte)ReadByte());
                            }
                            //buffer.Add((byte)'\0');
                            SetSdDir(System.Text.Encoding.Default.GetString(buffer.ToArray()));
                            if (GetSdDir().Contains("data") &&
                                GetSdDir().Contains(GetShimmerName()) &&
                                GetSdDir().Contains(GetExperimentID()) &&
                                GetSdDir().Contains(GetConfigTime().ToString()))
                            {
                            }
                            else
                            {
                                SetSdDir("");
                            }

                            if (isLogging)
                            {
                                if (GetSdDir() != "")
                                {
                                    String message = "BTStream + SDLog, Current SDLog Directory : " + GetSdDir();
                                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                    OnNewEvent(newEventArgs);
                                }
                                else
                                {
                                    String message = "BTStream + SDLog, Current SDLog Directory : Oops, could not read the directory name. Pos2";
                                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                    OnNewEvent(newEventArgs);
                                }
                            }
                            SetDataReceived(true);
                        }
                        buffer.Clear();
                        SetDataReceived(true);
                        break;

                    default: break;
                }
            }
            else
            {
                switch (b)
                {
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.TRIAL_CONFIG_RESPONSE:
                        for (i = 0; i < 3; i++)
                        {
                            // get 2 bytes trial config and 1 byte interval
                            buffer.Add((byte)ReadByte());
                        }
                        fillTrialShimmer3(buffer);
                        buffer.Clear();
                        SetDataReceived(true);
                        break;
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.CENTER_RESPONSE:
                        {
                            int len = ReadByte();
                            for (i = 0; i < len; i++)
                            {
                                buffer.Add((byte)ReadByte());
                            }
                            //buffer.Add((byte)'\0');
                            SetCenter(System.Text.Encoding.Default.GetString(buffer.ToArray()));
                            buffer.Clear();
                        }
                        SetDataReceived(true);
                        break;
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.SHIMMERNAME_RESPONSE:
                        {
                            int len = ReadByte();
                            for (i = 0; i < len; i++)
                            {
                                buffer.Add((byte)ReadByte());
                            }
                            //buffer.Add((byte)'\0');
                            SetShimmerName(System.Text.Encoding.Default.GetString(buffer.ToArray()));
                            buffer.Clear();
                        }
                        SetDataReceived(true);
                        break;
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.EXPID_RESPONSE:
                        {
                            int len = ReadByte();
                            for (i = 0; i < len; i++)
                            {
                                buffer.Add((byte)ReadByte());
                            }
                            //buffer.Add((byte)'\0');
                            SetExperimentID(System.Text.Encoding.Default.GetString(buffer.ToArray()));
                            buffer.Clear();
                        }
                        SetDataReceived(true);
                        break;
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.CONFIGTIME_RESPONSE:
                        {
                            int len = ReadByte();
                            for (i = 0; i < len; i++)
                            {
                                buffer.Add((byte)ReadByte());
                            }
                            //buffer.Add((byte)'\0');
                            try
                            {
                                SetConfigTime(Convert.ToInt32(System.Text.Encoding.Default.GetString(buffer.ToArray())));
                            }
                            catch (FormatException)
                            {
                                SetConfigTime(0);
                            }
                            buffer.Clear();
                        }
                        SetDataReceived(true);
                        break;
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.MYID_RESPONSE:
                        SetMyID(ReadByte());
                        SetDataReceived(true);
                        break;
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.NSHIMMER_RESPONSE:
                        SetNshimmer(ReadByte());
                        SetDataReceived(true);
                        break;
                    case (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.INSTREAM_CMD_RESPONSE: int inStreamCMD = ReadByte();
                        if (inStreamCMD == (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.STATUS_RESPONSE)
                        {
                            //STATUS: 0|0|0|0|0|SELFCMD|SENSING|DOCKED
                            int bufferint = ReadByte();
                            bool docked = (bufferint & 0x01) == 1;
                            bool sensing = ((bufferint >> 1) & 0x01) == 1;
                            bool selfcmd = ((bufferint >> 2) & 0x01) == 1;
                            if (CurrentDockStatus != docked)
                            {
                                CurrentDockStatus = docked;
                                if (docked && isLogging)
                                {

                                    String message = "Shimmer docking detected.\nStop writing to SD card.";
                                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message, (int)ShimmerSDBT.ShimmerSDBTMinorIdentifier.MSG_WARNING);
                                    OnNewEvent(newEventArgs);
                                }
                            }
                            if (CurrentSensingStatus != sensing)
                            {
                                CurrentSensingStatus = sensing;
                            }
                            if (selfcmd) //fw does this, and sw is forced to do the same
                            {
                                if (CurrentSensingStatus && (GetState() != ShimmerBluetooth.SHIMMER_STATE_STREAMING))// to start sensing
                                {
                                    //System.Console.WriteLine("S Stream 2");
                                    //StartStreamingandLog();
                                }
                                else if (!CurrentSensingStatus && (GetState() == ShimmerBluetooth.SHIMMER_STATE_STREAMING))// to stop sensing
                                {
                                    if (isLogging && (GetSdDir() != ""))
                                    {

                                        String message = "Stopped. Last SDLog Directory : " + GetSdDir();
                                        CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);

                                        OnNewEvent(newEventArgs);

                                    }
                                    else
                                    {
                                        String message = "Stopped.";
                                        CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                        OnNewEvent(newEventArgs);

                                    }
                                    SetSdDir("");
                                    if (IsConnectionOpen())
                                    {
                                        SetState(SHIMMER_STATE_CONNECTED);// = false;
                                        isLogging = false;
                                        //PChangeStatusLabel("Stopped");
                                        /*while (SerialPort.BytesToRead != 0)
                                        {
                                            ReadByte();
                                        }*/
                                        FlushInputConnection();
                                    }
                                    // todo: check button status

                                }
                            }

                            SetDataReceived(true);
                        }
                        else if (inStreamCMD == (byte)ShimmerSDBT.PacketTypeShimmer3SDBT.DIR_RESPONSE)
                        {
                            buffer.Clear();
                            int len = ReadByte();
                            for (i = 0; i < len; i++)
                            {
                                buffer.Add((byte)ReadByte());
                            }
                            //buffer.Add((byte)'\0');
                            SetSdDir(System.Text.Encoding.Default.GetString(buffer.ToArray()));
                            if (GetSdDir().Contains("data") &&
                                GetSdDir().Contains(GetShimmerName()) &&
                                GetSdDir().Contains(GetExperimentID()) &&
                                GetSdDir().Contains(GetConfigTime().ToString()))
                            {
                            }
                            else
                            {
                                SetSdDir("");
                            }

                            if (isLogging)
                            {
                                if (GetSdDir() != "")
                                {
                                    String message = "BTStream + SDLog, Current SDLog Directory : " + GetSdDir();
                                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                    OnNewEvent(newEventArgs);

                                }
                                else
                                {
                                    String message = "BTStream + SDLog, Current SDLog Directory : Oops, could not read the directory name. pos 3";
                                    CustomEventArgs newEventArgs = new CustomEventArgs((int)ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE, (object)message);
                                    OnNewEvent(newEventArgs);
                                }
                            }
                            SetDataReceived(true);
                        }
                        buffer.Clear();
                        SetDataReceived(true);
                        break;

                    default: break;

                }
            }
        }

        public override void SetShimmerAddress(String address)
        {
            ComPort = address;
        }
    }
}
