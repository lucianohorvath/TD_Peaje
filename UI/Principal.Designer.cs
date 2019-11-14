namespace UI
{
    partial class Principal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Principal));
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.labelCeroMetros = new System.Windows.Forms.Label();
            this.labelFinalMetros = new System.Windows.Forms.Label();
            this.buttonConfiguration = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 200;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // labelCeroMetros
            // 
            this.labelCeroMetros.AutoSize = true;
            this.labelCeroMetros.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.labelCeroMetros.Location = new System.Drawing.Point(492, 719);
            this.labelCeroMetros.Name = "labelCeroMetros";
            this.labelCeroMetros.Size = new System.Drawing.Size(67, 18);
            this.labelCeroMetros.TabIndex = 0;
            this.labelCeroMetros.Text = "0 metros";
            // 
            // labelFinalMetros
            // 
            this.labelFinalMetros.AutoSize = true;
            this.labelFinalMetros.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.labelFinalMetros.Location = new System.Drawing.Point(492, 154);
            this.labelFinalMetros.Name = "labelFinalMetros";
            this.labelFinalMetros.Size = new System.Drawing.Size(83, 18);
            this.labelFinalMetros.TabIndex = 1;
            this.labelFinalMetros.Text = "100 metros";
            // 
            // buttonConfiguration
            // 
            this.buttonConfiguration.Location = new System.Drawing.Point(495, 13);
            this.buttonConfiguration.Name = "buttonConfiguration";
            this.buttonConfiguration.Size = new System.Drawing.Size(91, 23);
            this.buttonConfiguration.TabIndex = 2;
            this.buttonConfiguration.Text = "Configuración";
            this.buttonConfiguration.UseVisualStyleBackColor = true;
            this.buttonConfiguration.Click += new System.EventHandler(this.buttonConfiguration_Click);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 780);
            this.Controls.Add(this.buttonConfiguration);
            this.Controls.Add(this.labelFinalMetros);
            this.Controls.Add(this.labelCeroMetros);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Principal";
            this.Text = "Simulador de Peaje";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Principal_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Label labelCeroMetros;
        private System.Windows.Forms.Label labelFinalMetros;
        private System.Windows.Forms.Button buttonConfiguration;
    }
}

