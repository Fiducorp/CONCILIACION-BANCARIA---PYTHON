namespace MOFIS_ERP.Forms.Contabilidad.ConciliacionBancaria
{
    partial class FormVisualizarExcel
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnVolver = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblRuta = new System.Windows.Forms.Label();
            this.btnAbrirCarpeta = new System.Windows.Forms.Button();
            this.dgvDatos = new System.Windows.Forms.DataGridView();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.White;
            this.pnlTop.Controls.Add(this.btnVolver);
            this.pnlTop.Controls.Add(this.lblTitulo);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1200, 60);
            this.pnlTop.TabIndex = 0;
            // 
            // btnVolver
            // 
            this.btnVolver.BackColor = System.Drawing.Color.White;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnVolver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnVolver.Location = new System.Drawing.Point(20, 13);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(100, 35);
            this.btnVolver.TabIndex = 1;
            this.btnVolver.Text = "← VOLVER";
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(1200, 60);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "RESULTADOS DE CONCILIACIÓN";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(96)))));
            this.pnlBottom.Controls.Add(this.btnAbrirCarpeta);
            this.pnlBottom.Controls.Add(this.lblRuta);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 700);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1200, 80);
            this.pnlBottom.TabIndex = 1;
            // 
            // lblRuta
            // 
            this.lblRuta.AutoSize = true;
            this.lblRuta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblRuta.ForeColor = System.Drawing.Color.White;
            this.lblRuta.Location = new System.Drawing.Point(240, 30);
            this.lblRuta.Name = "lblRuta";
            this.lblRuta.Size = new System.Drawing.Size(66, 20);
            this.lblRuta.TabIndex = 0;
            this.lblRuta.Text = "Archivo:";
            // 
            // btnAbrirCarpeta
            // 
            this.btnAbrirCarpeta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(16)))), ((int)(((byte)(242)))));
            this.btnAbrirCarpeta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbrirCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrirCarpeta.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAbrirCarpeta.ForeColor = System.Drawing.Color.White;
            this.btnAbrirCarpeta.Location = new System.Drawing.Point(20, 15);
            this.btnAbrirCarpeta.Name = "btnAbrirCarpeta";
            this.btnAbrirCarpeta.Size = new System.Drawing.Size(200, 50);
            this.btnAbrirCarpeta.TabIndex = 1;
            this.btnAbrirCarpeta.Text = "📂 ABRIR CARPETA";
            this.btnAbrirCarpeta.UseVisualStyleBackColor = false;
            this.btnAbrirCarpeta.Click += new System.EventHandler(this.btnAbrirCarpeta_Click);
            // 
            // dgvDatos
            // 
            this.dgvDatos.AllowUserToAddRows = false;
            this.dgvDatos.AllowUserToDeleteRows = false;
            this.dgvDatos.AllowUserToResizeColumns = false;
            this.dgvDatos.AllowUserToResizeRows = false;
            this.dgvDatos.BackgroundColor = System.Drawing.Color.White;
            this.dgvDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDatos.Location = new System.Drawing.Point(0, 60);
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.Size = new System.Drawing.Size(1200, 640);
            this.dgvDatos.TabIndex = 2;
            // 
            // FormVisualizarExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 780);
            this.Controls.Add(this.dgvDatos);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "FormVisualizarExcel";
            this.Text = "Visualizador de Resultados";
            this.pnlTop.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblRuta;
        private System.Windows.Forms.Button btnAbrirCarpeta;
        private System.Windows.Forms.DataGridView dgvDatos;
    }
}
