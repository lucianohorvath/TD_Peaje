using Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsolaApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Vehiculo> vehiculos = Configuracion.Facade.getVehiculos();
            IEnumerable<CabinaPeaje> cabinas = Configuracion.Facade.getCabinasPeaje();
            int frecuenciaVehiculos = Configuracion.Facade.getFrecuenciaVehiculos();
            Vehiculo.velocidadRefresco = Configuracion.Facade.getRefrescoConsola();
            bool carrilAleatorio = Configuracion.Facade.getCarrilesAleatorios();

            int j = 1;
            foreach (CabinaPeaje cp in cabinas)
            {
                Thread t = new Thread(cp.Operar);
                t.Name = $"Thread Cabina {j}";
                t.IsBackground = true;
                t.Start();
                j++;
            }

            int i = 1;
            foreach (Vehiculo v in vehiculos)
            {
                if (carrilAleatorio)
                {
                    Random random = new Random();
                    int numeroCabina = random.Next(1, cabinas.Count() + 1);
                    v.carril = (uint)numeroCabina;
                }
                v.cabina = cabinas.ToList().Find(c => c.numero == v.carril);
                Thread t = new Thread(v.Conducir);
                t.Name = $"Thread Vehículo {i}";
                t.IsBackground = true;
                t.Start();
                Thread.Sleep(frecuenciaVehiculos);

                i++;
            }         
            
            Console.ReadKey();
        }
    }
}
