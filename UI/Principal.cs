using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Principal : Form
    {
        // Constantes
        private const int AnchoCabina = 40;
        private const int AltoCabina = 50;
        private const float escalaImagen = 0.14F;
        private const int largoDeAuto = 4;          // metros de largo considerados en el Modelo

        // Atributos
        private Image redCar;
        private Image yellowCar;
        private Image speaker;
        private Image toll;
        private IEnumerable<Vehiculo> vehiculos;
        private float factorDeCorreccion;

        public Principal()
        {
            InitializeComponent();
            LoadImages();
            InitConfig();
            FixUI();
            Thread t = new Thread(this.startSimulation);
            t.IsBackground = true;
            t.Start();
        }

        private void LoadImages()
        {
            redCar = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("car_red"));
            yellowCar = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("car_yellow"));
            speaker = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("speaker"));
            toll = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("toll_booth"));
        }

        /// <summary>
        /// Inicializa todas las configuraciones necesarias para el funcionamiento de la UI.
        /// </summary>
        private void InitConfig()
        {
            timerRefresh.Interval = Configuracion.Facade.getRefrescoPantalla();
            factorDeCorreccion = redCar.Height * escalaImagen / largoDeAuto;
            vehiculos = Configuracion.Facade.getVehiculos();
        }

        private void FixUI()
        {
            labelFinalMetros.Text = $"{Vehiculo.finalAutopista} metros";
            labelFinalMetros.Location = new Point(labelFinalMetros.Location.X, this.Height - (int)(Vehiculo.finalAutopista * factorDeCorreccion));
        }

        /// <summary>
        /// Maneja el evento disparado por el timer asociado al formulario.
        /// </summary>
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            // Obligo a redibujar todo el formulario
            Invalidate();
        }

        /// <summary>
        /// Dispara la ejecución de todos los threads necesarios para la simulación
        /// </summary>
        private void startSimulation()
        {
            // Damos tiempo para que se levante el entorno gráfico...
            Thread.Sleep(1000);

            CabinaPeaje cabina1 = Configuracion.Facade.getCabinasPeaje().First();
            int frecuenciaVehiculos = Configuracion.Facade.getFrecuenciaVehiculos();
            Vehiculo.velocidadRefresco = Configuracion.Facade.getRefrescoConsola();

            Thread th = new Thread(cabina1.Operar);
            th.Name = "Thread Cabina 1";
            th.IsBackground = true;
            th.Start();

            Thread[] threadsVehiculo = new Thread[vehiculos.Count()];
            int i = 0;
            foreach (Vehiculo v in vehiculos)
            {
                v.cabina = cabina1;
                threadsVehiculo[i] = new Thread(v.Conducir);
                threadsVehiculo[i].Name = $"Thread Vehículo {i+1}";
                threadsVehiculo[i].IsBackground = true;
                threadsVehiculo[i].Start();

                Thread.Sleep(frecuenciaVehiculos);
                i++;
            }

            foreach (Thread t in threadsVehiculo)
            {
                t.Join();
            }
            preguntarReinicio();
        }

        /// <summary>
        /// Pregunta al usuario si desea reiniciar la simulación o cerrar la aplicación.
        /// </summary>
        private void preguntarReinicio()
        {
            DialogResult res = MessageBox.Show("¿Desea volver a realizar la simulación?", 
                "Fin de la simulación", 
                MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                vehiculos = Configuracion.Facade.getVehiculos();
                startSimulation();
            }
            else if (res == DialogResult.No)
            {
                MessageBox.Show("¡Hasta la próxima!");
                this.Invoke((MethodInvoker)delegate
                {
                    this.Close();
                });
            }
        }

        #region Gráficos
        /// <summary>
        /// Maneja el evento que redibuja el formulario.
        /// </summary>
        private void Principal_Paint(object sender, PaintEventArgs e)
        {
            // Invierto el eje Y-Axis para dibujar desde esquina inferior izquierda
            e.Graphics.ScaleTransform(1.0F, -1.0F);
            e.Graphics.TranslateTransform(0.0F, -(float)Height);
                        
            dibujarLineasPrincipioYFin(e.Graphics);

            // Cabinas de peaje
            //e.Graphics.FillRectangle(Brushes.Blue, 15, 50 * factorDeCorreccion + AltoCabina, AnchoCabina, AltoCabina);
            //e.Graphics.FillRectangle(Brushes.LightYellow, 15, 50 * factorDeCorreccion, this.Width - 100, AltoCabina);    // linea de epsera de autos
            // ver po que no coinciden perfectamente los autos en la linea de espera
            e.Graphics.DrawImage(toll, 15, 50 * factorDeCorreccion + AltoCabina, AnchoCabina, AltoCabina);

            // Autos
            int i = 0;
            foreach (Vehiculo v in vehiculos)
            {
                //Console.WriteLine((v.posicion * factorDeCorreccion) + redCar.Height * escalaImagen);
                e.Graphics.DrawImage(i % 2 == 0 ? redCar : yellowCar, 30, (v.posicion * factorDeCorreccion) + redCar.Height * escalaImagen, redCar.Width * escalaImagen, redCar.Height * escalaImagen);
                if (v.impaciente)
                {
                    // Dibujo el parlante con misma posición y tamaño que vehículo pero desplazado en eje X
                    e.Graphics.DrawImage(speaker, 30 + redCar.Width * escalaImagen, (v.posicion * factorDeCorreccion) + redCar.Height * escalaImagen, speaker.Width * escalaImagen, speaker.Height * escalaImagen);
                }
                i++;
            }
        }

        private void dibujarLineasPrincipioYFin(Graphics graphics)
        {
            Pen p = new Pen(Brushes.Black, 15);
            graphics.DrawLine(p, 10, 60, this.Width - 100, 60);
            graphics.DrawLine(p, 10, Vehiculo.finalAutopista * factorDeCorreccion + 15, this.Width - 100, Vehiculo.finalAutopista * factorDeCorreccion + 15);
        }
        #endregion

        private void buttonConfiguration_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Configuracion.Facade.getFilePath());
        }
    }
}
