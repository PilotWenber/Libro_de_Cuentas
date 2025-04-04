using System;
using System.Windows.Forms;
using Libro_de_Cuentas.Models;
using Libro_de_Cuentas.Data.Repositories;
using System.IO;
using System.Text;
using System.Linq;

namespace Libro_de_Cuentas
{
    public partial class FormHistorial: Form
    {
        private readonly Persona _persona;
        private readonly MovimientoRepository repo = new MovimientoRepository();
        public FormHistorial(Persona persona)
        {
            InitializeComponent();
            _persona = persona;
            lblPersona.Text = $"Historial de: {_persona.Nombre}";
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            var movimientos = repo.ObtenerMovimientos(_persona.Id);
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

            decimal saldo = totalDeuda - totalPago;

            lblTotalPago.Text = $"Total Pagado: ${totalPago}";
            lblTotalDeuda.Text = $"Total Deuda: ${totalDeuda}";
            lblSaldo.Text = $"Saldo: ${saldo}";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.CurrentRow?.DataBoundItem is Movimiento mov)
            {
                var result = MessageBox.Show("¿Seguro que querés eliminar este movimiento?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    repo.EliminarMovimiento(mov.Id);
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
            if (dgvMovimientos.CurrentRow?.DataBoundItem is Movimiento mov)
            {
                using (var formEditar = new FormEditarMovimiento(mov))
                {
                    if (formEditar.ShowDialog() == DialogResult.OK)
                    {
                        // Actualizar el movimiento en la base de datos
                        repo.ActualizarMovimiento(formEditar.MovimientoEditado);
                        CargarHistorial();
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccioná un movimiento primero");
            }
        }

        private void FiltrarPorFechas()
        {
            var movimientos = repo.ObtenerMovimientos(_persona.Id)
                .Where(m  => m.Fecha >= dtpDesde.Value.Date && m.Fecha <= dtpHasta.Value.Date.AddDays(1).AddTicks(-1))
                .ToList();

            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);

            dgvMovimientos.DataSource = movimientos;
        }

        private void btnFiltrarFechas_Click(object sender, EventArgs e)
        {
            FiltrarPorFechas();
        }

        private void btnQuitarFiltro_Click(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
            CargarHistorial();
        }

        private void btnExportarCSV_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.DataSource != null)
            {
                var movimientos = (dgvMovimientos.DataSource as System.Collections.IEnumerable).Cast<Movimiento>().ToList();
                var sfd = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = $"Historial_{_persona.Nombre}.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Id,Tipo,Monto,Fecha");
                    foreach (var mov in movimientos)
                    {
                        sb.AppendLine($"{mov.Id},{mov.Tipo},{mov.Monto},{mov.Fecha:dd/MM/yyyy}");
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Archivo exportado correctamente.");
                }
            }
        }
    }
}
