using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Modelo
{
    public class Vehiculo
    {
        #region Propiedades estáticas
        public static int finalAutopista = 80;          // Altura en metros a la que termina la autopista
        public static int largoAuto = 4;                // Largo en metros de cada vehículo
        public static int velocidadRefresco = 1000;     // Cada cuántos ms el vehículo recalcula su posición
        public static object objPos = new Object();     // Bloqueo para recalcular posiciones. Objeto compartido.
        // este último bloquea TODOS los vehículos, sería mejor tenerlo en la cabina para que bloquee sólo a los del carril
        #endregion

        #region Propiedades
        public string marca;
        public string patente;
        public uint velocidad;           // En km/h
        public uint posicion = 0;        // Posición en metros respecto a la autopista
        public uint paciencia;           // Cantidad de autos que es capaz de tolerar en la cola
        public bool impaciente;          // Estado actual del conductor
        public uint carril;              // El carril por el que conduce coincide con el número de cabina de peaje

        public CabinaPeaje cabina;
        #endregion

        public Vehiculo(string marca, string patente, uint velocidad, uint paciencia) 
        {
            this.marca = marca;
            this.patente = patente;
            this.velocidad = velocidad;
            this.paciencia = paciencia;
        }

        public void Conducir()
        {
            bool enAutopista = true;
            while (enAutopista)
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} - Vehículo {this.patente} - Posición actual: {this.posicion} metros");
                // Manejo durante x milisegundos...
                Thread.Sleep(velocidadRefresco);
                bool enCola = calcularNuevaPosicion();

                if (enCola)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} - Vehículo {this.patente} - Posición actual: {this.posicion}, esperando la cola del peaje");

                    int vehiculosAdelante = cabina.getVehiculosEsperando() - 1;
                    if (vehiculosAdelante > paciencia)
                    {
                        tocarBocina(vehiculosAdelante);
                    }

                    while(posicion < CabinaPeaje.posicion)
                    {
                        // Espero poder avanzar en la cola
                        cabina.semCola.WaitOne();
                        // Avanzo una posición (el largo de un auto)
                        posicion += 4;
                        Console.WriteLine($"{Thread.CurrentThread.Name} - Vehículo {this.patente} - Posición actual: {this.posicion} metros, avanzó en la cola del peaje");
                    }

                    // Comienzo a pagar...
                    Monitor.Enter(cabina.objPago);
                    Monitor.Pulse(cabina.objPago);
                    // Espero que la cabina de peaje me despierte...
                    Monitor.Wait(cabina.objPago);
                    if (impaciente)
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name} - Vehículo {this.patente} - ¡Al fin! El conductor dejó de tocar bocina");
                        impaciente = false;
                    }

                    // Me levantó la barrera
                    Monitor.Exit(cabina.objPago);
                }

                if (this.posicion > finalAutopista)
                {
                    salirDeAutopista();
                    enAutopista = false;
                }
            }
        }

        /// <summary>
        /// Calcula la nueva posición del vehículo.
        /// Puede avanzar tanto como su velocidad lo indica si no sobrepasa el 
        /// último auto en la cola (o la cabina de peaje si no existe cola).
        /// Se asume que todos los autos miden 4 metros de largo.
        /// No se calculan aquí las posiciones internas a una eventual cola.
        /// </summary>
        /// <returns>Verdadero si el auto está en cola.</returns>
        private bool calcularNuevaPosicion()
        {
            bool enCola = false;
            uint posiblePosicion = posicion + (velocidad * (uint)velocidadRefresco / 3600);
            Monitor.Enter(objPos);
            uint posicionUltimoAuto = CabinaPeaje.posicion - (uint)(cabina.getVehiculosEsperando() * largoAuto);

            if (posiblePosicion < posicionUltimoAuto || this.posicion >= CabinaPeaje.posicion)
            {
                this.posicion = posiblePosicion;
            }
            else {
                this.posicion = posicionUltimoAuto;
                enCola = true;
                cabina.vehiculos.Enqueue(this);
            }
            Monitor.Exit(objPos);

            return enCola;
        }

        private void tocarBocina(int vehiculosAdelante)
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} - Vehículo {this.patente} - Conductor impaciente: \"¿{vehiculosAdelante} vehículos? Qué desastre\" ¡Comienza a tocar bocina!");
            impaciente = true;
            cabina.bocinazos++;
        }

        private void salirDeAutopista()
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} - Vehículo {this.patente} - Saliendo de autopista...");
            this.posicion = 200;        // posición final, lo alejo de los otros gráficos
        }

        public override string ToString()
        {
            return this.patente;
        }
    }
}
