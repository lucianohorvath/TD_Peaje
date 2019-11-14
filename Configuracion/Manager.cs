using Modelo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Configuracion
{
    internal static class Manager
    {
        private static string filename = ConfigurationManager.AppSettings["ConfigFileName"];
        private static string fullPath = Path.GetFullPath(@"..\..\..\") + filename;
        private static string json;

        /// <summary>
        ///  Inicializaciones estáticas de la clase Manager
        /// </summary>
        static Manager() 
        {
            using (StreamReader r = new StreamReader(fullPath))
            {
                json = r.ReadToEnd();
            }
        }

        internal static IEnumerable<Vehiculo> getVehiculos()
        {
            return JsonConvert.DeserializeObject<ConfigObject>(json).vehiculos;
        }

        internal static IEnumerable<CabinaPeaje> getCabinasPeaje()
        {
            return JsonConvert.DeserializeObject<ConfigObject>(json).cabinas;
        }

        internal static int getFrecuenciaVehiculos()
        {
            return JsonConvert.DeserializeObject<ConfigObject>(json).frecuenciaVehiculos;
        }

        internal static int getRefrescoPantalla()
        {
            return JsonConvert.DeserializeObject<ConfigObject>(json).refrescoPantalla;
        }

        internal static int getRefrescoConsola()
        {
            return JsonConvert.DeserializeObject<ConfigObject>(json).refrescoConsola;
        }
    }
}
