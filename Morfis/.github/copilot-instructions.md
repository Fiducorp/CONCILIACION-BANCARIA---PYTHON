# Copilot Instructions

## General Guidelines
- Modify only the logo loading lines in forms; do not alter the rest of the file.
- Avoid conditional expressions (ternary) directly within string interpolations; calculate values (e.g., clean IDs or results of Regex.Replace) in variables before constructing the string to prevent syntax errors and complex escapes.

## UI Formatting
- Use ProveedorID formatted as PRV-{ProveedorID:D6} in the UI (txtCodigoProveedor); do not rely on the 'Codigo' column in the Proveedores table. This is the preferred method for displaying provider IDs. Ensure that forms are checked and adapted to display/use PRV-{ProveedorID:D6} and do not fail if the 'Codigo' column does not exist.
- When inserting, editing, or deleting providers or fideicomisos, always generate and display PRV-{ProveedorID:D6} for txtCodigoProveedor; ensure the UI updates immediately to reflect the newly generated PRV-ID from SQL. Additionally, upon adding, editing, or deleting a provider or fideicomiso, FormSolicitudPago must refresh the list and automatically select the updated record (or clear the selection if it was deleted), updating the dependent fields (txtRNCProveedor, lblTelefonoProveedor, txtCodigoFideicomiso, lblRNCFideicomiso) with the most recent data using BeginInvoke to ensure the selection occurs after the rebind.
- Upon saving or deleting providers, FormSolicitudPago must refresh the list and automatically select the updated provider (or clear the selection if it was deleted). Additionally, update txtRNCProveedor and lblTelefonoProveedor with the most recent formatted values. Validate document length: RNC must have 9 digits, CÉDULA must have 11 digits before saving.
- Validar longitud de documento en Leave de txtNumeroDocumento (RNC = 9 dígitos, CEDULA = 11) y actualizar UI inmediatamente; preferir recibir cambios uno a uno.
- Agregar helper FormatPhone en FormSolicitudPago para formatear números de teléfono: almacenar sin guiones, mostrar con guiones y paréntesis.
- Validar teléfono exactamente 10 dígitos.
- Hacer búsquedas en tiempo real (TextChanged) para `txtCodigoFideicomiso` y `txtRNCProveedor`; evitar selección inicial en `FormSolicitudPago` — dejar `cboFideicomiso`, `cboProveedor`, `txtCodigoFideicomiso` y `txtRNCProveedor` sin valor al abrir; usar flags `suppressFideicomisoEvents` y `suppressProveedorEvents` para suprimir eventos durante binding. La búsqueda en tiempo real debe realizarse solo con coincidencia exacta del código o del RNC/Cédula (no prefijos); seleccionar únicamente cuando la entrada coincide exactamente con el valor almacenado (limpio para RNC).
- Al usar TextChanged para autocompletar cboProveedor/cboFideicomiso, evitar usar ComboBox.SelectedValue porque ValueMember puede no estar configurado aún; en su lugar, buscar el DataRowView en cbo.Items y asignar SelectedIndex para evitar InvalidOperationException.
- Expandir el límite de comprobantes a 20 y crear una UI de comprobantes en formato 5 columnas × 4 filas con control para eliminar por hover (X).

## Form Validation and User Feedback
- Usar controles ErrorProvider (errProviderProveedor, errProviderFideicomiso) para validaciones en formularios de miniforms. En FormAgregarFideicomiso, usar el ErrorProvider `errProviderFideicomiso` exclusivamente para mostrar errores (incluyendo excepciones en operaciones BD), evitando MessageBox con MessageBoxIcon.Error; mantener MessageBox solo para confirmaciones y mensajes de éxito/información. Mostrar TODOS los errores y excepciones exclusivamente a través de este ErrorProvider (usar SetError en el control más relevante) y limpiar errores en rutas de éxito.
- Botones de guardar en miniforms deben cambiar texto y color cuando se carga un registro: cambiar a '?? Modificar' con color naranja; si en modo edición se cambia el documento clave, debe mostrar '?? Guardar como Nuevo' (verde). Mantener cambio de texto/color en `btnGuardarFideicomiso` al cargar registro (?? Modificar, naranja) y en modo nuevo (?? Guardar, verde).

## NCF Validation
- Validar el campo NCF según el tipo seleccionado en `cboTipoNCF` (longitud y prefijo).