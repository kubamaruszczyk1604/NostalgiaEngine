﻿using NostalgiaEngine.Core;
using NostalgiaEngine.RasterizerPipeline;
using System;

namespace NostalgiaEngine.Demos
{
    class NightGardenScene3D: Scene3D
    {
        public override bool OnLoad()
        {
            ScreenWidth = 280;
            ScreenHeight = 170;
            PixelWidth = 4;
            PixelHeight = 4;

            SceneSkybox = new Skybox("RasterizerDemoResources/skybox1");
            Mesh floorMesh = GeometryGenerator.CreateHorizontalQuad(15.0f, 15.0f, new NEVector4(0.0f, 0.0f, 0.0f), 7);
            Mesh cubeMesh = GeometryGenerator.GenerateCube2(1.0f, 1.0f, 1.0f, NEVector4.Zero, 4);
            Mesh teapotMesh = NEObjLoader.LoadObj("RasterizerDemoResources/teapot.obj",14);
            Mesh bunnyMesh = NEObjLoader.LoadObj("RasterizerDemoResources/bunny.obj",8);
            var luma = ResourceManager.Instance.GetLumaTexture("RasterizerDemoResources/uv_test_tex/luma.buf");

            var wallTexture = ResourceManager.Instance.GetLumaTexture("RasterizerDemoResources/textures/wall.buf");
            var treeTexture = ResourceManager.Instance.GetLumaTexture("RasterizerDemoResources/textures/tree_crown.buf");
            var groundTexture = ResourceManager.Instance.GetLumaTexture("RasterizerDemoResources/textures/grass.buf");


            Model floorModel = new Model(floorMesh, groundTexture);
            floorModel.UnlitColor = 3;

            Model bunnyModel = new Model(bunnyMesh, CullMode.None);
            bunnyModel.Transform.ScaleX = 10.5f;
            bunnyModel.Transform.ScaleY = 10.5f;
            bunnyModel.Transform.ScaleZ = 10.5f;
            bunnyModel.Transform.LocalPosition = new NEVector4(5.0f, -0.4f, -1.0f, 1.0f);
            
            Models.Add(floorModel);
            Models.Add(bunnyModel);
            


            MakeTree(new NEVector4(-3.2f, 0.0f, 1.0f), cubeMesh, treeTexture);
            MakeTree(new NEVector4(3.5f, 0.0f, 5.0f), cubeMesh, treeTexture);
            MakeTree2(new NEVector4(-12.0f, 0.0f, 10.0f), cubeMesh, treeTexture);
            MakeTree2(new NEVector4(8.4f, 0.0f, -5.3f), cubeMesh, treeTexture);

            MakeWall(new NEVector4(-10.2f, 0.0f, -7.4f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(-10.2f, 0.0f, -4.2f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(-10.2f, 0.0f, -1.0f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(-9.3f, 0.0f, 4.0f, 1.0f), cubeMesh, wallTexture, 1.0f);
            MakeWall(new NEVector4(-6.4f, 0.0f, 5.0f, 1.0f), cubeMesh, wallTexture, 1.0f);
            MakeWall(new NEVector4(-3.2f, 0.0f, 5.0f, 1.0f), cubeMesh, wallTexture, 1.0f);
            MakeWall(new NEVector4(-1.0f, 0.0f, 7.0f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(-1.0f, 0.0f, 10.2f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(2.9f, 0.0f, 7.0f, 1.0f), cubeMesh, wallTexture, 1.0f);
            MakeWall(new NEVector4(6.1f, 0.0f, 7.0f, 1.0f), cubeMesh, wallTexture, 1.0f);
            MakeWall(new NEVector4(9.3f, 0.0f, 7.0f, 1.0f), cubeMesh, wallTexture, 1.0f);
            MakeWall(new NEVector4(9.3f, 0.0f, 4.8f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(9.3f, 0.0f, 1.6f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(10.3f, 0.0f, -1.6f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);
            MakeWall(new NEVector4(10.3f, 0.0f, -4.8f, 1.0f), cubeMesh, wallTexture, 1.0f, 3.14f * 0.5f);


            MakePlatform(new NEVector4(0.0f,2.0f,-16.0f), cubeMesh, wallTexture,treeTexture);
            MakePlatform(new NEVector4(2.0f, 2.0f, -16.0f), cubeMesh, wallTexture, treeTexture);

            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 1.9f, -5.3f);


            NEColorPalette pal = NEColorPalette.FromFile("RasterizerDemoResources/palette.txt");
            pal.MultiplyBy(2.8f);
            NEColorManagement.SetPalette(pal);

            AddLight(new DirectionalLight(new NEVector4(-1.0f, 1.0f, 1.0f)));

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

            if (NEInput.CheckKeyPress(ConsoleKey.H))
            {
                ToggleHeadlamp();
            }

            if (NEInput.CheckKeyPress(ConsoleKey.T))
            {
                ToggleTexturing();
            }

            if (NEInput.CheckKeyPress(ConsoleKey.Escape))
            {
                Exit();
            }
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
        }

        Random rnd = new Random();
        private void MakeTree(NEVector4 pos, Mesh cubeMesh, NEFloatBuffer crownTexture)
        {
           
            Model trunk = new Model(cubeMesh, CullMode.Back);
            trunk.Transform.LocalPosition =  pos +  new NEVector4(0.0f, 1.0f, 0.0f);
            trunk.Transform.ScaleX = 0.4f;
            trunk.Transform.ScaleZ = 0.4f;
            //trunk.Transform.RotateY(0.4f);
            trunk.Color = 5;
            Models.Add(trunk);

            int numRings = rnd.Next(5, 10);
            
            for(int i = 0; i < numRings; ++i )
            {

                float t = ((float)i) / ((float)numRings);
                float a = NEMathHelper.Sin(t * 3.14f);
                Model crown = new Model(cubeMesh, CullMode.Back, crownTexture);
                crown.Transform.LocalPosition = pos + new NEVector4(0.0f, 2.15f+i*0.6f, 0.0f);
                crown.Transform.ScaleY = 0.3f;
                crown.Transform.ScaleX = 0.5f + a;
                crown.Transform.ScaleZ = 0.5f + a;
                crown.Transform.RotateY(t);
                crown.UnlitColor = 3;
                Models.Add(crown);
            }

        }

        private void MakeTree2(NEVector4 pos, Mesh cubeMesh, NEFloatBuffer crownTexture)
        {

            Model trunk = new Model(cubeMesh, CullMode.Back);
            trunk.Transform.LocalPosition = pos + new NEVector4(0.0f, 1.0f, 0.0f);
            trunk.Transform.ScaleX = 0.4f;
            trunk.Transform.ScaleZ = 0.4f;
            //trunk.Transform.RotateY(0.4f);
            trunk.Color = 5;
            Models.Add(trunk);

            int numRings = 10;

            for (int arm = 0; arm < 7; ++arm)
            {
                float s = arm / 7.0f;
                float xP = NEMathHelper.Cos(s*6.28f);
                float zP = NEMathHelper.Sin(s*6.28f);
                for (int i = 0; i < numRings; ++i)
                {

                    float t = ((float)i) / ((float)numRings);
                    float a = NEMathHelper.Sin(t * 3.14f * 1.2f);
                    Model crown = new Model(cubeMesh, CullMode.Back, crownTexture);
                    crown.Transform.LocalPosition = pos + new NEVector4( xP*i * 0.3f, 2.15f + a * 1.5f, zP * i * 0.3f);
                    crown.Transform.ScaleY = 0.3f;
                    crown.Transform.ScaleX = 0.4f;
                    crown.Transform.ScaleZ = 0.4f;
                    //crown.Transform.RotateY(t);
                    crown.UnlitColor = 3;
                    Models.Add(crown);
                }
            }

        }

        private void MakeWall(NEVector4 pos, Mesh cubeMesh, NEFloatBuffer texture, float scale = 1.0f, float rotationYRad = 0 )
        {
            Model wallModel = new Model(cubeMesh, CullMode.Back, texture);
            wallModel.Transform.LocalPosition = new NEVector4(0.0f,scale*0.6f,0.0f) + pos;
            wallModel.Transform.ScaleY = 0.6f * scale;
            wallModel.Transform.ScaleX = 1.6f * scale;
            wallModel.Color = 11;
            wallModel.UnlitColor = 10;
            //wallModel.AmbientIntensity = 0.4f;
            wallModel.Transform.RotateY(rotationYRad);
            Models.Add(wallModel);
        }

        private void MakePlatform(NEVector4 pos, Mesh cubeMesh, NEFloatBuffer groundTex, NEFloatBuffer topTex)
        {
            Model groundModel = new Model(cubeMesh, CullMode.Back, groundTex);
            groundModel.Transform.LocalPosition = new NEVector4(0.0f, 0.6f, 0.0f) + pos;
            groundModel.Transform.ScaleY = 0.6f ;
            groundModel.Transform.ScaleX = 1;
            groundModel.Color = 11;
            groundModel.UnlitColor = 10;
            Models.Add(groundModel);

            Model topdModel = new Model(cubeMesh, CullMode.Back,topTex);
            topdModel.Transform.LocalPosition = new NEVector4(0.0f,0.8f, 0.0f) + groundModel.Transform.LocalPosition;
            topdModel.Transform.ScaleY = 0.2f ;
            topdModel.Color = 7;
            topdModel.UnlitColor = 10;
            Models.Add(topdModel);


        }
    }
}
