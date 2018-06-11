﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Data format to decode the data.
    /// </summary>
    public class DataFormatViewModel : Caliburn.Micro.Screen, IDisposable, ICodecLayer, IActivate, IDeactivate
    {
        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Options.
        /// </summary>
        private DataFormatOptions _options;

        /// <summary>
        /// ADCP codec.
        /// </summary>
        private AdcpCodec _codec;

        /// <summary>
        /// Timer to update the data.
        /// </summary>
        private System.Timers.Timer _displayTimer;

        #endregion

        #region Properties

        #region Formats

        #region RTB

        /// <summary>
        /// Flag to turn on or off decoding RTB data.
        /// </summary>
        public bool IsRtb
        {
            get { return _options.IsRTB; }
            set
            {
                _options.IsRTB = value;
                NotifyOfPropertyChange(() => IsRtb);
            }
        }

        /// <summary>
        /// Number of ensembles received that was RTB data.
        /// </summary>
        private long _RtbDataCount;
        /// <summary>
        /// Number of ensembles received that was RTB data.
        /// </summary>
        public string RtbDataCount
        {
            get { return _RtbDataCount.ToString(); }
        }

        /// <summary>
        /// Number of ensembles received that was RTB data.
        /// </summary>
        private long _RtbDataBytes;
        /// <summary>
        /// Number of ensembles received that was RTB data.
        /// </summary>
        public string RtbDataBytes
        {
            get { return MathHelper.MemorySizeString(_RtbDataBytes); }
        }

        #endregion

        #region RTD

        /// <summary>
        /// Flag to turn on or off decoding RTD data.
        /// </summary>
        public bool IsRtd
        {
            get { return _options.IsRTD; }
            set
            {
                _options.IsRTD = value;
                NotifyOfPropertyChange(() => IsRtd);
            }
        }

        /// <summary>
        /// Number of ensembles received that was RTD data.
        /// </summary>
        private long _RtdDataCount;
        /// <summary>
        /// Number of ensembles received that was RTD data.
        /// </summary>
        public string RtdDataCount
        {
            get { return _RtdDataCount.ToString(); }
        }

        /// <summary>
        /// Number of ensembles received that was RTD data.
        /// </summary>
        private long _RtdDataBytes;
        /// <summary>
        /// Number of ensembles received that was RTD data.
        /// </summary>
        public string RtdDataBytes
        {
            get { return MathHelper.MemorySizeString(_RtdDataBytes); }
        }

        #endregion

        #region PD0

        /// <summary>
        /// Flag to turn on or off decoding PD0 data.
        /// </summary>
        public bool IsPd0
        {
            get { return _options.IsPD0; }
            set
            {
                _options.IsPD0 = value;
                NotifyOfPropertyChange(() => IsPd0);
            }
        }

        /// <summary>
        /// Number of ensembles received that was PD0 data.
        /// </summary>
        private long _Pd0DataCount;
        /// <summary>
        /// Number of ensembles received that was PD0 data.
        /// </summary>
        public string Pd0DataCount
        {
            get { return _Pd0DataCount.ToString(); }
        }

        /// <summary>
        /// Number of ensembles received that was PD0 data.
        /// </summary>
        private long _Pd0DataBytes;
        /// <summary>
        /// Number of ensembles received that was PD0 data.
        /// </summary>
        public string Pd0DataBytes
        {
            get { return MathHelper.MemorySizeString(_Pd0DataBytes); }
        }

        #endregion

        #region PD6_13

        /// <summary>
        /// Flag to turn on or off decoding PD6/13 data.
        /// </summary>
        public bool IsPd6_13
        {
            get { return _options.IsPD6_13; }
            set
            {
                _options.IsPD6_13 = value;
                NotifyOfPropertyChange(() => IsPd6_13);
            }
        }

        /// <summary>
        /// Number of ensembles received that was PD6/13 data.
        /// </summary>
        private long _Pd6_13DataCount;
        /// <summary>
        /// Number of ensembles received that was PD6/13 data.
        /// </summary>
        public string Pd6_13DataCount
        {
            get { return _Pd6_13DataCount.ToString(); }
        }

        /// <summary>
        /// Number of ensembles received that was PD6/13 data.
        /// </summary>
        private long _Pd6_13DataBytes;
        /// <summary>
        /// Number of ensembles received that was PD6/13 data.
        /// </summary>
        public string Pd6_13DataBytes
        {
            get { return MathHelper.MemorySizeString(_Pd6_13DataBytes); }
        }

        #endregion

        #region PD4_5

        /// <summary>
        /// Flag to turn on or off decoding PD4/5 data.
        /// </summary>
        public bool IsPd4_5
        {
            get { return _options.IsPD4_5; }
            set
            {
                _options.IsPD4_5 = value;
                NotifyOfPropertyChange(() => IsPd4_5);
            }
        }

        /// <summary>
        /// Number of ensembles received that was PD4/5 data.
        /// </summary>
        private long _Pd4_5DataCount;
        /// <summary>
        /// Number of ensembles received that was PD4/5 data.
        /// </summary>
        public string Pd4_5DataCount
        {
            get { return _Pd4_5DataCount.ToString(); }
        }

        /// <summary>
        /// Number of ensembles received that was PD4/5 data.
        /// </summary>
        private long _Pd4_5DataBytes;
        /// <summary>
        /// Number of ensembles received that was PD4/5 data.
        /// </summary>
        public string Pd4_5DataBytes
        {
            get { return MathHelper.MemorySizeString(_Pd4_5DataBytes); }
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Initialize.
        /// </summary>
        public DataFormatViewModel()
        {
            _options = new DataFormatOptions();

            // Initialize the codec
            _codec = new AdcpCodec();

            // Init values
            IsRtb = _options.IsRTB;
            _RtbDataCount = 0;
            _RtbDataBytes = 0;
            IsRtd = _options.IsRTD;
            _RtdDataCount = 0;
            _RtdDataBytes = 0;
            IsPd0 = _options.IsPD0;
            _Pd0DataCount = 0;
            _Pd0DataBytes = 0;
            IsPd6_13 = _options.IsPD6_13;
            _Pd6_13DataCount = 0;
            _Pd6_13DataBytes = 0;
            IsPd4_5 = _options.IsPD4_5;
            _Pd4_5DataCount = 0;
            _Pd4_5DataBytes = 0;

            // Setup event handlers
            _codec.ProcessDataEvent += _codec_ProcessDataEvent;

            // Update the display
            _displayTimer = new System.Timers.Timer(1000);
            _displayTimer.Elapsed += _displayTimer_Elapsed;
            _displayTimer.AutoReset = true;
            _displayTimer.Enabled = true;
        }

        #region Dispose

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public void Dispose()
        {
            // Stop the timer
            _displayTimer.Stop();
            _displayTimer.Dispose();

            // Shutdown the codec
            _codec.Dispose();
        }

        #endregion

        #region Activate / Deactivate

        /// <summary>
        /// Call if closing the screen.
        /// </summary>
        /// <param name="close"></param>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            // If shutting down, disconnect if still connected
            if (close)
            {
                _displayTimer.Stop();
            }
        }

        /// <summary>
        /// Start the timer on activate.
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();

            _displayTimer.Start();
        }

        #endregion

        #region Add Data

        /// <summary>
        /// Ensemble data to add to the codec.
        /// </summary>
        /// <param name="data">Ensemble data.</param>
        /// <returns>Negative number indicates an error.</returns>
        public int AddData(byte[] data, EnsembleSource source)
        {
            // Add data to codec, set flags for data to decode
            _codec.AddIncomingData(data,
                                    _options.IsRTB,
                                    _options.IsRTD,
                                    _options.IsPD0,
                                    _options.IsPD6_13,
                                    _options.IsPD4_5);

            log.Debug(string.Format("Add data to codec: Source: {0}  Count: {1} IsRTB: {2}  IsRTD: {3}  IsPD0 {4}  IsPD6_13: {5}  IsPD4_5: {6}", source, data.Length, _options.IsRTB, _options.IsRTD, _options.IsPD0, _options.IsPD6_13, _options.IsPD4_5));

            return data.Length;
        }

        /// <summary>
        /// Add the NMEA data to the codec.
        /// </summary>
        /// <param name="data">NMEA data.</param>
        /// <param name="source">Ensemble source.</param>
        /// <returns>Negative value indicates an error.</returns>
        public int AddNmeaData(byte[] data, EnsembleSource source)
        {
            // Convert the data to ASCII
            string ascii = System.Text.ASCIIEncoding.ASCII.GetString(data);

            // Add data to codec
            _codec.AddNmeaData(ascii);

            log.Debug(string.Format("Add NMEA data: {0}", ascii));

            return data.Length;
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Process the ensemble data from the codec.
        /// </summary>
        /// <param name="binaryEnsemble">Binary ensemble data.</param>
        /// <param name="ensemble">Ensemble object.</param>
        /// <param name="dataFormat">Format the data was decoded.</param>
        private void _codec_ProcessDataEvent(byte[] binaryEnsemble, DataSet.Ensemble ensemble, AdcpCodec.CodecEnum dataFormat)
        {
            // Determine the data format
            switch(dataFormat)
            {
                case AdcpCodec.CodecEnum.Binary:                                // RTB
                    _RtbDataBytes += binaryEnsemble.Length;
                    _RtbDataCount++;
                    log.Debug(string.Format("Binary Codec: Count: {0} Ensemble: {1}", binaryEnsemble.Length, ensemble));
                    break;
                case AdcpCodec.CodecEnum.DVL:                                   // RTD
                    _RtdDataBytes += binaryEnsemble.Length;
                    _RtdDataCount++;
                    log.Debug(string.Format("DVL Codec: Count: {0} Ensemble: {1}", binaryEnsemble.Length, ensemble));
                    break;
                case AdcpCodec.CodecEnum.PD0:                                   // PD0
                    _Pd0DataBytes += binaryEnsemble.Length;
                    _Pd0DataCount++;
                    log.Debug(string.Format("PD0 Codec: Count: {0} Ensemble: {1}", binaryEnsemble.Length, ensemble));
                    break;
                case AdcpCodec.CodecEnum.PD6_13:                                   // PD6/13
                    _Pd6_13DataBytes += binaryEnsemble.Length;
                    _Pd6_13DataCount++;
                    log.Debug(string.Format("PD6_13 Codec: Count: {0} Ensemble: {1}", binaryEnsemble.Length, ensemble));
                    break;
                case AdcpCodec.CodecEnum.PD4_5:                                   // PD4/5
                    _Pd4_5DataBytes += binaryEnsemble.Length;
                    _Pd4_5DataCount++;
                    log.Debug(string.Format("PD4_5 Codec: Count: {0} Ensemble: {1}", binaryEnsemble.Length, ensemble));
                    break;
                default:
                    break;
            }

            // Pass the data to screeners
            // Pass the data to all codec layers
            foreach (var vm in IoC.GetAllInstances(typeof(IScreenEnsLayer)))
            {
                ((IScreenEnsLayer)vm).ScreenEnsemble(binaryEnsemble, ensemble, EnsembleSource.Serial, dataFormat);
            }

        }

        /// <summary>
        /// Update the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _displayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                NotifyOfPropertyChange(() => RtbDataBytes);
                NotifyOfPropertyChange(() => RtbDataCount);
                NotifyOfPropertyChange(() => RtdDataBytes);
                NotifyOfPropertyChange(() => RtdDataCount);
                NotifyOfPropertyChange(() => Pd0DataBytes);
                NotifyOfPropertyChange(() => Pd0DataCount);
                NotifyOfPropertyChange(() => Pd6_13DataBytes);
                NotifyOfPropertyChange(() => Pd6_13DataCount);
                NotifyOfPropertyChange(() => Pd4_5DataBytes);
                NotifyOfPropertyChange(() => Pd4_5DataCount);
            }
            catch(TaskCanceledException)
            {

            }
            catch(Exception)
            {
                log.Error("Error with display timer.");
            }
        }

        #endregion
    }
}