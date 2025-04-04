using System;
using System.Windows.Forms;
using Libro_de_Cuentas.Models;

namespace Libro_de_Cuentas
{
    public partial class FormEditarMovimiento: Form
    {
        public Movimiento MovimientoEditado { get; set; }
        public FormEditarMovimiento(Movimiento movimiento)
        {
            InitializeComponent();

            //Cargar datos
            cmbTipo.Items.AddRange(new string[] { "Deuda", "Pago" });
            cmbTipo.SelectedItem = movimiento.Tipo;
            nudMonto.Value = movimiento.Monto;
            dtpFecha.Value = movimiento.Fecha;

            MovimientoEditado = movimiento;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            MovimientoEditado.Tipo = cmbTipo.SelectedItem.ToString();
            MovimientoEditado.Monto = nudMonto.Value;
            MovimientoEditado.Fecha = dtpFecha.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
