using API.DARP.API;
using DARP.Controllers;
using DARP.Splash;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using API.DARP.PSAPI.Processes;
using API.DARP.PSAPI;
using DARP.Processes;

namespace DARP.ViewModels.Main
{
    using API.DARP.Data.Model;
    using API.DARP.Data.Model.Settings;
    using Results;
    using Settings;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using EnumStatusType = Constants.StatusType;

    public class MainWindowViewModel : INotifyPropertyChanged
    {

        #region Fields
        IWindowController windowController;
        ProcessController processController;
        SplashController splashController;
        CommandList commands = new CommandList();
        #endregion


        #region Bindings

        #region ProblemsInput 

        private DataView problemsInput;

        public DataView ProblemsInput
        {
            get
            {
                return problemsInput;
            }

            set
            {
                if (value != problemsInput)
                {
                    problemsInput = value;
                    // OnProblemsInputChanged();
                    NotifyPropertyChanged(nameof(ProblemsInput));
                }
            }
        }

        #endregion

        #region SolutionsData 

        private DataView solutionsData;

        public DataView SolutionsData
        {
            get
            {
                return solutionsData;
            }

            set
            {
                if (value != solutionsData)
                {
                    solutionsData = value;
                    // OnSolutionsDataChanged();
                    NotifyPropertyChanged(nameof(SolutionsData));
                }
            }
        }

        #endregion

        #region DARPProblems 

        private List<string> darpProblems;

        public List<string> DARPProblems
        {
            get
            {
                return darpProblems;
            }

            set
            {
                if (value != darpProblems)
                {
                    darpProblems = value;
                    // OnDARPProblemsChanged();
                    NotifyPropertyChanged(nameof(DARPProblems));
                }
            }
        }

        #endregion

        #region DARPSelected 

        private string darpSelected;

        public string DARPSelected
        {
            get
            {
                return darpSelected;
            }

            set
            {
                if (value != darpSelected)
                {
                    darpSelected = value;
                    // OnDARPSelectedChanged();
                    NotifyPropertyChanged(nameof(DARPSelected));
                }
            }
        }

        #endregion

        #region DARPResults 

        private ResultsViewModel dARPResults;

        public ResultsViewModel DARPResults
        {
            get
            {
                return dARPResults;
            }

            set
            {
                if (value != dARPResults)
                {
                    dARPResults = value;
                    // OnDARPResultsChanged();
                    NotifyPropertyChanged(nameof(DARPResults));
                }
            }
        }

        #endregion

        #region ILSEvolution 

        private ILSEvolutionViewModel ilsEvolution;

        public ILSEvolutionViewModel ILSEvolution
        {
            get
            {
                return ilsEvolution;
            }

            set
            {
                if (value != ilsEvolution)
                {
                    ilsEvolution = value;
                    // OnILSEvolutionChanged();
                    NotifyPropertyChanged(nameof(ILSEvolution));
                }
            }
        }

        #endregion

        #region VNSOperators 

        private VNSOperatorsViewModel vnsOperators;

        public VNSOperatorsViewModel VNSOperators
        {
            get
            {
                return vnsOperators;
            }

            set
            {
                if (value != vnsOperators)
                {
                    vnsOperators = value;
                    // OnVNSOperatorsChanged();
                    NotifyPropertyChanged(nameof(VNSOperators));
                }
            }
        }

        #endregion
            
        #region AlgorithmSummary 

        private SummaryResultsViewModel summaryResultsViewModel;

        public SummaryResultsViewModel AlgorithmSummary
        {
            get
            {
                return summaryResultsViewModel;
            }

            set
            {
                if (value != summaryResultsViewModel)
                {
                    summaryResultsViewModel = value;
                    // OnAlgorithmSummaryChanged();
                    NotifyPropertyChanged(nameof(AlgorithmSummary));
                }
            }
        }

        #endregion

        #region ChartsViewModel 

        private ChartsViewModel chartsViewModel;

        public ChartsViewModel ChartsViewModel
        {
            get
            {
                return chartsViewModel;
            }

            set
            {
                if (value != chartsViewModel)
                {
                    chartsViewModel = value;
                    // OnChartsViewModelChanged();
                    NotifyPropertyChanged(nameof(ChartsViewModel));
                }
            }
        }

        #endregion





        #region HeuristicExecuted 

        private bool heuristicExecuted;

        public bool HeuristicExecuted
        {
            get
            {
                return heuristicExecuted;
            }

            set
            {
                if (value != heuristicExecuted)
                {
                    heuristicExecuted = value;
                    // OnHeuristicExecutedChanged();
                    NotifyPropertyChanged(nameof(HeuristicExecuted));
                }
            }
        }

        #endregion

        #region TabSelected 

        private int tabSelected;

        public int TabSelected
        {
            get
            {
                return tabSelected;
            }

            set
            {
                if (value != tabSelected)
                {
                    tabSelected = value;
                    // OnTabSelectedChanged();
                    NotifyPropertyChanged(nameof(TabSelected));
                }
            }
        }

