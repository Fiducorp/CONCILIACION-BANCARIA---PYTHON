namespace MOFIS_ERP.Forms.Sistema.GestionUsuarios
{
    partial class FormResetPassword
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.grpPassword = new System.Windows.Forms.GroupBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.chkForzarCambio = new System.Windows.Forms.CheckBox();
            this.btnGenerarPassword = new System.Windows.Forms.Button();
            this.txtPasswordTemporal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnResetear = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.grpPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(170, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(280, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Resetear Contraseña";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Resetear contraseña de:";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(299, 97);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(189, 24);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Text = "[Nombre del Usuario]";
            // 
            // grpPassword
            // 
            this.grpPassword.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.grpPassword.Controls.Add(this.lblInfo);
            this.grpPassword.Controls.Add(this.chkForzarCambio);
            this.grpPassword.Controls.Add(this.btnGenerarPassword);
            this.grpPassword.Controls.Add(this.txtPasswordTemporal);
            this.grpPassword.Controls.Add(this.label2);
            this.grpPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPassword.Location = new System.Drawing.Point(81, 174);
            this.grpPassword.Name = "grpPassword";
            this.grpPassword.Size = new System.Drawing.Size(475, 294);
            this.grpPassword.TabIndex = 3;
            this.grpPassword.TabStop = false;
            this.grpPassword.Text = "Nueva Contraseña Temporal";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblInfo.Location = new System.Drawing.Point(47, 229);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(388, 40);
            this.lblInfo.TabIndex = 4;
            this.lblInfo.Text = "El usuario recibirá esta contraseña temporal y deberá \r\ncambiarla al iniciar sesi" +
    "ón.";
            // 
            // chkForzarCambio
            // 
            this.chkForzarCambio.AutoSize = true;
            this.chkForzarCambio.Checked = true;
            this.chkForzarCambio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkForzarCambio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkForzarCambio.Location = new System.Drawing.Point(27, 188);
            this.chkForzarCambio.Name = "chkForzarCambio";
            this.chkForzarCambio.Size = new System.Drawing.Size(366, 24);
            this.chkForzarCambio.TabIndex = 3;
            this.chkForzarCambio.Text = "✓ Forzar cambio de contraseña en próximo login";
            this.chkForzarCambio.UseVisualStyleBackColor = true;
            // 
            // btnGenerarPassword
            // 
            this.btnGenerarPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerarPassword.Location = new System.Drawing.Point(217, 118);
            this.btnGenerarPassword.Name = "btnGenerarPassword";
            this.btnGenerarPassword.Size = new System.Drawing.Size(109, 45);
            this.btnGenerarPassword.TabIndex = 2;
            this.btnGenerarPassword.Text = "🔄 Nueva";
            this.btnGenerarPassword.UseVisualStyleBackColor = true;
            // 
            // txtPasswordTemporal
            // 
            this.txtPasswordTemporal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPasswordTemporal.Location = new System.Drawing.Point(217, 62);
            this.txtPasswordTemporal.Name = "txtPasswordTemporal";
            this.txtPasswordTemporal.ReadOnly = true;
            this.txtPasswordTemporal.Size = new System.Drawing.Size(218, 31);
            this.txtPasswordTemporal.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Contraseña generada:";
            // 
            // btnResetear
            // 
            this.btnResetear.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetear.Location = new System.Drawing.Point(81, 493);
            this.btnResetear.Name = "btnResetear";
            this.btnResetear.Size = new System.Drawing.Size(132, 53);
            this.btnResetear.TabIndex = 4;
            this.btnResetear.Text = "🔒 Resetear";
            this.btnResetear.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(424, 493);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(132, 53);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "❌ Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // FormResetPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(626, 558);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnResetear);
            this.Controls.Add(this.grpPassword);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormResetPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Resetear Contraseña";
            this.grpPassword.ResumeLayout(false);
            this.grpPassword.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.GroupBox grpPassword;
        private System.Windows.Forms.Button btnGenerarPassword;
        private System.Windows.Forms.TextBox txtPasswordTemporal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.CheckBox chkForzarCambio;
        private System.Windows.Forms.Button btnResetear;
        private System.Windows.Forms.Button btnCancelar;
    }
}