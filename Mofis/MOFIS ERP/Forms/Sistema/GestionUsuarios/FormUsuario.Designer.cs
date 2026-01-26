namespace MOFIS_ERP.Forms.Sistema.GestionUsuarios
{
    partial class FormUsuario
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
            this.Username = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNombreCompleto = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbRol = new System.Windows.Forms.ComboBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.grpPasswordTemporal = new System.Windows.Forms.GroupBox();
            this.lblRequisitos = new System.Windows.Forms.Label();
            this.btnGenerarPassword = new System.Windows.Forms.Button();
            this.txtPasswordTemporal = new System.Windows.Forms.TextBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.grpPasswordTemporal.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(193, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(421, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "\"Nuevo Usuario\" o \"Editar Usuario\"";
            // 
            // Username
            // 
            this.Username.AutoSize = true;
            this.Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Username.Location = new System.Drawing.Point(104, 113);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(87, 24);
            this.Username.TabIndex = 1;
            this.Username.Text = "Usuario:";
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(199, 106);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(467, 31);
            this.txtUsername.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre Completo:";
            // 
            // txtNombreCompleto
            // 
            this.txtNombreCompleto.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreCompleto.Location = new System.Drawing.Point(199, 151);
            this.txtNombreCompleto.Name = "txtNombreCompleto";
            this.txtNombreCompleto.Size = new System.Drawing.Size(467, 31);
            this.txtNombreCompleto.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(123, 210);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Email:";
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmail.Location = new System.Drawing.Point(199, 203);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(467, 31);
            this.txtEmail.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(144, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 24);
            this.label3.TabIndex = 1;
            this.label3.Text = "Rol:";
            // 
            // cmbRol
            // 
            this.cmbRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRol.FormattingEnabled = true;
            this.cmbRol.Items.AddRange(new object[] {
            "ROOT",
            "ADMIN",
            "GERENTE",
            "CONTADOR",
            "ANALISTA",
            "PROBADOR"});
            this.cmbRol.Location = new System.Drawing.Point(199, 260);
            this.cmbRol.Name = "cmbRol";
            this.cmbRol.Size = new System.Drawing.Size(250, 33);
            this.cmbRol.TabIndex = 3;
            // 
            // chkActivo
            // 
            this.chkActivo.AutoSize = true;
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkActivo.Location = new System.Drawing.Point(199, 318);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(181, 28);
            this.chkActivo.TabIndex = 4;
            this.chkActivo.Text = "✓ Usuario Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            // 
            // grpPasswordTemporal
            // 
            this.grpPasswordTemporal.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.grpPasswordTemporal.Controls.Add(this.lblRequisitos);
            this.grpPasswordTemporal.Controls.Add(this.btnGenerarPassword);
            this.grpPasswordTemporal.Controls.Add(this.txtPasswordTemporal);
            this.grpPasswordTemporal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPasswordTemporal.Location = new System.Drawing.Point(199, 367);
            this.grpPasswordTemporal.Name = "grpPasswordTemporal";
            this.grpPasswordTemporal.Size = new System.Drawing.Size(344, 202);
            this.grpPasswordTemporal.TabIndex = 5;
            this.grpPasswordTemporal.TabStop = false;
            this.grpPasswordTemporal.Text = "Contraseña Temporal";
            // 
            // lblRequisitos
            // 
            this.lblRequisitos.AutoSize = true;
            this.lblRequisitos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRequisitos.ForeColor = System.Drawing.Color.Gray;
            this.lblRequisitos.Location = new System.Drawing.Point(6, 150);
            this.lblRequisitos.Name = "lblRequisitos";
            this.lblRequisitos.Size = new System.Drawing.Size(332, 40);
            this.lblRequisitos.TabIndex = 2;
            this.lblRequisitos.Text = "Se generará automáticamente. \r\nEl usuario deberá cambiarla en el primer login.";
            // 
            // btnGenerarPassword
            // 
            this.btnGenerarPassword.Location = new System.Drawing.Point(115, 89);
            this.btnGenerarPassword.Name = "btnGenerarPassword";
            this.btnGenerarPassword.Size = new System.Drawing.Size(113, 42);
            this.btnGenerarPassword.TabIndex = 1;
            this.btnGenerarPassword.Text = "🔄 Asignar";
            this.btnGenerarPassword.UseVisualStyleBackColor = true;
            // 
            // txtPasswordTemporal
            // 
            this.txtPasswordTemporal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPasswordTemporal.Location = new System.Drawing.Point(22, 38);
            this.txtPasswordTemporal.Name = "txtPasswordTemporal";
            this.txtPasswordTemporal.ReadOnly = true;
            this.txtPasswordTemporal.Size = new System.Drawing.Size(306, 31);
            this.txtPasswordTemporal.TabIndex = 0;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Location = new System.Drawing.Point(199, 602);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(136, 50);
            this.btnGuardar.TabIndex = 4;
            this.btnGuardar.Text = "💾 Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(364, 602);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(136, 50);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "❌ Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // FormUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(685, 657);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.grpPasswordTemporal);
            this.Controls.Add(this.chkActivo);
            this.Controls.Add(this.cmbRol);
            this.Controls.Add(this.txtNombreCompleto);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Nuevo Usuario / Editar Usuario";
            this.grpPasswordTemporal.ResumeLayout(false);
            this.grpPasswordTemporal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label Username;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNombreCompleto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbRol;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.GroupBox grpPasswordTemporal;
        private System.Windows.Forms.Button btnGenerarPassword;
        private System.Windows.Forms.TextBox txtPasswordTemporal;
        private System.Windows.Forms.Label lblRequisitos;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}