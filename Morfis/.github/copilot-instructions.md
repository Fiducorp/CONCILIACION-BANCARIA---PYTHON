# Copilot Instructions

## General Guidelines
- Modify only the logo loading lines in forms; do not alter the rest of the file.

## UI Formatting
- Use ProveedorID formatted as PRV-{ProveedorID:D6} in the UI (txtCodigoProveedor); do not rely on the 'Codigo' column in the Proveedores table. This is the preferred method for displaying provider IDs. Ensure that forms are checked and adapted to display/use PRV-{ProveedorID:D6} and do not fail if the 'Codigo' column does not exist.
- When inserting, editing, or deleting providers or fideicomisos, always generate and display PRV-{ProveedorID:D6} for txtCodigoProveedor; ensure the UI updates immediately to reflect the newly generated PRV-ID from SQL. Additionally, upon adding, editing, or deleting a provider or fideicomiso, FormSolicitudPago must refresh the list and automatically select the updated record (or clear the selection if it was deleted), updating the dependent fields (txtRNCProveedor, lblTelefonoProveedor, txtCodigoFideicomiso, lblRNCFideicomiso) with the most recent data using BeginInvoke to ensure the selection occurs after the rebind.
- Upon saving or deleting providers, FormSolicitudPago must refresh the list and automatically select the updated provider (or clear the selection if it was deleted). Additionally, update txtRNCProveedor and lblTelefonoProveedor with the most recent formatted values. Validate document length: RNC must have 9 digits, CÉDULA must have 11 digits before saving.
- Validar longitud de documento en Leave de txtNumeroDocumento (RNC = 9 dígitos, CEDULA = 11) y actualizar UI inmediatamente; preferir recibir cambios uno a uno.
- Agregar helper FormatPhone en FormSolicitudPago para formatear números de teléfono.