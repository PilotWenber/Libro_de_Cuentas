using System.Data.SQLite;
using System.IO;

namespace Libro_de_Cuentas.Data
{
    public static class Database
    {
        private static string _connectionString = "Data Source=cuentas.db;Version=3";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public static void Inicializar()
        {
            bool nuevaBaseDeDatos = !File.Exists("cuentas.db");

            using (var connection = GetConnection())
            {
                connection.Open();

                string createPersonaTable = @"
                    CREATE TABLE IF NOT EXISTS Persona (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL
                    );";

                string createMovimientoTable = @"
                    CREATE TABLE IF NOT EXISTS Movimiento (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PersonaId INTEGER NOT NULL,
                        Tipo TEXT NOT NULL,
                        Monto DECIMAL NOT NULL,
                        Fecha DATETIME NOT NULL,
                        FOREIGN KEY (PersonaId) REFERENCES Persona(Id)
                    );";

                using (var cmd = new SQLiteCommand(createPersonaTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }               

                using (var cmd = new SQLiteCommand(createMovimientoTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
