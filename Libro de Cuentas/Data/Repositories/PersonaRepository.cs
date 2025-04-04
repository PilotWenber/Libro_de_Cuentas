using System.Collections.Generic;
using System.Data.SQLite;
using Libro_de_Cuentas.Models;

namespace Libro_de_Cuentas.Data.Repositories
{
    public class PersonaRepository
    {
        public void AgregarPersona(string nombre)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Personas (Nombre) VALUES (@nombre)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Persona> ObtenerPersonas()
        {
            var personas = new List<Persona>();

            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id, Nombre FROM Personas";
                using (var cmd = new SQLiteCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        personas.Add(new Persona
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        });
                    }
                }
            }

            return personas;
        }
    }
}
