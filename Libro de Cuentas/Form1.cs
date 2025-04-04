using System;
using System.Linq;
using System.Windows.Forms;
using Libro_de_Cuentas.Data.Repositories;
using Libro_de_Cuentas.Models;

namespace Libro_de_Cuentas
{
    public partial class Form1: Form
    {
        private readonly PersonaRepository personaRepo = new PersonaRepository();
        private readonly MovimientoRepository movimientoRepo = new MovimientoRepository();
        public Form1()
        {
            InitializeComponent();
            CargarPersonas();
            cboTipo.Items.AddRange(new[] { "Deuda", "Pago" });
            cboTipo.SelectedIndex = 0;
        }

        private void CargarPersonas()
        {
            var personas = personaRepo.ObtenerPersonas(); 
            foreach (var p in personas)
                lstPersonas.Items.Add(p);
            lstPersonas.DisplayMember = "Nombre";
        }

        private void btnAgregarPersona_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (!string.IsNullOrEmpty(nombre))
            {
                personaRepo.AgregarPersona(nombre);
                txtNombre.Clear();
                CargarPersonas();
            }
            else
            {
                MessageBox.Show("El nombre no puede estar vacío.");
            }
        }

        private void lstPersonas_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarSaldoYMovimiento();
        }

        private void btnAgregarMovimiento_Click(object sender, EventArgs e)
        {
            if (lstPersonas.SelectedItem is Persona persona && decimal.TryParse(txtMonto.Text, out decimal monto))
            {
                string tipo = cboTipo.SelectedItem.ToString();
                movimientoRepo.AgregarMovimiento(persona.Id, tipo, monto);
                txtMonto.Clear();

                decimal saldo = movimientoRepo.ObtenerSaldo(persona.Id);
                lblSaldo.Text = $"Saldo: ${saldo:0.00}";
            }
        }

        private void btnVerHistorial_Click(object sender, EventArgs e)
        {
            if (lstPersonas.SelectedItem is Persona persona)
            {
                var formHistorial = new FormHistorial(persona);
                formHistorial.ShowDialog();
                ActualizarSaldoYMovimiento();
            }
            else
            {
                MessageBox.Show("Seleccioná una persona para ver su historial.");
            }
        }

        private void ActualizarSaldoYMovimiento()
        {
            if (lstPersonas.SelectedItem is Persona persona)
            {
                var saldo = movimientoRepo.ObtenerSaldo(persona.Id);
                lblSaldo.Text = $"Saldo: ${saldo:0.00}";                
            }
        }
    }
}
