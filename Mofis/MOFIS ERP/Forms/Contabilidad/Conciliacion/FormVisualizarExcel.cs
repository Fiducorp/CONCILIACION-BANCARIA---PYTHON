using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ClosedXML.Excel;
using MOFIS_ERP.Forms;
using System.IO;

namespace MOFIS_ERP.Forms.Contabilidad.ConciliacionBancaria
{
    public partial class FormVisualizarExcel : Form
    {
        private FormMain formPrincipal;
        private string filePath;
        private Form formAnterior;

        public FormVisualizarExcel(FormMain formMain, string path, Form anterior)
        {
            InitializeComponent();
            this.formPrincipal = formMain;
            this.filePath = path;
            this.formAnterior = anterior;
            
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            lblRuta.Text = "Archivo: " + Path.GetFileName(filePath);
            CargarExcel();
        }

        private void CargarExcel()
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var range = worksheet.RangeUsed();
                    var dataTable = new DataTable();

                    bool firstRow = true;
                    foreach (var row in range.Rows())
                    {
                        if (firstRow)
                        {
                            foreach (var cell in row.Cells())
                            {
                                dataTable.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            dataTable.Rows.Add();
                            int i = 0;
                            foreach (var cell in row.Cells(1, dataTable.Columns.Count))
                            {
                                dataTable.Rows[dataTable.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }

                    dgvDatos.DataSource = dataTable;
                    ConfigurarGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el archivo Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGrid()
        {
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDatos.AllowUserToAddRows = false;
            dgvDatos.ReadOnly = true;
            dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDatos.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            formPrincipal.CargarContenidoPanel(formAnterior);
        }

        private void btnAbrirCarpeta_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    string folder = Path.GetDirectoryName(filePath);
                    if (Directory.Exists(folder))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", folder);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir la carpeta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
