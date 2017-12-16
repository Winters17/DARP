using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.API
{
    public interface IProcess<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {
        /// <summary>
        /// Invoca la ejecución del proceso
        /// </summary>
        /// <param name="parameters">Datos de entrada del proceso ("parámetros")</param>
        /// <returns>Datos de salida del proceso ("datos")</returns>
        /// <remarks>El proceso nunca debe devolver null como valor de salida.
        /// Los procesos que no devuelven resultados pueden utilizar VoidType.Empty</remarks>
        TOutput Execute(TInput parameters);
    }

}
