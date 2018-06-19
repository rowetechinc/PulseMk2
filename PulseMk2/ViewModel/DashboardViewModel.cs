﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataSet;

namespace RTI
{
    public class DashboardViewModel : Caliburn.Micro.Screen, IProcessEnsLayer, IProjectLayer
    {
        #region Variable

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Dictionary to hold the subsystem config and the view model.
        /// </summary>
        private Dictionary<ViewSubsystemConfig, DashboardSubsystemConfigViewModel> _dictSsConfig;

        #endregion

        #region Properties

        /// <summary>
        /// Collection of all the subsytem configurations.
        /// </summary>
        public ObservableCollection<DashboardSubsystemConfigViewModel> SsConfigList { get; set; }

        /// <summary>
        /// The selected Subsystem Config to remove.
        /// </summary>
        private DashboardSubsystemConfigViewModel _SelectedSsConfig;
        /// <summary>
        /// The selected Subsystem Config to remove.
        /// </summary>
        public DashboardSubsystemConfigViewModel SelectedSsConfig
        {
            get { return _SelectedSsConfig; }
            set
            {
                _SelectedSsConfig = value;
                NotifyOfPropertyChange(() => SelectedSsConfig);
            }
        }

        /// <summary>
        /// The selected index Subsystem Config.
        /// </summary>
        private int _SelectedSsConfigIndex;
        /// <summary>
        /// The selected index Subsystem Config.
        /// </summary>
        public int SelectedSsConfigIndex
        {
            get { return _SelectedSsConfigIndex; }
            set
            {
                _SelectedSsConfigIndex = value;
                NotifyOfPropertyChange(() => SelectedSsConfigIndex);
            }
        }

        /// <summary>
        /// Current status.
        /// </summary>
        private string _Status;
        /// <summary>
        /// Current status.
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        #endregion

        /// <summary>
        /// Initialize.
        /// </summary>
        public DashboardViewModel()
        {
            // Create the dict
            _dictSsConfig = new Dictionary<ViewSubsystemConfig, DashboardSubsystemConfigViewModel>();

            // Create the list to display all the views
            SsConfigList = new ObservableCollection<DashboardSubsystemConfigViewModel>();
        }

        #region Load Project

        /// <summary>
        /// Load the project file.
        /// </summary>
        /// <param name="project">Project to load.</param>
        public void LoadProject(Project project)
        {
            Status = string.Format("Loading project {0}...", project.GetProjectFullPath());

            // Only load the maximum number of configurations to see
            // all the configurations
            int numEns = project.GetNumberOfEnsembles();
            if(numEns > RTI.Commons.MAX_CONFIGURATIONS)
            {
                numEns = RTI.Commons.MAX_CONFIGURATIONS;
            }

            // Load all the configurations
            ViewSubsystemConfig prevConfig = null;
            for (int x = 1; x <= numEns; x++)
            {
                // Process the first 12 ensembles because there can only be up to 12 configurations
                ViewSubsystemConfig config = ProcessEnsemble(project.GetEnsemble(x), EnsembleSource.Playback, AdcpCodec.CodecEnum.Binary);

                if (config != null)
                {
                    // Only load new configs
                    if (config != prevConfig)
                    {
                        _dictSsConfig[config].LoadProject(project);
                    }

                    // Keep track of the previous config so we do not reload the same configuration
                    prevConfig = config;

                    log.Debug(string.Format("Add configuration {0}", config.Config.DescString()));
                }
            }

            // Select the last config add
            SelectedSsConfigIndex = SsConfigList.Count();

            Status = string.Format("Project {0}", project.GetProjectFullPath());
            log.Debug(string.Format("Add Project {0}", project.GetProjectFullPath()));
        }

        #endregion

        #region Remove Tab

        /// <summary>
        /// Remove the View model from the tab list.
        /// </summary>
        public void CloseTab()
        {
            Status = string.Format("Remove configuration {0}...", _SelectedSsConfig.Config.Config.DescString());
            log.Debug(string.Format("Remove configuration {0}...", _SelectedSsConfig.Config.Config.DescString()));

            // Remove it from the dictionary
            _dictSsConfig.Remove(_SelectedSsConfig.Config);

            // Remove it from the List
            SsConfigList.Remove(_SelectedSsConfig);

            // Select the last config add
            SelectedSsConfigIndex = SsConfigList.Count();
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Process the ensembles.  Look for new subsystem configurations with each ensemble.
        /// </summary>
        /// <param name="ensemble"></param>
        /// <param name="source"></param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public ViewSubsystemConfig ProcessEnsemble(Ensemble ensemble, EnsembleSource source, AdcpCodec.CodecEnum dataFormat)
        {
            if (ensemble != null && ensemble.IsEnsembleAvail)
            {
                // Create subsystem config
                ViewSubsystemConfig config = new ViewSubsystemConfig(ensemble.EnsembleData.SubsystemConfig, source);

                // Check if the config exist already
                if (!_dictSsConfig.ContainsKey(config))
                {
                    // Create the viewmodel for each subsystem config/source found
                    DashboardSubsystemConfigViewModel vm = new DashboardSubsystemConfigViewModel(config);

                    // Add the vm to the list
                    _dictSsConfig.Add(config, vm);
                    SsConfigList.Add(vm);
                    log.Debug(string.Format("Add configuration {0}", config.Config.DescString()));

                    // Select the last config add
                    SelectedSsConfigIndex = SsConfigList.Count();

                    // Pass the ensemble to the viewmodel
                    vm.ProcessEnsemble(ensemble, source);

                    return config;
                }
                else
                {
                    // Viewmodel already exist, so send the ensemble
                    DashboardSubsystemConfigViewModel vm = null;
                    if (_dictSsConfig.TryGetValue(config, out vm))
                    {
                        vm.ProcessEnsemble(ensemble, source);
                    }

                    return config;
                }
            }

            // Not a valid ensemble
            return null;
        }

        #endregion
    }
}
