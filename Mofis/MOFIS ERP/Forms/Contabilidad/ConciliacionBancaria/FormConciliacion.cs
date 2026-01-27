using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MOFIS_ERP.Classes;
using MOFIS_ERP.Forms;
using MOFIS_ERP.Forms.Contabilidad;

namespace MOFIS_ERP.Forms.Contabilidad.ConciliacionBancaria
{
    public partial class FormConciliacion : Form
    {
        private FormMain formPrincipal;
        private PythonService pythonService;
        private string scriptPythonPath;
        private string configFilePath;
        private string fullPathBanco;
        private string fullPathContable;
        private Dictionary<string, Control> paramControls = new Dictionary<string, Control>();
        private bool isSideMenuOpen = false;
        
        // Colores (pueden usarse para lógica dinámica si es necesario)
        private readonly Color ColorPrimario = Color.FromArgb(0, 120, 212);
        private readonly Color ColorExito = Color.FromArgb(16, 124, 16);
        private readonly Color ColorError = Color.FromArgb(168, 0, 0);

        public FormConciliacion(FormMain formMain)
        {
            this.formPrincipal = formMain;
            this.pythonService = new PythonService();
            
            InitializeComponent();
            ConfigurarFormulario();
            
            // HARDCODED SCRIPT PATH (NOT IN UI)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            scriptPythonPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\Conciliacion\conciliacion_bancaria.py"));
            
            // Ruta de configuración para persistencia
            configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config_conciliacion.txt");
            
            // Intentar cargar directorio persistido
            CargarConfiguracion();
            
            // Si falló la persistencia, intentar fallback al script dir
            if (string.IsNullOrEmpty(txtDirectorioTrabajo.Text) && File.Exists(scriptPythonPath))
            {
                string defaultWorkDir = Path.GetDirectoryName(scriptPythonPath);
                if (Directory.Exists(defaultWorkDir))
                {
                    txtDirectorioTrabajo.Text = defaultWorkDir;
                    CargarFideicomisos();
                }
            }

            if (cmbMoneda.Items.Count > 0)
                cmbMoneda.SelectedIndex = 0;

            InitializeParameters();
            CargarConfiguracion(); // Cargar de nuevo para sobreescribir con params guardados
            
            // Ajuste inicial del panel lateral
            pnlSide.Visible = false;
        }

        private void InitializeParameters()
        {
            flowParam.Controls.Clear();
            
            // Categoría: Tolerancias
            AddParamHeader("💰 Tolerancias");
            AddNumericParam("tol_exacta", "Exacta ($)", 0.01m, 0, 100, 2);
            AddNumericParam("tol_agrup", "Agrupación ($)", 1.00m, 0, 1000, 2);
            AddNumericParam("tol_parcial", "Parcial (%)", 0.02m, 0, 1, 2);

            // Categoría: Ventanas
            AddParamHeader("📅 Ventanas (Días)");
            AddNumericParam("ven_exacta", "Exacta", 10, 0, 365);
            AddNumericParam("ven_agrup", "Agrupación", 20, 0, 365);
            AddNumericParam("ven_flex", "Flexible", 30, 0, 365);
            AddNumericParam("ven_comis", "Comisiones", 45, 0, 365);

            // Categoría: Similitud
            AddParamHeader("🎯 Umbrales Similitud");
            AddNumericParam("umb_baja", "Baja (0-1)", 0.05m, 0, 1, 2);
            AddNumericParam("umb_media", "Media (0-1)", 0.20m, 0, 1, 2);
            AddNumericParam("umb_alta", "Alta (0-1)", 0.40m, 0, 1, 2);

            // Categoría: Avanzado
            AddParamHeader("🔧 Avanzado");
            AddBoolParam("solo_monto", "Permitir solo monto", true);
            AddBoolParam("usar_fechas", "Desambiguar con fechas", true);
            AddBoolParam("especiales", "Detectar casos especiales", true);
            AddBoolParam("profesional", "Formato profesional", true);
            AddBoolParam("segunda_pasada", "Ejecutar 2da pasada", true);
            AddBoolParam("exhaustiva", "Búsqueda exhaustiva", true);

            // Categoría: Comisiones
            AddParamHeader("💵 Comisiones");
            AddNumericParam("comision_usd", "Comisión USD ($)", 7.00m, 0, 100, 2);
            AddBoolParam("det_comis", "Detectar comisiones", true);

            // Categoría: Rendimiento
            AddParamHeader("⚡ Rendimiento");
            AddNumericParam("max_partidas", "Max Partidas Agrup.", 30, 1, 500);
            AddNumericParam("max_comb", "Max Comb. x Búsqueda", 10000, 100, 1000000);
            AddNumericParam("umb_exh", "Umbral Exhaustiva", 25, 1, 100);
            AddNumericParam("max_exh", "Max Comb. Exhaust.", 100000, 100, 5000000);
        }

