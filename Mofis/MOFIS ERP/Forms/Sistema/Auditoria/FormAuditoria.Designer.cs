namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    partial class FormAuditoria
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
            this.panelSuperior = new System.Windows.Forms.Panel();
            this.btnVolver = new System.Windows.Forms.Button();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.gbFiltros = new System.Windows.Forms.GroupBox();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.cmbUsuario = new System.Windows.Forms.ComboBox();
            this.cmbAccion = new System.Windows.Forms.ComboBox();
            this.cmbModulo = new System.Windows.Forms.ComboBox();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.lblAccion = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblModulo = new System.Windows.Forms.Label();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.panelBuscador = new System.Windows.Forms.Panel();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.panelEstadisticas = new System.Windows.Forms.Panel();
            this.lblFechaConsulta = new System.Windows.Forms.Label();
            this.lblTotalRegistros = new System.Windows.Forms.Label();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.panelInferior = new System.Windows.Forms.Panel();
            this.btnBuscarDetalles = new System.Windows.Forms.Button();
            this.btnReportePDF = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.dgvAuditoria = new System.Windows.Forms.DataGridView();
            this.panelSuperior.SuspendLayout();
            this.panelFiltros.SuspendLayout();
            this.gbFiltros.SuspendLayout();
            this.panelBuscador.SuspendLayout();
            this.panelEstadisticas.SuspendLayout();
            this.panelGrid.SuspendLayout();
            this.panelInferior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSuperior
            // 
            this.panelSuperior.BackColor = System.Drawing.Color.White;
            this.panelSuperior.Controls.Add(this.btnVolver);
            this.panelSuperior.Controls.Add(this.lblSubtitulo);
            this.panelSuperior.Controls.Add(this.lblTitulo);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(1924, 81);
            this.panelSuperior.TabIndex = 0;
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnVolver.Location = new System.Drawing.Point(1807, 18);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(105, 46);
            this.btnVolver.TabIndex = 2;
            this.btnVolver.Text = "← Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.DimGray;
            this.lblSubtitulo.Location = new System.Drawing.Point(15, 58);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(326, 21);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Historial completo de acciones en MOFIS-ERP";
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Black", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblTitulo.Location = new System.Drawing.Point(12, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(531, 40);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "AUDITORÍA GENERAL DEL SISTEMA";
            // 
            // panelFiltros
            // 
            this.panelFiltros.BackColor = System.Drawing.Color.White;
            this.panelFiltros.Controls.Add(this.gbFiltros);
            this.panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltros.Location = new System.Drawing.Point(0, 81);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Padding = new System.Windows.Forms.Padding(10);
            this.panelFiltros.Size = new System.Drawing.Size(1924, 130);
            this.panelFiltros.TabIndex = 1;
            // 
            // gbFiltros
            // 
            this.gbFiltros.Controls.Add(this.btnLimpiar);
            this.gbFiltros.Controls.Add(this.cmbUsuario);
            this.gbFiltros.Controls.Add(this.cmbAccion);
            this.gbFiltros.Controls.Add(this.cmbModulo);
            this.gbFiltros.Controls.Add(this.dtpHasta);
            this.gbFiltros.Controls.Add(this.lblUsuario);
            this.gbFiltros.Controls.Add(this.lblFechaHasta);
            this.gbFiltros.Controls.Add(this.lblAccion);
            this.gbFiltros.Controls.Add(this.dtpDesde);
            this.gbFiltros.Controls.Add(this.lblModulo);
            this.gbFiltros.Controls.Add(this.lblFechaDesde);
            this.gbFiltros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbFiltros.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFiltros.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.gbFiltros.Location = new System.Drawing.Point(10, 10);
            this.gbFiltros.Name = "gbFiltros";
            this.gbFiltros.Size = new System.Drawing.Size(1904, 110);
            this.gbFiltros.TabIndex = 0;
            this.gbFiltros.TabStop = false;
            this.gbFiltros.Text = "🔍 Filtros de Búsqueda";
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnLimpiar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(1779, 36);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(123, 46);
            this.btnLimpiar.TabIndex = 3;
            this.btnLimpiar.Text = "🗑️ Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            // 
            // cmbUsuario
            // 
            this.cmbUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuario.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUsuario.FormattingEnabled = true;
            this.cmbUsuario.Location = new System.Drawing.Point(962, 73);
            this.cmbUsuario.Name = "cmbUsuario";
            this.cmbUsuario.Size = new System.Drawing.Size(536, 33);
            this.cmbUsuario.TabIndex = 2;
            // 
            // cmbAccion
            // 
            this.cmbAccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccion.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAccion.FormattingEnabled = true;
            this.cmbAccion.Location = new System.Drawing.Point(530, 75);
            this.cmbAccion.Name = "cmbAccion";
            this.cmbAccion.Size = new System.Drawing.Size(300, 33);
            this.cmbAccion.TabIndex = 2;
            // 
            // cmbModulo
            // 
            this.cmbModulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModulo.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbModulo.FormattingEnabled = true;
            this.cmbModulo.Location = new System.Drawing.Point(101, 73);
            this.cmbModulo.Name = "cmbModulo";
            this.cmbModulo.Size = new System.Drawing.Size(301, 33);
            this.cmbModulo.TabIndex = 2;
            // 
            // dtpHasta
            // 
            this.dtpHasta.CalendarFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpHasta.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(530, 32);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.ShowCheckBox = true;
            this.dtpHasta.Size = new System.Drawing.Size(300, 33);
            this.dtpHasta.TabIndex = 1;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.ForeColor = System.Drawing.Color.Black;
            this.lblUsuario.Location = new System.Drawing.Point(870, 76);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(86, 25);
            this.lblUsuario.TabIndex = 0;
            this.lblUsuario.Text = "Usuario:";
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaHasta.ForeColor = System.Drawing.Color.Black;
            this.lblFechaHasta.Location = new System.Drawing.Point(447, 36);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(67, 25);
            this.lblFechaHasta.TabIndex = 0;
            this.lblFechaHasta.Text = "Hasta:";
            // 
            // lblAccion
            // 
            this.lblAccion.AutoSize = true;
            this.lblAccion.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccion.ForeColor = System.Drawing.Color.Black;
            this.lblAccion.Location = new System.Drawing.Point(447, 78);
            this.lblAccion.Name = "lblAccion";
            this.lblAccion.Size = new System.Drawing.Size(77, 25);
            this.lblAccion.TabIndex = 0;
            this.lblAccion.Text = "Acción:";
            // 
            // dtpDesde
            // 
            this.dtpDesde.CalendarFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDesde.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(101, 28);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.ShowCheckBox = true;
            this.dtpDesde.Size = new System.Drawing.Size(301, 33);
            this.dtpDesde.TabIndex = 1;
            // 
            // lblModulo
            // 
            this.lblModulo.AutoSize = true;
            this.lblModulo.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModulo.ForeColor = System.Drawing.Color.Black;
            this.lblModulo.Location = new System.Drawing.Point(9, 76);
            this.lblModulo.Name = "lblModulo";
            this.lblModulo.Size = new System.Drawing.Size(88, 25);
            this.lblModulo.TabIndex = 0;
            this.lblModulo.Text = "Módulo:";
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaDesde.ForeColor = System.Drawing.Color.Black;
            this.lblFechaDesde.Location = new System.Drawing.Point(9, 34);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(71, 25);
            this.lblFechaDesde.TabIndex = 0;
            this.lblFechaDesde.Text = "Desde:";
            // 
            // panelBuscador
            // 
            this.panelBuscador.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panelBuscador.Controls.Add(this.txtBuscar);
            this.panelBuscador.Controls.Add(this.lblBuscar);
            this.panelBuscador.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBuscador.Location = new System.Drawing.Point(0, 211);
            this.panelBuscador.Name = "panelBuscador";
            this.panelBuscador.Size = new System.Drawing.Size(1924, 56);
            this.panelBuscador.TabIndex = 2;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscar.Location = new System.Drawing.Point(111, 11);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(864, 33);
            this.txtBuscar.TabIndex = 1;
            this.txtBuscar.Text = "Buscar en detalle...";
            // 
            // lblBuscar
            // 
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Font = new System.Drawing.Font("Segoe UI Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuscar.ForeColor = System.Drawing.Color.Black;
            this.lblBuscar.Location = new System.Drawing.Point(19, 14);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(80, 25);
            this.lblBuscar.TabIndex = 0;
            this.lblBuscar.Text = "Buscar:";
            // 
            // panelEstadisticas
            // 
            this.panelEstadisticas.Controls.Add(this.lblFechaConsulta);
            this.panelEstadisticas.Controls.Add(this.lblTotalRegistros);
            this.panelEstadisticas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEstadisticas.Location = new System.Drawing.Point(0, 267);
            this.panelEstadisticas.Name = "panelEstadisticas";
            this.panelEstadisticas.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.panelEstadisticas.Size = new System.Drawing.Size(1924, 44);
            this.panelEstadisticas.TabIndex = 3;
            // 
            // lblFechaConsulta
            // 
            this.lblFechaConsulta.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaConsulta.ForeColor = System.Drawing.Color.Gray;
            this.lblFechaConsulta.Location = new System.Drawing.Point(457, 10);
            this.lblFechaConsulta.Name = "lblFechaConsulta";
            this.lblFechaConsulta.Size = new System.Drawing.Size(149, 25);
            this.lblFechaConsulta.TabIndex = 0;
            this.lblFechaConsulta.Text = "Última consulta: --";
            // 
            // lblTotalRegistros
            // 
            this.lblTotalRegistros.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalRegistros.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblTotalRegistros.Location = new System.Drawing.Point(19, 10);
            this.lblTotalRegistros.Name = "lblTotalRegistros";
            this.lblTotalRegistros.Size = new System.Drawing.Size(149, 25);
            this.lblTotalRegistros.TabIndex = 0;
            this.lblTotalRegistros.Text = "Total: 0 registros";
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.panelInferior);
            this.panelGrid.Controls.Add(this.dgvAuditoria);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 311);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(10);
            this.panelGrid.Size = new System.Drawing.Size(1924, 689);
            this.panelGrid.TabIndex = 4;
            // 
            // panelInferior
            // 
            this.panelInferior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panelInferior.Controls.Add(this.btnBuscarDetalles);
            this.panelInferior.Controls.Add(this.btnReportePDF);
            this.panelInferior.Controls.Add(this.btnDashboard);
            this.panelInferior.Controls.Add(this.btnActualizar);
            this.panelInferior.Controls.Add(this.btnExportar);
            this.panelInferior.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInferior.Location = new System.Drawing.Point(10, 609);
            this.panelInferior.Name = "panelInferior";
            this.panelInferior.Padding = new System.Windows.Forms.Padding(10);
            this.panelInferior.Size = new System.Drawing.Size(1904, 70);
            this.panelInferior.TabIndex = 1;
            // 
            // btnBuscarDetalles
            // 
            this.btnBuscarDetalles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnBuscarDetalles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscarDetalles.FlatAppearance.BorderSize = 0;
            this.btnBuscarDetalles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarDetalles.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarDetalles.ForeColor = System.Drawing.Color.White;
            this.btnBuscarDetalles.Location = new System.Drawing.Point(306, 8);
            this.btnBuscarDetalles.Name = "btnBuscarDetalles";
            this.btnBuscarDetalles.Size = new System.Drawing.Size(227, 55);
            this.btnBuscarDetalles.TabIndex = 0;
            this.btnBuscarDetalles.Text = "🔍 Búsqueda Avanzada";
            this.btnBuscarDetalles.UseVisualStyleBackColor = false;
            this.btnBuscarDetalles.Click += new System.EventHandler(this.BtnBuscarDetalles_Click);
            // 
            // btnReportePDF
            // 
            this.btnReportePDF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnReportePDF.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReportePDF.FlatAppearance.BorderSize = 0;
            this.btnReportePDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportePDF.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportePDF.ForeColor = System.Drawing.Color.White;
            this.btnReportePDF.Location = new System.Drawing.Point(926, 8);
            this.btnReportePDF.Name = "btnReportePDF";
            this.btnReportePDF.Size = new System.Drawing.Size(200, 55);
            this.btnReportePDF.TabIndex = 0;
            this.btnReportePDF.Text = "📄 Reporte PDF";
            this.btnReportePDF.UseVisualStyleBackColor = false;
            // 
            // btnDashboard
            // 
            this.btnDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(39)))), ((int)(((byte)(176)))));
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Location = new System.Drawing.Point(14, 8);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(200, 55);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "📈 Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = false;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // btnActualizar
            // 
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnActualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnActualizar.FlatAppearance.BorderSize = 0;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(1221, 8);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(200, 55);
            this.btnActualizar.TabIndex = 0;
            this.btnActualizar.Text = "🔄 Actualizar.";
            this.btnActualizar.UseVisualStyleBackColor = false;
            // 
            // btnExportar
            // 
            this.btnExportar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportar.FlatAppearance.BorderSize = 0;
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(630, 8);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(200, 55);
            this.btnExportar.TabIndex = 0;
            this.btnExportar.Text = "📊 Exportar a Excel";
            this.btnExportar.UseVisualStyleBackColor = false;
            // 
            // dgvAuditoria
            // 
            this.dgvAuditoria.AllowUserToAddRows = false;
            this.dgvAuditoria.AllowUserToDeleteRows = false;
            this.dgvAuditoria.AllowUserToResizeColumns = false;
            this.dgvAuditoria.AllowUserToResizeRows = false;
            this.dgvAuditoria.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAuditoria.BackgroundColor = System.Drawing.Color.White;
            this.dgvAuditoria.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAuditoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuditoria.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAuditoria.EnableHeadersVisualStyles = false;
            this.dgvAuditoria.Location = new System.Drawing.Point(10, 10);
            this.dgvAuditoria.MultiSelect = false;
            this.dgvAuditoria.Name = "dgvAuditoria";
            this.dgvAuditoria.ReadOnly = true;
            this.dgvAuditoria.RowHeadersVisible = false;
            this.dgvAuditoria.RowTemplate.Height = 35;
            this.dgvAuditoria.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAuditoria.Size = new System.Drawing.Size(1904, 669);
            this.dgvAuditoria.TabIndex = 0;
            // 
            // FormAuditoria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1924, 1000);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelEstadisticas);
            this.Controls.Add(this.panelBuscador);
            this.Controls.Add(this.panelFiltros);
            this.Controls.Add(this.panelSuperior);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormAuditoria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auditoría General";
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            this.panelFiltros.ResumeLayout(false);
            this.gbFiltros.ResumeLayout(false);
            this.gbFiltros.PerformLayout();
            this.panelBuscador.ResumeLayout(false);
            this.panelBuscador.PerformLayout();
            this.panelEstadisticas.ResumeLayout(false);
            this.panelGrid.ResumeLayout(false);
            this.panelInferior.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.GroupBox gbFiltros;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.ComboBox cmbModulo;
        private System.Windows.Forms.Label lblModulo;
        private System.Windows.Forms.ComboBox cmbAccion;
        private System.Windows.Forms.Label lblAccion;
        private System.Windows.Forms.ComboBox cmbUsuario;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Panel panelBuscador;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Panel panelEstadisticas;
        private System.Windows.Forms.Label lblTotalRegistros;
        private System.Windows.Forms.Label lblFechaConsulta;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.DataGridView dgvAuditoria;
        private System.Windows.Forms.Panel panelInferior;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnBuscarDetalles;
        private System.Windows.Forms.Button btnReportePDF;
        private System.Windows.Forms.Button btnDashboard;
    }
}