using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using MOFIS_ERP.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    public partial class FormBusquedaAvanzadaAuditoria : Form
    {
        private FormMain formPrincipal;

        // Ruta del logo para reportes
        private string rutaLogo = Path.Combine(Application.StartupPath, "Resources", "MOFIS ERP -LOGO.png");

        // Colores corporativos
        private readonly Color colorCorporativo = Color.FromArgb(0, 120, 212);

        // Cache de datos
        private DataTable dtAuditoriaCompleta = null;
        private List<string> cacheModulos = null;
        private List<string> cacheAcciones = null;
        private List<string> cacheCategorias = null;

        // Timer para búsqueda con delay
        private System.Windows.Forms.Timer busquedaTimer;

        // Constructor
        public FormBusquedaAvanzadaAuditoria(FormMain formMain)
        {
            InitializeComponent();
            formPrincipal = formMain;

            ConfigurarFormulario();

            this.Load += async (s, e) =>
            {
                await CargarDatosInicialesAsync();
            };
        }

        // ============================================================
        // CONFIGURACIÓN DEL FORMULARIO
        // ============================================================
        private void ConfigurarFormulario()
        {
            // Configurar botón volver
            // ConfigurarBoton(btnVolver, colorCorporativo, Color.White);

            // Configurar botones del panel inferior
            ConfigurarBoton(btnLimpiar, Color.FromArgb(108, 117, 125), Color.White);
            ConfigurarBoton(btnExportarPDF, Color.FromArgb(220, 53, 69), Color.White);
            ConfigurarBoton(btnExportarExcel, Color.FromArgb(34, 139, 34), Color.White);
            ConfigurarBoton(btnCargarMasDatos, colorCorporativo, Color.White);

            //// Eventos Tap Buscar por Usuario ////

            // Configurar placeholder para txtBusquedaRapida
            ConfigurarPlaceholder(txtBusquedaRapida, "Buscar...");

            // Configurar DataGridView de la pestaña Por Usuario
            ConfigurarDataGridViewUsuario();

            // Configurar timer de búsqueda
            busquedaTimer = new System.Windows.Forms.Timer();
            busquedaTimer.Interval = 300; // 300ms de delay
            busquedaTimer.Tick += (s, e) =>
            {
                busquedaTimer.Stop();
                AplicarFiltrosUsuario();
            };

            // Eventos de cambio en filtros (búsqueda en tiempo real)
            cmbUsuario.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            dtpDesde.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            dtpHasta.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            cmbAccion.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbCategoria.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbModulo.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            chkAgruparSesiones.CheckedChanged += (s, e) => IniciarBusquedaConDelay();

            txtBusquedaRapida.TextChanged += (s, e) =>
            {
                busquedaTimer.Stop();
                busquedaTimer.Start();
            };

            // Configurar fechas por defecto
            dtpDesde.Value = DateTime.Now.AddMonths(-1);
            dtpHasta.Value = DateTime.Now;

            // Eventos de botones
            btnLimpiar.Click += BtnLimpiar_Click;
            btnExportarExcel.Click += BtnExportarExcel_Click;
            btnExportarPDF.Click += BtnExportarPDF_Click;
            btnCargarMasDatos.Click += BtnCargarMasDatos_Click;

            // Evento de doble clic en DataGridView
            dgvResultadosUsuario.DoubleClick += DgvResultados_DoubleClick;

            // ============================================================
            // CONFIGURACIÓN PESTAÑA POR CATEGORÍA
            // ============================================================

            // Configurar DataGridView de la pestaña Por Categoría
            ConfigurarDataGridViewCategoria();

            // Eventos de cambio en filtros - Pestaña Por Categoría
            cmbCategoriaFiltro.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            dtpDesdeCategoria.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            dtpHastaCategoria.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            cmbUsuarioCategoria.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbModuloCategoria.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbAccionCategoria.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();

            // Evento de doble clic en DataGridView
            dgvResultadosCategoria.DoubleClick += DgvResultadosCategoria_DoubleClick;

            // Configurar fechas por defecto - Categoría
            dtpDesdeCategoria.Value = DateTime.Now.AddMonths(-1);
            dtpHastaCategoria.Value = DateTime.Now;

            // ============================================================
            // CONFIGURACIÓN PESTAÑA POR MÓDULO
            // ============================================================

            // Configurar DataGridView de la pestaña Por Módulo
            ConfigurarDataGridViewModulo();

            // Eventos de cambio en filtros - Pestaña Por Módulo
            cmbCategoriaModulo.SelectedIndexChanged += CmbCategoriaModulo_SelectedIndexChanged; // Cascada
            cmbModuloFiltro.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbFormularioModulo.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            dtpDesdeModulo.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            dtpHastaModulo.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            cmbUsuarioModulo.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbAccionModulo.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();

            // Evento de doble clic en DataGridView
            dgvResultadosModulo.DoubleClick += DgvResultadosModulo_DoubleClick;

            // Configurar fechas por defecto - Módulo
            dtpDesdeModulo.Value = DateTime.Now.AddMonths(-1);
            dtpHastaModulo.Value = DateTime.Now;

            // ============================================================
            // CONFIGURACIÓN PESTAÑA POR ACCIÓN
            // ============================================================
            ConfigurarDataGridViewAccion();

            // Eventos de cambio en filtros - Pestaña Por Acción
            cmbAccionFiltro.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            dtpDesdeAccion.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            dtpHastaAccion.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            cmbUsuarioAccion.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbCategoriaAccion.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbModuloAccion.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();

            // Evento de doble clic en DataGridView
            dgvResultadosAccion.DoubleClick += DgvResultadosAccion_DoubleClick;

            // Configurar fechas por defecto - Acción
            dtpDesdeAccion.Value = DateTime.Now.AddMonths(-1);
            dtpHastaAccion.Value = DateTime.Now;

            // ============================================================
            // CONFIGURACIÓN PESTAÑA POR HORARIO
            // ============================================================
            ConfigurarDataGridViewHorario();

            // Eventos de cambio en filtros - Pestaña Por Horario
            dtpFechaHorario.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            dtpHoraInicio.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            dtpHoraFin.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            cmbUsuarioHorario.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbCategoriaHorario.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbModuloHorario.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbAccionHorario.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            chkIncluirFinDeSemana.CheckedChanged += (s, e) => IniciarBusquedaConDelay();
            chkSoloFueraOficina.CheckedChanged += (s, e) => IniciarBusquedaConDelay();

            // Evento de doble clic en DataGridView
            dgvResultadosHorario.DoubleClick += DgvResultadosHorario_DoubleClick;

            // Configurar valores por defecto - Horario
            dtpFechaHorario.Value = DateTime.Now;
            dtpHoraInicio.Value = DateTime.Today.AddHours(0); // 00:00
            dtpHoraFin.Value = DateTime.Today.AddHours(23).AddMinutes(59); // 23:59

            // ============================================================
            // CONFIGURACIÓN PESTAÑA POR IP/MÁQUINA
            // ============================================================
            ConfigurarDataGridViewIP();

            // Eventos de cambio en filtros - Pestaña Por IP
            txtDireccionIP.TextChanged += (s, e) => IniciarBusquedaConDelay();
            txtNombreMaquina.TextChanged += (s, e) => IniciarBusquedaConDelay();
            dtpDesdeIP.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            dtpHastaIP.ValueChanged += (s, e) => IniciarBusquedaConDelay();
            cmbUsuarioIP.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbCategoriaIP.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbModuloIP.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            cmbAccionIP.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            chkAgruparPorMaquina.CheckedChanged += (s, e) => IniciarBusquedaConDelay();

            // Evento de doble clic en DataGridView
            dgvResultadosIP.DoubleClick += DgvResultadosIP_DoubleClick;

            // Configurar fechas por defecto - IP
            dtpDesdeIP.Value = DateTime.Now.AddMonths(-1);
            dtpHastaIP.Value = DateTime.Now;

            // ============================================================
            // CONFIGURACIÓN PESTAÑA POR REGISTRO
            // ============================================================
            ConfigurarDataGridViewRegistro();

            // Eventos de cambio en filtros - Pestaña Por Registro
            cmbTipoBusqueda.SelectedIndexChanged += CmbTipoBusqueda_SelectedIndexChanged;
            txtValorBusqueda.TextChanged += (s, e) => IniciarBusquedaConDelay();
            cmbModuloRegistro.SelectedIndexChanged += (s, e) => IniciarBusquedaConDelay();
            dtpDesdeRegistro.ValueChanged += (s, e) => { if (dtpDesdeRegistro.Checked) IniciarBusquedaConDelay(); };
            dtpHastaRegistro.ValueChanged += (s, e) => { if (dtpHastaRegistro.Checked) IniciarBusquedaConDelay(); };
            chkVistaTimeline.CheckedChanged += (s, e) => IniciarBusquedaConDelay();
            chkSoloFinesSemana.CheckedChanged += (s, e) => IniciarBusquedaConDelay();
            rbSinFiltroHorario.CheckedChanged += RbHorario_CheckedChanged;
            rbDentroHorario.CheckedChanged += RbHorario_CheckedChanged;
            rbFueraHorario.CheckedChanged += RbHorario_CheckedChanged;
            dtpHoraInicioRegistro.ValueChanged += (s, e) => { if (dtpHoraInicioRegistro.Enabled) IniciarBusquedaConDelay(); };
            dtpHoraFinRegistro.ValueChanged += (s, e) => { if (dtpHoraFinRegistro.Enabled) IniciarBusquedaConDelay(); };

            // Configurar valores por defecto de horario
            dtpHoraInicioRegistro.Value = DateTime.Today.AddHours(0);
            dtpHoraFinRegistro.Value = DateTime.Today.AddHours(23).AddMinutes(59);

            // Por defecto, "Personalizado" está seleccionado y los controles habilitados
            rbSinFiltroHorario.Checked = true;
            dtpHoraInicioRegistro.Enabled = true;
            dtpHoraFinRegistro.Enabled = true;
            lblHoraInicioRegistro.Enabled = true;
            lblHoraFinRegistro.Enabled = true;

            // Agregar ToolTips explicativos
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(dtpHoraInicioRegistro, "Hora de inicio del rango de búsqueda");
            toolTip.SetToolTip(dtpHoraFinRegistro, "Hora de fin del rango de búsqueda");
            toolTip.SetToolTip(rbDentroHorario, "Buscar solo dentro de horario de oficina\n(L-J: 8:00-18:00, V: 8:00-17:00)");
            toolTip.SetToolTip(rbFueraHorario, "Buscar solo fuera de horario de oficina\n(L-J: fuera de 8:00-18:00, V: fuera de 8:00-17:00)");
            toolTip.SetToolTip(chkSoloFinesSemana, "Buscar solo sábados y domingos");

            // ToolTip para cmbTipoBusqueda
            toolTip.SetToolTip(cmbTipoBusqueda,
            "• AuditoriaID: Busca por ID de registro de auditoría\n" +
            "• UsuarioID: Busca todas las acciones de un usuario\n" +
            "• RolID/PermisoID/etc: Busca en detalles\n" +
            "• Otro ID: Búsqueda genérica en TODOS los campos");

            // NO llenar el ComboBox aquí - se llenará en CargarDatosInicialesAsync

            // Evento de doble clic en DataGridView
            dgvResultadosRegistro.DoubleClick += DgvResultadosRegistro_DoubleClick;

            // Configurar fechas por defecto - Registro (sin marcar por defecto)
            dtpDesdeRegistro.Value = DateTime.Now.AddMonths(-1);
            dtpHastaRegistro.Value = DateTime.Now;
            dtpDesdeRegistro.Checked = false;
            dtpHastaRegistro.Checked = false;
        }

        // ============================================================
        // MÉTODOS DE CONFIGURACIÓN GENERAL
        // ============================================================

        // Configurar estilo de botones
        private void ConfigurarBoton(Button btn, Color backColor, Color foreColor)
        {
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;

            Color hoverColor = ControlPaint.Dark(backColor, 0.1f);
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = backColor;
        }

        // Configurar placeholder en TextBox
        private void ConfigurarPlaceholder(TextBox textBox, string placeholder)
        {
            Color placeholderColor = Color.Gray;
            Color normalColor = Color.Black;

            textBox.Text = placeholder;
            textBox.ForeColor = placeholderColor;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = normalColor;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = placeholderColor;
                }
            };
        }

        // Cambiar label según tipo de búsqueda seleccionado en pestaña "Por Registro"
        private void CmbTipoBusqueda_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Actualizar el label según el tipo seleccionado
            if (cmbTipoBusqueda.SelectedIndex > 0)
            {
                string tipoSeleccionado = cmbTipoBusqueda.SelectedItem.ToString();

                if (tipoSeleccionado.Contains("AuditoriaID"))
                {
                    lblValorBusqueda.Text = "AuditoriaID:";
                }
                else if (tipoSeleccionado.Contains("UsuarioID"))
                {
                    lblValorBusqueda.Text = "UsuarioID:";
                }
                else if (tipoSeleccionado.Contains("RolID"))
                {
                    lblValorBusqueda.Text = "RolID:";
                }
                else if (tipoSeleccionado.Contains("ProveedorID"))
                {
                    lblValorBusqueda.Text = "ProveedorID:";
                }
                else if (tipoSeleccionado.Contains("FacturaID"))
                {
                    lblValorBusqueda.Text = "FacturaID:";
                }
                else if (tipoSeleccionado.Contains("CuentaID"))
                {
                    lblValorBusqueda.Text = "CuentaID:";
                }
                else
                {
                    lblValorBusqueda.Text = "ID:";
                }
            }
            else
            {
                lblValorBusqueda.Text = "Ingrese ID:";
            }

            // Limpiar el textbox
            txtValorBusqueda.Clear();
        }

        // Manejar cambio en selección de RadioButtons de horario en pestaña "Por Registro"
        private void RbHorario_CheckedChanged(object sender, EventArgs e)
        {
            // Solo procesar si el RadioButton está siendo marcado
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked)
                return;

            if (rbSinFiltroHorario.Checked) // Ahora es "Personalizado"
            {
                // PERSONALIZADO - Habilitar controles para rango manual
                dtpHoraInicioRegistro.Enabled = true;
                dtpHoraFinRegistro.Enabled = true;
                lblHoraInicioRegistro.Enabled = true;
                lblHoraFinRegistro.Enabled = true;

                dtpHoraInicioRegistro.BackColor = Color.White;
                dtpHoraFinRegistro.BackColor = Color.White;

                // Valores por defecto (todo el día)
                dtpHoraInicioRegistro.Value = DateTime.Today.AddHours(0);
                dtpHoraFinRegistro.Value = DateTime.Today.AddHours(23).AddMinutes(59);
            }
            else if (rbDentroHorario.Checked)
            {
                // DENTRO DE HORARIO - Deshabilitar y usar horario predefinido
                dtpHoraInicioRegistro.Enabled = false;
                dtpHoraFinRegistro.Enabled = false;
                lblHoraInicioRegistro.Enabled = false;
                lblHoraFinRegistro.Enabled = false;

                dtpHoraInicioRegistro.BackColor = Color.FromArgb(240, 240, 240);
                dtpHoraFinRegistro.BackColor = Color.FromArgb(240, 240, 240);

                dtpHoraInicioRegistro.Value = DateTime.Today.AddHours(8);
                dtpHoraFinRegistro.Value = DateTime.Today.AddHours(18);
            }
            else if (rbFueraHorario.Checked)
            {
                // FUERA DE HORARIO - Deshabilitar y usar horario predefinido
                dtpHoraInicioRegistro.Enabled = false;
                dtpHoraFinRegistro.Enabled = false;
                lblHoraInicioRegistro.Enabled = false;
                lblHoraFinRegistro.Enabled = false;

                dtpHoraInicioRegistro.BackColor = Color.FromArgb(240, 240, 240);
                dtpHoraFinRegistro.BackColor = Color.FromArgb(240, 240, 240);

                dtpHoraInicioRegistro.Value = DateTime.Today.AddHours(8);
                dtpHoraFinRegistro.Value = DateTime.Today.AddHours(18);
            }

            IniciarBusquedaConDelay();
        }

        // ============================================================
        // CONFIGURACIÓN DE DATAGRIDVIEWS
        // ============================================================

        // DataGridView por Usuario
        private void ConfigurarDataGridViewUsuario()
        {
            dgvResultadosUsuario.AutoGenerateColumns = false;
            dgvResultadosUsuario.Columns.Clear();

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 200
            });

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                DataPropertyName = "Categoria",
                HeaderText = "Categoría",
                Width = 120
            });

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 150
            });

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                DataPropertyName = "Formulario",
                HeaderText = "Formulario",
                Width = 160
            });

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 120
            });

            dgvResultadosUsuario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Máquina",
                Width = 130
            });

            // Estilo de encabezados
            dgvResultadosUsuario.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvResultadosUsuario.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultadosUsuario.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvResultadosUsuario.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultadosUsuario.EnableHeadersVisualStyles = false;

            dgvResultadosUsuario.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultadosUsuario.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvResultadosUsuario.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // DataGridView por Categoría
        private void ConfigurarDataGridViewCategoria()
        {
            dgvResultadosCategoria.AutoGenerateColumns = false;
            dgvResultadosCategoria.Columns.Clear();

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 150
            });

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 180
            });

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                DataPropertyName = "Formulario",
                HeaderText = "Formulario",
                Width = 160
            });

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 200
            });

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 120
            });

            dgvResultadosCategoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Máquina",
                Width = 130
            });

            // Estilo de encabezados
            dgvResultadosCategoria.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvResultadosCategoria.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultadosCategoria.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvResultadosCategoria.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultadosCategoria.EnableHeadersVisualStyles = false;

            dgvResultadosCategoria.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultadosCategoria.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvResultadosCategoria.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // DataGridView por Módulo
        private void ConfigurarDataGridViewModulo()
        {
            dgvResultadosModulo.AutoGenerateColumns = false;
            dgvResultadosModulo.Columns.Clear();

            dgvResultadosModulo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvResultadosModulo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 150
            });

            dgvResultadosModulo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                DataPropertyName = "Formulario",
                HeaderText = "Formulario",
                Width = 180
            });

            dgvResultadosModulo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 200
            });

            dgvResultadosModulo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 250
            });

            dgvResultadosModulo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 120
            });

            dgvResultadosModulo.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Máquina",
                Width = 130
            });

            // Estilo de encabezados
            dgvResultadosModulo.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvResultadosModulo.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultadosModulo.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvResultadosModulo.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultadosModulo.EnableHeadersVisualStyles = false;

            dgvResultadosModulo.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultadosModulo.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvResultadosModulo.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // DataGridView por Acción
        private void ConfigurarDataGridViewAccion()
        {
            dgvResultadosAccion.AutoGenerateColumns = false;
            dgvResultadosAccion.Columns.Clear();

            dgvResultadosAccion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvResultadosAccion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 150
            });

            dgvResultadosAccion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                DataPropertyName = "Categoria",
                HeaderText = "Categoría",
                Width = 120
            });

            dgvResultadosAccion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 150
            });

            dgvResultadosAccion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                DataPropertyName = "Formulario",
                HeaderText = "Formulario",
                Width = 160
            });

            dgvResultadosAccion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });

            dgvResultadosAccion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 120
            });

            // Estilo de encabezados
            dgvResultadosAccion.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvResultadosAccion.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultadosAccion.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvResultadosAccion.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultadosAccion.EnableHeadersVisualStyles = false;

            dgvResultadosAccion.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultadosAccion.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvResultadosAccion.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // DataGridView por Horario
        private void ConfigurarDataGridViewHorario()
        {
            dgvResultadosHorario.AutoGenerateColumns = false;
            dgvResultadosHorario.Columns.Clear();

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 150
            });

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                DataPropertyName = "Categoria",
                HeaderText = "Categoría",
                Width = 120
            });

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 150
            });

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 180
            });

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 120
            });

            dgvResultadosHorario.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Máquina",
                Width = 130
            });

            // Estilo de encabezados
            dgvResultadosHorario.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvResultadosHorario.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultadosHorario.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvResultadosHorario.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultadosHorario.EnableHeadersVisualStyles = false;

            dgvResultadosHorario.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultadosHorario.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvResultadosHorario.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // DataGridView por IP/Máquina
        private void ConfigurarDataGridViewIP()
        {
            dgvResultadosIP.AutoGenerateColumns = false;
            dgvResultadosIP.Columns.Clear();

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 150
            });

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "Dirección IP",
                Width = 130
            });

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Nombre Máquina",
                Width = 150
            });

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                DataPropertyName = "Categoria",
                HeaderText = "Categoría",
                Width = 120
            });

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 150
            });

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 180
            });

            dgvResultadosIP.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });

            // Estilo de encabezados
            dgvResultadosIP.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvResultadosIP.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultadosIP.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvResultadosIP.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultadosIP.EnableHeadersVisualStyles = false;

            dgvResultadosIP.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultadosIP.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvResultadosIP.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // DataGridView por Registro
        private void ConfigurarDataGridViewRegistro()
        {
            dgvResultadosRegistro.AutoGenerateColumns = false;
            dgvResultadosRegistro.Columns.Clear();

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 150
            });

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 200
            });

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                DataPropertyName = "Categoria",
                HeaderText = "Categoría",
                Width = 120
            });

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 150
            });

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 250
            });

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 120
            });

            dgvResultadosRegistro.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Máquina",
                Width = 130
            });

            // Estilo de encabezados
            dgvResultadosRegistro.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvResultadosRegistro.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultadosRegistro.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvResultadosRegistro.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultadosRegistro.EnableHeadersVisualStyles = false;

            dgvResultadosRegistro.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultadosRegistro.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvResultadosRegistro.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // ============================================================
        // EVENTOS DE BOTONES
        // ============================================================

        // ======================================================
        // Limpiar filtros
        // ======================================================
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiar filtros según la pestaña activa
                if (tabControlModos.SelectedTab == tabPorUsuario)
                {
                    LimpiarFiltrosUsuario();
                }
                else if (tabControlModos.SelectedTab == tabPorCategoria)
                {
                    LimpiarFiltrosCategoria();
                }

                else if (tabControlModos.SelectedTab == tabPorModulo)
                {
                    LimpiarFiltrosModulo();
                }

                else if (tabControlModos.SelectedTab == tabPorAccion)
                {
                    LimpiarFiltrosAccion();
                }
                else if (tabControlModos.SelectedTab == tabPorHorario)
                {
                    LimpiarFiltrosHorario();
                }

                else if (tabControlModos.SelectedTab == tabPorIP)
                {
                    LimpiarFiltrosIP();
                }
                else if (tabControlModos.SelectedTab == tabPorRegistro)
                {
                    LimpiarFiltrosRegistro();
                }

                MessageBox.Show("Se han limpiado todos los filtros.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Limpiar filtros por Usuario
        private void LimpiarFiltrosUsuario()
        {
            if (cmbUsuario.Items.Count > 0)
                cmbUsuario.SelectedIndex = 0;

            if (cmbAccion.Items.Count > 0)
                cmbAccion.SelectedIndex = 0;

            if (cmbCategoria.Items.Count > 0)
                cmbCategoria.SelectedIndex = 0;

            if (cmbModulo.Items.Count > 0)
                cmbModulo.SelectedIndex = 0;

            dtpDesde.Value = DateTime.Now.AddMonths(-1);
            dtpHasta.Value = DateTime.Now;

            txtBusquedaRapida.Text = "Buscar...";
            txtBusquedaRapida.ForeColor = Color.Gray;

            chkAgruparSesiones.Checked = false;

            dgvResultadosUsuario.DataSource = null;
            rtbEstadisticasUsuario.Clear();

            lblTotalRegistros.Text = "Total: 0 registros";
        }
        // Limpiar filtros por Categoría
        private void LimpiarFiltrosCategoria()
        {
            if (cmbCategoriaFiltro.Items.Count > 0)
                cmbCategoriaFiltro.SelectedIndex = 0;

            if (cmbUsuarioCategoria.Items.Count > 0)
                cmbUsuarioCategoria.SelectedIndex = 0;

            if (cmbModuloCategoria.Items.Count > 0)
                cmbModuloCategoria.SelectedIndex = 0;

            if (cmbAccionCategoria.Items.Count > 0)
                cmbAccionCategoria.SelectedIndex = 0;

            dtpDesdeCategoria.Value = DateTime.Now.AddMonths(-1);
            dtpHastaCategoria.Value = DateTime.Now;

            dgvResultadosCategoria.DataSource = null;
            rtbEstadisticasCategoria.Clear();

            lblTotalRegistros.Text = "Total: 0 registros";
        }
        // Limpiar filtros por Módulo
        private void LimpiarFiltrosModulo()
        {
            if (cmbCategoriaModulo.Items.Count > 0)
                cmbCategoriaModulo.SelectedIndex = 0;

            if (cmbModuloFiltro.Items.Count > 0)
                cmbModuloFiltro.SelectedIndex = 0;

            if (cmbFormularioModulo.Items.Count > 0)
                cmbFormularioModulo.SelectedIndex = 0;

            if (cmbUsuarioModulo.Items.Count > 0)
                cmbUsuarioModulo.SelectedIndex = 0;

            if (cmbAccionModulo.Items.Count > 0)
                cmbAccionModulo.SelectedIndex = 0;

            dtpDesdeModulo.Value = DateTime.Now.AddMonths(-1);
            dtpHastaModulo.Value = DateTime.Now;

            dgvResultadosModulo.DataSource = null;
            rtbEstadisticasModulo.Clear();

            lblTotalRegistros.Text = "Total: 0 registros";
        }
        // Limpiar filtros por Acción
        private void LimpiarFiltrosAccion()
        {
            if (cmbAccionFiltro.Items.Count > 0)
                cmbAccionFiltro.SelectedIndex = 0;

            if (cmbUsuarioAccion.Items.Count > 0)
                cmbUsuarioAccion.SelectedIndex = 0;

            if (cmbCategoriaAccion.Items.Count > 0)
                cmbCategoriaAccion.SelectedIndex = 0;

            if (cmbModuloAccion.Items.Count > 0)
                cmbModuloAccion.SelectedIndex = 0;

            dtpDesdeAccion.Value = DateTime.Now.AddMonths(-1);
            dtpHastaAccion.Value = DateTime.Now;

            dgvResultadosAccion.DataSource = null;
            rtbEstadisticasAccion.Clear();

            lblTotalRegistros.Text = "Total: 0 registros";
        }
        // Limpiar filtros por Horario
        private void LimpiarFiltrosHorario()
        {
            dtpFechaHorario.Value = DateTime.Now;
            dtpHoraInicio.Value = DateTime.Today.AddHours(0);
            dtpHoraFin.Value = DateTime.Today.AddHours(23).AddMinutes(59);

            if (cmbUsuarioHorario.Items.Count > 0)
                cmbUsuarioHorario.SelectedIndex = 0;

            if (cmbCategoriaHorario.Items.Count > 0)
                cmbCategoriaHorario.SelectedIndex = 0;

            if (cmbModuloHorario.Items.Count > 0)
                cmbModuloHorario.SelectedIndex = 0;

            if (cmbAccionHorario.Items.Count > 0)
                cmbAccionHorario.SelectedIndex = 0;

            chkIncluirFinDeSemana.Checked = false;
            chkSoloFueraOficina.Checked = false;

            dgvResultadosHorario.DataSource = null;
            rtbEstadisticasHorario.Clear();

            lblTotalRegistros.Text = "Total: 0 registros";
        }

        // Limpiar filtros por IP/Máquina
        private void LimpiarFiltrosIP()
        {
            txtDireccionIP.Clear();
            txtNombreMaquina.Clear();

            dtpDesdeIP.Value = DateTime.Now.AddMonths(-1);
            dtpHastaIP.Value = DateTime.Now;

            if (cmbUsuarioIP.Items.Count > 0)
                cmbUsuarioIP.SelectedIndex = 0;

            if (cmbCategoriaIP.Items.Count > 0)
                cmbCategoriaIP.SelectedIndex = 0;

            if (cmbModuloIP.Items.Count > 0)
                cmbModuloIP.SelectedIndex = 0;

            if (cmbAccionIP.Items.Count > 0)
                cmbAccionIP.SelectedIndex = 0;

            chkAgruparPorMaquina.Checked = false;

            dgvResultadosIP.DataSource = null;
            rtbEstadisticasIP.Clear();

            lblTotalRegistros.Text = "Total: 0 registros";
        }

        // Limpiar filtros por Registro
        private void LimpiarFiltrosRegistro()
        {
            if (cmbTipoBusqueda.Items.Count > 0)
                cmbTipoBusqueda.SelectedIndex = 0;

            txtValorBusqueda.Clear();

            if (cmbModuloRegistro.Items.Count > 0)
                cmbModuloRegistro.SelectedIndex = 0;

            dtpDesdeRegistro.Value = DateTime.Now.AddMonths(-1);
            dtpHastaRegistro.Value = DateTime.Now;
            dtpDesdeRegistro.Checked = false;
            dtpHastaRegistro.Checked = false;

            chkVistaTimeline.Checked = true;
            chkSoloFinesSemana.Checked = false;

            rbSinFiltroHorario.Checked = true;

            dtpHoraInicioRegistro.Value = DateTime.Today.AddHours(0);
            dtpHoraFinRegistro.Value = DateTime.Today.AddHours(23).AddMinutes(59);
            dtpHoraInicioRegistro.Enabled = true;
            dtpHoraFinRegistro.Enabled = true;
            dtpHoraInicioRegistro.BackColor = Color.FromArgb(240, 240, 240);
            dtpHoraFinRegistro.BackColor = Color.FromArgb(240, 240, 240);

            dgvResultadosRegistro.DataSource = null;
            rtbTimeline.Clear();

            lblTotalRegistros.Text = "Total: 0 registros";
        }

        // =======================================================
        // EXPORTAR DATOS
        // =======================================================

        // =======================================================// =======================================================
        // Exportar a Excel
        private void BtnExportarExcel_Click(object sender, EventArgs e)
        {
            var dgv = ObtenerDataGridViewActivo();
            if (dgv == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel (*.xlsx)|*.xlsx";
                sfd.FileName = $"Auditoria_{ObtenerNombrePestanaActiva()}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    Cursor = Cursors.WaitCursor;

                    // Obtener datos del DataGridView
                    DataTable dt = new DataTable();
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        if (col.Visible)
                            dt.Columns.Add(col.HeaderText);
                    }

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;
                        var dr = dt.NewRow();
                        int idx = 0;
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            if (col.Visible)
                            {
                                dr[idx] = row.Cells[col.Index].Value ?? "";
                                idx++;
                            }
                        }
                        dt.Rows.Add(dr);
                    }

                    using (var wb = new XLWorkbook())
                    {
                        // 1. HOJA DE PORTADA
                        CrearPortadaExcel(wb, dt.Rows.Count);

                        // 2. HOJA DE RESUMEN EJECUTIVO
                        CrearResumenEjecutivoExcel(wb, dt);

                        // 3. HOJA DE DATOS DETALLADOS
                        var wsDatos = wb.Worksheets.Add("Datos Detallados");
                        var tabla = wsDatos.Cell(1, 1).InsertTable(dt);
                        tabla.Theme = XLTableTheme.TableStyleMedium2;
                        wsDatos.Columns().AdjustToContents();

                        // 4. HOJAS DE ANÁLISIS ESPECÍFICOS
                        CrearAnalisisTemporalExcel(wb, dt);
                        CrearAnalisisUsuariosExcel(wb, dt);
                        CrearAnalisisAccionesExcel(wb, dt);
                        CrearAnalisisModulosExcel(wb, dt);

                        // 5. HOJA DE GRÁFICOS
                        CrearGraficosExcel(wb, dt);

                        // 6. HOJA DE CONCLUSIONES
                        CrearConclusionesExcel(wb, dt);

                        wb.SaveAs(sfd.FileName);
                    }

                    Cursor = Cursors.Default;
                    if (MessageBox.Show($"✅ Reporte generado exitosamente:\n\n{sfd.FileName}\n\n¿Desea abrir el archivo?",
                        "Exportación Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show($"Error al exportar:\n\n{ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Crear portada del Excel
        private void CrearPortadaExcel(XLWorkbook wb, int totalRegistros)
        {
            var ws = wb.Worksheets.Add("Portada");
            ws.TabColor = XLColor.FromArgb(0, 120, 212);

            // Título principal
            ws.Cell("B3").Value = "REPORTE DE AUDITORÍA";
            ws.Cell("B3").Style.Font.FontSize = 28;
            ws.Cell("B3").Style.Font.Bold = true;
            ws.Cell("B3").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);

            ws.Cell("B5").Value = "MOFIS-ERP - Sistema de Gestión Empresarial";
            ws.Cell("B5").Style.Font.FontSize = 14;
            ws.Cell("B5").Style.Font.Italic = true;

            // Información del reporte
            int fila = 8;
            ws.Cell(fila, 2).Value = "Información del Reporte";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Font.FontSize = 14;
            ws.Cell(fila, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Cell(fila, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0, 120, 212);
            fila += 2;

            AgregarFilaInfo(ws, ref fila, "Tipo de Búsqueda:", ObtenerNombrePestanaActiva());
            AgregarFilaInfo(ws, ref fila, "Fecha de Generación:", DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy HH:mm:ss"));
            AgregarFilaInfo(ws, ref fila, "Generado por:", SesionActual.NombreCompleto);
            AgregarFilaInfo(ws, ref fila, "Usuario:", SesionActual.Username);
            AgregarFilaInfo(ws, ref fila, "Total de Registros:", totalRegistros.ToString("N0"));

            fila += 2;
            ws.Cell(fila, 2).Value = "Contenido del Reporte";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Font.FontSize = 14;
            ws.Cell(fila, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Cell(fila, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0, 120, 212);
            fila += 2;

            string[] hojas = { "Resumen Ejecutivo", "Datos Detallados", "Análisis Temporal",
                       "Análisis de Usuarios", "Análisis de Acciones", "Análisis de Módulos",
                       "Gráficos", "Conclusiones" };

            foreach (var hoja in hojas)
            {
                ws.Cell(fila, 3).Value = $"• {hoja}";
                ws.Cell(fila, 3).Style.Font.FontSize = 11;
                fila++;
            }

            // Pie de página
            ws.Cell(30, 2).Value = $"© {DateTime.Now.Year} Fiducorp - Todos los derechos reservados";
            ws.Cell(30, 2).Style.Font.Italic = true;
            ws.Cell(30, 2).Style.Font.FontColor = XLColor.Gray;

            ws.Columns().AdjustToContents();
        }
        // Agregar fila de información en la portada del Excel
        private void AgregarFilaInfo(IXLWorksheet ws, ref int fila, string etiqueta, string valor)
        {
            ws.Cell(fila, 2).Value = etiqueta;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 3).Value = valor;
            fila++;
        }
        // Crear resumen ejecutivo en Excel 
        private void CrearResumenEjecutivoExcel(XLWorkbook wb, DataTable dt)
        {
            var ws = wb.Worksheets.Add("Resumen Ejecutivo");
            ws.TabColor = XLColor.Green;

            int fila = 2;

            // Título
            ws.Cell(fila, 2).Value = "📊 RESUMEN EJECUTIVO";
            ws.Cell(fila, 2).Style.Font.FontSize = 18;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
            fila += 3;

            // KPIs principales
            ws.Cell(fila, 2).Value = "Indicadores Clave de Rendimiento (KPIs)";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Font.FontSize = 14;
            fila += 2;

            // Calcular KPIs
            int totalRegistros = dt.Rows.Count;
            int usuariosUnicos = dt.AsEnumerable().Where(r => r.Table.Columns.Contains("Username") && r["Username"] != DBNull.Value)
                .Select(r => r["Username"].ToString()).Distinct().Count();
            int accionesUnicas = dt.AsEnumerable().Where(r => r.Table.Columns.Contains("Acción") && r["Acción"] != DBNull.Value)
                .Select(r => r["Acción"].ToString()).Distinct().Count();

            CrearKPI(ws, ref fila, "Total de Registros", totalRegistros.ToString("N0"), XLColor.Blue);
            CrearKPI(ws, ref fila, "Usuarios Únicos", usuariosUnicos.ToString("N0"), XLColor.Green);
            CrearKPI(ws, ref fila, "Tipos de Acciones", accionesUnicas.ToString("N0"), XLColor.Orange);

            fila += 2;

            // Período de análisis
            if (dt.Columns.Contains("Fecha/Hora") && dt.Rows.Count > 0)
            {
                var fechas = dt.AsEnumerable()
                    .Where(r => r["Fecha/Hora"] != DBNull.Value)
                    .Select(r => Convert.ToDateTime(r["Fecha/Hora"]))
                    .OrderBy(f => f)
                    .ToList();

                if (fechas.Any())
                {
                    ws.Cell(fila, 2).Value = "Período de Análisis";
                    ws.Cell(fila, 2).Style.Font.Bold = true;
                    fila++;
                    ws.Cell(fila, 3).Value = $"Desde: {fechas.First():dd/MM/yyyy HH:mm}";
                    fila++;
                    ws.Cell(fila, 3).Value = $"Hasta: {fechas.Last():dd/MM/yyyy HH:mm}";
                    fila++;
                    ws.Cell(fila, 3).Value = $"Duración: {(fechas.Last() - fechas.First()).Days} días";
                    fila += 2;
                }
            }

            ws.Columns().AdjustToContents();
        }
        // Crear un KPI en el resumen ejecutivo de Excel
        private void CrearKPI(IXLWorksheet ws, ref int fila, string nombre, string valor, XLColor color)
        {
            ws.Range(fila, 2, fila, 4).Merge();
            ws.Cell(fila, 2).Value = nombre;
            ws.Cell(fila, 2).Style.Fill.BackgroundColor = color;
            ws.Cell(fila, 2).Style.Font.FontColor = XLColor.White;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            fila++;
            ws.Range(fila, 2, fila, 4).Merge();
            ws.Cell(fila, 2).Value = valor;
            ws.Cell(fila, 2).Style.Font.FontSize = 24;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(fila, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            fila += 2;
        }
        // Crear análisis temporal en Excel 
        private void CrearAnalisisTemporalExcel(XLWorkbook wb, DataTable dt)
        {
            if (!dt.Columns.Contains("Fecha/Hora")) return;

            var ws = wb.Worksheets.Add("Análisis Temporal");
            ws.TabColor = XLColor.Purple;

            int fila = 2;
            ws.Cell(fila, 2).Value = "📅 ANÁLISIS TEMPORAL";
            ws.Cell(fila, 2).Style.Font.FontSize = 18;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 3;

            // Actividad por día de la semana
            var porDia = dt.AsEnumerable()
                .Where(r => r["Fecha/Hora"] != DBNull.Value)
                .GroupBy(r => Convert.ToDateTime(r["Fecha/Hora"]).DayOfWeek)
                .Select(g => new { Dia = g.Key, Cantidad = g.Count() })
                .OrderBy(x => x.Dia);

            ws.Cell(fila, 2).Value = "Actividad por Día de la Semana";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 2;

            ws.Cell(fila, 2).Value = "Día";
            ws.Cell(fila, 3).Value = "Cantidad";
            ws.Cell(fila, 4).Value = "Porcentaje";
            ws.Range(fila, 2, fila, 4).Style.Font.Bold = true;
            ws.Range(fila, 2, fila, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
            ws.Range(fila, 2, fila, 4).Style.Font.FontColor = XLColor.White;
            fila++;

            string[] dias = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
            foreach (var item in porDia)
            {
                ws.Cell(fila, 2).Value = dias[(int)item.Dia];
                ws.Cell(fila, 3).Value = item.Cantidad;
                ws.Cell(fila, 4).Value = (item.Cantidad * 100.0 / dt.Rows.Count) / 100;
                ws.Cell(fila, 4).Style.NumberFormat.Format = "0.00%";
                fila++;
            }

            fila += 2;

            // Actividad por hora del día
            var porHora = dt.AsEnumerable()
                .Where(r => r["Fecha/Hora"] != DBNull.Value)
                .GroupBy(r => Convert.ToDateTime(r["Fecha/Hora"]).Hour)
                .Select(g => new { Hora = g.Key, Cantidad = g.Count() })
                .OrderBy(x => x.Hora);

            ws.Cell(fila, 2).Value = "Actividad por Hora del Día";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 2;

            ws.Cell(fila, 2).Value = "Hora";
            ws.Cell(fila, 3).Value = "Cantidad";
            ws.Range(fila, 2, fila, 3).Style.Font.Bold = true;
            ws.Range(fila, 2, fila, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
            ws.Range(fila, 2, fila, 3).Style.Font.FontColor = XLColor.White;
            fila++;

            foreach (var item in porHora)
            {
                ws.Cell(fila, 2).Value = $"{item.Hora:00}:00";
                ws.Cell(fila, 3).Value = item.Cantidad;
                fila++;
            }

            ws.Columns().AdjustToContents();
        }
        // Crear análisis de usuarios en Excel 
        private void CrearAnalisisUsuariosExcel(XLWorkbook wb, DataTable dt)
        {
            if (!dt.Columns.Contains("Usuario")) return;

            var ws = wb.Worksheets.Add("Análisis de Usuarios");
            ws.TabColor = XLColor.Cyan;

            int fila = 2;
            ws.Cell(fila, 2).Value = "👥 ANÁLISIS DE USUARIOS";
            ws.Cell(fila, 2).Style.Font.FontSize = 18;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 3;

            var topUsuarios = dt.AsEnumerable()
                .Where(r => r["Usuario"] != DBNull.Value)
                .GroupBy(r => r["Usuario"].ToString())
                .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(20);

            ws.Cell(fila, 2).Value = "Top 20 Usuarios Más Activos";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 2;

            ws.Cell(fila, 2).Value = "Ranking";
            ws.Cell(fila, 3).Value = "Usuario";
            ws.Cell(fila, 4).Value = "Cantidad";
            ws.Cell(fila, 5).Value = "Porcentaje";
            ws.Range(fila, 2, fila, 5).Style.Font.Bold = true;
            ws.Range(fila, 2, fila, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
            ws.Range(fila, 2, fila, 5).Style.Font.FontColor = XLColor.White;
            fila++;

            int ranking = 1;
            foreach (var item in topUsuarios)
            {
                ws.Cell(fila, 2).Value = ranking;
                ws.Cell(fila, 3).Value = item.Usuario;
                ws.Cell(fila, 4).Value = item.Cantidad;
                ws.Cell(fila, 5).Value = (item.Cantidad * 100.0 / dt.Rows.Count) / 100;
                ws.Cell(fila, 5).Style.NumberFormat.Format = "0.00%";

                if (ranking <= 3)
                    ws.Range(fila, 2, fila, 5).Style.Fill.BackgroundColor = XLColor.LightGreen;

                fila++;
                ranking++;
            }

            ws.Columns().AdjustToContents();
        }
        // Crear análisis de acciones en Excel 
        private void CrearAnalisisAccionesExcel(XLWorkbook wb, DataTable dt)
        {
            if (!dt.Columns.Contains("Acción")) return;

            var ws = wb.Worksheets.Add("Análisis de Acciones");
            ws.TabColor = XLColor.Red;

            int fila = 2;
            ws.Cell(fila, 2).Value = "🎯 ANÁLISIS DE ACCIONES";
            ws.Cell(fila, 2).Style.Font.FontSize = 18;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 3;

            var topAcciones = dt.AsEnumerable()
                .Where(r => r["Acción"] != DBNull.Value)
                .GroupBy(r => r["Acción"].ToString())
                .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad);

            ws.Cell(fila, 2).Value = "Distribución de Acciones";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 2;

            ws.Cell(fila, 2).Value = "Acción";
            ws.Cell(fila, 3).Value = "Cantidad";
            ws.Cell(fila, 4).Value = "Porcentaje";
            ws.Range(fila, 2, fila, 4).Style.Font.Bold = true;
            ws.Range(fila, 2, fila, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
            ws.Range(fila, 2, fila, 4).Style.Font.FontColor = XLColor.White;
            fila++;

            foreach (var item in topAcciones)
            {
                ws.Cell(fila, 2).Value = item.Accion;
                ws.Cell(fila, 3).Value = item.Cantidad;
                ws.Cell(fila, 4).Value = (item.Cantidad * 100.0 / dt.Rows.Count) / 100;
                ws.Cell(fila, 4).Style.NumberFormat.Format = "0.00%";
                fila++;
            }

            ws.Columns().AdjustToContents();
        }
        // Crear análisis de módulos en Excel 
        private void CrearAnalisisModulosExcel(XLWorkbook wb, DataTable dt)
        {
            if (!dt.Columns.Contains("Módulo")) return;

            var ws = wb.Worksheets.Add("Análisis de Módulos");
            ws.TabColor = XLColor.Yellow;

            int fila = 2;
            ws.Cell(fila, 2).Value = "🗂️ ANÁLISIS DE MÓDULOS";
            ws.Cell(fila, 2).Style.Font.FontSize = 18;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 3;

            var topModulos = dt.AsEnumerable()
                .Where(r => r["Módulo"] != DBNull.Value)
                .GroupBy(r => r["Módulo"].ToString())
                .Select(g => new { Modulo = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad);

            ws.Cell(fila, 2).Value = "Uso por Módulo";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            fila += 2;

            ws.Cell(fila, 2).Value = "Módulo";
            ws.Cell(fila, 3).Value = "Cantidad";
            ws.Cell(fila, 4).Value = "Porcentaje";
            ws.Range(fila, 2, fila, 4).Style.Font.Bold = true;
            ws.Range(fila, 2, fila, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
            ws.Range(fila, 2, fila, 4).Style.Font.FontColor = XLColor.White;
            fila++;

            foreach (var item in topModulos)
            {
                ws.Cell(fila, 2).Value = item.Modulo;
                ws.Cell(fila, 3).Value = item.Cantidad;
                ws.Cell(fila, 4).Value = (item.Cantidad * 100.0 / dt.Rows.Count) / 100;
                ws.Cell(fila, 4).Style.NumberFormat.Format = "0.00%";
                fila++;
            }

            ws.Columns().AdjustToContents();
        }
        // Crear hoja de gráficos en Excel 
        private void CrearGraficosExcel(XLWorkbook wb, DataTable dt)
        {
            var ws = wb.Worksheets.Add("Gráficos");
            ws.TabColor = XLColor.Magenta;

            ws.Cell(2, 2).Value = "📈 VISUALIZACIONES GRÁFICAS";
            ws.Cell(2, 2).Style.Font.FontSize = 18;
            ws.Cell(2, 2).Style.Font.Bold = true;

            ws.Cell(5, 2).Value = "Esta hoja está reservada para gráficos dinámicos.";
            ws.Cell(6, 2).Value = "Los gráficos se generan automáticamente en Excel basándose en los datos de análisis.";
            ws.Cell(7, 2).Value = "Puede crear sus propios gráficos utilizando las pestañas de análisis.";
        }
        // Hoja de Conclusiones y Recomendaciones en Excel 
        private void CrearConclusionesExcel(XLWorkbook wb, DataTable dt)
        {
            var ws = wb.Worksheets.Add("Conclusiones");
            ws.TabColor = XLColor.DarkBlue;

            int fila = 2;
            ws.Cell(fila, 2).Value = "📋 CONCLUSIONES Y RECOMENDACIONES";
            ws.Cell(fila, 2).Style.Font.FontSize = 18;
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
            fila += 3;

            ws.Cell(fila, 2).Value = "Resumen General";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Font.FontSize = 14;
            fila += 2;

            ws.Cell(fila, 2).Value = $"• Total de registros analizados: {dt.Rows.Count:N0}";
            fila++;
            ws.Cell(fila, 2).Value = $"• Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            fila++;
            ws.Cell(fila, 2).Value = $"• Tipo de análisis: {ObtenerNombrePestanaActiva()}";
            fila += 3;

            ws.Cell(fila, 2).Value = "Hallazgos Clave";
            ws.Cell(fila, 2).Style.Font.Bold = true;
            ws.Cell(fila, 2).Style.Font.FontSize = 14;
            fila += 2;

            ws.Cell(fila, 2).Value = "✓ Los datos reflejan la actividad del sistema en el período analizado.";
            fila++;
            ws.Cell(fila, 2).Value = "✓ Se recomienda revisar las pestañas de análisis para información detallada.";
            fila++;
            ws.Cell(fila, 2).Value = "✓ Este reporte puede ser utilizado para auditorías y análisis de seguridad.";
            fila += 3;

            ws.Cell(fila, 2).Value = $"Generado por MOFIS-ERP © {DateTime.Now.Year} Fiducorp";
            ws.Cell(fila, 2).Style.Font.Italic = true;
            ws.Cell(fila, 2).Style.Font.FontColor = XLColor.Gray;

            ws.Columns().AdjustToContents();
        }
        // =======================================================// =======================================================

        // =======================================================// =======================================================
        // Exportar a PDF
        private void BtnExportarPDF_Click(object sender, EventArgs e)
        {
            var dgv = ObtenerDataGridViewActivo();
            if (dgv == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = $"Auditoria_{ObtenerNombrePestanaActiva()}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    Cursor = Cursors.WaitCursor;

                    // Obtener datos
                    DataTable dt = new DataTable();
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        if (col.Visible)
                            dt.Columns.Add(col.HeaderText);
                    }

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;
                        var dr = dt.NewRow();
                        int idx = 0;
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            if (col.Visible)
                            {
                                dr[idx] = row.Cells[col.Index].Value ?? "";
                                idx++;
                            }
                        }
                        dt.Rows.Add(dr);
                    }

                    // Crear documento con márgenes
                    Document doc = new Document(PageSize.A4, 40, 40, 80, 50);
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));

                    // Agregar eventos para header y footer
                    var pageEvents = new PDFPageEvents(rutaLogo);
                    writer.PageEvent = pageEvents;

                    doc.Open();

                    // PORTADA
                    AgregarPortadaPDF(doc, dt.Rows.Count);
                    doc.NewPage();

                    // RESUMEN EJECUTIVO
                    AgregarResumenEjecutivoPDF(doc, dt);
                    doc.NewPage();

                    // ANÁLISIS TEMPORAL
                    if (dt.Columns.Contains("Fecha/Hora"))
                    {
                        AgregarAnalisisTemporalPDF(doc, dt);
                        doc.NewPage();
                    }

                    // ANÁLISIS DE USUARIOS
                    if (dt.Columns.Contains("Usuario"))
                    {
                        AgregarAnalisisUsuariosPDF(doc, dt);
                        doc.NewPage();
                    }

                    // ANÁLISIS DE ACCIONES
                    if (dt.Columns.Contains("Acción"))
                    {
                        AgregarAnalisisAccionesPDF(doc, dt);
                        doc.NewPage();
                    }

                    // DATOS DETALLADOS
                    AgregarDatosDetalladosPDF(doc, dt, dgv);
                    doc.NewPage();

                    // CONCLUSIONES
                    AgregarConclusionesPDF(doc, dt);

                    doc.Close();

                    Cursor = Cursors.Default;

                    if (MessageBox.Show($"✅ Reporte PDF generado exitosamente:\n\n{sfd.FileName}\n\n¿Desea abrir el archivo?",
                        "Exportación Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show($"Error al exportar PDF:\n\n{ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Clase interna para eventos de página
        private class PDFPageEvents : PdfPageEventHelper
        {
            private string logoPath;
            private iTextSharp.text.Image logo;

            public PDFPageEvents(string rutaLogo)
            {
                this.logoPath = rutaLogo;
                try
                {
                    if (File.Exists(rutaLogo))
                    {
                        logo = iTextSharp.text.Image.GetInstance(rutaLogo);
                        logo.ScaleToFit(80f, 60f);
                    }
                }
                catch { }
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // Header con logo
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.TotalWidth = document.PageSize.Width - 80;
                headerTable.SetWidths(new float[] { 3f, 1f });

                // Celda izquierda - Título
                PdfPCell cellTitulo = new PdfPCell(new Phrase("REPORTE DE AUDITORÍA - MOFIS-ERP",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)));
                cellTitulo.BackgroundColor = new BaseColor(0, 120, 212);
                cellTitulo.HorizontalAlignment = Element.ALIGN_LEFT;
                cellTitulo.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellTitulo.Border = 0;
                cellTitulo.PaddingLeft = 10;
                cellTitulo.PaddingTop = 8;
                cellTitulo.PaddingBottom = 8;
                headerTable.AddCell(cellTitulo);

                // Celda derecha - Logo
                if (logo != null)
                {
                    PdfPCell cellLogo = new PdfPCell(logo);
                    cellLogo.BackgroundColor = new BaseColor(0, 120, 212);
                    cellLogo.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellLogo.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cellLogo.Border = 0;
                    cellLogo.PaddingRight = 10;
                    cellLogo.PaddingTop = 5;
                    cellLogo.PaddingBottom = 5;
                    headerTable.AddCell(cellLogo);
                }
                else
                {
                    PdfPCell cellEmpty = new PdfPCell(new Phrase(""));
                    cellEmpty.BackgroundColor = new BaseColor(0, 120, 212);
                    cellEmpty.Border = 0;
                    headerTable.AddCell(cellEmpty);
                }

                headerTable.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);

                // Footer
                PdfPTable footerTable = new PdfPTable(3);
                footerTable.TotalWidth = document.PageSize.Width - 80;
                footerTable.SetWidths(new float[] { 1f, 1f, 1f });

                // Footer izquierdo
                PdfPCell cellFooterLeft = new PdfPCell(new Phrase($"© {DateTime.Now.Year} Fiducorp",
                    FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY)));
                cellFooterLeft.HorizontalAlignment = Element.ALIGN_LEFT;
                cellFooterLeft.Border = iTextSharp.text.Rectangle.TOP_BORDER;
                cellFooterLeft.BorderColor = BaseColor.LIGHT_GRAY;
                cellFooterLeft.PaddingTop = 5;
                footerTable.AddCell(cellFooterLeft);

                // Footer centro
                PdfPCell cellFooterCenter = new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY)));
                cellFooterCenter.HorizontalAlignment = Element.ALIGN_CENTER;
                cellFooterCenter.Border = iTextSharp.text.Rectangle.TOP_BORDER;
                cellFooterCenter.BorderColor = BaseColor.LIGHT_GRAY;
                cellFooterCenter.PaddingTop = 5;
                footerTable.AddCell(cellFooterCenter);

                // Footer derecho - Número de página
                PdfPCell cellFooterRight = new PdfPCell(new Phrase($"Página {writer.PageNumber}",
                    FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY)));
                cellFooterRight.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellFooterRight.Border = iTextSharp.text.Rectangle.TOP_BORDER;
                cellFooterRight.BorderColor = BaseColor.LIGHT_GRAY;
                cellFooterRight.PaddingTop = 5;
                footerTable.AddCell(cellFooterRight);

                footerTable.WriteSelectedRows(0, -1, 40, 40, writer.DirectContent);
            }
        }

        private void AgregarPortadaPDF(Document doc, int totalRegistros)
        {
            // Espaciado superior
            doc.Add(new Paragraph("\n\n\n\n"));

            // Título principal
            var titulo = new Paragraph("REPORTE DE AUDITORÍA",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 32, new BaseColor(0, 120, 212)));
            titulo.Alignment = Element.ALIGN_CENTER;
            doc.Add(titulo);

            doc.Add(new Paragraph("\n"));

            // Subtítulo
            var subtitulo = new Paragraph("Sistema de Gestión Empresarial MOFIS-ERP",
                FontFactory.GetFont(FontFactory.HELVETICA, 16, BaseColor.GRAY));
            subtitulo.Alignment = Element.ALIGN_CENTER;
            doc.Add(subtitulo);

            doc.Add(new Paragraph("\n\n\n"));

            // Cuadro de información
            PdfPTable infoTable = new PdfPTable(2);
            infoTable.WidthPercentage = 70;
            infoTable.HorizontalAlignment = Element.ALIGN_CENTER;
            infoTable.SetWidths(new float[] { 1f, 2f });

            var fontLabel = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLACK);
            var fontValue = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.DARK_GRAY);

            AgregarFilaInfoPDF(infoTable, "Tipo de Análisis:", ObtenerNombrePestanaActiva(), fontLabel, fontValue);
            AgregarFilaInfoPDF(infoTable, "Fecha de Generación:", DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy"), fontLabel, fontValue);
            AgregarFilaInfoPDF(infoTable, "Hora:", DateTime.Now.ToString("HH:mm:ss"), fontLabel, fontValue);
            AgregarFilaInfoPDF(infoTable, "Generado por:", SesionActual.NombreCompleto, fontLabel, fontValue);
            AgregarFilaInfoPDF(infoTable, "Usuario:", SesionActual.Username, fontLabel, fontValue);
            AgregarFilaInfoPDF(infoTable, "Rol:", SesionActual.NombreRol, fontLabel, fontValue);
            AgregarFilaInfoPDF(infoTable, "Total de Registros:", totalRegistros.ToString("N0"), fontLabel, fontValue);

            doc.Add(infoTable);

            doc.Add(new Paragraph("\n\n\n"));

            // Nota de confidencialidad
            var nota = new Paragraph("DOCUMENTO CONFIDENCIAL",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.RED));
            nota.Alignment = Element.ALIGN_CENTER;
            doc.Add(nota);

            var notaDetalle = new Paragraph("Este documento contiene información sensible de auditoría del sistema.\nSu distribución está restringida a personal autorizado.",
                FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY));
            notaDetalle.Alignment = Element.ALIGN_CENTER;
            doc.Add(notaDetalle);
        }

        private void AgregarFilaInfoPDF(PdfPTable table, string label, string value, iTextSharp.text.Font fontLabel, iTextSharp.text.Font fontValue)
        {
            PdfPCell cellLabel = new PdfPCell(new Phrase(label, fontLabel));
            cellLabel.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cellLabel.PaddingTop = 5;
            cellLabel.PaddingBottom = 5;
            cellLabel.PaddingRight = 10;
            table.AddCell(cellLabel);

            PdfPCell cellValue = new PdfPCell(new Phrase(value, fontValue));
            cellValue.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cellValue.PaddingTop = 5;
            cellValue.PaddingBottom = 5;
            table.AddCell(cellValue);
        }

        private void AgregarResumenEjecutivoPDF(Document doc, DataTable dt)
        {
            // Título de sección
            AgregarTituloSeccion(doc, "RESUMEN EJECUTIVO");

            // KPIs en tabla
            PdfPTable kpiTable = new PdfPTable(3);
            kpiTable.WidthPercentage = 100;
            kpiTable.SpacingBefore = 10;
            kpiTable.SpacingAfter = 20;

            int totalRegistros = dt.Rows.Count;
            int usuariosUnicos = dt.Columns.Contains("Usuario") ?
                dt.AsEnumerable().Where(r => r["Usuario"] != DBNull.Value)
                    .Select(r => r["Usuario"].ToString()).Distinct().Count() : 0;
            int accionesUnicas = dt.Columns.Contains("Acción") ?
                dt.AsEnumerable().Where(r => r["Acción"] != DBNull.Value)
                    .Select(r => r["Acción"].ToString()).Distinct().Count() : 0;

            AgregarKPIPDF(kpiTable, "Total de Registros", totalRegistros.ToString("N0"), new BaseColor(41, 128, 185));
            AgregarKPIPDF(kpiTable, "Usuarios Únicos", usuariosUnicos.ToString("N0"), new BaseColor(39, 174, 96));
            AgregarKPIPDF(kpiTable, "Tipos de Acciones", accionesUnicas.ToString("N0"), new BaseColor(230, 126, 34));

            doc.Add(kpiTable);

            // Período de análisis
            if (dt.Columns.Contains("Fecha/Hora") && dt.Rows.Count > 0)
            {
                var fechas = dt.AsEnumerable()
                    .Where(r => r["Fecha/Hora"] != DBNull.Value)
                    .Select(r => Convert.ToDateTime(r["Fecha/Hora"]))
                    .OrderBy(f => f)
                    .ToList();

                if (fechas.Any())
                {
                    var periodo = new Paragraph($"\nPeríodo analizado: Desde {fechas.First():dd/MM/yyyy HH:mm} hasta {fechas.Last():dd/MM/yyyy HH:mm} ({(fechas.Last() - fechas.First()).Days} días)",
                        FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.DARK_GRAY));
                    periodo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(periodo);
                }
            }
        }

        private void AgregarKPIPDF(PdfPTable table, string nombre, string valor, BaseColor color)
        {
            PdfPCell cell = new PdfPCell();
            cell.Border = iTextSharp.text.Rectangle.BOX;
            cell.BorderColor = color;
            cell.BorderWidth = 2;
            cell.Padding = 15;

            var nombre_p = new Paragraph(nombre, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, color));
            nombre_p.Alignment = Element.ALIGN_CENTER;

            var valor_p = new Paragraph(valor, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, color));
            valor_p.Alignment = Element.ALIGN_CENTER;
            valor_p.SpacingBefore = 5;

            cell.AddElement(nombre_p);
            cell.AddElement(valor_p);

            table.AddCell(cell);
        }

        private void AgregarAnalisisTemporalPDF(Document doc, DataTable dt)
        {
            AgregarTituloSeccion(doc, "ANÁLISIS TEMPORAL");

            // Actividad por día de la semana
            var porDia = dt.AsEnumerable()
                .Where(r => r["Fecha/Hora"] != DBNull.Value)
                .GroupBy(r => Convert.ToDateTime(r["Fecha/Hora"]).DayOfWeek)
                .Select(g => new { Dia = g.Key, Cantidad = g.Count() })
                .OrderBy(x => x.Dia)
                .ToList();

            doc.Add(new Paragraph("Distribución por Día de la Semana",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK)));
            doc.Add(new Paragraph(" "));

            PdfPTable tablaDias = new PdfPTable(3);
            tablaDias.WidthPercentage = 80;
            tablaDias.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaDias.SetWidths(new float[] { 2f, 1f, 1f });

            // Headers
            AgregarCeldaHeader(tablaDias, "Día");
            AgregarCeldaHeader(tablaDias, "Cantidad");
            AgregarCeldaHeader(tablaDias, "Porcentaje");

            string[] dias = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
            foreach (var item in porDia)
            {
                AgregarCeldaDato(tablaDias, dias[(int)item.Dia]);
                AgregarCeldaDato(tablaDias, item.Cantidad.ToString("N0"));
                AgregarCeldaDato(tablaDias, $"{(item.Cantidad * 100.0 / dt.Rows.Count):F1}%");
            }

            doc.Add(tablaDias);
            doc.Add(new Paragraph("\n"));

            // Actividad por hora
            var porHora = dt.AsEnumerable()
                .Where(r => r["Fecha/Hora"] != DBNull.Value)
                .GroupBy(r => Convert.ToDateTime(r["Fecha/Hora"]).Hour)
                .Select(g => new { Hora = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(10);

            doc.Add(new Paragraph("Top 10 Horas Más Activas",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK)));
            doc.Add(new Paragraph(" "));

            PdfPTable tablaHoras = new PdfPTable(2);
            tablaHoras.WidthPercentage = 60;
            tablaHoras.HorizontalAlignment = Element.ALIGN_CENTER;

            AgregarCeldaHeader(tablaHoras, "Hora");
            AgregarCeldaHeader(tablaHoras, "Cantidad");

            foreach (var item in porHora)
            {
                AgregarCeldaDato(tablaHoras, $"{item.Hora:00}:00");
                AgregarCeldaDato(tablaHoras, item.Cantidad.ToString("N0"));
            }

            doc.Add(tablaHoras);
        }

        private void AgregarAnalisisUsuariosPDF(Document doc, DataTable dt)
        {
            AgregarTituloSeccion(doc, "ANÁLISIS DE USUARIOS");

            var topUsuarios = dt.AsEnumerable()
                .Where(r => r["Usuario"] != DBNull.Value)
                .GroupBy(r => r["Usuario"].ToString())
                .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(15);

            doc.Add(new Paragraph("Top 15 Usuarios Más Activos",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK)));
            doc.Add(new Paragraph(" "));

            PdfPTable tabla = new PdfPTable(4);
            tabla.WidthPercentage = 90;
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.SetWidths(new float[] { 0.5f, 2f, 1f, 1f });

            AgregarCeldaHeader(tabla, "#");
            AgregarCeldaHeader(tabla, "Usuario");
            AgregarCeldaHeader(tabla, "Acciones");
            AgregarCeldaHeader(tabla, "%");

            int ranking = 1;
            foreach (var item in topUsuarios)
            {
                var cellRanking = new PdfPCell(new Phrase(ranking.ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)));
                cellRanking.HorizontalAlignment = Element.ALIGN_CENTER;
                cellRanking.Padding = 5;

                if (ranking <= 3)
                    cellRanking.BackgroundColor = new BaseColor(255, 250, 205);

                tabla.AddCell(cellRanking);
                AgregarCeldaDato(tabla, item.Usuario, ranking <= 3);
                AgregarCeldaDato(tabla, item.Cantidad.ToString("N0"), ranking <= 3);
                AgregarCeldaDato(tabla, $"{(item.Cantidad * 100.0 / dt.Rows.Count):F1}%", ranking <= 3);

                ranking++;
            }

            doc.Add(tabla);
        }

        private void AgregarAnalisisAccionesPDF(Document doc, DataTable dt)
        {
            AgregarTituloSeccion(doc, "ANÁLISIS DE ACCIONES");

            var topAcciones = dt.AsEnumerable()
                .Where(r => r["Acción"] != DBNull.Value)
                .GroupBy(r => r["Acción"].ToString())
                .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .Take(15);

            doc.Add(new Paragraph("Distribución de Acciones",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK)));
            doc.Add(new Paragraph(" "));

            PdfPTable tabla = new PdfPTable(3);
            tabla.WidthPercentage = 80;
            tabla.HorizontalAlignment = Element.ALIGN_CENTER;
            tabla.SetWidths(new float[] { 3f, 1f, 1f });

            AgregarCeldaHeader(tabla, "Acción");
            AgregarCeldaHeader(tabla, "Cantidad");
            AgregarCeldaHeader(tabla, "Porcentaje");

            foreach (var item in topAcciones)
            {
                AgregarCeldaDato(tabla, item.Accion);
                AgregarCeldaDato(tabla, item.Cantidad.ToString("N0"));
                AgregarCeldaDato(tabla, $"{(item.Cantidad * 100.0 / dt.Rows.Count):F1}%");
            }

            doc.Add(tabla);
        }

        private void AgregarDatosDetalladosPDF(Document doc, DataTable dt, DataGridView dgv)
        {
            AgregarTituloSeccion(doc, "DATOS DETALLADOS");

            doc.Add(new Paragraph($"Total de registros: {dt.Rows.Count:N0}",
                FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.DARK_GRAY)));
            doc.Add(new Paragraph(" "));

            // Tomar solo las primeras 100 filas para no saturar el PDF
            int maxFilas = Math.Min(100, dt.Rows.Count);

            var colsVisibles = dgv.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
            PdfPTable tabla = new PdfPTable(colsVisibles.Count);
            tabla.WidthPercentage = 100;
            tabla.HeaderRows = 1;

            // Encabezados
            foreach (var col in colsVisibles)
            {
                AgregarCeldaHeader(tabla, col.HeaderText);
            }

            // Datos (máximo 100 registros)
            for (int i = 0; i < maxFilas; i++)
            {
                foreach (var col in colsVisibles)
                {
                    var val = dt.Rows[i][col.HeaderText];
                    string texto = val is DateTime dt_val ? dt_val.ToString("dd/MM/yyyy HH:mm") : val?.ToString() ?? "";

                    var cell = new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA, 7)));
                    cell.Padding = 3;
                    tabla.AddCell(cell);
                }
            }

            doc.Add(tabla);

            if (dt.Rows.Count > 100)
            {
                doc.Add(new Paragraph($"\n* Mostrando los primeros 100 registros de {dt.Rows.Count:N0} totales.",
                    FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY)));
            }
        }

        private void AgregarConclusionesPDF(Document doc, DataTable dt)
        {
            AgregarTituloSeccion(doc, "CONCLUSIONES Y RECOMENDACIONES");

            doc.Add(new Paragraph("Resumen General",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK)));
            doc.Add(new Paragraph(" "));

            var bullet = new List(List.UNORDERED);
            bullet.SetListSymbol("\u2022");
            bullet.IndentationLeft = 20;

            bullet.Add(new ListItem($"Se analizaron un total de {dt.Rows.Count:N0} registros de auditoría.",
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            bullet.Add(new ListItem($"Fecha de generación del reporte: {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            bullet.Add(new ListItem($"Tipo de análisis realizado: {ObtenerNombrePestanaActiva()}",
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));

            doc.Add(bullet);
            doc.Add(new Paragraph("\n"));

            doc.Add(new Paragraph("Hallazgos Principales",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK)));
            doc.Add(new Paragraph(" "));

            var hallazgos = new List(List.UNORDERED);
            hallazgos.SetListSymbol("\u2713");
            hallazgos.IndentationLeft = 20;

            hallazgos.Add(new ListItem("Los datos reflejan la actividad completa del sistema en el período analizado.",
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            hallazgos.Add(new ListItem("Se recomienda revisar las secciones de análisis para información detallada por categoría.",
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            hallazgos.Add(new ListItem("Este reporte puede ser utilizado para auditorías internas, análisis de seguridad y toma de decisiones.",
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            hallazgos.Add(new ListItem("Se sugiere mantener un monitoreo continuo de las actividades del sistema.",
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));

            doc.Add(hallazgos);
            doc.Add(new Paragraph("\n\n"));

            // Firma
            var firma = new Paragraph($"Generado automáticamente por MOFIS-ERP\n© {DateTime.Now.Year} Fiducorp - Todos los derechos reservados",
                FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 9, BaseColor.GRAY));
            firma.Alignment = Element.ALIGN_CENTER;
            doc.Add(firma);
        }

        // Métodos auxiliares de formato
        private void AgregarTituloSeccion(Document doc, string titulo)
        {
            var p = new Paragraph(titulo, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(0, 120, 212)));
            p.SpacingBefore = 10;
            p.SpacingAfter = 15;
            p.Alignment = Element.ALIGN_CENTER;

            var linea = new LineSeparator(1f, 100f, new BaseColor(0, 120, 212), Element.ALIGN_CENTER, -2);

            doc.Add(p);
            doc.Add(new Chunk(linea));
            doc.Add(new Paragraph(" "));
        }

        private void AgregarCeldaHeader(PdfPTable tabla, string texto)
        {
            var cell = new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE)));
            cell.BackgroundColor = new BaseColor(0, 120, 212);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 8;
            tabla.AddCell(cell);
        }

        private void AgregarCeldaDato(PdfPTable tabla, string texto, bool resaltar = false)
        {
            var cell = new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA, 8)));
            cell.Padding = 5;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            if (resaltar)
                cell.BackgroundColor = new BaseColor(255, 250, 205);

            tabla.AddCell(cell);
        }

        // =======================================================// =======================================================

        // ============================================================
        // EVENTOS DE DATAGRIDVIEWS - DOBLE CLIC PARA VER DETALLES
        // ============================================================

        // Doble clic en DataGridView por Usuario
        private void DgvResultados_DoubleClick(object sender, EventArgs e)
        {
            if (dgvResultadosUsuario.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow filaSeleccionada = dgvResultadosUsuario.SelectedRows[0];
                DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;

                if (rowView != null)
                {
                    DataRow row = rowView.Row;

                    // Abrir formulario de detalle (reutilizamos FormDetalleAuditoria)
                    using (var formDetalle = new FormDetalleAuditoria(row))
                    {
                        formDetalle.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalle:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Doble clic en DataGridView por Categoría
        private void DgvResultadosCategoria_DoubleClick(object sender, EventArgs e)
        {
            if (dgvResultadosCategoria.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow filaSeleccionada = dgvResultadosCategoria.SelectedRows[0];
                DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;

                if (rowView != null)
                {
                    DataRow row = rowView.Row;

                    using (var formDetalle = new FormDetalleAuditoria(row))
                    {
                        formDetalle.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalle:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Doble clic en DataGridView por Módulo
        private void DgvResultadosModulo_DoubleClick(object sender, EventArgs e)
        {
            if (dgvResultadosModulo.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow filaSeleccionada = dgvResultadosModulo.SelectedRows[0];
                DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;

                if (rowView != null)
                {
                    DataRow row = rowView.Row;

                    using (var formDetalle = new FormDetalleAuditoria(row))
                    {
                        formDetalle.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalle:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Doble clic en DataGridView por Acción
        private void DgvResultadosAccion_DoubleClick(object sender, EventArgs e)
        {
            if (dgvResultadosAccion.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow filaSeleccionada = dgvResultadosAccion.SelectedRows[0];
                DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;

                if (rowView != null)
                {
                    DataRow row = rowView.Row;
                    using (var formDetalle = new FormDetalleAuditoria(row))
                    {
                        formDetalle.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalle:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Doble clic en DataGridView por Horario
        private void DgvResultadosHorario_DoubleClick(object sender, EventArgs e)
        {
            if (dgvResultadosHorario.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow filaSeleccionada = dgvResultadosHorario.SelectedRows[0];
                DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;

                if (rowView != null)
                {
                    DataRow row = rowView.Row;
                    using (var formDetalle = new FormDetalleAuditoria(row))
                    {
                        formDetalle.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalle:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Doble clic en DataGridView por IP/Máquina
        private void DgvResultadosIP_DoubleClick(object sender, EventArgs e)
        {
            if (dgvResultadosIP.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow filaSeleccionada = dgvResultadosIP.SelectedRows[0];
                DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;

                if (rowView != null)
                {
                    DataRow row = rowView.Row;
                    using (var formDetalle = new FormDetalleAuditoria(row))
                    {
                        formDetalle.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalle:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Doble clic en DataGridView por Registro
        private void DgvResultadosRegistro_DoubleClick(object sender, EventArgs e)
        {
            if (dgvResultadosRegistro.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow filaSeleccionada = dgvResultadosRegistro.SelectedRows[0];
                DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;

                if (rowView != null)
                {
                    DataRow row = rowView.Row;
                    using (var formDetalle = new FormDetalleAuditoria(row))
                    {
                        formDetalle.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalle:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar datos iniciales de la base de datos
        private async System.Threading.Tasks.Task CargarDatosInicialesAsync()
        {
            try
            {
                this.SuspendLayout();

                lblTotalRegistros.Text = "⏳ Cargando datos...";
                this.Cursor = Cursors.WaitCursor;

                await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        // Cargar categorías
                        cacheCategorias = new List<string>();
                        string sqlCategorias = "SELECT DISTINCT Categoria FROM Auditoria WHERE Categoria IS NOT NULL ORDER BY Categoria";
                        using (var cmd = new SqlCommand(sqlCategorias, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cacheCategorias.Add(reader.GetString(0));
                            }
                        }

                        // Cargar módulos
                        cacheModulos = new List<string>();
                        string sqlModulos = "SELECT DISTINCT Modulo FROM Auditoria WHERE Modulo IS NOT NULL ORDER BY Modulo";
                        using (var cmd = new SqlCommand(sqlModulos, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cacheModulos.Add(reader.GetString(0));
                            }
                        }

                        // Cargar acciones
                        cacheAcciones = new List<string>();
                        string sqlAcciones = "SELECT DISTINCT Accion FROM Auditoria WHERE Accion IS NOT NULL ORDER BY Accion";
                        using (var cmd = new SqlCommand(sqlAcciones, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cacheAcciones.Add(reader.GetString(0));
                            }
                        }

                        // Cargar usuarios
                        var usuarios = new List<ComboBoxUsuario>();
                        string sqlUsuarios = @"
                    SELECT UsuarioID, Username, NombreCompleto
                    FROM Usuarios
                    WHERE Activo = 1 AND EsEliminado = 0
                    ORDER BY NombreCompleto";

                        using (var cmd = new SqlCommand(sqlUsuarios, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new ComboBoxUsuario
                                {
                                    UsuarioID = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    NombreCompleto = reader.GetString(2)
                                });
                            }
                        }

                        // Cargar datos de auditoría (ÚLTIMOS 3 MESES inicialmente)
                        dtAuditoriaCompleta = new DataTable();
                        string sqlAuditoria = @"
                    SELECT 
                        A.AuditoriaID,
                        A.FechaHora,
                        A.UsuarioID,
                        U.Username,
                        A.Categoria,
                        A.Modulo,
                        A.Formulario,
                        A.Accion,
                        A.Detalle,
                        A.DireccionIP,
                        A.NombreMaquina
                    FROM Auditoria A WITH (NOLOCK)
                    LEFT JOIN Usuarios U WITH (NOLOCK) ON A.UsuarioID = U.UsuarioID
                    WHERE A.FechaHora >= DATEADD(MONTH, -3, GETDATE())
                    ORDER BY A.FechaHora DESC";

                        using (var cmd = new SqlCommand(sqlAuditoria, conn))
                        {
                            cmd.CommandTimeout = 60;
                            using (var adapter = new SqlDataAdapter(cmd))
                            {
                                adapter.Fill(dtAuditoriaCompleta);
                            }
                        }

                        // Verificar si hay datos más antiguos
                        bool hayDatosAntiguos = false;
                        string sqlVerificar = @"
                    SELECT COUNT(*) 
                    FROM Auditoria WITH (NOLOCK)
                    WHERE FechaHora < DATEADD(MONTH, -3, GETDATE())";

                        using (var cmd = new SqlCommand(sqlVerificar, conn))
                        {
                            int cantidadAntiguos = (int)cmd.ExecuteScalar();
                            hayDatosAntiguos = cantidadAntiguos > 0;
                        }

                        // Actualizar UI en el hilo principal
                        this.Invoke((MethodInvoker)delegate
                        {
                            // Llenar ComboBox de Categorías
                            cmbCategoria.Items.Clear();
                            cmbCategoria.Items.Add("Todas las Categorías");
                            foreach (var cat in cacheCategorias)
                            {
                                cmbCategoria.Items.Add(cat);
                            }
                            cmbCategoria.SelectedIndex = 0;

                            // Llenar ComboBox de Módulos
                            cmbModulo.Items.Clear();
                            cmbModulo.Items.Add("Todos los Módulos");
                            foreach (var mod in cacheModulos)
                            {
                                cmbModulo.Items.Add(mod);
                            }
                            cmbModulo.SelectedIndex = 0;

                            // Llenar ComboBox de Acciones
                            cmbAccion.Items.Clear();
                            cmbAccion.Items.Add("Todas las Acciones");
                            foreach (var acc in cacheAcciones)
                            {
                                cmbAccion.Items.Add(acc);
                            }
                            cmbAccion.SelectedIndex = 0;

                            // Llenar ComboBox de Usuarios
                            cmbUsuario.Items.Clear();
                            cmbUsuario.Items.Add("Seleccione un usuario...");
                            foreach (var user in usuarios)
                            {
                                cmbUsuario.Items.Add(user);
                            }
                            cmbUsuario.DisplayMember = "Display";
                            cmbUsuario.SelectedIndex = 0;

                            // Llenar ComboBoxes de otras pestañas (Por Categoría)
                            cmbCategoriaFiltro.Items.Clear();
                            cmbCategoriaFiltro.Items.Add("Seleccione una categoría...");
                            foreach (var cat in cacheCategorias)
                            {
                                cmbCategoriaFiltro.Items.Add(cat);
                            }
                            cmbCategoriaFiltro.SelectedIndex = 0;

                            cmbUsuarioCategoria.Items.Clear();
                            cmbUsuarioCategoria.Items.Add("Todos los Usuarios");
                            foreach (var user in usuarios)
                            {
                                cmbUsuarioCategoria.Items.Add(user);
                            }
                            cmbUsuarioCategoria.DisplayMember = "Display";
                            cmbUsuarioCategoria.SelectedIndex = 0;

                            cmbModuloCategoria.Items.Clear();
                            cmbModuloCategoria.Items.Add("Todos los Módulos");
                            foreach (var mod in cacheModulos)
                            {
                                cmbModuloCategoria.Items.Add(mod);
                            }
                            cmbModuloCategoria.SelectedIndex = 0;

                            cmbAccionCategoria.Items.Clear();
                            cmbAccionCategoria.Items.Add("Todas las Acciones");
                            foreach (var acc in cacheAcciones)
                            {
                                cmbAccionCategoria.Items.Add(acc);
                            }
                            cmbAccionCategoria.SelectedIndex = 0;

                            // Llenar ComboBoxes de pestaña Por Módulo
                            cmbCategoriaModulo.Items.Clear();
                            cmbCategoriaModulo.Items.Add("Todas las Categorías");
                            foreach (var cat in cacheCategorias)
                            {
                                cmbCategoriaModulo.Items.Add(cat);
                            }
                            cmbCategoriaModulo.SelectedIndex = 0;

                            cmbModuloFiltro.Items.Clear();
                            cmbModuloFiltro.Items.Add("Seleccione un módulo...");
                            foreach (var mod in cacheModulos)
                            {
                                cmbModuloFiltro.Items.Add(mod);
                            }
                            cmbModuloFiltro.SelectedIndex = 0;

                            cmbFormularioModulo.Items.Clear();
                            cmbFormularioModulo.Items.Add("Todos los Formularios");
                            cmbFormularioModulo.SelectedIndex = 0;

                            cmbUsuarioModulo.Items.Clear();
                            cmbUsuarioModulo.Items.Add("Todos los Usuarios");
                            foreach (var user in usuarios)
                            {
                                cmbUsuarioModulo.Items.Add(user);
                            }
                            cmbUsuarioModulo.DisplayMember = "Display";
                            cmbUsuarioModulo.SelectedIndex = 0;

                            cmbAccionModulo.Items.Clear();
                            cmbAccionModulo.Items.Add("Todas las Acciones");
                            foreach (var acc in cacheAcciones)
                            {
                                cmbAccionModulo.Items.Add(acc);
                            }
                            cmbAccionModulo.SelectedIndex = 0;

                            // Llenar ComboBoxes de pestaña Por Acción
                            cmbAccionFiltro.Items.Clear();
                            cmbAccionFiltro.Items.Add("Seleccione una acción...");
                            foreach (var acc in cacheAcciones)
                            {
                                cmbAccionFiltro.Items.Add(acc);
                            }
                            cmbAccionFiltro.SelectedIndex = 0;

                            cmbUsuarioAccion.Items.Clear();
                            cmbUsuarioAccion.Items.Add("Todos los Usuarios");
                            foreach (var user in usuarios)
                            {
                                cmbUsuarioAccion.Items.Add(user);
                            }
                            cmbUsuarioAccion.DisplayMember = "Display";
                            cmbUsuarioAccion.SelectedIndex = 0;

                            cmbCategoriaAccion.Items.Clear();
                            cmbCategoriaAccion.Items.Add("Todas las Categorías");
                            foreach (var cat in cacheCategorias)
                            {
                                cmbCategoriaAccion.Items.Add(cat);
                            }
                            cmbCategoriaAccion.SelectedIndex = 0;

                            cmbModuloAccion.Items.Clear();
                            cmbModuloAccion.Items.Add("Todos los Módulos");
                            foreach (var mod in cacheModulos)
                            {
                                cmbModuloAccion.Items.Add(mod);
                            }
                            cmbModuloAccion.SelectedIndex = 0;

                            // Llenar ComboBoxes de pestaña Por Horario
                            cmbUsuarioHorario.Items.Clear();
                            cmbUsuarioHorario.Items.Add("Todos los Usuarios");
                            foreach (var user in usuarios)
                            {
                                cmbUsuarioHorario.Items.Add(user);
                            }
                            cmbUsuarioHorario.DisplayMember = "Display";
                            cmbUsuarioHorario.SelectedIndex = 0;

                            cmbCategoriaHorario.Items.Clear();
                            cmbCategoriaHorario.Items.Add("Todas las Categorías");
                            foreach (var cat in cacheCategorias)
                            {
                                cmbCategoriaHorario.Items.Add(cat);
                            }
                            cmbCategoriaHorario.SelectedIndex = 0;

                            cmbModuloHorario.Items.Clear();
                            cmbModuloHorario.Items.Add("Todos los Módulos");
                            foreach (var mod in cacheModulos)
                            {
                                cmbModuloHorario.Items.Add(mod);
                            }
                            cmbModuloHorario.SelectedIndex = 0;

                            cmbAccionHorario.Items.Clear();
                            cmbAccionHorario.Items.Add("Todas las Acciones");
                            foreach (var acc in cacheAcciones)
                            {
                                cmbAccionHorario.Items.Add(acc);
                            }
                            cmbAccionHorario.SelectedIndex = 0;

                            // Llenar ComboBoxes de pestaña Por IP
                            cmbUsuarioIP.Items.Clear();
                            cmbUsuarioIP.Items.Add("Todos los Usuarios");
                            foreach (var user in usuarios)
                            {
                                cmbUsuarioIP.Items.Add(user);
                            }
                            cmbUsuarioIP.DisplayMember = "Display";
                            cmbUsuarioIP.SelectedIndex = 0;

                            cmbCategoriaIP.Items.Clear();
                            cmbCategoriaIP.Items.Add("Todas las Categorías");
                            foreach (var cat in cacheCategorias)
                            {
                                cmbCategoriaIP.Items.Add(cat);
                            }
                            cmbCategoriaIP.SelectedIndex = 0;

                            cmbModuloIP.Items.Clear();
                            cmbModuloIP.Items.Add("Todos los Módulos");
                            foreach (var mod in cacheModulos)
                            {
                                cmbModuloIP.Items.Add(mod);
                            }
                            cmbModuloIP.SelectedIndex = 0;

                            cmbAccionIP.Items.Clear();
                            cmbAccionIP.Items.Add("Todas las Acciones");
                            foreach (var acc in cacheAcciones)
                            {
                                cmbAccionIP.Items.Add(acc);
                            }
                            cmbAccionIP.SelectedIndex = 0;

                            // Llenar ComboBoxes de pestaña Por Registro
                            cmbModuloRegistro.Items.Clear();
                            cmbModuloRegistro.Items.Add("Todos los Módulos");
                            foreach (var mod in cacheModulos)
                            {
                                cmbModuloRegistro.Items.Add(mod);
                            }
                            cmbModuloRegistro.SelectedIndex = 0;

                            // Llenar tipos de búsqueda dinámicamente
                            CargarTiposBusquedaDinamicos(conn);

                            // Mostrar información de carga
                            string mensajeCarga = $"Cargados: {dtAuditoriaCompleta.Rows.Count:N0} registros (últimos 3 meses)";

                            if (hayDatosAntiguos)
                            {
                                btnCargarMasDatos.Visible = true;
                                mensajeCarga += " - Hay datos más antiguos disponibles";
                            }

                            lblTotalRegistros.Text = mensajeCarga;
                            lblFechaConsulta.Text = $"Última consulta: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                        });
                    }
                });

                this.Cursor = Cursors.Default;
                this.ResumeLayout();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                this.ResumeLayout();
                MessageBox.Show($"Error al cargar datos:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar tipos de búsqueda dinámicos desde la base de datos
        private void CargarTiposBusquedaDinamicos(SqlConnection conn)
        {
            try
            {
                cmbTipoBusqueda.Items.Clear();
                cmbTipoBusqueda.Items.Add("Seleccione tipo de búsqueda...");

                // Siempre agregar AuditoriaID
                cmbTipoBusqueda.Items.Add("AuditoriaID (Registro de Auditoría)");

                // FORZAR la inclusión de UsuarioID (el más importante)
                var tiposIDEncontrados = new HashSet<string>();

                // Verificar explícitamente si hay registros con UsuarioID
                string sqlVerificarUsuario = @"
            SELECT COUNT(*) 
            FROM Auditoria WITH (NOLOCK)
            WHERE Detalle LIKE '%usuario%' 
                AND (
                    Detalle LIKE '%UsuarioID%' OR 
                    Detalle LIKE '%Usuario ID%' OR
                    Detalle LIKE '%usuarioID%' OR
                    Detalle LIKE '%ID de usuario%' OR
                    Detalle LIKE '%usuario:%' OR
                    Detalle LIKE '%Usuario:%'
                )";

                using (var cmd = new SqlCommand(sqlVerificarUsuario, conn))
                {
                    int cantidadUsuario = (int)cmd.ExecuteScalar();
                    if (cantidadUsuario > 0)
                    {
                        tiposIDEncontrados.Add("UsuarioID");
                    }
                }

                // Verificar otros tipos conocidos
                var tiposConocidos = new Dictionary<string, string>
        {
            { "RolID", "%rol%" },
            { "PermisoID", "%permiso%" },
            { "CategoriaID", "%categoria%" },
            { "ModuloID", "%modulo%" },
            { "FormularioID", "%formulario%" },
            { "AccionID", "%accion%" },
            { "PermisoRolID", "%permisorol%" },
            { "PermisoUsuarioID", "%permisousuario%" }
        };

                foreach (var tipo in tiposConocidos)
                {
                    string sqlVerificar = $@"
                SELECT COUNT(*) 
                FROM Auditoria WITH (NOLOCK)
                WHERE Detalle LIKE '{tipo.Value}'";

                    using (var cmd = new SqlCommand(sqlVerificar, conn))
                    {
                        cmd.CommandTimeout = 30;
                        int cantidad = (int)cmd.ExecuteScalar();
                        if (cantidad > 0)
                        {
                            tiposIDEncontrados.Add(tipo.Key);
                        }
                    }
                }

                // ADEMÁS, hacer búsqueda con regex en una muestra
                string sql = @"
            SELECT TOP 1000 Detalle
            FROM Auditoria WITH (NOLOCK)
            WHERE Detalle IS NOT NULL 
                AND Detalle != ''
                AND (
                    Detalle LIKE '%ID:%' OR 
                    Detalle LIKE '%Id:%' OR
                    Detalle LIKE '%id:%' OR
                    Detalle LIKE '% ID %'
                )
            ORDER BY AuditoriaID DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 60;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                string detalle = reader.GetString(0);

                                // Múltiples patrones de búsqueda
                                var patterns = new[]
                                {
                            @"(\w+)ID\s*:\s*\d+",                    // UsuarioID: 123
                            @"(\w+)\s+ID\s*:\s*\d+",                 // Usuario ID: 123
                            @"ID\s+de\s+(\w+)\s*:\s*\d+",            // ID de Usuario: 123
                            @"(\w+)ID\s*=\s*\d+",                    // UsuarioID=123
                            @"(\w+)\s+ID\s*=\s*\d+",                 // Usuario ID=123
                            @"(\w+)\s*:\s*\d+",                      // Usuario: 123
                            @"ID\s*:\s*(\w+)\s*-\s*\d+",             // ID: Usuario-123
                            @"para\s+(\w+)\s+ID\s*:\s*\d+",          // para Usuario ID: 123
                            @"del\s+(\w+)\s+ID\s*:\s*\d+",           // del Usuario ID: 123
                            @"modificados\s+para\s+(\w+)\s+ID",      // modificados para rol ID
                            @"creado\s+con\s+(\w+)\s+ID",            // creado con rol ID
                            @"(\w+)\s+creado",                       // Usuario creado
                            @"(\w+)\s+modificado",                   // Usuario modificado
                            @"(\w+)\s+eliminado",                    // Usuario eliminado
                        };

                                foreach (var pattern in patterns)
                                {
                                    var matches = System.Text.RegularExpressions.Regex.Matches(
                                        detalle,
                                        pattern,
                                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                                    );

                                    foreach (System.Text.RegularExpressions.Match match in matches)
                                    {
                                        if (match.Groups.Count > 1)
                                        {
                                            string palabra = match.Groups[1].Value.Trim();

                                            // Ignorar palabras comunes que no son tipos
                                            var palabrasIgnorar = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                    {
                                        "el", "la", "los", "las", "un", "una", "para", "con", "de", "del",
                                        "registro", "campo", "valor", "dato", "nuevo", "anterior"
                                    };

                                            if (palabrasIgnorar.Contains(palabra))
                                                continue;

                                            // Normalizar
                                            palabra = char.ToUpper(palabra[0]) + palabra.Substring(1).ToLower();

                                            // Crear tipo ID
                                            string tipoID;
                                            if (!palabra.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                                            {
                                                tipoID = palabra + "ID";
                                            }
                                            else
                                            {
                                                tipoID = palabra.Substring(0, palabra.Length - 2) + "ID";
                                            }

                                            // Lista blanca de tipos permitidos
                                            var tiposPermitidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                    {
                                        "UsuarioID", "RolID", "PermisoID", "CategoriaID",
                                        "ModuloID", "FormularioID", "AccionID", "PermisoRolID",
                                        "PermisoUsuarioID"
                                    };

                                            if (tiposPermitidos.Contains(tipoID))
                                            {
                                                tiposIDEncontrados.Add(tipoID);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Remover AuditoriaID si se encontró
                tiposIDEncontrados.Remove("AuditoriaID");

                // GARANTIZAR que UsuarioID siempre esté presente (es el más importante)
                if (!tiposIDEncontrados.Contains("UsuarioID"))
                {
                    // Forzar su inclusión porque es crítico
                    tiposIDEncontrados.Add("UsuarioID");
                }

                // Ordenar: UsuarioID primero, luego alfabéticamente
                var tiposOrdenados = tiposIDEncontrados.OrderBy(t => t == "UsuarioID" ? "0" : t).ToList();

                // Agregar al ComboBox
                foreach (var tipo in tiposOrdenados)
                {
                    string descripcion = ObtenerDescripcionTipoID(tipo);
                    cmbTipoBusqueda.Items.Add($"{tipo} ({descripcion})");
                }

                // Siempre agregar "Otro ID" al final
                cmbTipoBusqueda.Items.Add("Otro ID (Búsqueda genérica)");

                cmbTipoBusqueda.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Fallback con valores mínimos garantizados
                cmbTipoBusqueda.Items.Clear();
                cmbTipoBusqueda.Items.Add("Seleccione tipo de búsqueda...");
                cmbTipoBusqueda.Items.Add("AuditoriaID (Registro de Auditoría)");
                cmbTipoBusqueda.Items.Add("UsuarioID (Usuario del Sistema)");
                cmbTipoBusqueda.Items.Add("RolID (Rol del Sistema)");
                cmbTipoBusqueda.Items.Add("Otro ID (Búsqueda genérica)");
                cmbTipoBusqueda.SelectedIndex = 0;

                System.Diagnostics.Debug.WriteLine($"Error al cargar tipos de búsqueda: {ex.Message}");
            }
        }

        // Obtener descripción legible para cada tipo de ID
        private string ObtenerDescripcionTipoID(string tipoID)
        {
            // Diccionario basado en tu esquema de BD
            var descripciones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "UsuarioID", "Usuario del Sistema" },
        { "RolID", "Rol del Sistema" },
        { "PermisoID", "Permiso (Legacy)" },
        { "CategoriaID", "Categoría del Sistema" },
        { "ModuloID", "Módulo del Sistema" },
        { "FormularioID", "Formulario del Sistema" },
        { "AccionID", "Acción del Sistema" },
        { "PermisoRolID", "Permiso de Rol" },
        { "PermisoUsuarioID", "Permiso de Usuario" }
    };

            if (descripciones.ContainsKey(tipoID))
            {
                return descripciones[tipoID];
            }

            // Generar descripción automática
            string nombreBase = tipoID.Replace("ID", "");
            return $"{nombreBase} del Sistema";
        }

        // ============================================================
        // BÚSQUEDA CON DELAY
        // ============================================================

        // Iniciar búsqueda con delay para evitar múltiples consultas rápidas
        private void IniciarBusquedaConDelay()
        {
            busquedaTimer.Stop();
            busquedaTimer.Tick -= BusquedaTimer_Tick; // Remover handler anterior
            busquedaTimer.Tick += BusquedaTimer_Tick; // Agregar nuevo handler
            busquedaTimer.Start();
        }

        // Evento del timer para ejecutar la búsquedas
        private void BusquedaTimer_Tick(object sender, EventArgs e)
        {
            busquedaTimer.Stop();

            // Detectar pestaña activa y aplicar filtros correspondientes
            if (tabControlModos.SelectedTab == tabPorUsuario)
            {
                AplicarFiltrosUsuario();
            }
            else if (tabControlModos.SelectedTab == tabPorCategoria)
            {
                AplicarFiltrosCategoria();
            }
            else if (tabControlModos.SelectedTab == tabPorModulo)
            {
                AplicarFiltrosModulo();
            }
            else if (tabControlModos.SelectedTab == tabPorAccion)
            {
                AplicarFiltrosAccion();
            }
            else if (tabControlModos.SelectedTab == tabPorHorario)
            {
                AplicarFiltrosHorario();
            }
            else if (tabControlModos.SelectedTab == tabPorIP)
            {
                AplicarFiltrosIP();
            }
            else if (tabControlModos.SelectedTab == tabPorRegistro)
            {
                AplicarFiltrosRegistro();
            }
        }

        // ============================================================
        // FILTROS Y BÚSQUEDAS
        // ============================================================

        // Pestaña Por Usuario
        private void AplicarFiltrosUsuario()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Si no hay usuario seleccionado, no mostrar nada
                if (cmbUsuario.SelectedIndex <= 0)
                {
                    dgvResultadosUsuario.DataSource = null;
                    rtbEstadisticasUsuario.Clear();
                    lblTotalRegistros.Text = "Total: 0 registros";
                    return;
                }

                var usuarioSeleccionado = cmbUsuario.SelectedItem as ComboBoxUsuario;

                // Filtrar datos
                var filtros = new List<string>();
                filtros.Add($"Username = '{usuarioSeleccionado.Username.Replace("'", "''")}'");

                // Filtro de fechas
                if (dtpDesde.Value.Date <= dtpHasta.Value.Date)
                {
                    string fechaDesde = dtpDesde.Value.Date.ToString("yyyy-MM-dd");
                    string fechaHasta = dtpHasta.Value.Date.AddDays(1).ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}# AND FechaHora < #{fechaHasta}#");
                }

                // Filtro de acción
                if (cmbAccion.SelectedIndex > 0)
                {
                    filtros.Add($"Accion = '{cmbAccion.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de categoría
                if (cmbCategoria.SelectedIndex > 0)
                {
                    filtros.Add($"Categoria = '{cmbCategoria.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de módulo
                if (cmbModulo.SelectedIndex > 0)
                {
                    filtros.Add($"Modulo = '{cmbModulo.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de búsqueda rápida
                if (!string.IsNullOrWhiteSpace(txtBusquedaRapida.Text) && txtBusquedaRapida.Text != "Buscar...")
                {
                    string busqueda = txtBusquedaRapida.Text.Replace("'", "''");
                    filtros.Add($"(Detalle LIKE '%{busqueda}%' OR Accion LIKE '%{busqueda}%' OR Modulo LIKE '%{busqueda}%' OR Formulario LIKE '%{busqueda}%')");
                }

                string filtroFinal = string.Join(" AND ", filtros);
                DataView dv = new DataView(dtAuditoriaCompleta);
                dv.RowFilter = filtroFinal;

                dgvResultadosUsuario.DataSource = dv;

                lblTotalRegistros.Text = $"Total: {dv.Count:N0} registros";

                // Generar estadísticas
                GenerarEstadisticasUsuario(dv, usuarioSeleccionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pestaña Por Categoría
        private void AplicarFiltrosCategoria()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Si no hay categoría seleccionada, no mostrar nada
                if (cmbCategoriaFiltro.SelectedIndex <= 0)
                {
                    dgvResultadosCategoria.DataSource = null;
                    rtbEstadisticasCategoria.Clear();
                    lblTotalRegistros.Text = "Total: 0 registros";
                    return;
                }

                string categoriaSeleccionada = cmbCategoriaFiltro.SelectedItem.ToString();

                // Filtrar datos
                var filtros = new List<string>();
                filtros.Add($"Categoria = '{categoriaSeleccionada.Replace("'", "''")}'");

                // Filtro de fechas
                if (dtpDesdeCategoria.Value.Date <= dtpHastaCategoria.Value.Date)
                {
                    string fechaDesde = dtpDesdeCategoria.Value.Date.ToString("yyyy-MM-dd");
                    string fechaHasta = dtpHastaCategoria.Value.Date.AddDays(1).ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}# AND FechaHora < #{fechaHasta}#");
                }

                // Filtro de usuario
                if (cmbUsuarioCategoria.SelectedIndex > 0)
                {
                    var usuario = cmbUsuarioCategoria.SelectedItem as ComboBoxUsuario;
                    filtros.Add($"Username = '{usuario.Username.Replace("'", "''")}'");
                }

                // Filtro de módulo
                if (cmbModuloCategoria.SelectedIndex > 0)
                {
                    filtros.Add($"Modulo = '{cmbModuloCategoria.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de acción
                if (cmbAccionCategoria.SelectedIndex > 0)
                {
                    filtros.Add($"Accion = '{cmbAccionCategoria.SelectedItem.ToString().Replace("'", "''")}'");
                }

                string filtroFinal = string.Join(" AND ", filtros);
                DataView dv = new DataView(dtAuditoriaCompleta);
                dv.RowFilter = filtroFinal;
                dv.Sort = "FechaHora DESC";

                dgvResultadosCategoria.DataSource = dv;

                lblTotalRegistros.Text = $"Total: {dv.Count:N0} registros";

                // Generar estadísticas
                GenerarEstadisticasCategoria(dv, categoriaSeleccionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pestaña Por Módulo
        private void AplicarFiltrosModulo()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Si no hay módulo seleccionado, no mostrar nada
                if (cmbModuloFiltro.SelectedIndex <= 0)
                {
                    dgvResultadosModulo.DataSource = null;
                    rtbEstadisticasModulo.Clear();
                    lblTotalRegistros.Text = "Total: 0 registros";
                    return;
                }

                string moduloSeleccionado = cmbModuloFiltro.SelectedItem.ToString();

                // Filtrar datos
                var filtros = new List<string>();
                filtros.Add($"Modulo = '{moduloSeleccionado.Replace("'", "''")}'");

                // Filtro de categoría (si está seleccionada)
                if (cmbCategoriaModulo.SelectedIndex > 0)
                {
                    filtros.Add($"Categoria = '{cmbCategoriaModulo.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de formulario
                if (cmbFormularioModulo.SelectedIndex > 0)
                {
                    filtros.Add($"Formulario = '{cmbFormularioModulo.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de fechas
                if (dtpDesdeModulo.Value.Date <= dtpHastaModulo.Value.Date)
                {
                    string fechaDesde = dtpDesdeModulo.Value.Date.ToString("yyyy-MM-dd");
                    string fechaHasta = dtpHastaModulo.Value.Date.AddDays(1).ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}# AND FechaHora < #{fechaHasta}#");
                }

                // Filtro de usuario
                if (cmbUsuarioModulo.SelectedIndex > 0)
                {
                    var usuario = cmbUsuarioModulo.SelectedItem as ComboBoxUsuario;
                    filtros.Add($"Username = '{usuario.Username.Replace("'", "''")}'");
                }

                // Filtro de acción
                if (cmbAccionModulo.SelectedIndex > 0)
                {
                    filtros.Add($"Accion = '{cmbAccionModulo.SelectedItem.ToString().Replace("'", "''")}'");
                }

                string filtroFinal = string.Join(" AND ", filtros);
                DataView dv = new DataView(dtAuditoriaCompleta);
                dv.RowFilter = filtroFinal;
                dv.Sort = "FechaHora DESC";

                dgvResultadosModulo.DataSource = dv;

                lblTotalRegistros.Text = $"Total: {dv.Count:N0} registros";

                // Generar estadísticas
                GenerarEstadisticasModulo(dv, moduloSeleccionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pestaña Por Acción
        private void AplicarFiltrosAccion()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Si no hay acción seleccionada, no mostrar nada
                if (cmbAccionFiltro.SelectedIndex <= 0)
                {
                    dgvResultadosAccion.DataSource = null;
                    rtbEstadisticasAccion.Clear();
                    lblTotalRegistros.Text = "Total: 0 registros";
                    return;
                }

                string accionSeleccionada = cmbAccionFiltro.SelectedItem.ToString();

                // Filtrar datos
                var filtros = new List<string>();
                filtros.Add($"Accion = '{accionSeleccionada.Replace("'", "''")}'");

                // Filtro de fechas
                if (dtpDesdeAccion.Value.Date <= dtpHastaAccion.Value.Date)
                {
                    string fechaDesde = dtpDesdeAccion.Value.Date.ToString("yyyy-MM-dd");
                    string fechaHasta = dtpHastaAccion.Value.Date.AddDays(1).ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}# AND FechaHora < #{fechaHasta}#");
                }

                // Filtro de usuario
                if (cmbUsuarioAccion.SelectedIndex > 0)
                {
                    var usuario = cmbUsuarioAccion.SelectedItem as ComboBoxUsuario;
                    filtros.Add($"Username = '{usuario.Username.Replace("'", "''")}'");
                }

                // Filtro de categoría
                if (cmbCategoriaAccion.SelectedIndex > 0)
                {
                    filtros.Add($"Categoria = '{cmbCategoriaAccion.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de módulo
                if (cmbModuloAccion.SelectedIndex > 0)
                {
                    filtros.Add($"Modulo = '{cmbModuloAccion.SelectedItem.ToString().Replace("'", "''")}'");
                }

                string filtroFinal = string.Join(" AND ", filtros);
                DataView dv = new DataView(dtAuditoriaCompleta);
                dv.RowFilter = filtroFinal;
                dv.Sort = "FechaHora DESC";

                dgvResultadosAccion.DataSource = dv;

                lblTotalRegistros.Text = $"Total: {dv.Count:N0} registros";

                // Generar estadísticas
                GenerarEstadisticasAccion(dv, accionSeleccionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pestaña Por Horario
        private void AplicarFiltrosHorario()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Filtrar datos
                var filtros = new List<string>();

                // Filtro de fecha
                string fecha = dtpFechaHorario.Value.Date.ToString("yyyy-MM-dd");
                string fechaSiguiente = dtpFechaHorario.Value.Date.AddDays(1).ToString("yyyy-MM-dd");
                filtros.Add($"FechaHora >= #{fecha}# AND FechaHora < #{fechaSiguiente}#");

                // Filtro de horario
                TimeSpan horaInicio = dtpHoraInicio.Value.TimeOfDay;
                TimeSpan horaFin = dtpHoraFin.Value.TimeOfDay;

                // Filtro de usuario
                if (cmbUsuarioHorario.SelectedIndex > 0)
                {
                    var usuario = cmbUsuarioHorario.SelectedItem as ComboBoxUsuario;
                    filtros.Add($"Username = '{usuario.Username.Replace("'", "''")}'");
                }

                // Filtro de categoría
                if (cmbCategoriaHorario.SelectedIndex > 0)
                {
                    filtros.Add($"Categoria = '{cmbCategoriaHorario.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de módulo
                if (cmbModuloHorario.SelectedIndex > 0)
                {
                    filtros.Add($"Modulo = '{cmbModuloHorario.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de acción
                if (cmbAccionHorario.SelectedIndex > 0)
                {
                    filtros.Add($"Accion = '{cmbAccionHorario.SelectedItem.ToString().Replace("'", "''")}'");
                }

                string filtroFinal = string.Join(" AND ", filtros);
                DataView dv = new DataView(dtAuditoriaCompleta);
                dv.RowFilter = filtroFinal;

                // Filtrar por horario (en memoria ya que DataView no soporta TimeOfDay)
                var resultadosFiltrados = dv.Cast<DataRowView>()
                    .Where(r =>
                    {
                        DateTime fechaHora = Convert.ToDateTime(r["FechaHora"]);
                        TimeSpan hora = fechaHora.TimeOfDay;

                        // Aplicar filtro de horario
                        bool dentroHorario = (horaInicio <= horaFin)
                            ? (hora >= horaInicio && hora <= horaFin)
                            : (hora >= horaInicio || hora <= horaFin);

                        if (!dentroHorario)
                            return false;

                        // Filtro de fin de semana
                        if (!chkIncluirFinDeSemana.Checked)
                        {
                            if (fechaHora.DayOfWeek == DayOfWeek.Saturday || fechaHora.DayOfWeek == DayOfWeek.Sunday)
                                return false;
                        }

                        // Filtro de fuera de oficina (antes de 8am o después de 6pm)
                        if (chkSoloFueraOficina.Checked)
                        {
                            TimeSpan inicioOficina = new TimeSpan(8, 0, 0);
                            TimeSpan finOficina = new TimeSpan(18, 0, 0);
                            if (hora >= inicioOficina && hora <= finOficina)
                                return false;
                        }

                        return true;
                    })
                    .ToList();

                // Crear DataTable con resultados filtrados
                DataTable dtFiltrado = dtAuditoriaCompleta.Clone();
                foreach (var row in resultadosFiltrados)
                {
                    dtFiltrado.ImportRow(row.Row);
                }

                DataView dvFinal = new DataView(dtFiltrado);
                dvFinal.Sort = "FechaHora DESC";

                dgvResultadosHorario.DataSource = dvFinal;

                lblTotalRegistros.Text = $"Total: {dvFinal.Count:N0} registros";

                // Generar estadísticas
                GenerarEstadisticasHorario(dvFinal, dtpFechaHorario.Value, horaInicio, horaFin);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pestaña Por IP/Máquina
        private void AplicarFiltrosIP()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Si no hay IP ni máquina, no mostrar nada
                bool tieneIP = !string.IsNullOrWhiteSpace(txtDireccionIP.Text);
                bool tieneMaquina = !string.IsNullOrWhiteSpace(txtNombreMaquina.Text);

                if (!tieneIP && !tieneMaquina)
                {
                    dgvResultadosIP.DataSource = null;
                    rtbEstadisticasIP.Clear();
                    lblTotalRegistros.Text = "Total: 0 registros";
                    return;
                }

                // Filtrar datos
                var filtros = new List<string>();

                // Filtro de IP
                if (tieneIP)
                {
                    string ip = txtDireccionIP.Text.Replace("'", "''");
                    filtros.Add($"DireccionIP LIKE '%{ip}%'");
                }

                // Filtro de máquina
                if (tieneMaquina)
                {
                    string maquina = txtNombreMaquina.Text.Replace("'", "''");
                    filtros.Add($"NombreMaquina LIKE '%{maquina}%'");
                }

                // Filtro de fechas
                if (dtpDesdeIP.Value.Date <= dtpHastaIP.Value.Date)
                {
                    string fechaDesde = dtpDesdeIP.Value.Date.ToString("yyyy-MM-dd");
                    string fechaHasta = dtpHastaIP.Value.Date.AddDays(1).ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}# AND FechaHora < #{fechaHasta}#");
                }

                // Filtro de usuario
                if (cmbUsuarioIP.SelectedIndex > 0)
                {
                    var usuario = cmbUsuarioIP.SelectedItem as ComboBoxUsuario;
                    filtros.Add($"Username = '{usuario.Username.Replace("'", "''")}'");
                }

                // Filtro de categoría
                if (cmbCategoriaIP.SelectedIndex > 0)
                {
                    filtros.Add($"Categoria = '{cmbCategoriaIP.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de módulo
                if (cmbModuloIP.SelectedIndex > 0)
                {
                    filtros.Add($"Modulo = '{cmbModuloIP.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de acción
                if (cmbAccionIP.SelectedIndex > 0)
                {
                    filtros.Add($"Accion = '{cmbAccionIP.SelectedItem.ToString().Replace("'", "''")}'");
                }

                string filtroFinal = string.Join(" AND ", filtros);
                DataView dv = new DataView(dtAuditoriaCompleta);
                dv.RowFilter = filtroFinal;
                dv.Sort = "FechaHora DESC";

                dgvResultadosIP.DataSource = dv;

                lblTotalRegistros.Text = $"Total: {dv.Count:N0} registros";

                // Generar estadísticas
                GenerarEstadisticasIP(dv, txtDireccionIP.Text, txtNombreMaquina.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pestaña Por Registro
        private void AplicarFiltrosRegistro()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Validar que haya tipo de búsqueda seleccionado
                if (cmbTipoBusqueda.SelectedIndex <= 0)
                {
                    dgvResultadosRegistro.DataSource = null;
                    rtbTimeline.Clear();
                    lblTotalRegistros.Text = "Total: 0 registros - Seleccione un tipo de búsqueda";
                    return;
                }

                // Validar que haya un valor de búsqueda
                if (string.IsNullOrWhiteSpace(txtValorBusqueda.Text))
                {
                    dgvResultadosRegistro.DataSource = null;
                    rtbTimeline.Clear();
                    lblTotalRegistros.Text = "Total: 0 registros - Ingrese un valor";
                    return;
                }

                string tipoBusqueda = cmbTipoBusqueda.SelectedItem.ToString();
                string valorBusqueda = txtValorBusqueda.Text.Trim();

                // Construir el filtro según el tipo de búsqueda
                var filtros = new List<string>();

                if (tipoBusqueda.Contains("AuditoriaID"))
                {
                    // Búsqueda directa por ID de auditoría
                    if (int.TryParse(valorBusqueda, out int auditoriaID))
                    {
                        filtros.Add($"AuditoriaID = {auditoriaID}");
                    }
                    else
                    {
                        MessageBox.Show("El AuditoriaID debe ser un número.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (tipoBusqueda.Contains("UsuarioID"))
                {
                    // Búsqueda por UsuarioID en el CAMPO UsuarioID de la tabla
                    if (int.TryParse(valorBusqueda, out int usuarioID))
                    {
                        filtros.Add($"UsuarioID = {usuarioID}");
                    }
                    else
                    {
                        MessageBox.Show("El UsuarioID debe ser un número.", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (tipoBusqueda.Contains("RolID"))
                {
                    // Buscar en el detalle para RolID
                    filtros.Add($"(Detalle LIKE '%RolID: {valorBusqueda.Replace("'", "''")}%' OR " +
                               $"Detalle LIKE '%Rol ID: {valorBusqueda.Replace("'", "''")}%' OR " +
                               $"Detalle LIKE '%rol ID: {valorBusqueda.Replace("'", "''")}%' OR " +
                               $"Detalle LIKE '%rol #{valorBusqueda.Replace("'", "''")}%')");
                }
                else if (tipoBusqueda.Contains("Otro ID"))
                {
                    // Búsqueda VERDADERAMENTE GENÉRICA
                    // Busca en TODOS los campos: AuditoriaID, UsuarioID, y en Detalle

                    if (int.TryParse(valorBusqueda, out int idNumerico))
                    {
                        // Si es un número, buscar en campos ID Y en el detalle
                        filtros.Add($"(" +
                                   $"AuditoriaID = {idNumerico} OR " +
                                   $"UsuarioID = {idNumerico} OR " +
                                   $"Detalle LIKE '%ID: {valorBusqueda.Replace("'", "''")}%' OR " +
                                   $"Detalle LIKE '%ID:{valorBusqueda.Replace("'", "''")}%' OR " +
                                   $"Detalle LIKE '%id: {valorBusqueda.Replace("'", "''")}%' OR " +
                                   $"Detalle LIKE '%Id: {valorBusqueda.Replace("'", "''")}%' OR " +
                                   $"Detalle LIKE '%#{valorBusqueda.Replace("'", "''")}%' OR " +
                                   $"Detalle LIKE '%ID {valorBusqueda.Replace("'", "''")} %' OR " +
                                   $"Detalle LIKE '%ID {valorBusqueda.Replace("'", "''")}.%' OR " +
                                   $"Detalle LIKE '%ID {valorBusqueda.Replace("'", "''")})%' OR " +
                                   $"Detalle LIKE '%ID {valorBusqueda.Replace("'", "''")},�%'" +
                                   $")");
                    }
                    else
                    {
                        // Si no es un número, buscar solo en Detalle (texto libre)
                        filtros.Add($"(" +
                                   $"Detalle LIKE '%{valorBusqueda.Replace("'", "''")}%'" +
                                   $")");
                    }
                }

                // Filtro de módulo
                if (cmbModuloRegistro.SelectedIndex > 0)
                {
                    filtros.Add($"Modulo = '{cmbModuloRegistro.SelectedItem.ToString().Replace("'", "''")}'");
                }

                // Filtro de fechas (opcionales)
                if (dtpDesdeRegistro.Checked && dtpHastaRegistro.Checked)
                {
                    string fechaDesde = dtpDesdeRegistro.Value.Date.ToString("yyyy-MM-dd");
                    string fechaHasta = dtpHastaRegistro.Value.Date.AddDays(1).ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}# AND FechaHora < #{fechaHasta}#");
                }

                string filtroFinal = string.Join(" AND ", filtros);
                DataView dv = new DataView(dtAuditoriaCompleta);
                dv.RowFilter = filtroFinal;
                dv.Sort = "FechaHora ASC";

                // Aplicar filtros de horario (en memoria)
                var resultadosFiltrados = dv.Cast<DataRowView>().AsEnumerable();

                // Filtro de fines de semana
                if (chkSoloFinesSemana.Checked)
                {
                    resultadosFiltrados = resultadosFiltrados.Where(r =>
                    {
                        DateTime fecha = Convert.ToDateTime(r["FechaHora"]);
                        return fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday;
                    });
                }

                // Filtro de horario
                if (rbSinFiltroHorario.Checked) // Ahora es "Personalizado"
                {
                    // PERSONALIZADO - Aplicar rango de horas manual
                    TimeSpan horaInicio = dtpHoraInicioRegistro.Value.TimeOfDay;
                    TimeSpan horaFin = dtpHoraFinRegistro.Value.TimeOfDay;

                    // Solo aplicar si el rango no es todo el día completo
                    bool esRangoCompleto = (horaInicio.TotalSeconds == 0 && horaFin.TotalSeconds >= 86340);

                    if (!esRangoCompleto)
                    {
                        resultadosFiltrados = resultadosFiltrados.Where(r =>
                        {
                            DateTime fecha = Convert.ToDateTime(r["FechaHora"]);
                            TimeSpan hora = fecha.TimeOfDay;

                            // Manejar rangos que cruzan medianoche
                            if (horaInicio <= horaFin)
                            {
                                return hora >= horaInicio && hora <= horaFin;
                            }
                            else
                            {
                                return hora >= horaInicio || hora <= horaFin;
                            }
                        });
                    }
                }
                else if (rbDentroHorario.Checked)
                {
                    // DENTRO de horario de oficina (L-J 8:00-18:00, V 8:00-17:00)
                    resultadosFiltrados = resultadosFiltrados.Where(r =>
                    {
                        DateTime fecha = Convert.ToDateTime(r["FechaHora"]);
                        TimeSpan hora = fecha.TimeOfDay;

                        // Fines de semana NO son horario laboral
                        if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                            return false;

                        // Viernes: dentro de 8:00-17:00
                        if (fecha.DayOfWeek == DayOfWeek.Friday)
                        {
                            TimeSpan inicioViernes = new TimeSpan(8, 0, 0);
                            TimeSpan finViernes = new TimeSpan(17, 0, 0);
                            return hora >= inicioViernes && hora <= finViernes;
                        }

                        // Lunes a Jueves: dentro de 8:00-18:00
                        TimeSpan inicioSemana = new TimeSpan(8, 0, 0);
                        TimeSpan finSemana = new TimeSpan(18, 0, 0);
                        return hora >= inicioSemana && hora <= finSemana;
                    });
                }
                else if (rbFueraHorario.Checked)
                {
                    // FUERA de horario de oficina (L-J 8:00-18:00, V 8:00-17:00)
                    resultadosFiltrados = resultadosFiltrados.Where(r =>
                    {
                        DateTime fecha = Convert.ToDateTime(r["FechaHora"]);
                        TimeSpan hora = fecha.TimeOfDay;

                        // Fines de semana siempre son fuera de horario
                        if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                            return true;

                        // Viernes: fuera de 8:00-17:00
                        if (fecha.DayOfWeek == DayOfWeek.Friday)
                        {
                            TimeSpan inicioViernes = new TimeSpan(8, 0, 0);
                            TimeSpan finViernes = new TimeSpan(17, 0, 0);
                            return hora < inicioViernes || hora > finViernes;
                        }

                        // Lunes a Jueves: fuera de 8:00-18:00
                        TimeSpan inicioSemana = new TimeSpan(8, 0, 0);
                        TimeSpan finSemana = new TimeSpan(18, 0, 0);
                        return hora < inicioSemana || hora > finSemana;
                    });
                }

                // Crear DataTable filtrado
                DataTable dtFiltrado = dtAuditoriaCompleta.Clone();
                foreach (var row in resultadosFiltrados)
                {
                    dtFiltrado.ImportRow(row.Row);
                }

                // Crear nuevo DataView con los datos filtrados
                dv = new DataView(dtFiltrado);
                dv.Sort = "FechaHora ASC";

                dgvResultadosRegistro.DataSource = dv;

                lblTotalRegistros.Text = $"Total: {dv.Count:N0} registros";

                // Agregar información extra para búsqueda genérica
                if (tipoBusqueda.Contains("Otro ID"))
                {
                    lblTotalRegistros.Text += " (búsqueda en AuditoriaID, UsuarioID y Detalle)";
                }

                // Generar timeline o estadísticas
                if (chkVistaTimeline.Checked)
                {
                    GenerarTimeline(dv, tipoBusqueda, valorBusqueda);
                }
                else
                {
                    GenerarEstadisticasRegistro(dv, tipoBusqueda, valorBusqueda);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // GENERACIÓN DE ESTADÍSTICAS
        // ============================================================

        // Pestaña Por Usuario
        private void GenerarEstadisticasUsuario(DataView datos, ComboBoxUsuario usuario)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("  📊 ESTADÍSTICAS DEL USUARIO");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Usuario: {usuario.NombreCompleto}");
                sb.AppendLine($"Username: {usuario.Username}");
                sb.AppendLine($"Total de acciones: {datos.Count:N0}");
                sb.AppendLine();

                if (datos.Count > 0)
                {
                    // Primera y última acción
                    DateTime primeraAccion = datos.Cast<DataRowView>()
                        .Min(r => Convert.ToDateTime(r["FechaHora"]));
                    DateTime ultimaAccion = datos.Cast<DataRowView>()
                        .Max(r => Convert.ToDateTime(r["FechaHora"]));

                    sb.AppendLine($"Primera acción: {primeraAccion:dd/MM/yyyy HH:mm}");
                    sb.AppendLine($"Última acción: {ultimaAccion:dd/MM/yyyy HH:mm}");
                    sb.AppendLine();

                    // Top 5 acciones más frecuentes
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("📌 ACCIONES MÁS FRECUENTES:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topAcciones = datos.Cast<DataRowView>()
                        .GroupBy(r => r["Accion"].ToString())
                        .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    int index = 1;
                    foreach (var item in topAcciones)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        string barra = new string('█', (int)(porcentaje / 5)) + new string('░', 20 - (int)(porcentaje / 5));

                        sb.AppendLine($"{index}. {item.Accion}");
                        sb.AppendLine($"   {item.Cantidad} veces ({porcentaje:F1}%)");
                        sb.AppendLine($"   {barra}");
                        sb.AppendLine();
                        index++;
                    }

                    // Top 5 módulos más utilizados
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("🗂️ MÓDULOS MÁS UTILIZADOS:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topModulos = datos.Cast<DataRowView>()
                        .Where(r => r["Modulo"] != DBNull.Value)
                        .GroupBy(r => r["Modulo"].ToString())
                        .Select(g => new { Modulo = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    index = 1;
                    foreach (var item in topModulos)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        string barra = new string('█', (int)(porcentaje / 5)) + new string('░', 20 - (int)(porcentaje / 5));

                        sb.AppendLine($"{index}. {item.Modulo}");
                        sb.AppendLine($"   {item.Cantidad} acciones ({porcentaje:F1}%)");
                        sb.AppendLine($"   {barra}");
                        sb.AppendLine();
                        index++;
                    }
                }
                                
                rtbEstadisticasUsuario.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbEstadisticasUsuario.Text = $"Error al generar estadísticas:\n\n{ex.Message}";
            }
        }

        // Pestaña Por Categoría
        private void GenerarEstadisticasCategoria(DataView datos, string categoria)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine($"  📂 ANÁLISIS: {categoria}");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Total de acciones: {datos.Count:N0}");

                // Contar módulos únicos
                var modulosUnicos = datos.Cast<DataRowView>()
                    .Where(r => r["Modulo"] != DBNull.Value)
                    .Select(r => r["Modulo"].ToString())
                    .Distinct()
                    .Count();
                sb.AppendLine($"Total de módulos: {modulosUnicos}");

                // Contar usuarios participantes
                var usuariosUnicos = datos.Cast<DataRowView>()
                    .Where(r => r["Username"] != DBNull.Value)
                    .Select(r => r["Username"].ToString())
                    .Distinct()
                    .Count();
                sb.AppendLine($"Usuarios participantes: {usuariosUnicos}");
                sb.AppendLine();

                if (datos.Count > 0)
                {
                    // Top 10 módulos más activos
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("🏆 MÓDULOS MÁS ACTIVOS:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topModulos = datos.Cast<DataRowView>()
                        .Where(r => r["Modulo"] != DBNull.Value)
                        .GroupBy(r => r["Modulo"].ToString())
                        .Select(g => new {
                            Modulo = g.Key,
                            Cantidad = g.Count(),
                            UsuariosActivos = g.Select(x => x["Username"].ToString()).Distinct().Count(),
                            UltimaActividad = g.Max(x => Convert.ToDateTime(x["FechaHora"]))
                        })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(10);

                    int index = 1;
                    foreach (var item in topModulos)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        string barra = new string('█', (int)(porcentaje / 5)) + new string('░', 20 - (int)(porcentaje / 5));

                        sb.AppendLine($"{index}. {item.Modulo}");
                        sb.AppendLine($"   {item.Cantidad} acciones ({porcentaje:F1}%)");
                        sb.AppendLine($"   {item.UsuariosActivos} usuarios activos");
                        sb.AppendLine($"   {barra}");
                        sb.AppendLine($"   Última actividad: {item.UltimaActividad:dd/MM HH:mm}");
                        sb.AppendLine();
                        index++;
                    }

                    // Top 5 usuarios más activos
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("👥 USUARIOS MÁS ACTIVOS:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topUsuarios = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .GroupBy(r => r["Username"].ToString())
                        .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    index = 1;
                    foreach (var item in topUsuarios)
                    {
                        sb.AppendLine($"{index}. {item.Usuario}: {item.Cantidad:N0} acciones");
                        index++;
                    }
                }

                rtbEstadisticasCategoria.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbEstadisticasCategoria.Text = $"Error al generar estadísticas:\n\n{ex.Message}";
            }
        }

        // Pestaña Por Módulo
        private void GenerarEstadisticasModulo(DataView datos, string modulo)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine($"  🗂️ ANÁLISIS: {modulo}");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Total de acciones: {datos.Count:N0}");

                // Contar usuarios únicos
                var usuariosUnicos = datos.Cast<DataRowView>()
                    .Where(r => r["Username"] != DBNull.Value)
                    .Select(r => r["Username"].ToString())
                    .Distinct()
                    .Count();
                sb.AppendLine($"Usuarios activos: {usuariosUnicos}");

                // Contar formularios involucrados
                var formulariosUnicos = datos.Cast<DataRowView>()
                    .Where(r => r["Formulario"] != DBNull.Value)
                    .Select(r => r["Formulario"].ToString())
                    .Distinct()
                    .Count();
                sb.AppendLine($"Formularios involucrados: {formulariosUnicos}");
                sb.AppendLine();

                if (datos.Count > 0)
                {
                    // Acciones por tipo
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("📊 ACCIONES POR TIPO:");
                    sb.AppendLine("─────────────────────────────────────");

                    var accionesPorTipo = datos.Cast<DataRowView>()
                        .Where(r => r["Accion"] != DBNull.Value)
                        .GroupBy(r => r["Accion"].ToString())
                        .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(10);

                    foreach (var item in accionesPorTipo)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        string barra = new string('█', (int)(porcentaje / 5)) + new string('░', 20 - (int)(porcentaje / 5));

                        sb.AppendLine($"{item.Accion}: {item.Cantidad} ({porcentaje:F1}%)");
                        sb.AppendLine($"  {barra}");
                        sb.AppendLine();
                    }

                    // Distribución temporal (por día de la semana)
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("📅 DISTRIBUCIÓN TEMPORAL:");
                    sb.AppendLine("─────────────────────────────────────");

                    var dias = new[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
                    var porDia = datos.Cast<DataRowView>()
                        .GroupBy(r => ((DateTime)r["FechaHora"]).DayOfWeek)
                        .Select(g => new { Dia = (int)g.Key, Cantidad = g.Count() })
                        .OrderBy(x => x.Dia)
                        .ToList();

                    foreach (var item in porDia)
                    {
                        sb.AppendLine($"{dias[item.Dia]}: {item.Cantidad:N0} acciones");
                    }
                }

                rtbEstadisticasModulo.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbEstadisticasModulo.Text = $"Error al generar estadísticas:\n\n{ex.Message}";
            }
        }

        // Pestaña Por Acción
        private void GenerarEstadisticasAccion(DataView datos, string accion)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine($"  🎯 ANÁLISIS: {accion}");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Total de ejecuciones: {datos.Count:N0}");

                if (datos.Count > 0)
                {
                    // Usuarios que ejecutan esta acción
                    var usuariosUnicos = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .Select(r => r["Username"].ToString())
                        .Distinct()
                        .Count();
                    sb.AppendLine($"Usuarios que la ejecutan: {usuariosUnicos}");
                    sb.AppendLine();

                    // Tendencia (comparar con período anterior)
                    DateTime fechaInicio = dtpDesdeAccion.Value.Date;
                    DateTime fechaFin = dtpHastaAccion.Value.Date;
                    TimeSpan periodo = fechaFin - fechaInicio;

                    DateTime fechaInicioAnterior = fechaInicio.AddDays(-periodo.TotalDays);

                    int registrosPeriodoActual = datos.Count;
                    int registrosPeriodoAnterior = datos.Cast<DataRowView>()
                        .Count(r => Convert.ToDateTime(r["FechaHora"]) >= fechaInicioAnterior &&
                                   Convert.ToDateTime(r["FechaHora"]) < fechaInicio);

                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("📈 TENDENCIA:");
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine($"Período actual: {registrosPeriodoActual} ejecuciones");
                    sb.AppendLine($"Período anterior: {registrosPeriodoAnterior} ejecuciones");

                    if (registrosPeriodoAnterior > 0)
                    {
                        double cambio = ((registrosPeriodoActual - registrosPeriodoAnterior) * 100.0) / registrosPeriodoAnterior;
                        string tendencia = cambio > 0 ? "📈 AUMENTANDO" : (cambio < 0 ? "📉 DISMINUYENDO" : "➡️ ESTABLE");
                        sb.AppendLine($"Cambio: {cambio:+0.0;-0.0}%");
                        sb.AppendLine($"Tendencia: {tendencia}");
                    }
                    sb.AppendLine();

                    // Top usuarios que ejecutan esta acción
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("👥 TOP USUARIOS:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topUsuarios = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .GroupBy(r => r["Username"].ToString())
                        .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    int index = 1;
                    foreach (var item in topUsuarios)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        sb.AppendLine($"{index}. {item.Usuario}: {item.Cantidad} veces ({porcentaje:F1}%)");
                        index++;
                    }
                    sb.AppendLine();

                    // Módulos donde se ejecuta
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("🗂️ MÓDULOS:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topModulos = datos.Cast<DataRowView>()
                        .Where(r => r["Modulo"] != DBNull.Value)
                        .GroupBy(r => r["Modulo"].ToString())
                        .Select(g => new { Modulo = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    index = 1;
                    foreach (var item in topModulos)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        sb.AppendLine($"{index}. {item.Modulo}: {item.Cantidad} veces ({porcentaje:F1}%)");
                        index++;
                    }
                }

                rtbEstadisticasAccion.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbEstadisticasAccion.Text = $"Error al generar estadísticas:\n\n{ex.Message}";
            }
        }

        // Pestaña Por Horario
        private void GenerarEstadisticasHorario(DataView datos, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("  🕐 ANÁLISIS POR HORARIO");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Fecha: {fecha:dd/MM/yyyy}");
                sb.AppendLine($"Horario: {horaInicio:hh\\:mm} - {horaFin:hh\\:mm}");
                sb.AppendLine($"Total de acciones: {datos.Count:N0}");
                sb.AppendLine();

                if (datos.Count > 0)
                {
                    // Usuarios activos en este horario
                    var usuariosUnicos = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .Select(r => r["Username"].ToString())
                        .Distinct()
                        .Count();
                    sb.AppendLine($"Usuarios activos: {usuariosUnicos}");
                    sb.AppendLine();

                    // Distribución por hora
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("⏰ DISTRIBUCIÓN POR HORA:");
                    sb.AppendLine("─────────────────────────────────────");

                    var porHora = datos.Cast<DataRowView>()
                        .GroupBy(r => Convert.ToDateTime(r["FechaHora"]).Hour)
                        .Select(g => new { Hora = g.Key, Cantidad = g.Count() })
                        .OrderBy(x => x.Hora);

                    foreach (var item in porHora)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        string barra = new string('█', (int)(porcentaje / 5)) + new string('░', 20 - (int)(porcentaje / 5));
                        sb.AppendLine($"{item.Hora:00}:00 - {item.Cantidad} acciones");
                        sb.AppendLine($"  {barra}");
                    }
                    sb.AppendLine();

                    // Top acciones en este horario
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("🎯 ACCIONES MÁS FRECUENTES:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topAcciones = datos.Cast<DataRowView>()
                        .Where(r => r["Accion"] != DBNull.Value)
                        .GroupBy(r => r["Accion"].ToString())
                        .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    int index = 1;
                    foreach (var item in topAcciones)
                    {
                        sb.AppendLine($"{index}. {item.Accion}: {item.Cantidad} veces");
                        index++;
                    }
                    sb.AppendLine();

                    // Top usuarios
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("👥 USUARIOS MÁS ACTIVOS:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topUsuarios = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .GroupBy(r => r["Username"].ToString())
                        .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    index = 1;
                    foreach (var item in topUsuarios)
                    {
                        sb.AppendLine($"{index}. {item.Usuario}: {item.Cantidad} acciones");
                        index++;
                    }
                }

                rtbEstadisticasHorario.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbEstadisticasHorario.Text = $"Error al generar estadísticas:\n\n{ex.Message}";
            }
        }

        // Pestaña Por IP/Máquina
        private void GenerarEstadisticasIP(DataView datos, string ip, string maquina)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("  🌐 ANÁLISIS POR IP/MÁQUINA");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();

                if (!string.IsNullOrWhiteSpace(ip))
                    sb.AppendLine($"IP: {ip}");
                if (!string.IsNullOrWhiteSpace(maquina))
                    sb.AppendLine($"Máquina: {maquina}");

                sb.AppendLine($"Total de acciones: {datos.Count:N0}");
                sb.AppendLine();

                if (datos.Count > 0)
                {
                    // IPs únicas
                    var ipsUnicas = datos.Cast<DataRowView>()
                        .Where(r => r["DireccionIP"] != DBNull.Value)
                        .Select(r => r["DireccionIP"].ToString())
                        .Distinct()
                        .Count();
                    sb.AppendLine($"Direcciones IP únicas: {ipsUnicas}");

                    // Máquinas únicas
                    var maquinasUnicas = datos.Cast<DataRowView>()
                        .Where(r => r["NombreMaquina"] != DBNull.Value)
                        .Select(r => r["NombreMaquina"].ToString())
                        .Distinct()
                        .Count();
                    sb.AppendLine($"Máquinas únicas: {maquinasUnicas}");

                    // Usuarios desde esta IP/máquina
                    var usuariosUnicos = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .Select(r => r["Username"].ToString())
                        .Distinct()
                        .Count();
                    sb.AppendLine($"Usuarios diferentes: {usuariosUnicos}");
                    sb.AppendLine();

                    // Top máquinas (si se buscó por IP)
                    if (!string.IsNullOrWhiteSpace(ip) && string.IsNullOrWhiteSpace(maquina))
                    {
                        sb.AppendLine("─────────────────────────────────────");
                        sb.AppendLine("🖥️ MÁQUINAS CON ESTA IP:");
                        sb.AppendLine("─────────────────────────────────────");

                        var topMaquinas = datos.Cast<DataRowView>()
                            .Where(r => r["NombreMaquina"] != DBNull.Value)
                            .GroupBy(r => r["NombreMaquina"].ToString())
                            .Select(g => new { Maquina = g.Key, Cantidad = g.Count() })
                            .OrderByDescending(x => x.Cantidad)
                            .Take(5);

                        foreach (var item in topMaquinas)
                        {
                            sb.AppendLine($"• {item.Maquina}: {item.Cantidad} acciones");
                        }
                        sb.AppendLine();
                    }

                    // Top IPs (si se buscó por máquina)
                    if (string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(maquina))
                    {
                        sb.AppendLine("─────────────────────────────────────");
                        sb.AppendLine("📡 IPs DE ESTA MÁQUINA:");
                        sb.AppendLine("─────────────────────────────────────");

                        var topIPs = datos.Cast<DataRowView>()
                            .Where(r => r["DireccionIP"] != DBNull.Value)
                            .GroupBy(r => r["DireccionIP"].ToString())
                            .Select(g => new { IP = g.Key, Cantidad = g.Count() })
                            .OrderByDescending(x => x.Cantidad)
                            .Take(5);

                        foreach (var item in topIPs)
                        {
                            sb.AppendLine($"• {item.IP}: {item.Cantidad} acciones");
                        }
                        sb.AppendLine();
                    }

                    // Top usuarios
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("👥 USUARIOS MÁS ACTIVOS:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topUsuarios = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .GroupBy(r => r["Username"].ToString())
                        .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    int index = 1;
                    foreach (var item in topUsuarios)
                    {
                        sb.AppendLine($"{index}. {item.Usuario}: {item.Cantidad} acciones");
                        index++;
                    }
                    sb.AppendLine();

                    // Top acciones
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("🎯 ACCIONES MÁS FRECUENTES:");
                    sb.AppendLine("─────────────────────────────────────");

                    var topAcciones = datos.Cast<DataRowView>()
                        .Where(r => r["Accion"] != DBNull.Value)
                        .GroupBy(r => r["Accion"].ToString())
                        .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(5);

                    index = 1;
                    foreach (var item in topAcciones)
                    {
                        sb.AppendLine($"{index}. {item.Accion}: {item.Cantidad} veces");
                        index++;
                    }
                }

                rtbEstadisticasIP.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbEstadisticasIP.Text = $"Error al generar estadísticas:\n\n{ex.Message}";
            }
        }

        // ============================================================
        // GENERACIÓN DE TIMELINE DETALLADO
        // ============================================================

        // Pestaña Por Registro
        private void GenerarTimeline(DataView datos, string tipoBusqueda, string valorBusqueda)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("  📋 HISTORIAL DEL REGISTRO");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Tipo: {tipoBusqueda}");
                sb.AppendLine($"Valor: {valorBusqueda}");

                if (datos.Count > 0)
                {
                    var primerRegistro = datos.Cast<DataRowView>().First();
                    var ultimoRegistro = datos.Cast<DataRowView>().Last();

                    string modulo = primerRegistro["Modulo"] != DBNull.Value ?
                        primerRegistro["Modulo"].ToString() : "N/A";

                    sb.AppendLine($"Módulo principal: {modulo}");
                    sb.AppendLine($"Total de cambios: {datos.Count}");

                    DateTime primeraFecha = Convert.ToDateTime(primerRegistro["FechaHora"]);
                    DateTime ultimaFecha = Convert.ToDateTime(ultimoRegistro["FechaHora"]);
                    TimeSpan tiempoVida = ultimaFecha - primeraFecha;

                    sb.AppendLine($"Tiempo de vida: {tiempoVida.Days} días");
                    sb.AppendLine();
                    sb.AppendLine($"Primer evento: {primeraFecha:dd/MM/yyyy HH:mm:ss}");
                    sb.AppendLine($"Por: {primerRegistro["Username"]}");
                    sb.AppendLine($"Acción: {primerRegistro["Accion"]}");
                    sb.AppendLine();
                    sb.AppendLine($"Último evento: {ultimaFecha:dd/MM/yyyy HH:mm:ss}");
                    sb.AppendLine($"Por: {ultimoRegistro["Username"]}");
                    sb.AppendLine($"Acción: {ultimoRegistro["Accion"]}");
                    sb.AppendLine();

                    sb.AppendLine("═══════════════════════════════════════");
                    sb.AppendLine("  📅 LÍNEA DE TIEMPO CRONOLÓGICA");
                    sb.AppendLine("═══════════════════════════════════════");
                    sb.AppendLine();

                    DateTime? fechaAnterior = null;
                    int contador = 0;

                    foreach (DataRowView row in datos)
                    {
                        contador++;
                        DateTime fecha = Convert.ToDateTime(row["FechaHora"]);
                        string accion = row["Accion"].ToString();
                        string usuario = row["Username"].ToString();
                        string ip = row["DireccionIP"] != DBNull.Value ? row["DireccionIP"].ToString() : "N/A";
                        string maquina = row["NombreMaquina"] != DBNull.Value ? row["NombreMaquina"].ToString() : "N/A";
                        string detalle = row["Detalle"] != DBNull.Value ? row["Detalle"].ToString() : "";
                        string categoria = row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() : "";
                        string moduloEvento = row["Modulo"] != DBNull.Value ? row["Modulo"].ToString() : "";

                        // Mostrar tiempo transcurrido
                        if (fechaAnterior.HasValue)
                        {
                            TimeSpan transcurrido = fecha - fechaAnterior.Value;

                            if (transcurrido.TotalMinutes < 1)
                            {
                                sb.AppendLine("   ▼ (segundos después)");
                            }
                            else if (transcurrido.TotalHours < 1)
                            {
                                sb.AppendLine($"   ▼ ({transcurrido.Minutes}m después)");
                            }
                            else if (transcurrido.TotalDays < 1)
                            {
                                sb.AppendLine($"   ▼ ({transcurrido.Hours}h {transcurrido.Minutes}m después)");
                            }
                            else
                            {
                                sb.AppendLine($"   ▼ ({transcurrido.Days}d {transcurrido.Hours}h después)");
                            }
                            sb.AppendLine();
                        }

                        // Icono según tipo de acción
                        string icono = "📝";
                        if (accion.Contains("CREAR")) icono = "🆕";
                        else if (accion.Contains("MODIFICAR") || accion.Contains("ACTUALIZAR") || accion.Contains("EDITAR")) icono = "✏️";
                        else if (accion.Contains("ELIMINAR") || accion.Contains("BORRAR")) icono = "🗑️";
                        else if (accion.Contains("ACTIVAR")) icono = "✅";
                        else if (accion.Contains("DESACTIVAR")) icono = "❌";
                        else if (accion.Contains("CONSULTAR") || accion.Contains("VER")) icono = "👁️";
                        else if (accion.Contains("EXPORTAR") || accion.Contains("IMPRIMIR")) icono = "📄";

                        sb.AppendLine($"{icono} EVENTO #{contador} - {fecha:dd/MM/yyyy HH:mm:ss}");
                        sb.AppendLine($"   ┃");
                        sb.AppendLine($"   ┃ 🎯 ACCIÓN: {accion}");
                        sb.AppendLine($"   ┃ 👤 Usuario: {usuario}");
                        sb.AppendLine($"   ┃ 📂 Categoría: {categoria}");
                        sb.AppendLine($"   ┃ 🗂️  Módulo: {moduloEvento}");
                        sb.AppendLine($"   ┃ 🌐 IP: {ip}");
                        sb.AppendLine($"   ┃ 🖥️  Máquina: {maquina}");

                        if (!string.IsNullOrWhiteSpace(detalle))
                        {
                            sb.AppendLine($"   ┃");
                            sb.AppendLine($"   ┃ 📋 DETALLE:");

                            // Mostrar detalle en líneas cortas
                            var lineasDetalle = detalle.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var linea in lineasDetalle.Take(5)) // Máximo 5 líneas
                            {
                                string lineaLimpia = linea.Trim();
                                if (lineaLimpia.Length > 45)
                                {
                                    sb.AppendLine($"   ┃    • {lineaLimpia.Substring(0, 42)}...");
                                }
                                else if (!string.IsNullOrWhiteSpace(lineaLimpia))
                                {
                                    sb.AppendLine($"   ┃    • {lineaLimpia}");
                                }
                            }

                            if (lineasDetalle.Length > 5)
                            {
                                sb.AppendLine($"   ┃    ... (+{lineasDetalle.Length - 5} líneas más)");
                            }
                        }

                        sb.AppendLine("   ┃");

                        fechaAnterior = fecha;
                    }

                    sb.AppendLine("   ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    sb.AppendLine();

                    // Resumen
                    sb.AppendLine("═══════════════════════════════════════");
                    sb.AppendLine("  📊 RESUMEN DEL REGISTRO");
                    sb.AppendLine("═══════════════════════════════════════");
                    sb.AppendLine();

                    // Cambios por usuario
                    var cambiosPorUsuario = datos.Cast<DataRowView>()
                        .GroupBy(r => r["Username"].ToString())
                        .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad);

                    sb.AppendLine("👥 CAMBIOS POR USUARIO:");
                    foreach (var item in cambiosPorUsuario)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        sb.AppendLine($"  • {item.Usuario,-20}: {item.Cantidad} eventos ({porcentaje:F0}%)");
                    }
                    sb.AppendLine();

                    // Cambios por tipo de acción
                    var cambiosPorAccion = datos.Cast<DataRowView>()
                        .GroupBy(r => r["Accion"].ToString())
                        .Select(g => new { Accion = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad);

                    sb.AppendLine("🎯 TIPOS DE EVENTOS:");
                    foreach (var item in cambiosPorAccion)
                    {
                        sb.AppendLine($"  • {item.Accion}: {item.Cantidad}");
                    }
                    sb.AppendLine();

                    // Estado actual (basado en la última acción)
                    string ultimaAccion = ultimoRegistro["Accion"].ToString();
                    string estadoFinalTexto = "ACTIVO";
                    Color estadoFinalColor = Color.Blue;

                    if (ultimaAccion.Contains("ELIMINAR") || ultimaAccion.Contains("BORRAR"))
                    {
                        estadoFinalTexto = "ELIMINADO";
                        estadoFinalColor = Color.Red;
                    }
                    else if (ultimaAccion.Contains("DESACTIVAR"))
                    {
                        estadoFinalTexto = "INACTIVO";
                        estadoFinalColor = Color.DarkGray;
                    }
                    else if (ultimaAccion.Contains("ACTIVAR"))
                    {
                        estadoFinalTexto = "ACTIVO";
                        estadoFinalColor = Color.Green;
                    }

                    sb.AppendLine($"📌 ESTADO FINAL: {estadoFinalTexto}");
                    sb.AppendLine($"   Última acción: {ultimaAccion}");
                    sb.AppendLine($"   Fecha: {ultimaFecha:dd/MM/yyyy HH:mm:ss}");

                    // Si es búsqueda por UsuarioID, mostrar estado actual del usuario
                    if (tipoBusqueda.Contains("UsuarioID"))
                    {
                        sb.AppendLine();
                        sb.AppendLine($"💡 ESTADO ACTUAL EN BASE DE DATOS:");

                        var estadoUsuario = ObtenerEstadoActualUsuarioConColor(valorBusqueda);
                        sb.AppendLine($"   Estado: {estadoUsuario.Item1}");
                        sb.AppendLine($"   {estadoUsuario.Item2}");

                        // Aplicar color al RichTextBox
                        int inicioTexto = rtbTimeline.Text.Length;
                        rtbTimeline.Text = sb.ToString();

                        // Colorear "ESTADO FINAL"
                        int indiceEstadoFinal = rtbTimeline.Text.IndexOf($"ESTADO FINAL: {estadoFinalTexto}");
                        if (indiceEstadoFinal >= 0)
                        {
                            int inicioColor = indiceEstadoFinal + "ESTADO FINAL: ".Length;
                            rtbTimeline.Select(inicioColor, estadoFinalTexto.Length);
                            rtbTimeline.SelectionColor = estadoFinalColor;
                            rtbTimeline.SelectionFont = new System.Drawing.Font(rtbTimeline.Font, FontStyle.Bold);
                        }

                        // Colorear "ESTADO ACTUAL"
                        if (tipoBusqueda.Contains("UsuarioID"))
                        {
                            int indiceEstadoActual = rtbTimeline.Text.IndexOf("Estado: ");
                            if (indiceEstadoActual >= 0)
                            {
                                int inicioEstado = indiceEstadoActual + "Estado: ".Length;
                                rtbTimeline.Select(inicioEstado, estadoUsuario.Item1.Length);
                                rtbTimeline.SelectionColor = estadoUsuario.Item3;
                                rtbTimeline.SelectionFont = new System.Drawing.Font(rtbTimeline.Font, FontStyle.Bold);
                            }
                        }

                        rtbTimeline.Select(0, 0); // Deseleccionar
                        return;
                    }

                    rtbTimeline.Text = sb.ToString();

                    // Aplicar color solo al estado final
                    int indiceEstado = rtbTimeline.Text.IndexOf($"ESTADO FINAL: {estadoFinalTexto}");
                    if (indiceEstado >= 0)
                    {
                        int inicioColor = indiceEstado + "ESTADO FINAL: ".Length;
                        rtbTimeline.Select(inicioColor, estadoFinalTexto.Length);
                        rtbTimeline.SelectionColor = estadoFinalColor;
                        rtbTimeline.SelectionFont = new System.Drawing.Font(rtbTimeline.Font, FontStyle.Bold);
                    }

                    rtbTimeline.Select(0, 0); // Deseleccionar
                }
                else
                {
                    sb.AppendLine("No se encontraron registros para esta búsqueda.");
                    sb.AppendLine();
                    sb.AppendLine("💡 SUGERENCIAS:");
                    sb.AppendLine("• Verifique que el ID sea correcto");
                    sb.AppendLine("• Intente ampliar el rango de fechas");
                    sb.AppendLine("• Pruebe con un módulo diferente");
                    sb.AppendLine("• Use 'Otro ID' para búsqueda genérica");
                }

                rtbTimeline.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbTimeline.Text = $"Error al generar timeline:\n\n{ex.Message}";
            }
        }

        // Obtener estado actual del usuario desde la base de datos (para búsqueda por UsuarioID)
        private Tuple<string, string, Color> ObtenerEstadoActualUsuarioConColor(string usuarioID)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                SELECT 
                    Username,
                    NombreCompleto,
                    Activo,
                    EsEliminado,
                    UltimoAcceso
                FROM Usuarios WITH (NOLOCK)
                WHERE UsuarioID = @UsuarioID";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool activo = reader.GetBoolean(2);
                                bool eliminado = reader.GetBoolean(3);
                                string username = reader.GetString(0);
                                string nombreCompleto = reader.GetString(1);
                                DateTime? ultimoAcceso = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);

                                string estadoTexto;
                                Color estadoColor;

                                if (eliminado)
                                {
                                    estadoTexto = "ELIMINADO";
                                    estadoColor = Color.Red;
                                }
                                else if (!activo)
                                {
                                    estadoTexto = "INACTIVO";
                                    estadoColor = Color.DarkGray;
                                }
                                else
                                {
                                    estadoTexto = "ACTIVO";
                                    estadoColor = Color.Green;
                                }

                                var sb = new StringBuilder();
                                sb.AppendLine($"Usuario: {username}");
                                sb.AppendLine($"   Nombre: {nombreCompleto}");

                                if (ultimoAcceso.HasValue)
                                {
                                    sb.AppendLine($"   Último acceso: {ultimoAcceso.Value:dd/MM/yyyy HH:mm:ss}");
                                }
                                else
                                {
                                    sb.AppendLine($"   Último acceso: Nunca");
                                }

                                return Tuple.Create(estadoTexto, sb.ToString(), estadoColor);
                            }
                            else
                            {
                                return Tuple.Create("NO ENCONTRADO", "❌ Usuario no encontrado en la BD", Color.Red);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create("ERROR", $"❌ Error al consultar estado: {ex.Message}", Color.Red);
            }
        }

        // Pestaña Por Registro - Estadísticas resumidas 
        private void GenerarEstadisticasRegistro(DataView datos, string tipoBusqueda, string valorBusqueda)
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine("  📊 ESTADÍSTICAS DEL REGISTRO");
                sb.AppendLine("═══════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Tipo: {tipoBusqueda}");
                sb.AppendLine($"Valor: {valorBusqueda}");
                sb.AppendLine($"Total de eventos: {datos.Count:N0}");
                sb.AppendLine();

                if (datos.Count > 0)
                {
                    // Usuarios que interactuaron
                    var usuariosUnicos = datos.Cast<DataRowView>()
                        .Where(r => r["Username"] != DBNull.Value)
                        .Select(r => r["Username"].ToString())
                        .Distinct()
                        .Count();
                    sb.AppendLine($"Usuarios involucrados: {usuariosUnicos}");

                    // Módulos involucrados
                    var modulosUnicos = datos.Cast<DataRowView>()
                        .Where(r => r["Modulo"] != DBNull.Value)
                        .Select(r => r["Modulo"].ToString())
                        .Distinct()
                        .Count();
                    sb.AppendLine($"Módulos involucrados: {modulosUnicos}");
                    sb.AppendLine();

                    // Eventos por tipo de acción
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("🎯 EVENTOS POR TIPO:");
                    sb.AppendLine("─────────────────────────────────────");

                    var porTipo = datos.Cast<DataRowView>()
                        .GroupBy(r => r["Accion"].ToString())
                        .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad);

                    foreach (var item in porTipo)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        sb.AppendLine($"  {item.Tipo}");
                        sb.AppendLine($"     {item.Cantidad} eventos ({porcentaje:F1}%)");
                    }
                    sb.AppendLine();

                    // Eventos por usuario
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("👥 EVENTOS POR USUARIO:");
                    sb.AppendLine("─────────────────────────────────────");

                    var porUsuario = datos.Cast<DataRowView>()
                        .GroupBy(r => r["Username"].ToString())
                        .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad);

                    foreach (var item in porUsuario)
                    {
                        double porcentaje = (item.Cantidad * 100.0) / datos.Count;
                        sb.AppendLine($"  {item.Usuario,-20}: {item.Cantidad} ({porcentaje:F0}%)");
                    }
                    sb.AppendLine();

                    // Distribución temporal
                    sb.AppendLine("─────────────────────────────────────");
                    sb.AppendLine("📅 DISTRIBUCIÓN TEMPORAL:");
                    sb.AppendLine("─────────────────────────────────────");

                    var porDia = datos.Cast<DataRowView>()
                        .GroupBy(r => Convert.ToDateTime(r["FechaHora"]).Date)
                        .Select(g => new { Fecha = g.Key, Cantidad = g.Count() })
                        .OrderBy(x => x.Fecha)
                        .Take(10); // Últimos 10 días con actividad

                    foreach (var item in porDia)
                    {
                        sb.AppendLine($"  {item.Fecha:dd/MM/yyyy}: {item.Cantidad} eventos");
                    }

                    if (datos.Count > 0)
                    {
                        var primerEvento = datos.Cast<DataRowView>().First();
                        var ultimoEvento = datos.Cast<DataRowView>().Last();

                        sb.AppendLine();
                        sb.AppendLine("─────────────────────────────────────");
                        sb.AppendLine("⏱️  LÍNEA DE TIEMPO:");
                        sb.AppendLine("─────────────────────────────────────");
                        sb.AppendLine($"  Primer evento: {Convert.ToDateTime(primerEvento["FechaHora"]):dd/MM/yyyy HH:mm}");
                        sb.AppendLine($"  Último evento: {Convert.ToDateTime(ultimoEvento["FechaHora"]):dd/MM/yyyy HH:mm}");

                        TimeSpan duracion = Convert.ToDateTime(ultimoEvento["FechaHora"]) -
                                           Convert.ToDateTime(primerEvento["FechaHora"]);
                        sb.AppendLine($"  Duración: {duracion.Days} días, {duracion.Hours} horas");
                    }
                }
                else
                {
                    sb.AppendLine("No se encontraron registros para esta búsqueda.");
                }

                rtbTimeline.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                rtbTimeline.Text = $"Error al generar estadísticas:\n\n{ex.Message}";
            }
        }


        /// Evento de cambio de categoría en pestaña Por Módulo (cascada)///
        private void CmbCategoriaModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Filtrar módulos según categoría seleccionada
                cmbModuloFiltro.SelectedIndexChanged -= (s, ev) => IniciarBusquedaConDelay(); // Desactivar evento temporalmente

                cmbModuloFiltro.Items.Clear();
                cmbModuloFiltro.Items.Add("Seleccione un módulo...");

                if (cmbCategoriaModulo.SelectedIndex > 0 && dtAuditoriaCompleta != null)
                {
                    // Filtrar módulos por categoría
                    string categoriaSeleccionada = cmbCategoriaModulo.SelectedItem.ToString();

                    var modulosFiltrados = dtAuditoriaCompleta.AsEnumerable()
                        .Where(r => r["Categoria"] != DBNull.Value &&
                                   r["Categoria"].ToString() == categoriaSeleccionada &&
                                   r["Modulo"] != DBNull.Value)
                        .Select(r => r["Modulo"].ToString())
                        .Distinct()
                        .OrderBy(m => m)
                        .ToList();

                    foreach (var mod in modulosFiltrados)
                    {
                        cmbModuloFiltro.Items.Add(mod);
                    }
                }
                else
                {
                    // Mostrar todos los módulos
                    foreach (var mod in cacheModulos)
                    {
                        cmbModuloFiltro.Items.Add(mod);
                    }
                }

                cmbModuloFiltro.SelectedIndex = 0;
                cmbModuloFiltro.SelectedIndexChanged += (s, ev) => IniciarBusquedaConDelay(); // Reactivar evento
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar módulos:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón Volver
        private void btnVolver_Click(object sender, EventArgs e)
        {
            FormAuditoria formAuditoria = new FormAuditoria(formPrincipal);
            formPrincipal.CargarContenidoPanel(formAuditoria);
        }

        // Botón Cargar más datos históricos
        private async void BtnCargarMasDatos_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Desea cargar TODOS los datos históricos?\n\n" +
                "Esto puede tardar varios segundos dependiendo de la cantidad de registros.\n\n" +
                "NOTA: Una vez cargados, permanecerán en memoria durante esta sesión.",
                "Cargar Datos Históricos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    btnCargarMasDatos.Enabled = false;
                    btnCargarMasDatos.Text = "⏳ Cargando...";
                    lblTotalRegistros.Text = "⏳ Cargando todos los datos históricos...";

                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        using (var conn = DatabaseConnection.GetConnection())
                        {
                            conn.Open();

                            // Cargar TODOS los datos
                            DataTable dtTodosDatos = new DataTable();
                            string sqlTodosDatos = @"
                        SELECT 
                            A.AuditoriaID,
                            A.FechaHora,
                            A.UsuarioID,
                            U.Username,
                            A.Categoria,
                            A.Modulo,
                            A.Formulario,
                            A.Accion,
                            A.Detalle,
                            A.DireccionIP,
                            A.NombreMaquina
                        FROM Auditoria A WITH (NOLOCK)
                        LEFT JOIN Usuarios U WITH (NOLOCK) ON A.UsuarioID = U.UsuarioID
                        ORDER BY A.FechaHora DESC";

                            using (var cmd = new SqlCommand(sqlTodosDatos, conn))
                            {
                                cmd.CommandTimeout = 120; // 2 minutos
                                using (var adapter = new SqlDataAdapter(cmd))
                                {
                                    adapter.Fill(dtTodosDatos);
                                }
                            }

                            this.Invoke((MethodInvoker)delegate
                            {
                                dtAuditoriaCompleta = dtTodosDatos;

                                // Recargar tipos de búsqueda con todos los datos
                                using (var connInvoke = DatabaseConnection.GetConnection())
                                {
                                    connInvoke.Open();
                                    CargarTiposBusquedaDinamicos(connInvoke);
                                }

                                btnCargarMasDatos.Visible = false;
                                lblTotalRegistros.Text = $"Total: {dtAuditoriaCompleta.Rows.Count:N0} registros (historial completo)";
                                lblFechaConsulta.Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                                MessageBox.Show(
                                    $"✅ Se han cargado {dtAuditoriaCompleta.Rows.Count:N0} registros históricos.\n\n" +
                                    "Los datos permanecerán en memoria durante esta sesión.",
                                    "Carga Completada",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            });
                        }
                    });

                    this.Cursor = Cursors.Default;
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Default;
                    btnCargarMasDatos.Enabled = true;
                    btnCargarMasDatos.Text = "⏳ Cargar más datos";

                    MessageBox.Show($"Error al cargar datos históricos:\n\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Clase auxiliar para ComboBox de usuarios
        private class ComboBoxUsuario
        {
            public int UsuarioID { get; set; }
            public string Username { get; set; }
            public string NombreCompleto { get; set; }
            public string Display => $"{NombreCompleto} ({Username})";
        }

        // Obtener el DataGridView activo según la pestaña seleccionada
        private DataGridView ObtenerDataGridViewActivo()
        {
            if (tabControlModos.SelectedTab == tabPorUsuario) return dgvResultadosUsuario;
            if (tabControlModos.SelectedTab == tabPorCategoria) return dgvResultadosCategoria;
            if (tabControlModos.SelectedTab == tabPorModulo) return dgvResultadosModulo;
            if (tabControlModos.SelectedTab == tabPorAccion) return dgvResultadosAccion;
            if (tabControlModos.SelectedTab == tabPorHorario) return dgvResultadosHorario;
            if (tabControlModos.SelectedTab == tabPorIP) return dgvResultadosIP;
            if (tabControlModos.SelectedTab == tabPorRegistro) return dgvResultadosRegistro;
            return null;
        }
        // Obtener el nombre de la pestaña activa sin emojis
        private string ObtenerNombrePestanaActiva()
        {
            return tabControlModos.SelectedTab.Text
                .Replace("👤 ", "").Replace("📂 ", "").Replace("🗂️ ", "")
                .Replace("🎯 ", "").Replace("🕐 ", "").Replace("🌐 ", "")
                .Replace("📋 ", "").Trim();
        }
    }
}