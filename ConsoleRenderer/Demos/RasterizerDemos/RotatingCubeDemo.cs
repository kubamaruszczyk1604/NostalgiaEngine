using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
using NostalgiaEngine.RasterizerPipeline;
namespace NostalgiaEngine.Demos
{
    public class RotatingCubeDemo: Scene3D
    {

        // Loads resources. Called once, when the scene is loaded.
        public override bool OnLoad()
        {
            ScreenWidth = 240;
            ScreenHeight = 150;
            //ScreenWidth = 350;
            //ScreenHeight = 230;
            PixelWidth = 4;
            PixelHeight = 4;
            Mesh cubeMesh = GeometryGenerator.GenerateCube2(1.0f, 1.0f, 1.0f, NEVector4.Zero, 9);
            var texture = ResourceManager.Instance.GetLumaTexture("RasterizerDemoResources/uv_test_tex/luma.buf");
            Model cubeModel = new Model(cubeMesh, CullMode.Back, texture);


            Models.Add(cubeModel);

           AddLight(new DirectionalLight(new NEVector4(-1.0f, 1.0f, -3.0f)));

            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 0.3f, -4.3f);

            TogglePalette();

            return base.OnLoad();
        }

        // Called once per frame
        public override void OnUpdate(float deltaTime)
        {
            //animate 
            Models[0].Transform.RotateY(deltaTime * 0.5f);
            Models[0].Transform.RotateZ(deltaTime * 0.75f);
            Models[0].Transform.PositionY = (float)(Math.Sin(Engine.Instance.TotalTime) * 0.1);

            // Show/hide color palette strip
            if (NEInput.CheckKeyPress(ConsoleKey.P))
            {
                TogglePalette();
            }

            // Enable/disable default headlamp
            if (NEInput.CheckKeyPress(ConsoleKey.H))
            {
                ToggleHeadlamp();
            }
            if(NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }

            //FPS like camera movement
            Movement(deltaTime);

            //clear screen
            NEScreenBuffer.Clear();

            base.OnUpdate(deltaTime);
        }



        //Called for each column of screen characters. Multiple instances of this method are called in parallel during each frame. 
        public override void OnDrawPerColumn(int x)
        {
            // this part perform the rasterization
            base.OnDrawPerColumn(x);
        }

        //Called once per frame. If returns true screen buffer will be drawn
        public override bool OnDraw()
        {
            //this part draws the skybox
            return base.OnDraw();
        }

        //this makes the skybox prettier, but it's optional
        protected override NEColorSample OnSkyboxSample(NEVector4 direction, float sampledValue)
        {
            // direction is the direction vector used to sample the skybox
            // sampledValue is the intensity value sampled from the skybox (in range from 0 to 1)

            int color0 = 0;
            int color1 = 8;
            if (sampledValue > 0.7f)
            {
                color0 = 6;
                color1 = 12;
            }
            return NEColorSample.MakeColFromBlocks10((ConsoleColor)color0, (ConsoleColor)color1, sampledValue);
        }



        //this is just a helper method to move the camera around in the FPS fashion
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
