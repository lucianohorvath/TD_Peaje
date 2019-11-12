﻿using Modelo;
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
            IEnumerable<Vehiculo> vehiculos = ConfigManager.getVehiculos();
            CabinaPeaje cabina1 = ConfigManager.getCabinasPeaje().First();
            int frecuenciaVehiculos = ConfigManager.getFrecuenciaVehiculos();

            int i = 1;
            foreach (Vehiculo v in vehiculos)
            {
                v.cabina = cabina1;

                Thread t = new Thread(v.Conducir);
                t.Name = $"Thread Vehículo {i}";
                t.IsBackground = true;
                t.Start();
                Thread.Sleep(200);

                i++;
            }
            
            Thread th = new Thread(cabina1.Operar);
            th.Name = "Thread Cabina 1";
            th.IsBackground = true;
            th.Start();

            Console.ReadKey();
        }
    }
}
