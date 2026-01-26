namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    partial class FormReportesPermisos
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
            this.panelPrincipal = new System.Windows.Forms.Panel();
            this.dgvAuditoria = new System.Windows.Forms.DataGridView();
            this.panelBotones = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnVolver = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.cmbAccion = new System.Windows.Forms.ComboBox();
            this.lblAccion = new System.Windows.Forms.Label();
            this.cmbUsuario = new System.Windows.Forms.ComboBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblDesde = new System.Windows.Forms.Label();
            this.panelTitulo = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelPrincipal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).BeginInit();
            this.panelBotones.SuspendLayout();
            this.panelFiltros.SuspendLayout();
            this.panelTitulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPrincipal
            // 
            this.panelPrincipal.Controls.Add(this.dgvAuditoria);
            this.panelPrincipal.Controls.Add(this.panelBotones);
            this.panelPrincipal.Controls.Add(this.panelFiltros);
            this.panelPrincipal.Controls.Add(this.panelTitulo);
            this.panelPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPrincipal.Location = new System.Drawing.Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Padding = new System.Windows.Forms.Padding(20);
            this.panelPrincipal.Size = new System.Drawing.Size(1400, 900);
            this.panelPrincipal.TabIndex = 0;
            // 
            // dgvAuditoria
            // 
            this.dgvAuditoria.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAuditoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAuditoria.Location = new System.Drawing.Point(20, 280);
            this.dgvAuditoria.Name = "dgvAuditoria";
            this.dgvAuditoria.ReadOnly = true;
            this.dgvAuditoria.Size = new System.Drawing.Size(1360, 520);
            this.dgvAuditoria.TabIndex = 3;
            // 
            // panelBotones
            // 
            this.panelBotones.Controls.Add(this.lblTotal);
            this.panelBotones.Controls.Add(this.btnVolver);
            this.panelBotones.Controls.Add(this.btnExportar);
            this.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBotones.Location = new System.Drawing.Point(20, 810);
            this.panelBotones.Name = "panelBotones";
            this.panelBotones.Size = new System.Drawing.Size(1360, 70);
            this.panelBotones.TabIndex = 2;
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Gray;
            this.lblTotal.Location = new System.Drawing.Point(10, 25);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(139, 20);
            this.lblTotal.TabIndex = 2;
            this.lblTotal.Text = "Total de registros: 0";
            // 
            // btnVolver
            // 
            this.btnVolver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVolver.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.Location = new System.Drawing.Point(1240, 15);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(110, 40);
            this.btnVolver.TabIndex = 1;
            this.btnVolver.Text = "← Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            // 
            // btnExportar
            // 
            this.btnExportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportar.Location = new System.Drawing.Point(1070, 15);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(160, 40);
            this.btnExportar.TabIndex = 0;
            this.btnExportar.Text = "📊 Exportar Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            // 
            // panelFiltros
            // 
            this.panelFiltros.Controls.Add(this.label1);
            this.panelFiltros.Controls.Add(this.btnLimpiar);
            this.panelFiltros.Controls.Add(this.btnBuscar);
            this.panelFiltros.Controls.Add(this.cmbAccion);
            this.panelFiltros.Controls.Add(this.lblAccion);
            this.panelFiltros.Controls.Add(this.cmbUsuario);
            this.panelFiltros.Controls.Add(this.lblUsuario);
            this.panelFiltros.Controls.Add(this.dtpHasta);
            this.panelFiltros.Controls.Add(this.lblHasta);
            this.panelFiltros.Controls.Add(this.dtpDesde);
            this.panelFiltros.Controls.Add(this.lblDesde);
            this.panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltros.Location = new System.Drawing.Point(20, 120);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Size = new System.Drawing.Size(1360, 140);
            this.panelFiltros.TabIndex = 1;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.Location = new System.Drawing.Point(800, 85);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(120, 35);
            this.btnLimpiar.TabIndex = 9;
            this.btnLimpiar.Text = "🗑️ Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(650, 85);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(130, 35);
            this.btnBuscar.TabIndex = 8;
            this.btnBuscar.Text = "🔍 Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            // 
            // cmbAccion
            // 
            this.cmbAccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAccion.FormattingEnabled = true;
            this.cmbAccion.Location = new System.Drawing.Point(735, 45);
            this.cmbAccion.Name = "cmbAccion";
            this.cmbAccion.Size = new System.Drawing.Size(300, 28);
            this.cmbAccion.TabIndex = 7;
            // 
            // lblAccion
            // 
            this.lblAccion.AutoSize = true;
            this.lblAccion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccion.Location = new System.Drawing.Point(650, 48);
            this.lblAccion.Name = "lblAccion";
            this.lblAccion.Size = new System.Drawing.Size(60, 20);
            this.lblAccion.TabIndex = 6;
            this.lblAccion.Text = "Acción:";
            // 
            // cmbUsuario
            // 
            this.cmbUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUsuario.FormattingEnabled = true;
            this.cmbUsuario.Location = new System.Drawing.Point(120, 45);
            this.cmbUsuario.Name = "cmbUsuario";
            this.cmbUsuario.Size = new System.Drawing.Size(400, 28);
            this.cmbUsuario.TabIndex = 5;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(10, 48);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(67, 20);
            this.lblUsuario.TabIndex = 4;
            this.lblUsuario.Text = "Usuario:";
            // 
            // dtpHasta
            // 
            this.dtpHasta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpHasta.Location = new System.Drawing.Point(470, 10);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(250, 27);
            this.dtpHasta.TabIndex = 3;
            // 
            // lblHasta
            // 
            this.lblHasta.AutoSize = true;
            this.lblHasta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHasta.Location = new System.Drawing.Point(380, 13);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(95, 20);
            this.lblHasta.TabIndex = 2;
            this.lblHasta.Text = "Fecha hasta:";
            // 
            // dtpDesde
            // 
            this.dtpDesde.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDesde.Location = new System.Drawing.Point(120, 10);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(250, 27);
            this.dtpDesde.TabIndex = 1;
            // 
            // lblDesde
            // 
            this.lblDesde.AutoSize = true;
            this.lblDesde.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDesde.Location = new System.Drawing.Point(10, 13);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(98, 20);
            this.lblDesde.TabIndex = 0;
            this.lblDesde.Text = "Fecha desde:";
            // 
            // panelTitulo
            // 
            this.panelTitulo.Controls.Add(this.lblSubtitulo);
            this.panelTitulo.Controls.Add(this.lblTitulo);
            this.panelTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitulo.Location = new System.Drawing.Point(20, 20);
            this.panelTitulo.Name = "panelTitulo";
            this.panelTitulo.Size = new System.Drawing.Size(1360, 100);
            this.panelTitulo.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.Gray;
            this.lblSubtitulo.Location = new System.Drawing.Point(10, 60);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(355, 21);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Historial completo de modificaciones en permisos";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblTitulo.Location = new System.Drawing.Point(10, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(620, 45);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "AUDITORÍA DE CAMBIOS EN PERMISOS";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(188, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "label1xcvxcvcxvxvcxvxc";
            // 
            // FormReportesPermisos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Controls.Add(this.panelPrincipal);
            this.Name = "FormReportesPermisos";
            this.Text = "Auditoría de Permisos";
            this.panelPrincipal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).EndInit();
            this.panelBotones.ResumeLayout(false);
            this.panelBotones.PerformLayout();
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            this.panelTitulo.ResumeLayout(false);
            this.panelTitulo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPrincipal;
        private System.Windows.Forms.Panel panelTitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.ComboBox cmbUsuario;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.ComboBox cmbAccion;
        private System.Windows.Forms.Label lblAccion;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.DataGridView dgvAuditoria;
        private System.Windows.Forms.Panel panelBotones;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label label1;
    }
}
