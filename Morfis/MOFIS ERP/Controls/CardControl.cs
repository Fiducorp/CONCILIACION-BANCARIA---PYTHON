using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOFIS_ERP.Controls
{
    /// <summary>
    /// Control personalizado tipo Card con animaciones para navegación
    /// </summary>
    public partial class CardControl : UserControl
    {
        // Colores corporativos
        private Color colorPrimario = Color.FromArgb(0, 120, 212);      // #0078D4
        private Color colorHover = Color.FromArgb(0, 90, 158);          // #005A9E
        private Color colorSeleccionado = Color.FromArgb(0, 60, 112);   // Más oscuro
        private Color colorFondo = Color.White;
        private Color colorTexto = Color.FromArgb(51, 51, 51);

        // Estados
        private bool isHovered = false;
        private bool isSelected = false;

        // Animación
        private Timer animationTimer;
        private float currentScale = 1.0f;
        private float targetScale = 1.0f;
        private float currentElevation = 4f;
        private float targetElevation = 4f;
        private const float scaleSpeed = 0.3f;
        private const float elevationSpeed = 0.3f;

        // Propiedades públicas
        private string titulo = "Card Title";
        private string icono = "📋";
        private string descripcion = "";

        /// <summary>
        /// Título de la card
        /// </summary>
        public string Titulo
        {
            get => titulo;
            set
            {
                titulo = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Icono emoji de la card
        /// </summary>
        public string Icono
        {
            get => icono;
            set
            {
                icono = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Descripción breve (opcional)
        /// </summary>
        public string Descripcion
        {
            get => descripcion;
            set
            {
                descripcion = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Evento que se dispara al hacer clic en la card
        /// </summary>
        public event EventHandler CardClick;

        public CardControl()
        {
            InitializeComponent();

            // Configurar propiedades base del control
            this.BackColor = Color.Transparent;
            this.Size = new Size(320, 280);
            this.Cursor = Cursors.Hand;

            // Inicializar elevación base (sombra flotante siempre visible)
            targetElevation = 4f;
            currentElevation = 4f;

            ConfigurarControl();
        }

        private void ConfigurarControl()
        {
            // Habilitar doble buffer para animaciones suaves
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.SupportsTransparentBackColor, true);

            // Eventos de mouse para animaciones
            this.MouseEnter += CardControl_MouseEnter;
            this.MouseLeave += CardControl_MouseLeave;
            this.Click += CardControl_Click;

            // Inicializar timer de animación
            animationTimer = new Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void CardControl_MouseEnter(object sender, EventArgs e)
        {
            isHovered = true;
            targetScale = 1.08f; // Agrandar 8%
                                 // Mantener elevación constante
        }

        private void CardControl_MouseLeave(object sender, EventArgs e)
        {
            isHovered = false;
            targetScale = 1.0f;
            // Mantener elevación constante
        }

        private void CardControl_Click(object sender, EventArgs e)
        {
            // Efecto de rebote (bounce) suave
            isSelected = true;
            targetScale = 0.96f; // Comprimir

            Timer bounceTimer = new Timer { Interval = 80 };
            bounceTimer.Tick += (s, args) =>
            {
                targetScale = 1.08f; // Expandir
                bounceTimer.Stop();

                Timer resetTimer = new Timer { Interval = 100 };
                resetTimer.Tick += (s2, args2) =>
                {
                    isSelected = false;
                    targetScale = isHovered ? 1.08f : 1.0f;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();

                bounceTimer.Dispose();
            };
            bounceTimer.Start();

            // Invocar evento personalizado
            CardClick?.Invoke(this, e);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            bool needsRedraw = false;

            // Animar escala con interpolación suave
            if (Math.Abs(currentScale - targetScale) > 0.001f)
            {
                currentScale += (targetScale - currentScale) * scaleSpeed;
                needsRedraw = true;
            }

            // Animar elevación
            if (Math.Abs(currentElevation - targetElevation) > 0.1f)
            {
                currentElevation += (targetElevation - currentElevation) * elevationSpeed;
                needsRedraw = true;
            }

            if (needsRedraw)
                Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Calcular transformación de escala
            int scaledWidth = (int)(this.Width * currentScale);
            int scaledHeight = (int)(this.Height * currentScale);
            int offsetX = (this.Width - scaledWidth) / 2;
            int offsetY = (this.Height - scaledHeight) / 2;

            // Determinar colores según estado
            Color colorFondoCard = Color.White;
            Color colorBorde = colorPrimario;
            Color colorIcono = Color.Black;

            if (isSelected)
            {
                colorFondoCard = Color.FromArgb(240, 245, 250);
                colorBorde = colorSeleccionado;
            }
            else if (isHovered)
            {
                colorFondoCard = Color.FromArgb(248, 252, 255);
                colorBorde = colorHover;
                colorIcono = colorHover;
            }

            // Dibujar sombra con efecto de profundidad (box-shadow)
            int shadowLayers = Math.Max(1, (int)currentElevation);
            for (int i = 0; i < shadowLayers; i++)
            {
                int alpha = Math.Max(10, 60 - (i * 4));
                int blur = i * 2;

                Rectangle shadowRect = new Rectangle(
                    offsetX + blur,
                    offsetY + blur + (i / 2),
                    scaledWidth - (blur * 2),
                    scaledHeight - (blur * 2)
                );

                using (GraphicsPath path = GetRoundedRect(shadowRect, 14))
                {
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(path))
                    {
                        shadowBrush.CenterColor = Color.FromArgb(alpha, 0, 0, 0);
                        shadowBrush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                        g.FillPath(shadowBrush, path);
                    }
                }
            }

            // Dibujar fondo de la card con bordes redondeados
            Rectangle cardRect = new Rectangle(offsetX, offsetY, scaledWidth - 1, scaledHeight - 1);
            using (GraphicsPath path = GetRoundedRect(cardRect, 12))
            {
                using (SolidBrush brush = new SolidBrush(colorFondoCard))
                {
                    g.FillPath(brush, path);
                }

                using (Pen pen = new Pen(colorBorde, 2.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Dibujar icono (emoji) - SIN escalar para suavidad
            using (Font iconFont = new Font("Segoe UI Emoji", 60, FontStyle.Regular))
            {
                using (SolidBrush iconBrush = new SolidBrush(colorIcono))
                {
                    SizeF iconSize = g.MeasureString(icono, iconFont);
                    PointF iconPos = new PointF(
                        offsetX + (scaledWidth - iconSize.Width) / 2,
                        offsetY + 35
                    );
                    g.DrawString(icono, iconFont, iconBrush, iconPos);
                }
            }

            // Dibujar título con MÁS espacio respecto al emoji - SIN escalar
            using (Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold))
            {
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    RectangleF titleRect = new RectangleF(
                        offsetX + 15,
                        offsetY + 150, // MÁS espacio desde el emoji
                        scaledWidth - 30,
                        45
                    );
                    using (SolidBrush brush = new SolidBrush(colorTexto))
                    {
                        g.DrawString(titulo, titleFont, brush, titleRect, sf);
                    }
                }
            }

            // Dibujar descripción - SIN escalar
            if (!string.IsNullOrEmpty(descripcion))
            {
                using (Font descFont = new Font("Segoe UI", 9.5f, FontStyle.Regular))
                {
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;

                        RectangleF descRect = new RectangleF(
                            offsetX + 15,
                            offsetY + 210,
                            scaledWidth - 30,
                            50
                        );
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, 120, 120)))
                        {
                            g.DrawString(descripcion, descFont, brush, descRect, sf);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Crea un rectángulo con bordes redondeados
        /// </summary>
        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}