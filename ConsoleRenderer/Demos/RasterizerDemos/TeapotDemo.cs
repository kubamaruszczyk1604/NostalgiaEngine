using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
using NostalgiaEngine.RasterizerPipeline;
namespace NostalgiaEngine.Demos
{
    public class TeapotDemo: Scene3D
    {

        public override bool OnLoad()
        {
            SceneSkybox = new Skybox("RasterizerDemoResources/skybox1");
            Mesh teapotMesh = NEObjLoader.LoadObj("RasterizerDemoResources/teapot.obj", 14);
            Model teapotModel = new Model(teapotMesh, CullMode.None);
            teapotModel.Transform.ScaleX = 0.5f;
            teapotModel.Transform.ScaleY = 0.5f;
            teapotModel.Transform.ScaleZ = 0.5f;
            teapotModel.Transform.LocalPosition = new NEVector4(0.0f, -0.5f, -1.0f, 1.0f);

            Models.Add(teapotModel);

            AddLight(new DirectionalLight(new NEVector4(-1.0f, 1.0f, 0.0f)));
            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 0.0f, -5.3f);


            NEColorPalette pal = NEColorPalette.FromFile("RasterizerDemoResources/palette.txt");
            pal.MultiplyBy(2.8f);
            NEColorManagement.SetPalette(pal);
            return base.OnLoad();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (NEInput.CheckKeyPress(ConsoleKey.P))
            {
                TogglePalette();
            }

            if (NEInput.CheckKeyPress(ConsoleKey.H))
            {
                ToggleHeadlamp();
            }

            Movement(deltaTime);
            NEScreenBuffer.Clear();

            base.OnUpdate(deltaTime);
        }
        public override bool OnDraw()
        {
            return base.OnDraw();
        }

        public override void OnDrawPerColumn(int x)
        {
            base.OnDrawPerColumn(x);
        }

        protected override NEColorSample OnSkyboxSample(NEVector4 direction, float sampledValue)
        {
            int low = 0;
            int high = 8;
            if (sampledValue > 0.7f)
            {
                low = 6;
                high = 12;
            }
            return NEColorSample.MakeColFromBlocks10((ConsoleColor)low, (ConsoleColor)high, sampledValue);
        }


        private void Movement(float dt)
        {
            if (NEInput.CheckKeyDown(ConsoleKey.LeftArrow))
            {
                if (NEInput.CheckKeyDown(NEKey.Alt))
                {
                    MainCamera.Transform.LocalPosition = MainCamera.Transform.LocalPosition - MainCamera.Transform.Right * dt;
                }
                else
                {
                    MainCamera.Transform.RotateY(dt);
                }
            }

            if (NEInput.CheckKeyDown(ConsoleKey.RightArrow))
            {
                if (NEInput.CheckKeyDown(NEKey.Alt))
                {
                    MainCamera.Transform.LocalPosition = MainCamera.Transform.LocalPosition + MainCamera.Transform.Right * dt;
                }
                else
                {
                    MainCamera.Transform.RotateY(-dt);
                }
            }

            if (NEInput.CheckKeyDown(ConsoleKey.UpArrow))
            {
                if (NEInput.CheckKeyDown(NEKey.Alt))
                {
                    MainCamera.Transform.LocalPosition = MainCamera.Transform.LocalPosition + MainCamera.Transform.Up * dt;
                }
                else
                {
                    MainCamera.Transform.LocalPosition = MainCamera.Transform.LocalPosition + MainCamera.Transform.Forward * dt;
                }
            }

            if (NEInput.CheckKeyDown(ConsoleKey.DownArrow))
            {

                if (NEInput.CheckKeyDown(NEKey.Alt))
                {
                    MainCamera.Transform.LocalPosition = MainCamera.Transform.LocalPosition - MainCamera.Transform.Up * dt;
                }
                else
                {
                    MainCamera.Transform.LocalPosition = MainCamera.Transform.LocalPosition - MainCamera.Transform.Forward * dt;
                }

            }

            if (NEInput.CheckKeyDown(ConsoleKey.W))
            {
                MainCamera.Transform.RotateX(-dt);
            }

            if (NEInput.CheckKeyDown(ConsoleKey.S))
            {
                MainCamera.Transform.RotateX(dt);
            }
        }
    }
}
