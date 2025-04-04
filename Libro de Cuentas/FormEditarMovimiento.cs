using System;
using System.Drawing;
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

            // Estética
            this.BackColor = Color.WhiteSmoke;
            btnGuardar.BackColor = Color.MediumSeaGreen;
            btnCancelar.BackColor = Color.LightCoral;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatStyle = FlatStyle.Flat;

            // Inicializar combo box
            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("Deuda");
            cmbTipo.Items.Add("Pago");

            // Valores para NumericUpDown
            nudMonto.Minimum = -10000;
            nudMonto.Maximum = 1000000;

            // Asignar valores actuales del movimiento
            
            try
            {
                nudMonto.Value = movimiento.Monto;
            }
            catch (ArgumentOutOfRangeException)
            {
                nudMonto.Value = nudMonto.Minimum;
            }

            dtpFecha.Value = movimiento.Fecha;

            if (cmbTipo.Items.Contains(movimiento.Tipo))
            {
                cmbTipo.SelectedItem = movimiento.Tipo;
            }
            else
            {
                cmbTipo.SelectedIndex = -1; // Ninguno seleccionado
            }

            MovimientoEditado = movimiento;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbTipo.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona un tipo de movimiento.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
