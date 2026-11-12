using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EFCore1.Modelos
{
    public static class Conexion
    {
        
        public static string CadenaDeConexion()
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
            if (!File.Exists(ruta))
            {
                throw new FileNotFoundException("Archivo no encontrado");
            }
            try
            {
                var conf = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(ruta))!;
                var cadena = new MySqlConnectionStringBuilder
                {
                    Server = conf["Servidor"],
                    Port = uint.Parse(conf["Puerto"]),
                    Database = conf["Database"],
                    UserID = conf["user"],
                    Password = conf["contraseña"]
                }.ConnectionString;
                return cadena;
            }
            catch (JsonException ex)
            {
                throw new Exception("Error al deserializar el archivo de configuración", ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Error al leer el archivo de configuración", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Error al obtener la cadena de conexión", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error desconocido al obtener la cadena de conexión", ex);
            }
        }
    }
}
