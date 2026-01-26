using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    public partial class FormVistaPreviaCambios : Form
    {
        public bool CambiosConfirmados { get; private set; }

        private List<CambioPermiso> cambios;
        private string contexto;

        public FormVistaPreviaCambios(List<CambioPermiso> listaCambios, string contextoDescripcion)
        {
            InitializeComponent();
            cambios = listaCambios;
            contexto = contextoDescripcion;
            ConfigurarFormulario();
            CargarCambios();
        }

        private void ConfigurarFormulario()
        {
            // Mejorar mensaje de contexto
            if (contexto.StartsWith("Usuario:"))
            {
                string nombreUsuario = contexto.Replace("Usuario: ", "");
                lblContexto.Text = $"Cambios pendientes de aplicar para usuario: {nombreUsuario}";
            }
            else if (contexto.StartsWith("Rol:"))
            {
                string nombreRol = contexto.Replace("Rol: ", "");
                lblContexto.Text = $"Cambios pendientes de aplicar para rol: {nombreRol}";
            }
            else
            {
                lblContexto.Text = $"Cambios pendientes de aplicar para: {contexto}";
            }

            // Estilos de botones
            btnConfirmarGuardar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatAppearance.BorderSize = 0;

            // Eventos
            btnConfirmarGuardar.Click += (s, e) =>
            {
                CambiosConfirmados = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Configurar DataGridView
            ConfigurarDataGridView();

            // Crear panel de leyenda moderno
            CrearPanelLeyendaModerno();

            // Navegación con teclado
            this.AcceptButton = btnConfirmarGuardar;
            this.CancelButton = btnCancelar;
        }

        private void ConfigurarDataGridView()
        {
            dgvCambios.Columns.Clear();
            dgvCambios.AutoGenerateColumns = false;
            dgvCambios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // AGREGAR ESTA LÍNEA

            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoría",
                FillWeight = 18  // Usar FillWeight en lugar de Width
            });

            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                HeaderText = "Módulo",
                FillWeight = 18
            });

            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                HeaderText = "Formulario",
                FillWeight = 25
            });

            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                HeaderText = "Acción",
                FillWeight = 15
            });

            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValorAnterior",
                HeaderText = "Anterior",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValorNuevo",
                HeaderText = "Nuevo",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            // Estilos
            dgvCambios.EnableHeadersVisualStyles = false;
            dgvCambios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dgvCambios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCambios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvCambios.ColumnHeadersHeight = 40;
            dgvCambios.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvCambios.RowTemplate.Height = 35;
            dgvCambios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // Bloquear redimensionamiento
            dgvCambios.AllowUserToResizeRows = false;
            dgvCambios.AllowUserToResizeColumns = false;
        }

        private void CargarCambios()
        {
            dgvCambios.Rows.Clear();

            int activados = 0;
            int desactivados = 0;

            foreach (var cambio in cambios)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvCambios);

                row.Cells[0].Value = cambio.Categoria;
                row.Cells[1].Value = cambio.Modulo;
                row.Cells[2].Value = cambio.Formulario;
                row.Cells[3].Value = cambio.Accion;
                row.Cells[4].Value = cambio.ValorAnterior ? "✅ SÍ" : "❌ NO";
                row.Cells[5].Value = cambio.ValorNuevo ? "✅ SÍ" : "❌ NO";

                // Colorear según el tipo de cambio
                if (!cambio.ValorAnterior && cambio.ValorNuevo)
                {
                    // ACTIVADO (antes NO, ahora SÍ) - Verde Material Design claro
                    row.DefaultCellStyle.BackColor = Color.FromArgb(200, 230, 201);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(165, 214, 167);
                    activados++;
                }
                else if (cambio.ValorAnterior && !cambio.ValorNuevo)
                {
                    // DESACTIVADO (antes SÍ, ahora NO) - Rojo Material Design claro
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 205, 210);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 154, 154);
                    desactivados++;
                }

                dgvCambios.Rows.Add(row);
            }

            // Actualizar resumen
            lblResumen.Text = $"Total de cambios: {cambios.Count}  |  " +
                             $"🟢 Activados: {activados}  |  " +
                             $"🔴 Desactivados: {desactivados}";
        }

        private void CrearPanelLeyendaModerno()
        {
            panelLeyenda.Controls.Clear();
            panelLeyenda.BackColor = Color.White;
            panelLeyenda.BorderStyle = BorderStyle.FixedSingle;
            panelLeyenda.Height = 90;

            // Título "LEYENDA:" centrado
            Label lblTituloLeyenda = new Label
            {
                Text = "LEYENDA",
                Location = new Point(0, 12),
                Size = new Size(panelLeyenda.Width, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                TextAlign = ContentAlignment.TopCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panelLeyenda.Controls.Add(lblTituloLeyenda);

            // Calcular posición centrada para los 3 elementos
            int anchoCuadro = 30;
            int anchoTexto1 = 180; // "Permiso Activado"
            int anchoTexto2 = 190; // "Permiso Desactivado"
            int anchoTexto3 = 130; // "Sin cambios"
            int espacioEntreCuadros = 20; // Espacio entre cada conjunto

            int anchoTotal = (anchoCuadro + 5 + anchoTexto1) + espacioEntreCuadros +
                             (anchoCuadro + 5 + anchoTexto2) + espacioEntreCuadros +
                             (anchoCuadro + 5 + anchoTexto3);

            int inicioX = (panelLeyenda.Width - anchoTotal) / 2;

            // Cuadro VERDE - Permiso Activado
            Panel pnlVerde = new Panel
            {
                Location = new Point(inicioX, 50),
                Size = new Size(anchoCuadro, anchoCuadro),
                BackColor = Color.FromArgb(76, 175, 80), // Verde Material Design
                BorderStyle = BorderStyle.FixedSingle
            };
            panelLeyenda.Controls.Add(pnlVerde);

            Label lblVerde = new Label
            {
                Text = "Permiso Activado",
                Location = new Point(inicioX + anchoCuadro + 5, 50),
                Size = new Size(anchoTexto1, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelLeyenda.Controls.Add(lblVerde);

            // Cuadro ROJO - Permiso Desactivado
            int inicioRojo = inicioX + (anchoCuadro + 5 + anchoTexto1) + espacioEntreCuadros;
            Panel pnlRojo = new Panel
            {
                Location = new Point(inicioRojo, 50),
                Size = new Size(anchoCuadro, anchoCuadro),
                BackColor = Color.FromArgb(244, 67, 54), // Rojo Material Design
                BorderStyle = BorderStyle.FixedSingle
            };
            panelLeyenda.Controls.Add(pnlRojo);

            Label lblRojo = new Label
            {
                Text = "Permiso Desactivado",
                Location = new Point(inicioRojo + anchoCuadro + 5, 50),
                Size = new Size(anchoTexto2, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelLeyenda.Controls.Add(lblRojo);

            // Cuadro GRIS - Sin cambios
            int inicioGris = inicioRojo + (anchoCuadro + 5 + anchoTexto2) + espacioEntreCuadros;
            Panel pnlGris = new Panel
            {
                Location = new Point(inicioGris, 50),
                Size = new Size(anchoCuadro, anchoCuadro),
                BackColor = Color.FromArgb(189, 189, 189), // Gris claro
                BorderStyle = BorderStyle.FixedSingle
            };
            panelLeyenda.Controls.Add(pnlGris);

            Label lblGris = new Label
            {
                Text = "Sin cambios",
                Location = new Point(inicioGris + anchoCuadro + 5, 50),
                Size = new Size(anchoTexto3, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelLeyenda.Controls.Add(lblGris);
        }
    }


    // Clase para representar un cambio de permiso
    public class CambioPermiso
    {
        public string Categoria { get; set; }
        public string Modulo { get; set; }
        public string Formulario { get; set; }
        public string Accion { get; set; }
        public bool ValorAnterior { get; set; }
        public bool ValorNuevo { get; set; }
    }
}