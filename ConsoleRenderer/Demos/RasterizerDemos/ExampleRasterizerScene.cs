using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
using NostalgiaEngine.RasterizerPipeline;

namespace NostalgiaEngine.Demos
{
    class ExampleRasterizerScene: Scene3D
    {
        public override bool OnLoad()
        {

            SceneSkybox = new Skybox("RasterizerDemoResources/skybox1");
            Mesh cubeMesh = GeometryGenerator.GenerateCube2(1.0f, 1.0f, 1.0f, NEVector4.Zero, 7);
            Mesh floorMesh = GeometryGenerator.CreateHorizontalQuad(10.0f, 10.0f, new NEVector4(0.0f, -1.3f, 0.0f));
            Mesh teapotMesh = NEObjLoader.LoadObj("RasterizerDemoResources/teapot.obj");
            Mesh bunnyMesh = NEObjLoader.LoadObj("RasterizerDemoResources/bunny.obj",3);
            var luma = ResourceManager.Instance.GetLumaTexture("RasterizerDemoResources/uv_test_tex/luma.buf");


            Model cubeModel = new Model(cubeMesh, CullMode.Back, luma);
            cubeModel.Transform.LocalPosition = new NEVector4(1.9f, 2.0f, 0.0f);

            Model floorModel = new Model(floorMesh, luma);

            Model teapotModel = new Model(teapotMesh, CullMode.None);
            teapotModel.Transform.ScaleX = 0.5f;
            teapotModel.Transform.ScaleY = 0.5f;
            teapotModel.Transform.ScaleZ = 0.5f;
            teapotModel.Transform.LocalPosition = new NEVector4(-1.5f, 0.05f, -1.0f, 1.0f);

            Model bunnyModel = new Model(bunnyMesh, CullMode.None);
            bunnyModel.Transform.ScaleX = 10.5f;
            bunnyModel.Transform.ScaleY = 10.5f;
            bunnyModel.Transform.ScaleZ = 10.5f;
            bunnyModel.Transform.LocalPosition = new NEVector4(3.0f, 0.5f, -1.0f, 1.0f);

            Models.Add(cubeModel);
            Models.Add(teapotModel);
            Models.Add(floorModel);
           // Models.Add(bunnyModel);

            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 1.0f, -5.0f);

            TogglePalette();

            return base.OnLoad();
        }


        public override void OnUpdate(float deltaTime)
        {
            Movement(deltaTime);
            if(NEInput.CheckKeyPress(ConsoleKey.P))
            {
                TogglePalette();
            }

            if(NEInput.CheckKeyPress(ConsoleKey.C))
            {
                ToggleShowClipping();
            }

            Models[1].Transform.RotateY(deltaTime * 0.5f);
            Models[1].Transform.PositionY = 0.1f + (float)(Math.Sin(Engine.Instance.TotalTime) * 0.3);
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
        }
    }
}
