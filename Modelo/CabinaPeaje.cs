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
        // Representa la posición en metros de la cabina en la autopista
        public const uint posicion = 50;
        
        #region Propiedades
        // Representa el número de cabina
        [ThreadStatic] public uint numero;

        // Representa cuántos milisegundos demora la cabina en operar (cobrar y levantar la barrera)
        [ThreadStatic] public int demora;

        // Representa cuántos vehículos están demorados en un instante determinado
        [ThreadStatic] public Queue<string> vehiculos;

        // Representa cuántos bocinazos es capaz de tolerar antes de levantar la barrera
        [JsonProperty("limite_bocinazos")]
        [ThreadStatic] public uint limiteBocinazos;

        // Representa cuántos bocinazos está escuchando en un instante determinado
        [ThreadStatic] public uint bocinazos = 0;

        // Representa el bloqueo en la operación de pagar
        [ThreadStatic] public object objPago;
        #endregion

        public Semaphore semCola = new Semaphore(0, 10);

        public CabinaPeaje(uint numero, int demora)
        {
            this.numero = numero;
            this.demora = demora;
            this.vehiculos = new Queue<string>();
            this.objPago = new Object();
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
                    //Monitor.Enter(this.objPago);

                    Console.WriteLine($"Cabina número {this.numero} - Operando con vehículo {vehiculos.Peek()}");
                    Thread.Sleep(demora);
                    Console.WriteLine($"Cabina número {this.numero} - Barrera levantada, pasa el vehículo {vehiculos.Dequeue()}");
                    if (vehiculos.Count > 0)
                    {
                        Console.WriteLine($"Cabina número {this.numero} - Aviso: Pueden avanzar los {vehiculos.Count} vehículos que están esperando");
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
            Console.WriteLine($"Cabina número {this.numero} - Ya no tolera los bocinazos. Levantando barrera...");
            while (bocinazos > limiteBocinazos / 2)
            {
                Monitor.Enter(this.objPago);
                Thread.Sleep(100);
                Console.WriteLine($"Cabina número {this.numero} - Barrera levantada, pasa el vehículo {vehiculos.Dequeue()}");
                this.semCola.Release();

                Monitor.Pulse(this.objPago);
                Monitor.Exit(this.objPago);
            }
            Console.WriteLine($"Cabina número {this.numero} - Funcionamiento normal reestablecido");
        }
    }
}
