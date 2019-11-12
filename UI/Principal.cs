using ConsolaApp;
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
        private const int AnchoCabina = 30;
        private const int AltoCabina = 50;
        private const float escalaImagen = 0.1F;
        private const int largoDeAuto = 4;          // metros de largo considerados en el Modelo

        // Atributos
        private Image carImage;
        private IEnumerable<Vehiculo> vehiculos;
        private float factorDeCorreccion;

        public Principal()
        {
            InitializeComponent();
            LoadImages();
            factorDeCorreccion = carImage.Height * escalaImagen / largoDeAuto;
            vehiculos = ConfigManager.getVehiculos();
            Thread t = new Thread(this.startThreads);
            t.IsBackground = true;
            t.Start();
            //startThreads();
        }

        private void LoadImages()
        {
            carImage = new Bitmap((Image)Properties.Resources.ResourceManager.GetObject("rotated_car"));
        }

        /// <summary>
        /// Maneja el evento disparado por el timer asociado al formulario.
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Obligo a redibujar todo el formulario
            Invalidate();
        }

        /// <summary>
        /// Maneja el evento que redibuja el formulario.
        /// </summary>
        private void Principal_Paint(object sender, PaintEventArgs e)
        {
            // Invierto el eje Y-Axis para dibujar desde esquina inferior izquierda
            e.Graphics.ScaleTransform(1.0F, -1.0F);
            e.Graphics.TranslateTransform(0.0F, -(float)Height);

            Pen p = new Pen(Brushes.Black, 15);
            e.Graphics.DrawLine(p, 10, 60, this.Width - 100, 60);
            e.Graphics.DrawLine(p, 10, 100*factorDeCorreccion+15, this.Width - 100, 100 * factorDeCorreccion + 15);

            // Cabinas de peaje
            e.Graphics.FillRectangle(Brushes.Blue, 15, 50 * factorDeCorreccion + AltoCabina, AnchoCabina, AltoCabina);

            // Autos
            foreach (Vehiculo v in vehiculos)
            {
                e.Graphics.DrawImage(carImage, 20, (v.posicion * factorDeCorreccion) + carImage.Height * escalaImagen, carImage.Width * escalaImagen, carImage.Height * escalaImagen);
            }
        }

        private void startThreads()
        {
            // Damos tiempo a que se levante el entorno gráfico...
            Thread.Sleep(2000);

            CabinaPeaje cabina1 = ConfigManager.getCabinasPeaje().First();
            int frecuenciaVehiculos = ConfigManager.getFrecuenciaVehiculos();

            Thread th = new Thread(cabina1.Operar);
            th.Name = "Thread Cabina 1";
            th.IsBackground = true;
            th.Start();

            int i = 1;
            foreach (Vehiculo v in vehiculos)
            {
                v.cabina = cabina1;

                Thread t = new Thread(v.Conducir);
                t.Name = $"Thread Vehículo {i}";
                t.IsBackground = true;
                t.Start();
                Thread.Sleep(frecuenciaVehiculos);

                i++;
            }
        }
    }
}
