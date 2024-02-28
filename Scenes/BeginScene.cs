using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;
using SME;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;

namespace MyGame
{    
    /// <summary>
    /// The first Scene of your game, don't rename or remove it.
    /// </summary>
    class BeginScene : Scene
    {
        private Point resolution = new Vector2(2560f * 0.98f, 1440f * 0.98f).ToPoint();//nnbPiexel en x et en y
        private Vector2 domainSize;
        private Vector2 center;
        private int maxIter = 200;
        private Color[] mandelbrot;
        private float[] iterPercentage;
        private Color cIn, cOut;
        private Texture2D image;

        //Inputs
        float mouseWheelScale; //%age de réduction/augmentation des dim du domaine
        Vector2 oldMousePos;

        //benchmark
        private bool useGPU;
        private bool useCPUMultiThreadings;
        int totalImageCalculated = 0;
        float totalTimeSpend = 0f;

        //GPU
        dynamic wrapper;

        public BeginScene() : base()
        {
            cIn = Color.Black;
            cOut = Color.CadetBlue;
            center = Vector2.Zero;
            float sizex = 4f;
            domainSize = new Vector2(sizex, sizex * ((float)resolution.Y / resolution.X));
            useGPU = true;
            useCPUMultiThreadings = true;

            mouseWheelScale = 0.3f;
        }

        public override void Load()
        {
            base.Load();
            Screen.SetDevelopementScreen(resolution.ToVector2());
            Screen.ChangeResolution(resolution.X, resolution.Y);
            mandelbrot = new Color[resolution.X * resolution.Y];
            iterPercentage = new float[mandelbrot.Length];
            oldMousePos = Input.GetMousePosition();

            //GPU
            if(useGPU)
            {
                /*
                cuda.GetDeviceProperties(out cudaDeviceProp prop, 0);
                Debug.WriteLine("Ma carte graphique : " + prop.name);

                GPURunner = HybRunner.Cuda().SetDistrib(prop.multiProcessorCount, 512);
                wrapper = GPURunner.Wrap(new GPUProgram());
                */
            }

            CalculateImage();
        }        

        private void CalculateImage()
        {
            if (useGPU)
                CalculateImageGPU();
            else
                CalculateImageCPU();
        }

        #region CPU

        private void CalculateImageCPU()
        {
            StartCalculusCPU();
            CreateTexture();
        }

        private float CalculateMandelbrotColorCPU(ComplexD c)
        {
            ComplexD current = ComplexD.zero;
            int iter = 0;
            while(iter < maxIter)
            {
                current = current.Sqr() + c;
                iter++;
                if (current.SqrModule > 4f)
                {
                    return ((float)iter) / maxIter;
                }
            }
            return 1f;
        }

        private void StartCalculusCPU()
        {
            double xStep = domainSize.X / resolution.X;
            double yStep = domainSize.Y / resolution.Y;

            if(useCPUMultiThreadings)
            {
                double xBeg = center.X - domainSize.X * 0.5d + xStep * 0.5d;
                double yBeg = center.Y - domainSize.Y * 0.5d + yStep * 0.5d;

                Action<int> f = (int i) =>
                {
                    int x = i % resolution.X;
                    int y = i / resolution.X;
                    iterPercentage[i] = CalculateMandelbrotColorCPU(new ComplexD(xBeg + x * xStep, yBeg + y * yStep));
                };

                Parallel.For(0, resolution.X * resolution.Y, f);
            }
            else
            {
                ComplexD currentPoint = new ComplexD(center.X - domainSize.X * 0.5d + xStep * 0.5d, center.Y - domainSize.Y * 0.5d + yStep * 0.5d);
                double xBeg = currentPoint.a;

                int tmp;
                for (int y = 0; y < resolution.Y; y++)
                {
                    tmp = y * resolution.X;
                    for (int x = 0; x < resolution.X; x++)
                    {
                        iterPercentage[x + tmp] = CalculateMandelbrotColorCPU(currentPoint);
                        currentPoint = new ComplexD(currentPoint.a + xStep, currentPoint.b);
                    }
                    currentPoint = new ComplexD(xBeg, currentPoint.b + yStep);
                }
            }
        }

