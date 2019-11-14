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

        public static int getFrecuenciaVehiculos()
        {
            return Manager.getFrecuenciaVehiculos();
        }

        public static int getRefrescoPantalla()
        {
            return Manager.getRefrescoPantalla();
        }

        public static int getRefrescoConsola()
        {
            return Manager.getRefrescoConsola();
        }
    }
}
