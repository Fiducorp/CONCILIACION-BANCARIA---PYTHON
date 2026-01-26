namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    partial class FormConfiguracionReportePDF
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelSuperior = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelContenido = new System.Windows.Forms.Panel();
            this.gbSeccionesIncluir = new System.Windows.Forms.GroupBox();
            this.chkConclusiones = new System.Windows.Forms.CheckBox();
            this.chkGraficosAnaliticos = new System.Windows.Forms.CheckBox();
            this.chkResumenUsuario = new System.Windows.Forms.CheckBox();
            this.chkResumenModulo = new System.Windows.Forms.CheckBox();
            this.chkDetalleRegistros = new System.Windows.Forms.CheckBox();
            this.chkTop10 = new System.Windows.Forms.CheckBox();
            this.chkEstadisticas = new System.Windows.Forms.CheckBox();
            this.chkCriteriosBusqueda = new System.Windows.Forms.CheckBox();
            this.chkResumenEjecutivo = new System.Windows.Forms.CheckBox();
            this.gbOpcionesDocumento = new System.Windows.Forms.GroupBox();
            this.chkFirmas = new System.Windows.Forms.CheckBox();
            this.chkGraficos = new System.Windows.Forms.CheckBox();
            this.chkIndice = new System.Windows.Forms.CheckBox();
            this.chkPortada = new System.Windows.Forms.CheckBox();
            this.gbFormatoDocumento = new System.Windows.Forms.GroupBox();
            this.cmbTamanoPagina = new System.Windows.Forms.ComboBox();
            this.lblTamanoPagina = new System.Windows.Forms.Label();
            this.rbHorizontal = new System.Windows.Forms.RadioButton();
            this.rbVertical = new System.Windows.Forms.RadioButton();
            this.lblOrientacion = new System.Windows.Forms.Label();
            this.gbTipoReporte = new System.Windows.Forms.GroupBox();
            this.rbComparativo = new System.Windows.Forms.RadioButton();
            this.rbDetallado = new System.Windows.Forms.RadioButton();
            this.rbEjecutivo = new System.Windows.Forms.RadioButton();
            this.rbCompleto = new System.Windows.Forms.RadioButton();
            this.panelInferior = new System.Windows.Forms.Panel();
            this.lblInfoRegistros = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.panelSuperior.SuspendLayout();
            this.panelContenido.SuspendLayout();
            this.gbSeccionesIncluir.SuspendLayout();
            this.gbOpcionesDocumento.SuspendLayout();
            this.gbFormatoDocumento.SuspendLayout();
            this.gbTipoReporte.SuspendLayout();
            this.panelInferior.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSuperior
            // 
            this.panelSuperior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.panelSuperior.Controls.Add(this.lblSubtitulo);
            this.panelSuperior.Controls.Add(this.lblTitulo);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(889, 80);
            this.panelSuperior.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.White;
            this.lblSubtitulo.Location = new System.Drawing.Point(32, 53);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(479, 20);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Configure las opciones del reporte antes de generar el documento PDF";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Black", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(18, 16);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(528, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "📄 CONFIGURACIÓN DE REPORTE PDF";
            // 
            // panelContenido
            // 
            this.panelContenido.AutoScroll = true;
            this.panelContenido.Controls.Add(this.gbSeccionesIncluir);
            this.panelContenido.Controls.Add(this.gbOpcionesDocumento);
            this.panelContenido.Controls.Add(this.gbFormatoDocumento);
            this.panelContenido.Controls.Add(this.gbTipoReporte);
            this.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenido.Location = new System.Drawing.Point(0, 80);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Padding = new System.Windows.Forms.Padding(20);
            this.panelContenido.Size = new System.Drawing.Size(889, 492);
            this.panelContenido.TabIndex = 1;
            // 
            // gbSeccionesIncluir
            // 
            this.gbSeccionesIncluir.Controls.Add(this.chkConclusiones);
            this.gbSeccionesIncluir.Controls.Add(this.chkGraficosAnaliticos);
            this.gbSeccionesIncluir.Controls.Add(this.chkResumenUsuario);
            this.gbSeccionesIncluir.Controls.Add(this.chkResumenModulo);
            this.gbSeccionesIncluir.Controls.Add(this.chkDetalleRegistros);
            this.gbSeccionesIncluir.Controls.Add(this.chkTop10);
            this.gbSeccionesIncluir.Controls.Add(this.chkEstadisticas);
            this.gbSeccionesIncluir.Controls.Add(this.chkCriteriosBusqueda);
            this.gbSeccionesIncluir.Controls.Add(this.chkResumenEjecutivo);
            this.gbSeccionesIncluir.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSeccionesIncluir.Location = new System.Drawing.Point(460, 180);
            this.gbSeccionesIncluir.Name = "gbSeccionesIncluir";
            this.gbSeccionesIncluir.Size = new System.Drawing.Size(400, 280);
            this.gbSeccionesIncluir.TabIndex = 3;
            this.gbSeccionesIncluir.TabStop = false;
            this.gbSeccionesIncluir.Text = "Secciones a Incluir";
            // 
            // chkConclusiones
            // 
            this.chkConclusiones.AutoSize = true;
            this.chkConclusiones.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkConclusiones.Location = new System.Drawing.Point(15, 254);
            this.chkConclusiones.Name = "chkConclusiones";
            this.chkConclusiones.Size = new System.Drawing.Size(285, 25);
            this.chkConclusiones.TabIndex = 8;
            this.chkConclusiones.Text = "💡 Conclusiones y recomendaciones";
            this.chkConclusiones.UseVisualStyleBackColor = true;
            // 
            // chkGraficosAnaliticos
            // 
            this.chkGraficosAnaliticos.AutoSize = true;
            this.chkGraficosAnaliticos.Checked = true;
            this.chkGraficosAnaliticos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGraficosAnaliticos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkGraficosAnaliticos.Location = new System.Drawing.Point(15, 226);
            this.chkGraficosAnaliticos.Name = "chkGraficosAnaliticos";
            this.chkGraficosAnaliticos.Size = new System.Drawing.Size(181, 25);
            this.chkGraficosAnaliticos.TabIndex = 7;
            this.chkGraficosAnaliticos.Text = "📈 Gráficos analíticos";
            this.chkGraficosAnaliticos.UseVisualStyleBackColor = true;
            // 
            // chkResumenUsuario
            // 
            this.chkResumenUsuario.AutoSize = true;
            this.chkResumenUsuario.Checked = true;
            this.chkResumenUsuario.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkResumenUsuario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkResumenUsuario.Location = new System.Drawing.Point(15, 198);
            this.chkResumenUsuario.Name = "chkResumenUsuario";
            this.chkResumenUsuario.Size = new System.Drawing.Size(204, 25);
            this.chkResumenUsuario.TabIndex = 6;
            this.chkResumenUsuario.Text = "👤 Resumen por usuario";
            this.chkResumenUsuario.UseVisualStyleBackColor = true;
            // 
            // chkResumenModulo
            // 
            this.chkResumenModulo.AutoSize = true;
            this.chkResumenModulo.Checked = true;
            this.chkResumenModulo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkResumenModulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkResumenModulo.Location = new System.Drawing.Point(15, 170);
            this.chkResumenModulo.Name = "chkResumenModulo";
            this.chkResumenModulo.Size = new System.Drawing.Size(206, 25);
            this.chkResumenModulo.TabIndex = 5;
            this.chkResumenModulo.Text = "📦 Resumen por módulo";
            this.chkResumenModulo.UseVisualStyleBackColor = true;
            // 
            // chkDetalleRegistros
            // 
            this.chkDetalleRegistros.AutoSize = true;
            this.chkDetalleRegistros.Checked = true;
            this.chkDetalleRegistros.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDetalleRegistros.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDetalleRegistros.Location = new System.Drawing.Point(15, 142);
            this.chkDetalleRegistros.Name = "chkDetalleRegistros";
            this.chkDetalleRegistros.Size = new System.Drawing.Size(305, 25);
            this.chkDetalleRegistros.TabIndex = 4;
            this.chkDetalleRegistros.Text = "📝 Detalle de registros (tabla completa)";
            this.chkDetalleRegistros.UseVisualStyleBackColor = true;
            // 
            // chkTop10
            // 
            this.chkTop10.AutoSize = true;
            this.chkTop10.Checked = true;
            this.chkTop10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTop10.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTop10.Location = new System.Drawing.Point(15, 114);
            this.chkTop10.Name = "chkTop10";
            this.chkTop10.Size = new System.Drawing.Size(314, 25);
            this.chkTop10.TabIndex = 3;
            this.chkTop10.Text = "🏆 Top 10 acciones/usuarios más activos";
            this.chkTop10.UseVisualStyleBackColor = true;
            // 
            // chkEstadisticas
            // 
            this.chkEstadisticas.AutoSize = true;
            this.chkEstadisticas.Checked = true;
            this.chkEstadisticas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEstadisticas.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEstadisticas.Location = new System.Drawing.Point(15, 86);
            this.chkEstadisticas.Name = "chkEstadisticas";
            this.chkEstadisticas.Size = new System.Drawing.Size(205, 25);
            this.chkEstadisticas.TabIndex = 2;
            this.chkEstadisticas.Text = "📊 Estadísticas generales";
            this.chkEstadisticas.UseVisualStyleBackColor = true;
            // 
            // chkCriteriosBusqueda
            // 
            this.chkCriteriosBusqueda.AutoSize = true;
            this.chkCriteriosBusqueda.Checked = true;
            this.chkCriteriosBusqueda.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCriteriosBusqueda.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCriteriosBusqueda.Location = new System.Drawing.Point(15, 58);
            this.chkCriteriosBusqueda.Name = "chkCriteriosBusqueda";
            this.chkCriteriosBusqueda.Size = new System.Drawing.Size(276, 25);
            this.chkCriteriosBusqueda.TabIndex = 1;
            this.chkCriteriosBusqueda.Text = "🔍 Criterios de búsqueda aplicados";
            this.chkCriteriosBusqueda.UseVisualStyleBackColor = true;
            // 
            // chkResumenEjecutivo
            // 
            this.chkResumenEjecutivo.AutoSize = true;
            this.chkResumenEjecutivo.Checked = true;
            this.chkResumenEjecutivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkResumenEjecutivo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkResumenEjecutivo.Location = new System.Drawing.Point(15, 30);
            this.chkResumenEjecutivo.Name = "chkResumenEjecutivo";
            this.chkResumenEjecutivo.Size = new System.Drawing.Size(186, 25);
            this.chkResumenEjecutivo.TabIndex = 0;
            this.chkResumenEjecutivo.Text = "📋 Resumen ejecutivo";
            this.chkResumenEjecutivo.UseVisualStyleBackColor = true;
            // 
            // gbOpcionesDocumento
            // 
            this.gbOpcionesDocumento.Controls.Add(this.chkFirmas);
            this.gbOpcionesDocumento.Controls.Add(this.chkGraficos);
            this.gbOpcionesDocumento.Controls.Add(this.chkIndice);
            this.gbOpcionesDocumento.Controls.Add(this.chkPortada);
            this.gbOpcionesDocumento.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbOpcionesDocumento.Location = new System.Drawing.Point(23, 180);
            this.gbOpcionesDocumento.Name = "gbOpcionesDocumento";
            this.gbOpcionesDocumento.Size = new System.Drawing.Size(420, 140);
            this.gbOpcionesDocumento.TabIndex = 2;
            this.gbOpcionesDocumento.TabStop = false;
            this.gbOpcionesDocumento.Text = "Opciones del Documento";
            // 
            // chkFirmas
            // 
            this.chkFirmas.AutoSize = true;
            this.chkFirmas.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.chkFirmas.Location = new System.Drawing.Point(15, 114);
            this.chkFirmas.Name = "chkFirmas";
            this.chkFirmas.Size = new System.Drawing.Size(244, 25);
            this.chkFirmas.TabIndex = 3;
            this.chkFirmas.Text = "✍️ Incluir espacios para firmas";
            this.chkFirmas.UseVisualStyleBackColor = true;
            // 
            // chkGraficos
            // 
            this.chkGraficos.AutoSize = true;
            this.chkGraficos.Checked = true;
            this.chkGraficos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGraficos.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.chkGraficos.Location = new System.Drawing.Point(15, 86);
            this.chkGraficos.Name = "chkGraficos";
            this.chkGraficos.Size = new System.Drawing.Size(157, 25);
            this.chkGraficos.TabIndex = 2;
            this.chkGraficos.Text = "📊 Incluir gráficos";
            this.chkGraficos.UseVisualStyleBackColor = true;
            // 
            // chkIndice
            // 
            this.chkIndice.AutoSize = true;
            this.chkIndice.Checked = true;
            this.chkIndice.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIndice.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.chkIndice.Location = new System.Drawing.Point(15, 58);
            this.chkIndice.Name = "chkIndice";
            this.chkIndice.Size = new System.Drawing.Size(143, 25);
            this.chkIndice.TabIndex = 1;
            this.chkIndice.Text = "📑 Incluir índice";
            this.chkIndice.UseVisualStyleBackColor = true;
            // 
            // chkPortada
            // 
            this.chkPortada.AutoSize = true;
            this.chkPortada.Checked = true;
            this.chkPortada.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPortada.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.chkPortada.Location = new System.Drawing.Point(15, 30);
            this.chkPortada.Name = "chkPortada";
            this.chkPortada.Size = new System.Drawing.Size(156, 25);
            this.chkPortada.TabIndex = 0;
            this.chkPortada.Text = "✅ Incluir portada";
            this.chkPortada.UseVisualStyleBackColor = true;
            // 
            // gbFormatoDocumento
            // 
            this.gbFormatoDocumento.Controls.Add(this.cmbTamanoPagina);
            this.gbFormatoDocumento.Controls.Add(this.lblTamanoPagina);
            this.gbFormatoDocumento.Controls.Add(this.rbHorizontal);
            this.gbFormatoDocumento.Controls.Add(this.rbVertical);
            this.gbFormatoDocumento.Controls.Add(this.lblOrientacion);
            this.gbFormatoDocumento.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFormatoDocumento.Location = new System.Drawing.Point(460, 23);
            this.gbFormatoDocumento.Name = "gbFormatoDocumento";
            this.gbFormatoDocumento.Size = new System.Drawing.Size(400, 140);
            this.gbFormatoDocumento.TabIndex = 1;
            this.gbFormatoDocumento.TabStop = false;
            this.gbFormatoDocumento.Text = "Formato del Documento";
            // 
            // cmbTamanoPagina
            // 
            this.cmbTamanoPagina.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTamanoPagina.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTamanoPagina.FormattingEnabled = true;
            this.cmbTamanoPagina.Items.AddRange(new object[] {
            "A4 (210 x 297 mm)",
            "Carta (216 x 279 mm)",
            "Legal (216 x 356 mm)"});
            this.cmbTamanoPagina.Location = new System.Drawing.Point(18, 95);
            this.cmbTamanoPagina.Name = "cmbTamanoPagina";
            this.cmbTamanoPagina.Size = new System.Drawing.Size(360, 29);
            this.cmbTamanoPagina.TabIndex = 4;
            // 
            // lblTamanoPagina
            // 
            this.lblTamanoPagina.AutoSize = true;
            this.lblTamanoPagina.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblTamanoPagina.Location = new System.Drawing.Point(15, 70);
            this.lblTamanoPagina.Name = "lblTamanoPagina";
            this.lblTamanoPagina.Size = new System.Drawing.Size(139, 21);
            this.lblTamanoPagina.TabIndex = 3;
            this.lblTamanoPagina.Text = "Tamaño de página:";
            // 
            // rbHorizontal
            // 
            this.rbHorizontal.AutoSize = true;
            this.rbHorizontal.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.rbHorizontal.Location = new System.Drawing.Point(240, 28);
            this.rbHorizontal.Name = "rbHorizontal";
            this.rbHorizontal.Size = new System.Drawing.Size(126, 25);
            this.rbHorizontal.TabIndex = 2;
            this.rbHorizontal.Text = "📃 Horizontal";
            this.rbHorizontal.UseVisualStyleBackColor = true;
            // 
            // rbVertical
            // 
            this.rbVertical.AutoSize = true;
            this.rbVertical.Checked = true;
            this.rbVertical.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.rbVertical.Location = new System.Drawing.Point(120, 28);
            this.rbVertical.Name = "rbVertical";
            this.rbVertical.Size = new System.Drawing.Size(105, 25);
            this.rbVertical.TabIndex = 1;
            this.rbVertical.TabStop = true;
            this.rbVertical.Text = "📄 Vertical";
            this.rbVertical.UseVisualStyleBackColor = true;
            // 
            // lblOrientacion
            // 
            this.lblOrientacion.AutoSize = true;
            this.lblOrientacion.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblOrientacion.Location = new System.Drawing.Point(15, 30);
            this.lblOrientacion.Name = "lblOrientacion";
            this.lblOrientacion.Size = new System.Drawing.Size(94, 21);
            this.lblOrientacion.TabIndex = 0;
            this.lblOrientacion.Text = "Orientación:";
            // 
            // gbTipoReporte
            // 
            this.gbTipoReporte.Controls.Add(this.rbComparativo);
            this.gbTipoReporte.Controls.Add(this.rbDetallado);
            this.gbTipoReporte.Controls.Add(this.rbEjecutivo);
            this.gbTipoReporte.Controls.Add(this.rbCompleto);
            this.gbTipoReporte.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTipoReporte.Location = new System.Drawing.Point(23, 23);
            this.gbTipoReporte.Name = "gbTipoReporte";
            this.gbTipoReporte.Size = new System.Drawing.Size(420, 140);
            this.gbTipoReporte.TabIndex = 0;
            this.gbTipoReporte.TabStop = false;
            this.gbTipoReporte.Text = "Tipo de Reporte";
            // 
            // rbComparativo
            // 
            this.rbComparativo.AutoSize = true;
            this.rbComparativo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.rbComparativo.Location = new System.Drawing.Point(15, 114);
            this.rbComparativo.Name = "rbComparativo";
            this.rbComparativo.Size = new System.Drawing.Size(361, 25);
            this.rbComparativo.TabIndex = 3;
            this.rbComparativo.Text = "Reporte Comparativo (dos periodos lado a lado)";
            this.rbComparativo.UseVisualStyleBackColor = true;
            // 
            // rbDetallado
            // 
            this.rbDetallado.AutoSize = true;
            this.rbDetallado.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.rbDetallado.Location = new System.Drawing.Point(15, 86);
            this.rbDetallado.Name = "rbDetallado";
            this.rbDetallado.Size = new System.Drawing.Size(365, 25);
            this.rbDetallado.TabIndex = 2;
            this.rbDetallado.Text = "Reporte Detallado (listado completo sin gráficos)";
            this.rbDetallado.UseVisualStyleBackColor = true;
            // 
            // rbEjecutivo
            // 
            this.rbEjecutivo.AutoSize = true;
            this.rbEjecutivo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.rbEjecutivo.Location = new System.Drawing.Point(15, 58);
            this.rbEjecutivo.Name = "rbEjecutivo";
            this.rbEjecutivo.Size = new System.Drawing.Size(367, 25);
            this.rbEjecutivo.TabIndex = 1;
            this.rbEjecutivo.Text = "Reporte Ejecutivo (resumen + gráficos analíticos)";
            this.rbEjecutivo.UseVisualStyleBackColor = true;
            // 
            // rbCompleto
            // 
            this.rbCompleto.AutoSize = true;
            this.rbCompleto.Checked = true;
            this.rbCompleto.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.rbCompleto.Location = new System.Drawing.Point(15, 30);
            this.rbCompleto.Name = "rbCompleto";
            this.rbCompleto.Size = new System.Drawing.Size(370, 25);
            this.rbCompleto.TabIndex = 0;
            this.rbCompleto.TabStop = true;
            this.rbCompleto.Text = "Reporte Completo (todo lo filtrado + estadísticas)";
            this.rbCompleto.UseVisualStyleBackColor = true;
            // 
            // panelInferior
            // 
            this.panelInferior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelInferior.Controls.Add(this.lblInfoRegistros);
            this.panelInferior.Controls.Add(this.btnCancelar);
            this.panelInferior.Controls.Add(this.btnGenerar);
            this.panelInferior.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInferior.Location = new System.Drawing.Point(0, 572);
            this.panelInferior.Name = "panelInferior";
            this.panelInferior.Size = new System.Drawing.Size(889, 70);
            this.panelInferior.TabIndex = 2;
            // 
            // lblInfoRegistros
            // 
            this.lblInfoRegistros.AutoSize = true;
            this.lblInfoRegistros.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfoRegistros.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblInfoRegistros.Location = new System.Drawing.Point(20, 25);
            this.lblInfoRegistros.Name = "lblInfoRegistros";
            this.lblInfoRegistros.Size = new System.Drawing.Size(317, 25);
            this.lblInfoRegistros.TabIndex = 2;
            this.lblInfoRegistros.Text = "Se generarán 0 registros en el PDF";
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(540, 13);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(146, 43);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "❌ Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            // 
            // btnGenerar
            // 
            this.btnGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerar.ForeColor = System.Drawing.Color.White;
            this.btnGenerar.Location = new System.Drawing.Point(714, 13);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(146, 43);
            this.btnGenerar.TabIndex = 0;
            this.btnGenerar.Text = "📄 Generar PDF";
            this.btnGenerar.UseVisualStyleBackColor = false;
            // 
            // FormConfiguracionReportePDF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 642);
            this.Controls.Add(this.panelContenido);
            this.Controls.Add(this.panelInferior);
            this.Controls.Add(this.panelSuperior);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfiguracionReportePDF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de Reporte PDF - MOFIS ERP";
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            this.panelContenido.ResumeLayout(false);
            this.gbSeccionesIncluir.ResumeLayout(false);
            this.gbSeccionesIncluir.PerformLayout();
            this.gbOpcionesDocumento.ResumeLayout(false);
            this.gbOpcionesDocumento.PerformLayout();
            this.gbFormatoDocumento.ResumeLayout(false);
            this.gbFormatoDocumento.PerformLayout();
            this.gbTipoReporte.ResumeLayout(false);
            this.gbTipoReporte.PerformLayout();
            this.panelInferior.ResumeLayout(false);
            this.panelInferior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.GroupBox gbSeccionesIncluir;
        private System.Windows.Forms.CheckBox chkConclusiones;
        private System.Windows.Forms.CheckBox chkGraficosAnaliticos;
        private System.Windows.Forms.CheckBox chkResumenUsuario;
        private System.Windows.Forms.CheckBox chkResumenModulo;
        private System.Windows.Forms.CheckBox chkDetalleRegistros;
        private System.Windows.Forms.CheckBox chkTop10;
        private System.Windows.Forms.CheckBox chkEstadisticas;
        private System.Windows.Forms.CheckBox chkCriteriosBusqueda;
        private System.Windows.Forms.CheckBox chkResumenEjecutivo;
        private System.Windows.Forms.GroupBox gbOpcionesDocumento;
        private System.Windows.Forms.CheckBox chkFirmas;
        private System.Windows.Forms.CheckBox chkGraficos;
        private System.Windows.Forms.CheckBox chkIndice;
        private System.Windows.Forms.CheckBox chkPortada;
        private System.Windows.Forms.GroupBox gbFormatoDocumento;
        private System.Windows.Forms.ComboBox cmbTamanoPagina;
        private System.Windows.Forms.Label lblTamanoPagina;
        private System.Windows.Forms.RadioButton rbHorizontal;
        private System.Windows.Forms.RadioButton rbVertical;
        private System.Windows.Forms.Label lblOrientacion;
        private System.Windows.Forms.GroupBox gbTipoReporte;
        private System.Windows.Forms.RadioButton rbComparativo;
        private System.Windows.Forms.RadioButton rbDetallado;
        private System.Windows.Forms.RadioButton rbEjecutivo;
        private System.Windows.Forms.RadioButton rbCompleto;
        private System.Windows.Forms.Panel panelInferior;
        private System.Windows.Forms.Label lblInfoRegistros;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGenerar;
    }
}
