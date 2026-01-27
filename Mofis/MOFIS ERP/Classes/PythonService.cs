using System;
using System.Diagnostics;
using System.IO;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Servicio para manejar la ejecución de scripts de Python
    /// </summary>
    public class PythonService
    {
        /// <summary>
        /// Ejecuta el script de conciliación en el entorno especificado
        /// </summary>
        /// <param name="scriptPath">Ruta al script .py</param>
        /// <param name="workingDir">Directorio de trabajo (donde están los fideicomisos)</param>
        /// <param name="fideicomiso">Nombre del fideicomiso seleccionado</param>
        public void ExecuteConciliacion(string scriptPath, string workingDir, string fideicomiso)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException("No se encontró el script de Python.", scriptPath);

            if (!Directory.Exists(workingDir))
                throw new DirectoryNotFoundException($"No se encontró el directorio de trabajo: {workingDir}");

            // Construir argumentos
            // TODO: Si el script de python acepta argumentos por línea de comandos, se deben pasar aquí.
            // Por ahora, asumimos que el script es interactivo o necesita variables de entorno, 
            // pero lo ejecutaremos en una ventana visible para que el usuario pueda interactuar si es necesario.
            
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // Asume que python está en el PATH
            start.Arguments = $"\"{scriptPath}\""; 
            start.WorkingDirectory = workingDir; // Ejecutar en el directorio seleccionado
            start.UseShellExecute = true; // Abrir en nueva ventana (necesario para input/output visible)
            start.CreateNoWindow = false; // Mostrar la ventana

            // Opcional: Si modificamos el script de python para aceptar argumentos, podríamos pasar el fideicomiso así:
            // start.Arguments = $"\"{scriptPath}\" --fideicomiso \"{fideicomiso}\"";

            try
            {
                using (Process process = Process.Start(start))
                {
                    // No esperamos a que termine para no bloquear la UI principal, 
                    // o podemos esperar si queremos saber cuando terminó.
                    // process.WaitForExit(); 
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar Python: {ex.Message}. Asegúrate de que Python esté instalado y en el PATH.", ex);
            }
        }

        public bool IsPythonInstalled()
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(start))
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
