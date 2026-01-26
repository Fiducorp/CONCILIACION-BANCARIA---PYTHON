namespace MOFIS_ERP.Forms.Sistema.GestionUsuarios
{
    partial class FormConfirmarEliminacion
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
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblInstruccion2 = new System.Windows.Forms.Label();
            this.txtConfirmacion = new System.Windows.Forms.TextBox();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblInstrucciones
            // 
            this.lblInstrucciones.AutoSize = true;
            this.lblInstrucciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstrucciones.Location = new System.Drawing.Point(69, 26);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Size = new System.Drawing.Size(354, 24);
            this.lblInstrucciones.TabIndex = 0;
            this.lblInstrucciones.Text = "Para confirmar la eliminación del usuario:";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(429, 26);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(93, 24);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "[Usuario]";
            // 
            // lblInstruccion2
            // 
            this.lblInstruccion2.AutoSize = true;
            this.lblInstruccion2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstruccion2.Location = new System.Drawing.Point(10, 91);
            this.lblInstruccion2.Name = "lblInstruccion2";
            this.lblInstruccion2.Size = new System.Drawing.Size(413, 24);
            this.lblInstruccion2.TabIndex = 2;
            this.lblInstruccion2.Text = "Escriba la palabra CONFIRMAR en mayúsculas:";
            // 
            // txtConfirmacion
            // 
            this.txtConfirmacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfirmacion.Location = new System.Drawing.Point(433, 91);
            this.txtConfirmacion.Name = "txtConfirmacion";
            this.txtConfirmacion.Size = new System.Drawing.Size(312, 29);
            this.txtConfirmacion.TabIndex = 0;
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Enabled = false;
            this.btnConfirmar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmar.Location = new System.Drawing.Point(332, 159);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(255, 45);
            this.btnConfirmar.TabIndex = 1;
            this.btnConfirmar.Text = "✓ Confirmar Eliminación";
            this.btnConfirmar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(607, 159);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(138, 45);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "✗ Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.UseWaitCursor = true;
            // 
            // FormConfirmarEliminacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(757, 216);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.txtConfirmacion);
            this.Controls.Add(this.lblInstruccion2);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblInstrucciones);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfirmarEliminacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Confirmar Eliminación";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstrucciones;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblInstruccion2;
        private System.Windows.Forms.TextBox txtConfirmacion;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCancelar;
    }
}