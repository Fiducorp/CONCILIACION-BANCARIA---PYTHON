namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    partial class FormAdministrarPermisos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splMain = new System.Windows.Forms.SplitContainer();
            this.btnVolver = new System.Windows.Forms.Button();
            this.gbResumen = new System.Windows.Forms.GroupBox();
            this.lblDenegados = new System.Windows.Forms.Label();
            this.lblPermitidos = new System.Windows.Forms.Label();
            this.lblTotalPermisos = new System.Windows.Forms.Label();
            this.lblTituloRoles = new System.Windows.Forms.Label();
            this.tvRoles = new System.Windows.Forms.TreeView();
            this.dgvPermisos = new System.Windows.Forms.DataGridView();
            this.panelBotones = new System.Windows.Forms.Panel();
            this.btnReportes = new System.Windows.Forms.Button();
            this.btnRecargar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.gbAccionesMasivas = new System.Windows.Forms.GroupBox();
            this.gbPorAccion = new System.Windows.Forms.GroupBox();
            this.btnToggleEditar = new System.Windows.Forms.Button();
            this.btnToggleImprimir = new System.Windows.Forms.Button();
            this.btnToggleExportar = new System.Windows.Forms.Button();
            this.btnToggleVer = new System.Windows.Forms.Button();
            this.gbPresets = new System.Windows.Forms.GroupBox();
            this.btnSoloLectura = new System.Windows.Forms.Button();
            this.btnSinAcceso = new System.Windows.Forms.Button();
            this.btnGestionCompleta = new System.Windows.Forms.Button();
            this.btnAccesoTotal = new System.Windows.Forms.Button();
            this.gbPorCategoria = new System.Windows.Forms.GroupBox();
            this.btnSoloConsulta = new System.Windows.Forms.Button();
            this.btnConsultaAvanzada = new System.Windows.Forms.Button();
            this.btnDenegarCategoria = new System.Windows.Forms.Button();
            this.btnAccesoCompleto = new System.Windows.Forms.Button();
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.btnLimpiarFiltros = new System.Windows.Forms.Button();
            this.txtBuscarFormulario = new System.Windows.Forms.TextBox();
            this.cmbModulo = new System.Windows.Forms.ComboBox();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPermisos = new System.Windows.Forms.TabControl();
            this.tabPorRol = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbRolesPersonalizados = new System.Windows.Forms.ComboBox();
            this.btnCopiarPermisos = new System.Windows.Forms.Button();
            this.btnEliminarRol = new System.Windows.Forms.Button();
            this.btnCrearRol = new System.Windows.Forms.Button();
            this.tabPorUsuario = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbUsuarios = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splMain)).BeginInit();
            this.splMain.Panel1.SuspendLayout();
            this.splMain.Panel2.SuspendLayout();
            this.splMain.SuspendLayout();
            this.gbResumen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisos)).BeginInit();
            this.panelBotones.SuspendLayout();
            this.gbAccionesMasivas.SuspendLayout();
            this.gbPorAccion.SuspendLayout();
            this.gbPresets.SuspendLayout();
            this.gbPorCategoria.SuspendLayout();
            this.panelFiltros.SuspendLayout();
            this.tabPermisos.SuspendLayout();
            this.tabPorRol.SuspendLayout();
            this.tabPorUsuario.SuspendLayout();
            this.SuspendLayout();
            // 
            // splMain
            // 
            this.splMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splMain.IsSplitterFixed = true;
            this.splMain.Location = new System.Drawing.Point(0, 0);
            this.splMain.Name = "splMain";
            // 
            // splMain.Panel1
            // 
            this.splMain.Panel1.Controls.Add(this.btnVolver);
            this.splMain.Panel1.Controls.Add(this.gbResumen);
            this.splMain.Panel1.Controls.Add(this.lblTituloRoles);
            this.splMain.Panel1.Controls.Add(this.tvRoles);
            this.splMain.Panel1MinSize = 250;
            // 
            // splMain.Panel2
            // 
            this.splMain.Panel2.Controls.Add(this.dgvPermisos);
            this.splMain.Panel2.Controls.Add(this.panelBotones);
            this.splMain.Panel2.Controls.Add(this.gbAccionesMasivas);
            this.splMain.Panel2.Controls.Add(this.panelFiltros);
            this.splMain.Panel2.Controls.Add(this.tabPermisos);
            this.splMain.Panel2MinSize = 800;
            this.splMain.Size = new System.Drawing.Size(1940, 848);
            this.splMain.SplitterDistance = 300;
            this.splMain.TabIndex = 0;
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.Location = new System.Drawing.Point(3, 791);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(110, 45);
            this.btnVolver.TabIndex = 2;
            this.btnVolver.Text = "← Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            // 
            // gbResumen
            // 
            this.gbResumen.BackColor = System.Drawing.Color.White;
            this.gbResumen.Controls.Add(this.lblDenegados);
            this.gbResumen.Controls.Add(this.lblPermitidos);
            this.gbResumen.Controls.Add(this.lblTotalPermisos);
            this.gbResumen.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbResumen.Location = new System.Drawing.Point(0, 430);
            this.gbResumen.Name = "gbResumen";
            this.gbResumen.Size = new System.Drawing.Size(297, 355);
            this.gbResumen.TabIndex = 1;
            this.gbResumen.TabStop = false;
            this.gbResumen.Text = "Resumen de Permisos";
            // 
            // lblDenegados
            // 
            this.lblDenegados.AutoSize = true;
            this.lblDenegados.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDenegados.ForeColor = System.Drawing.Color.Red;
            this.lblDenegados.Location = new System.Drawing.Point(12, 147);
            this.lblDenegados.Name = "lblDenegados";
            this.lblDenegados.Size = new System.Drawing.Size(145, 24);
            this.lblDenegados.TabIndex = 0;
            this.lblDenegados.Text = "✗ Denegados: 0";
            // 
            // lblPermitidos
            // 
            this.lblPermitidos.AutoSize = true;
            this.lblPermitidos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPermitidos.ForeColor = System.Drawing.Color.Green;
            this.lblPermitidos.Location = new System.Drawing.Point(12, 100);
            this.lblPermitidos.Name = "lblPermitidos";
            this.lblPermitidos.Size = new System.Drawing.Size(134, 24);
            this.lblPermitidos.TabIndex = 0;
            this.lblPermitidos.Text = "✓ Permitidos: 0";
            // 
            // lblTotalPermisos
            // 
            this.lblTotalPermisos.AutoSize = true;
            this.lblTotalPermisos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPermisos.Location = new System.Drawing.Point(12, 49);
            this.lblTotalPermisos.Name = "lblTotalPermisos";
            this.lblTotalPermisos.Size = new System.Drawing.Size(153, 24);
            this.lblTotalPermisos.TabIndex = 0;
            this.lblTotalPermisos.Text = "Total: 0 permisos\n";
            // 
            // lblTituloRoles
            // 
            this.lblTituloRoles.AutoSize = true;
            this.lblTituloRoles.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloRoles.Location = new System.Drawing.Point(43, 9);
            this.lblTituloRoles.Name = "lblTituloRoles";
            this.lblTituloRoles.Size = new System.Drawing.Size(215, 30);
            this.lblTituloRoles.TabIndex = 1;
            this.lblTituloRoles.Text = "ROLES DEL SISTEMA";
            // 
            // tvRoles
            // 
            this.tvRoles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvRoles.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvRoles.FullRowSelect = true;
            this.tvRoles.HideSelection = false;
            this.tvRoles.Location = new System.Drawing.Point(0, 52);
            this.tvRoles.Name = "tvRoles";
            this.tvRoles.ShowLines = false;
            this.tvRoles.ShowRootLines = false;
            this.tvRoles.Size = new System.Drawing.Size(297, 372);
            this.tvRoles.TabIndex = 0;
            // 
            // dgvPermisos
            // 
            this.dgvPermisos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPermisos.Location = new System.Drawing.Point(14, 155);
            this.dgvPermisos.Name = "dgvPermisos";
            this.dgvPermisos.Size = new System.Drawing.Size(1622, 373);
            this.dgvPermisos.TabIndex = 5;
            // 
            // panelBotones
            // 
            this.panelBotones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBotones.BackColor = System.Drawing.Color.White;
            this.panelBotones.Controls.Add(this.btnReportes);
            this.panelBotones.Controls.Add(this.btnRecargar);
            this.panelBotones.Controls.Add(this.btnGuardar);
            this.panelBotones.Location = new System.Drawing.Point(10, 756);
            this.panelBotones.Name = "panelBotones";
            this.panelBotones.Size = new System.Drawing.Size(1626, 80);
            this.panelBotones.TabIndex = 4;
            // 
            // btnReportes
            // 
            this.btnReportes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnReportes.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportes.ForeColor = System.Drawing.Color.White;
            this.btnReportes.Location = new System.Drawing.Point(683, 11);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(235, 60);
            this.btnReportes.TabIndex = 0;
            this.btnReportes.Text = "📊 Generar Reporte";
            this.btnReportes.UseVisualStyleBackColor = false;
            // 
            // btnRecargar
            // 
            this.btnRecargar.BackColor = System.Drawing.Color.Gray;
            this.btnRecargar.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecargar.ForeColor = System.Drawing.Color.White;
            this.btnRecargar.Location = new System.Drawing.Point(972, 11);
            this.btnRecargar.Name = "btnRecargar";
            this.btnRecargar.Size = new System.Drawing.Size(235, 60);
            this.btnRecargar.TabIndex = 0;
            this.btnRecargar.Text = "🔄 Recargar";
            this.btnRecargar.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(400, 11);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(235, 60);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "💾 Guardar Cambios";
            this.btnGuardar.UseVisualStyleBackColor = false;
            // 
            // gbAccionesMasivas
            // 
            this.gbAccionesMasivas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAccionesMasivas.Controls.Add(this.gbPorAccion);
            this.gbAccionesMasivas.Controls.Add(this.gbPresets);
            this.gbAccionesMasivas.Controls.Add(this.gbPorCategoria);
            this.gbAccionesMasivas.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbAccionesMasivas.Location = new System.Drawing.Point(10, 548);
            this.gbAccionesMasivas.Name = "gbAccionesMasivas";
            this.gbAccionesMasivas.Size = new System.Drawing.Size(1842, 218);
            this.gbAccionesMasivas.TabIndex = 3;
            this.gbAccionesMasivas.TabStop = false;
            this.gbAccionesMasivas.Text = "⚡ ACCIONES MASIVAS";
            // 
            // gbPorAccion
            // 
            this.gbPorAccion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gbPorAccion.Controls.Add(this.btnToggleEditar);
            this.gbPorAccion.Controls.Add(this.btnToggleImprimir);
            this.gbPorAccion.Controls.Add(this.btnToggleExportar);
            this.gbPorAccion.Controls.Add(this.btnToggleVer);
            this.gbPorAccion.Location = new System.Drawing.Point(1089, 38);
            this.gbPorAccion.Name = "gbPorAccion";
            this.gbPorAccion.Size = new System.Drawing.Size(537, 165);
            this.gbPorAccion.TabIndex = 0;
            this.gbPorAccion.TabStop = false;
            this.gbPorAccion.Text = "Activar/Desactivar Acciones";
            // 
            // btnToggleEditar
            // 
            this.btnToggleEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleEditar.Location = new System.Drawing.Point(279, 101);
            this.btnToggleEditar.Name = "btnToggleEditar";
            this.btnToggleEditar.Size = new System.Drawing.Size(235, 51);
            this.btnToggleEditar.TabIndex = 2;
            this.btnToggleEditar.Text = "✏️ Activar Editar (0/0)";
            this.btnToggleEditar.UseVisualStyleBackColor = true;
            // 
            // btnToggleImprimir
            // 
            this.btnToggleImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleImprimir.Location = new System.Drawing.Point(18, 37);
            this.btnToggleImprimir.Name = "btnToggleImprimir";
            this.btnToggleImprimir.Size = new System.Drawing.Size(235, 51);
            this.btnToggleImprimir.TabIndex = 1;
            this.btnToggleImprimir.Text = "🖨️ Activar Imprimir (0/0)";
            this.btnToggleImprimir.UseVisualStyleBackColor = true;
            // 
            // btnToggleExportar
            // 
            this.btnToggleExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleExportar.Location = new System.Drawing.Point(279, 37);
            this.btnToggleExportar.Name = "btnToggleExportar";
            this.btnToggleExportar.Size = new System.Drawing.Size(235, 51);
            this.btnToggleExportar.TabIndex = 0;
            this.btnToggleExportar.Text = "📋 Activar Exportar (0/0)";
            this.btnToggleExportar.UseVisualStyleBackColor = true;
            // 
            // btnToggleVer
            // 
            this.btnToggleVer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleVer.Location = new System.Drawing.Point(18, 101);
            this.btnToggleVer.Name = "btnToggleVer";
            this.btnToggleVer.Size = new System.Drawing.Size(235, 51);
            this.btnToggleVer.TabIndex = 0;
            this.btnToggleVer.Text = "👁️ Activar Ver (0/0)";
            this.btnToggleVer.UseVisualStyleBackColor = true;
            // 
            // gbPresets
            // 
            this.gbPresets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gbPresets.Controls.Add(this.btnSoloLectura);
            this.gbPresets.Controls.Add(this.btnSinAcceso);
            this.gbPresets.Controls.Add(this.btnGestionCompleta);
            this.gbPresets.Controls.Add(this.btnAccesoTotal);
            this.gbPresets.Location = new System.Drawing.Point(544, 38);
            this.gbPresets.Name = "gbPresets";
            this.gbPresets.Size = new System.Drawing.Size(466, 165);
            this.gbPresets.TabIndex = 0;
            this.gbPresets.TabStop = false;
            this.gbPresets.Text = "Perfiles Predefinidos";
            // 
            // btnSoloLectura
            // 
            this.btnSoloLectura.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSoloLectura.Location = new System.Drawing.Point(22, 101);
            this.btnSoloLectura.Name = "btnSoloLectura";
            this.btnSoloLectura.Size = new System.Drawing.Size(194, 51);
            this.btnSoloLectura.TabIndex = 2;
            this.btnSoloLectura.Text = "📒 Solo Lectura";
            this.btnSoloLectura.UseVisualStyleBackColor = true;
            // 
            // btnSinAcceso
            // 
            this.btnSinAcceso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSinAcceso.Location = new System.Drawing.Point(247, 101);
            this.btnSinAcceso.Name = "btnSinAcceso";
            this.btnSinAcceso.Size = new System.Drawing.Size(194, 51);
            this.btnSinAcceso.TabIndex = 3;
            this.btnSinAcceso.Text = "🚫 Sin Acceso";
            this.btnSinAcceso.UseVisualStyleBackColor = true;
            // 
            // btnGestionCompleta
            // 
            this.btnGestionCompleta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGestionCompleta.Location = new System.Drawing.Point(246, 37);
            this.btnGestionCompleta.Name = "btnGestionCompleta";
            this.btnGestionCompleta.Size = new System.Drawing.Size(194, 51);
            this.btnGestionCompleta.TabIndex = 1;
            this.btnGestionCompleta.Text = "📝 Gestión Completa";
            this.btnGestionCompleta.UseVisualStyleBackColor = true;
            // 
            // btnAccesoTotal
            // 
            this.btnAccesoTotal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccesoTotal.Location = new System.Drawing.Point(22, 37);
            this.btnAccesoTotal.Name = "btnAccesoTotal";
            this.btnAccesoTotal.Size = new System.Drawing.Size(194, 51);
            this.btnAccesoTotal.TabIndex = 0;
            this.btnAccesoTotal.Text = "👑 Acceso Total";
            this.btnAccesoTotal.UseVisualStyleBackColor = true;
            // 
            // gbPorCategoria
            // 
            this.gbPorCategoria.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gbPorCategoria.Controls.Add(this.btnSoloConsulta);
            this.gbPorCategoria.Controls.Add(this.btnConsultaAvanzada);
            this.gbPorCategoria.Controls.Add(this.btnDenegarCategoria);
            this.gbPorCategoria.Controls.Add(this.btnAccesoCompleto);
            this.gbPorCategoria.Location = new System.Drawing.Point(0, 38);
            this.gbPorCategoria.Name = "gbPorCategoria";
            this.gbPorCategoria.Size = new System.Drawing.Size(469, 165);
            this.gbPorCategoria.TabIndex = 0;
            this.gbPorCategoria.TabStop = false;
            this.gbPorCategoria.Text = "Acciones por Categoría Seleccionada";
            // 
            // btnSoloConsulta
            // 
            this.btnSoloConsulta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSoloConsulta.Location = new System.Drawing.Point(24, 101);
            this.btnSoloConsulta.Name = "btnSoloConsulta";
            this.btnSoloConsulta.Size = new System.Drawing.Size(194, 51);
            this.btnSoloConsulta.TabIndex = 2;
            this.btnSoloConsulta.Text = " 👁️ Solo Consulta";
            this.btnSoloConsulta.UseVisualStyleBackColor = true;
            // 
            // btnConsultaAvanzada
            // 
            this.btnConsultaAvanzada.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConsultaAvanzada.Location = new System.Drawing.Point(247, 101);
            this.btnConsultaAvanzada.Name = "btnConsultaAvanzada";
            this.btnConsultaAvanzada.Size = new System.Drawing.Size(194, 51);
            this.btnConsultaAvanzada.TabIndex = 3;
            this.btnConsultaAvanzada.Text = "📊 Consulta Avanzada";
            this.btnConsultaAvanzada.UseVisualStyleBackColor = true;
            // 
            // btnDenegarCategoria
            // 
            this.btnDenegarCategoria.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDenegarCategoria.Location = new System.Drawing.Point(246, 37);
            this.btnDenegarCategoria.Name = "btnDenegarCategoria";
            this.btnDenegarCategoria.Size = new System.Drawing.Size(194, 51);
            this.btnDenegarCategoria.TabIndex = 1;
            this.btnDenegarCategoria.Text = "🔒 Denegar Todo";
            this.btnDenegarCategoria.UseVisualStyleBackColor = true;
            // 
            // btnAccesoCompleto
            // 
            this.btnAccesoCompleto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccesoCompleto.Location = new System.Drawing.Point(24, 37);
            this.btnAccesoCompleto.Name = "btnAccesoCompleto";
            this.btnAccesoCompleto.Size = new System.Drawing.Size(194, 51);
            this.btnAccesoCompleto.TabIndex = 0;
            this.btnAccesoCompleto.Text = "🔓 Acceso Completo";
            this.btnAccesoCompleto.UseVisualStyleBackColor = true;
            // 
            // panelFiltros
            // 
            this.panelFiltros.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFiltros.BackColor = System.Drawing.Color.White;
            this.panelFiltros.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFiltros.Controls.Add(this.btnLimpiarFiltros);
            this.panelFiltros.Controls.Add(this.txtBuscarFormulario);
            this.panelFiltros.Controls.Add(this.cmbModulo);
            this.panelFiltros.Controls.Add(this.cmbCategoria);
            this.panelFiltros.Controls.Add(this.label1);
            this.panelFiltros.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelFiltros.Location = new System.Drawing.Point(14, 96);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Size = new System.Drawing.Size(1838, 53);
            this.panelFiltros.TabIndex = 1;
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.BackColor = System.Drawing.Color.DimGray;
            this.btnLimpiarFiltros.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiarFiltros.ForeColor = System.Drawing.Color.White;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(1389, 6);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(204, 40);
            this.btnLimpiarFiltros.TabIndex = 3;
            this.btnLimpiarFiltros.Text = "🔄 Limpiar Filtros";
            this.btnLimpiarFiltros.UseVisualStyleBackColor = false;
            // 
            // txtBuscarFormulario
            // 
            this.txtBuscarFormulario.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarFormulario.Location = new System.Drawing.Point(933, 12);
            this.txtBuscarFormulario.Name = "txtBuscarFormulario";
            this.txtBuscarFormulario.Size = new System.Drawing.Size(344, 31);
            this.txtBuscarFormulario.TabIndex = 2;
            this.txtBuscarFormulario.Text = "Buscar formulario...";
            // 
            // cmbModulo
            // 
            this.cmbModulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbModulo.FormattingEnabled = true;
            this.cmbModulo.Location = new System.Drawing.Point(484, 11);
            this.cmbModulo.Name = "cmbModulo";
            this.cmbModulo.Size = new System.Drawing.Size(360, 32);
            this.cmbModulo.TabIndex = 1;
            // 
            // cmbCategoria
            // 
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCategoria.FormattingEnabled = true;
            this.cmbCategoria.Location = new System.Drawing.Point(85, 11);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(360, 32);
            this.cmbCategoria.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filtrar:";
            // 
            // tabPermisos
            // 
            this.tabPermisos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabPermisos.Controls.Add(this.tabPorRol);
            this.tabPermisos.Controls.Add(this.tabPorUsuario);
            this.tabPermisos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPermisos.Location = new System.Drawing.Point(14, 10);
            this.tabPermisos.Name = "tabPermisos";
            this.tabPermisos.SelectedIndex = 0;
            this.tabPermisos.Size = new System.Drawing.Size(1834, 84);
            this.tabPermisos.TabIndex = 0;
            // 
            // tabPorRol
            // 
            this.tabPorRol.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabPorRol.Controls.Add(this.label2);
            this.tabPorRol.Controls.Add(this.cmbRolesPersonalizados);
            this.tabPorRol.Controls.Add(this.btnCopiarPermisos);
            this.tabPorRol.Controls.Add(this.btnEliminarRol);
            this.tabPorRol.Controls.Add(this.btnCrearRol);
            this.tabPorRol.Location = new System.Drawing.Point(4, 29);
            this.tabPorRol.Name = "tabPorRol";
            this.tabPorRol.Padding = new System.Windows.Forms.Padding(3);
            this.tabPorRol.Size = new System.Drawing.Size(1826, 51);
            this.tabPorRol.TabIndex = 0;
            this.tabPorRol.Text = "🔗 Permisos por Rol";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(354, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Roles personalizados:";
            // 
            // cmbRolesPersonalizados
            // 
            this.cmbRolesPersonalizados.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRolesPersonalizados.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRolesPersonalizados.FormattingEnabled = true;
            this.cmbRolesPersonalizados.Location = new System.Drawing.Point(546, 10);
            this.cmbRolesPersonalizados.Name = "cmbRolesPersonalizados";
            this.cmbRolesPersonalizados.Size = new System.Drawing.Size(326, 32);
            this.cmbRolesPersonalizados.TabIndex = 1;
            // 
            // btnCopiarPermisos
            // 
            this.btnCopiarPermisos.FlatAppearance.BorderSize = 0;
            this.btnCopiarPermisos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiarPermisos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiarPermisos.Location = new System.Drawing.Point(930, 8);
            this.btnCopiarPermisos.Name = "btnCopiarPermisos";
            this.btnCopiarPermisos.Size = new System.Drawing.Size(178, 34);
            this.btnCopiarPermisos.TabIndex = 0;
            this.btnCopiarPermisos.Text = "📋 Copiar Permisos";
            this.btnCopiarPermisos.UseVisualStyleBackColor = true;
            // 
            // btnEliminarRol
            // 
            this.btnEliminarRol.FlatAppearance.BorderSize = 0;
            this.btnEliminarRol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarRol.Location = new System.Drawing.Point(169, 9);
            this.btnEliminarRol.Name = "btnEliminarRol";
            this.btnEliminarRol.Size = new System.Drawing.Size(138, 34);
            this.btnEliminarRol.TabIndex = 0;
            this.btnEliminarRol.Text = "🗑️ Eliminar Rol";
            this.btnEliminarRol.UseVisualStyleBackColor = true;
            // 
            // btnCrearRol
            // 
            this.btnCrearRol.FlatAppearance.BorderSize = 0;
            this.btnCrearRol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrearRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrearRol.Location = new System.Drawing.Point(9, 9);
            this.btnCrearRol.Name = "btnCrearRol";
            this.btnCrearRol.Size = new System.Drawing.Size(121, 34);
            this.btnCrearRol.TabIndex = 0;
            this.btnCrearRol.Text = "➕ Crear Rol";
            this.btnCrearRol.UseVisualStyleBackColor = true;
            // 
            // tabPorUsuario
            // 
            this.tabPorUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabPorUsuario.Controls.Add(this.label3);
            this.tabPorUsuario.Controls.Add(this.cmbUsuarios);
            this.tabPorUsuario.Location = new System.Drawing.Point(4, 29);
            this.tabPorUsuario.Name = "tabPorUsuario";
            this.tabPorUsuario.Padding = new System.Windows.Forms.Padding(3);
            this.tabPorUsuario.Size = new System.Drawing.Size(1826, 51);
            this.tabPorUsuario.TabIndex = 1;
            this.tabPorUsuario.Text = "👤 Permisos por Usuario";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Usuarios:";
            // 
            // cmbUsuarios
            // 
            this.cmbUsuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuarios.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUsuarios.FormattingEnabled = true;
            this.cmbUsuarios.Location = new System.Drawing.Point(104, 11);
            this.cmbUsuarios.Name = "cmbUsuarios";
            this.cmbUsuarios.Size = new System.Drawing.Size(505, 32);
            this.cmbUsuarios.TabIndex = 1;
            // 
            // FormAdministrarPermisos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1940, 848);
            this.Controls.Add(this.splMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAdministrarPermisos";
            this.Text = "Administrar Permisos";
            this.splMain.Panel1.ResumeLayout(false);
            this.splMain.Panel1.PerformLayout();
            this.splMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splMain)).EndInit();
            this.splMain.ResumeLayout(false);
            this.gbResumen.ResumeLayout(false);
            this.gbResumen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermisos)).EndInit();
            this.panelBotones.ResumeLayout(false);
            this.gbAccionesMasivas.ResumeLayout(false);
            this.gbPorAccion.ResumeLayout(false);
            this.gbPresets.ResumeLayout(false);
            this.gbPorCategoria.ResumeLayout(false);
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            this.tabPermisos.ResumeLayout(false);
            this.tabPorRol.ResumeLayout(false);
            this.tabPorRol.PerformLayout();
            this.tabPorUsuario.ResumeLayout(false);
            this.tabPorUsuario.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splMain;
        private System.Windows.Forms.Label lblTituloRoles;
        private System.Windows.Forms.TreeView tvRoles;
        private System.Windows.Forms.GroupBox gbResumen;
        private System.Windows.Forms.Label lblTotalPermisos;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label lblDenegados;
        private System.Windows.Forms.Label lblPermitidos;
        private System.Windows.Forms.TabControl tabPermisos;
        private System.Windows.Forms.TabPage tabPorRol;
        private System.Windows.Forms.TabPage tabPorUsuario;
        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBuscarFormulario;
        private System.Windows.Forms.ComboBox cmbModulo;
        private System.Windows.Forms.GroupBox gbAccionesMasivas;
        private System.Windows.Forms.GroupBox gbPorCategoria;
        private System.Windows.Forms.GroupBox gbPresets;
        private System.Windows.Forms.Button btnSoloLectura;
        private System.Windows.Forms.Button btnSinAcceso;
        private System.Windows.Forms.Button btnGestionCompleta;
        private System.Windows.Forms.Button btnAccesoTotal;
        private System.Windows.Forms.Button btnSoloConsulta;
        private System.Windows.Forms.Button btnConsultaAvanzada;
        private System.Windows.Forms.Button btnDenegarCategoria;
        private System.Windows.Forms.Button btnAccesoCompleto;
        private System.Windows.Forms.GroupBox gbPorAccion;
        private System.Windows.Forms.Button btnToggleEditar;
        private System.Windows.Forms.Button btnToggleImprimir;
        private System.Windows.Forms.Button btnToggleVer;
        private System.Windows.Forms.Panel panelBotones;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnRecargar;
        private System.Windows.Forms.ComboBox cmbUsuarios;
        private System.Windows.Forms.Button btnLimpiarFiltros;
        private System.Windows.Forms.DataGridView dgvPermisos;
        private System.Windows.Forms.Button btnToggleExportar;
        private System.Windows.Forms.Button btnCrearRol;
        private System.Windows.Forms.Button btnCopiarPermisos;
        private System.Windows.Forms.Button btnEliminarRol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbRolesPersonalizados;
        private System.Windows.Forms.Button btnReportes;
        private System.Windows.Forms.Label label3;
    }
}