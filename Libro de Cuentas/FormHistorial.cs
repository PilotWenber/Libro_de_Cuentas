using System;
using System.Windows.Forms;
using Libro_de_Cuentas.Models;
using Libro_de_Cuentas.Data.Repositories;
using System.Linq;
using System.IO;
using System.Text;

namespace Libro_de_Cuentas
{
    public partial class FormHistorial: Form
    {
        private Persona persona;
        private MovimientoRepository repo = new MovimientoRepository();
        public FormHistorial(Persona personaSeleccionada)
        {
            InitializeComponent();
            persona = personaSeleccionada;
            lblPersona.Text = $"Historial de: {persona.Nombre}";
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            var movimientos = repo.ObtenerMovimientos(persona.Id);
            dgvMovimientos.DataSource = movimientos;

            // Opcional: configurar columas
            dgvMovimientos.Columns["Id"].Visible = false;
            dgvMovimientos.Columns["PersonaId"].Visible = false;
            dgvMovimientos.Columns["Tipo"].HeaderText = "Tipo";
            dgvMovimientos.Columns["Monto"].HeaderText = "Monto ($)";
            dgvMovimientos.Columns["Fecha"].HeaderText = "Fecha";

            //Calcular resumen
            decimal totalPago = movimientos
                .Where(m => m.Tipo == "Pago")
                .Sum(m => m.Monto);

            decimal totalDeuda = movimientos
                .Where(m => m.Tipo == "Deuda")
                .Sum(m => m.Monto);

            decimal saldo = totalPago - totalDeuda;

            lblTotalPago.Text = $"Total Pagado: ${totalPago}";
            lblTotalDeuda.Text = $"Total Deuda: ${totalDeuda}";
            lblSaldo.Text = $"Saldo: ${saldo}";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("¿Seguro que querés eliminar este movimiento?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    var movimiento = (Movimiento)dgvMovimientos.SelectedRows[0].DataBoundItem;

                    var repo = new MovimientoRepository();
                    repo.EliminarMovimiento(movimiento.Id);

                    MessageBox.Show("Movimiento eliminado.");
                    CargarHistorial();
                }
            }
            else
            {
                MessageBox.Show("Seleccioná un movimiento primero");
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.SelectedRows.Count > 0)
            {
                var movimiento = (Movimiento)dgvMovimientos.SelectedRows[0].DataBoundItem;

                var formEditar = new FormEditarMovimiento(movimiento);
                var result = formEditar.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var repo = new MovimientoRepository();
                    repo.ActualizarMovimiento(formEditar.MovimientoEditado);
                    CargarHistorial();
                    MessageBox.Show("Movimiento actualizado");
                }
            }
            else
            {
                MessageBox.Show("Seleccioná un movimiento primero");
            }
        }

        private void FiltrarPorFechas()
        {
            var movimientos = repo.ObtenerMovimientos(persona.Id);

            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);

            var filtrados = movimientos.Where(m => m.Fecha >= desde && m.Fecha <= hasta).ToList();

            dgvMovimientos.DataSource = filtrados;

            //Actualizar resumen con los filtrados
            decimal totalPago = filtrados
                .Where(m => m.Tipo == "Pago")
                .Sum(m => m.Monto);

            decimal totalDeuda = filtrados
                .Where(m => m.Tipo == "Deuda")
                .Sum(m => m.Monto);

            decimal saldo = totalPago - totalDeuda;

            lblTotalPago.Text = $"Total Pagado: ${totalPago}";
            lblTotalDeuda.Text = $"Total Deuda: ${totalDeuda}";
            lblSaldo.Text = $"Saldo: ${saldo}";
        }

        private void btnFiltrarFechas_Click(object sender, EventArgs e)
        {
            FiltrarPorFechas();
        }

        private void btnQuitarFiltro_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void btnExportarCSV_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.Rows.Count == 0)
            {
                MessageBox.Show("No hay movimientos para exportar.");
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Archivo CSV (*.csv)|*.csv";
                saveFileDialog.Title = "Guardar historial como CSV";
                saveFileDialog.FileName = $"{persona.Nombre}_historial.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                        {
                            // Encabezados
                            sw.WriteLine("Id,Tipo,Monto,Fecha");

                            //Filas
                            foreach (DataGridViewRow row in dgvMovimientos.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    var movimiento = (Movimiento)row.DataBoundItem;
                                    string linea = $"{movimiento.Id},{movimiento.Tipo},{movimiento.Monto},{movimiento.Fecha:dd/MM/yyyy}";
                                    sw.WriteLine(linea);
                                }
                            }
                        }

                        MessageBox.Show("Historial exportado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al exportar: {ex.Message}");
                    }
                }
            }
        }
    }
}
