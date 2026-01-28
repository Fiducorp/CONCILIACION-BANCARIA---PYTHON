using System.Drawing;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.ConciliacionBancaria
{
    partial class FormConciliacion
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnVolver = new System.Windows.Forms.Button();
            this.lblTituloPrincipal = new System.Windows.Forms.Label();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlLog = new System.Windows.Forms.Panel();
            this.rtbConsola = new System.Windows.Forms.RichTextBox();
            this.btnToggleLog = new System.Windows.Forms.Button();
            this.pnlSectionTop = new System.Windows.Forms.Panel();
            this.pnlSectionBottom = new System.Windows.Forms.Panel();
            this.lblDir = new System.Windows.Forms.Label();
            this.txtDirectorioTrabajo = new System.Windows.Forms.TextBox();
            this.btnBuscarDir = new System.Windows.Forms.Button();
            this.lblFideicomiso = new System.Windows.Forms.Label();
            this.cmbFideicomisos = new System.Windows.Forms.ComboBox();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.lblArchivoBanco = new System.Windows.Forms.Label();
            this.txtArchivoBanco = new System.Windows.Forms.TextBox();
            this.btnSelBanco = new System.Windows.Forms.Button();
            this.lblEstadoBanco = new System.Windows.Forms.Label();
            this.lblArchivoContable = new System.Windows.Forms.Label();
            this.txtArchivoContable = new System.Windows.Forms.TextBox();
            this.btnSelContable = new System.Windows.Forms.Button();
            this.lblEstadoContable = new System.Windows.Forms.Label();
            this.lblCuenta = new System.Windows.Forms.Label();
            this.cmbCuenta = new System.Windows.Forms.ComboBox();
            this.lblMoneda = new System.Windows.Forms.Label();
            this.btnMonedaDOP = new System.Windows.Forms.Button();
            this.btnMonedaUSD = new System.Windows.Forms.Button();
            this.cmbMoneda = new System.Windows.Forms.ComboBox();
            this.pnlSide = new System.Windows.Forms.Panel();
            this.flowParam = new System.Windows.Forms.FlowLayoutPanel();
            this.lblParamTitulo = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnEjecutar = new System.Windows.Forms.Button();
            this.btnToggleParam = new System.Windows.Forms.Button();
            this.btnAyuda = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.pnlLog.SuspendLayout();
            this.pnlSectionTop.SuspendLayout();
            this.pnlSectionBottom.SuspendLayout();
            this.pnlSide.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.White;
            this.pnlTop.Controls.Add(this.btnVolver);
            this.pnlTop.Controls.Add(this.lblTituloPrincipal);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1920, 60);
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
            this.btnVolver.Text = "← SALIR";
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // lblTituloPrincipal
            // 
            this.lblTituloPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTituloPrincipal.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTituloPrincipal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblTituloPrincipal.Location = new System.Drawing.Point(0, 0);
            this.lblTituloPrincipal.Name = "lblTituloPrincipal";
            this.lblTituloPrincipal.Size = new System.Drawing.Size(1920, 60);
            this.lblTituloPrincipal.TabIndex = 0;
            this.lblTituloPrincipal.Text = "CONCILIACIÓN BANCARIA";
            this.lblTituloPrincipal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTituloPrincipal.Click += new System.EventHandler(this.lblTituloPrincipal_Click);
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.pnlSectionBottom);
            this.pnlMiddle.Controls.Add(this.pnlSectionTop);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(0, 60);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(1920, 921);
            this.pnlMiddle.TabIndex = 1;
            // 
            // pnlSectionTop
            // 
            this.pnlSectionTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.pnlSectionTop.Controls.Add(this.lblDir);
            this.pnlSectionTop.Controls.Add(this.txtDirectorioTrabajo);
            this.pnlSectionTop.Controls.Add(this.btnBuscarDir);
            this.pnlSectionTop.Controls.Add(this.btnRefrescar);
            this.pnlSectionTop.Controls.Add(this.btnToggleLog);
            this.pnlSectionTop.Controls.Add(this.pnlLog);
            this.pnlSectionTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSectionTop.Location = new System.Drawing.Point(0, 0);
            this.pnlSectionTop.Name = "pnlSectionTop";
            this.pnlSectionTop.Padding = new System.Windows.Forms.Padding(40, 20, 40, 20);
            this.pnlSectionTop.Size = new System.Drawing.Size(1920, 260);
            this.pnlSectionTop.TabIndex = 0;
            // 
            // pnlSectionBottom
            // 
            this.pnlSectionBottom.Controls.Add(this.lblFideicomiso);
            this.pnlSectionBottom.Controls.Add(this.cmbFideicomisos);
            this.pnlSectionBottom.Controls.Add(this.lblCuenta);
            this.pnlSectionBottom.Controls.Add(this.cmbCuenta);
            this.pnlSectionBottom.Controls.Add(this.lblMoneda);
            this.pnlSectionBottom.Controls.Add(this.btnMonedaDOP);
            this.pnlSectionBottom.Controls.Add(this.btnMonedaUSD);
            this.pnlSectionBottom.Controls.Add(this.lblArchivoBanco);
            this.pnlSectionBottom.Controls.Add(this.txtArchivoBanco);
            this.pnlSectionBottom.Controls.Add(this.btnSelBanco);
            this.pnlSectionBottom.Controls.Add(this.lblEstadoBanco);
            this.pnlSectionBottom.Controls.Add(this.lblArchivoContable);
            this.pnlSectionBottom.Controls.Add(this.txtArchivoContable);
            this.pnlSectionBottom.Controls.Add(this.btnSelContable);
            this.pnlSectionBottom.Controls.Add(this.lblEstadoContable);
            this.pnlSectionBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSectionBottom.Location = new System.Drawing.Point(0, 260);
            this.pnlSectionBottom.Name = "pnlSectionBottom";
            this.pnlSectionBottom.Padding = new System.Windows.Forms.Padding(40);
            this.pnlSectionBottom.Size = new System.Drawing.Size(1920, 661);
            this.pnlSectionBottom.TabIndex = 1;
            // 
            // pnlLog
            // 
            this.pnlLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLog.BackColor = System.Drawing.Color.White;
            this.pnlLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLog.Controls.Add(this.rtbConsola);
            this.pnlLog.Location = new System.Drawing.Point(1000, 20);
            this.pnlLog.Name = "pnlLog";
            this.pnlLog.Size = new System.Drawing.Size(880, 220);
            this.pnlLog.TabIndex = 3;
            this.pnlLog.Visible = false;
            // 
            // rtbConsola
            // 
            this.rtbConsola.BackColor = System.Drawing.Color.Black;
            this.rtbConsola.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbConsola.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConsola.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbConsola.ForeColor = System.Drawing.Color.Lime;
            this.rtbConsola.Location = new System.Drawing.Point(0, 0);
            this.rtbConsola.Name = "rtbConsola";
            this.rtbConsola.ReadOnly = true;
            this.rtbConsola.Size = new System.Drawing.Size(878, 218);
            this.rtbConsola.TabIndex = 0;
            this.rtbConsola.Text = "";
            // 
            // btnToggleLog
            // 
            this.btnToggleLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(109)))), ((int)(((byte)(122)))));
            this.btnToggleLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleLog.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnToggleLog.ForeColor = System.Drawing.Color.White;
            this.btnToggleLog.Location = new System.Drawing.Point(860, 150);
            this.btnToggleLog.Name = "btnToggleLog";
            this.btnToggleLog.Size = new System.Drawing.Size(120, 40);
            this.btnToggleLog.TabIndex = 2;
            this.btnToggleLog.Text = "LOG";
            this.btnToggleLog.UseVisualStyleBackColor = false;
            // 
            // lblDir
            // 
            this.lblDir.AutoSize = true;
            this.lblDir.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblDir.Location = new System.Drawing.Point(40, 40);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(79, 21);
            this.lblDir.TabIndex = 0;
            this.lblDir.Text = "CARPETA";
            // 
            // txtDirectorioTrabajo
            // 
            this.txtDirectorioTrabajo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDirectorioTrabajo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtDirectorioTrabajo.Location = new System.Drawing.Point(40, 70);
            this.txtDirectorioTrabajo.Name = "txtDirectorioTrabajo";
            this.txtDirectorioTrabajo.ReadOnly = true;
            this.txtDirectorioTrabajo.Size = new System.Drawing.Size(400, 27);
            this.txtDirectorioTrabajo.TabIndex = 0;
            // 
            // btnBuscarDir
            // 
            this.btnBuscarDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.btnBuscarDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarDir.ForeColor = System.Drawing.Color.White;
            this.btnBuscarDir.Location = new System.Drawing.Point(446, 68);
            this.btnBuscarDir.Name = "btnBuscarDir";
            this.btnBuscarDir.Size = new System.Drawing.Size(50, 31);
            this.btnBuscarDir.TabIndex = 1;
            this.btnBuscarDir.Text = "...";
            this.btnBuscarDir.UseVisualStyleBackColor = false;
            this.btnBuscarDir.Click += new System.EventHandler(this.BtnBuscarDir_Click);
            // 
            // btnRefrescar
            // 
            this.btnRefrescar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnRefrescar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefrescar.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnRefrescar.Location = new System.Drawing.Point(40, 150);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(150, 40);
            this.btnRefrescar.TabIndex = 4;
            this.btnRefrescar.Text = "REESCANEAR";
            this.btnRefrescar.UseVisualStyleBackColor = false;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            // 
            // lblFideicomiso
            // 
            this.lblFideicomiso.AutoSize = true;
            this.lblFideicomiso.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblFideicomiso.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblFideicomiso.Location = new System.Drawing.Point(40, 20);
            this.lblFideicomiso.Name = "lblFideicomiso";
            this.lblFideicomiso.Size = new System.Drawing.Size(104, 20);
            this.lblFideicomiso.TabIndex = 0;
            this.lblFideicomiso.Text = "FIDEICOMISO";
            // 
            // cmbFideicomisos
            // 
            this.cmbFideicomisos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFideicomisos.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbFideicomisos.Location = new System.Drawing.Point(40, 50);
            this.cmbFideicomisos.Name = "cmbFideicomisos";
            this.cmbFideicomisos.Size = new System.Drawing.Size(400, 28);
            this.cmbFideicomisos.TabIndex = 1;
            // 
            // lblCuenta
            // 
            this.lblCuenta.AutoSize = true;
            this.lblCuenta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblCuenta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblCuenta.Location = new System.Drawing.Point(40, 100);
            this.lblCuenta.Name = "lblCuenta";
            this.lblCuenta.Size = new System.Drawing.Size(68, 20);
            this.lblCuenta.TabIndex = 2;
            this.lblCuenta.Text = "CUENTA";
            // 
            // cmbCuenta
            // 
            this.cmbCuenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCuenta.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbCuenta.Items.AddRange(new object[] {
            "-- NULO --"});
            this.cmbCuenta.Location = new System.Drawing.Point(40, 130);
            this.cmbCuenta.Name = "cmbCuenta";
            this.cmbCuenta.Size = new System.Drawing.Size(200, 28);
            this.cmbCuenta.TabIndex = 3;
            // 
            // lblMoneda
            // 
            this.lblMoneda.AutoSize = true;
            this.lblMoneda.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblMoneda.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblMoneda.Location = new System.Drawing.Point(260, 100);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new System.Drawing.Size(76, 20);
            this.lblMoneda.TabIndex = 4;
            this.lblMoneda.Text = "MONEDA";
            // 
            // btnMonedaDOP
            // 
            this.btnMonedaDOP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.btnMonedaDOP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMonedaDOP.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMonedaDOP.ForeColor = System.Drawing.Color.White;
            this.btnMonedaDOP.Location = new System.Drawing.Point(260, 130);
            this.btnMonedaDOP.Name = "btnMonedaDOP";
            this.btnMonedaDOP.Size = new System.Drawing.Size(80, 40);
            this.btnMonedaDOP.TabIndex = 5;
            this.btnMonedaDOP.Text = "DOP";
            this.btnMonedaDOP.UseVisualStyleBackColor = false;
            // 
            // btnMonedaUSD
            // 
            this.btnMonedaUSD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnMonedaUSD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMonedaUSD.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMonedaUSD.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnMonedaUSD.Location = new System.Drawing.Point(346, 130);
            this.btnMonedaUSD.Name = "btnMonedaUSD";
            this.btnMonedaUSD.Size = new System.Drawing.Size(80, 40);
            this.btnMonedaUSD.TabIndex = 6;
            this.btnMonedaUSD.Text = "USD";
            this.btnMonedaUSD.UseVisualStyleBackColor = false;
            // 
            // lblArchivoBanco
            // 
            this.lblArchivoBanco.AutoSize = true;
            this.lblArchivoBanco.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblArchivoBanco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblArchivoBanco.Location = new System.Drawing.Point(520, 20);
            this.lblArchivoBanco.Name = "lblArchivoBanco";
            this.lblArchivoBanco.Size = new System.Drawing.Size(62, 20);
            this.lblArchivoBanco.TabIndex = 7;
            this.lblArchivoBanco.Text = "BANCO";
            // 
            // txtArchivoBanco
            // 
            this.txtArchivoBanco.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtArchivoBanco.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtArchivoBanco.Location = new System.Drawing.Point(520, 50);
            this.txtArchivoBanco.Name = "txtArchivoBanco";
            this.txtArchivoBanco.ReadOnly = true;
            this.txtArchivoBanco.Size = new System.Drawing.Size(350, 27);
            this.txtArchivoBanco.TabIndex = 8;
            // 
            // btnSelBanco
            // 
            this.btnSelBanco.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.btnSelBanco.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelBanco.ForeColor = System.Drawing.Color.White;
            this.btnSelBanco.Location = new System.Drawing.Point(876, 48);
            this.btnSelBanco.Name = "btnSelBanco";
            this.btnSelBanco.Size = new System.Drawing.Size(50, 31);
            this.btnSelBanco.TabIndex = 9;
            this.btnSelBanco.Text = "...";
            this.btnSelBanco.UseVisualStyleBackColor = false;
            this.btnSelBanco.Click += new System.EventHandler(this.BtnSelBanco_Click);
            // 
            // lblEstadoBanco
            // 
            this.lblEstadoBanco.AutoSize = true;
            this.lblEstadoBanco.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadoBanco.ForeColor = System.Drawing.Color.Gray;
            this.lblEstadoBanco.Location = new System.Drawing.Point(520, 85);
            this.lblEstadoBanco.Name = "lblEstadoBanco";
            this.lblEstadoBanco.Size = new System.Drawing.Size(20, 15);
            this.lblEstadoBanco.TabIndex = 10;
            this.lblEstadoBanco.Text = "⚫";
            // 
            // lblArchivoContable
            // 
            this.lblArchivoContable.AutoSize = true;
            this.lblArchivoContable.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblArchivoContable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblArchivoContable.Location = new System.Drawing.Point(950, 20);
            this.lblArchivoContable.Name = "lblArchivoContable";
            this.lblArchivoContable.Size = new System.Drawing.Size(86, 20);
            this.lblArchivoContable.TabIndex = 11;
            this.lblArchivoContable.Text = "CONTABLE";
            // 
            // txtArchivoContable
            // 
            this.txtArchivoContable.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtArchivoContable.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtArchivoContable.Location = new System.Drawing.Point(950, 50);
            this.txtArchivoContable.Name = "txtArchivoContable";
            this.txtArchivoContable.ReadOnly = true;
            this.txtArchivoContable.Size = new System.Drawing.Size(350, 27);
            this.txtArchivoContable.TabIndex = 12;
            // 
            // btnSelContable
            // 
            this.btnSelContable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.btnSelContable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelContable.ForeColor = System.Drawing.Color.White;
            this.btnSelContable.Location = new System.Drawing.Point(1306, 48);
            this.btnSelContable.Name = "btnSelContable";
            this.btnSelContable.Size = new System.Drawing.Size(50, 31);
            this.btnSelContable.TabIndex = 13;
            this.btnSelContable.Text = "...";
            this.btnSelContable.UseVisualStyleBackColor = false;
            this.btnSelContable.Click += new System.EventHandler(this.BtnSelContable_Click);
            // 
            // lblEstadoContable
            // 
            this.lblEstadoContable.AutoSize = true;
            this.lblEstadoContable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadoContable.ForeColor = System.Drawing.Color.Gray;
            this.lblEstadoContable.Location = new System.Drawing.Point(950, 85);
            this.lblEstadoContable.Name = "lblEstadoContable";
            this.lblEstadoContable.Size = new System.Drawing.Size(20, 15);
            this.lblEstadoContable.TabIndex = 14;
            this.lblEstadoContable.Text = "⚫";
            // 
            // cmbMoneda
            // 
            this.cmbMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMoneda.Items.AddRange(new object[] {
            "DOP",
            "USD"});
            this.cmbMoneda.Location = new System.Drawing.Point(0, 0);
            this.cmbMoneda.Name = "cmbMoneda";
            this.cmbMoneda.Size = new System.Drawing.Size(121, 21);
            this.cmbMoneda.TabIndex = 100;
            this.cmbMoneda.Visible = false;
            // 
            // pnlSide
            // 
            this.pnlSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlSide.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSide.Controls.Add(this.flowParam);
            this.pnlSide.Controls.Add(this.lblParamTitulo);
            this.pnlSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSide.Location = new System.Drawing.Point(0, 60);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(400, 921);
            this.pnlSide.TabIndex = 2;
            // 
            // flowParam
            // 
            this.flowParam.AutoScroll = true;
            this.flowParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowParam.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowParam.Location = new System.Drawing.Point(0, 40);
            this.flowParam.Name = "flowParam";
            this.flowParam.Padding = new System.Windows.Forms.Padding(10, 10, 30, 10);
            this.flowParam.Size = new System.Drawing.Size(398, 879);
            this.flowParam.TabIndex = 1;
            this.flowParam.WrapContents = false;
            // 
            // lblParamTitulo
            // 
            this.lblParamTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.lblParamTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblParamTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblParamTitulo.ForeColor = System.Drawing.Color.White;
            this.lblParamTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblParamTitulo.Name = "lblParamTitulo";
            this.lblParamTitulo.Size = new System.Drawing.Size(398, 40);
            this.lblParamTitulo.TabIndex = 0;
            this.lblParamTitulo.Text = "⚙️ PARAMETROS";
            this.lblParamTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblParamTitulo.Click += new System.EventHandler(this.lblParamTitulo_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(96)))));
            this.pnlBottom.Controls.Add(this.btnEjecutar);
            this.pnlBottom.Controls.Add(this.btnToggleParam);
            this.pnlBottom.Controls.Add(this.btnAyuda);
            this.pnlBottom.Controls.Add(this.btnLimpiar);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 981);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1920, 80);
            this.pnlBottom.TabIndex = 3;
            // 
            // btnEjecutar
            // 
            this.btnEjecutar.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.btnEjecutar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEjecutar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEjecutar.ForeColor = System.Drawing.Color.White;
            this.btnEjecutar.Location = new System.Drawing.Point(1660, 16);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(240, 50);
            this.btnEjecutar.TabIndex = 0;
            this.btnEjecutar.Text = "EJECUTAR";
            this.btnEjecutar.UseVisualStyleBackColor = false;
            this.btnEjecutar.Click += new System.EventHandler(this.BtnEjecutar_Click);
            // 
            // btnToggleParam
            // 
            this.btnToggleParam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(16)))), ((int)(((byte)(242)))));
            this.btnToggleParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleParam.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnToggleParam.ForeColor = System.Drawing.Color.White;
            this.btnToggleParam.Location = new System.Drawing.Point(30, 15);
            this.btnToggleParam.Name = "btnToggleParam";
            this.btnToggleParam.Size = new System.Drawing.Size(150, 50);
            this.btnToggleParam.TabIndex = 3;
            this.btnToggleParam.Text = "⚙️ AJUSTES";
            this.btnToggleParam.UseVisualStyleBackColor = false;
            this.btnToggleParam.Click += new System.EventHandler(this.btnToggleParam_Click);
            // 
            // btnAyuda
            // 
            this.btnAyuda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnAyuda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAyuda.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAyuda.ForeColor = System.Drawing.Color.White;
            this.btnAyuda.Location = new System.Drawing.Point(1515, 16);
            this.btnAyuda.Name = "btnAyuda";
            this.btnAyuda.Size = new System.Drawing.Size(120, 50);
            this.btnAyuda.TabIndex = 2;
            this.btnAyuda.Text = "❔ AYUDA";
            this.btnAyuda.UseVisualStyleBackColor = false;
            this.btnAyuda.Click += new System.EventHandler(this.btnAyuda_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(109)))), ((int)(((byte)(122)))));
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(202, 15);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(120, 50);
            this.btnLimpiar.TabIndex = 1;
            this.btnLimpiar.Text = "🧹 LIMPIAR";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // FormConciliacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1920, 1061);
            this.Controls.Add(this.pnlSide);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlMiddle);
            this.Name = "FormConciliacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conciliación Bancaria - ERP Interface";
            this.pnlTop.ResumeLayout(false);
            this.pnlMiddle.ResumeLayout(false);
            this.pnlLog.ResumeLayout(false);
            this.pnlSectionTop.ResumeLayout(false);
            this.pnlSectionTop.PerformLayout();
            this.pnlSectionBottom.ResumeLayout(false);
            this.pnlSectionBottom.PerformLayout();
            this.pnlSide.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSectionTop;
        private System.Windows.Forms.Panel pnlSectionBottom;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTituloPrincipal;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Label lblDir;
        private System.Windows.Forms.TextBox txtDirectorioTrabajo;
        private System.Windows.Forms.Button btnBuscarDir;
        private System.Windows.Forms.Label lblFideicomiso;
        private System.Windows.Forms.ComboBox cmbFideicomisos;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Label lblMoneda;
        private System.Windows.Forms.Button btnMonedaDOP;
        private System.Windows.Forms.Button btnMonedaUSD;
        private System.Windows.Forms.ComboBox cmbMoneda; // Keep hidden/used for logic
        private System.Windows.Forms.Label lblCuenta;
        private System.Windows.Forms.ComboBox cmbCuenta;
        private System.Windows.Forms.Button btnToggleLog;
        private System.Windows.Forms.Panel pnlLog;
        private System.Windows.Forms.Label lblArchivoBanco;
        private System.Windows.Forms.TextBox txtArchivoBanco;
        private System.Windows.Forms.Button btnSelBanco;
        private System.Windows.Forms.Label lblEstadoBanco;
        private System.Windows.Forms.Label lblArchivoContable;
        private System.Windows.Forms.TextBox txtArchivoContable;
        private System.Windows.Forms.Button btnSelContable;
        private System.Windows.Forms.Label lblEstadoContable;
        private System.Windows.Forms.RichTextBox rtbConsola;
        private System.Windows.Forms.Panel pnlSide;
        private System.Windows.Forms.Label lblParamTitulo;
        private System.Windows.Forms.FlowLayoutPanel flowParam;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnToggleParam;
        private System.Windows.Forms.Button btnAyuda;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnEjecutar;
    }
}