        #endregion


        #region StatusText 

        private string statusText;

        public string StatusText
        {
            get
            {
                return statusText;
            }

            set
            {
                if (value != statusText)
                {
                    statusText = value;
                    // OnStatusTextChanged();
                    NotifyPropertyChanged(nameof(StatusText));
                }
            }
        }

        #endregion

        #region StatusType 

        private EnumStatusType statusType;

        public EnumStatusType StatusType
        {
            get
            {
                return statusType;
            }

            set
            {
                if (value != statusType)
                {
                    statusType = value;
                    // OnStatusTypeChanged();
                    NotifyPropertyChanged(nameof(StatusType));
                }
            }
        }

        #endregion





        #endregion

        #region Commands

        public ICommand LoadInput { get; }
        public ICommand SettingsCommand { get; }
        public ICommand RunHeuristic { get; }

        public ICommand ExportCSV { get; set; }



        #endregion


        public MainWindowViewModel()
        {
            DARPResults = null;
            ILSEvolution = null;
            AlgorithmSummary = null;
            ChartsViewModel = null;
            VNSOperators = null;
            ProblemsInput = null;
            SolutionsData = null;
            DARPProblems = new List<string>();
            DARPSelected = null;
            HeuristicExecuted = false;
            TabSelected = 0;

            //Inicializate Commands
            LoadInput = commands.AddAsyncCommand(ExecuteLoadProblems, CanExecuteLoadProblems);
            SettingsCommand = commands.AddCommand(ExecuteOpenSettings, CanExecuteOpenSettings);
            RunHeuristic = commands.AddAsyncCommand(ExecuteRunHeuristic, CanExecuteRunHeuristic);
            ExportCSV = commands.AddAsyncCommand(ExecuteExportCSV, CanExecuteExportCSV);

            //Load Settings
            Context.Instance.Settings = DARP.Settings.CheckDeserializeSettings(LoadLasSettings());

            //Inicializate controllers.
            var controllers = ViewModelControllers.Instance;
            windowController = controllers.WindowController;
            processController = ProcessController.Instance;
            splashController = controllers.SplashController;
        }






        #region CommandImplementations


        async Task ExecuteExportCSV()
        {
            try
            {
                var path = Context.Instance.Settings.GeneralSettings.RootPathSolutions + "\\" + Context.Instance.Solutions.First().ID_DARP + "-" + DateTime.Now.ToString("MM-dd-yyyy HHmmss") + ".csv";
                ExportCSVInput csvInput = new ExportCSVInput { Solution = Context.Instance.Solutions.First(), Path = path };
                ExportCSVOutput csvOutput = await processController.Specific.ExportCSV.ExecuteProcess(csvInput);

                if (csvOutput != null)
                {
                    SetStatus(Constants.StatusType.Ok, String.Format(Properties.Resources.CSV_EXPORT_SUCCESFULLY));
                }
                else
                {
                    SetStatus(Constants.StatusType.Error, String.Format(Properties.Resources.ERROR_EXPORTING_CSV));
                }
            }
            finally
            {

            }
        }

        private bool CanExecuteExportCSV()
        {
            return DARPResults != null;
        }


        private bool CanExecuteOpenSettings()
        {
            return true;
        }

        private void ExecuteOpenSettings()
        {
            SettingsViewModel vm = new SettingsViewModel();
            if (windowController.ShowWindow(vm))
            {
                SaveLastSettingsConfiguration();
                SetStatus(Constants.StatusType.Ok, String.Format(Properties.Resources.SETTINGS_SAVED_SUCCESFULLY));
            }
            else
                SetStatus(Constants.StatusType.Ok, String.Format(Properties.Resources.SETTINGS_ERROR_SAVING));
        }


        async Task ExecuteRunHeuristic()
        {
            try
            {
                List<Problem> problems = null;
                IHeuristicConfigurationSetting settings = null;
                if (Context.Instance.Settings.GeneralSettings.MultiplesExecutions)
                {
                    problems = Context.Instance.Problems;
                }
                else
                    problems = new List<Problem> { Context.Instance.Problems.Where(t => t.ID_Problem.Equals(DARPSelected)).ToList().First() };

                switch (Context.Instance.Settings.GeneralSettings.DefaultHeuristic)
                {
                    case "ILS":
                        settings = Context.Instance.Settings.ILSConfigurationSettings;
                        break;
                    default:
                        settings = Context.Instance.Settings.ILSConfigurationSettings;
                        break;
                }

                //Mostrar Splash
                splashController.ShowSplash(new SplashInfo());
                RunMetaheuristicInput input = new RunMetaheuristicInput { Problems = problems, HeuristicSettings = settings, Random = new Random(1) };
                RunMetaheuristicOutput output = await processController.Specific.RunMetaheuristic.ExecuteProcess(input);
                if (output != null)
                {
                    Context.Instance.Solutions = output.Solutions;
                    DARPResults = new ResultsViewModel(output.Solutions.First());
                    ILSEvolution = new ILSEvolutionViewModel(output.Solutions.First());
                    VNSOperators = new VNSOperatorsViewModel(output.Solutions.First());
                    AlgorithmSummary = new SummaryResultsViewModel(output.Solutions.First());
                    ChartsViewModel = new ChartsViewModel(output.Solutions.First());
                    HeuristicExecuted = true;
                    TabSelected = 1;
                    SetStatus(Constants.StatusType.Ok, String.Format(Properties.Resources.HEURISTIC_EXECUTED_SUCCESFULLY));
                }
                else
                {
                    SetStatus(Constants.StatusType.Ok, String.Format(Properties.Resources.HEURISTIC_EXECUTED_FAIL));
                }
            }
            finally
            {
                splashController.HideSplash();
            }
        }

