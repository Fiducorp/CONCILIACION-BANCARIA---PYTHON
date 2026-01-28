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
            this.grpDatosGenerales = new System.Windows.Forms.GroupBox();
            this.lblDir = new System.Windows.Forms.Label();
            this.txtDirectorioTrabajo = new System.Windows.Forms.TextBox();
            this.btnBuscarDir = new System.Windows.Forms.Button();
            this.lblFideicomiso = new System.Windows.Forms.Label();
            this.cmbFideicomisos = new System.Windows.Forms.ComboBox();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.grpArchivos = new System.Windows.Forms.GroupBox();
            this.lblArchivoBanco = new System.Windows.Forms.Label();
            this.txtArchivoBanco = new System.Windows.Forms.TextBox();
            this.btnSelBanco = new System.Windows.Forms.Button();
            this.lblEstadoBanco = new System.Windows.Forms.Label();
            this.lblArchivoContable = new System.Windows.Forms.Label();
            this.txtArchivoContable = new System.Windows.Forms.TextBox();
            this.lblMoneda = new System.Windows.Forms.Label();
            this.cmbMoneda = new System.Windows.Forms.ComboBox();
            this.btnSelContable = new System.Windows.Forms.Button();
            this.lblEstadoContable = new System.Windows.Forms.Label();
            this.grpConsola = new System.Windows.Forms.GroupBox();
            this.rtbConsola = new System.Windows.Forms.RichTextBox();
            this.pnlSide = new System.Windows.Forms.Panel();
            this.flowParam = new System.Windows.Forms.FlowLayoutPanel();
            this.lblParamTitulo = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnToggleParam = new System.Windows.Forms.Button();
            this.btnAyuda = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnEjecutar = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.grpDatosGenerales.SuspendLayout();
            this.grpArchivos.SuspendLayout();
            this.grpConsola.SuspendLayout();
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
            this.btnVolver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnVolver.ForeColor = System.Drawing.Color.White;
            this.btnVolver.Location = new System.Drawing.Point(20, 13);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(100, 35);
            this.btnVolver.TabIndex = 1;
            this.btnVolver.Text = "SALIR";
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
            this.lblTituloPrincipal.Text = "CONCILIACIÓN BANCARIA IQ";
            this.lblTituloPrincipal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.grpDatosGenerales);
            this.pnlMiddle.Controls.Add(this.grpArchivos);
            this.pnlMiddle.Controls.Add(this.grpConsola);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(0, 0);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Padding = new System.Windows.Forms.Padding(20);
            this.pnlMiddle.Size = new System.Drawing.Size(1920, 1061);
            this.pnlMiddle.TabIndex = 1;
            // 
            // grpDatosGenerales
            // 
            this.grpDatosGenerales.Controls.Add(this.lblDir);
            this.grpDatosGenerales.Controls.Add(this.txtDirectorioTrabajo);
            this.grpDatosGenerales.Controls.Add(this.btnBuscarDir);
            this.grpDatosGenerales.Controls.Add(this.lblFideicomiso);
            this.grpDatosGenerales.Controls.Add(this.cmbFideicomisos);
            this.grpDatosGenerales.Controls.Add(this.btnRefrescar);
            this.grpDatosGenerales.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDatosGenerales.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpDatosGenerales.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.grpDatosGenerales.Location = new System.Drawing.Point(20, 20);
            this.grpDatosGenerales.Name = "grpDatosGenerales";
            this.grpDatosGenerales.Size = new System.Drawing.Size(1880, 200);
            this.grpDatosGenerales.TabIndex = 0;
            this.grpDatosGenerales.TabStop = false;
            this.grpDatosGenerales.Text = "📊 DATOS GENERALES";
            // 
            // lblDir
            // 
            this.lblDir.AutoSize = true;
            this.lblDir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDir.Location = new System.Drawing.Point(17, 66);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(127, 15);
            this.lblDir.TabIndex = 0;
            this.lblDir.Text = "Directorio de Trabajo:";
            // 
            // txtDirectorioTrabajo
            // 
            this.txtDirectorioTrabajo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDirectorioTrabajo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDirectorioTrabajo.Location = new System.Drawing.Point(20, 94);
            this.txtDirectorioTrabajo.Name = "txtDirectorioTrabajo";
            this.txtDirectorioTrabajo.ReadOnly = true;
            this.txtDirectorioTrabajo.Size = new System.Drawing.Size(550, 23);
            this.txtDirectorioTrabajo.TabIndex = 1;
            // 
            // btnBuscarDir
            // 
            this.btnBuscarDir.Location = new System.Drawing.Point(580, 86);
            this.btnBuscarDir.Name = "btnBuscarDir";
            this.btnBuscarDir.Size = new System.Drawing.Size(60, 35);
            this.btnBuscarDir.TabIndex = 2;
            this.btnBuscarDir.Text = "...";
            this.btnBuscarDir.UseVisualStyleBackColor = true;
            this.btnBuscarDir.Click += new System.EventHandler(this.BtnBuscarDir_Click);
            // 
            // lblFideicomiso
            // 
            this.lblFideicomiso.AutoSize = true;
            this.lblFideicomiso.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFideicomiso.Location = new System.Drawing.Point(739, 67);
            this.lblFideicomiso.Name = "lblFideicomiso";
            this.lblFideicomiso.Size = new System.Drawing.Size(75, 15);
            this.lblFideicomiso.TabIndex = 3;
            this.lblFideicomiso.Text = "Fideicomiso:";
            // 
            // cmbFideicomisos
            // 
            this.cmbFideicomisos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFideicomisos.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbFideicomisos.Location = new System.Drawing.Point(739, 92);
            this.cmbFideicomisos.Name = "cmbFideicomisos";
            this.cmbFideicomisos.Size = new System.Drawing.Size(420, 25);
            this.cmbFideicomisos.TabIndex = 4;
            // 
            // btnRefrescar
            // 
            this.btnRefrescar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnRefrescar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefrescar.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnRefrescar.Location = new System.Drawing.Point(1169, 86);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(140, 35);
            this.btnRefrescar.TabIndex = 5;
            this.btnRefrescar.Text = "REESCANEAR";
            this.btnRefrescar.UseVisualStyleBackColor = false;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            // 
            // grpArchivos
            // 
            this.grpArchivos.Controls.Add(this.lblArchivoBanco);
            this.grpArchivos.Controls.Add(this.txtArchivoBanco);
            this.grpArchivos.Controls.Add(this.btnSelBanco);
            this.grpArchivos.Controls.Add(this.lblEstadoBanco);
            this.grpArchivos.Controls.Add(this.lblArchivoContable);
            this.grpArchivos.Controls.Add(this.txtArchivoContable);
            this.grpArchivos.Controls.Add(this.lblMoneda);
            this.grpArchivos.Controls.Add(this.cmbMoneda);
            this.grpArchivos.Controls.Add(this.btnSelContable);
            this.grpArchivos.Controls.Add(this.lblEstadoContable);
            this.grpArchivos.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpArchivos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.grpArchivos.Location = new System.Drawing.Point(20, 240);
            this.grpArchivos.Name = "grpArchivos";
            this.grpArchivos.Size = new System.Drawing.Size(1880, 250);
            this.grpArchivos.TabIndex = 1;
            this.grpArchivos.TabStop = false;
            this.grpArchivos.Text = "📂 ARCHIVOS SELECCIONADOS";
            // 
            // lblArchivoBanco
            // 
            this.lblArchivoBanco.AutoSize = true;
            this.lblArchivoBanco.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArchivoBanco.Location = new System.Drawing.Point(20, 40);
            this.lblArchivoBanco.Name = "lblArchivoBanco";
            this.lblArchivoBanco.Size = new System.Drawing.Size(87, 15);
            this.lblArchivoBanco.TabIndex = 0;
            this.lblArchivoBanco.Text = "Archivo Banco:";
            // 
            // txtArchivoBanco
            // 
            this.txtArchivoBanco.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtArchivoBanco.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtArchivoBanco.Location = new System.Drawing.Point(20, 65);
            this.txtArchivoBanco.Name = "txtArchivoBanco";
            this.txtArchivoBanco.ReadOnly = true;
            this.txtArchivoBanco.Size = new System.Drawing.Size(550, 23);
            this.txtArchivoBanco.TabIndex = 1;
            // 
            // btnSelBanco
            // 
            this.btnSelBanco.Location = new System.Drawing.Point(580, 59);
            this.btnSelBanco.Name = "btnSelBanco";
            this.btnSelBanco.Size = new System.Drawing.Size(60, 35);
            this.btnSelBanco.TabIndex = 2;
            this.btnSelBanco.Text = "...";
            this.btnSelBanco.UseVisualStyleBackColor = true;
            this.btnSelBanco.Click += new System.EventHandler(this.BtnSelBanco_Click);
            // 
            // lblEstadoBanco
            // 
            this.lblEstadoBanco.AutoSize = true;
            this.lblEstadoBanco.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadoBanco.ForeColor = System.Drawing.Color.Gray;
            this.lblEstadoBanco.Location = new System.Drawing.Point(437, 105);
            this.lblEstadoBanco.Name = "lblEstadoBanco";
            this.lblEstadoBanco.Size = new System.Drawing.Size(133, 15);
            this.lblEstadoBanco.TabIndex = 3;
            this.lblEstadoBanco.Text = "⚫ Esperando archivo...";
            // 
            // lblArchivoContable
            // 
            this.lblArchivoContable.AutoSize = true;
            this.lblArchivoContable.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArchivoContable.Location = new System.Drawing.Point(660, 40);
            this.lblArchivoContable.Name = "lblArchivoContable";
            this.lblArchivoContable.Size = new System.Drawing.Size(131, 15);
            this.lblArchivoContable.TabIndex = 4;
            this.lblArchivoContable.Text = "Archivo Libro Contable:";
            // 
            // txtArchivoContable
            // 
            this.txtArchivoContable.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtArchivoContable.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtArchivoContable.Location = new System.Drawing.Point(660, 65);
            this.txtArchivoContable.Name = "txtArchivoContable";
            this.txtArchivoContable.ReadOnly = true;
            this.txtArchivoContable.Size = new System.Drawing.Size(550, 23);
            this.txtArchivoContable.TabIndex = 5;
            // 
            // lblMoneda
            // 
            this.lblMoneda.AutoSize = true;
            this.lblMoneda.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoneda.Location = new System.Drawing.Point(23, 100);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new System.Drawing.Size(54, 15);
            this.lblMoneda.TabIndex = 6;
            this.lblMoneda.Text = "Moneda:";
            // 
            // cmbMoneda
            // 
            this.cmbMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMoneda.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.cmbMoneda.Items.AddRange(new object[] {
            "DOP",
            "USD"});
            this.cmbMoneda.Location = new System.Drawing.Point(23, 125);
            this.cmbMoneda.Name = "cmbMoneda";
            this.cmbMoneda.Size = new System.Drawing.Size(120, 28);
            this.cmbMoneda.TabIndex = 7;
            // 
            // btnSelContable
            // 
            this.btnSelContable.Location = new System.Drawing.Point(1220, 59);
            this.btnSelContable.Name = "btnSelContable";
            this.btnSelContable.Size = new System.Drawing.Size(60, 35);
            this.btnSelContable.TabIndex = 6;
            this.btnSelContable.Text = "...";
            this.btnSelContable.UseVisualStyleBackColor = true;
            this.btnSelContable.Click += new System.EventHandler(this.BtnSelContable_Click);
            // 
            // lblEstadoContable
            // 
            this.lblEstadoContable.AutoSize = true;
            this.lblEstadoContable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadoContable.ForeColor = System.Drawing.Color.Gray;
            this.lblEstadoContable.Location = new System.Drawing.Point(1077, 105);
            this.lblEstadoContable.Name = "lblEstadoContable";
            this.lblEstadoContable.Size = new System.Drawing.Size(133, 15);
            this.lblEstadoContable.TabIndex = 7;
            this.lblEstadoContable.Text = "⚫ Esperando archivo...";
            // 
            // grpConsola
            // 
            this.grpConsola.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConsola.Controls.Add(this.rtbConsola);
            this.grpConsola.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpConsola.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(114)))), ((int)(((byte)(198)))));
            this.grpConsola.Location = new System.Drawing.Point(20, 510);
            this.grpConsola.Name = "grpConsola";
            this.grpConsola.Padding = new System.Windows.Forms.Padding(10);
            this.grpConsola.Size = new System.Drawing.Size(1880, 250);
            this.grpConsola.TabIndex = 2;
            this.grpConsola.TabStop = false;
            this.grpConsola.Text = "💻 LOG DE EJECUCIÓN";
            // 
            // rtbConsola
            // 
            this.rtbConsola.BackColor = System.Drawing.Color.Black;
            this.rtbConsola.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbConsola.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConsola.Font = new System.Drawing.Font("Consolas", 10F);
            this.rtbConsola.ForeColor = System.Drawing.Color.Lime;
            this.rtbConsola.Location = new System.Drawing.Point(10, 28);
            this.rtbConsola.Name = "rtbConsola";
            this.rtbConsola.ReadOnly = true;
            this.rtbConsola.Size = new System.Drawing.Size(1860, 212);
            this.rtbConsola.TabIndex = 0;
            this.rtbConsola.Text = "";
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
            this.pnlBottom.Controls.Add(this.btnToggleParam);
            this.pnlBottom.Controls.Add(this.btnAyuda);
            this.pnlBottom.Controls.Add(this.btnLimpiar);
            this.pnlBottom.Controls.Add(this.btnEjecutar);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 981);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1920, 80);
            this.pnlBottom.TabIndex = 3;
            // 
            // btnToggleParam
            // 
            this.btnToggleParam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(16)))), ((int)(((byte)(242)))));
            this.btnToggleParam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleParam.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnToggleParam.ForeColor = System.Drawing.Color.White;
            this.btnToggleParam.Location = new System.Drawing.Point(20, 15);
            this.btnToggleParam.Name = "btnToggleParam";
            this.btnToggleParam.Size = new System.Drawing.Size(150, 50);
            this.btnToggleParam.TabIndex = 3;
            this.btnToggleParam.Text = "⚙️ AJUSTES";
            this.btnToggleParam.UseVisualStyleBackColor = false;
            this.btnToggleParam.Click += new System.EventHandler(this.btnToggleParam_Click);
            // 
            // btnAyuda
            // 
            this.btnAyuda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnAyuda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAyuda.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAyuda.ForeColor = System.Drawing.Color.White;
            this.btnAyuda.Location = new System.Drawing.Point(1400, 15);
            this.btnAyuda.Name = "btnAyuda";
            this.btnAyuda.Size = new System.Drawing.Size(120, 50);
            this.btnAyuda.TabIndex = 2;
            this.btnAyuda.Text = "❔ AYUDA";
            this.btnAyuda.UseVisualStyleBackColor = false;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(109)))), ((int)(((byte)(122)))));
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(1530, 15);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(120, 50);
            this.btnLimpiar.TabIndex = 1;
            this.btnLimpiar.Text = "🧹 LIMPIAR";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnEjecutar
            // 
            this.btnEjecutar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnEjecutar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEjecutar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEjecutar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEjecutar.ForeColor = System.Drawing.Color.White;
            this.btnEjecutar.Location = new System.Drawing.Point(1660, 15);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(240, 50);
            this.btnEjecutar.TabIndex = 0;
            this.btnEjecutar.Text = "🚀 EJECUTAR";
            this.btnEjecutar.UseVisualStyleBackColor = false;
            this.btnEjecutar.Click += new System.EventHandler(this.BtnEjecutar_Click);
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
            this.grpDatosGenerales.ResumeLayout(false);
            this.grpDatosGenerales.PerformLayout();
            this.grpArchivos.ResumeLayout(false);
            this.grpArchivos.PerformLayout();
            this.grpConsola.ResumeLayout(false);
            this.pnlSide.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTituloPrincipal;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.GroupBox grpDatosGenerales;
        private System.Windows.Forms.Label lblDir;
        private System.Windows.Forms.TextBox txtDirectorioTrabajo;
        private System.Windows.Forms.Button btnBuscarDir;
        private System.Windows.Forms.Label lblFideicomiso;
        private System.Windows.Forms.ComboBox cmbFideicomisos;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Label lblMoneda;
        private System.Windows.Forms.ComboBox cmbMoneda;
        private System.Windows.Forms.GroupBox grpArchivos;
        private System.Windows.Forms.Label lblArchivoBanco;
        private System.Windows.Forms.TextBox txtArchivoBanco;
        private System.Windows.Forms.Button btnSelBanco;
        private System.Windows.Forms.Label lblEstadoBanco;
        private System.Windows.Forms.Label lblArchivoContable;
        private System.Windows.Forms.TextBox txtArchivoContable;
        private System.Windows.Forms.Button btnSelContable;
        private System.Windows.Forms.Label lblEstadoContable;
        private System.Windows.Forms.GroupBox grpConsola;
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
