using System;
using System.Data.SQLite;
using System.IO;

namespace Libro_de_Cuentas.Data
{
    public static class Database
    {
        private static string dbFile = "cuentas.db";
        private static string connectionString = $"Data Source={dbFile};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        public static void Initialize()
        {
            // Si no existe el archivo, lo crea
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                string createPersonasTable = @"
                    CREATE TABLE IF NOT EXISTS Personas (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL
                    );";

                string createMovimientosTable = @"
                    CREATE TABLE IF NOT EXISTS Movimientos (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PersonaId INTEGER NOT NULL,
                        Tipo TEXT CHECK(Tipo IN ('deuda', 'pago')) NOT NULL,
                        Monto REAL NOT NULL,
                        Fecha TEXT NOT NULL,
                        FOREIGN KEY (PersonaId) REFERENCES Personas(Id)
                    );";

                using (var cmd = new SQLiteCommand(createPersonasTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SQLiteCommand(createMovimientosTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