        private bool CanExecuteRunHeuristic()
        {
            return ProblemsInput != null;
        }


        private bool CanExecuteLoadProblems()
        {
            return true;
        }

        async Task ExecuteLoadProblems()
        {
            DARPProblems.Clear();
            DARPSelected = String.Empty;
            try
            {
                DARPResults = null;
                ILSEvolution = null;
                VNSOperators = null;
                AlgorithmSummary = null;
                ChartsViewModel = null;
                var root_Path = Context.Instance.Settings.GeneralSettings.RootPath;
                List<string> files = new List<string>();
                if (!string.IsNullOrEmpty(root_Path))
                {
                    if (Context.Instance.Settings.GeneralSettings.ExploreSubFolders)
                    {
                        //Hay que buscar también en las subcarpetas.
                        string[] filesT = Directory.GetFiles(root_Path);
                        foreach (var primary in filesT)
                        {
                            files.Add(primary);
                        }
                        string[] subdirEntries = Directory.GetDirectories(root_Path);
                        foreach (var entry in subdirEntries)
                        {
                            var subEntries = Directory.GetFiles(entry);
                            foreach (var entry2 in subEntries)
                            {
                                files.Add(entry2);
                            }
                        }
                    }
                    else
                        files = Directory.GetFiles(root_Path).ToList();
                    if (files.Count() > 0)
                    {
                        //Mostrar Splash
                        splashController.ShowSplash(new SplashInfo());
                        LoadDARPInput input = new LoadDARPInput { Files = files.ToArray() };
                        LoadDARPOutput output = await processController.Specific.LoadDARP.ExecuteProcess(input);
                        if (output != null)
                        {
                            Context.Instance.Problems = output.DataLoaded.Problems;
                            foreach (var problem in Context.Instance.Problems)
                            {
                                DARPProblems.Add(problem.ID_Problem);
                            }
                            DARPSelected = DARPProblems.First();
                            ProblemsInput = output.DataLoaded.TableProblems.DefaultView;
                            DARPResults = null;
                            ILSEvolution = null;
                            VNSOperators = null;
                            AlgorithmSummary = null;
                            ChartsViewModel = null;
                            HeuristicExecuted = false;
                            TabSelected = 0;
                            SetStatus(Constants.StatusType.Ok, String.Format(Properties.Resources.PROBLEMS_LOADED, Context.Instance.Problems.Count));
                        }
                        else
                        {
                            ProblemsInput = null;
                            SetStatus(Constants.StatusType.Error, String.Format(Properties.Resources.ERROR_PROBLEMS_LOADED));
                        }
                    }
                    else
                    {
                        ProblemsInput = null;
                        SetStatus(Constants.StatusType.Error, String.Format(Properties.Resources.ERROR_LOADING_NOT_INSTANCES));
                    }
                }
                else
                {
                    ProblemsInput = null;
                    SetStatus(Constants.StatusType.Warning, String.Format(Properties.Resources.PROBLEMS_WARNING));
                }
            }
            finally
            {
                splashController.HideSplash();
            }
        }


        #region Methods

        private void SetStatus(Constants.StatusType status, string message)
        {
            StatusText = message;
            StatusType = status;
        }
        #endregion

        #endregion


        #region Settings
        private DARP.Settings LoadLasSettings()
        {
            DARP.Settings saved = null;

            try
            {
                // Deserialize it from binary
                if (System.IO.File.Exists(Constants.SpecificLastExecutionPath))
                {
                    IFormatter formatter = new BinaryFormatter();

                    using (var stream = File.OpenRead(Constants.SpecificLastExecutionPath))
                    {
                        saved = formatter.Deserialize(stream) as DARP.Settings;
                    }
                }
            }
            catch
            {
                // Ignore deserializing errors
            }



            return saved ?? new DARP.Settings();
        }

        private void SaveLastSettingsConfiguration()
        {
            // Serialize it to Binary format
            IFormatter formatter = new BinaryFormatter();

            if (!Directory.Exists(Constants.ConfigurationFolder))
                Directory.CreateDirectory(Constants.ConfigurationFolder);
            // To write to a file, create a StreamWriter object.
            using (Stream ioStream = File.Create(Constants.SpecificLastExecutionPath))
            {
                formatter.Serialize(ioStream, Context.Instance.Settings);
            }
        }
        #endregion


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