        private void AddParamHeader(string text)
        {
            Label lbl = new Label { Text = text, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 15, 0, 5), AutoSize = true, ForeColor = Color.FromArgb(0, 120, 212) };
            flowParam.Controls.Add(lbl);
        }

        private void AddNumericParam(string key, string label, decimal def, decimal min, decimal max, int decimals = 0)
        {
            Panel p = new Panel { Width = 240, Height = 25, Margin = new Padding(0, 2, 0, 2) };
            Label l = new Label { Text = label, AutoSize = false, Width = 140, Location = new Point(0, 3), Font = new Font("Segoe UI", 9) };
            NumericUpDown n = new NumericUpDown { Name = key, DecimalPlaces = decimals, Width = 90, Location = new Point(145, 0), Font = new Font("Segoe UI", 9) };
            
            // IMPORTANTE: Establecer límites ANTES del valor para evitar ArgumentOutOfRangeException
            n.Minimum = min;
            n.Maximum = max;
            n.Value = def;

            p.Controls.Add(l);
            p.Controls.Add(n);
            flowParam.Controls.Add(p);
            paramControls[key] = n;
        }

        private void AddBoolParam(string key, string label, bool def)
        {
            CheckBox c = new CheckBox { Name = key, Text = label, Checked = def, Width = 240, Margin = new Padding(0, 2, 0, 2), Font = new Font("Segoe UI", 9) };
            flowParam.Controls.Add(c);
            paramControls[key] = c;
        }

