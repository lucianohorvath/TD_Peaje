using Modelo;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Configuracion
{
    // Representa la estructura del JSON de configuración para poder parsearlo correctamente
    internal class ConfigObject
    {
        public List<Vehiculo> vehiculos;
        public List<CabinaPeaje> cabinas;

        // Representa cada cuántos milisegundos aparece un vehículo nuevo
        [JsonProperty("frecuencia_vehiculos")]
        internal int frecuenciaVehiculos;

        // Representa cada cuántos milisegundos aparece un vehículo nuevo
        [JsonProperty("refresco_pantalla")]
        internal int refrescoPantalla;

        // Representa cada cuántos milisegundos se refresca la salida de consola.
        // Es también la frecuencia de recálculo de posición de los vehículos.
        [JsonProperty("refresco_consola")]
        internal int refrescoConsola;

        // Representa si los vehículos aparecerán en carriles aleatorios
        [JsonProperty("carriles_aleatorios")]
        internal bool carrilesAleatorios;
    }
}
