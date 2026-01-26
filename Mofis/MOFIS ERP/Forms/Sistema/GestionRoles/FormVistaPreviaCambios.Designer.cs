namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    partial class FormVistaPreviaCambios
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblContexto = new System.Windows.Forms.Label();
            this.panelLeyenda = new System.Windows.Forms.Panel();
            this.lblResumen = new System.Windows.Forms.Label();
            this.dgvCambios = new System.Windows.Forms.DataGridView();
            this.btnConfirmarGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambios)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Black", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(409, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(340, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "VISTA PREVIA DE CAMBIOS";
            // 
            // lblContexto
            // 
            this.lblContexto.AutoSize = true;
            this.lblContexto.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContexto.Location = new System.Drawing.Point(15, 72);
            this.lblContexto.Name = "lblContexto";
            this.lblContexto.Size = new System.Drawing.Size(366, 25);
            this.lblContexto.TabIndex = 1;
            this.lblContexto.Text = "Cambios pendientes para: [Rol/Usuario]";
            // 
            // panelLeyenda
            // 
            this.panelLeyenda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLeyenda.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelLeyenda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLeyenda.Location = new System.Drawing.Point(20, 100);
            this.panelLeyenda.Name = "panelLeyenda";
            this.panelLeyenda.Size = new System.Drawing.Size(1120, 90);
            this.panelLeyenda.TabIndex = 2;
            // 
            // lblResumen
            // 
            this.lblResumen.AutoSize = true;
            this.lblResumen.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResumen.Location = new System.Drawing.Point(16, 204);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(191, 22);
            this.lblResumen.TabIndex = 3;
            this.lblResumen.Text = "Total de cambios: 0";
            // 
            // dgvCambios
            // 
            this.dgvCambios.AllowUserToAddRows = false;
            this.dgvCambios.AllowUserToDeleteRows = false;
            this.dgvCambios.AllowUserToResizeColumns = false;
            this.dgvCambios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCambios.BackgroundColor = System.Drawing.Color.White;
            this.dgvCambios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCambios.Location = new System.Drawing.Point(20, 238);
            this.dgvCambios.MultiSelect = false;
            this.dgvCambios.Name = "dgvCambios";
            this.dgvCambios.ReadOnly = true;
            this.dgvCambios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCambios.Size = new System.Drawing.Size(1120, 391);
            this.dgvCambios.TabIndex = 4;
            // 
            // btnConfirmarGuardar
            // 
            this.btnConfirmarGuardar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnConfirmarGuardar.FlatAppearance.BorderSize = 0;
            this.btnConfirmarGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmarGuardar.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmarGuardar.ForeColor = System.Drawing.Color.White;
            this.btnConfirmarGuardar.Location = new System.Drawing.Point(20, 649);
            this.btnConfirmarGuardar.Name = "btnConfirmarGuardar";
            this.btnConfirmarGuardar.Size = new System.Drawing.Size(270, 60);
            this.btnConfirmarGuardar.TabIndex = 5;
            this.btnConfirmarGuardar.Text = "✅ Confirmar y Guardar";
            this.btnConfirmarGuardar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(981, 649);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(159, 60);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "❌ Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            // 
            // FormVistaPreviaCambios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(1162, 721);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnConfirmarGuardar);
            this.Controls.Add(this.dgvCambios);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.panelLeyenda);
            this.Controls.Add(this.lblContexto);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "FormVistaPreviaCambios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vista Previa de Cambios - Permisos";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambios)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblContexto;
        private System.Windows.Forms.Panel panelLeyenda;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.DataGridView dgvCambios;
        private System.Windows.Forms.Button btnConfirmarGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}