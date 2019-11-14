using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Modelo
{
    public class CabinaPeaje
    {
        #region Constantes
        // Representa la posición en metros de la cabina en la autopista
        public const uint posicion = 50;
        #endregion

        #region Propiedades
        // Representa el número de cabina
        public uint numero;

        // Representa cuántos milisegundos demora la cabina en operar (cobrar y levantar la barrera)
        public int demora;

        // Representa cuántos vehículos están demorados en un instante determinado
        public Queue<Vehiculo> vehiculos;

        // Representa cuántos bocinazos es capaz de tolerar antes de levantar la barrera
        [JsonProperty("limite_bocinazos")]
        public uint limiteBocinazos;

        // Representa cuántos bocinazos está escuchando en un instante determinado
        public uint bocinazos = 0;

        // Representa si la barrera está levantada en un instante determinado (por bocinazos)
        public bool barreraLevantada;

        // Representa el bloqueo en la operación de pagar
        public object objPago;
        
        // Representa la cola de espera. La cabina determina cuántos pueden avanzar
        public Semaphore semCola = new Semaphore(0, 15);
        #endregion
        
        public CabinaPeaje(uint numero, int demora)
        {
            // No lo invoco de forma directa, pero la librería de JSON sí lo hace
            this.numero = numero;
            this.demora = demora;
            this.vehiculos = new Queue<Vehiculo>();
            this.objPago = new Object();
        }

        /// <summary>
        /// Retorna la cantidad de vehículos que están esperando en la cola del peaje.
        /// </summary>
        public int getVehiculosEsperando()
        {
            return vehiculos.Count;
        }

        public void Operar()
        {
            while (true)
            {
                Monitor.Enter(this.objPago);
                Thread.Sleep(100);
                if (bocinazos > limiteBocinazos)
                {
                    LevantarBarrera();
                }
                if (vehiculos.Count > 0)
                {
                    Console.WriteLine($"--> Cabina número {this.numero} - Operando con vehículo {vehiculos.Peek()}");
                    Thread.Sleep(demora);
                    Console.WriteLine($"--> Cabina número {this.numero} - Barrera levantada, pasa el vehículo {vehiculos.Dequeue()}");
                    if (vehiculos.Count > 0)
                    {
                        Console.WriteLine($"--> Cabina número {this.numero} - Aviso: Pueden avanzar los {vehiculos.Count} vehículos que están esperando");
                        this.semCola.Release(vehiculos.Count);
                    }
                }
                Monitor.Pulse(this.objPago);
                Monitor.Wait(this.objPago);
                Monitor.Exit(this.objPago);
            }        
        }

        private void LevantarBarrera()
        {
            this.barreraLevantada = true;
            Console.WriteLine($"--> Cabina número {this.numero} - Ya no tolera los bocinazos. Levantando barrera...");
            while (bocinazos > limiteBocinazos / 2)
            {
                Monitor.Enter(this.objPago);
                Thread.Sleep(500);
                Vehiculo vehiculoEnPeaje = vehiculos.Dequeue();
                Console.WriteLine($"--> Cabina número {this.numero} - Barrera levantada (libre), pasa el vehículo {vehiculoEnPeaje}");
                if (vehiculoEnPeaje.impaciente)
                {
                    bocinazos--;
                }
                if (vehiculos.Count > 0)
                {
                    Console.WriteLine($"--> Cabina número {this.numero} - Aviso: Pueden avanzar los {vehiculos.Count} vehículos que están esperando");
                    this.semCola.Release(vehiculos.Count);
                }

                Monitor.Pulse(this.objPago);
                // Si no me despiertan en 1500 ms, bajo la barrera (no hay más autos, el último pasó libre)
                if (! Monitor.Wait(this.objPago, 1500))
                {
                    Console.WriteLine($"--> Cabina número {this.numero} - ¿No viene nadie? Bueno...");
                }
                Monitor.Exit(this.objPago);
            }
            this.barreraLevantada = false;
            Console.WriteLine($"--> Cabina número {this.numero} - Funcionamiento normal reestablecido");
        }
    }
}
