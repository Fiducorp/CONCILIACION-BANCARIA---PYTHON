using System;
using System.Collections.Generic;
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
        /// <param name="workingDir">Directorio de trabajo del fideicomiso</param>
        /// <param name="bankFile">Ruta completa al archivo de banco</param>
        /// <param name="ledgerFile">Ruta completa al archivo de libro contable</param>
        /// <param name="currency">Moneda (DOP/USD)</param>
        /// <param name="parameters">Diccionario de parámetros opcionales (tolerancias, etc.)</param>
        /// <param name="onOutputReceived">Acción a ejecutar cuando se recibe una línea de salida</param>
        /// <param name="onExited">Acción a ejecutar cuando el proceso termina</param>
        public void ExecuteConciliacion(string scriptPath, string workingDir, string bankFile, string ledgerFile, string currency, Dictionary<string, object> parameters = null, Action<string> onOutputReceived = null, Action<int> onExited = null)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException("No se encontró el script de Python.", scriptPath);

            if (!Directory.Exists(workingDir))
                throw new DirectoryNotFoundException($"No se encontró el directorio de trabajo: {workingDir}");

            // Construir argumentos para modo HEADLESS
            string args = $"\"{scriptPath}\" --dir \"{workingDir}\" --bank \"{bankFile}\" --ledger \"{ledgerFile}\" --currency \"{currency}\"";
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param.Value == null) continue;
                    
                    string value = param.Value.ToString().ToLower();
                    if (param.Value is bool)
                        value = (bool)param.Value ? "true" : "false";

                    args += $" --{param.Key} \"{value}\"";
                }
            }
            
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // Asume que python está en el PATH
            start.Arguments = args; 
            start.WorkingDirectory = workingDir; 
            
            if (onOutputReceived != null)
            {
                start.UseShellExecute = false; 
                start.CreateNoWindow = true; 
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                
                // Force UTF-8 and Unbuffered for real-time redirection
                start.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
                start.EnvironmentVariables["PYTHONUTF8"] = "1";
                start.EnvironmentVariables["PYTHONUNBUFFERED"] = "1";
            }
            else
            {
                start.UseShellExecute = true; 
                start.CreateNoWindow = false; 
            }

            try
            {
                Process process = new Process();
                process.StartInfo = start;

                if (onOutputReceived != null)
                {
                    process.OutputDataReceived += (s, e) => { if (e.Data != null) onOutputReceived(e.Data); };
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) onOutputReceived("ERR: " + e.Data); };
                    
                    if (onExited != null)
                    {
                        process.EnableRaisingEvents = true;
                        process.Exited += (s, e) => onExited(process.ExitCode);
                    }

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
                else
                {
                    process.Start();
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
