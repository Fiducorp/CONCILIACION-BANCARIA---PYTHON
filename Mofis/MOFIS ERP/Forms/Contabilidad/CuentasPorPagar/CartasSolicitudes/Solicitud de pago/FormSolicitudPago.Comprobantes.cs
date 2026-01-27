using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class FormSolicitudPago
    {
        private const int MaxComprobantes = 25; // actualizado a 25
        private const int ComprobantesColumns = 5; // 5 columnas (5x5)
        private List<ComprobanteItem> comprobantes = new List<ComprobanteItem>();
        private bool flpResizeHooked = false;

        private class ComprobanteItem
        {
            public int? TipoNCFID { get; set; }        // ID de la tabla TiposNCF
            public string TipoComprobanteNombre { get; set; }
            public string TipoNCFDisplay { get; set; } // puede ser null
            public string NumeroNCF { get; set; }      // NCF completo (prefijo + secuencial) o referencia
            public string NumeroSecuencial { get; set; } // sólo la parte secuencial
        }

        // Handler a enlazar en ConfigurarEventos():
        // if (btnAgregarComprobante != null) btnAgregarComprobante.Click += BtnAgregarComprobante_Click;
        private void BtnAgregarComprobante_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboTipoComprobante == null || cboTipoComprobante.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione el tipo de comprobante.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboTipoComprobante?.Focus();
                    return;
                }

                // Determinar si el tipo requiere NCF
                bool requiereNCF = false;
                string tipoNombre = cboTipoComprobante.Text ?? "Comprobante";
                try
                {
                    if (cboTipoComprobante.SelectedItem is System.Data.DataRowView drvTipo)
                    {
                        if (drvTipo.Row.Table.Columns.Contains("RequiereNCF"))
                            requiereNCF = Convert.ToBoolean(drvTipo["RequiereNCF"]);
                        if (!requiereNCF && tipoNombre.Trim().Equals("NCF", StringComparison.OrdinalIgnoreCase))
                            requiereNCF = true;
                    }
                }
                catch { }

                // Leer secuencial ingresado (txtNumeroNCF debe contener solo la parte secuencial)
                string numeroIngresado = txtNumeroNCF?.Text?.Trim() ?? string.Empty;
                if (numeroNCFPlaceholderActivo)
                    numeroIngresado = string.Empty;

                // Lógica para NCF
                if (requiereNCF)
                {
                    if (cboTipoNCF == null || cboTipoNCF.SelectedItem == null)
                    {
                        MessageBox.Show("Seleccione el tipo de NCF.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cboTipoNCF.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(numeroIngresado))
                    {
                        MessageBox.Show("Ingrese la parte secuencial del NCF.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNumeroNCF.Focus();
                        return;
                    }

                    // Prefijo y longitud esperada total
                    string prefix = GetNCFPrefixFromSelectedTipo() ?? string.Empty;
                    int expectedTotal = GetNCFExpectedLength();
                    int expectedSeqLen = Math.Max(0, expectedTotal - prefix.Length);

                    // Normalizar secuencial: sólo dígitos
                    string sec = Regex.Replace(numeroIngresado, @"\D", string.Empty);

                    // Validar que el usuario haya ingresado solo dígitos (no letras) y longitud esperada
                    if (!Regex.IsMatch(sec, @"^\d+$") || sec.Length != expectedSeqLen)
                    {
                        MessageBox.Show($"La parte secuencial debe contener exactamente {expectedSeqLen} dígitos numéricos.", "Validación NCF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNumeroNCF.Focus();
                        return;
                    }

                    string fullNCF = prefix + sec;

                    // Duplicado exacto
                    if (comprobantes.Any(c => string.Equals(c.NumeroNCF, fullNCF, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("El NCF ya fue agregado en la lista.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (comprobantes.Count >= MaxComprobantes)
                    {
                        MessageBox.Show($"Ha alcanzado el máximo de {MaxComprobantes} comprobantes.", "Límite alcanzado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    int ncfId = 0;
                    if (cboTipoNCF.SelectedValue != null) ncfId = Convert.ToInt32(cboTipoNCF.SelectedValue);

                    var item = new ComprobanteItem
                    {
                        TipoNCFID = ncfId,
                        TipoComprobanteNombre = tipoNombre,
                        TipoNCFDisplay = cboTipoNCF.Text,
                        NumeroNCF = fullNCF,
                        NumeroSecuencial = sec
                    };

                    comprobantes.Add(item);
                    AddComprobanteControl(item);

                    // Restaurar placeholder
                    SetNumeroNCFPlaceholder();
                }
                else
                {
                    // Para otros tipos, txtNumeroNCF obligatorio (según requisito)
                    if (string.IsNullOrWhiteSpace(numeroIngresado))
                    {
                        MessageBox.Show("Ingrese el número o referencia del comprobante.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNumeroNCF.Focus();
                        return;
                    }

                    string entered = numeroIngresado;

                    // Duplicado por tipo+referencia
                    if (comprobantes.Any(c => string.Equals(c.TipoComprobanteNombre, tipoNombre, StringComparison.OrdinalIgnoreCase)
                                             && string.Equals(c.NumeroNCF, entered, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("El comprobante ya fue agregado.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (comprobantes.Count >= MaxComprobantes)
                    {
                        MessageBox.Show($"Ha alcanzado el máximo de {MaxComprobantes} comprobantes.", "Límite alcanzado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var item = new ComprobanteItem
                    {
                        TipoNCFID = null, // No es un NCF tipificado
                        TipoComprobanteNombre = tipoNombre,
                        TipoNCFDisplay = null,
                        NumeroNCF = entered,
                        NumeroSecuencial = entered
                    };

                    comprobantes.Add(item);
                    AddComprobanteControl(item);

                    // Restaurar placeholder
                    SetNumeroNCFPlaceholder();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar comprobante: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddComprobanteControl(ComprobanteItem item)
        {
            if (flpComprobantes == null) return;

            var ctl = new ComprobanteItemControl(item.TipoComprobanteNombre, item.TipoNCFDisplay, item.NumeroNCF);
            ctl.Tag = item;
            ctl.RemoveRequested += (s, e) =>
            {
                try
                {
                    var ci = ctl.Tag as ComprobanteItem;
                    if (ci != null)
                    {
                        var existing = comprobantes.FirstOrDefault(x => string.Equals(x.NumeroNCF, ci.NumeroNCF, StringComparison.OrdinalIgnoreCase)
                                                                       && string.Equals(x.TipoComprobanteNombre, ci.TipoComprobanteNombre, StringComparison.OrdinalIgnoreCase));
                        if (existing != null)
                            comprobantes.Remove(existing);
                    }

                    flpComprobantes.Controls.Remove(ctl);
                    ctl.Dispose();

                    // reajustar anchos después de eliminar
                    AdjustComprobanteWidths();
                }
                catch { }
            };

            // Márgen reducido para poder ajustar 5 columnas
            ctl.Margin = new Padding(4);

            // Ajuste de tamaño para 5 columnas usando helper centralizado
            flpComprobantes.Controls.Add(ctl);

            // Hookear resize del FlowLayoutPanel una sola vez
            if (!flpResizeHooked)
            {
                flpResizeHooked = true;
                flpComprobantes.Resize += (s, e) => AdjustComprobanteWidths();
            }

            // reajustar anchos inmediatamente
            AdjustComprobanteWidths();
        }

        // Recalcula y aplica el ancho ideal para cada control dentro de flpComprobantes
        private void AdjustComprobanteWidths()
        {
            if (flpComprobantes == null) return;
            if (flpComprobantes.Controls.Count == 0) return;

            // Espacio disponible sin scrollbars (aprox)
            int totalPadding = flpComprobantes.Padding.Left + flpComprobantes.Padding.Right;
            int available = flpComprobantes.ClientSize.Width - totalPadding;

            // Calcular ancho por columna considerando márgenes internos de cada control
            // Tomamos el margen horizontal de un control promedio (left+right)
            int sampleMargin = 8; // 4 left + 4 right (tal como se estableció)
            int colWidth = Math.Max(80, (available - (ComprobantesColumns * sampleMargin)) / ComprobantesColumns);

            foreach (Control c in flpComprobantes.Controls)
            {
                c.Width = colWidth;
                // altura ya definida por el control (asegura pequeña tarjeta)
            }

            flpComprobantes.Invalidate();
        }

        // Método público para obtener la lista (si es necesario al guardar)
        public IEnumerable<(int? TipoNCFID, string Tipo, string TipoNCF, string NumeroNCF, string Secuencial)> ObtenerComprobantes()
        {
            return comprobantes.Select(c => (c.TipoNCFID, c.TipoComprobanteNombre, c.TipoNCFDisplay, c.NumeroNCF, c.NumeroSecuencial));
        }
    }
}