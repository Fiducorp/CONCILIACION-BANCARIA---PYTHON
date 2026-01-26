namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    partial class FormGenerarReporte
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
            this.gbOpciones = new System.Windows.Forms.GroupBox();
            this.chkIncluirColores = new System.Windows.Forms.CheckBox();
            this.chkIncluirEstadisticas = new System.Windows.Forms.CheckBox();
            this.rbSoloExcepciones = new System.Windows.Forms.RadioButton();
            this.rbSoloMatriz = new System.Windows.Forms.RadioButton();
            this.rbCompleto = new System.Windows.Forms.RadioButton();
            this.gbUbicacion = new System.Windows.Forms.GroupBox();
            this.chkCrearCarpeta = new System.Windows.Forms.CheckBox();
            this.btnExaminar = new System.Windows.Forms.Button();
            this.txtRuta = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.rbRolEspecifico = new System.Windows.Forms.RadioButton();
            this.cmbRolEspecifico = new System.Windows.Forms.ComboBox();
            this.gbOpciones.SuspendLayout();
            this.gbUbicacion.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(133, 13);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(490, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "📊 GENERAR REPORTE DE PERMISOS";
            // 
            // gbOpciones
            // 
            this.gbOpciones.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gbOpciones.Controls.Add(this.cmbRolEspecifico);
            this.gbOpciones.Controls.Add(this.chkIncluirColores);
            this.gbOpciones.Controls.Add(this.chkIncluirEstadisticas);
            this.gbOpciones.Controls.Add(this.rbRolEspecifico);
            this.gbOpciones.Controls.Add(this.rbSoloExcepciones);
            this.gbOpciones.Controls.Add(this.rbSoloMatriz);
            this.gbOpciones.Controls.Add(this.rbCompleto);
            this.gbOpciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbOpciones.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbOpciones.Location = new System.Drawing.Point(28, 98);
            this.gbOpciones.Name = "gbOpciones";
            this.gbOpciones.Size = new System.Drawing.Size(729, 313);
            this.gbOpciones.TabIndex = 1;
            this.gbOpciones.TabStop = false;
            this.gbOpciones.Text = "Opciones de Reporte";
            // 
            // chkIncluirColores
            // 
            this.chkIncluirColores.AutoSize = true;
            this.chkIncluirColores.Checked = true;
            this.chkIncluirColores.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncluirColores.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIncluirColores.Location = new System.Drawing.Point(10, 261);
            this.chkIncluirColores.Name = "chkIncluirColores";
            this.chkIncluirColores.Size = new System.Drawing.Size(258, 29);
            this.chkIncluirColores.TabIndex = 1;
            this.chkIncluirColores.Text = "Incluir formato con colores";
            this.chkIncluirColores.UseVisualStyleBackColor = true;
            // 
            // chkIncluirEstadisticas
            // 
            this.chkIncluirEstadisticas.AutoSize = true;
            this.chkIncluirEstadisticas.Checked = true;
            this.chkIncluirEstadisticas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncluirEstadisticas.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIncluirEstadisticas.Location = new System.Drawing.Point(10, 210);
            this.chkIncluirEstadisticas.Name = "chkIncluirEstadisticas";
            this.chkIncluirEstadisticas.Size = new System.Drawing.Size(277, 29);
            this.chkIncluirEstadisticas.TabIndex = 1;
            this.chkIncluirEstadisticas.Text = "Incluir estadísticas y resumen";
            this.chkIncluirEstadisticas.UseVisualStyleBackColor = true;
            // 
            // rbSoloExcepciones
            // 
            this.rbSoloExcepciones.AutoSize = true;
            this.rbSoloExcepciones.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbSoloExcepciones.Location = new System.Drawing.Point(10, 146);
            this.rbSoloExcepciones.Name = "rbSoloExcepciones";
            this.rbSoloExcepciones.Size = new System.Drawing.Size(253, 25);
            this.rbSoloExcepciones.TabIndex = 0;
            this.rbSoloExcepciones.TabStop = true;
            this.rbSoloExcepciones.Text = "Solo Excepciones de Usuarios";
            this.rbSoloExcepciones.UseVisualStyleBackColor = true;
            // 
            // rbSoloMatriz
            // 
            this.rbSoloMatriz.AutoSize = true;
            this.rbSoloMatriz.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbSoloMatriz.Location = new System.Drawing.Point(10, 99);
            this.rbSoloMatriz.Name = "rbSoloMatriz";
            this.rbSoloMatriz.Size = new System.Drawing.Size(174, 25);
            this.rbSoloMatriz.TabIndex = 0;
            this.rbSoloMatriz.TabStop = true;
            this.rbSoloMatriz.Text = "Solo Matriz por Rol";
            this.rbSoloMatriz.UseVisualStyleBackColor = true;
            // 
            // rbCompleto
            // 
            this.rbCompleto.AutoSize = true;
            this.rbCompleto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbCompleto.Location = new System.Drawing.Point(10, 55);
            this.rbCompleto.Name = "rbCompleto";
            this.rbCompleto.Size = new System.Drawing.Size(295, 25);
            this.rbCompleto.TabIndex = 0;
            this.rbCompleto.TabStop = true;
            this.rbCompleto.Text = "Reporte Completo (todas las hojas)";
            this.rbCompleto.UseVisualStyleBackColor = true;
            // 
            // gbUbicacion
            // 
            this.gbUbicacion.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gbUbicacion.Controls.Add(this.chkCrearCarpeta);
            this.gbUbicacion.Controls.Add(this.btnExaminar);
            this.gbUbicacion.Controls.Add(this.txtRuta);
            this.gbUbicacion.Controls.Add(this.label1);
            this.gbUbicacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbUbicacion.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbUbicacion.Location = new System.Drawing.Point(28, 453);
            this.gbUbicacion.Name = "gbUbicacion";
            this.gbUbicacion.Size = new System.Drawing.Size(729, 138);
            this.gbUbicacion.TabIndex = 2;
            this.gbUbicacion.TabStop = false;
            this.gbUbicacion.Text = "Ubicación del Archivo";
            // 
            // chkCrearCarpeta
            // 
            this.chkCrearCarpeta.AutoSize = true;
            this.chkCrearCarpeta.Checked = true;
            this.chkCrearCarpeta.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCrearCarpeta.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCrearCarpeta.Location = new System.Drawing.Point(110, 93);
            this.chkCrearCarpeta.Name = "chkCrearCarpeta";
            this.chkCrearCarpeta.Size = new System.Drawing.Size(330, 29);
            this.chkCrearCarpeta.TabIndex = 3;
            this.chkCrearCarpeta.Text = "📂 Crear carpeta \'Reportes_MOFIS\'";
            this.chkCrearCarpeta.UseVisualStyleBackColor = true;
            // 
            // btnExaminar
            // 
            this.btnExaminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnExaminar.FlatAppearance.BorderSize = 0;
            this.btnExaminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExaminar.ForeColor = System.Drawing.Color.White;
            this.btnExaminar.Location = new System.Drawing.Point(576, 38);
            this.btnExaminar.Name = "btnExaminar";
            this.btnExaminar.Size = new System.Drawing.Size(132, 40);
            this.btnExaminar.TabIndex = 2;
            this.btnExaminar.Text = "📁 Examinar";
            this.btnExaminar.UseVisualStyleBackColor = false;
            // 
            // txtRuta
            // 
            this.txtRuta.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRuta.Location = new System.Drawing.Point(110, 46);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.Size = new System.Drawing.Size(460, 29);
            this.txtRuta.TabIndex = 1;
            this.txtRuta.Text = "C:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Guardar en:";
            // 
            // btnGenerar
            // 
            this.btnGenerar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGenerar.FlatAppearance.BorderSize = 0;
            this.btnGenerar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerar.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerar.ForeColor = System.Drawing.Color.White;
            this.btnGenerar.Location = new System.Drawing.Point(28, 620);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(175, 55);
            this.btnGenerar.TabIndex = 3;
            this.btnGenerar.Text = "Generar Reporte";
            this.btnGenerar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(582, 620);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(175, 55);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            // 
            // rbRolEspecifico
            // 
            this.rbRolEspecifico.AutoSize = true;
            this.rbRolEspecifico.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbRolEspecifico.Location = new System.Drawing.Point(455, 55);
            this.rbRolEspecifico.Name = "rbRolEspecifico";
            this.rbRolEspecifico.Size = new System.Drawing.Size(243, 25);
            this.rbRolEspecifico.TabIndex = 0;
            this.rbRolEspecifico.TabStop = true;
            this.rbRolEspecifico.Text = "📋 Detalle de Rol Específico";
            this.rbRolEspecifico.UseVisualStyleBackColor = true;
            // 
            // cmbRolEspecifico
            // 
            this.cmbRolEspecifico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRolEspecifico.Enabled = false;
            this.cmbRolEspecifico.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRolEspecifico.FormattingEnabled = true;
            this.cmbRolEspecifico.Location = new System.Drawing.Point(474, 95);
            this.cmbRolEspecifico.Name = "cmbRolEspecifico";
            this.cmbRolEspecifico.Size = new System.Drawing.Size(224, 33);
            this.cmbRolEspecifico.TabIndex = 2;
            // 
            // FormGenerarReporte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(779, 693);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.gbUbicacion);
            this.Controls.Add(this.gbOpciones);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGenerarReporte";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Generar Reporte de Permisos";
            this.gbOpciones.ResumeLayout(false);
            this.gbOpciones.PerformLayout();
            this.gbUbicacion.ResumeLayout(false);
            this.gbUbicacion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbOpciones;
        private System.Windows.Forms.RadioButton rbSoloExcepciones;
        private System.Windows.Forms.RadioButton rbSoloMatriz;
        private System.Windows.Forms.RadioButton rbCompleto;
        private System.Windows.Forms.CheckBox chkIncluirColores;
        private System.Windows.Forms.CheckBox chkIncluirEstadisticas;
        private System.Windows.Forms.GroupBox gbUbicacion;
        private System.Windows.Forms.Button btnExaminar;
        private System.Windows.Forms.TextBox txtRuta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkCrearCarpeta;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ComboBox cmbRolEspecifico;
        private System.Windows.Forms.RadioButton rbRolEspecifico;
    }
}