﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NostalgiaEngine.Core;
using NostalgiaEngine.RasterizerPipeline;

namespace NostalgiaEngine.Demos.RasterizerDemos
{
    class ExampleRasterizerScene: Scene3D
    {
        public override bool OnLoad()
        {

            SceneSkybox = new Skybox("c:/test/skybox3");
            Mesh cubeMesh = GeometryGenerator.GenerateCube2(1.0f, 1.0f, 1.0f, NEVector4.Zero, 7);
            Mesh floorMesh = GeometryGenerator.CreateHorizontalQuad(10.0f, 10.0f, new NEVector4(0.0f, -1.3f, 0.0f));
            Mesh teapotMesh = NEObjLoader.LoadObj("C:/Users/Kuba/Desktop/tst/teapot.obj");
            var luma = ResourceManager.Instance.GetLumaTexture("C:/test/ruler/luma.buf");
            Model cubeModel = new Model(cubeMesh, CullMode.Back, luma);
            cubeModel.Transform.LocalPosition = new NEVector4(0.9f, 2.0f, 1.0f);


            Model floorModel = new Model(floorMesh, luma);

            Model teapotModel = new Model(teapotMesh, CullMode.None);
            teapotModel.Transform.ScaleX = 0.5f;
            teapotModel.Transform.ScaleY = 0.5f;
            teapotModel.Transform.ScaleZ = 0.5f;
            teapotModel.Transform.LocalPosition = new NEVector4(-2.0f, 0.05f, 1.0f, 1.0f);

            Models.Add(cubeModel);
            Models.Add(teapotModel);
            Models.Add(floorModel);

            MainCamera = new Camera(ScreenWidth, ScreenHeight, 1.05f, 0.1f, 100.0f);
            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 1.0f, -5.0f);



            return base.OnLoad();
        }


        public override void OnUpdate(float deltaTime)
        {
           Movement(deltaTime);
            Models[1].Transform.RotateY(deltaTime * 0.5f);
            Models[1].Transform.PositionY = 0.1f + (float)(Math.Sin(Engine.Instance.TotalTime) * 0.3);
            NEScreenBuffer.ClearColor(2);
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

                // m_Camera.Transform.RotateX(dt);
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
