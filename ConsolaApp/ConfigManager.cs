using Modelo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolaApp
{
    public static class ConfigManager
    {
        private static string filename = ConfigurationManager.AppSettings["ConfigFileName"];
        private static string fullPath = Path.GetFullPath(@"..\..\..\") + filename;

        public static IEnumerable<Vehiculo> getVehiculos()
        {
            List<Vehiculo> vehiculos = new List<Vehiculo>();

            using (StreamReader r = new StreamReader(fullPath))
            {
                string json = r.ReadToEnd();
                vehiculos = JsonConvert.DeserializeObject<ConfigObject>(json).vehiculos;
            }

            return vehiculos;
        }

        public static IEnumerable<CabinaPeaje> getCabinasPeaje()
        {
            List<CabinaPeaje> cabinas = new List<CabinaPeaje>();

            using (StreamReader r = new StreamReader(fullPath))
            {
                string json = r.ReadToEnd();
                cabinas = JsonConvert.DeserializeObject<ConfigObject>(json).cabinas;
            }

            return cabinas;
        }

        public static int getFrecuenciaVehiculos()
        {
            using (StreamReader r = new StreamReader(fullPath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<ConfigObject>(json).frecuenciaVehiculos;
            }
        }
    }


    // Representa la estructura del JSON de configuración para poder parsearlo correctamente
    internal class ConfigObject
    {
        public List<Vehiculo> vehiculos;
        public List<CabinaPeaje> cabinas;

        // Representa cada cuántos milisegundos aparece un vehículo nuevo
        [JsonProperty("frecuencia_vehiculos")]
        public int frecuenciaVehiculos;
    }
}
