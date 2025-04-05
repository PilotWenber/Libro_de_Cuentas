using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Libro_de_Cuentas.Models;

namespace Libro_de_Cuentas.Data.Repositories
{
    public class MovimientoRepository
    {
        public void AgregarMovimiento(int personaId, string tipo, decimal monto)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Movimiento (PersonaId, Tipo, Monto, Fecha)
                    VALUES (@personaId, @tipo, @monto, @fecha)";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@personaId", personaId);
                    cmd.Parameters.AddWithValue("@tipo", tipo.ToLower());
                    cmd.Parameters.AddWithValue("@monto", monto);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public decimal ObtenerSaldo(int personaId)
        {
            decimal totalDeuda = 0;
            decimal totalPago = 0;

            using (var connection = Database.GetConnection())
            {
                connection.Open();

                string queryDeuda = "SELECT IFNULL(SUM(Monto), 0) FROM Movimiento WHERE PersonaId = @id AND Tipo = 'deuda'";
                using (var cmd = new SQLiteCommand(queryDeuda, connection))
                {
                    cmd.Parameters.AddWithValue("@id", personaId);
                    totalDeuda = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                string queryPago = "SELECT IFNULL(SUM(Monto), 0) FROM Movimiento WHERE PersonaId = @id AND Tipo = 'pago'";
                using (var cmd = new SQLiteCommand(queryPago, connection))
                {
                    cmd.Parameters.AddWithValue("@id", personaId);
                    totalPago = Convert.ToDecimal(cmd.ExecuteScalar());
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
                    SELECT Id, PersonaId, Tipo, Monto, Fecha
                    FROM Movimiento
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
                                Fecha = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public void EliminarMovimiento(int id)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Movimiento WHERE Id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Movimiento WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
        }

        public void ActualizarMovimiento(Movimiento movimiento)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Movimiento
                                    SET Tipo = @tipo, Monto = @monto, Fecha = @fecha
                                    WHERE Id = @id";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                cmd.Parameters.AddWithValue("@tipo", movimiento.Tipo);
                cmd.Parameters.AddWithValue("@monto", movimiento.Monto);
                cmd.Parameters.AddWithValue("@fecha", movimiento.Fecha);
                cmd.Parameters.AddWithValue("@id", movimiento.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
}
