using System;
using System.Data;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    /// <summary>
    /// Generador profesional de reportes PDF usando QuestPDF
    /// CORREGIDO: Compatible con C# 7.3 y sin conflictos de color
    /// </summary>
    public class PDFReporteGenerator
    {
        private readonly DataView datos;
        private readonly ConfiguracionReportePDF config;
        private readonly string rutaArchivo;

        // Información de filtros aplicados
        private readonly DateTime? fechaDesde;
        private readonly DateTime? fechaHasta;
        private readonly string moduloFiltrado;
        private readonly string usuarioFiltrado;
        private readonly string accionFiltrada;
        private readonly string textoBuscado;

        // ✅ CORRECCIÓN: Colores como constantes string (compatible con C# 7.3)
        private const string ColorAzul = "#0078D4";
        private const string ColorVerde = "#228B22";
        private const string ColorRojo = "#DC3545";
        private const string ColorGris = "#6C757D";
        private const string ColorGrisClaro = "#F0F0F0";

        public PDFReporteGenerator(
            DataView datos,
            ConfiguracionReportePDF config,
            string rutaArchivo,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string moduloFiltrado = null,
            string usuarioFiltrado = null,
            string accionFiltrada = null,
            string textoBuscado = null)
        {
            this.datos = datos;
            this.config = config;
            this.rutaArchivo = rutaArchivo;
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.moduloFiltrado = moduloFiltrado;
            this.usuarioFiltrado = usuarioFiltrado;
            this.accionFiltrada = accionFiltrada;
            this.textoBuscado = textoBuscado;

            // Configurar licencia de QuestPDF (Community License)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Genera el PDF completo según la configuración
        /// </summary>
        public void Generar()
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configurar página
                    ConfigurarPagina(page);

                    // Header
                    page.Header().Element(ComposeHeader);

                    // Contenido principal
                    page.Content().Element(ComposeContent);

                    // Footer
                    page.Footer().Element(ComposeFooter);
                });
            })
            .GeneratePdf(rutaArchivo);
        }

        private void ConfigurarPagina(PageDescriptor page)
        {
            // Tamaño de página
            PageSize tamanoPagina;
            switch (config.TamanoPagina)
            {
                case TamanoPagina.A4:
                    tamanoPagina = PageSizes.A4;
                    break;
                case TamanoPagina.Carta:
                    tamanoPagina = PageSizes.Letter;
                    break;
                case TamanoPagina.Legal:
                    tamanoPagina = PageSizes.Legal;
                    break;
                default:
                    tamanoPagina = PageSizes.A4;
                    break;
            }

            page.Size(tamanoPagina);

            // Orientación
            if (config.Orientacion == OrientacionPagina.Horizontal)
            {
                page.Size(tamanoPagina.Landscape());
            }

            // Márgenes
            page.Margin(40);

            // Color de fondo
            page.PageColor(Colors.White);
        }

        // ============================================
        // HEADER - Aparece en todas las hojas
        // ============================================
        private void ComposeHeader(IContainer container)
        {
            container
                .Height(60)
                .PaddingBottom(5)
                .Row(row =>
                {
                    // Izquierda: Títulos del sistema
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("MOFIS ERP")
                            .FontSize(16)
                            .SemiBold()
                            .FontColor(ColorAzul);

                        column.Item().Text("Sistema de Auditoría")
                            .FontSize(10)
                            .FontColor(ColorGris);
                    });

                    // Derecha: Logo corporativo
                    string rutaLogo = ObtenerRutaLogo();
                    if (!string.IsNullOrEmpty(rutaLogo))
                    {
                        row.ConstantItem(80)
                            .AlignRight()
                            .AlignMiddle()
                            .Image(rutaLogo)
                            .FitArea();
                    }
                    else
                    {
                        row.ConstantItem(80);
                    }
                });
        }

        // ============================================
        // CONTENT
        // ============================================
        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // Portada
                if (config.IncluirPortada)
                {
                    column.Item().Element(ComposePortada);
                    column.Item().PageBreak();
                }

                // Índice
                if (config.IncluirIndice)
                {
                    column.Item().Element(ComposeIndice);
                    column.Item().PageBreak();
                }

                // Resumen Ejecutivo
                if (config.IncluirResumenEjecutivo)
                {
                    column.Item().Element(ComposeResumenEjecutivo);
                    column.Item().PageBreak();
                }

                // Criterios de Búsqueda
                if (config.IncluirCriteriosBusqueda)
                {
                    column.Item().Element(ComposeCriteriosBusqueda);
                    column.Item().PageBreak();
                }

                // Detalle de Registros
                if (config.IncluirDetalleRegistros)
                {
                    column.Item().Element(ComposeDetalleRegistros);
                    column.Item().PageBreak();
                }

                // Firmas
                if (config.IncluirFirmas)
                {
                    column.Item().Element(ComposeFirmas);
                }
            });
        }

        // ============================================
        // FOOTER - ✅ CORREGIDO
        // ============================================
        private void ComposeFooter(IContainer container)
        {
            // ✅ CORRECCIÓN: No intentar capturar el resultado de Text()
            container.AlignCenter().Text(text =>
            {
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
                text.DefaultTextStyle(style => style.FontSize(9).FontColor(ColorGris));
            });
        }

        // ============================================
        // PORTADA
        // ============================================
        // ============================================
        // PORTADA
        // ============================================
        private void ComposePortada(IContainer container)
        {
            container.Column(column =>
            {
                // Espaciado superior
                column.Item().Height(100);

                // Título principal
                column.Item().AlignCenter().Text("MOFIS ERP")
                    .FontSize(48)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(10);

                // Subtítulo
                column.Item().AlignCenter().Text("REPORTE DE AUDITORÍA DEL SISTEMA")
                    .FontSize(24)
                    .SemiBold();

                column.Item().PaddingVertical(30);

                // Información
                column.Item().AlignCenter().Column(info =>
                {
                    info.Item().Text(string.Format("Periodo: {0}", ObtenerPeriodoTexto()))
                        .FontSize(14);

                    info.Item().PaddingVertical(5);

                    info.Item().Text(string.Format("Generado: {0:dd/MM/yyyy HH:mm:ss}", DateTime.Now))
                        .FontSize(12);

                    info.Item().Text(string.Format("Por: {0} ({1})", SesionActual.NombreCompleto, SesionActual.Username))
                        .FontSize(12);
                });

                column.Item().PaddingVertical(50);

                // Confidencial
                column.Item().AlignCenter().Border(2, ColorRojo).Padding(10)
                    .Text("CONFIDENCIAL")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorRojo);
            });
        }

        // ============================================
        // HELPERS
        // ============================================

        /// <summary>
        /// Obtiene la ruta completa del logo desde la carpeta Resources
        /// </summary>
        private string ObtenerRutaLogo()
        {
            try
            {
                // Obtener el directorio base de la aplicación
                string directorioBase = AppDomain.CurrentDomain.BaseDirectory;

                // Construir ruta al logo
                string rutaLogo = Path.Combine(directorioBase, "Resources", "MOFIS ERP -LOGO.png");

                // Verificar si existe
                if (File.Exists(rutaLogo))
                {
                    return rutaLogo;
                }

                // Intentar ruta alternativa (en caso de que Resources esté en el directorio padre)
                rutaLogo = Path.Combine(Directory.GetParent(directorioBase).FullName, "Resources", "MOFIS ERP -LOGO.png");

                if (File.Exists(rutaLogo))
                {
                    return rutaLogo;
                }

                // Si no se encuentra, retornar null (no se mostrará el logo pero no causará error)
                return null;
            }
            catch
            {
                return null;
            }
        }

        // ============================================
        // ÍNDICE
        // ============================================
        private void ComposeIndice(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("ÍNDICE")
                    .FontSize(24)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(20);

                // Tabla de contenidos
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.ConstantColumn(50);
                    });

                    int pagina = 3;
                    AgregarLineaIndice(table, "1. Resumen Ejecutivo", pagina++);
                    AgregarLineaIndice(table, "2. Criterios de Filtrado", pagina++);
                    AgregarLineaIndice(table, "3. Detalle de Registros", pagina++);
                    AgregarLineaIndice(table, "4. Firmas y Aprobaciones", pagina++);
                });
            });
        }

        private void AgregarLineaIndice(TableDescriptor table, string seccion, int pagina)
        {
            table.Cell().PaddingVertical(5).Text(seccion).FontSize(12);
            table.Cell().AlignRight().PaddingVertical(5).Text(pagina.ToString()).FontSize(12);
        }

        // ============================================
        // RESUMEN EJECUTIVO
        // ============================================
        private void ComposeResumenEjecutivo(IContainer container)
        {
            container.Column(column =>
            {
                // Título
                column.Item().Text("RESUMEN EJECUTIVO")
                    .FontSize(20)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(15);

                // Estadísticas (tarjetas)
                column.Item().Row(row =>
                {
                    // Tarjeta 1: Total de Registros
                    row.RelativeItem().Border(2, ColorAzul).Background(Colors.Blue.Lighten4).Padding(20)
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text(datos.Count.ToString())
                                .FontSize(36)
                                .Bold()
                                .FontColor(ColorAzul);

                            col.Item().AlignCenter().Text("Total de Registros")
                                .FontSize(12);
                        });

                    row.ConstantItem(20); // Espacio

                    // Tarjeta 2: Módulos Únicos
                    var modulosUnicos = datos.ToTable().AsEnumerable()
                        .Select(r => r.Field<string>("Modulo"))
                        .Where(m => !string.IsNullOrEmpty(m))
                        .Distinct()
                        .Count();

                    row.RelativeItem().Border(2, ColorVerde).Background(Colors.Green.Lighten4).Padding(20)
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text(modulosUnicos.ToString())
                                .FontSize(36)
                                .Bold()
                                .FontColor(ColorVerde);

                            col.Item().AlignCenter().Text("Módulos Activos")
                                .FontSize(12);
                        });
                });

                column.Item().PaddingVertical(20);

                // TOP 10 ACCIONES
                if (config.IncluirTop10)
                {
                    column.Item().Element(ComposeTop10Acciones);

                    // ✅ SALTO DE PÁGINA (NUEVO)
                    column.Item().PageBreak();

                    column.Item().Element(ComposeTop10Usuarios);
                }
            });
        }

        private void ComposeTop10Acciones(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("TOP 10 ACCIONES MÁS FRECUENTES")
                    .FontSize(14)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(10);

                var accionesPorTipo = datos.ToTable().AsEnumerable()
                    .GroupBy(r => r.Field<string>("Accion"))
                    .Select(g => new { Accion = g.Key ?? "N/A", Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .Take(10)
                    .ToList();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);  // #
                        columns.RelativeColumn(3);   // Acción
                        columns.RelativeColumn(1);   // Cantidad
                        columns.RelativeColumn(1);   // %
                    });

                    // Headers
                    table.Header(header =>
                    {
                        header.Cell().Background(ColorAzul).Padding(5).Text("#").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Acción").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Cantidad").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("%").FontColor(Colors.White).Bold();
                    });

                    // Datos
                    int ranking = 1;
                    foreach (var item in accionesPorTipo)
                    {
                        double porcentaje = (double)item.Cantidad / datos.Count * 100;

                        // ✅ CORRECCIÓN: Variable explícita para el color
                        string bgColor = (ranking % 2 == 0) ? ColorGrisClaro : Colors.White.ToString();

                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(ranking.ToString());
                        table.Cell().Background(bgColor).Padding(5).Text(item.Accion);
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(item.Cantidad.ToString());
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(string.Format("{0:F2}%", porcentaje));

                        ranking++;
                    }
                });
            });
        }

        private void ComposeTop10Usuarios(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("TOP 10 USUARIOS MÁS ACTIVOS")
                    .FontSize(14)
                    .Bold()
                    .FontColor(ColorVerde);

                column.Item().PaddingVertical(10);

                var usuariosMasActivos = datos.ToTable().AsEnumerable()
                    .GroupBy(r => r.Field<string>("Username") ?? "N/A")
                    .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .Take(10)
                    .ToList();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(ColorVerde).Padding(5).Text("#").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorVerde).Padding(5).Text("Usuario").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorVerde).Padding(5).Text("Acciones").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorVerde).Padding(5).Text("%").FontColor(Colors.White).Bold();
                    });

                    int ranking = 1;
                    foreach (var item in usuariosMasActivos)
                    {
                        double porcentaje = (double)item.Cantidad / datos.Count * 100;

                        string bgColor = (ranking % 2 == 0) ? ColorGrisClaro : Colors.White.ToString();

                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(ranking.ToString());
                        table.Cell().Background(bgColor).Padding(5).Text(item.Usuario);
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(item.Cantidad.ToString());
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(string.Format("{0:F2}%", porcentaje));

                        ranking++;
                    }
                });
            });
        }

        // ============================================
        // CRITERIOS DE BÚSQUEDA
        // ============================================
        private void ComposeCriteriosBusqueda(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("CRITERIOS DE FILTRADO APLICADOS")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(15);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                    });

                    bool hayCriterios = false;

                    if (fechaDesde.HasValue)
                    {
                        AgregarCriterio(table, "Fecha Desde:", fechaDesde.Value.ToString("dd/MM/yyyy"));
                        hayCriterios = true;
                    }

                    if (fechaHasta.HasValue)
                    {
                        AgregarCriterio(table, "Fecha Hasta:", fechaHasta.Value.ToString("dd/MM/yyyy"));
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(moduloFiltrado))
                    {
                        AgregarCriterio(table, "Módulo:", moduloFiltrado);
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(usuarioFiltrado))
                    {
                        AgregarCriterio(table, "Usuario:", usuarioFiltrado);
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(accionFiltrada))
                    {
                        AgregarCriterio(table, "Acción:", accionFiltrada);
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(textoBuscado))
                    {
                        AgregarCriterio(table, "Búsqueda:", textoBuscado);
                        hayCriterios = true;
                    }

                    if (!hayCriterios)
                    {
                        table.Cell().ColumnSpan(2).Padding(10)
                            .Text("Sin filtros aplicados (mostrando todos los registros)")
                            .Italic()
                            .FontColor(ColorGris);
                    }
                });
            });
        }

        private void AgregarCriterio(TableDescriptor table, string etiqueta, string valor)
        {
            table.Cell().Background(ColorGrisClaro).Padding(8).Text(etiqueta).Bold();
            table.Cell().Padding(8).Text(valor);
        }

        // ============================================
        // DETALLE DE REGISTROS
        // ============================================
        private void ComposeDetalleRegistros(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("DETALLE COMPLETO DE REGISTROS")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(15);

                // Limitar a 1000 registros para no saturar el PDF
                int maxRegistros = Math.Min(datos.Count, 1000);

                column.Item().Table(table =>
                {
                    // Definir columnas según orientación
                    if (config.Orientacion == OrientacionPagina.Horizontal)
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.2f); // Fecha/Hora
                            columns.RelativeColumn(1);    // Usuario
                            columns.RelativeColumn(1);    // Módulo
                            columns.RelativeColumn(1.5f); // Formulario
                            columns.RelativeColumn(1.5f); // Acción
                            columns.RelativeColumn(3);    // Detalle
                        });
                    }
                    else
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.5f); // Fecha/Hora
                            columns.RelativeColumn(1);    // Usuario
                            columns.RelativeColumn(1);    // Acción
                            columns.RelativeColumn(3);    // Detalle
                        });
                    }

                    // Headers
                    table.Header(header =>
                    {
                        header.Cell().Background(ColorAzul).Padding(5).Text("Fecha/Hora").FontSize(8).FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Usuario").FontSize(8).FontColor(Colors.White).Bold();

                        if (config.Orientacion == OrientacionPagina.Horizontal)
                        {
                            header.Cell().Background(ColorAzul).Padding(5).Text("Módulo").FontSize(8).FontColor(Colors.White).Bold();
                            header.Cell().Background(ColorAzul).Padding(5).Text("Formulario").FontSize(8).FontColor(Colors.White).Bold();
                        }

                        header.Cell().Background(ColorAzul).Padding(5).Text("Acción").FontSize(8).FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Detalle").FontSize(8).FontColor(Colors.White).Bold();
                    });

                    // Datos
                    for (int i = 0; i < maxRegistros; i++)
                    {
                        DataRowView rowView = datos[i];
                        DataRow row = rowView.Row;

                        string bgColor = (i % 2 == 0) ? Colors.White.ToString() : ColorGrisClaro;

                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "FechaHora")).FontSize(7);
                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Username")).FontSize(7);

                        if (config.Orientacion == OrientacionPagina.Horizontal)
                        {
                            table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Modulo")).FontSize(7);
                            table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Formulario")).FontSize(7);
                        }

                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Accion")).FontSize(7);
                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Detalle")).FontSize(7);
                    }
                });

                // Nota si hay más registros
                if (datos.Count > 1000)
                {
                    column.Item().PaddingTop(10).Text(string.Format("Nota: Se muestran los primeros 1,000 registros de un total de {0:N0}. Para ver el detalle completo, utilice la exportación a Excel.", datos.Count))
                        .FontSize(9)
                        .Italic()
                        .FontColor(ColorGris);
                }
            });
        }

        // ============================================
        // FIRMAS
        // ============================================
        private void ComposeFirmas(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(100).Text("FIRMAS Y APROBACIONES")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(30);

                column.Item().Row(row =>
                {
                    // Generado por
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Generado por:").Bold();
                        col.Item().PaddingTop(5).AlignCenter().Text(SesionActual.NombreCompleto);
                        col.Item().PaddingTop(40).AlignCenter().Text("_____________________");
                        col.Item().PaddingTop(5).AlignCenter().Text("Firma").FontSize(8);
                    });

                    row.ConstantItem(50);

                    // Revisado por
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Revisado por:").Bold();
                        col.Item().PaddingTop(45).AlignCenter().Text("_____________________");
                        col.Item().PaddingTop(5).AlignCenter().Text("Firma").FontSize(8);
                    });
                });

                column.Item().PaddingTop(30).AlignCenter()
                    .Text(string.Format("Fecha: {0:dd/MM/yyyy}", DateTime.Now));
            });
        }

        // ============================================
        // HELPERS
        // ============================================
        private string ObtenerValor(DataRow row, string columna)
        {
            try
            {
                if (row[columna] == DBNull.Value) return "";

                if (columna == "FechaHora" && row[columna] is DateTime fecha)
                {
                    return fecha.ToString("dd/MM/yyyy HH:mm");
                }

                return row[columna].ToString();
            }
            catch
            {
                return "";
            }
        }

        private string ObtenerPeriodoTexto()
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                return string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", fechaDesde.Value, fechaHasta.Value);
            }
            else if (fechaDesde.HasValue)
            {
                return string.Format("Desde {0:dd/MM/yyyy}", fechaDesde.Value);
            }
            else if (fechaHasta.HasValue)
            {
                return string.Format("Hasta {0:dd/MM/yyyy}", fechaHasta.Value);
            }
            else
            {
                return "Todos los registros";
            }
        }
    }
}