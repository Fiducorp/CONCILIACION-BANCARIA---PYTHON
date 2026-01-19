namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    partial class FormCopiarPermisos
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
            this.gbOrigen = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRolOrigen = new System.Windows.Forms.ComboBox();
            this.gbDestino = new System.Windows.Forms.GroupBox();
            this.cmbRolDestino = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkSobrescribir = new System.Windows.Forms.CheckBox();
            this.btnCopiar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.gbOrigen.SuspendLayout();
            this.gbDestino.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Black", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(29, 19);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(453, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "COPIAR PERMISOS ENTRE ROLES";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbOrigen
            // 
            this.gbOrigen.Controls.Add(this.cmbRolOrigen);
            this.gbOrigen.Controls.Add(this.label1);
            this.gbOrigen.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbOrigen.Location = new System.Drawing.Point(36, 98);
            this.gbOrigen.Name = "gbOrigen";
            this.gbOrigen.Size = new System.Drawing.Size(450, 146);
            this.gbOrigen.TabIndex = 1;
            this.gbOrigen.TabStop = false;
            this.gbOrigen.Text = "Rol Origen (Copiar desde)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(408, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seleccione el rol desde el cual copiar los permisos";
            // 
            // cmbRolOrigen
            // 
            this.cmbRolOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRolOrigen.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRolOrigen.FormattingEnabled = true;
            this.cmbRolOrigen.Location = new System.Drawing.Point(79, 90);
            this.cmbRolOrigen.Name = "cmbRolOrigen";
            this.cmbRolOrigen.Size = new System.Drawing.Size(304, 33);
            this.cmbRolOrigen.TabIndex = 1;
            // 
            // gbDestino
            // 
            this.gbDestino.Controls.Add(this.cmbRolDestino);
            this.gbDestino.Controls.Add(this.label2);
            this.gbDestino.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDestino.Location = new System.Drawing.Point(36, 284);
            this.gbDestino.Name = "gbDestino";
            this.gbDestino.Size = new System.Drawing.Size(450, 146);
            this.gbDestino.TabIndex = 1;
            this.gbDestino.TabStop = false;
            this.gbDestino.Text = "Rol Destino (Copiar hacia)";
            // 
            // cmbRolDestino
            // 
            this.cmbRolDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRolDestino.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRolDestino.FormattingEnabled = true;
            this.cmbRolDestino.Location = new System.Drawing.Point(79, 90);
            this.cmbRolDestino.Name = "cmbRolDestino";
            this.cmbRolDestino.Size = new System.Drawing.Size(304, 33);
            this.cmbRolDestino.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(348, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "Seleccione el rol que recibirá los permisos:";
            // 
            // chkSobrescribir
            // 
            this.chkSobrescribir.AutoSize = true;
            this.chkSobrescribir.Checked = true;
            this.chkSobrescribir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSobrescribir.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSobrescribir.Location = new System.Drawing.Point(36, 464);
            this.chkSobrescribir.Name = "chkSobrescribir";
            this.chkSobrescribir.Size = new System.Drawing.Size(448, 27);
            this.chkSobrescribir.TabIndex = 2;
            this.chkSobrescribir.Text = "⚠️ Sobrescribir permisos existentes en el rol destino";
            this.chkSobrescribir.UseVisualStyleBackColor = true;
            // 
            // btnCopiar
            // 
            this.btnCopiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnCopiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiar.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiar.ForeColor = System.Drawing.Color.White;
            this.btnCopiar.Location = new System.Drawing.Point(36, 530);
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.Size = new System.Drawing.Size(210, 50);
            this.btnCopiar.TabIndex = 3;
            this.btnCopiar.Text = "📋 Copiar Permisos";
            this.btnCopiar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(276, 530);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(210, 50);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "❌ Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            // 
            // FormCopiarPermisos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 598);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCopiar);
            this.Controls.Add(this.chkSobrescribir);
            this.Controls.Add(this.gbDestino);
            this.Controls.Add(this.gbOrigen);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCopiarPermisos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copiar Permisos Entre Roles";
            this.gbOrigen.ResumeLayout(false);
            this.gbOrigen.PerformLayout();
            this.gbDestino.ResumeLayout(false);
            this.gbDestino.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbOrigen;
        private System.Windows.Forms.ComboBox cmbRolOrigen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbDestino;
        private System.Windows.Forms.ComboBox cmbRolDestino;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSobrescribir;
        private System.Windows.Forms.Button btnCopiar;
        private System.Windows.Forms.Button btnCancelar;
    }
}