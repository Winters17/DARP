namespace DARP.Processes
{
    using API.DARP.API;
    using API.DARP.Data.Error;
    using Controllers;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;



    internal static class PSAPI 
    {        
        #region New PSAPI
               
        public async static Task<TOutput> ExecuteProcess<TInput, TOutput>(this IProcess<TInput, TOutput> process, TInput input)
            where TInput : class
            where TOutput : class
        {
            try
            {
                TOutput output = await Task.Run(() => process.Execute(input));
                return output;
            }
            catch (Exception ex)
            {
                ViewModelControllers.Instance.SplashController.HideSplash();
                ViewModelControllers.Instance.DialogController.ShowError(new ErrorInfo(), ex);
                return null;
            }
        }

        #endregion
    }
}
