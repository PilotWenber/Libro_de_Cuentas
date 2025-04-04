using System;
using System.Windows.Forms;
using Libro_de_Cuentas.Data.Repositories;
using Libro_de_Cuentas.Models;

namespace Libro_de_Cuentas
{
    public partial class Form1: Form
    {
        private PersonaRepository personaRepo = new PersonaRepository();
        private MovimientoRepository movimientoRepo = new MovimientoRepository();
        public Form1()
        {
            InitializeComponent();
            cboTipo.Items.Add("deuda");
            cboTipo.Items.Add("pago");
            cboTipo.SelectedIndex = 0;

            CargarPersonas();
        }

        private void CargarPersonas()
        {
            lstPersonas.Items.Clear();
            var personas = personaRepo.ObtenerPersonas();
            foreach (var p in personas)
                lstPersonas.Items.Add(p);
        }

        private void btnAgregarPersona_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                personaRepo.AgregarPersona(txtNombre.Text);
                txtNombre.Clear();
                CargarPersonas();
            }
        }

        private void lstPersonas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPersonas.SelectedItem is Persona persona)
            {
                double saldo = movimientoRepo.ObtenerSaldo(persona.Id);
                lblSaldo.Text = $"Saldo: ${saldo:0.00}";
            }
        }

        private void btnAgregarMovimiento_Click(object sender, EventArgs e)
        {
            if (lstPersonas.SelectedItem is Persona persona && double.TryParse(txtMonto.Text, out double monto))
            {
                string tipo = cboTipo.SelectedItem.ToString();
                movimientoRepo.AgregarMovimiento(persona.Id, tipo, monto);
                txtMonto.Clear();

                double saldo = movimientoRepo.ObtenerSaldo(persona.Id);
                lblSaldo.Text = $"Saldo: ${saldo:0.00}";
            }
        }

        private void btnVerHistorial_Click(object sender, EventArgs e)
        {
            if (lstPersonas.SelectedItem is Persona persona)
            {
                var formHistorial = new FormHistorial(persona);
                formHistorial.ShowDialog();
            }
        }
    }
}
