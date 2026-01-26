using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    public partial class FormAdministrarPermisos : Form
    {
        // Referencia al formulario principal
        private FormMain formPrincipal;

        // ID del rol seleccionado (si aplica)
        private int rolSeleccionadoID = 0;

        // ID del usuario seleccionado (si aplica)
        private int usuarioSeleccionadoID = 0;

        // DataTable para almacenar permisos cargados
        private DataTable dtPermisos;

        // Indicador de carga de datos
        private bool cargandoDatos = false;

        // Placeholder text
        private const string PLACEHOLDER_BUSCAR = "Buscar formulario...";

        // Indicador de placeholder activo
        private bool isPlaceholder = true;

        // Evitar loops al sincronizar selección de roles
        private bool sincronizandoRoles = false;

        // Estado de cambios sin guardar
        private bool hayCambiosSinGuardar = false;

        // Evitar recargas múltiples de permisos
        private bool cargandoPermisos = false;

        // Variables para vista previa de cambios
        private DataTable dtPermisosOriginal = null;

        public FormAdministrarPermisos(FormMain formMain)
        {
            InitializeComponent();
            formPrincipal = formMain;
            ConfigurarFormulario();
            ConfigurarPlaceholder();
            ConfigurarEventos();

            // Configurar grid vacío al inicio (solo estructura)
            ConfigurarDataGridView();

            CargarRoles();
            CargarComboBoxes();

            // AGREGAR ESTA LÍNEA:
            CargarRolesPersonalizados();
        }

        private void InicializarResumenVacio()
        {
            if (lblTotalPermisos != null)
                lblTotalPermisos.Text = "Total: 0 permisos (0 formularios)";

            if (lblPermitidos != null)
                lblPermitidos.Text = "✓ Permitidos: 0 (0.0%)";

            if (lblDenegados != null)
                lblDenegados.Text = "✗ Denegados: 0";

            var gbResumen = splMain.Panel1.Controls.OfType<GroupBox>()
                .FirstOrDefault(gb => gb.Name == "gbResumen");

            if (gbResumen != null)
            {
                var lblAccesoCompleto = gbResumen.Controls["lblAccesoCompleto"] as Label;
                var lblSinAcceso = gbResumen.Controls["lblSinAcceso"] as Label;

                if (lblAccesoCompleto != null)
                    lblAccesoCompleto.Text = "🔓 Acceso completo: 0";

                if (lblSinAcceso != null)
                    lblSinAcceso.Text = "🔒 Sin acceso: 0";
            }
        }

        // Colores específicos para btnGuardar
        private Color colorGuardarNormal = Color.FromArgb(34, 139, 34);
        private Color colorGuardarCambios = Color.FromArgb(255, 152, 0);
        private Color colorGuardarNormalHover = Color.FromArgb(40, 167, 69);
        private Color colorGuardarCambiosHover = Color.FromArgb(255, 120, 0);

        private void ConfigurarEstiloBotonGuardar()
        {
            btnGuardar.BackColor = colorGuardarNormal;
            btnGuardar.ForeColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Cursor = Cursors.Hand;

            btnGuardar.MouseEnter += (s, e) =>
            {
                if (hayCambiosSinGuardar)
                    btnGuardar.BackColor = colorGuardarCambiosHover;
                else
                    btnGuardar.BackColor = colorGuardarNormalHover;
            };

            btnGuardar.MouseLeave += (s, e) =>
            {
                if (hayCambiosSinGuardar)
                    btnGuardar.BackColor = colorGuardarCambios;
                else
                    btnGuardar.BackColor = colorGuardarNormal;
            };
        }

        private void ConfigurarFormulario()
        {
            // Hacer que el SplitContainer ocupe toda la pantalla
            splMain.Dock = DockStyle.Fill;

            // Configurar estilos de botones principales
            ConfigurarEstiloBoton(btnVolver, Color.White, Color.FromArgb(0, 120, 212), true);

            // Configurar btnGuardar con manejo especial de colores
            ConfigurarEstiloBotonGuardar();

            ConfigurarEstiloBoton(btnRecargar, Color.FromArgb(108, 117, 125), Color.White, false);
            ConfigurarEstiloBoton(btnLimpiarFiltros, Color.FromArgb(108, 117, 125), Color.White, false);

            // TODOS los botones de acciones masivas en azul uniforme
            ConfigurarEstiloBoton(btnAccesoCompleto, Color.FromArgb(0, 120, 212), Color.White, false);
            ConfigurarEstiloBoton(btnDenegarCategoria, Color.FromArgb(0, 120, 212), Color.White, false);
            ConfigurarEstiloBoton(btnSoloConsulta, Color.FromArgb(0, 120, 212), Color.White, false);
            ConfigurarEstiloBoton(btnConsultaAvanzada, Color.FromArgb(0, 120, 212), Color.White, false);
            ConfigurarEstiloBoton(btnAccesoTotal, Color.FromArgb(0, 120, 212), Color.White, false);
            ConfigurarEstiloBoton(btnGestionCompleta, Color.FromArgb(0, 120, 212), Color.White, false);
            ConfigurarEstiloBoton(btnSoloLectura, Color.FromArgb(0, 120, 212), Color.White, false);
            ConfigurarEstiloBoton(btnSinAcceso, Color.FromArgb(0, 120, 212), Color.White, false);

            // Configurar anchors para que todo ocupe el espacio
            tabPermisos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvPermisos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbAccionesMasivas.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Ocultar ComboBox de usuarios por defecto
            cmbUsuarios.Visible = false;

            // Indicador de modo (Usuario/Rol) - Mejorado
            Label lblIndicadorModo = new Label
            {
                Name = "lblIndicadorModo",
                Text = "Modo: Esperando selección...",
                Location = new Point(1250, 0),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            splMain.Panel2.Controls.Add(lblIndicadorModo);
            lblIndicadorModo.BringToFront();

            // Configurar estilo de botones de roles dinámicos
            ConfigurarEstiloBotonesRoles();

            // Al final de ConfigurarFormulario(), agregar:
            ConfigurarBotonesToggle();
        }

        private void ConfigurarBotonesToggle()
        {
            // Configurar estilo base SIN eventos hover (se manejan en ActualizarBotonToggle)
            var botonesToggle = new[] { btnToggleVer, btnToggleEditar, btnToggleImprimir, btnToggleExportar };

            foreach (var btn in botonesToggle)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                btn.ForeColor = Color.White;
                btn.BackColor = Color.FromArgb(0, 120, 212); // Color inicial
            }
        }

        private void ActualizarIndicadorModo()
        {
            var lblIndicador = splMain.Panel2.Controls["lblIndicadorModo"] as Label;
            if (lblIndicador == null) return;

            if (usuarioSeleccionadoID > 0)
            {
                var usuario = cmbUsuarios.SelectedItem as ComboBoxUsuario;
                lblIndicador.Text = $"Modo: Usuario ({usuario.Username})";
                lblIndicador.ForeColor = Color.FromArgb(255, 152, 0); // Naranja
            }
            else if (rolSeleccionadoID > 0)
            {
                string nombreRol = "";
                string tipoRol = "";

                // Buscar en botones de roles del sistema
                bool encontrado = false;
                foreach (Control ctrl in splMain.Panel1.Controls)
                {
                    if (ctrl is Button btn && btn.Name.StartsWith("btnRol") && (int)btn.Tag == rolSeleccionadoID)
                    {
                        nombreRol = btn.Text.Replace("● ", "");
                        tipoRol = "Sistema";
                        encontrado = true;
                        break;
                    }
                }

                // Si no está en botones, buscar en ComboBox de roles personalizados
                if (!encontrado && cmbRolesPersonalizados.SelectedItem is ComboBoxRol rolPersonalizado)
                {
                    nombreRol = rolPersonalizado.NombreRol;
                    tipoRol = rolPersonalizado.EsSistema ? "Sistema" : "Personalizado";
                }

                // Mostrar con indicador de tipo
                lblIndicador.Text = $"Modo: Rol {tipoRol} ({nombreRol})";
                lblIndicador.ForeColor = Color.FromArgb(0, 120, 212); // Azul
            }
            else
            {
                lblIndicador.Text = "Modo: Esperando selección...";
                lblIndicador.ForeColor = Color.Gray;
            }
        }

        private void AjustarAnchoAccionesMasivas()
        {
            // Calcular ancho total disponible
            int anchoTotal = gbAccionesMasivas.Width - 30; // 30 = márgenes
            int anchoPorGrupo = (anchoTotal - 30) / 3; // Dividir en 3 grupos

            gbPorCategoria.Width = anchoPorGrupo;
            gbPresets.Left = gbPorCategoria.Right + 15;
            gbPresets.Width = anchoPorGrupo;
            gbPorAccion.Left = gbPresets.Right + 15;
            gbPorAccion.Width = anchoPorGrupo;

            // Ajustar botones dentro de cada grupo para ocupar todo el ancho
            int anchoBtnCategoria = gbPorCategoria.Width - 20;
            btnAccesoCompleto.Width = anchoBtnCategoria;
            btnDenegarCategoria.Width = anchoBtnCategoria;
            btnSoloConsulta.Width = (anchoBtnCategoria - 5) / 2;
            btnConsultaAvanzada.Width = (anchoBtnCategoria - 5) / 2;
            btnConsultaAvanzada.Left = btnSoloConsulta.Right + 5;

            int anchoBtnPreset = (gbPresets.Width - 25) / 2;
            btnAccesoTotal.Width = anchoBtnPreset;
            btnGestionCompleta.Width = anchoBtnPreset;
            btnGestionCompleta.Left = btnAccesoTotal.Right + 5;
            btnSoloLectura.Width = anchoBtnPreset;
            btnSinAcceso.Width = anchoBtnPreset;
            btnSinAcceso.Left = btnSoloLectura.Right + 5;

            int anchoBtnAccion = gbPorAccion.Width - 20;
            btnToggleImprimir.Width = (anchoBtnAccion - 5) / 2;
            btnToggleEditar.Width = anchoBtnAccion;
        }

        private void ConfigurarPlaceholder()
        {
            txtBuscarFormulario.Text = PLACEHOLDER_BUSCAR;
            txtBuscarFormulario.ForeColor = Color.Gray;
            isPlaceholder = true;

            txtBuscarFormulario.Enter += TxtBuscar_Enter;
            txtBuscarFormulario.Leave += TxtBuscar_Leave;
        }

        private void TxtBuscar_Enter(object sender, EventArgs e)
        {
            if (isPlaceholder)
            {
                txtBuscarFormulario.Text = "";
                txtBuscarFormulario.ForeColor = Color.Black;
                isPlaceholder = false;
            }
        }

        private void TxtBuscar_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscarFormulario.Text))
            {
                txtBuscarFormulario.Text = PLACEHOLDER_BUSCAR;
                txtBuscarFormulario.ForeColor = Color.Gray;
                isPlaceholder = true;
            }
        }

        private void ConfigurarEstiloBoton(Button btn, Color backColor, Color foreColor, bool esBorde)
        {
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;

            if (esBorde)
            {
                btn.FlatAppearance.BorderColor = foreColor == Color.White ? backColor : foreColor;
                btn.FlatAppearance.BorderSize = 1;
            }
            else
            {
                btn.FlatAppearance.BorderSize = 0;
            }

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = ControlPaint.Dark(backColor, 0.1f);
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = backColor;
            };
        }

        // Configurar eventos de controles del formulario para acciones masivas y otros
        private void ConfigurarEventos()
        {
            // Eventos de tabs
            tabPermisos.SelectedIndexChanged += TabPermisos_SelectedIndexChanged;

            // Eventos de filtros
            cmbCategoria.SelectedIndexChanged += Filtros_Changed;
            cmbModulo.SelectedIndexChanged += Filtros_Changed;
            txtBuscarFormulario.TextChanged += Filtros_Changed;

            // Eventos de usuario
            cmbUsuarios.SelectedIndexChanged += CmbUsuarios_SelectedIndexChanged;

            // Eventos de botones principales
            btnVolver.Click += BtnVolver_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnRecargar.Click += BtnRecargar_Click;
            btnReportes.Click += BtnReportes_Click;

            // ========================================
            // EVENTOS DE ACCIONES MASIVAS - REDISEÑADOS
            // ========================================

            // GRUPO 1: Por Categoría Filtrada
            btnAccesoCompleto.Click += (s, e) => AplicarAccionMasiva("ACCESO_COMPLETO_CATEGORIA");
            btnSoloConsulta.Click += (s, e) => AplicarAccionMasiva("SOLO_CONSULTA_CATEGORIA");
            btnConsultaAvanzada.Click += (s, e) => AplicarAccionMasiva("CONSULTA_AVANZADA_CATEGORIA");
            btnDenegarCategoria.Click += (s, e) => AplicarAccionMasiva("DENEGAR_CATEGORIA");

            // GRUPO 2: Perfiles Predefinidos
            btnAccesoTotal.Click += (s, e) => AplicarAccionMasiva("ACCESO_TOTAL");
            btnGestionCompleta.Click += (s, e) => AplicarAccionMasiva("GESTION_COMPLETA");
            btnSoloLectura.Click += (s, e) => AplicarAccionMasiva("SOLO_LECTURA");
            btnSinAcceso.Click += (s, e) => AplicarAccionMasiva("SIN_ACCESO");

            // GRUPO 3: Toggle por Acción (Inteligentes)
            btnToggleVer.Click += (s, e) => AplicarToggle("VIEW");
            btnToggleEditar.Click += (s, e) => AplicarToggle("EDIT");
            btnToggleImprimir.Click += (s, e) => AplicarToggle("PRINT");
            btnToggleExportar.Click += (s, e) => AplicarToggle("EXPORT");

            // Evento de limpiar filtros
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;

            // Eventos de gestión de roles
            btnCrearRol.Click += BtnCrearRol_Click;
            btnEliminarRol.Click += BtnEliminarRol_Click;
            btnCopiarPermisos.Click += BtnCopiarPermisos_Click;
            cmbRolesPersonalizados.SelectedIndexChanged += CmbRolesPersonalizados_SelectedIndexChanged;
        }

        private void BtnReportes_Click(object sender, EventArgs e)
        {
            FormGenerarReporte formReporte = new FormGenerarReporte();
            formReporte.ShowDialog();
        }

        private void AplicarToggle(string codigoAccion)
        {
            // Analizar estado actual de esta acción
            int total = 0;
            int activados = 0;

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (!row.Visible) continue;
                total++;

                if (row.Cells[codigoAccion].Value != null && (bool)row.Cells[codigoAccion].Value)
                {
                    activados++;
                }
            }

            // Determinar acción: si todos están activados → desactivar, sino → activar
            bool nuevoEstado = activados < total;

            // Aplicar nuevo estado
            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (!row.Visible) continue;
                row.Cells[codigoAccion].Value = nuevoEstado;
            }

            ActualizarResumen();
            ActualizarBotonesToggle();

            // Verificar si hay cambios reales después del toggle
            bool hayCambiosReales = HayCambiosReales();

            if (hayCambiosReales)
            {
                hayCambiosSinGuardar = true;
                btnGuardar.Text = "💾 Guardar Cambios *";
                btnGuardar.BackColor = colorGuardarCambios;
            }
            else
            {
                hayCambiosSinGuardar = false;
                btnGuardar.Text = "💾 Guardar Cambios";
                btnGuardar.BackColor = colorGuardarNormal;
            }

        }

        private void ActualizarBotonesToggle()
        {
            ActualizarBotonToggle(btnToggleVer, "VIEW", "Ver", "👁️");
            ActualizarBotonToggle(btnToggleEditar, "EDIT", "Editar", "✏️");
            ActualizarBotonToggle(btnToggleImprimir, "PRINT", "Imprimir", "🖨️");
            ActualizarBotonToggle(btnToggleExportar, "EXPORT", "Exportar", "📋");
        }

        private void ActualizarBotonToggle(Button btn, string codigoAccion, string nombreAccion, string icono)
        {
            int total = 0;
            int activados = 0;

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (!row.Visible) continue;
                total++;

                if (row.Cells[codigoAccion].Value != null && (bool)row.Cells[codigoAccion].Value)
                {
                    activados++;
                }
            }

            // Colores
            Color colorActivar = Color.FromArgb(0, 120, 212); // Azul
            Color colorDesactivar = Color.FromArgb(239, 108, 0); // Naranja suave

            if (total == 0)
            {
                btn.Text = $"{icono} Activar {nombreAccion} (0/0)";
                btn.BackColor = colorActivar;
                btn.ForeColor = Color.White;
                btn.Tag = "ACTIVAR";

                // Quitar eventos previos
                btn.MouseEnter -= Btn_MouseEnter;
                btn.MouseLeave -= Btn_MouseLeave;
                return;
            }

            if (activados == total)
            {
                // Todos activados → Mostrar DESACTIVAR
                btn.Text = $"✓ Desactivar {nombreAccion} ({activados}/{total})";
                btn.BackColor = colorDesactivar;
                btn.ForeColor = Color.White;
                btn.Tag = "DESACTIVAR";
            }
            else
            {
                // Algunos o ninguno → Mostrar ACTIVAR
                btn.Text = $"{icono} Activar {nombreAccion} ({activados}/{total})";
                btn.BackColor = colorActivar;
                btn.ForeColor = Color.White;
                btn.Tag = "ACTIVAR";
            }

            // Quitar eventos anteriores para evitar duplicados
            btn.MouseEnter -= Btn_MouseEnter;
            btn.MouseLeave -= Btn_MouseLeave;

            // Agregar eventos hover que respetan el estado
            btn.MouseEnter += Btn_MouseEnter;
            btn.MouseLeave += Btn_MouseLeave;
        }

        // Eventos compartidos para hover de botones toggle
        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            if (btn.Tag?.ToString() == "DESACTIVAR")
            {
                btn.BackColor = Color.FromArgb(213, 96, 0); // Naranja más oscuro
            }
            else
            {
                btn.BackColor = Color.FromArgb(0, 100, 180); // Azul más oscuro
            }
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            if (btn.Tag?.ToString() == "DESACTIVAR")
            {
                btn.BackColor = Color.FromArgb(239, 108, 0); // Naranja suave original
            }
            else
            {
                btn.BackColor = Color.FromArgb(0, 120, 212); // Azul original
            }
        }

        private void BtnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            // Verificar cambios sin guardar PRIMERO
            if (!VerificarCambiosSinGuardar())
            {
                return; // No limpiar si hay cambios sin guardar
            }

            cargandoDatos = true;

            // Limpiar filtros
            cmbCategoria.SelectedIndex = 0;
            cmbModulo.SelectedIndex = 0;
            txtBuscarFormulario.Text = PLACEHOLDER_BUSCAR;
            txtBuscarFormulario.ForeColor = Color.Gray;
            isPlaceholder = true;

            // Deseleccionar usuario si está en tab de usuario
            if (tabPermisos.SelectedTab == tabPorUsuario)
            {
                cmbUsuarios.SelectedIndex = 0;
                usuarioSeleccionadoID = 0;
            }

            // Deseleccionar rol personalizado si está en tab de rol
            if (tabPermisos.SelectedTab == tabPorRol)
            {
                cmbRolesPersonalizados.SelectedIndex = 0;
            }

            // Deseleccionar rol y limpiar grid
            rolSeleccionadoID = 0;

            // Deseleccionar todos los botones de roles
            foreach (Control ctrl in splMain.Panel1.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("btnRol"))
                {
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.FromArgb(70, 70, 70);
                    btn.Width = 260;
                    btn.Left = 20;
                }
            }

            // Limpiar grid
            dgvPermisos.Rows.Clear();

            cargandoDatos = false;

            // Actualizar resumen a cero
            ActualizarResumen();

            // RESTAURAR INDICADOR DE MODO
            ActualizarIndicadorModo();

            // AGREGAR ESTA LÍNEA:
            ActualizarBotonesToggle();

            MessageBox.Show("Filtros y selecciones limpiadas correctamente.",
                "Limpiar Filtros",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #region Gestión de Roles Dinámicos

        private void BtnCrearRol_Click(object sender, EventArgs e)
        {
            // Verificar cambios sin guardar
            if (!VerificarCambiosSinGuardar())
            {
                return;
            }

            // Solo ROOT puede crear roles
            if (!SesionActual.EsRoot())
            {
                MessageBox.Show("Solo el usuario ROOT puede crear roles.", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormCrearRol formCrear = new FormCrearRol();
            if (formCrear.ShowDialog() == DialogResult.OK)
            {
                // Recargar ComboBox de roles
                CargarRolesPersonalizados();

                // AGREGAR: Recargar panel de botones
                CargarRoles();

                // Seleccionar el nuevo rol
                for (int i = 0; i < cmbRolesPersonalizados.Items.Count; i++)
                {
                    var item = cmbRolesPersonalizados.Items[i] as ComboBoxRol;
                    if (item != null && item.RolID == formCrear.NuevoRolID)
                    {
                        cmbRolesPersonalizados.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void BtnEliminarRol_Click(object sender, EventArgs e)
        {
            // Verificar cambios sin guardar
            if (!VerificarCambiosSinGuardar())
            {
                return;
            }

            // Solo ROOT puede eliminar roles
            if (!SesionActual.EsRoot())
            {
                MessageBox.Show("Solo el usuario ROOT puede eliminar roles.", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbRolesPersonalizados.SelectedIndex <= 0)
            {
                MessageBox.Show("Debe seleccionar un rol para eliminar.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rolSeleccionado = cmbRolesPersonalizados.SelectedItem as ComboBoxRol;

            // Validar que no sea un rol de sistema
            if (rolSeleccionado.EsSistema)
            {
                MessageBox.Show("No puede eliminar roles del sistema (ROOT, ADMIN, etc.).\n\n" +
                    "Solo se pueden eliminar roles personalizados.", "Rol Protegido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar que no haya usuarios con este rol
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sqlCount = "SELECT COUNT(*) FROM Usuarios WHERE RolID = @RolID AND EsEliminado = 0";
                using (var cmd = new SqlCommand(sqlCount, conn))
                {
                    cmd.Parameters.AddWithValue("@RolID", rolSeleccionado.RolID);
                    int cantidadUsuarios = (int)cmd.ExecuteScalar();

                    if (cantidadUsuarios > 0)
                    {
                        MessageBox.Show($"No puede eliminar este rol porque tiene {cantidadUsuarios} usuario(s) asignado(s).\n\n" +
                            "Primero reasigne los usuarios a otro rol.", "Rol en Uso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de eliminar el rol '{rolSeleccionado.NombreRol}'?\n\n" +
                "Esta acción también eliminará todos sus permisos.",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Eliminar permisos del rol
                            string sqlPermisos = "DELETE FROM PermisosRol WHERE RolID = @RolID";
                            using (var cmdPermisos = new SqlCommand(sqlPermisos, conn, transaction))
                            {
                                cmdPermisos.Parameters.AddWithValue("@RolID", rolSeleccionado.RolID);
                                cmdPermisos.ExecuteNonQuery();
                            }

                            // Eliminar rol
                            string sqlRol = "DELETE FROM Roles WHERE RolID = @RolID";
                            using (var cmdRol = new SqlCommand(sqlRol, conn, transaction))
                            {
                                cmdRol.Parameters.AddWithValue("@RolID", rolSeleccionado.RolID);
                                cmdRol.ExecuteNonQuery();
                            }

                            // Auditoría
                            AuditoriaHelper.RegistrarAccion(
                                SesionActual.UsuarioID,
                                "ELIMINAR_ROL",
                                "SISTEMA",
                                "Gestión de Roles",
                                "FormAdministrarPermisos",
                                registroID: rolSeleccionado.RolID,
                                detalle: $"Rol '{rolSeleccionado.NombreRol}' eliminado"
                            );

                            transaction.Commit();

                            MessageBox.Show("Rol eliminado exitosamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Recargar ComboBox
                            CargarRolesPersonalizados();

                            // AGREGAR: Recargar panel de botones
                            CargarRoles();

                            // Limpiar grid
                            dgvPermisos.Rows.Clear();
                            ActualizarResumen();
                            ActualizarIndicadorModo();

                            // QUITAR FOCO de los botones del panel1
                            splMain.Panel2.Focus(); // Dar foco al panel derecho en su lugar
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar rol:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCopiarPermisos_Click(object sender, EventArgs e)
        {
            FormCopiarPermisos formCopiar = new FormCopiarPermisos();

            if (formCopiar.ShowDialog() == DialogResult.OK)
            {
                // Los permisos se guardaron directamente en la BD
                // Ofrecer recargar si hay algo seleccionado
                if (rolSeleccionadoID > 0 || usuarioSeleccionadoID > 0)
                {
                    DialogResult recargar = MessageBox.Show(
                        "✅ Los permisos se copiaron y guardaron exitosamente en la base de datos.\n\n" +
                        "¿Desea RECARGAR los permisos del rol/usuario actual para ver los cambios?",
                        "Permisos Copiados",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (recargar == DialogResult.Yes)
                    {
                        // Recargar según el contexto
                        if (usuarioSeleccionadoID > 0)
                        {
                            CargarPermisosUsuario();
                        }
                        else
                        {
                            CargarPermisos();
                        }

                        MessageBox.Show(
                            "Permisos recargados exitosamente.",
                            "Recarga Completa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
                else
                {
                    MessageBox.Show(
                        "✅ Los permisos se copiaron y guardaron exitosamente en la base de datos.\n\n" +
                        "Seleccione el rol destino para ver los cambios.",
                        "Permisos Copiados",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        private void CmbRolesPersonalizados_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si estamos sincronizando, no hacer nada (evitar loop)
            if (sincronizandoRoles) return;

            // Verificar cambios sin guardar
            if (!VerificarCambiosSinGuardar())
            {
                return;
            }

            if (cmbRolesPersonalizados.SelectedIndex > 0)
            {
                var rolSeleccionado = cmbRolesPersonalizados.SelectedItem as ComboBoxRol;
                if (rolSeleccionado != null)
                {
                    rolSeleccionadoID = rolSeleccionado.RolID;

                    // Deseleccionar usuario
                    usuarioSeleccionadoID = 0;
                    if (cmbUsuarios.SelectedIndex > 0)
                    {
                        cmbUsuarios.SelectedIndex = 0;
                    }

                    // Deseleccionar todos los botones de roles primero
                    foreach (Control ctrl in splMain.Panel1.Controls)
                    {
                        if (ctrl is Button btn && btn.Name.StartsWith("btnRol"))
                        {
                            btn.BackColor = Color.White;
                            btn.ForeColor = Color.FromArgb(70, 70, 70);
                            btn.Width = 260;
                            btn.Left = 20;
                        }
                    }

                    // SINCRONIZAR botón del panel1 si el rol existe ahí
                    SincronizarBotonRolSistema(rolSeleccionadoID);

                    // Cargar permisos del ROL (no del usuario)
                    CargarPermisos();
                    ActualizarIndicadorModo();
                }
            }
            else
            {
                rolSeleccionadoID = 0;
                dgvPermisos.Rows.Clear();
                ActualizarResumen();
                ActualizarIndicadorModo();
            }
        }

        private void SincronizarBotonRolSistema(int rolID)
        {
            // Buscar si el rol existe en los botones del panel1
            foreach (Control ctrl in splMain.Panel1.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("btnRol"))
                {
                    if ((int)btn.Tag == rolID)
                    {
                        // Seleccionar el botón
                        btn.BackColor = Color.FromArgb(0, 120, 212);
                        btn.ForeColor = Color.White;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Carga de Datos

        private void CargarRoles()
        {
            try
            {
                // Limpiar panel izquierdo (Panel1 del SplitContainer)
                splMain.Panel1.Controls.Clear();

                // Título
                Label lblTitulo = new Label
                {
                    Text = "ROLES DEL SISTEMA",
                    Location = new Point(20, 20),
                    Size = new Size(260, 30),
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 120, 212)
                };
                splMain.Panel1.Controls.Add(lblTitulo);

                int yPosition = 60;

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Cargar máximo 7 roles: primero los de sistema, luego personalizados
                    string sql = @"
                    SELECT TOP 7 
                        RolID, 
                        NombreRol, 
                        Descripcion,
                        EsSistema
                    FROM Roles
                    WHERE Activo = 1
                    ORDER BY 
                        CASE WHEN EsSistema = 1 THEN 0 ELSE 1 END,  -- Primero sistema, luego personalizados
                        FechaCreacion                                -- Por orden de creación
                     ";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rolID = reader.GetInt32(0);
                            string nombreRol = reader.GetString(1);
                            string descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            bool esSistema = reader.GetBoolean(3);

                            // Crear botón del rol
                            Button btnRol = new Button
                            {
                                Name = $"btnRol_{rolID}",
                                Text = esSistema ? $"● {nombreRol}" : $"⚙ {nombreRol}", // ⚙ para personalizados
                                Location = new Point(20, yPosition),
                                Size = new Size(260, 50),
                                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                                BackColor = Color.White,
                                ForeColor = Color.FromArgb(70, 70, 70),
                                FlatStyle = FlatStyle.Flat,
                                TextAlign = ContentAlignment.MiddleLeft,
                                Cursor = Cursors.Hand,
                                Tag = rolID
                            };

                            btnRol.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
                            btnRol.FlatAppearance.BorderSize = 1;

                            // Tooltip mejorado con temporizador
                            ToolTip tooltip = new ToolTip
                            {
                                AutoPopDelay = 3000,
                                InitialDelay = 500,
                                ReshowDelay = 200,
                                ShowAlways = true
                            };

                            string tipoRol = esSistema ? "Rol del Sistema" : "Rol Personalizado";
                            string tooltipText = $"{tipoRol}\n\n{descripcion}";

                            // Agregar cantidad de usuarios con este rol
                            using (var connTooltip = DatabaseConnection.GetConnection())
                            {
                                connTooltip.Open();
                                string sqlCount = "SELECT COUNT(*) FROM Usuarios WHERE RolID = @RolID AND Activo = 1 AND EsEliminado = 0";
                                using (var cmdCount = new SqlCommand(sqlCount, connTooltip))
                                {
                                    cmdCount.Parameters.AddWithValue("@RolID", rolID);
                                    int cantidadUsuarios = (int)cmdCount.ExecuteScalar();
                                    tooltipText += $"\n\nUsuarios activos: {cantidadUsuarios}";
                                }
                            }

                            tooltip.SetToolTip(btnRol, tooltipText);

                            // Evento para ocultar tooltip al quitar el mouse
                            btnRol.MouseLeave += (s, e) =>
                            {
                                tooltip.Hide(btnRol);
                            };

                            tooltip.SetToolTip(btnRol, tooltipText);

                            // Variables para animación
                            Timer animTimer = new Timer { Interval = 10 };
                            int targetWidth = 260;
                            int targetLeft = 20;
                            bool isExpanding = false;

                            // Evento MouseEnter (Hover)
                            btnRol.MouseEnter += (s, e) =>
                            {
                                if (rolSeleccionadoID != rolID)
                                {
                                    isExpanding = true;
                                    targetWidth = 270;
                                    targetLeft = 15;
                                    animTimer.Start();
                                    btnRol.BackColor = Color.FromArgb(240, 248, 255);
                                    btnRol.ForeColor = Color.FromArgb(0, 120, 212);
                                }
                            };

                            // Evento MouseLeave
                            btnRol.MouseLeave += (s, e) =>
                            {
                                if (rolSeleccionadoID != rolID)
                                {
                                    isExpanding = false;
                                    targetWidth = 260;
                                    targetLeft = 20;
                                    animTimer.Start();
                                    btnRol.BackColor = Color.White;
                                    btnRol.ForeColor = Color.FromArgb(70, 70, 70);
                                }
                            };

                            // Timer para animación suave
                            animTimer.Tick += (s, e) =>
                            {
                                Button btn = btnRol;
                                bool needsUpdate = false;

                                if (isExpanding)
                                {
                                    if (btn.Width < targetWidth)
                                    {
                                        btn.Width += 2;
                                        needsUpdate = true;
                                    }
                                    if (btn.Left > targetLeft)
                                    {
                                        btn.Left -= 1;
                                        needsUpdate = true;
                                    }
                                }
                                else
                                {
                                    if (btn.Width > targetWidth)
                                    {
                                        btn.Width -= 2;
                                        needsUpdate = true;
                                    }
                                    if (btn.Left < targetLeft)
                                    {
                                        btn.Left += 1;
                                        needsUpdate = true;
                                    }
                                }

                                if (!needsUpdate)
                                {
                                    animTimer.Stop();
                                }
                            };

                            // Evento Click
                            // Evento Click
                            btnRol.Click += (s, e) =>
                            {

                                // Verificar cambios sin guardar
                                if (!VerificarCambiosSinGuardar())
                                {
                                    return;
                                }

                                // VALIDAR: Si hay usuario seleccionado y el rol no coincide
                                if (usuarioSeleccionadoID > 0)
                                {
                                    // Obtener rol del usuario
                                    int rolUsuario = 0;
                                    using (var connValidacion = DatabaseConnection.GetConnection())
                                    {
                                        connValidacion.Open();
                                        string sqlRol = "SELECT RolID FROM Usuarios WHERE UsuarioID = @UsuarioID";
                                        using (var cmdValidacion = new SqlCommand(sqlRol, connValidacion))
                                        {
                                            cmdValidacion.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);
                                            rolUsuario = Convert.ToInt32(cmdValidacion.ExecuteScalar());
                                        }
                                    }

                                    // Si el rol clickeado NO es el del usuario
                                    if (rolID != rolUsuario)
                                    {
                                        MessageBox.Show(
                                            "No puede cambiar de rol mientras tenga un usuario seleccionado.\n\n" +
                                            "Se ha deseleccionado el usuario para permitir la gestión de permisos del rol.",
                                            "Usuario Deseleccionado",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information
                                        );

                                        // Deseleccionar usuario
                                        usuarioSeleccionadoID = 0;
                                        cmbUsuarios.SelectedIndex = 0;
                                    }
                                    else
                                    {
                                        // Es el mismo rol, verificar si es doble click
                                        if (btnRol.BackColor == Color.FromArgb(0, 120, 212))
                                        {
                                            // Es doble click - deseleccionar usuario
                                            MessageBox.Show(
                                                "Usuario deseleccionado.\n\nAhora aplicará permisos al ROL completo.",
                                                "Modo: Permisos de Rol",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information
                                            );
                                            usuarioSeleccionadoID = 0;
                                            cmbUsuarios.SelectedIndex = 0;
                                        }
                                    }
                                }

                                // Deseleccionar todos los botones
                                foreach (Control ctrl in splMain.Panel1.Controls)
                                {
                                    if (ctrl is Button btn && btn.Name.StartsWith("btnRol"))
                                    {
                                        btn.BackColor = Color.White;
                                        btn.ForeColor = Color.FromArgb(70, 70, 70);
                                        btn.Width = 260;
                                        btn.Left = 20;
                                    }
                                }

                                // Seleccionar este botón (animación de "salto")
                                rolSeleccionadoID = rolID;
                                btnRol.BackColor = Color.FromArgb(0, 120, 212);
                                btnRol.ForeColor = Color.White;

                                // SINCRONIZAR cmbRolesPersonalizados
                                sincronizandoRoles = true;
                                SincronizarComboRolPersonalizado(rolID);
                                sincronizandoRoles = false;

                                // Animación de salto
                                Timer jumpTimer = new Timer { Interval = 20 };
                                int jumpHeight = 0;
                                bool jumpingUp = true;

                                jumpTimer.Tick += (sender2, e2) =>
                                {
                                    if (jumpingUp)
                                    {
                                        jumpHeight += 2;
                                        btnRol.Top -= 2;
                                        if (jumpHeight >= 10)
                                        {
                                            jumpingUp = false;
                                        }
                                    }
                                    else
                                    {
                                        jumpHeight -= 2;
                                        btnRol.Top += 2;
                                        if (jumpHeight <= 0)
                                        {
                                            jumpTimer.Stop();
                                        }
                                    }
                                };
                                jumpTimer.Start();

                                CargarPermisos();
                            };

                            splMain.Panel1.Controls.Add(btnRol);
                            yPosition += 60;
                        }
                    }
                }

                // Seleccionar el primer rol por defecto
                if (splMain.Panel1.Controls.Count > 1)
                {
                    var primerBoton = splMain.Panel1.Controls[1] as Button;
                    if (primerBoton != null)
                    {
                        primerBoton.PerformClick();
                    }
                }

                // Agregar GroupBox de Resumen más abajo
                AgregarResumenPermisos(yPosition + 50);

                // Agregar botón Volver al final
                Button btnVolverPanel = new Button
                {
                    Name = "btnVolverPanel",
                    Text = "← Volver",
                    Location = new Point(20, 940),
                    Size = new Size(120, 40),
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(0, 120, 212),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnVolverPanel.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 212);
                btnVolverPanel.FlatAppearance.BorderSize = 1;
                btnVolverPanel.Click += BtnVolver_Click;

                splMain.Panel1.Controls.Add(btnVolverPanel);

                // AGREGAR AL FINAL DEL MÉTODO:
                // Asegurar que ningún botón quede seleccionado visualmente
                foreach (Control ctrl in splMain.Panel1.Controls)
                {
                    if (ctrl is Button btn && btn.Name.StartsWith("btnRol"))
                    {
                        btn.BackColor = Color.White;
                        btn.ForeColor = Color.FromArgb(70, 70, 70);
                        btn.Width = 260;
                        btn.Left = 20;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarRolesPersonalizados()
        {
            try
            {
                cmbRolesPersonalizados.Items.Clear();
                cmbRolesPersonalizados.Items.Add("Seleccione un rol...");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Cargar TODOS los roles (sistema y personalizados)
                    string sql = @"SELECT RolID, NombreRol, Descripcion, EsSistema, Activo
                          FROM Roles
                          WHERE Activo = 1
                          ORDER BY EsSistema DESC, NombreRol";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ComboBoxRol
                            {
                                RolID = reader.GetInt32(0),
                                NombreRol = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                EsSistema = reader.GetBoolean(3),
                                Activo = reader.GetBoolean(4)
                            };

                            cmbRolesPersonalizados.Items.Add(item);
                        }
                    }
                }

                cmbRolesPersonalizados.DisplayMember = "DisplayConEtiqueta";
                cmbRolesPersonalizados.ValueMember = "RolID";
                cmbRolesPersonalizados.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AgregarResumenPermisos(int yPosition)
        {
            // Eliminar resumen anterior si existe
            var resumenAnterior = splMain.Panel1.Controls.OfType<GroupBox>()
                .FirstOrDefault(gb => gb.Name == "gbResumen");
            if (resumenAnterior != null)
            {
                splMain.Panel1.Controls.Remove(resumenAnterior);
            }

            GroupBox gbResumen = new GroupBox
            {
                Name = "gbResumen",
                Text = "Resumen de Permisos",
                Location = new Point(20, yPosition),
                Size = new Size(260, 355),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.White
            };

            lblTotalPermisos = new Label
            {
                Name = "lblTotalPermisos",
                Text = "Total: 0 permisos",
                Location = new Point(15, 40),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Regular)
            };

            lblPermitidos = new Label
            {
                Name = "lblPermitidos",
                Text = "✓ Permitidos: 0",
                Location = new Point(15, 85),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(34, 139, 34)
            };

            lblDenegados = new Label
            {
                Name = "lblDenegados",
                Text = "✗ Denegados: 0",
                Location = new Point(15, 130),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            Label lblAccesoCompleto = new Label
            {
                Name = "lblAccesoCompleto",
                Text = "🔓 Acceso completo: 0",
                Location = new Point(15, 200),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(0, 120, 212)
            };

            Label lblSinAcceso = new Label
            {
                Name = "lblSinAcceso",
                Text = "🔒 Sin acceso: 0",
                Location = new Point(15, 260),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            // Línea separadora visual
            Label lblSeparador1 = new Label
            {
                Location = new Point(15, 175),
                Size = new Size(230, 2),
                BackColor = Color.FromArgb(200, 200, 200)
            };

            gbResumen.Controls.Add(lblTotalPermisos);
            gbResumen.Controls.Add(lblPermitidos);
            gbResumen.Controls.Add(lblDenegados);
            gbResumen.Controls.Add(lblSeparador1);
            gbResumen.Controls.Add(lblAccesoCompleto);
            gbResumen.Controls.Add(lblSinAcceso);

            splMain.Panel1.Controls.Add(gbResumen);
        }

        private void CargarComboBoxes()
        {
            try
            {
                cargandoDatos = true;

                // Cargar categorías
                cmbCategoria.Items.Clear();
                cmbCategoria.Items.Add("Todas las Categorías");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT NombreCategoria 
                                  FROM CatalogoCategorias 
                                  WHERE Activo = 1 
                                  ORDER BY OrdenVisualizacion";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbCategoria.Items.Add(reader.GetString(0));
                        }
                    }
                }

                cmbCategoria.SelectedIndex = 0;

                // Cargar módulos (inicialmente todos)
                CargarModulos();

                // Cargar usuarios (para tab de usuario)
                CargarUsuarios();

                cargandoDatos = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar filtros:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cargandoDatos = false;
            }
        }

        private void CargarModulos(string categoria = null)
        {
            try
            {
                cargandoDatos = true;

                cmbModulo.Items.Clear();
                cmbModulo.Items.Add("Todos los Módulos");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT DISTINCT M.NombreModulo 
                          FROM CatalogoModulos M
                          INNER JOIN CatalogoFormularios F ON M.ModuloID = F.ModuloID";

                    if (!string.IsNullOrEmpty(categoria) && categoria != "Todas las Categorías")
                    {
                        sql += @" INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                         WHERE C.NombreCategoria = @Categoria AND M.Activo = 1 AND F.Activo = 1";
                    }
                    else
                    {
                        sql += " WHERE M.Activo = 1 AND F.Activo = 1";
                    }

                    sql += " ORDER BY M.NombreModulo";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (!string.IsNullOrEmpty(categoria) && categoria != "Todas las Categorías")
                        {
                            cmd.Parameters.AddWithValue("@Categoria", categoria);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbModulo.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                cmbModulo.SelectedIndex = 0;
                cargandoDatos = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar módulos:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cargandoDatos = false;
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                cmbUsuarios.Items.Clear();
                cmbUsuarios.Items.Add("Seleccione un usuario...");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT U.UsuarioID, U.Username, U.NombreCompleto, R.NombreRol
                          FROM Usuarios U
                          INNER JOIN Roles R ON U.RolID = R.RolID
                          WHERE U.Activo = 1 AND U.EsEliminado = 0
                          ORDER BY U.NombreCompleto";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ComboBoxUsuario
                            {
                                UsuarioID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                NombreCompleto = reader.GetString(2),
                                NombreRol = reader.GetString(3)
                            };

                            cmbUsuarios.Items.Add(item);
                        }
                    }
                }

                cmbUsuarios.DisplayMember = "Display";
                cmbUsuarios.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPermisos()
        {
            if (rolSeleccionadoID == 0)
                return;

            try
            {
                // ACTIVAR FLAG para evitar detección de cambios durante carga
                cargandoPermisos = true;

                this.Cursor = Cursors.WaitCursor;

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Consulta compleja que trae formularios y permisos
                    string sql = @"
                SELECT 
                    C.CategoriaID,
                    C.NombreCategoria,
                    M.ModuloID,
                    M.NombreModulo,
                    F.FormularioID,
                    F.CodigoFormulario,
                    F.NombreFormulario,
                    A.AccionID,
                    A.CodigoAccion,
                    A.NombreAccion,
                    ISNULL(PR.Permitido, 0) AS Permitido
                FROM CatalogoFormularios F
                INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                CROSS JOIN CatalogoAcciones A
                LEFT JOIN PermisosRol PR ON PR.FormularioID = F.FormularioID 
                    AND PR.AccionID = A.AccionID 
                    AND PR.RolID = @RolID
                WHERE F.Activo = 1 AND A.Activo = 1
                    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'REPRINT', 'EXPORT', 'RESET', 'ACTIVATE')
                ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion, A.OrdenVisualizacion";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@RolID", rolSeleccionadoID);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            dtPermisos = new DataTable();
                            adapter.Fill(dtPermisos);
                        }
                    }
                }

                LlenarDataGridView();
                ActualizarResumen();
                ActualizarIndicadorModo();

                // GUARDAR ESTADO ORIGINAL para vista previa
                GuardarEstadoOriginal();

                // DESACTIVAR FLAG después de cargar
                cargandoPermisos = false;

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                cargandoPermisos = false; // Asegurar que se desactive en caso de error

                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar permisos:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Configuración del DataGridView

        private void ConfigurarDataGridView()
        {
            dgvPermisos.Columns.Clear();
            dgvPermisos.AutoGenerateColumns = false;
            dgvPermisos.AllowUserToAddRows = false;
            dgvPermisos.AllowUserToDeleteRows = false;
            dgvPermisos.RowHeadersVisible = false;
            dgvPermisos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPermisos.BorderStyle = BorderStyle.FixedSingle;
            dgvPermisos.BackgroundColor = Color.White;
            dgvPermisos.GridColor = Color.FromArgb(230, 230, 230);

            // Deshabilitar redimensionamiento
            dgvPermisos.AllowUserToResizeRows = false;
            dgvPermisos.AllowUserToResizeColumns = false;
            dgvPermisos.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvPermisos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Columnas de información (alineadas a la izquierda)
            dgvPermisos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoría",
                FillWeight = 15,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            dgvPermisos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                HeaderText = "Módulo",
                FillWeight = 18,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            dgvPermisos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                HeaderText = "Formulario",
                FillWeight = 20,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            // Columnas de acciones (checkboxes - centrados)
            var acciones = new[]
                {
                    new { Codigo = "VIEW", Nombre = "Ver" },
                    new { Codigo = "CREATE", Nombre = "Crear" },
                    new { Codigo = "EDIT", Nombre = "Editar" },
                    new { Codigo = "DELETE", Nombre = "Eliminar" },
                    new { Codigo = "PRINT", Nombre = "Imprimir" },
                    new { Codigo = "REPRINT", Nombre = "Reimp." },
                    new { Codigo = "EXPORT", Nombre = "Exportar" },
                    new { Codigo = "RESET", Nombre = "Reset" },
                    new { Codigo = "ACTIVATE", Nombre = "Activar" }
                };

            foreach (var accion in acciones)
            {
                var col = new DataGridViewCheckBoxColumn
                {
                    Name = accion.Codigo,
                    HeaderText = accion.Nombre,
                    FillWeight = 5,
                    ReadOnly = false
                };
                dgvPermisos.Columns.Add(col);
            }

            // Estilos generales
            dgvPermisos.EnableHeadersVisualStyles = false;
            dgvPermisos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dgvPermisos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPermisos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvPermisos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPermisos.ColumnHeadersHeight = 35;

            // Tamaño de letra más grande para celdas
            dgvPermisos.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            dgvPermisos.RowTemplate.Height = 45;
            dgvPermisos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // Alineación de headers de las primeras 3 columnas a la izquierda
            dgvPermisos.Columns["Categoria"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvPermisos.Columns["Modulo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvPermisos.Columns["Formulario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Evento para validar cambios
            dgvPermisos.CellValueChanged += DgvPermisos_CellValueChanged;
            dgvPermisos.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvPermisos.IsCurrentCellDirty)
                {
                    dgvPermisos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
        }

        private void LlenarDataGridView()
        {
            if (dtPermisos == null || dtPermisos.Rows.Count == 0)
                return;

            dgvPermisos.Rows.Clear();

            // Determinar si estamos en modo usuario (para mostrar excepciones)
            bool modoUsuario = usuarioSeleccionadoID > 0;

            // Agrupar por formulario
            var formularios = dtPermisos.AsEnumerable()
                .GroupBy(r => new
                {
                    CategoriaID = r.Field<int>("CategoriaID"),
                    Categoria = r.Field<string>("NombreCategoria"),
                    ModuloID = r.Field<int>("ModuloID"),
                    Modulo = r.Field<string>("NombreModulo"),
                    FormularioID = r.Field<int>("FormularioID"),
                    CodigoFormulario = r.Field<string>("CodigoFormulario"),
                    NombreFormulario = r.Field<string>("NombreFormulario")
                })
                .Select(g => new
                {
                    g.Key.CategoriaID,
                    g.Key.Categoria,
                    g.Key.ModuloID,
                    g.Key.Modulo,
                    g.Key.FormularioID,
                    g.Key.CodigoFormulario,
                    g.Key.NombreFormulario,
                    Permisos = g.ToDictionary(
                        r => r.Field<string>("CodigoAccion"),
                        r => r.Field<bool>("Permitido")
                    ),
                    // Agregar diccionario de excepciones si está en modo usuario
                    Excepciones = modoUsuario && dtPermisos.Columns.Contains("EsExcepcion")
                        ? g.ToDictionary(
                            r => r.Field<string>("CodigoAccion"),
                            r => r.Field<bool>("EsExcepcion")
                          )
                        : new Dictionary<string, bool>()
                });

            foreach (var form in formularios)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvPermisos);

                row.Cells[0].Value = form.Categoria;
                row.Cells[1].Value = form.Modulo;
                row.Cells[2].Value = form.NombreFormulario;

                // Tag con información adicional
                row.Tag = new
                {
                    form.CategoriaID,
                    form.ModuloID,
                    form.FormularioID,
                    form.CodigoFormulario,
                    form.Categoria,
                    form.Modulo
                };

                // Establecer valores de checkboxes
                for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                {
                    string codigoAccion = dgvPermisos.Columns[i].Name;

                    // Valor del permiso
                    if (form.Permisos.ContainsKey(codigoAccion))
                    {
                        row.Cells[i].Value = form.Permisos[codigoAccion];
                    }
                    else
                    {
                        row.Cells[i].Value = false;
                    }

                    // Colorear si es excepción del usuario
                    if (modoUsuario && form.Excepciones.ContainsKey(codigoAccion) && form.Excepciones[codigoAccion])
                    {
                        // Fondo naranja claro para excepciones
                        row.Cells[i].Style.BackColor = Color.FromArgb(255, 243, 205);
                        row.Cells[i].Style.SelectionBackColor = Color.FromArgb(255, 200, 100);

                        // Tooltip explicativo
                        row.Cells[i].ToolTipText = "⚠️ Excepción personal del usuario (sobrescribe permisos del rol)";
                    }
                }

                dgvPermisos.Rows.Add(row);
            }

            // AGREGAR AL FINAL
            ActualizarBotonesToggle();
        }   
        #endregion

        #region Eventos

        private void TvRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                rolSeleccionadoID = (int)e.Node.Tag;
                CargarPermisos();
            }
        }

        private void TabPermisos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabPermisos.SelectedTab == tabPorUsuario)
            {
                cmbUsuarios.Visible = true;

                // Efecto fade-in
                cmbUsuarios.Enabled = false;
                Timer fadeTimer = new Timer { Interval = 50 };
                fadeTimer.Tick += (s, ev) =>
                {
                    cmbUsuarios.Enabled = true;
                    fadeTimer.Stop();
                };
                fadeTimer.Start();
            }
            else
            {
                cmbUsuarios.Visible = false;

                // NO deseleccionar usuario automáticamente
                // Solo actualizar visibilidad del control
            }

            // NO llamar a ActualizarIndicadorModo() aquí
            // El indicador se actualiza solo cuando HAY UNA SELECCIÓN real
        }

        private void CmbUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Verificar cambios sin guardar
            if (!VerificarCambiosSinGuardar())
            {
                // Restaurar selección anterior (evitar cambio)
                return;
            }

            if (cmbUsuarios.SelectedIndex > 0)
            {
                // Verificar que el item sea del tipo correcto
                if (cmbUsuarios.SelectedItem is ComboBoxUsuario usuario)
                {
                    usuarioSeleccionadoID = usuario.UsuarioID;

                    // Obtener el RolID del usuario desde la BD
                    int rolUsuario = 0;
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        string sqlRol = "SELECT RolID FROM Usuarios WHERE UsuarioID = @UsuarioID";
                        using (var cmd = new SqlCommand(sqlRol, conn))
                        {
                            cmd.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);
                            rolUsuario = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }

                    // Si el rol actual NO coincide con el del usuario, cambiarlo
                    if (rolSeleccionadoID != rolUsuario)
                    {
                        // ACTIVAR FLAG para evitar que eventos en cascada sobrescriban
                        sincronizandoRoles = true;

                        // Seleccionar automáticamente el rol del usuario en el panel izquierdo
                        foreach (Control ctrl in splMain.Panel1.Controls)
                        {
                            if (ctrl is Button btn && btn.Name.StartsWith("btnRol"))
                            {
                                if ((int)btn.Tag == rolUsuario)
                                {
                                    // Deseleccionar todos primero
                                    foreach (Control ctrlDesel in splMain.Panel1.Controls)
                                    {
                                        if (ctrlDesel is Button btnDesel && btnDesel.Name.StartsWith("btnRol"))
                                        {
                                            btnDesel.BackColor = Color.White;
                                            btnDesel.ForeColor = Color.FromArgb(70, 70, 70);
                                            btnDesel.Width = 260;
                                            btnDesel.Left = 20;
                                        }
                                    }

                                    // Seleccionar el correcto
                                    btn.BackColor = Color.FromArgb(0, 120, 212);
                                    btn.ForeColor = Color.White;
                                    rolSeleccionadoID = rolUsuario;
                                    break;
                                }
                            }
                        }

                        // SINCRONIZAR cmbRolesPersonalizados también
                        SincronizarComboRolPersonalizado(rolUsuario);

                        // DESACTIVAR FLAG
                        sincronizandoRoles = false;
                    }

                    // SIEMPRE cargar permisos del usuario (no del rol)
                    CargarPermisosUsuario();
                    ActualizarIndicadorModo();
                }
            }
            else
            {
                usuarioSeleccionadoID = 0;
                dgvPermisos.Rows.Clear();
                ActualizarResumen();
                ActualizarIndicadorModo();
            }
        }

        private void SincronizarComboRolPersonalizado(int rolID)
        {
            // Buscar y seleccionar el rol en cmbRolesPersonalizados
            for (int i = 0; i < cmbRolesPersonalizados.Items.Count; i++)
            {
                var item = cmbRolesPersonalizados.Items[i] as ComboBoxRol;
                if (item != null && item.RolID == rolID)
                {
                    cmbRolesPersonalizados.SelectedIndex = i;
                    break;
                }
            }
        }

        private void CargarPermisosUsuario()
        {
            if (usuarioSeleccionadoID == 0)
                return;

            try
            {
                // ACTIVAR FLAG para evitar detección de cambios durante carga
                cargandoPermisos = true;

                this.Cursor = Cursors.WaitCursor;

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Obtener el RolID del usuario
                    string sqlRol = "SELECT RolID FROM Usuarios WHERE UsuarioID = @UsuarioID";
                    int rolIDUsuario = 0;

                    using (var cmd = new SqlCommand(sqlRol, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);
                        object resultRol = cmd.ExecuteScalar();

                        if (resultRol == null || resultRol == DBNull.Value)
                        {
                            MessageBox.Show("No se encontró el rol del usuario.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Cursor = Cursors.Default;
                            return;
                        }

                        rolIDUsuario = Convert.ToInt32(resultRol);
                    }

                    // Cargar permisos del rol + excepciones del usuario
                    string sql = @"
                SELECT 
                    C.CategoriaID,
                    C.NombreCategoria,
                    M.ModuloID,
                    M.NombreModulo,
                    F.FormularioID,
                    F.CodigoFormulario,
                    F.NombreFormulario,
                    A.AccionID,
                    A.CodigoAccion,
                    A.NombreAccion,
                    CAST(COALESCE(PU.Permitido, PR.Permitido, 0) AS BIT) AS Permitido,
                    CAST(CASE WHEN PU.PermisoUsuarioID IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS EsExcepcion
                FROM CatalogoFormularios F
                INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                CROSS JOIN CatalogoAcciones A
                LEFT JOIN PermisosRol PR ON PR.FormularioID = F.FormularioID 
                    AND PR.AccionID = A.AccionID 
                    AND PR.RolID = @RolID
                LEFT JOIN PermisosUsuario PU ON PU.FormularioID = F.FormularioID
                    AND PU.AccionID = A.AccionID
                    AND PU.UsuarioID = @UsuarioID
                WHERE F.Activo = 1 AND A.Activo = 1
                    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'REPRINT', 'EXPORT', 'RESET', 'ACTIVATE')
                ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion, A.OrdenVisualizacion";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@RolID", rolIDUsuario);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            dtPermisos = new DataTable();
                            adapter.Fill(dtPermisos);
                        }
                    }
                }

                LlenarDataGridView();
                ActualizarResumen();
                ActualizarIndicadorModo();

                // GUARDAR ESTADO ORIGINAL para vista previa
                GuardarEstadoOriginal();

                // DESACTIVAR FLAG después de cargar
                cargandoPermisos = false;

                this.Cursor = Cursors.Default;

                MessageBox.Show(
                    "Mostrando permisos del usuario.\n\n" +
                    "Ahora puede editar los checkboxes para asignar permisos específicos a este usuario.\n\n" +
                    "Los cambios se guardarán en la tabla PermisosUsuario.",
                    "Permisos por Usuario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                cargandoPermisos = false; // Asegurar que se desactive en caso de error

                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar permisos del usuario:\n\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            if (cargandoDatos) return;

            // Si cambia categoría, recargar módulos
            if (sender == cmbCategoria)
            {
                string categoriaSeleccionada = cmbCategoria.SelectedItem?.ToString();
                CargarModulos(categoriaSeleccionada);
            }

            AplicarFiltros();
            ActualizarBotonesToggle();
        }

        private void AplicarFiltros()
        {
            if (dgvPermisos.Rows.Count == 0) return;

            string categoriaFiltro = cmbCategoria.SelectedItem?.ToString();
            string moduloFiltro = cmbModulo.SelectedItem?.ToString();
            string formularioFiltro = isPlaceholder ? "" : txtBuscarFormulario.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                bool visible = true;

                // Filtro por categoría
                if (!string.IsNullOrEmpty(categoriaFiltro) && categoriaFiltro != "Todas las Categorías")
                {
                    visible = visible && row.Cells["Categoria"].Value?.ToString() == categoriaFiltro;
                }

                // Filtro por módulo
                if (!string.IsNullOrEmpty(moduloFiltro) && moduloFiltro != "Todos los Módulos")
                {
                    visible = visible && row.Cells["Modulo"].Value?.ToString() == moduloFiltro;
                }

                // Filtro por formulario
                if (!string.IsNullOrEmpty(formularioFiltro))
                {
                    string nombreForm = row.Cells["Formulario"].Value?.ToString().ToLower() ?? "";
                    visible = visible && nombreForm.Contains(formularioFiltro);
                }

                row.Visible = visible;
            }

            ActualizarResumen();
        }

        // Actualizar resumen de permisos al cambiar un checkbox (BOTON GUARDAR CAMBIOS)
        private void DgvPermisos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // NO marcar cambios si estamos cargando datos desde la BD
            if (cargandoPermisos) return;

            // Verificar si realmente hay cambios comparando con el estado original
            bool hayCambiosReales = HayCambiosReales();

            if (hayCambiosReales)
            {
                hayCambiosSinGuardar = true;
                btnGuardar.Text = "💾 Guardar Cambios *";
                btnGuardar.BackColor = colorGuardarCambios;
            }
            else
            {
                // Volvimos al estado original
                hayCambiosSinGuardar = false;
                btnGuardar.Text = "💾 Guardar Cambios";
                btnGuardar.BackColor = colorGuardarNormal;
            }

            ActualizarResumen();
            ActualizarBotonesToggle();
        }

        private void ActualizarResumen()
        {
            int totalFormularios = 0;
            int totalPermisos = 0;
            int permitidos = 0;
            int denegados = 0;
            int formulariosSinAcceso = 0;
            int formulariosAccesoCompleto = 0;

            var formulariosProcesados = new HashSet<int>();

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (!row.Visible || row.Tag == null) continue;

                try
                {
                    dynamic tag = row.Tag;
                    int formularioID = tag.FormularioID;

                    if (!formulariosProcesados.Contains(formularioID))
                    {
                        totalFormularios++;
                        formulariosProcesados.Add(formularioID);

                        // Contar si tiene acceso completo o sin acceso
                        bool tieneAlgunPermiso = false;
                        bool tieneAccesoCompleto = true;

                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            bool valor = row.Cells[i].Value != null && (bool)row.Cells[i].Value;
                            if (valor) tieneAlgunPermiso = true;
                            if (!valor) tieneAccesoCompleto = false;
                        }

                        if (!tieneAlgunPermiso) formulariosSinAcceso++;
                        if (tieneAccesoCompleto) formulariosAccesoCompleto++;
                    }

                    for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                    {
                        totalPermisos++;
                        bool valor = row.Cells[i].Value != null && (bool)row.Cells[i].Value;
                        if (valor)
                            permitidos++;
                        else
                            denegados++;
                    }
                }
                catch
                {
                    // Ignorar filas con problemas
                    continue;
                }
            }

            double porcentajePermitidos = totalPermisos > 0 ? (permitidos * 100.0 / totalPermisos) : 0;

            // Actualizar labels con información más detallada
            if (lblTotalPermisos != null)
                lblTotalPermisos.Text = $"Total: {totalPermisos} permisos ({totalFormularios} formularios)";

            if (lblPermitidos != null)
                lblPermitidos.Text = $"✓ Permitidos: {permitidos} ({porcentajePermitidos:F1}%)";

            if (lblDenegados != null)
                lblDenegados.Text = $"✗ Denegados: {denegados}";

            // Buscar labels adicionales en el GroupBox
            var gbResumen = splMain.Panel1.Controls.OfType<GroupBox>()
                .FirstOrDefault(gb => gb.Name == "gbResumen");

            if (gbResumen != null)
            {
                var lblAccesoCompleto = gbResumen.Controls["lblAccesoCompleto"] as Label;
                var lblSinAcceso = gbResumen.Controls["lblSinAcceso"] as Label;

                if (lblAccesoCompleto != null)
                    lblAccesoCompleto.Text = $"🔓 Acceso completo: {formulariosAccesoCompleto}";

                if (lblSinAcceso != null)
                    lblSinAcceso.Text = $"🔒 Sin acceso: {formulariosSinAcceso}";
            }
        }
        #endregion

        #region Acciones Masivas

        private void AplicarAccionMasiva(string tipo)
        {
            if (dgvPermisos.Rows.Count == 0)
            {
                MessageBox.Show("No hay permisos cargados.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar si es acción de categoría
            bool esAccionCategoria = tipo.Contains("CATEGORIA");

            string categoriaSeleccionada = "";
            if (esAccionCategoria)
            {
                categoriaSeleccionada = cmbCategoria.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(categoriaSeleccionada) || categoriaSeleccionada == "Todas las Categorías")
                {
                    MessageBox.Show("Debe seleccionar una categoría específica para aplicar acciones masivas.",
                        "Seleccione Categoría", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Mensaje de confirmación
            string accionDescripcion = tipo.Replace("_", " ").Replace("CATEGORIA", "");
            string mensaje = "";

            if (tipo == "DENEGAR_CATEGORIA")
            {
                mensaje = $"¿Está seguro de DENEGAR TODOS los permisos de la categoría '{categoriaSeleccionada}'?";
            }
            else if (tipo == "SIN_ACCESO")
            {
                mensaje = $"¿Está seguro de DENEGAR TODOS los permisos de TODOS los formularios visibles?\n\nEsta acción afectará TODOS los formularios, no solo una categoría.";
            }
            else if (esAccionCategoria)
            {
                mensaje = $"¿Está seguro de aplicar '{accionDescripcion}' a todos los formularios de '{categoriaSeleccionada}'?";
            }
            else
            {
                mensaje = $"¿Está seguro de aplicar '{accionDescripcion}' a TODOS los formularios visibles?";
            }

            DialogResult confirmacion = MessageBox.Show(mensaje, "Confirmar Acción Masiva",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (!row.Visible) continue;

                // Si es acción de categoría, verificar que pertenezca a esa categoría
                if (esAccionCategoria)
                {
                    string categoriaFila = row.Cells["Categoria"].Value?.ToString() ?? "";
                    if (categoriaFila != categoriaSeleccionada)
                        continue;
                }

                switch (tipo)
                {
                    case "ACCESO_COMPLETO_CATEGORIA":
                        // VIEW + CREATE + EDIT + DELETE + PRINT + EXPORT + REPRINT
                        row.Cells["VIEW"].Value = true;
                        row.Cells["CREATE"].Value = true;
                        row.Cells["EDIT"].Value = true;
                        row.Cells["DELETE"].Value = true;
                        row.Cells["PRINT"].Value = true;
                        row.Cells["EXPORT"].Value = true;
                        row.Cells["REPRINT"].Value = true;
                        break;

                    case "SOLO_CONSULTA_CATEGORIA":
                        // Solo VIEW
                        row.Cells["VIEW"].Value = true;
                        row.Cells["CREATE"].Value = false;
                        row.Cells["EDIT"].Value = false;
                        row.Cells["DELETE"].Value = false;
                        row.Cells["PRINT"].Value = false;
                        row.Cells["EXPORT"].Value = false;
                        row.Cells["REPRINT"].Value = false;
                        break;

                    case "CONSULTA_AVANZADA_CATEGORIA":
                        // VIEW + PRINT + EXPORT
                        row.Cells["VIEW"].Value = true;
                        row.Cells["CREATE"].Value = false;
                        row.Cells["EDIT"].Value = false;
                        row.Cells["DELETE"].Value = false;
                        row.Cells["PRINT"].Value = true;
                        row.Cells["EXPORT"].Value = true;
                        row.Cells["REPRINT"].Value = false;
                        break;

                    case "DENEGAR_CATEGORIA":
                        // Todo desactivado
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = false;
                        }
                        break;

                    case "ACCESO_TOTAL":
                        // Todos los permisos de TODO
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = true;
                        }
                        break;

                    case "GESTION_COMPLETA":
                        // VIEW + CREATE + EDIT + DELETE (sin imprimir/exportar)
                        row.Cells["VIEW"].Value = true;
                        row.Cells["CREATE"].Value = true;
                        row.Cells["EDIT"].Value = true;
                        row.Cells["DELETE"].Value = true;
                        row.Cells["PRINT"].Value = false;
                        row.Cells["EXPORT"].Value = false;
                        row.Cells["REPRINT"].Value = false;
                        break;

                    case "SOLO_LECTURA":
                        // VIEW + PRINT + EXPORT
                        row.Cells["VIEW"].Value = true;
                        row.Cells["CREATE"].Value = false;
                        row.Cells["EDIT"].Value = false;
                        row.Cells["DELETE"].Value = false;
                        row.Cells["PRINT"].Value = true;
                        row.Cells["EXPORT"].Value = true;
                        row.Cells["REPRINT"].Value = false;
                        break;

                    case "SIN_ACCESO":
                        // Todo desactivado
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = false;
                        }
                        break;
                }
            }

            ActualizarResumen();
            ActualizarBotonesToggle();

            // Verificar si hay cambios reales después de la acción masiva
            bool hayCambiosReales = HayCambiosReales();

            if (hayCambiosReales)
            {
                hayCambiosSinGuardar = true;
                btnGuardar.Text = "💾 Guardar Cambios *";
                btnGuardar.BackColor = colorGuardarCambios;
            }
            else
            {
                hayCambiosSinGuardar = false;
                btnGuardar.Text = "💾 Guardar Cambios";
                btnGuardar.BackColor = colorGuardarNormal;
            }
        }

        private void AplicarPreset(string preset)
        {
            if (dgvPermisos.Rows.Count == 0)
            {
                MessageBox.Show("No hay permisos cargados.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de aplicar el preset '{preset}' a TODOS los formularios visibles?",
                "Confirmar Preset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes) return;

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (!row.Visible) continue;

                switch (preset)
                {
                    case "AUDITOR":
                        // Solo ver
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = false;
                        }
                        row.Cells["VIEW"].Value = true;
                        break;

                    case "OPERADOR":
                        // Ver, crear, editar (sin eliminar)
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = false;
                        }
                        row.Cells["VIEW"].Value = true;
                        row.Cells["CREATE"].Value = true;
                        row.Cells["EDIT"].Value = true;
                        break;

                    case "ADMINISTRADOR":
                        // CRUD + impresión + exportar
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = false;
                        }
                        row.Cells["VIEW"].Value = true;
                        row.Cells["CREATE"].Value = true;
                        row.Cells["EDIT"].Value = true;
                        row.Cells["DELETE"].Value = true;
                        row.Cells["PRINT"].Value = true;
                        row.Cells["EXPORT"].Value = true;
                        break;

                    case "SIN_ACCESO":
                        // Denegar todo
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = false;
                        }
                        break;
                }
            }

            ActualizarResumen();
        }

        private void AplicarPorAccion(string tipoAccion)
        {
            if (dgvPermisos.Rows.Count == 0)
            {
                MessageBox.Show("No hay permisos cargados.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de aplicar esta acción a TODOS los formularios visibles?",
                "Confirmar Acción",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes) return;

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (!row.Visible) continue;

                switch (tipoAccion)
                {
                    case "SOLO_VER":
                        // Denegar todo primero
                        for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                        {
                            row.Cells[i].Value = false;
                        }
                        // Solo activar VIEW
                        row.Cells["VIEW"].Value = true;
                        break;

                    case "ACTIVAR_IMPRESION":
                        row.Cells["PRINT"].Value = true;
                        break;

                    case "ACTIVAR_REIMPRESION":
                        row.Cells["REPRINT"].Value = true;
                        break;

                    case "DENEGAR_ELIMINAR":
                        row.Cells["DELETE"].Value = false;
                        break;
                }
            }

            ActualizarResumen();
        }

        #endregion

        #region Guardar y Recargar

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // VALIDACIÓN 1: Debe haber un rol seleccionado SIEMPRE
            if (rolSeleccionadoID == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un ROL en 'ROLES DEL SISTEMA'.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // VALIDACIÓN 2: Determinar modo de guardado
            bool modoUsuarioEspecifico = usuarioSeleccionadoID > 0;

            // Si estamos en tab de usuario pero NO hay usuario seleccionado, avisar
            if (tabPermisos.SelectedTab == tabPorUsuario && usuarioSeleccionadoID == 0)
            {
                DialogResult cambiarTab = MessageBox.Show(
                    "Está en el tab 'Permisos por Usuario' pero no ha seleccionado ningún usuario.\n\n" +
                    "¿Desea guardar los permisos para TODO el rol seleccionado?\n\n" +
                    "Presione SÍ para continuar con el guardado del ROL.\n" +
                    "Presione NO para cancelar y seleccionar un usuario.",
                    "Confirmar Acción",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (cambiarTab == DialogResult.No)
                    return;
            }

            // VALIDACIÓN 3: Si hay usuario seleccionado, validar que el rol coincida
            if (modoUsuarioEspecifico)
            {
                var usuarioSeleccionado = cmbUsuarios.SelectedItem as ComboBoxUsuario;

                // Obtener el RolID del usuario desde la BD
                int rolRealUsuario = 0;
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sqlRol = "SELECT RolID FROM Usuarios WHERE UsuarioID = @UsuarioID";
                    using (var cmd = new SqlCommand(sqlRol, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);
                        rolRealUsuario = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                // Validar que el rol seleccionado coincida con el rol del usuario
                if (rolRealUsuario != rolSeleccionadoID)
                {
                    // Obtener nombre del rol correcto
                    string nombreRolCorrecto = "";
                    foreach (Control ctrl in splMain.Panel1.Controls)
                    {
                        if (ctrl is Button btn && btn.Name.StartsWith("btnRol") && (int)btn.Tag == rolRealUsuario)
                        {
                            nombreRolCorrecto = btn.Text.Replace("● ", "");
                            break;
                        }
                    }

                    MessageBox.Show(
                        $"ERROR: El usuario {usuarioSeleccionado.NombreCompleto} pertenece al rol:\n\n" +
                        $"'{nombreRolCorrecto}'\n\n" +
                        $"Debe seleccionar ese rol en 'ROLES DEL SISTEMA' antes de guardar permisos específicos.",
                        "Rol Incorrecto",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }

            // PREPARAR CONTEXTO para vista previa
            string nombreRol = "";
            string contextoVista = "";

            if (modoUsuarioEspecifico)
            {
                var usuarioSeleccionado = cmbUsuarios.SelectedItem as ComboBoxUsuario;
                contextoVista = $"Usuario: {usuarioSeleccionado.NombreCompleto} ({usuarioSeleccionado.Username})";
            }
            else
            {
                // Obtener nombre del rol seleccionado
                foreach (Control ctrl in splMain.Panel1.Controls)
                {
                    if (ctrl is Button btn && btn.Name.StartsWith("btnRol") && (int)btn.Tag == rolSeleccionadoID)
                    {
                        nombreRol = btn.Text.Replace("● ", "");
                        break;
                    }
                }

                if (string.IsNullOrEmpty(nombreRol) && cmbRolesPersonalizados.SelectedItem is ComboBoxRol rolPersonalizado)
                {
                    nombreRol = rolPersonalizado.NombreRol;
                }

                contextoVista = $"Rol: {nombreRol}";
            }

            // VALIDACIÓN DE SEGURIDAD: Solicitar contraseña para roles críticos
            if (!modoUsuarioEspecifico) // Solo para modificación de roles completos
            {
                // Verificar si es un rol crítico (ROOT o ADMIN)
                bool esRolCritico = nombreRol.ToUpper() == "ROOT" || nombreRol.ToUpper() == "ADMIN";

                if (esRolCritico)
                {
                    FormConfirmarPassword formPassword = new FormConfirmarPassword(nombreRol);

                    if (formPassword.ShowDialog() != DialogResult.OK)
                    {
                        // Usuario canceló o contraseña incorrecta
                        MessageBox.Show(
                            "Operación cancelada.\n\nNo se modificaron los permisos.",
                            "Cancelado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }
                }
            }

            // DETECTAR CAMBIOS para vista previa
            List<CambioPermiso> cambiosDetectados = DetectarCambios();

            if (cambiosDetectados.Count == 0)
            {
                MessageBox.Show(
                    "No hay cambios pendientes para guardar.",
                    "Sin Cambios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // MOSTRAR VISTA PREVIA DE CAMBIOS
            FormVistaPreviaCambios formVistaPrevia = new FormVistaPreviaCambios(cambiosDetectados, contextoVista);

            if (formVistaPrevia.ShowDialog() != DialogResult.OK)
            {
                // Usuario canceló desde la vista previa
                return;
            }

            // EJECUTAR GUARDADO (si usuario confirmó)
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (modoUsuarioEspecifico)
                {
                    GuardarPermisosUsuario();
                }
                else
                {
                    GuardarPermisosRol();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al guardar permisos:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GuardarPermisosRol()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvPermisos.Rows)
                        {
                            if (row.Tag == null) continue;

                            dynamic tag = row.Tag;
                            int formularioID = tag.FormularioID;

                            for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                            {
                                string codigoAccion = dgvPermisos.Columns[i].Name;
                                bool permitido = row.Cells[i].Value != null && (bool)row.Cells[i].Value;

                                // Obtener AccionID
                                string sqlAccionID = "SELECT AccionID FROM CatalogoAcciones WHERE CodigoAccion = @CodigoAccion";
                                int accionID;

                                using (var cmdAccion = new SqlCommand(sqlAccionID, conn, transaction))
                                {
                                    cmdAccion.Parameters.AddWithValue("@CodigoAccion", codigoAccion);
                                    accionID = (int)cmdAccion.ExecuteScalar();
                                }

                                // Verificar si ya existe el permiso
                                string sqlExiste = @"SELECT COUNT(*) FROM PermisosRol 
                                            WHERE RolID = @RolID 
                                            AND FormularioID = @FormularioID 
                                            AND AccionID = @AccionID";

                                bool existe;
                                using (var cmdExiste = new SqlCommand(sqlExiste, conn, transaction))
                                {
                                    cmdExiste.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                                    cmdExiste.Parameters.AddWithValue("@FormularioID", formularioID);
                                    cmdExiste.Parameters.AddWithValue("@AccionID", accionID);
                                    existe = (int)cmdExiste.ExecuteScalar() > 0;
                                }

                                if (existe)
                                {
                                    // Actualizar
                                    string sqlUpdate = @"UPDATE PermisosRol 
                                                SET Permitido = @Permitido,
                                                    FechaModificacion = GETDATE(),
                                                    ModificadoPorUsuarioID = @ModificadoPor
                                                WHERE RolID = @RolID 
                                                AND FormularioID = @FormularioID 
                                                AND AccionID = @AccionID";

                                    using (var cmdUpdate = new SqlCommand(sqlUpdate, conn, transaction))
                                    {
                                        cmdUpdate.Parameters.AddWithValue("@Permitido", permitido);
                                        cmdUpdate.Parameters.AddWithValue("@ModificadoPor", SesionActual.UsuarioID);
                                        cmdUpdate.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                                        cmdUpdate.Parameters.AddWithValue("@FormularioID", formularioID);
                                        cmdUpdate.Parameters.AddWithValue("@AccionID", accionID);
                                        cmdUpdate.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Insertar
                                    string sqlInsert = @"INSERT INTO PermisosRol 
                                                (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
                                                VALUES (@RolID, @FormularioID, @AccionID, @Permitido, @CreadoPor)";

                                    using (var cmdInsert = new SqlCommand(sqlInsert, conn, transaction))
                                    {
                                        cmdInsert.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                                        cmdInsert.Parameters.AddWithValue("@FormularioID", formularioID);
                                        cmdInsert.Parameters.AddWithValue("@AccionID", accionID);
                                        cmdInsert.Parameters.AddWithValue("@Permitido", permitido);
                                        cmdInsert.Parameters.AddWithValue("@CreadoPor", SesionActual.UsuarioID);
                                        cmdInsert.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        // Registrar en auditoría
                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            "MODIFICAR_PERMISOS_ROL",
                            "SISTEMA",
                            "Gestión de Roles",
                            "FormAdministrarPermisos",
                            registroID: rolSeleccionadoID,
                            detalle: $"Permisos modificados para rol ID: {rolSeleccionadoID}"
                        );

                        transaction.Commit();

                        MessageBox.Show("Permisos del ROL guardados exitosamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Después del MessageBox de éxito, agregar:
                        hayCambiosSinGuardar = false;
                        btnGuardar.Text = "💾 Guardar Cambios";
                        btnGuardar.BackColor = colorGuardarNormal;

                        // SINCRONIZAR estado original con grid (sin recargar desde BD)
                        SincronizarEstadoOriginalConGrid();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void GuardarPermisosUsuario()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvPermisos.Rows)
                        {
                            if (row.Tag == null) continue;

                            dynamic tag = row.Tag;
                            int formularioID = tag.FormularioID;

                            for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                            {
                                string codigoAccion = dgvPermisos.Columns[i].Name;
                                bool permitido = row.Cells[i].Value != null && (bool)row.Cells[i].Value;

                                // Obtener AccionID
                                string sqlAccionID = "SELECT AccionID FROM CatalogoAcciones WHERE CodigoAccion = @CodigoAccion";
                                int accionID;

                                using (var cmdAccion = new SqlCommand(sqlAccionID, conn, transaction))
                                {
                                    cmdAccion.Parameters.AddWithValue("@CodigoAccion", codigoAccion);
                                    accionID = (int)cmdAccion.ExecuteScalar();
                                }

                                // Verificar si ya existe el permiso de usuario
                                string sqlExiste = @"SELECT COUNT(*) FROM PermisosUsuario 
                                            WHERE UsuarioID = @UsuarioID 
                                            AND FormularioID = @FormularioID 
                                            AND AccionID = @AccionID";

                                bool existe;
                                using (var cmdExiste = new SqlCommand(sqlExiste, conn, transaction))
                                {
                                    cmdExiste.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);
                                    cmdExiste.Parameters.AddWithValue("@FormularioID", formularioID);
                                    cmdExiste.Parameters.AddWithValue("@AccionID", accionID);
                                    existe = (int)cmdExiste.ExecuteScalar() > 0;
                                }

                                if (existe)
                                {
                                    // Actualizar
                                    string sqlUpdate = @"UPDATE PermisosUsuario 
                                                SET Permitido = @Permitido,
                                                    FechaModificacion = GETDATE(),
                                                    ModificadoPorUsuarioID = @ModificadoPor
                                                WHERE UsuarioID = @UsuarioID 
                                                AND FormularioID = @FormularioID 
                                                AND AccionID = @AccionID";

                                    using (var cmdUpdate = new SqlCommand(sqlUpdate, conn, transaction))
                                    {
                                        cmdUpdate.Parameters.AddWithValue("@Permitido", permitido);
                                        cmdUpdate.Parameters.AddWithValue("@ModificadoPor", SesionActual.UsuarioID);
                                        cmdUpdate.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);
                                        cmdUpdate.Parameters.AddWithValue("@FormularioID", formularioID);
                                        cmdUpdate.Parameters.AddWithValue("@AccionID", accionID);
                                        cmdUpdate.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Insertar
                                    string sqlInsert = @"INSERT INTO PermisosUsuario 
                                                (UsuarioID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID, Motivo)
                                                VALUES (@UsuarioID, @FormularioID, @AccionID, @Permitido, @CreadoPor, @Motivo)";

                                    using (var cmdInsert = new SqlCommand(sqlInsert, conn, transaction))
                                    {
                                        cmdInsert.Parameters.AddWithValue("@UsuarioID", usuarioSeleccionadoID);
                                        cmdInsert.Parameters.AddWithValue("@FormularioID", formularioID);
                                        cmdInsert.Parameters.AddWithValue("@AccionID", accionID);
                                        cmdInsert.Parameters.AddWithValue("@Permitido", permitido);
                                        cmdInsert.Parameters.AddWithValue("@CreadoPor", SesionActual.UsuarioID);
                                        cmdInsert.Parameters.AddWithValue("@Motivo", "Permiso específico asignado manualmente");
                                        cmdInsert.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        // Registrar en auditoría
                        var usuarioSeleccionado = cmbUsuarios.SelectedItem as ComboBoxUsuario;
                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            "MODIFICAR_PERMISOS_USUARIO",
                            "SISTEMA",
                            "Gestión de Roles",
                            "FormAdministrarPermisos",
                            registroID: usuarioSeleccionadoID,
                            detalle: $"Permisos específicos modificados para usuario: {usuarioSeleccionado.Username}"
                        );

                        transaction.Commit();

                        MessageBox.Show($"Permisos específicos del usuario {usuarioSeleccionado.NombreCompleto} guardados exitosamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Después del MessageBox de éxito, agregar:
                        hayCambiosSinGuardar = false;
                        btnGuardar.Text = "💾 Guardar Cambios";
                        btnGuardar.BackColor = colorGuardarNormal;

                        // SINCRONIZAR estado original con grid (sin recargar desde BD)
                        SincronizarEstadoOriginalConGrid();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void SincronizarEstadoOriginalConGrid()
        {
            if (dtPermisosOriginal == null || dtPermisos == null)
                return;

            // Recorrer el grid y actualizar el estado original
            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (row.Tag == null) continue;

                dynamic tag = row.Tag;
                int formularioID = tag.FormularioID;

                // Actualizar cada acción
                for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                {
                    string codigoAccion = dgvPermisos.Columns[i].Name;
                    bool valorActual = row.Cells[i].Value != null && (bool)row.Cells[i].Value;

                    // Actualizar en dtPermisos
                    var rowsPermisos = dtPermisos.AsEnumerable()
                        .Where(r => r.Field<int>("FormularioID") == formularioID &&
                                   r.Field<string>("CodigoAccion") == codigoAccion)
                        .ToList();

                    foreach (var rowPermiso in rowsPermisos)
                    {
                        rowPermiso["Permitido"] = valorActual;
                    }

                    // Actualizar en dtPermisosOriginal
                    var rowsOriginales = dtPermisosOriginal.AsEnumerable()
                        .Where(r => r.Field<int>("FormularioID") == formularioID &&
                                   r.Field<string>("CodigoAccion") == codigoAccion)
                        .ToList();

                    foreach (var rowOriginal in rowsOriginales)
                    {
                        rowOriginal["Permitido"] = valorActual;
                    }
                }
            }
        }

        // Guardar una copia del estado original de los permisos para comparación
        private void GuardarEstadoOriginal()
        {
            // Clonar el DataTable de permisos actuales
            if (dtPermisos != null)
            {
                dtPermisosOriginal = dtPermisos.Copy();
            }
        }
        private void BtnRecargar_Click(object sender, EventArgs e)
        {
            // Determinar qué estamos recargando
            bool hayUsuarioSeleccionado = usuarioSeleccionadoID > 0;
            bool hayRolSeleccionado = rolSeleccionadoID > 0;

            if (!hayUsuarioSeleccionado && !hayRolSeleccionado)
            {
                MessageBox.Show(
                    "No hay nada seleccionado para recargar.\n\n" +
                    "Seleccione un rol o usuario primero.",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            string mensaje = "";
            if (hayUsuarioSeleccionado)
            {
                var usuario = cmbUsuarios.SelectedItem as ComboBoxUsuario;
                mensaje = $"¿Está seguro de recargar los permisos del usuario:\n\n" +
                          $"{usuario.NombreCompleto} ({usuario.Username})?\n\n" +
                          $"Se perderán los cambios no guardados.";
            }
            else
            {
                string nombreRol = "";

                // Buscar nombre del rol
                foreach (Control ctrl in splMain.Panel1.Controls)
                {
                    if (ctrl is Button btn && btn.Name.StartsWith("btnRol") && (int)btn.Tag == rolSeleccionadoID)
                    {
                        nombreRol = btn.Text.Replace("● ", "");
                        break;
                    }
                }

                if (string.IsNullOrEmpty(nombreRol) && cmbRolesPersonalizados.SelectedItem is ComboBoxRol rolPersonalizado)
                {
                    nombreRol = rolPersonalizado.NombreRol;
                }

                mensaje = $"¿Está seguro de recargar los permisos del rol:\n\n" +
                          $"{nombreRol}?\n\n" +
                          $"Se perderán los cambios no guardados.";
            }

            DialogResult confirmacion = MessageBox.Show(
                mensaje,
                "Confirmar Recarga",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Recargar según el contexto
                    if (hayUsuarioSeleccionado)
                    {
                        // Recargar permisos del USUARIO
                        CargarPermisosUsuario();

                        // RESETEAR indicador de cambios
                        hayCambiosSinGuardar = false;
                        btnGuardar.Text = "💾 Guardar Cambios";
                        btnGuardar.BackColor = colorGuardarNormal;

                        MessageBox.Show(
                            "Permisos del usuario recargados exitosamente.",
                            "Recarga Completa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        // Recargar permisos del ROL
                        CargarPermisos();

                        // RESETEAR indicador de cambios
                        hayCambiosSinGuardar = false;
                        btnGuardar.Text = "💾 Guardar Cambios";
                        btnGuardar.BackColor = colorGuardarNormal;

                        MessageBox.Show(
                            "Permisos del rol recargados exitosamente.",
                            "Recarga Completa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }

                    this.Cursor = Cursors.Default;
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(
                        $"Error al recargar permisos:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            FormDashboardRoles dashboardRoles = new FormDashboardRoles(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardRoles);
        }

        #endregion

        // Clase auxiliar para el ComboBox de usuarios
        private class ComboBoxUsuario
        {
            public int UsuarioID { get; set; }
            public string Username { get; set; }
            public string NombreCompleto { get; set; }
            public string NombreRol { get; set; }
            public string Display => $"{NombreCompleto} ({Username}) - {NombreRol}";
        }

        // Clase auxiliar para el ComboBox de roles
        private class ComboBoxRol
        {
            public int RolID { get; set; }
            public string NombreRol { get; set; }
            public string Descripcion { get; set; }
            public bool EsSistema { get; set; }
            public bool Activo { get; set; }

            // Display SIN etiqueta por defecto
            public string Display => NombreRol;

            // Display CON etiqueta cuando se necesite
            public string DisplayConEtiqueta => NombreRol + (EsSistema ? " (Sistema)" : " (Personalizado)");
        }

        private void ConfigurarEstiloBotonesRoles()
        {
            // Estilo moderno con animación para btnCrearRol
            ConfigurarBotonConAnimacion(btnCrearRol,
                Color.FromArgb(34, 139, 34),
                Color.FromArgb(40, 167, 69),
                "➕");

            // Estilo moderno con animación para btnEliminarRol
            ConfigurarBotonConAnimacion(btnEliminarRol,
                Color.FromArgb(220, 53, 69),
                Color.FromArgb(200, 35, 51),
                "🗑️");

            // Estilo moderno con animación para btnCopiarPermisos
            ConfigurarBotonConAnimacion(btnCopiarPermisos,
                Color.FromArgb(0, 120, 212),
                Color.FromArgb(0, 100, 180),
                "📋");
        }

        private void ConfigurarBotonConAnimacion(Button btn, Color colorBase, Color colorHover, string emoji)
        {
            // Configuración base
            btn.BackColor = colorBase;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Variables para animación
            Timer animTimer = new Timer { Interval = 10 };
            int targetHeight = btn.Height;
            int originalHeight = btn.Height;
            int targetY = btn.Top;
            bool isExpanding = false;

            // Efecto de sombra simulado con borde
            btn.FlatAppearance.BorderColor = Color.Black;

            // MouseEnter - Animación de elevación
            btn.MouseEnter += (s, e) =>
            {
                isExpanding = true;
                targetHeight = originalHeight + 6;
                targetY = btn.Top - 3;
                animTimer.Start();

                // Cambiar color
                btn.BackColor = colorHover;

                // Agregar "sombra"
                btn.FlatAppearance.BorderSize = 3;
                btn.FlatAppearance.BorderColor = Color.FromArgb(50, 0, 0, 0);
            };

            // MouseLeave - Volver a posición original
            btn.MouseLeave += (s, e) =>
            {
                isExpanding = false;
                targetHeight = originalHeight;
                targetY = btn.Top + 3;
                animTimer.Start();

                // Restaurar color
                btn.BackColor = colorBase;

                // Quitar "sombra"
                btn.FlatAppearance.BorderSize = 0;
            };

            // Timer para animación suave
            animTimer.Tick += (s, e) =>
            {
                bool needsUpdate = false;

                if (isExpanding)
                {
                    if (btn.Height < targetHeight)
                    {
                        btn.Height += 1;
                        needsUpdate = true;
                    }
                    if (btn.Top > targetY)
                    {
                        btn.Top -= 1;
                        needsUpdate = true;
                    }
                }
                else
                {
                    if (btn.Height > targetHeight)
                    {
                        btn.Height -= 1;
                        needsUpdate = true;
                    }
                    if (btn.Top < targetY)
                    {
                        btn.Top += 1;
                        needsUpdate = true;
                    }
                }

                if (!needsUpdate)
                {
                    animTimer.Stop();
                }
            };

            // Click - Efecto de pulso
            btn.Click += (s, e) =>
            {
                Timer pulseTimer = new Timer { Interval = 50 };
                int pulseCount = 0;
                Color originalColor = btn.BackColor;

                pulseTimer.Tick += (sender, args) =>
                {
                    pulseCount++;

                    if (pulseCount % 2 == 0)
                        btn.BackColor = ControlPaint.Light(originalColor, 0.3f);
                    else
                        btn.BackColor = originalColor;

                    if (pulseCount >= 4)
                    {
                        btn.BackColor = originalColor;
                        pulseTimer.Stop();
                    }
                };

                pulseTimer.Start();
            };
        }

        private bool VerificarCambiosSinGuardar()
        {
            if (hayCambiosSinGuardar)
            {
                DialogResult resultado = MessageBox.Show(
                    "Tiene cambios sin guardar.\n\n" +
                    "¿Desea continuar sin guardar?\n\n" +
                    "Los cambios se perderán.",
                    "⚠️ Cambios Sin Guardar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.No)
                {
                    return false; // No continuar
                }

                // Usuario decidió continuar, resetear indicador
                hayCambiosSinGuardar = false;
                btnGuardar.Text = "💾 Guardar Cambios";
                btnGuardar.BackColor = Color.FromArgb(34, 139, 34);
            }

            return true; // Continuar
        }

        // Método para detectar cambios entre el estado original y el actual
        private List<CambioPermiso> DetectarCambios()
        {
            List<CambioPermiso> cambios = new List<CambioPermiso>();

            if (dtPermisosOriginal == null || dtPermisos == null)
                return cambios;

            // Recorrer todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (row.Tag == null) continue;

                dynamic tag = row.Tag;
                int formularioID = tag.FormularioID;
                string categoria = tag.Categoria;
                string modulo = tag.Modulo;
                string formulario = row.Cells[2].Value.ToString();

                // Recorrer todas las columnas de acciones (checkboxes)
                for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                {
                    string codigoAccion = dgvPermisos.Columns[i].Name;
                    bool valorActual = row.Cells[i].Value != null && (bool)row.Cells[i].Value;

                    // Buscar valor original en dtPermisosOriginal
                    var rowsOriginales = dtPermisosOriginal.AsEnumerable()
                        .Where(r => r.Field<int>("FormularioID") == formularioID &&
                                   r.Field<string>("CodigoAccion") == codigoAccion)
                        .ToList();

                    if (rowsOriginales.Any())
                    {
                        bool valorOriginal = rowsOriginales.First().Field<bool>("Permitido");

                        // Si hay diferencia, es un cambio
                        if (valorOriginal != valorActual)
                        {
                            cambios.Add(new CambioPermiso
                            {
                                Categoria = categoria,
                                Modulo = modulo,
                                Formulario = formulario,
                                Accion = dgvPermisos.Columns[i].HeaderText,
                                ValorAnterior = valorOriginal,
                                ValorNuevo = valorActual
                            });
                        }
                    }
                }
            }

            return cambios;
        }

        private bool HayCambiosReales()
        {
            if (dtPermisosOriginal == null || dtPermisos == null)
                return false;

            // Recorrer todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                if (row.Tag == null) continue;

                dynamic tag = row.Tag;
                int formularioID = tag.FormularioID;

                // Recorrer todas las columnas de acciones (checkboxes)
                for (int i = 3; i < dgvPermisos.Columns.Count; i++)
                {
                    string codigoAccion = dgvPermisos.Columns[i].Name;
                    bool valorActual = row.Cells[i].Value != null && (bool)row.Cells[i].Value;

                    // Buscar valor original
                    var rowsOriginales = dtPermisosOriginal.AsEnumerable()
                        .Where(r => r.Field<int>("FormularioID") == formularioID &&
                                   r.Field<string>("CodigoAccion") == codigoAccion)
                        .ToList();

                    if (rowsOriginales.Any())
                    {
                        bool valorOriginal = rowsOriginales.First().Field<bool>("Permitido");

                        // Si hay diferencia, hay cambios reales
                        if (valorOriginal != valorActual)
                        {
                            return true;
                        }
                    }
                }
            }

            // Si llegamos aquí, no hay cambios reales
            return false;
        }

        // Clase auxiliar para estado de acciones
        private class EstadoAccion
        {
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public int Activados { get; set; }
            public int Total { get; set; }
            public bool TodosActivados { get; set; }
            public bool NingunoActivado { get; set; }
        }

    }
}