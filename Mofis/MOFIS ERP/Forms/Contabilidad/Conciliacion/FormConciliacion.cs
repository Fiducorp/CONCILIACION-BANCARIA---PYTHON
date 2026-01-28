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
        private string pathUltimoResultado;
        private Dictionary<string, Control> paramControls = new Dictionary<string, Control>();
        private bool isSideMenuOpen = false;
        
        // Colores (pueden usarse para lógica dinámica si es necesario)
        private readonly Color ColorPrimario = Color.ForestGreen;
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

            if (cmbMoneda.Items.Count > 0 && cmbMoneda.SelectedIndex == -1)
            {
                cmbMoneda.SelectedIndex = 0;
                ActualizarEstiloMoneda();
            }
            else if (cmbMoneda.SelectedIndex != -1)
            {
                ActualizarEstiloMoneda();
            }
            cmbMoneda.SelectedIndexChanged += (s, e) => ValidarFideicomiso();

            InitializeParameters();
            CargarConfiguracion(); // Cargar de nuevo para sobreescribir con params guardados
            
            // Ajuste inicial del panel lateral
            pnlSide.Visible = false;

            // Eventos nuevos para el rediseño
            this.btnToggleLog.Click += new System.EventHandler(this.btnToggleLog_Click);
            this.btnMonedaDOP.Click += new System.EventHandler(this.btnMoneda_Click);
            this.btnMonedaUSD.Click += new System.EventHandler(this.btnMoneda_Click);

            // Asegurar estado inicial correcto del botón
            if (cmbCuenta.Items.Count > 0) cmbCuenta.SelectedIndex = 0;
            ValidarFideicomiso();
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

        private void btnMoneda_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string selected = btn.Text; // "DOP" o "USD"

            // Actualizar el combo oculto (fuente de verdad)
            cmbMoneda.SelectedItem = selected;

            // Actualizar estilos visuales
            ActualizarEstiloMoneda();

            // Validar estado del botón ejecutar
            ValidarFideicomiso();
        }

        private void ActualizarEstiloMoneda()
        {
            Color colorSeleccionado = Color.FromArgb(0, 114, 198);
            Color colorDesactivado = Color.FromArgb(240, 240, 240);
            Color textoSeleccionado = Color.White;
            Color textoDesactivado = Color.FromArgb(64, 64, 64);

            if (cmbMoneda.SelectedItem?.ToString() == "DOP")
            {
                btnMonedaDOP.BackColor = colorSeleccionado;
                btnMonedaDOP.ForeColor = textoSeleccionado;
                btnMonedaUSD.BackColor = colorDesactivado;
                btnMonedaUSD.ForeColor = textoDesactivado;
            }
            else
            {
                btnMonedaUSD.BackColor = colorSeleccionado;
                btnMonedaUSD.ForeColor = textoSeleccionado;
                btnMonedaDOP.BackColor = colorDesactivado;
                btnMonedaDOP.ForeColor = textoDesactivado;
            }
        }

        private void btnToggleLog_Click(object sender, EventArgs e)
        {
            pnlLog.Visible = !pnlLog.Visible;
            btnToggleLog.BackColor = pnlLog.Visible ? Color.FromArgb(0, 114, 198) : Color.FromArgb(83, 109, 122);
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
            if (cmbFideicomisos.SelectedItem != null)
            {
                string fideicomiso = cmbFideicomisos.SelectedItem.ToString();
                string path = Path.Combine(txtDirectorioTrabajo.Text, fideicomiso);
                string pathBancoDir = Path.Combine(path, "Archivos Banco");
                string pathContableDir = Path.Combine(path, "Archivos Libro Contable");
                
                AutoDetectFiles(pathBancoDir, pathContableDir);
            }
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

            bool bancoOk = !string.IsNullOrEmpty(fullPathBanco) && File.Exists(fullPathBanco) && fullPathBanco.Contains(path);
            bool contableOk = !string.IsNullOrEmpty(fullPathContable) && File.Exists(fullPathContable) && fullPathContable.Contains(path);
            bool monedaOk = cmbMoneda.SelectedItem != null;

            ActualizarEstado(lblEstadoBanco, bancoOk);
            ActualizarEstado(lblEstadoContable, contableOk);

            btnEjecutar.Enabled = bancoOk && contableOk && monedaOk && File.Exists(scriptPythonPath);
            
            // Si algo cambió, resetear el botón de "VER CONCILIACIÓN" a "EJECUTAR"
            if (btnEjecutar.Text == "VER CONCILIACIÓN")
            {
                btnEjecutar.Text = "EJECUTAR";
                pathUltimoResultado = null;
            }

            btnEjecutar.BackColor = btnEjecutar.Enabled ? ColorPrimario : Color.Gray;
        }

        private void AutoDetectFiles(string pathBancoDir, string pathContableDir)
        {
            // If the current file doesn't belong to the current fideicomiso, clear it
            if (!string.IsNullOrEmpty(fullPathBanco) && !fullPathBanco.StartsWith(Path.GetDirectoryName(pathBancoDir)))
            {
                fullPathBanco = "";
                txtArchivoBanco.Text = "";
            }
            if (!string.IsNullOrEmpty(fullPathContable) && !fullPathContable.StartsWith(Path.GetDirectoryName(pathContableDir)))
            {
                fullPathContable = "";
                txtArchivoContable.Text = "";
            }

            if (Directory.Exists(pathBancoDir))
            {
                var files = Directory.GetFiles(pathBancoDir, "*.*").Where(IsExcelOrCsv).ToList();
                if (files.Count == 1)
                {
                    fullPathBanco = files[0];
                    txtArchivoBanco.Text = Path.GetFileName(fullPathBanco);
                }
                else if (files.Count != 1 && !string.IsNullOrEmpty(fullPathBanco) && !fullPathBanco.StartsWith(pathBancoDir))
                {
                    // Only clear if it wasn't already a valid path in THIS folder
                    fullPathBanco = "";
                    txtArchivoBanco.Text = "";
                }
            }
            else
            {
                fullPathBanco = "";
                txtArchivoBanco.Text = "";
            }

            if (Directory.Exists(pathContableDir))
            {
                var files = Directory.GetFiles(pathContableDir, "*.*").Where(IsExcelOrCsv).ToList();
                if (files.Count == 1)
                {
                    fullPathContable = files[0];
                    txtArchivoContable.Text = Path.GetFileName(fullPathContable);
                }
                else if (files.Count != 1 && !string.IsNullOrEmpty(fullPathContable) && !fullPathContable.StartsWith(pathContableDir))
                {
                    fullPathContable = "";
                    txtArchivoContable.Text = "";
                }
            }
            else
            {
                fullPathContable = "";
                txtArchivoContable.Text = "";
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
                    TryAutoMatchOtherFile(fullPathBanco, true);
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
                    TryAutoMatchOtherFile(fullPathContable, false);
                    ValidarFideicomiso();
                }
            }
        }

        private bool IsExcelOrCsv(string f)
        {
            if (string.IsNullOrEmpty(f)) return false;
            string ext = Path.GetExtension(f).ToLower();
            return ext == ".xlsx" || ext == ".xls" || ext == ".csv";
        }

        private void TryAutoMatchOtherFile(string selectedFilePath, bool isBankFile)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath)) return;

                string fileName = Path.GetFileNameWithoutExtension(selectedFilePath).ToUpper();
                string fideicomiso = cmbFideicomisos.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(fideicomiso)) return;

                string targetDirName = isBankFile ? "Archivos Libro Contable" : "Archivos Banco";
                string targetPath = Path.Combine(txtDirectorioTrabajo.Text, fideicomiso, targetDirName);

                if (!Directory.Exists(targetPath)) return;

                // 1. Extraer palabras clave (Banco, Mes, Año, Números largos)
                var keywords = ExtractKeywords(fileName);
                if (keywords.Count == 0) return;

                // 2. Buscar en la carpeta de destino
                var candidates = Directory.GetFiles(targetPath, "*.*")
                    .Where(IsExcelOrCsv)
                    .ToList();

                if (candidates.Count == 0) return;

                string bestMatch = null;
                int highestScore = 0;

                foreach (var candidate in candidates)
                {
                    string candidateName = Path.GetFileNameWithoutExtension(candidate).ToUpper();
                    int score = CalculateMatchScore(candidateName, keywords);

                    if (score > highestScore)
                    {
                        highestScore = score;
                        bestMatch = candidate;
                    }
                }

                // Umbral mínimo de coincidencia (por ejemplo, al menos 2 palabras clave o una muy específica)
                if (highestScore >= 1)
                {
                    if (isBankFile)
                    {
                        fullPathContable = bestMatch;
                        txtArchivoContable.Text = Path.GetFileName(fullPathContable);
                    }
                    else
                    {
                        fullPathBanco = bestMatch;
                        txtArchivoBanco.Text = Path.GetFileName(fullPathBanco);
                    }
                }
            }
            catch { /* Ignorar errores en auto-match */ }
        }

        private List<string> ExtractKeywords(string text)
        {
            List<string> keywords = new List<string>();
            if (string.IsNullOrEmpty(text)) return keywords;

            // Bancos
            string[] banks = { "POPULAR", "BPD", "RESERVAS", "BANRESERVAS", "BHD", "BANESCO", "SCOTIA", "PROMERICA", "SANTA CRUZ", "ADEMI", "LOPEZ", "BELL", "APAP" };
            foreach (var bank in banks)
                if (text.Contains(bank)) keywords.Add(bank);

            // Meses (ES)
            string[] monthsES = { "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
            string[] monthsES_Short = { "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" };
            
            for (int i = 0; i < monthsES.Length; i++)
            {
                if (text.Contains(monthsES[i])) keywords.Add(monthsES[i]);
                else if (text.Contains(monthsES_Short[i]) && monthsES_Short[i].Length >= 3) keywords.Add(monthsES_Short[i]);
            }

            // Años (2020-2029)
            var yearMatch = System.Text.RegularExpressions.Regex.Match(text, @"202\d");
            if (yearMatch.Success) keywords.Add(yearMatch.Value);

            return keywords;
        }

        private int CalculateMatchScore(string text, List<string> keywords)
        {
            int score = 0;
            foreach (var kw in keywords)
            {
                if (text.Contains(kw)) score++;
            }
            return score;
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
            // Verificación redundante de seguridad para modo EJECUTAR
            // Solo permitimos el clic si el botón está habilitado O si estamos en modo "VER CONCILIACIÓN"
            if (!btnEjecutar.Enabled && btnEjecutar.Text != "VER CONCILIACIÓN") 
                return;

            if (btnEjecutar.Text == "VER CONCILIACIÓN" && !string.IsNullOrEmpty(pathUltimoResultado))
            {
                formPrincipal.CargarContenidoPanel(new FormVisualizarExcel(formPrincipal, pathUltimoResultado, this));
                return;
            }

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
                            btnEjecutar.Text = "EJECUTAR";
                            btnEjecutar.BackColor = ColorPrimario;
                            
                            if (exitCode == 0)
                            {
                                AppendLog("PROCESO FINALIZADO CON ÉXITO.");
                                string resultPath = Path.Combine(fullWorkDir, "Resultados");
                                
                                // Buscar el archivo creado más reciente en la carpeta de resultados
                                if (Directory.Exists(resultPath))
                                {
                                    var resultFiles = Directory.GetFiles(resultPath, "*.xlsx")
                                        .Select(f => new FileInfo(f))
                                        .OrderByDescending(f => f.LastWriteTime)
                                        .ToList();

                                    if (resultFiles.Count > 0)
                                    {
                                        pathUltimoResultado = resultFiles[0].FullName;
                                        btnEjecutar.Text = "VER CONCILIACIÓN";
                                        btnEjecutar.BackColor = Color.FromArgb(0, 120, 212); // Azul corporativo
                                    }
                                }

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
                btnEjecutar.Text = "EJECUTAR";
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
            
            // Si el botón estaba en modo "VER CONCILIACIÓN", resetearlo
            if (btnEjecutar.Text == "VER CONCILIACIÓN")
            {
                btnEjecutar.Text = "EJECUTAR";
                pathUltimoResultado = null;
            }

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

        private void btnAyuda_Click(object sender, EventArgs e)
        {
            string mensaje = "ℹ️ AYUDA Y RECOMENDACIONES\n\n" +
                             "• ¿QUÉ HACE ESTE PROGRAMA?\n" +
                             "Automatiza la conciliación de movimientos bancarios comparando archivos de banco contra el libro contable.\n\n" +
                             "• FORMATOS SOPORTADOS\n" +
                             "Los documentos DEBEN estar en formato .xlsx (Excel) o .csv. Otros formatos no son compatibles.\n\n" +
                             "• RECOMENDACIÓN DE USO\n" +
                             "Nombre sus archivos incluyendo el nombre del banco y el mes/año (ej. POPULAR_MARZO_2024). " +
                             "Esto permite que el sistema detecte y seleccione automáticamente el archivo correspondiente al elegir uno.";

            MessageBox.Show(mensaje, "Información del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lblParamTitulo_Click(object sender, EventArgs e)
        {

        }

        private void lblTituloPrincipal_Click(object sender, EventArgs e)
        {

        }

        private void rtbConsola_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
