using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class FormSolicitudPago
    {
        private const int MaxComprobantes = 20;
        private List<ComprobanteItem> comprobantes = new List<ComprobanteItem>();

        private class ComprobanteItem
        {
            public string TipoComprobanteNombre { get; set; }
            public string TipoNCFDisplay { get; set; } // puede ser null
            public string NumeroNCF { get; set; }      // NCF completo (prefijo + secuencial) o referencia
            public string NumeroSecuencial { get; set; } // sólo la parte secuencial (lo que queda en txtNumeroNCF)
        }

        // Handler a enlazar en ConfigurarEventos:
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

                    if (sec.Length != expectedSeqLen)
                    {
                        MessageBox.Show($"La parte secuencial debe tener {expectedSeqLen} dígitos (actual: {sec.Length}).", "Validación NCF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    var item = new ComprobanteItem
                    {
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
                        TipoComprobanteNombre = tipoNombre,
                        TipoNCFDisplay = null,
                        NumeroNCF = entered,
                        NumeroSecuencial = entered
                    };

                    comprobantes.Add(item);
                    AddComprobanteControl(item);

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
                    // eliminar del modelo
                    var ci = ctl.Tag as ComprobanteItem;
                    if (ci != null)
                    {
                        var existing = comprobantes.FirstOrDefault(x => string.Equals(x.NumeroNCF, ci.NumeroNCF, StringComparison.OrdinalIgnoreCase)
                                                                       && string.Equals(x.TipoComprobanteNombre, ci.TipoComprobanteNombre, StringComparison.OrdinalIgnoreCase));
                        if (existing != null)
                            comprobantes.Remove(existing);
                    }

                    // eliminar control visual
                    flpComprobantes.Controls.Remove(ctl);
                    ctl.Dispose();
                }
                catch { }
            };

            // Ajuste de tamaño: preferimos ancho fijo para 5 columnas
            int ancho = Math.Max(160, (flpComprobantes.ClientSize.Width - 24) / 5);
            ctl.Width = ancho;
            ctl.Height = 56;

            flpComprobantes.Controls.Add(ctl);
        }

        // Método público para obtener la lista (si es necesario al guardar)
        public IEnumerable<(string Tipo, string TipoNCF, string NumeroNCF, string Secuencial)> ObtenerComprobantes()
        {
            return comprobantes.Select(c => (c.TipoComprobanteNombre, c.TipoNCFDisplay, c.NumeroNCF, c.NumeroSecuencial));
        }
    }
}