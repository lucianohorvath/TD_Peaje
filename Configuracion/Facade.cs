using Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuracion
{
    public static class Facade
    {
        public static IEnumerable<Vehiculo> getVehiculos()
        {
            return Manager.getVehiculos();
        }

        public static IEnumerable<CabinaPeaje> getCabinasPeaje()
        {
            return Manager.getCabinasPeaje();
        }

        /// <summary>
        /// Obtiene cada cuántos milisegundos aparece un vehículo nuevo en la autopista.
        /// </summary>
        /// <returns>Cantidad de milisegundos</returns>
        public static int getFrecuenciaVehiculos()
        {
            return Manager.getFrecuenciaVehiculos();
        }

        /// <summary>
        /// Obtiene cada cuántos milisegundos debe redibujarse la interfaz gráfica.
        /// </summary>
        /// <returns>Cantidad de milisegundos</returns>
        public static int getRefrescoPantalla()
        {
            return Manager.getRefrescoPantalla();
        }

        /// <summary>
        /// Obtiene cada cuántos milisegundos debe refrescarse la consola.
        /// Es, a su vez, la frecuencia de recálculo de posición de los vehículos,
        /// por lo cual influye en la cantidad de posiciones reportadas y en la 
        /// fluidez de la UI.
        /// </summary>
        /// <returns>Cantidad de milisegundos</returns>
        public static int getRefrescoConsola()
        {
            return Manager.getRefrescoConsola();
        }

        /// <summary>
        /// Obtiene si los vehículos aparecerán en carriles aleatorios.
        /// Si es falso, se respetará el carril indicado en cada vehículo.
        /// </summary>
        public static bool getCarrilesAleatorios()
        {
            return Manager.getCarrilesAleatorios();
        }

        /// <summary>
        /// Obtiene el path absoluto del archivo de configuración.
        /// </summary>
        public static string getFilePath()
        {
            return Manager.getFileFullPath();
        }
    }
}
