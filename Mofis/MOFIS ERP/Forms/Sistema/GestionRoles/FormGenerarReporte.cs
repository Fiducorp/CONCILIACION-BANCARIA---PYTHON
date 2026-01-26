using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using MOFIS_ERP.Classes;
using ClosedXML.Excel;

namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    public partial class FormGenerarReporte : Form
    {
        private int rolEspecificoID = 0;

        public FormGenerarReporte()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarRoles();
        }

        private void ConfigurarFormulario()
        {
            // Estilos de botones
            btnGenerar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnExaminar.FlatAppearance.BorderSize = 0;

            // Eventos
            btnGenerar.Click += BtnGenerar_Click;
            btnCancelar.Click += (s, e) => this.Close();
            btnExaminar.Click += BtnExaminar_Click;

            // Evento para habilitar ComboBox solo si se selecciona rbRolEspecifico
            rbRolEspecifico.CheckedChanged += (s, e) =>
            {
                cmbRolEspecifico.Enabled = rbRolEspecifico.Checked;
                if (rbRolEspecifico.Checked && cmbRolEspecifico.SelectedIndex == 0)
                {
                    cmbRolEspecifico.SelectedIndex = 1; // Seleccionar primer rol
                }
            };

            // Navegación con teclado
            this.AcceptButton = btnGenerar;
            this.CancelButton = btnCancelar;
        }

        private void CargarRoles()
        {
            try
            {
                cmbRolEspecifico.Items.Clear();
                cmbRolEspecifico.Items.Add("Seleccione un rol...");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT RolID, NombreRol 
                                  FROM Roles 
                                  WHERE Activo = 1 
                                  ORDER BY NombreRol";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ComboBoxRol
                            {
                                RolID = reader.GetInt32(0),
                                NombreRol = reader.GetString(1)
                            };
                            cmbRolEspecifico.Items.Add(item);
                        }
                    }
                }

                cmbRolEspecifico.DisplayMember = "NombreRol";
                cmbRolEspecifico.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExaminar_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Seleccione la carpeta donde guardar el reporte";
                dialog.SelectedPath = txtRuta.Text;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtRuta.Text = dialog.SelectedPath;
                }
            }
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            // Validar ruta
            if (string.IsNullOrWhiteSpace(txtRuta.Text))
            {
                MessageBox.Show("Debe especificar una ruta válida.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRuta.Focus();
                return;
            }

            // Validar selección de rol específico
            if (rbRolEspecifico.Checked)
            {
                if (cmbRolEspecifico.SelectedIndex <= 0)
                {
                    MessageBox.Show("Debe seleccionar un rol específico.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbRolEspecifico.Focus();
                    return;
                }

                var rolSeleccionado = cmbRolEspecifico.SelectedItem as ComboBoxRol;
                rolEspecificoID = rolSeleccionado.RolID;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnGenerar.Enabled = false;

                // Construir ruta completa
                string rutaBase = txtRuta.Text.TrimEnd('\\');

                if (chkCrearCarpeta.Checked)
                {
                    rutaBase = Path.Combine(rutaBase, "Reportes_MOFIS");

                    // Crear carpeta si no existe
                    if (!Directory.Exists(rutaBase))
                    {
                        Directory.CreateDirectory(rutaBase);
                    }
                }

                // Nombre del archivo
                string nombreArchivo = $"Reporte_Permisos_{DateTime.Now:yyyy-MM-dd_HHmmss}.xlsx";
                string rutaCompleta = Path.Combine(rutaBase, nombreArchivo);

                // Generar reporte según tipo
                if (rbCompleto.Checked)
                {
                    GenerarReporteCompleto(rutaCompleta);
                }
                else if (rbSoloMatriz.Checked)
                {
                    GenerarSoloMatriz(rutaCompleta);
                }
                else if (rbSoloExcepciones.Checked)
                {
                    GenerarSoloExcepciones(rutaCompleta);
                }
                else if (rbRolEspecifico.Checked)
                {
                    GenerarRolEspecifico(rutaCompleta);
                }

                this.Cursor = Cursors.Default;
                btnGenerar.Enabled = true;

                // Preguntar si desea abrir el archivo
                DialogResult abrir = MessageBox.Show(
                    $"✅ Reporte generado exitosamente.\n\n" +
                    $"Ubicación:\n{rutaCompleta}\n\n" +
                    $"¿Desea abrir el archivo ahora?",
                    "Reporte Generado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (abrir == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(rutaCompleta);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                btnGenerar.Enabled = true;

                MessageBox.Show($"Error al generar reporte:\n\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarReporteCompleto(string rutaArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
                // Hoja 1: Resumen General
                if (chkIncluirEstadisticas.Checked)
                {
                    CrearHojaResumen(workbook);
                }

                // Hoja 2: Matriz Completa
                CrearHojaMatrizCompleta(workbook);

                // Hoja 3: Detalle por Roles
                CrearHojaDetallePorRoles(workbook);

                // Hoja 4: Detalle por Usuarios
                CrearHojaDetallePorUsuarios(workbook);

                // Hoja 5: Solo Excepciones
                CrearHojaSoloExcepciones(workbook);

                workbook.SaveAs(rutaArchivo);
            }

            // Registrar en auditoría
            AuditoriaHelper.RegistrarAccion(
                SesionActual.UsuarioID,
                "GENERAR_REPORTE_PERMISOS",
                "SISTEMA",
                "Gestión de Roles",
                "FormGenerarReporte",
                detalle: $"Reporte completo generado: {Path.GetFileName(rutaArchivo)}"
            );
        }

        private void GenerarSoloMatriz(string rutaArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
                CrearHojaMatrizCompleta(workbook);
                workbook.SaveAs(rutaArchivo);
            }

            AuditoriaHelper.RegistrarAccion(
                SesionActual.UsuarioID,
                "GENERAR_REPORTE_MATRIZ",
                "SISTEMA",
                "Gestión de Roles",
                "FormGenerarReporte",
                detalle: $"Reporte de matriz generado: {Path.GetFileName(rutaArchivo)}"
            );
        }

        private void GenerarSoloExcepciones(string rutaArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
                CrearHojaSoloExcepciones(workbook);
                workbook.SaveAs(rutaArchivo);
            }

            AuditoriaHelper.RegistrarAccion(
                SesionActual.UsuarioID,
                "GENERAR_REPORTE_EXCEPCIONES",
                "SISTEMA",
                "Gestión de Roles",
                "FormGenerarReporte",
                detalle: $"Reporte de excepciones generado: {Path.GetFileName(rutaArchivo)}"
            );
        }

        private void GenerarRolEspecifico(string rutaArchivo)
        {
            using (var workbook = new XLWorkbook())
            {
                var rolSeleccionado = cmbRolEspecifico.SelectedItem as ComboBoxRol;
                CrearHojaDetalleRol(workbook, rolSeleccionado.RolID, rolSeleccionado.NombreRol);
                workbook.SaveAs(rutaArchivo);
            }

            var rol = cmbRolEspecifico.SelectedItem as ComboBoxRol;
            AuditoriaHelper.RegistrarAccion(
                SesionActual.UsuarioID,
                "GENERAR_REPORTE_ROL",
                "SISTEMA",
                "Gestión de Roles",
                "FormGenerarReporte",
                registroID: rol.RolID,
                detalle: $"Reporte del rol '{rol.NombreRol}' generado: {Path.GetFileName(rutaArchivo)}"
            );
        }

        #region Generación de Hojas

        private void CrearHojaResumen(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Resumen General");

            // Configurar ancho de columnas
            worksheet.Column(1).Width = 25;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 15;
            worksheet.Column(6).Width = 15;

            // ====================
            // TÍTULO PRINCIPAL
            // ====================
            var tituloRange = worksheet.Range("A1:F2");
            tituloRange.Merge();
            tituloRange.Value = "REPORTE GENERAL DE PERMISOS";
            tituloRange.Style.Font.Bold = true;
            tituloRange.Style.Font.FontSize = 20;
            tituloRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
            tituloRange.Style.Font.FontColor = XLColor.White;
            tituloRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            tituloRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            tituloRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            // Subtítulo
            var subtituloRange = worksheet.Range("A3:F3");
            subtituloRange.Merge();
            subtituloRange.Value = "MOFIS ERP - Sistema de Gestión de Permisos";
            subtituloRange.Style.Font.FontSize = 12;
            subtituloRange.Style.Font.Italic = true;
            subtituloRange.Style.Fill.BackgroundColor = XLColor.FromArgb(230, 230, 230);
            subtituloRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Fecha y usuario
            worksheet.Cell("A5").Value = $"📅 Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Cell("A5").Style.Font.FontSize = 11;
            worksheet.Cell("A5").Style.Font.Bold = true;

            worksheet.Cell("D5").Value = $"👤 Generado por: {SesionActual.Username}";
            worksheet.Cell("D5").Style.Font.FontSize = 11;
            worksheet.Cell("D5").Style.Font.Bold = true;

            int fila = 7;

            // ====================
            // ESTADÍSTICAS GLOBALES
            // ====================
            var tituloEstadisticas = worksheet.Range(fila, 1, fila, 6);
            tituloEstadisticas.Merge();
            tituloEstadisticas.Value = "📊 ESTADÍSTICAS GLOBALES";
            tituloEstadisticas.Style.Font.Bold = true;
            tituloEstadisticas.Style.Font.FontSize = 16;
            tituloEstadisticas.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 240);
            tituloEstadisticas.Style.Font.FontColor = XLColor.White;
            tituloEstadisticas.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            tituloEstadisticas.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            fila += 2;

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Crear tarjetas de estadísticas
                // Total de roles
                string sqlRoles = "SELECT COUNT(*) FROM Roles WHERE Activo = 1";
                using (var cmd = new SqlCommand(sqlRoles, conn))
                {
                    int totalRoles = (int)cmd.ExecuteScalar();
                    CrearTarjetaEstadistica(worksheet, fila, 1, "Roles Activos", totalRoles.ToString(), XLColor.FromArgb(52, 168, 83));
                }

                // Total de usuarios
                string sqlUsuarios = "SELECT COUNT(*) FROM Usuarios WHERE Activo = 1 AND EsEliminado = 0";
                using (var cmd = new SqlCommand(sqlUsuarios, conn))
                {
                    int totalUsuarios = (int)cmd.ExecuteScalar();
                    CrearTarjetaEstadistica(worksheet, fila, 3, "Usuarios Activos", totalUsuarios.ToString(), XLColor.FromArgb(66, 133, 244));
                }

                // Total de formularios
                string sqlForms = "SELECT COUNT(*) FROM CatalogoFormularios WHERE Activo = 1";
                using (var cmd = new SqlCommand(sqlForms, conn))
                {
                    int totalForms = (int)cmd.ExecuteScalar();
                    CrearTarjetaEstadistica(worksheet, fila, 5, "Formularios", totalForms.ToString(), XLColor.FromArgb(251, 188, 5));
                }

                fila += 4;

                // ====================
                // RESUMEN POR ROL
                // ====================
                var tituloRoles = worksheet.Range(fila, 1, fila, 6);
                tituloRoles.Merge();
                tituloRoles.Value = "📋 RESUMEN POR ROL";
                tituloRoles.Style.Font.Bold = true;
                tituloRoles.Style.Font.FontSize = 16;
                tituloRoles.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 240);
                tituloRoles.Style.Font.FontColor = XLColor.White;
                tituloRoles.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                tituloRoles.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                fila += 2;

                // Encabezados de tabla
                worksheet.Cell(fila, 1).Value = "Rol";
                worksheet.Cell(fila, 2).Value = "Permisos Activos";
                worksheet.Cell(fila, 3).Value = "Total Permisos";
                worksheet.Cell(fila, 4).Value = "% Activados";
                worksheet.Cell(fila, 5).Value = "Usuarios";
                worksheet.Cell(fila, 6).Value = "Estado";

                var headerRange = worksheet.Range(fila, 1, fila, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontSize = 12;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                fila++;

                string sqlResumen = @"
            SELECT 
                R.NombreRol,
                COUNT(CASE WHEN PR.Permitido = 1 THEN 1 END) AS PermisosActivos,
                COUNT(*) AS TotalPermisos,
                (SELECT COUNT(*) FROM Usuarios WHERE RolID = R.RolID AND Activo = 1 AND EsEliminado = 0) AS Usuarios
            FROM Roles R
            LEFT JOIN PermisosRol PR ON R.RolID = PR.RolID
            WHERE R.Activo = 1
            GROUP BY R.RolID, R.NombreRol
            ORDER BY R.NombreRol";

                using (var cmd = new SqlCommand(sqlResumen, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string rol = reader.GetString(0);
                        int activos = reader.GetInt32(1);
                        int total = reader.GetInt32(2);
                        int usuarios = reader.GetInt32(3);
                        double porcentaje = total > 0 ? (activos * 100.0 / total) : 0;

                        worksheet.Cell(fila, 1).Value = rol;
                        worksheet.Cell(fila, 2).Value = activos;
                        worksheet.Cell(fila, 3).Value = total;
                        worksheet.Cell(fila, 4).Value = $"{porcentaje:F1}%";
                        worksheet.Cell(fila, 5).Value = usuarios;

                        // Estado visual
                        string estado = porcentaje >= 80 ? "✅ Completo" :
                                       porcentaje >= 50 ? "⚠️ Medio" :
                                       "❌ Bajo";
                        worksheet.Cell(fila, 6).Value = estado;

                        // Colorear fila según porcentaje
                        if (chkIncluirColores.Checked)
                        {
                            XLColor color = porcentaje >= 80 ? XLColor.FromArgb(200, 255, 200) :
                                           porcentaje >= 50 ? XLColor.FromArgb(255, 243, 205) :
                                           XLColor.FromArgb(255, 200, 200);

                            var rowRange = worksheet.Range(fila, 1, fila, 6);
                            rowRange.Style.Fill.BackgroundColor = color;
                        }

                        worksheet.Cell(fila, 1).Style.Font.FontSize = 11;
                        worksheet.Cell(fila, 1).Style.Font.Bold = true;

                        var dataRange = worksheet.Range(fila, 1, fila, 6);
                        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(fila, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        fila++;
                    }
                }

                fila += 2;

                // ====================
                // USUARIOS CON EXCEPCIONES
                // ====================
                string sqlExcepciones = @"
            SELECT 
                U.NombreCompleto,
                R.NombreRol,
                COUNT(*) AS CantidadExcepciones
            FROM PermisosUsuario PU
            INNER JOIN Usuarios U ON PU.UsuarioID = U.UsuarioID
            INNER JOIN Roles R ON U.RolID = R.RolID
            WHERE U.Activo = 1 AND U.EsEliminado = 0
            GROUP BY U.UsuarioID, U.NombreCompleto, R.NombreRol
            ORDER BY CantidadExcepciones DESC";

                var dtExcepciones = new DataTable();
                using (var adapter = new SqlDataAdapter(sqlExcepciones, conn))
                {
                    adapter.Fill(dtExcepciones);
                }

                if (dtExcepciones.Rows.Count > 0)
                {
                    var tituloExcepciones = worksheet.Range(fila, 1, fila, 6);
                    tituloExcepciones.Merge();
                    tituloExcepciones.Value = "⚠️ USUARIOS CON EXCEPCIONES";
                    tituloExcepciones.Style.Font.Bold = true;
                    tituloExcepciones.Style.Font.FontSize = 16;
                    tituloExcepciones.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 152, 0);
                    tituloExcepciones.Style.Font.FontColor = XLColor.White;
                    tituloExcepciones.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    tituloExcepciones.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    fila += 2;

                    // Encabezados
                    worksheet.Cell(fila, 1).Value = "Usuario";
                    worksheet.Cell(fila, 2).Value = "Rol";
                    worksheet.Cell(fila, 3).Value = "Excepciones";

                    var excHeaderRange = worksheet.Range(fila, 1, fila, 3); 
                    excHeaderRange.Style.Font.Bold = true;
                    excHeaderRange.Style.Font.FontSize = 12;
                    excHeaderRange.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 152, 0);
                    excHeaderRange.Style.Font.FontColor = XLColor.White;
                    excHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    fila++;

                    foreach (DataRow row in dtExcepciones.Rows)
                    {
                        worksheet.Cell(fila, 1).Value = row["NombreCompleto"].ToString();
                        worksheet.Cell(fila, 2).Value = row["NombreRol"].ToString();
                        worksheet.Cell(fila, 3).Value = row["CantidadExcepciones"].ToString();

                        worksheet.Cell(fila, 1).Style.Font.FontSize = 11;

                        if (chkIncluirColores.Checked)
                        {
                            var excRowRange = worksheet.Range(fila, 1, fila, 3);
                            excRowRange.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 243, 205);
                        }

                        var excDataRange = worksheet.Range(fila, 1, fila, 3);
                        excDataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        fila++;
                    }
                }
            }

            // Ajustar todas las filas de altura
            worksheet.Rows().Height = 20;
        }

        // Método auxiliar para crear tarjetas de estadísticas
        private void CrearTarjetaEstadistica(IXLWorksheet worksheet, int fila, int col, string titulo, string valor, XLColor color)
        {
            // Celda de título
            var celdaTitulo = worksheet.Cell(fila, col);
            celdaTitulo.Value = titulo;
            celdaTitulo.Style.Font.FontSize = 10;
            celdaTitulo.Style.Font.Bold = true;
            celdaTitulo.Style.Fill.BackgroundColor = color;
            celdaTitulo.Style.Font.FontColor = XLColor.White;
            celdaTitulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            celdaTitulo.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Celda de valor
            var celdaValor = worksheet.Cell(fila + 1, col);
            celdaValor.Value = valor;
            celdaValor.Style.Font.FontSize = 18;
            celdaValor.Style.Font.Bold = true;
            celdaValor.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            celdaValor.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Merge para el valor (2 filas de altura)
            var rangoValor = worksheet.Range(fila + 1, col, fila + 2, col);
            rangoValor.Merge();
            rangoValor.Style.Font.FontSize = 18;
            rangoValor.Style.Font.Bold = true;
            rangoValor.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangoValor.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rangoValor.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        private void CrearHojaMatrizCompleta(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Matriz Completa");

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Obtener roles activos
                var roles = new DataTable();
                string sqlRoles = "SELECT RolID, NombreRol FROM Roles WHERE Activo = 1 ORDER BY NombreRol";
                using (var adapter = new SqlDataAdapter(sqlRoles, conn))
                {
                    adapter.Fill(roles);
                }

                // Obtener todos los formularios con sus permisos
                string sqlPermisos = @"
                    SELECT 
                        C.NombreCategoria,
                        M.NombreModulo,
                        F.NombreFormulario,
                        A.CodigoAccion,
                        A.NombreAccion,
                        R.RolID,
                        ISNULL(PR.Permitido, 0) AS Permitido
                    FROM CatalogoFormularios F
                    INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                    INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                    CROSS JOIN CatalogoAcciones A
                    CROSS JOIN Roles R
                    LEFT JOIN PermisosRol PR ON PR.FormularioID = F.FormularioID 
                        AND PR.AccionID = A.AccionID 
                        AND PR.RolID = R.RolID
                    WHERE F.Activo = 1 
                        AND A.Activo = 1 
                        AND R.Activo = 1
                        AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
                    ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion, A.OrdenVisualizacion";

                var dtPermisos = new DataTable();
                using (var adapter = new SqlDataAdapter(sqlPermisos, conn))
                {
                    adapter.Fill(dtPermisos);
                }

                // Encabezados
                int col = 1;
                worksheet.Cell(1, col++).Value = "Categoría";
                worksheet.Cell(1, col++).Value = "Módulo";
                worksheet.Cell(1, col++).Value = "Formulario";
                worksheet.Cell(1, col++).Value = "Acción";

                foreach (DataRow rolRow in roles.Rows)
                {
                    worksheet.Cell(1, col++).Value = rolRow["NombreRol"].ToString();
                }

                // Estilo de encabezados
                var headerRange = worksheet.Range(1, 1, 1, col - 1);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Datos
                int fila = 2;
                var agrupado = dtPermisos.AsEnumerable()
                    .GroupBy(r => new
                    {
                        Categoria = r.Field<string>("NombreCategoria"),
                        Modulo = r.Field<string>("NombreModulo"),
                        Formulario = r.Field<string>("NombreFormulario"),
                        Accion = r.Field<string>("NombreAccion")
                    });

                foreach (var grupo in agrupado)
                {
                    col = 1;
                    worksheet.Cell(fila, col++).Value = grupo.Key.Categoria;
                    worksheet.Cell(fila, col++).Value = grupo.Key.Modulo;
                    worksheet.Cell(fila, col++).Value = grupo.Key.Formulario;
                    worksheet.Cell(fila, col++).Value = grupo.Key.Accion;

                    foreach (DataRow rolRow in roles.Rows)
                    {
                        int rolID = (int)rolRow["RolID"];
                        var permiso = grupo.FirstOrDefault(r => r.Field<int>("RolID") == rolID);
                        bool permitido = permiso != null && permiso.Field<bool>("Permitido");

                        var cell = worksheet.Cell(fila, col);
                        cell.Value = permitido ? "✓" : "✗";

                        if (chkIncluirColores.Checked)
                        {
                            cell.Style.Fill.BackgroundColor = permitido
                                ? XLColor.FromArgb(200, 255, 200)  // Verde claro
                                : XLColor.FromArgb(255, 200, 200); // Rojo claro
                        }

                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        col++;
                    }

                    fila++;
                }
            }

            // Aumentar tamaño de fuente en todas las celdas
            worksheet.Cells().Style.Font.FontSize = 14;

            // Ajustar columnas
            worksheet.Columns().AdjustToContents();
        }

        private void CrearHojaDetallePorRoles(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Detalle por Roles");

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        R.NombreRol,
                        C.NombreCategoria,
                        M.NombreModulo,
                        F.NombreFormulario,
                        MAX(CASE WHEN A.CodigoAccion = 'VIEW' THEN PR.Permitido ELSE 0 END) AS Ver,
                        MAX(CASE WHEN A.CodigoAccion = 'CREATE' THEN PR.Permitido ELSE 0 END) AS Crear,
                        MAX(CASE WHEN A.CodigoAccion = 'EDIT' THEN PR.Permitido ELSE 0 END) AS Editar,
                        MAX(CASE WHEN A.CodigoAccion = 'DELETE' THEN PR.Permitido ELSE 0 END) AS Eliminar,
                        MAX(CASE WHEN A.CodigoAccion = 'PRINT' THEN PR.Permitido ELSE 0 END) AS Imprimir,
                        MAX(CASE WHEN A.CodigoAccion = 'EXPORT' THEN PR.Permitido ELSE 0 END) AS Exportar
                    FROM Roles R
                    CROSS JOIN CatalogoFormularios F
                    INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                    INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                    CROSS JOIN CatalogoAcciones A
                    LEFT JOIN PermisosRol PR ON PR.RolID = R.RolID 
                        AND PR.FormularioID = F.FormularioID 
                        AND PR.AccionID = A.AccionID
                    WHERE R.Activo = 1 
                        AND F.Activo = 1 
                        AND A.Activo = 1
                        AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
                    GROUP BY R.RolID, R.NombreRol, C.OrdenVisualizacion, C.NombreCategoria, 
                             M.OrdenVisualizacion, M.NombreModulo, F.OrdenVisualizacion, F.NombreFormulario
                    ORDER BY R.NombreRol, C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion";

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(sql, conn))
                {
                    adapter.Fill(dt);
                }

                // Encabezados
                string[] headers = { "Rol", "Categoría", "Módulo", "Formulario", "Ver", "Crear", "Editar", "Eliminar", "Imprimir", "Exportar" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }

                var headerRange = worksheet.Range(1, 1, 1, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                headerRange.Style.Font.FontColor = XLColor.White;

                // Datos
                int fila = 2;
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        worksheet.Cell(fila, i + 1).Value = row[i].ToString();
                    }

                    for (int i = 4; i < 10; i++)
                    {
                        bool permitido = row[i] != DBNull.Value && Convert.ToBoolean(row[i]);
                        var cell = worksheet.Cell(fila, i + 1);
                        cell.Value = permitido ? "✓" : "✗";

                        if (chkIncluirColores.Checked)
                        {
                            cell.Style.Fill.BackgroundColor = permitido
                                ? XLColor.FromArgb(200, 255, 200)
                                : XLColor.FromArgb(255, 200, 200);
                        }

                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    fila++;
                }
            }

            // Aumentar tamaño de fuente en todas las celdas
            worksheet.Cells().Style.Font.FontSize = 11;

            worksheet.Columns().AdjustToContents();
        }

        private void CrearHojaDetallePorUsuarios(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Detalle por Usuarios");

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        U.Username,
                        U.NombreCompleto,
                        R.NombreRol,
                        C.NombreCategoria,
                        M.NombreModulo,
                        F.NombreFormulario,
                        MAX(CASE WHEN A.CodigoAccion = 'VIEW' THEN COALESCE(PU.Permitido, PR.Permitido, 0) ELSE 0 END) AS Ver,
                        MAX(CASE WHEN A.CodigoAccion = 'CREATE' THEN COALESCE(PU.Permitido, PR.Permitido, 0) ELSE 0 END) AS Crear,
                        MAX(CASE WHEN A.CodigoAccion = 'EDIT' THEN COALESCE(PU.Permitido, PR.Permitido, 0) ELSE 0 END) AS Editar,
                        MAX(CASE WHEN A.CodigoAccion = 'DELETE' THEN COALESCE(PU.Permitido, PR.Permitido, 0) ELSE 0 END) AS Eliminar,
                        MAX(CASE WHEN A.CodigoAccion = 'PRINT' THEN COALESCE(PU.Permitido, PR.Permitido, 0) ELSE 0 END) AS Imprimir,
                        MAX(CASE WHEN A.CodigoAccion = 'EXPORT' THEN COALESCE(PU.Permitido, PR.Permitido, 0) ELSE 0 END) AS Exportar,
                        CAST(MAX(CASE WHEN PU.PermisoUsuarioID IS NOT NULL THEN 1 ELSE 0 END) AS BIT) AS TieneExcepcion
                    FROM Usuarios U
                    INNER JOIN Roles R ON U.RolID = R.RolID
                    CROSS JOIN CatalogoFormularios F
                    INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                    INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                    CROSS JOIN CatalogoAcciones A
                    LEFT JOIN PermisosRol PR ON PR.RolID = R.RolID 
                        AND PR.FormularioID = F.FormularioID 
                        AND PR.AccionID = A.AccionID
                    LEFT JOIN PermisosUsuario PU ON PU.UsuarioID = U.UsuarioID
                        AND PU.FormularioID = F.FormularioID
                        AND PU.AccionID = A.AccionID
                    WHERE U.Activo = 1 
                        AND U.EsEliminado = 0
                        AND F.Activo = 1 
                        AND A.Activo = 1
                        AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
                    GROUP BY U.UsuarioID, U.Username, U.NombreCompleto, R.NombreRol,
                             C.OrdenVisualizacion, C.NombreCategoria, 
                             M.OrdenVisualizacion, M.NombreModulo, 
                             F.OrdenVisualizacion, F.NombreFormulario
                    ORDER BY U.NombreCompleto, C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion";

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(sql, conn))
                {
                    adapter.Fill(dt);
                }

                // Encabezados
                string[] headers = { "Usuario", "Nombre Completo", "Rol", "Categoría", "Módulo", "Formulario", "Ver", "Crear", "Editar", "Eliminar", "Imprimir", "Exportar", "Excep." };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }

                var headerRange = worksheet.Range(1, 1, 1, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                headerRange.Style.Font.FontColor = XLColor.White;

                // Datos
                int fila = 2;
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        worksheet.Cell(fila, i + 1).Value = row[i].ToString();
                    }

                    bool tieneExcepcion = Convert.ToBoolean(row["TieneExcepcion"]);

                    for (int i = 6; i < 12; i++)
                    {
                        bool permitido = row[i] != DBNull.Value && Convert.ToBoolean(row[i]);
                        var cell = worksheet.Cell(fila, i + 1);
                        cell.Value = permitido ? "✓" : "✗";

                        if (chkIncluirColores.Checked)
                        {
                            if (tieneExcepcion)
                            {
                                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 243, 205); // Naranja (excepción)
                            }
                            else
                            {
                                cell.Style.Fill.BackgroundColor = permitido
                                    ? XLColor.FromArgb(200, 255, 200)
                                    : XLColor.FromArgb(255, 200, 200);
                            }
                        }

                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    worksheet.Cell(fila, 13).Value = tieneExcepcion ? "⚠️" : "";
                    worksheet.Cell(fila, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    fila++;
                }
            }

            // Aumentar tamaño de fuente en todas las celdas
            worksheet.Cells().Style.Font.FontSize = 11;

            worksheet.Columns().AdjustToContents();
        }

        private void CrearHojaSoloExcepciones(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Solo Excepciones");

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        U.Username,
                        U.NombreCompleto,
                        R.NombreRol,
                        C.NombreCategoria,
                        M.NombreModulo,
                        F.NombreFormulario,
                        A.NombreAccion,
                        ISNULL(PR.Permitido, 0) AS PermisoRol,
                        PU.Permitido AS PermisoUsuario
                    FROM PermisosUsuario PU
                    INNER JOIN Usuarios U ON PU.UsuarioID = U.UsuarioID
                    INNER JOIN Roles R ON U.RolID = R.RolID
                    INNER JOIN CatalogoFormularios F ON PU.FormularioID = F.FormularioID
                    INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                    INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                    INNER JOIN CatalogoAcciones A ON PU.AccionID = A.AccionID
                    LEFT JOIN PermisosRol PR ON PR.RolID = R.RolID 
                        AND PR.FormularioID = F.FormularioID 
                        AND PR.AccionID = A.AccionID
                    WHERE U.Activo = 1 
                        AND U.EsEliminado = 0
                        AND F.Activo = 1
                    ORDER BY U.NombreCompleto, C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion, A.OrdenVisualizacion";

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(sql, conn))
                {
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    worksheet.Cell(1, 1).Value = "No hay usuarios con excepciones de permisos.";
                    return;
                }

                // Encabezados
                string[] headers = { "Usuario", "Nombre Completo", "Rol", "Categoría", "Módulo", "Formulario", "Acción", "Heredado", "Personal", "Diferencia" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }

                var headerRange = worksheet.Range(1, 1, 1, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                headerRange.Style.Font.FontColor = XLColor.White;

                // Datos
                int fila = 2;
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        worksheet.Cell(fila, i + 1).Value = row[i].ToString();
                    }

                    bool permisoRol = Convert.ToBoolean(row["PermisoRol"]);
                    bool permisoUsuario = Convert.ToBoolean(row["PermisoUsuario"]);

                    worksheet.Cell(fila, 8).Value = permisoRol ? "✓" : "✗";
                    worksheet.Cell(fila, 9).Value = permisoUsuario ? "✓" : "✗";
                    worksheet.Cell(fila, 10).Value = permisoRol != permisoUsuario ? "⚠️ Difiere" : "Igual";

                    if (chkIncluirColores.Checked && permisoRol != permisoUsuario)
                    {
                        var rangoFila = worksheet.Range(fila, 1, fila, 10);
                        rangoFila.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 243, 205);
                    }

                    worksheet.Cell(fila, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(fila, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    fila++;
                }
            }

            // Aumentar tamaño de fuente en todas las celdas
            worksheet.Cells().Style.Font.FontSize = 11;

            worksheet.Columns().AdjustToContents();
        }

        private void CrearHojaDetalleRol(XLWorkbook workbook, int rolID, string nombreRol)
        {
            var worksheet = workbook.Worksheets.Add($"Detalle {nombreRol}");

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Título y resumen
                worksheet.Cell(1, 1).Value = $"PERMISOS DEL ROL: {nombreRol}";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                var tituloRange = worksheet.Range("A1:F1");
                tituloRange.Merge();

                worksheet.Cell(2, 1).Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";

                // Obtener estadísticas del rol
                string sqlStats = @"
                    SELECT 
                        COUNT(CASE WHEN PR.Permitido = 1 THEN 1 END) AS Activos,
                        COUNT(*) AS Total,
                        (SELECT COUNT(*) FROM Usuarios WHERE RolID = @RolID AND Activo = 1 AND EsEliminado = 0) AS Usuarios
                    FROM PermisosRol PR
                    WHERE PR.RolID = @RolID";

                using (var cmd = new SqlCommand(sqlStats, conn))
                {
                    cmd.Parameters.AddWithValue("@RolID", rolID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int activos = reader.GetInt32(0);
                            int total = reader.GetInt32(1);
                            int usuarios = reader.GetInt32(2);
                            double porcentaje = total > 0 ? (activos * 100.0 / total) : 0;

                            worksheet.Cell(3, 1).Value = $"Total de Permisos: {activos} / {total} ({porcentaje:F1}%)";
                            worksheet.Cell(4, 1).Value = $"Usuarios con este rol: {usuarios}";
                        }
                    }
                }

                // Permisos detallados
                string sql = @"
                    SELECT 
                        C.NombreCategoria,
                        M.NombreModulo,
                        F.NombreFormulario,
                        MAX(CASE WHEN A.CodigoAccion = 'VIEW' THEN ISNULL(PR.Permitido, 0) ELSE 0 END) AS Ver,
                        MAX(CASE WHEN A.CodigoAccion = 'CREATE' THEN ISNULL(PR.Permitido, 0) ELSE 0 END) AS Crear,
                        MAX(CASE WHEN A.CodigoAccion = 'EDIT' THEN ISNULL(PR.Permitido, 0) ELSE 0 END) AS Editar,
                        MAX(CASE WHEN A.CodigoAccion = 'DELETE' THEN ISNULL(PR.Permitido, 0) ELSE 0 END) AS Eliminar,
                        MAX(CASE WHEN A.CodigoAccion = 'PRINT' THEN ISNULL(PR.Permitido, 0) ELSE 0 END) AS Imprimir,
                        MAX(CASE WHEN A.CodigoAccion = 'EXPORT' THEN ISNULL(PR.Permitido, 0) ELSE 0 END) AS Exportar
                    FROM CatalogoFormularios F
                    INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                    INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                    CROSS JOIN CatalogoAcciones A
                    LEFT JOIN PermisosRol PR ON PR.FormularioID = F.FormularioID 
                        AND PR.AccionID = A.AccionID 
                        AND PR.RolID = @RolID
                    WHERE F.Activo = 1 
                        AND A.Activo = 1
                        AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
                    GROUP BY C.OrdenVisualizacion, C.NombreCategoria, 
                             M.OrdenVisualizacion, M.NombreModulo, 
                             F.OrdenVisualizacion, F.NombreFormulario
                    ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion";

                var dt = new DataTable();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RolID", rolID);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                // Encabezados
                int headerRow = 6;
                string[] headers = { "Categoría", "Módulo", "Formulario", "Ver", "Crear", "Editar", "Eliminar", "Imprimir", "Exportar" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(headerRow, i + 1).Value = headers[i];
                }

                var headerRange = worksheet.Range(headerRow, 1, headerRow, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                headerRange.Style.Font.FontColor = XLColor.White;

                // Datos
                int fila = headerRow + 1;
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        worksheet.Cell(fila, i + 1).Value = row[i].ToString();
                    }

                    for (int i = 3; i < 9; i++)
                    {
                        bool permitido = row[i] != DBNull.Value && Convert.ToBoolean(row[i]);
                        var cell = worksheet.Cell(fila, i + 1);
                        cell.Value = permitido ? "✓" : "✗";

                        if (chkIncluirColores.Checked)
                        {
                            cell.Style.Fill.BackgroundColor = permitido
                                ? XLColor.FromArgb(200, 255, 200)
                                : XLColor.FromArgb(255, 200, 200);
                        }

                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    fila++;
                }
            }

            // Aumentar tamaño de fuente en todas las celdas
            worksheet.Cells().Style.Font.FontSize = 11;

            worksheet.Columns().AdjustToContents();
        }

        #endregion

        // Clase auxiliar
        private class ComboBoxRol
        {
            public int RolID { get; set; }
            public string NombreRol { get; set; }
        }
    }
}