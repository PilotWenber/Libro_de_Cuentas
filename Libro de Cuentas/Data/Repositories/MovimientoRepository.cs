using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Libro_de_Cuentas.Models;

namespace Libro_de_Cuentas.Data.Repositories
{
    public class MovimientoRepository
    {
        private readonly string _connectionString = "Data Source=cuentas.db";
        public void AgregarMovimiento(int personaId, string tipo, double monto)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Movimientos (PersonaId, Tipo, Monto, Fecha)
                    VALUES (@personaId, @tipo, @monto, @fecha)";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@personaId", personaId);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@monto", monto);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("dd/MM/yyyy"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public double ObtenerSaldo(int personaId)
        {
            double totalDeuda = 0;
            double totalPago = 0;

            using (var connection = Database.GetConnection())
            {
                connection.Open();

                string queryDeuda = "SELECT SUM(Monto) FROM Movimientos WHERE PersonaId = @id AND Tipo = 'deuda'";
                using (var cmd = new SQLiteCommand(queryDeuda, connection))
                {
                    cmd.Parameters.AddWithValue("@id", personaId);
                    var result = cmd.ExecuteScalar();
                    totalDeuda = result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }

                string queryPago = "SELECT SUM(Monto) FROM Movimientos WHERE PersonaId = @id AND Tipo = 'pago'";
                using (var cmd = new SQLiteCommand(queryPago, connection))
                {
                    cmd.Parameters.AddWithValue("@id", personaId);
                    var result = cmd.ExecuteScalar();
                    totalPago = result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }
            }

            return totalDeuda - totalPago;
        }

        public List<Movimiento> ObtenerMovimientos(int personaId)
        {
            var lista = new List<Movimiento>();

            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT Id, PersonaId,TIpo,Monto,Fecha
                    FROM Movimientos
                    WHERE PersonaId = @id
                    ORDER BY Fecha DESC";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", personaId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Movimiento
                            {
                                Id = reader.GetInt32(0),
                                PersonaId = reader.GetInt32(1),
                                Tipo = reader.GetString(2),
                                Monto = reader.GetDecimal(3),
                                Fecha = DateTime.Parse(reader.GetString(4))
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public void EliminarMovimiento(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Movimiento WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarMovimiento(Movimiento movimiento)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE Movimiento
                                    SET Tipo = @tipo, Monto = @monto, Fecha = @fecha
                                    WHERE Id = @id";
                cmd.Parameters.AddWithValue("@tipo", movimiento.Tipo);
                cmd.Parameters.AddWithValue("@monto", movimiento.Monto);
                cmd.Parameters.AddWithValue("@fecha", movimiento.Fecha);
                cmd.Parameters.AddWithValue("@id", movimiento.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
