using System;
using System.Windows.Forms;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Probar conexión antes de mostrar login
            if (!DatabaseConnection.TestConnection(out string mensaje))
            {
                MessageBox.Show($"No se pudo conectar a la base de datos:\n\n{mensaje}\n\nVerifique que SQL Server esté en ejecución.",
                    "Error de Conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Mostrar formulario de login
            FormLogin formLogin = new FormLogin();
            if (formLogin.ShowDialog() == DialogResult.OK)
            {
                // Login exitoso, abrir formulario principal
                Application.Run(new Forms.FormMain());
            }
        }
    }
}