        #endregion

        #region GPU

        private void CalculateImageGPU()
        {
            //StartCalculusGPU(iterPercentage, center.X, center.Y, domainSize.X, domainSize.Y, resolution.X, resolution.Y, maxIter);
            GPUProgram.StartCalculusGPU(iterPercentage, center.X, center.Y, domainSize.X, domainSize.Y, resolution.X, resolution.Y, maxIter);
            CreateTexture();
        }

        public class GPUProgram
        {
            [Kernel]
            public static float CalculateMandelbrotColorGPU(double cx, double cy, int maxIter)
            {
                double x = 0d;
                double y = 0d;
                double tmp;

                int iter = 0;
                while (iter < maxIter)
                {
                    tmp = x * x - y * y + cx;
                    y = 2.0d * x * y + cy;
                    x = tmp;
                    iter++;
                    if (x * x + y * y > 4f)
                    {
                        return ((float)iter) / maxIter;
                    }
                }
                return 1f;
            }

            [EntryPoint]
            public static void StartCalculusGPU(float[] percentage, double centerX, double centerY, double domainSizeX, double domainSizeY, int resX, int resY, int maxIter)
            {
                double xStep = domainSizeX / resX;
                double yStep = domainSizeY / resY;

                double xBeg = centerX - domainSizeX * 0.5d + xStep * 0.5d;
                double yBeg = centerY - domainSizeY * 0.5d + yStep * 0.5d;

                Action<int> f = (int i) =>
                {
                    int x = i % resX;
                    int y = i / resX;
                    percentage[i] = CalculateMandelbrotColorGPU(xBeg + x * xStep, yBeg + y * yStep, maxIter);
                };

                Parallel.For(0, resX * resY, f);
            }
        }

        #endregion

        private void CreateTexture()
        {
            image = new Texture2D(MainGame.mainGame.GraphicsDevice, resolution.X, resolution.Y, false, SurfaceFormat.Color);
            int end = resolution.X * resolution.Y;
            for (int i = 0; i < end; i++)
            {
                mandelbrot[i] = Color.Lerp(cOut, cIn, MathF.Pow(iterPercentage[i], 0.5f));
            }

            image.SetData(mandelbrot);
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Update()
        {
            base.Update();

            bool recalculateImage = false;

            Vector2 mousePos = Input.GetMousePosition();
            if (Input.GetKey(Input.MouseButtons.left) && Input.MouseVelocity().LengthSquared() >= 1f)
            {
                recalculateImage = true;
                center -= ((mousePos - oldMousePos) / resolution.ToVector2()) * domainSize;
            }

            if(Input.MouseWheel(out Input.MouseWheelDirection dir))
            {
                switch (dir)
                {
                    case Input.MouseWheelDirection.up:
                        domainSize *= (1f - mouseWheelScale);
                        recalculateImage = true;
                        break;
                    case Input.MouseWheelDirection.down:
                        domainSize *= (1f + mouseWheelScale);
                        recalculateImage = true;
                        break;
                    case Input.MouseWheelDirection.none:
                        break;
                    default:
                        break;
                }
            }

            if(Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
            {
                maxIter = (int)(maxIter * 1.33f);
                recalculateImage = true;
                Debug.WriteLine("The max iteration in now : " + maxIter);
            }
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.O))
            {
                maxIter = (int)(maxIter / 1.33f);
                recalculateImage = true;
                Debug.WriteLine("The max iteration in now : " + maxIter);
            }

            if (recalculateImage)
            {
                DateTime now = DateTime.Now;

                CalculateImage();

                TimeSpan deltaTime = DateTime.Now - now;
                totalTimeSpend += (float)deltaTime.TotalMilliseconds;
                totalImageCalculated++;
                Debug.WriteLine("Calculus takes in average " + totalTimeSpend / totalImageCalculated + " ms with double floating numbers");
            }

            oldMousePos = mousePos;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(image, new Rectangle(Point.Zero, resolution), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