        private void ConfigurarFormulario()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        private void CargarConfiguracion()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath);
                    if (lines.Length > 0 && Directory.Exists(lines[0].Trim()))
                    {
                        txtDirectorioTrabajo.Text = lines[0].Trim();
                        CargarFideicomisos();
                    }

                    // Cargar parámetros si existen (formato key=value)
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var parts = lines[i].Split('=');
                        if (parts.Length == 2 && paramControls.ContainsKey(parts[0]))
                        {
                            var ctrl = paramControls[parts[0]];
                            if (ctrl is NumericUpDown n)
                            {
                                decimal val = decimal.Parse(parts[1]);
                                if (val < n.Minimum) val = n.Minimum;
                                if (val > n.Maximum) val = n.Maximum;
                                n.Value = val;
                            }
                            else if (ctrl is CheckBox c) c.Checked = bool.Parse(parts[1]);
                        }
                    }
                }
            }
            catch { /* Ignorar errores de carga */ }
        }

        private void GuardarConfiguracion()
        {
            try
            {
                List<string> lines = new List<string>();
                lines.Add(txtDirectorioTrabajo.Text);
                
                foreach (var kvp in paramControls)
                {
                    if (kvp.Value is NumericUpDown n) lines.Add($"{kvp.Key}={n.Value}");
                    else if (kvp.Value is CheckBox c) lines.Add($"{kvp.Key}={c.Checked}");
                }

                File.WriteAllLines(configFilePath, lines.ToArray());
            }
            catch { /* Ignorar errores de guardado */ }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            formPrincipal.CargarContenidoPanel(new FormDashboardContabilidad(formPrincipal));
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarFideicomisos();
        }

        private void BtnBuscarDir_Click(object sender, EventArgs e)
        {
            // Usando OpenFileDialog para carpetas (según requerimiento del usuario)
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.ValidateNames = false;
                ofd.CheckFileExists = false;
                ofd.CheckPathExists = true;
                ofd.FileName = "Seleccione cualquier archivo en la carpeta deseada";
                ofd.Title = "Seleccionar Directorio de Trabajo";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtDirectorioTrabajo.Text = Path.GetDirectoryName(ofd.FileName);
                    GuardarConfiguracion();
                    CargarFideicomisos();
                }
            }
        }

        private void CargarFideicomisos()
        {
            cmbFideicomisos.Items.Clear();
            string path = txtDirectorioTrabajo.Text;
            
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;

            try
            {
                var dirs = Directory.GetDirectories(path)
                    .Select(d => new DirectoryInfo(d))
                    .Where(d => IsFideicomisoFolder(d))
                    .Select(d => d.Name)
                    .ToList();

                dirs.Sort(new NaturalSortComparer());
                cmbFideicomisos.Items.AddRange(dirs.ToArray());

                if (cmbFideicomisos.Items.Count > 0)
                    cmbFideicomisos.SelectedIndex = 0;
                else
                    ResetEstados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error leyendo directorios: " + ex.Message);
            }
        }

        private bool IsFideicomisoFolder(DirectoryInfo dir)
        {
            if (dir.Name.StartsWith(".") || dir.Name == "__pycache__") return false;
            return true; 
        }

        private void CmbFideicomisos_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidarFideicomiso();
        }

        private void ValidarFideicomiso()
        {
            if (cmbFideicomisos.SelectedItem == null)
            {
                ResetEstados();
                return;
            }

            string fideicomiso = cmbFideicomisos.SelectedItem.ToString();
            string path = Path.Combine(txtDirectorioTrabajo.Text, fideicomiso);

            string pathBancoDir = Path.Combine(path, "Archivos Banco");
            string pathContableDir = Path.Combine(path, "Archivos Libro Contable");

            // Auto-detect files if textboxes are empty or if we just changed fideicomiso
            AutoDetectFiles(pathBancoDir, pathContableDir);

            bool bancoOk = !string.IsNullOrEmpty(fullPathBanco) && File.Exists(fullPathBanco);
            bool contableOk = !string.IsNullOrEmpty(fullPathContable) && File.Exists(fullPathContable);

            ActualizarEstado(lblEstadoBanco, bancoOk);
            ActualizarEstado(lblEstadoContable, contableOk);

            btnEjecutar.Enabled = bancoOk && contableOk && File.Exists(scriptPythonPath);
            btnEjecutar.BackColor = btnEjecutar.Enabled ? ColorPrimario : Color.Gray;
        }

        private void AutoDetectFiles(string pathBancoDir, string pathContableDir)
        {
            if (Directory.Exists(pathBancoDir))
            {
                var files = Directory.GetFiles(pathBancoDir, "*.*").Where(IsExcelOrCsv).ToList();
                if (files.Count == 1)
                {
                    fullPathBanco = files[0];
                    txtArchivoBanco.Text = Path.GetFileName(fullPathBanco);
                }
                else if (files.Count == 0)
                {
                    fullPathBanco = "";
                    txtArchivoBanco.Text = "";
                }
            }

            if (Directory.Exists(pathContableDir))
            {
                var files = Directory.GetFiles(pathContableDir, "*.*").Where(IsExcelOrCsv).ToList();
                if (files.Count == 1)
                {
                    fullPathContable = files[0];
                    txtArchivoContable.Text = Path.GetFileName(fullPathContable);
                }
                else if (files.Count == 0)
                {
                    fullPathContable = "";
                    txtArchivoContable.Text = "";
                }
            }
        }

        private void BtnSelBanco_Click(object sender, EventArgs e)
        {
            if (cmbFideicomisos.SelectedItem == null) return;
            string fideicomiso = cmbFideicomisos.SelectedItem.ToString();
            string initDir = Path.Combine(txtDirectorioTrabajo.Text, fideicomiso, "Archivos Banco");
            
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Excel y CSV|*.xlsx;*.xls;*.csv";
                if (Directory.Exists(initDir)) ofd.InitialDirectory = initDir;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    fullPathBanco = ofd.FileName;
                    txtArchivoBanco.Text = Path.GetFileName(fullPathBanco);
                    ValidarFideicomiso();
                }
            }
        }

        private void BtnSelContable_Click(object sender, EventArgs e)
        {
            if (cmbFideicomisos.SelectedItem == null) return;
            string fideicomiso = cmbFideicomisos.SelectedItem.ToString();
            string initDir = Path.Combine(txtDirectorioTrabajo.Text, fideicomiso, "Archivos Libro Contable");

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Excel y CSV|*.xlsx;*.xls;*.csv";
                if (Directory.Exists(initDir)) ofd.InitialDirectory = initDir;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    fullPathContable = ofd.FileName;
                    txtArchivoContable.Text = Path.GetFileName(fullPathContable);
                    ValidarFideicomiso();
                }
            }
        }

        private bool IsExcelOrCsv(string f)
        {
            string ext = Path.GetExtension(f).ToLower();
            return ext == ".xlsx" || ext == ".xls" || ext == ".csv";
        }

        private void ActualizarEstado(Label lbl, bool valido)
        {
            string baseText = lbl.Text.Length > 2 ? lbl.Text.Substring(2) : lbl.Text;
            if (valido)
            {
                lbl.Text = "✅ " + baseText;
                lbl.ForeColor = ColorExito;
            }
            else
            {
                lbl.Text = "❌ " + baseText;
                lbl.ForeColor = ColorError;
            }
        }

        private void ResetEstados()
        {
            lblEstadoContable.Text = "⚫ Archivos Contable";
            lblEstadoContable.ForeColor = Color.Gray;
            fullPathBanco = "";
            fullPathContable = "";
            txtArchivoBanco.Text = "";
            txtArchivoContable.Text = "";
            btnEjecutar.Enabled = false;
            btnEjecutar.BackColor = Color.Gray;
        }

        private void BtnEjecutar_Click(object sender, EventArgs e)
        {
            if (!pythonService.IsPythonInstalled())
            {
                MessageBox.Show("No se detectó Python en el sistema. Por favor instálalo o agrégalo al PATH.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string script = scriptPythonPath;
                string workDir = txtDirectorioTrabajo.Text;
                string fideicomiso = cmbFideicomisos.SelectedItem.ToString();
                string bankFile = fullPathBanco;
                string ledgerFile = fullPathContable;
                string currency = cmbMoneda.SelectedItem?.ToString() ?? "DOP";
                
                string fullWorkDir = Path.Combine(workDir, fideicomiso);
                
                // Recopilar parámetros
                var parameters = new Dictionary<string, object>();
                foreach (var kvp in paramControls)
                {
                    if (kvp.Value is NumericUpDown n) parameters[kvp.Key] = n.Value;
                    else if (kvp.Value is CheckBox c) parameters[kvp.Key] = c.Checked;
                }

                // Guardar auto antes de ejecutar
                GuardarConfiguracion();

                rtbConsola.Clear();
                AppendLog("Iniciando motor de conciliación...");
                AppendLog("POR FAVOR ESPERE: Este proceso puede tardar unos minutos (No cierre el programa).");
                
                btnEjecutar.Enabled = false;
                btnEjecutar.Text = "EJECUTANDO...";
                btnEjecutar.BackColor = Color.Gray;

                pythonService.ExecuteConciliacion(script, fullWorkDir, bankFile, ledgerFile, currency, parameters, 
                    (line) => {
                        AppendLog(line);
                    },
                    (exitCode) => {
                        this.Invoke(new Action(() => {
                            btnEjecutar.Enabled = true;
                            btnEjecutar.Text = "EJECUTAR CONCILIACIÓN";
                            btnEjecutar.BackColor = ColorPrimario;
                            
                            if (exitCode == 0)
                            {
                                AppendLog("PROCESO FINALIZADO CON ÉXITO.");
                                string resultPath = Path.Combine(fullWorkDir, "Resultado");
                                MessageBox.Show($"La conciliación ha finalizado con éxito.\n\nLos resultados se encuentran en:\n{resultPath}", "Proceso Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                AppendLog($"EL PROCESO TERMINÓ CON ERRORES (Código: {exitCode}).");
                                MessageBox.Show($"El proceso terminó con errores (Código: {exitCode}). Revise el log para más detalles.", "Error en Ejecución", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }));
                    }
                );
            }
            catch (Exception ex)
            {
                AppendLog("C# ERROR: " + ex.Message);
                btnEjecutar.Enabled = true;
                btnEjecutar.Text = "EJECUTAR CONCILIACIÓN";
                btnEjecutar.BackColor = ColorPrimario;
                MessageBox.Show($"Error al iniciar la conciliación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnToggleParam_Click(object sender, EventArgs e)
        {
            isSideMenuOpen = !isSideMenuOpen;
            pnlSide.Visible = isSideMenuOpen;
            btnToggleParam.Text = isSideMenuOpen ? "✖️ CERRAR" : "⚙️ AJUSTES";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            rtbConsola.Clear();
            txtArchivoBanco.Clear();
            txtArchivoContable.Clear();
            fullPathBanco = "";
            fullPathContable = "";
            ValidarFideicomiso();
        }

        private void AppendLog(string text)
        {
            if (this.IsDisposed) return;

            if (rtbConsola.InvokeRequired)
            {
                rtbConsola.Invoke(new Action<string>(AppendLog), text);
                return;
            }

            rtbConsola.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            rtbConsola.SelectionStart = rtbConsola.Text.Length;
            rtbConsola.ScrollToCaret();
        }

        private void lblParamTitulo_Click(object sender, EventArgs e)
        {

        }
    }
}
