using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model.Settings
{

    [Serializable]
    public class GeneralSettings
    {

        public bool ExploreSubFolders { get; set; }

        public string RootPath { get; set; }

        public string RootPathSolutions { get; set; }


        public bool MultiplesExecutions { get; set; }

        public List<string> ListHeuristics { get; set; }

        public string DefaultHeuristic { get; set; }

        public GeneralSettings()
        {

        }

        public GeneralSettings(GeneralSettings generalSettings)
        {
            ExploreSubFolders = generalSettings.ExploreSubFolders;
            RootPath = generalSettings.RootPath;
            ListHeuristics = generalSettings.ListHeuristics;
            MultiplesExecutions = generalSettings.MultiplesExecutions;
            DefaultHeuristic = generalSettings.DefaultHeuristic;
            RootPathSolutions = generalSettings.RootPathSolutions;
        }


        public static GeneralSettings CreateDefaults()
        {
            return new GeneralSettings { RootPath = String.Empty, RootPathSolutions= String.Empty, ExploreSubFolders = false, MultiplesExecutions = false, ListHeuristics = Constants.ListHeuristics, DefaultHeuristic = Constants.DEFAULT_HEURISTIC };
        }

    }
}
