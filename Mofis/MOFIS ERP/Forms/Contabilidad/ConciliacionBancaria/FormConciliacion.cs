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
        
        // Colores
        private readonly Color ColorPrimario = Color.FromArgb(0, 120, 212);
        private readonly Color ColorFondo = Color.FromArgb(240, 240, 240);
        private readonly Color ColorExito = Color.FromArgb(16, 124, 16);
        private readonly Color ColorError = Color.FromArgb(168, 0, 0);

        // Controles
        private TextBox txtDirectorioTrabajo;
        private ComboBox cmbFideicomisos;
        private Label lblEstadoBanco;
        private Label lblEstadoContable;
        private Button btnEjecutar;
        private TextBox txtScriptPython;

        public FormConciliacion(FormMain formMain)
        {
            this.formPrincipal = formMain;
            this.pythonService = new PythonService();
            
            InitializeComponent();
            ConfigurarFormulario();
            CrearInterfaz();

            // Cargar ruta por defecto (asumimos que está al nivel de la carpeta Mofis)
            // Estructura: Root/Mofis/MOFIS ERP/bin/Debug/...
            // Script: Root/Conciliacion/conciliacion_bancaria_v0.9.6.py
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string defaultScriptPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\Conciliacion\conciliacion_bancaria_v0.9.6.py"));
            if (File.Exists(defaultScriptPath))
            {
                txtScriptPython.Text = defaultScriptPath;
                // Asumir directorio de trabajo en la raiz del script por defecto
                string defaultWorkDir = Path.GetDirectoryName(defaultScriptPath);
                if (Directory.Exists(defaultWorkDir))
                {
                    txtDirectorioTrabajo.Text = defaultWorkDir;
                    CargarFideicomisos();
                }
            }
        }

        private void ConfigurarFormulario()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = ColorFondo;
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;
        }

        private void CrearInterfaz()
        {
            Panel panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                AutoScroll = true
            };
            this.Controls.Add(panelPrincipal);

            // 1. Encabezado y Botón Volver
            Button btnVolver = new Button
            {
                Text = "← Volver",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Size = new Size(100, 35),
                Location = new Point(30, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = ColorPrimario,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderColor = ColorPrimario;
            btnVolver.Click += (s, e) => formPrincipal.CargarContenidoPanel(new FormDashboardContabilidad(formPrincipal));
            panelPrincipal.Controls.Add(btnVolver);

            Label lblTitulo = new Label
            {
                Text = "Conciliación Bancaria",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = ColorPrimario,
                AutoSize = true,
                Location = new Point(30, 70)
            };
            panelPrincipal.Controls.Add(lblTitulo);

            // 2. Panel de Configuración de Rutas
            GroupBox grpRutas = new GroupBox
            {
                Text = "Configuración",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(700, 180),
                Location = new Point(30, 130),
                BackColor = Color.White
            };
            panelPrincipal.Controls.Add(grpRutas);

            // Script Python
            Label lblScript = new Label { Text = "Script Python:", Font = new Font("Segoe UI", 10, FontStyle.Regular), Location = new Point(20, 35), AutoSize = true };
            txtScriptPython = new TextBox { Location = new Point(20, 60), Width = 550, Font = new Font("Segoe UI", 10), ReadOnly = true, BackColor = Color.WhiteSmoke };
            Button btnBuscarScript = CrearBotonBusqueda("...", new Point(580, 59));
            btnBuscarScript.Click += BtnBuscarScript_Click;
            
            grpRutas.Controls.Add(lblScript);
            grpRutas.Controls.Add(txtScriptPython);
            grpRutas.Controls.Add(btnBuscarScript);

            // Directorio Trabajo
            Label lblDir = new Label { Text = "Directorio de Trabajo (Fideicomisos):", Font = new Font("Segoe UI", 10, FontStyle.Regular), Location = new Point(20, 100), AutoSize = true };
            txtDirectorioTrabajo = new TextBox { Location = new Point(20, 125), Width = 550, Font = new Font("Segoe UI", 10), ReadOnly = true, BackColor = Color.WhiteSmoke };
            Button btnBuscarDir = CrearBotonBusqueda("...", new Point(580, 124));
            btnBuscarDir.Click += BtnBuscarDir_Click;

            grpRutas.Controls.Add(lblDir);
            grpRutas.Controls.Add(txtDirectorioTrabajo);
            grpRutas.Controls.Add(btnBuscarDir);

            // 3. Selección y Ejecución
            GroupBox grpEjecucion = new GroupBox
            {
                Text = "Ejecución",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(700, 250),
                Location = new Point(30, 330),
                BackColor = Color.White
            };
            panelPrincipal.Controls.Add(grpEjecucion);

            Label lblFideicomiso = new Label { Text = "Seleccionar Fideicomiso:", Font = new Font("Segoe UI", 10, FontStyle.Regular), Location = new Point(20, 40), AutoSize = true };
            cmbFideicomisos = new ComboBox { Location = new Point(20, 65), Width = 400, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFideicomisos.SelectedIndexChanged += CmbFideicomisos_SelectedIndexChanged;
            Button btnRefrescar = new Button { Text = "↻", Location = new Point(430, 64), Size = new Size(40, 25), FlatStyle = FlatStyle.Flat };
            btnRefrescar.Click += (s, e) => CargarFideicomisos();

            grpEjecucion.Controls.Add(lblFideicomiso);
            grpEjecucion.Controls.Add(cmbFideicomisos);
            grpEjecucion.Controls.Add(btnRefrescar);

            // Indicadores de estado
            lblEstadoBanco = CrearLabelEstado("Archivos Banco", new Point(20, 110));
            lblEstadoContable = CrearLabelEstado("Archivos Contable", new Point(200, 110));
            
            grpEjecucion.Controls.Add(lblEstadoBanco);
            grpEjecucion.Controls.Add(lblEstadoContable);

            // Botón Ejecutar
            btnEjecutar = new Button
            {
                Text = "EJECUTAR CONCILIACIÓN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(250, 50),
                Location = new Point(20, 160),
                BackColor = ColorPrimario,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEjecutar.FlatAppearance.BorderSize = 0;
            btnEjecutar.Click += BtnEjecutar_Click;
            grpEjecucion.Controls.Add(btnEjecutar);
        }

        private Button CrearBotonBusqueda(string texto, Point ubicacion)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = ubicacion,
                Size = new Size(40, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(224, 224, 224),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Label CrearLabelEstado(string texto, Point ubicacion)
        {
            return new Label
            {
                Text = "⚫ " + texto,
                Location = ubicacion,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Gray
            };
        }

        private void BtnBuscarScript_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Python Script (*.py)|*.py";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtScriptPython.Text = ofd.FileName;
                }
            }
        }

        private void BtnBuscarDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtDirectorioTrabajo.Text = fbd.SelectedPath;
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
                // Obtener directorios que parezcan fideicomisos
                var dirs = Directory.GetDirectories(path)
                    .Select(d => new DirectoryInfo(d))
                    .Where(d => IsFideicomisoFolder(d))
                    .Select(d => d.Name)
                    .ToList();

                // ORDENAMIENTO NATURAL
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
            // Criterio: Debe tener carpeta 'Archivos Banco' o 'Archivos Libro Contable'
            // O simplemente ser una carpeta que no sea del sistema (.git, etc)
            if (dir.Name.StartsWith(".") || dir.Name == "__pycache__") return false;
            
            bool tieneBanco = dir.GetDirectories("Archivos Banco").Any();
            bool tieneContable = dir.GetDirectories("Archivos Libro Contable").Any();
            
            // Relajar criterio: si tiene cualquiera de las dos, o si es una carpeta "candidata"
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

            string pathBanco = Path.Combine(path, "Archivos Banco");
            string pathContable = Path.Combine(path, "Archivos Libro Contable");

            bool existeBanco = Directory.Exists(pathBanco) && Directory.GetFiles(pathBanco, "*.*").Any(IsExcelOrCsv);
            bool existeContable = Directory.Exists(pathContable) && Directory.GetFiles(pathContable, "*.*").Any(IsExcelOrCsv);

            ActualizarEstado(lblEstadoBanco, existeBanco);
            ActualizarEstado(lblEstadoContable, existeContable);

            btnEjecutar.Enabled = existeBanco && existeContable && !string.IsNullOrEmpty(txtScriptPython.Text);
            btnEjecutar.BackColor = btnEjecutar.Enabled ? ColorPrimario : Color.Gray;
        }

        private bool IsExcelOrCsv(string f)
        {
            string ext = Path.GetExtension(f).ToLower();
            return ext == ".xlsx" || ext == ".xls" || ext == ".csv";
        }

        private void ActualizarEstado(Label lbl, bool valido)
        {
            if (valido)
            {
                lbl.Text = "✅ " + lbl.Text.Substring(2);
                lbl.ForeColor = ColorExito;
            }
            else
            {
                lbl.Text = "❌ " + lbl.Text.Substring(2);
                lbl.ForeColor = ColorError;
            }
        }

        private void ResetEstados()
        {
            lblEstadoBanco.Text = "⚫ Archivos Banco";
            lblEstadoBanco.ForeColor = Color.Gray;
            lblEstadoContable.Text = "⚫ Archivos Contable";
            lblEstadoContable.ForeColor = Color.Gray;
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
                string script = txtScriptPython.Text;
                string workDir = txtDirectorioTrabajo.Text;
                string fideicomiso = cmbFideicomisos.SelectedItem.ToString();

                // Ejecutar el script
                // NOTA: Como el script original es un menú interactivo, idealmente deberíamos modificarlo 
                // para aceptar argumentos y evitar el menú, PERO por ahora lo lanzaremos tal cual,
                // asegurando que se abra en el directorio correcto. 
                // El usuario tendrá que seleccionar la opción en la consola que se abre.
                
                // MEJORA FUTURA: Pasar argumentos al script python para automatizar la selección.
                // Por ahora el usuario seleccionará "Manual" en la consola.
                
                pythonService.ExecuteConciliacion(script, workDir, fideicomiso);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar la conciliación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
