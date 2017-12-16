using API.DARP.API;
using API.DARP.Data.Model;
using API.DARP.Data.Writers;

namespace API.DARP.PSAPI.Processes
{

    public class ExportCSVInput
    {
        public Solution Solution { get; set; }

        public string Path { get; set; }

    }

    public class ExportCSVOutput
    {

    }

    public interface IExportCSV : IProcess<ExportCSVInput, ExportCSVOutput>
    {
    }

    class ExportCSV : IExportCSV
    {
        #region Dependencies
        DARPWriter writer = Controllers.Instance.Specific.Data.Writers.DARPWriter;
        #endregion

        public ExportCSVOutput Execute(ExportCSVInput parameters)
        {
            ExportCSVOutput output = new ExportCSVOutput();
            // TODO: Lógica del proceso
            writer.WriteCSV(parameters.Solution, parameters.Path);

            return output;
        }
    }
}
