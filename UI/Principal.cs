using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Principal : Form
    {
        #region Constantes
        private const int anchoCabina = 40;
        private const int altoCabina = 50;
        private const int distanciaEntreCabinas = 140;
        private const float escalaImagen = 0.14F;
        private const int largoDeAuto = 4;          // metros de largo considerados en el Modelo
        #endregion

        #region Atributos
        private Image redCar;
        private Image yellowCar;
        private Image speaker;
        private Image toll;
        private Image semaphore;
        private SoundPlayer carHorn1;
        private SoundPlayer carHorn2;
        private IEnumerable<Vehiculo> vehiculos;
        private IEnumerable<CabinaPeaje> cabinas;
        private float factorDeCorreccion;
        #endregion

        public Principal()
        {
            InitializeComponent();
            LoadImages();
            LoadSounds();
            InitConfig();
            FixUI();
            Thread t = new Thread(this.startSimulation);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Carga todas las imágenes en bitmaps una sola vez al levantar el proyecto.
        /// Rota las imágenes ya que se trabaja con un eje cartesiano invertido.
        /// </summary>
        private void LoadImages()
        {
            redCar = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("car_red"));
            yellowCar = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("car_yellow"));
            speaker = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("speaker"));
            toll = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("toll_booth"));
            semaphore = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("semaphore"));
            toll.RotateFlip(RotateFlipType.Rotate180FlipX);
            semaphore.RotateFlip(RotateFlipType.Rotate180FlipX);
        }

        private void LoadSounds()
        {
            carHorn1 = new SoundPlayer(Properties.Resources.car_horn_1);
            carHorn2 = new SoundPlayer(Properties.Resources.car_horn_2);
        }

        /// <summary>
        /// Inicializa todas las configuraciones necesarias para el funcionamiento de la UI.
        /// </summary>
        private void InitConfig()
        {
            timerRefresh.Interval = Configuracion.Facade.getRefrescoPantalla();
            factorDeCorreccion = redCar.Height * escalaImagen / largoDeAuto;
            vehiculos = Configuracion.Facade.getVehiculos();
            cabinas = Configuracion.Facade.getCabinasPeaje();
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

            int frecuenciaVehiculos = Configuracion.Facade.getFrecuenciaVehiculos();
            Vehiculo.velocidadRefresco = Configuracion.Facade.getRefrescoConsola();
            bool carrilAleatorio = Configuracion.Facade.getCarrilesAleatorios();

            int j = 1;
            foreach(CabinaPeaje cp in cabinas)
            {
                Thread th = new Thread(cp.Operar);
                th.Name = $"Thread Cabina {j}";
                th.IsBackground = true;
                th.Start();
                j++;
            }

            Thread[] threadsVehiculo = new Thread[vehiculos.Count()];
            int i = 0;
            foreach (Vehiculo v in vehiculos)
            {
                if (carrilAleatorio)
                {
                    Random random = new Random();
                    int numeroCabina = random.Next(1, cabinas.Count() + 1);
                    v.carril = (uint)numeroCabina;
                }
                v.cabina = cabinas.ToList().Find(c => c.numero == v.carril);
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
                // Reinicializo configuración antes de empezar
                InitConfig();
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
            dibujarCabinas(e.Graphics);
            dibujarVehiculos(e.Graphics);
        }

        private void dibujarLineasPrincipioYFin(Graphics graphics)
        {
            Pen p = new Pen(Brushes.Black, 15);
            graphics.DrawLine(p, 10, 60, this.Width - 100, 60);
            graphics.DrawLine(p, 10, Vehiculo.finalAutopista * factorDeCorreccion + 15, this.Width - 100, Vehiculo.finalAutopista * factorDeCorreccion + 15);
        }

        private void dibujarCabinas(Graphics graphics)
        {
            // Línea de espera de los vehículos
            graphics.FillRectangle(Brushes.LightYellow, 15, 50 * factorDeCorreccion, this.Width - 100, altoCabina);

            int xPos = 25;
            foreach(CabinaPeaje cp in cabinas)
            {
                graphics.DrawImage(toll, xPos, 50 * factorDeCorreccion + altoCabina, anchoCabina, altoCabina);
                if (cp.barreraLevantada)
                {
                    graphics.DrawImage(semaphore, xPos - 20, 50 * factorDeCorreccion + altoCabina, 23, 46);
                }
                xPos += distanciaEntreCabinas;
            }
        }

        private void dibujarVehiculos(Graphics graphics)
        {
            int i = 0;
            foreach (Vehiculo v in vehiculos)
            {
                int xPos = 40 + (((int)v.carril - 1) * distanciaEntreCabinas);
                float yPos = (v.posicion * factorDeCorreccion) + redCar.Height * escalaImagen;

                graphics.DrawImage(i % 2 == 0 ? redCar : yellowCar, 
                    xPos, yPos, 
                    redCar.Width * escalaImagen, redCar.Height * escalaImagen);

                if (v.impaciente)
                {
                    // Dibujo el parlante a la derecha del vehículo
                    graphics.DrawImage(speaker, xPos + redCar.Width * escalaImagen, yPos, speaker.Width * escalaImagen, speaker.Height * escalaImagen);

                    // Hago sonar una bocina
                    if (i % 2 == 0)
                    {
                        carHorn1.Play();
                    } 
                    else
                    {
                        carHorn2.Play();
                    }
                }
                i++;
            }
        }
        #endregion

        private void buttonConfiguration_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Configuracion.Facade.getFilePath());
        }
    }
}
