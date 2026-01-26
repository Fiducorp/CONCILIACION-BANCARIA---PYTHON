namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes.Solicitud_de_pago
{
    partial class FormAgregarProveedor
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
            this.components = new System.ComponentModel.Container();
            this.btnCancelarProveedor = new System.Windows.Forms.Button();
            this.btnGuardarProveedor = new System.Windows.Forms.Button();
            this.cboTipoDocumento = new System.Windows.Forms.ComboBox();
            this.txtNombreProv = new System.Windows.Forms.TextBox();
            this.lblTipoDoc = new System.Windows.Forms.Label();
            this.lblNombreProv = new System.Windows.Forms.Label();
            this.txtNumeroDocumento = new System.Windows.Forms.TextBox();
            this.lblNumeroDoc = new System.Windows.Forms.Label();
            this.txtIdProveedor = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCodigoProveedor = new System.Windows.Forms.Label();
            this.lblTelefonoProv = new System.Windows.Forms.Label();
            this.txtTelefonoProv = new System.Windows.Forms.TextBox();
            this.lblEmailProv = new System.Windows.Forms.Label();
            this.txtEmailProv = new System.Windows.Forms.TextBox();
            this.errProviderProveedor = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnEliminarProveedor = new System.Windows.Forms.Button();
            this.btnBuscarNumero = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderProveedor)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancelarProveedor
            // 
            this.btnCancelarProveedor.BackColor = System.Drawing.Color.Gray;
            this.btnCancelarProveedor.FlatAppearance.BorderSize = 0;
            this.btnCancelarProveedor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelarProveedor.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarProveedor.ForeColor = System.Drawing.Color.White;
            this.btnCancelarProveedor.Location = new System.Drawing.Point(664, 309);
            this.btnCancelarProveedor.Name = "btnCancelarProveedor";
            this.btnCancelarProveedor.Size = new System.Drawing.Size(123, 39);
            this.btnCancelarProveedor.TabIndex = 13;
            this.btnCancelarProveedor.Text = "✖ Cancelar";
            this.btnCancelarProveedor.UseVisualStyleBackColor = false;
            // 
            // btnGuardarProveedor
            // 
            this.btnGuardarProveedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnGuardarProveedor.FlatAppearance.BorderSize = 0;
            this.btnGuardarProveedor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardarProveedor.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarProveedor.ForeColor = System.Drawing.Color.White;
            this.btnGuardarProveedor.Location = new System.Drawing.Point(475, 309);
            this.btnGuardarProveedor.Name = "btnGuardarProveedor";
            this.btnGuardarProveedor.Size = new System.Drawing.Size(153, 39);
            this.btnGuardarProveedor.TabIndex = 14;
            this.btnGuardarProveedor.Text = "💾 Guardar";
            this.btnGuardarProveedor.UseVisualStyleBackColor = false;
            // 
            // cboTipoDocumento
            // 
            this.cboTipoDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoDocumento.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTipoDocumento.FormattingEnabled = true;
            this.cboTipoDocumento.Location = new System.Drawing.Point(187, 193);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(187, 33);
            this.cboTipoDocumento.TabIndex = 12;
            // 
            // txtNombreProv
            // 
            this.txtNombreProv.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreProv.Location = new System.Drawing.Point(187, 138);
            this.txtNombreProv.MaxLength = 200;
            this.txtNombreProv.Name = "txtNombreProv";
            this.txtNombreProv.Size = new System.Drawing.Size(600, 33);
            this.txtNombreProv.TabIndex = 9;
            // 
            // lblTipoDoc
            // 
            this.lblTipoDoc.AutoSize = true;
            this.lblTipoDoc.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoDoc.Location = new System.Drawing.Point(14, 196);
            this.lblTipoDoc.Name = "lblTipoDoc";
            this.lblTipoDoc.Size = new System.Drawing.Size(167, 25);
            this.lblTipoDoc.TabIndex = 4;
            this.lblTipoDoc.Text = "Tipo Documento:";
            // 
            // lblNombreProv
            // 
            this.lblNombreProv.AutoSize = true;
            this.lblNombreProv.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreProv.Location = new System.Drawing.Point(90, 141);
            this.lblNombreProv.Name = "lblNombreProv";
            this.lblNombreProv.Size = new System.Drawing.Size(91, 25);
            this.lblNombreProv.TabIndex = 5;
            this.lblNombreProv.Text = "Nombre:";
            // 
            // txtNumeroDocumento
            // 
            this.txtNumeroDocumento.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumeroDocumento.Location = new System.Drawing.Point(600, 193);
            this.txtNumeroDocumento.MaxLength = 11;
            this.txtNumeroDocumento.Name = "txtNumeroDocumento";
            this.txtNumeroDocumento.Size = new System.Drawing.Size(187, 33);
            this.txtNumeroDocumento.TabIndex = 10;
            // 
            // lblNumeroDoc
            // 
            this.lblNumeroDoc.AutoSize = true;
            this.lblNumeroDoc.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroDoc.Location = new System.Drawing.Point(459, 196);
            this.lblNumeroDoc.Name = "lblNumeroDoc";
            this.lblNumeroDoc.Size = new System.Drawing.Size(135, 25);
            this.lblNumeroDoc.TabIndex = 6;
            this.lblNumeroDoc.Text = "RNC / Cédula:";
            // 
            // txtIdProveedor
            // 
            this.txtIdProveedor.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIdProveedor.Location = new System.Drawing.Point(187, 83);
            this.txtIdProveedor.MaxLength = 20;
            this.txtIdProveedor.Name = "txtIdProveedor";
            this.txtIdProveedor.ReadOnly = true;
            this.txtIdProveedor.Size = new System.Drawing.Size(187, 33);
            this.txtIdProveedor.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.label1.Location = new System.Drawing.Point(317, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 32);
            this.label1.TabIndex = 7;
            this.label1.Text = "AGREGAR PROVEEDOR";
            // 
            // lblCodigoProveedor
            // 
            this.lblCodigoProveedor.AutoSize = true;
            this.lblCodigoProveedor.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodigoProveedor.Location = new System.Drawing.Point(99, 86);
            this.lblCodigoProveedor.Name = "lblCodigoProveedor";
            this.lblCodigoProveedor.Size = new System.Drawing.Size(82, 25);
            this.lblCodigoProveedor.TabIndex = 8;
            this.lblCodigoProveedor.Text = "Código:";
            // 
            // lblTelefonoProv
            // 
            this.lblTelefonoProv.AutoSize = true;
            this.lblTelefonoProv.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTelefonoProv.Location = new System.Drawing.Point(500, 86);
            this.lblTelefonoProv.Name = "lblTelefonoProv";
            this.lblTelefonoProv.Size = new System.Drawing.Size(94, 25);
            this.lblTelefonoProv.TabIndex = 6;
            this.lblTelefonoProv.Text = "Teléfono:";
            // 
            // txtTelefonoProv
            // 
            this.txtTelefonoProv.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTelefonoProv.Location = new System.Drawing.Point(600, 83);
            this.txtTelefonoProv.MaxLength = 10;
            this.txtTelefonoProv.Name = "txtTelefonoProv";
            this.txtTelefonoProv.Size = new System.Drawing.Size(187, 33);
            this.txtTelefonoProv.TabIndex = 10;
            // 
            // lblEmailProv
            // 
            this.lblEmailProv.AutoSize = true;
            this.lblEmailProv.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmailProv.Location = new System.Drawing.Point(117, 254);
            this.lblEmailProv.Name = "lblEmailProv";
            this.lblEmailProv.Size = new System.Drawing.Size(64, 25);
            this.lblEmailProv.TabIndex = 6;
            this.lblEmailProv.Text = "Email:";
            // 
            // txtEmailProv
            // 
            this.txtEmailProv.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmailProv.Location = new System.Drawing.Point(187, 251);
            this.txtEmailProv.MaxLength = 100;
            this.txtEmailProv.Name = "txtEmailProv";
            this.txtEmailProv.Size = new System.Drawing.Size(600, 33);
            this.txtEmailProv.TabIndex = 10;
            // 
            // errProviderProveedor
            // 
            this.errProviderProveedor.ContainerControl = this;
            // 
            // btnEliminarProveedor
            // 
            this.btnEliminarProveedor.BackColor = System.Drawing.Color.Red;
            this.btnEliminarProveedor.FlatAppearance.BorderSize = 0;
            this.btnEliminarProveedor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarProveedor.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarProveedor.ForeColor = System.Drawing.Color.White;
            this.btnEliminarProveedor.Location = new System.Drawing.Point(297, 315);
            this.btnEliminarProveedor.Name = "btnEliminarProveedor";
            this.btnEliminarProveedor.Size = new System.Drawing.Size(96, 33);
            this.btnEliminarProveedor.TabIndex = 15;
            this.btnEliminarProveedor.Text = "Eliminar";
            this.btnEliminarProveedor.UseVisualStyleBackColor = false;
            // 
            // btnBuscarNumero
            // 
            this.btnBuscarNumero.BackColor = System.Drawing.Color.DarkSlateGray;
            this.btnBuscarNumero.FlatAppearance.BorderSize = 0;
            this.btnBuscarNumero.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarNumero.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarNumero.ForeColor = System.Drawing.Color.White;
            this.btnBuscarNumero.Location = new System.Drawing.Point(187, 315);
            this.btnBuscarNumero.Name = "btnBuscarNumero";
            this.btnBuscarNumero.Size = new System.Drawing.Size(83, 33);
            this.btnBuscarNumero.TabIndex = 16;
            this.btnBuscarNumero.Text = "Buscar";
            this.btnBuscarNumero.UseVisualStyleBackColor = false;
            // 
            // FormAgregarProveedor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(811, 360);
            this.Controls.Add(this.btnEliminarProveedor);
            this.Controls.Add(this.btnBuscarNumero);
            this.Controls.Add(this.btnCancelarProveedor);
            this.Controls.Add(this.btnGuardarProveedor);
            this.Controls.Add(this.cboTipoDocumento);
            this.Controls.Add(this.txtNombreProv);
            this.Controls.Add(this.lblTipoDoc);
            this.Controls.Add(this.lblNombreProv);
            this.Controls.Add(this.txtEmailProv);
            this.Controls.Add(this.lblEmailProv);
            this.Controls.Add(this.txtTelefonoProv);
            this.Controls.Add(this.lblTelefonoProv);
            this.Controls.Add(this.txtNumeroDocumento);
            this.Controls.Add(this.lblNumeroDoc);
            this.Controls.Add(this.txtIdProveedor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCodigoProveedor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "FormAgregarProveedor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Agregar Proveedor";
            ((System.ComponentModel.ISupportInitialize)(this.errProviderProveedor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancelarProveedor;
        private System.Windows.Forms.Button btnGuardarProveedor;
        private System.Windows.Forms.ComboBox cboTipoDocumento;
        private System.Windows.Forms.TextBox txtNombreProv;
        private System.Windows.Forms.Label lblTipoDoc;
        private System.Windows.Forms.Label lblNombreProv;
        private System.Windows.Forms.TextBox txtNumeroDocumento;
        private System.Windows.Forms.Label lblNumeroDoc;
        private System.Windows.Forms.TextBox txtIdProveedor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCodigoProveedor;
        private System.Windows.Forms.Label lblTelefonoProv;
        private System.Windows.Forms.TextBox txtTelefonoProv;
        private System.Windows.Forms.Label lblEmailProv;
        private System.Windows.Forms.TextBox txtEmailProv;
        private System.Windows.Forms.ErrorProvider errProviderProveedor;
        private System.Windows.Forms.Button btnEliminarProveedor;
        private System.Windows.Forms.Button btnBuscarNumero;
    }
}