namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes.Solicitud_de_pago
{
    partial class FormConfigNota
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

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.lblInfoModo = new System.Windows.Forms.Label();
            this.chkAfectarSubtotal = new System.Windows.Forms.CheckBox();
            this.chkMostrarDetalle = new System.Windows.Forms.CheckBox();
            this.groupBoxResultados = new System.Windows.Forms.GroupBox();
            this.lblTotalNota = new System.Windows.Forms.Label();
            this.lblMontoITBIS = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numPorcentajeITBIS = new System.Windows.Forms.NumericUpDown();
            this.chkAplicaITBIS = new System.Windows.Forms.CheckBox();
            this.txtSubtotal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlContenido.SuspendLayout();
            this.groupBoxResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPorcentajeITBIS)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(434, 50);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "NOTA DE CRÉDITO";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlContenido
            // 
            this.pnlContenido.BackColor = System.Drawing.Color.White;
            this.pnlContenido.Controls.Add(this.chkMostrarDetalle);
            this.pnlContenido.Controls.Add(this.lblInfoModo);
            this.pnlContenido.Controls.Add(this.chkAfectarSubtotal);
            this.pnlContenido.Controls.Add(this.groupBoxResultados);
            this.pnlContenido.Controls.Add(this.txtDescripcion);
            this.pnlContenido.Controls.Add(this.label3);
            this.pnlContenido.Controls.Add(this.numPorcentajeITBIS);
            this.pnlContenido.Controls.Add(this.chkAplicaITBIS);
            this.pnlContenido.Controls.Add(this.txtSubtotal);
            this.pnlContenido.Controls.Add(this.label1);
            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlContenido.Location = new System.Drawing.Point(0, 50);
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Padding = new System.Windows.Forms.Padding(15);
            this.pnlContenido.Size = new System.Drawing.Size(434, 420);
            this.pnlContenido.TabIndex = 1;
            // 
            // lblInfoModo
            // 
            this.lblInfoModo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblInfoModo.ForeColor = System.Drawing.Color.DimGray;
            this.lblInfoModo.Location = new System.Drawing.Point(34, 375);
            this.lblInfoModo.Name = "lblInfoModo";
            this.lblInfoModo.Size = new System.Drawing.Size(370, 40);
            this.lblInfoModo.TabIndex = 8;
            this.lblInfoModo.Text = "Modo: Texto explicativo del modo.";
            // 
            // chkAfectarSubtotal
            // 
            this.chkAfectarSubtotal.AutoSize = true;
            this.chkAfectarSubtotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.chkAfectarSubtotal.Location = new System.Drawing.Point(19, 347);
            this.chkAfectarSubtotal.Name = "chkAfectarSubtotal";
            this.chkAfectarSubtotal.Size = new System.Drawing.Size(267, 23);
            this.chkAfectarSubtotal.TabIndex = 7;
            this.chkAfectarSubtotal.Text = "Afectar Base Imponible de Solicitud";
            this.chkAfectarSubtotal.UseVisualStyleBackColor = true;
            // 
            // chkMostrarDetalle
            // 
            this.chkMostrarDetalle.AutoSize = true;
            this.chkMostrarDetalle.Location = new System.Drawing.Point(19, 320);
            this.chkMostrarDetalle.Name = "chkMostrarDetalle";
            this.chkMostrarDetalle.Size = new System.Drawing.Size(150, 23);
            this.chkMostrarDetalle.TabIndex = 9;
            this.chkMostrarDetalle.Text = "Mostrar Detalle";
            this.chkMostrarDetalle.UseVisualStyleBackColor = true;
            // 
            // groupBoxResultados
            // 
            this.groupBoxResultados.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBoxResultados.Controls.Add(this.lblTotalNota);
            this.groupBoxResultados.Controls.Add(this.lblMontoITBIS);
            this.groupBoxResultados.Controls.Add(this.label5);
            this.groupBoxResultados.Controls.Add(this.label4);
            this.groupBoxResultados.Location = new System.Drawing.Point(19, 219);
            this.groupBoxResultados.Name = "groupBoxResultados";
            this.groupBoxResultados.Size = new System.Drawing.Size(385, 110);
            this.groupBoxResultados.TabIndex = 6;
            this.groupBoxResultados.TabStop = false;
            // 
            // lblTotalNota
            // 
            this.lblTotalNota.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalNota.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblTotalNota.Location = new System.Drawing.Point(135, 60);
            this.lblTotalNota.Name = "lblTotalNota";
            this.lblTotalNota.Size = new System.Drawing.Size(235, 30);
            this.lblTotalNota.TabIndex = 3;
            this.lblTotalNota.Text = "RD$ 0.00";
            this.lblTotalNota.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMontoITBIS
            // 
            this.lblMontoITBIS.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblMontoITBIS.Location = new System.Drawing.Point(135, 20);
            this.lblMontoITBIS.Name = "lblMontoITBIS";
            this.lblMontoITBIS.Size = new System.Drawing.Size(235, 25);
            this.lblMontoITBIS.TabIndex = 2;
            this.lblMontoITBIS.Text = "RD$ 0.00";
            this.lblMontoITBIS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(15, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 21);
            this.label5.TabIndex = 1;
            this.label5.Text = "TOTAL NOTA:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label4.Location = new System.Drawing.Point(15, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 19);
            this.label4.TabIndex = 0;
            this.label4.Text = "Monto ITBIS:";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDescripcion.Location = new System.Drawing.Point(19, 155);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(385, 48);
            this.txtDescripcion.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label3.Location = new System.Drawing.Point(19, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "Descripción:";
            // 
            // numPorcentajeITBIS
            // 
            this.numPorcentajeITBIS.DecimalPlaces = 2;
            this.numPorcentajeITBIS.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.numPorcentajeITBIS.Location = new System.Drawing.Point(324, 78);
            this.numPorcentajeITBIS.Name = "numPorcentajeITBIS";
            this.numPorcentajeITBIS.Size = new System.Drawing.Size(80, 27);
            this.numPorcentajeITBIS.TabIndex = 3;
            this.numPorcentajeITBIS.Value = new decimal(new int[] {
            18,
            0,
            0,
            0});
            // 
            // chkAplicaITBIS
            // 
            this.chkAplicaITBIS.AutoSize = true;
            this.chkAplicaITBIS.Checked = true;
            this.chkAplicaITBIS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAplicaITBIS.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.chkAplicaITBIS.Location = new System.Drawing.Point(19, 80);
            this.chkAplicaITBIS.Name = "chkAplicaITBIS";
            this.chkAplicaITBIS.Size = new System.Drawing.Size(166, 24);
            this.chkAplicaITBIS.TabIndex = 2;
            this.chkAplicaITBIS.Text = "Aplicar ITBIS a nota  ";
            this.chkAplicaITBIS.UseVisualStyleBackColor = true;
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSubtotal.Location = new System.Drawing.Point(100, 25);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.Size = new System.Drawing.Size(304, 29);
            this.txtSubtotal.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(15, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Subtotal:";
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAceptar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAceptar.FlatAppearance.BorderSize = 0;
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(219, 485);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(130, 40);
            this.btnAceptar.TabIndex = 2;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.Gray;
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(83, 485);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 40);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            // 
            // FormConfigNota
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(434, 541);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigNota";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurar Nota";
            this.pnlContenido.ResumeLayout(false);
            this.pnlContenido.PerformLayout();
            this.groupBoxResultados.ResumeLayout(false);
            this.groupBoxResultados.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPorcentajeITBIS)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSubtotal;
        private System.Windows.Forms.CheckBox chkAplicaITBIS;
        private System.Windows.Forms.NumericUpDown numPorcentajeITBIS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.GroupBox groupBoxResultados;
        private System.Windows.Forms.Label lblTotalNota;
        private System.Windows.Forms.Label lblMontoITBIS;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkAfectarSubtotal;
        private System.Windows.Forms.CheckBox chkMostrarDetalle;
        private System.Windows.Forms.Label lblInfoModo;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
