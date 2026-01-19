namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    partial class FormDetalleAuditoria
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.panelContenido = new System.Windows.Forms.Panel();
            this.gbDetalle = new System.Windows.Forms.GroupBox();
            this.txtDetalle = new System.Windows.Forms.TextBox();
            this.gbInformacionTecnica = new System.Windows.Forms.GroupBox();
            this.lblMaquinaValor = new System.Windows.Forms.Label();
            this.lblMaquina = new System.Windows.Forms.Label();
            this.lblIPValor = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.gbUbicacionSistema = new System.Windows.Forms.GroupBox();
            this.lblFormularioValor = new System.Windows.Forms.Label();
            this.lblFormulario = new System.Windows.Forms.Label();
            this.lblModuloValor = new System.Windows.Forms.Label();
            this.lblModulo = new System.Windows.Forms.Label();
            this.lblCategoriaValor = new System.Windows.Forms.Label();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.gbInformacionUsuario = new System.Windows.Forms.GroupBox();
            this.btnVerUsuario = new System.Windows.Forms.Button();
            this.lblUsuarioValor = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.gbInformacionPrincipal = new System.Windows.Forms.GroupBox();
            this.lblAccionValor = new System.Windows.Forms.Label();
            this.lblAccion = new System.Windows.Forms.Label();
            this.lblFechaHoraValor = new System.Windows.Forms.Label();
            this.lblFechaHora = new System.Windows.Forms.Label();
            this.lblAuditoriaIDValor = new System.Windows.Forms.Label();
            this.lblAuditoriaID = new System.Windows.Forms.Label();
            this.panelInferior = new System.Windows.Forms.Panel();
            this.btnCopiarDetalle = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panelSuperior.SuspendLayout();
            this.panelContenido.SuspendLayout();
            this.gbDetalle.SuspendLayout();
            this.gbInformacionTecnica.SuspendLayout();
            this.gbUbicacionSistema.SuspendLayout();
            this.gbInformacionUsuario.SuspendLayout();
            this.gbInformacionPrincipal.SuspendLayout();
            this.panelInferior.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSuperior
            // 
            this.panelSuperior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.panelSuperior.Controls.Add(this.lblTitulo);
            this.panelSuperior.Controls.Add(this.lblSubtitulo);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(930, 80);
            this.panelSuperior.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Black", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(16, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(494, 40);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "📋 DETALLE DE AUDITORÍA";
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.White;
            this.lblSubtitulo.Location = new System.Drawing.Point(70, 49);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(332, 21);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Información completa del registro de auditoría";
            // 
            // panelContenido
            // 
            this.panelContenido.AutoScroll = true;
            this.panelContenido.Controls.Add(this.gbDetalle);
            this.panelContenido.Controls.Add(this.gbInformacionTecnica);
            this.panelContenido.Controls.Add(this.gbUbicacionSistema);
            this.panelContenido.Controls.Add(this.gbInformacionUsuario);
            this.panelContenido.Controls.Add(this.gbInformacionPrincipal);
            this.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenido.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelContenido.Location = new System.Drawing.Point(0, 80);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Padding = new System.Windows.Forms.Padding(20);
            this.panelContenido.Size = new System.Drawing.Size(930, 546);
            this.panelContenido.TabIndex = 1;
            // 
            // gbDetalle
            // 
            this.gbDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.gbDetalle.Controls.Add(this.txtDetalle);
            this.gbDetalle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDetalle.Location = new System.Drawing.Point(23, 369);
            this.gbDetalle.Name = "gbDetalle";
            this.gbDetalle.Size = new System.Drawing.Size(884, 150);
            this.gbDetalle.TabIndex = 4;
            this.gbDetalle.TabStop = false;
            this.gbDetalle.Text = "Detalle Completo";
            // 
            // txtDetalle
            // 
            this.txtDetalle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDetalle.Location = new System.Drawing.Point(15, 25);
            this.txtDetalle.Multiline = true;
            this.txtDetalle.Name = "txtDetalle";
            this.txtDetalle.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetalle.Size = new System.Drawing.Size(860, 119);
            this.txtDetalle.TabIndex = 0;
            // 
            // gbInformacionTecnica
            // 
            this.gbInformacionTecnica.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.gbInformacionTecnica.Controls.Add(this.lblMaquinaValor);
            this.gbInformacionTecnica.Controls.Add(this.lblMaquina);
            this.gbInformacionTecnica.Controls.Add(this.lblIPValor);
            this.gbInformacionTecnica.Controls.Add(this.lblIP);
            this.gbInformacionTecnica.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbInformacionTecnica.Location = new System.Drawing.Point(545, 265);
            this.gbInformacionTecnica.Name = "gbInformacionTecnica";
            this.gbInformacionTecnica.Size = new System.Drawing.Size(362, 88);
            this.gbInformacionTecnica.TabIndex = 3;
            this.gbInformacionTecnica.TabStop = false;
            this.gbInformacionTecnica.Text = "Información Técnica";
            // 
            // lblMaquinaValor
            // 
            this.lblMaquinaValor.AutoSize = true;
            this.lblMaquinaValor.BackColor = System.Drawing.Color.Transparent;
            this.lblMaquinaValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaquinaValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblMaquinaValor.Location = new System.Drawing.Point(188, 56);
            this.lblMaquinaValor.Name = "lblMaquinaValor";
            this.lblMaquinaValor.Size = new System.Drawing.Size(117, 21);
            this.lblMaquinaValor.TabIndex = 3;
            this.lblMaquinaValor.Text = "(No disponible)";
            // 
            // lblMaquina
            // 
            this.lblMaquina.AutoSize = true;
            this.lblMaquina.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaquina.Location = new System.Drawing.Point(15, 56);
            this.lblMaquina.Name = "lblMaquina";
            this.lblMaquina.Size = new System.Drawing.Size(172, 21);
            this.lblMaquina.TabIndex = 2;
            this.lblMaquina.Text = "Nombre de Máquina:";
            // 
            // lblIPValor
            // 
            this.lblIPValor.AutoSize = true;
            this.lblIPValor.BackColor = System.Drawing.Color.Transparent;
            this.lblIPValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIPValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblIPValor.Location = new System.Drawing.Point(127, 29);
            this.lblIPValor.Name = "lblIPValor";
            this.lblIPValor.Size = new System.Drawing.Size(117, 21);
            this.lblIPValor.TabIndex = 1;
            this.lblIPValor.Text = "(No disponible)";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIP.Location = new System.Drawing.Point(15, 30);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(106, 21);
            this.lblIP.TabIndex = 0;
            this.lblIP.Text = "Dirección IP:";
            // 
            // gbUbicacionSistema
            // 
            this.gbUbicacionSistema.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.gbUbicacionSistema.Controls.Add(this.lblFormularioValor);
            this.gbUbicacionSistema.Controls.Add(this.lblFormulario);
            this.gbUbicacionSistema.Controls.Add(this.lblModuloValor);
            this.gbUbicacionSistema.Controls.Add(this.lblModulo);
            this.gbUbicacionSistema.Controls.Add(this.lblCategoriaValor);
            this.gbUbicacionSistema.Controls.Add(this.lblCategoria);
            this.gbUbicacionSistema.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbUbicacionSistema.Location = new System.Drawing.Point(23, 265);
            this.gbUbicacionSistema.Name = "gbUbicacionSistema";
            this.gbUbicacionSistema.Size = new System.Drawing.Size(516, 88);
            this.gbUbicacionSistema.TabIndex = 2;
            this.gbUbicacionSistema.TabStop = false;
            this.gbUbicacionSistema.Text = "Ubicación en el Sistema";
            // 
            // lblFormularioValor
            // 
            this.lblFormularioValor.AutoSize = true;
            this.lblFormularioValor.BackColor = System.Drawing.Color.Transparent;
            this.lblFormularioValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormularioValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblFormularioValor.Location = new System.Drawing.Point(119, 60);
            this.lblFormularioValor.Name = "lblFormularioValor";
            this.lblFormularioValor.Size = new System.Drawing.Size(130, 21);
            this.lblFormularioValor.TabIndex = 5;
            this.lblFormularioValor.Text = "(No especificado)";
            // 
            // lblFormulario
            // 
            this.lblFormulario.AutoSize = true;
            this.lblFormulario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormulario.Location = new System.Drawing.Point(15, 60);
            this.lblFormulario.Name = "lblFormulario";
            this.lblFormulario.Size = new System.Drawing.Size(98, 21);
            this.lblFormulario.TabIndex = 4;
            this.lblFormulario.Text = "Formulario:";
            // 
            // lblModuloValor
            // 
            this.lblModuloValor.AutoSize = true;
            this.lblModuloValor.BackColor = System.Drawing.Color.Transparent;
            this.lblModuloValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModuloValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblModuloValor.Location = new System.Drawing.Point(357, 30);
            this.lblModuloValor.Name = "lblModuloValor";
            this.lblModuloValor.Size = new System.Drawing.Size(130, 21);
            this.lblModuloValor.TabIndex = 3;
            this.lblModuloValor.Text = "(No especificado)";
            // 
            // lblModulo
            // 
            this.lblModulo.AutoSize = true;
            this.lblModulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModulo.Location = new System.Drawing.Point(277, 30);
            this.lblModulo.Name = "lblModulo";
            this.lblModulo.Size = new System.Drawing.Size(74, 21);
            this.lblModulo.TabIndex = 2;
            this.lblModulo.Text = "Módulo:";
            // 
            // lblCategoriaValor
            // 
            this.lblCategoriaValor.AutoSize = true;
            this.lblCategoriaValor.BackColor = System.Drawing.Color.Transparent;
            this.lblCategoriaValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategoriaValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblCategoriaValor.Location = new System.Drawing.Point(109, 30);
            this.lblCategoriaValor.Name = "lblCategoriaValor";
            this.lblCategoriaValor.Size = new System.Drawing.Size(145, 21);
            this.lblCategoriaValor.TabIndex = 1;
            this.lblCategoriaValor.Text = "(No especificado)";
            // 
            // lblCategoria
            // 
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategoria.Location = new System.Drawing.Point(15, 30);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Size = new System.Drawing.Size(88, 21);
            this.lblCategoria.TabIndex = 0;
            this.lblCategoria.Text = "Categoría:";
            // 
            // gbInformacionUsuario
            // 
            this.gbInformacionUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.gbInformacionUsuario.Controls.Add(this.btnVerUsuario);
            this.gbInformacionUsuario.Controls.Add(this.lblUsuarioValor);
            this.gbInformacionUsuario.Controls.Add(this.lblUsuario);
            this.gbInformacionUsuario.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbInformacionUsuario.Location = new System.Drawing.Point(23, 165);
            this.gbInformacionUsuario.Name = "gbInformacionUsuario";
            this.gbInformacionUsuario.Size = new System.Drawing.Size(884, 88);
            this.gbInformacionUsuario.TabIndex = 1;
            this.gbInformacionUsuario.TabStop = false;
            this.gbInformacionUsuario.Text = "Información del Usuario";
            // 
            // btnVerUsuario
            // 
            this.btnVerUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(139)))), ((int)(((byte)(34)))));
            this.btnVerUsuario.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerUsuario.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerUsuario.ForeColor = System.Drawing.Color.White;
            this.btnVerUsuario.Location = new System.Drawing.Point(640, 28);
            this.btnVerUsuario.Name = "btnVerUsuario";
            this.btnVerUsuario.Size = new System.Drawing.Size(235, 50);
            this.btnVerUsuario.TabIndex = 2;
            this.btnVerUsuario.Text = "👤 Ver Info del Usuario";
            this.btnVerUsuario.UseVisualStyleBackColor = false;
            // 
            // lblUsuarioValor
            // 
            this.lblUsuarioValor.AutoSize = true;
            this.lblUsuarioValor.BackColor = System.Drawing.Color.Transparent;
            this.lblUsuarioValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuarioValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblUsuarioValor.Location = new System.Drawing.Point(94, 44);
            this.lblUsuarioValor.Name = "lblUsuarioValor";
            this.lblUsuarioValor.Size = new System.Drawing.Size(130, 21);
            this.lblUsuarioValor.TabIndex = 1;
            this.lblUsuarioValor.Text = "(No disponible)";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(15, 44);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(73, 21);
            this.lblUsuario.TabIndex = 0;
            this.lblUsuario.Text = "Usuario:";
            // 
            // gbInformacionPrincipal
            // 
            this.gbInformacionPrincipal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.gbInformacionPrincipal.Controls.Add(this.lblAccionValor);
            this.gbInformacionPrincipal.Controls.Add(this.lblAccion);
            this.gbInformacionPrincipal.Controls.Add(this.lblFechaHoraValor);
            this.gbInformacionPrincipal.Controls.Add(this.lblFechaHora);
            this.gbInformacionPrincipal.Controls.Add(this.lblAuditoriaIDValor);
            this.gbInformacionPrincipal.Controls.Add(this.lblAuditoriaID);
            this.gbInformacionPrincipal.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbInformacionPrincipal.Location = new System.Drawing.Point(23, 23);
            this.gbInformacionPrincipal.Name = "gbInformacionPrincipal";
            this.gbInformacionPrincipal.Size = new System.Drawing.Size(884, 130);
            this.gbInformacionPrincipal.TabIndex = 0;
            this.gbInformacionPrincipal.TabStop = false;
            this.gbInformacionPrincipal.Text = "Información Principal";
            // 
            // lblAccionValor
            // 
            this.lblAccionValor.AutoSize = true;
            this.lblAccionValor.BackColor = System.Drawing.Color.Transparent;
            this.lblAccionValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccionValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblAccionValor.Location = new System.Drawing.Point(87, 92);
            this.lblAccionValor.Name = "lblAccionValor";
            this.lblAccionValor.Size = new System.Drawing.Size(117, 21);
            this.lblAccionValor.TabIndex = 5;
            this.lblAccionValor.Text = "(No disponible)";
            // 
            // lblAccion
            // 
            this.lblAccion.AutoSize = true;
            this.lblAccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccion.Location = new System.Drawing.Point(15, 92);
            this.lblAccion.Name = "lblAccion";
            this.lblAccion.Size = new System.Drawing.Size(66, 21);
            this.lblAccion.TabIndex = 4;
            this.lblAccion.Text = "Acción:";
            // 
            // lblFechaHoraValor
            // 
            this.lblFechaHoraValor.AutoSize = true;
            this.lblFechaHoraValor.BackColor = System.Drawing.Color.Transparent;
            this.lblFechaHoraValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaHoraValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblFechaHoraValor.Location = new System.Drawing.Point(123, 61);
            this.lblFechaHoraValor.Name = "lblFechaHoraValor";
            this.lblFechaHoraValor.Size = new System.Drawing.Size(117, 21);
            this.lblFechaHoraValor.TabIndex = 3;
            this.lblFechaHoraValor.Text = "(No disponible)";
            // 
            // lblFechaHora
            // 
            this.lblFechaHora.AutoSize = true;
            this.lblFechaHora.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaHora.Location = new System.Drawing.Point(15, 61);
            this.lblFechaHora.Name = "lblFechaHora";
            this.lblFechaHora.Size = new System.Drawing.Size(102, 21);
            this.lblFechaHora.TabIndex = 2;
            this.lblFechaHora.Text = "Fecha/Hora:";
            // 
            // lblAuditoriaIDValor
            // 
            this.lblAuditoriaIDValor.AutoSize = true;
            this.lblAuditoriaIDValor.BackColor = System.Drawing.Color.Transparent;
            this.lblAuditoriaIDValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuditoriaIDValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblAuditoriaIDValor.Location = new System.Drawing.Point(142, 31);
            this.lblAuditoriaIDValor.Name = "lblAuditoriaIDValor";
            this.lblAuditoriaIDValor.Size = new System.Drawing.Size(130, 21);
            this.lblAuditoriaIDValor.TabIndex = 1;
            this.lblAuditoriaIDValor.Text = "(No disponible)";
            // 
            // lblAuditoriaID
            // 
            this.lblAuditoriaID.AutoSize = true;
            this.lblAuditoriaID.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuditoriaID.Location = new System.Drawing.Point(15, 31);
            this.lblAuditoriaID.Name = "lblAuditoriaID";
            this.lblAuditoriaID.Size = new System.Drawing.Size(121, 21);
            this.lblAuditoriaID.TabIndex = 0;
            this.lblAuditoriaID.Text = "ID de Registro:";
            // 
            // panelInferior
            // 
            this.panelInferior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panelInferior.Controls.Add(this.btnCopiarDetalle);
            this.panelInferior.Controls.Add(this.btnCerrar);
            this.panelInferior.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInferior.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelInferior.Location = new System.Drawing.Point(0, 626);
            this.panelInferior.Name = "panelInferior";
            this.panelInferior.Size = new System.Drawing.Size(930, 64);
            this.panelInferior.TabIndex = 2;
            // 
            // btnCopiarDetalle
            // 
            this.btnCopiarDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnCopiarDetalle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCopiarDetalle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiarDetalle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiarDetalle.ForeColor = System.Drawing.Color.White;
            this.btnCopiarDetalle.Location = new System.Drawing.Point(23, 6);
            this.btnCopiarDetalle.Name = "btnCopiarDetalle";
            this.btnCopiarDetalle.Size = new System.Drawing.Size(181, 50);
            this.btnCopiarDetalle.TabIndex = 1;
            this.btnCopiarDetalle.Text = "📋 Copiar Detalle";
            this.btnCopiarDetalle.UseVisualStyleBackColor = false;
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCerrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(737, 6);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(181, 50);
            this.btnCerrar.TabIndex = 0;
            this.btnCerrar.Text = "❌ Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            // 
            // FormDetalleAuditoria
            // 
            this.AcceptButton = this.btnCerrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(930, 690);
            this.Controls.Add(this.panelContenido);
            this.Controls.Add(this.panelInferior);
            this.Controls.Add(this.panelSuperior);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDetalleAuditoria";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Detalle de Auditoría - MOFIS ERP";
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            this.panelContenido.ResumeLayout(false);
            this.gbDetalle.ResumeLayout(false);
            this.gbDetalle.PerformLayout();
            this.gbInformacionTecnica.ResumeLayout(false);
            this.gbInformacionTecnica.PerformLayout();
            this.gbUbicacionSistema.ResumeLayout(false);
            this.gbUbicacionSistema.PerformLayout();
            this.gbInformacionUsuario.ResumeLayout(false);
            this.gbInformacionUsuario.PerformLayout();
            this.gbInformacionPrincipal.ResumeLayout(false);
            this.gbInformacionPrincipal.PerformLayout();
            this.panelInferior.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.GroupBox gbInformacionPrincipal;
        private System.Windows.Forms.Label lblAuditoriaIDValor;
        private System.Windows.Forms.Label lblAuditoriaID;
        private System.Windows.Forms.Label lblFechaHoraValor;
        private System.Windows.Forms.Label lblFechaHora;
        private System.Windows.Forms.Label lblAccionValor;
        private System.Windows.Forms.Label lblAccion;
        private System.Windows.Forms.GroupBox gbInformacionUsuario;
        private System.Windows.Forms.Label lblUsuarioValor;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Button btnVerUsuario;
        private System.Windows.Forms.GroupBox gbUbicacionSistema;
        private System.Windows.Forms.Label lblCategoriaValor;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.Label lblModuloValor;
        private System.Windows.Forms.Label lblModulo;
        private System.Windows.Forms.Label lblFormularioValor;
        private System.Windows.Forms.Label lblFormulario;
        private System.Windows.Forms.GroupBox gbInformacionTecnica;
        private System.Windows.Forms.Label lblIPValor;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblMaquinaValor;
        private System.Windows.Forms.Label lblMaquina;
        private System.Windows.Forms.GroupBox gbDetalle;
        private System.Windows.Forms.TextBox txtDetalle;
        private System.Windows.Forms.Panel panelInferior;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnCopiarDetalle;
    }

}