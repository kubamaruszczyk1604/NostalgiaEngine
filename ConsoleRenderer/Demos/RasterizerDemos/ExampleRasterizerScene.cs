using NostalgiaEngine.Core;
using NostalgiaEngine.RasterizerPipeline;
using System;

namespace NostalgiaEngine.Demos
{
    class ExampleRasterizerScene: Scene3D
    {
        public override bool OnLoad()
        {

            SceneSkybox = new Skybox("RasterizerDemoResources/skybox1");
            Mesh floorMesh = GeometryGenerator.CreateHorizontalQuad(15.0f, 15.0f, new NEVector4(0.0f, 0.0f, 0.0f), 4);
            Mesh cubeMesh = GeometryGenerator.GenerateCube2(1.0f, 1.0f, 1.0f, NEVector4.Zero, 4);
            Mesh teapotMesh = NEObjLoader.LoadObj("RasterizerDemoResources/teapot.obj",1);
            Mesh bunnyMesh = NEObjLoader.LoadObj("RasterizerDemoResources/bunny.obj",3);
            var luma = ResourceManager.Instance.GetLumaTexture("RasterizerDemoResources/uv_test_tex/luma.buf");




            Model floorModel = new Model(floorMesh, luma);

            Model teapotModel = new Model(teapotMesh, CullMode.None);
            teapotModel.Transform.ScaleX = 0.5f;
            teapotModel.Transform.ScaleY = 0.5f;
            teapotModel.Transform.ScaleZ = 0.5f;
            teapotModel.Transform.LocalPosition = new NEVector4(0.0f, 0.5f, -1.0f, 1.0f);


            Model bunnyModel = new Model(bunnyMesh, CullMode.None);
            bunnyModel.Transform.ScaleX = 10.5f;
            bunnyModel.Transform.ScaleY = 10.5f;
            bunnyModel.Transform.ScaleZ = 10.5f;
            bunnyModel.Transform.LocalPosition = new NEVector4(3.0f, 1.5f, -1.0f, 1.0f);

            
            Models.Add(floorModel);
            Models.Add(teapotModel);
            //  Models.Add(bunnyModel);

           

            //MakeTree(new NEVector4(-3.2f, 0.0f, 1.0f), cubeMesh, luma);
            //MakeTree(new NEVector4(3.5f, 0.0f, 5.0f), cubeMesh, luma);



            MainCamera.Transform.LocalPosition = new NEVector4(0.0f, 2.8f, -5.3f);

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

            //Models[1].Transform.RotateY(deltaTime * 0.5f);
            //Models[1].Transform.PositionY = 1.6f + (float)(Math.Sin(Engine.Instance.TotalTime) * 0.1);
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

        Random rnd = new Random();
        private void MakeTree(NEVector4 pos, Mesh cubeMesh, NEFloatBuffer texture)
        {
           
            Model trunk = new Model(cubeMesh, CullMode.Back);
            trunk.Transform.LocalPosition =  pos +  new NEVector4(0.0f, 1.0f, 0.0f);
            trunk.Transform.ScaleX = 0.4f;
            trunk.Transform.ScaleZ = 0.4f;
            trunk.Transform.RotateY(0.4f);
            trunk.Color = 5;
            Models.Add(trunk);

            int numRings = rnd.Next(10, 25);
            
            for(int i =0; i < numRings; ++i )
            {

                float t = ((float)i) / ((float)numRings);
                float a = NEMathHelper.Sin(t * 3.14f);
                Model crown = new Model(cubeMesh, CullMode.Back, texture);
                crown.Transform.LocalPosition = pos + new NEVector4(0.0f, 2.15f+i*0.3f, 0.0f);
                crown.Transform.ScaleY = 0.15f;

                crown.Transform.ScaleX = 0.5f + a;
                crown.Transform.ScaleZ = 0.5f + a;
                crown.Transform.RotateY(t);
                Models.Add(crown);
            }

            //Model crown = new Model(cubeMesh, CullMode.Back, texture);
            //crown.Transform.LocalPosition = pos + new NEVector4(0.0f, 2.2f, 0.0f);
            //crown.Transform.ScaleY = 0.3f;


            //Model crown2 = new Model(cubeMesh, CullMode.Back, texture);
            //crown2.Transform.LocalPosition = pos + new NEVector4(0.0f, 2.8f, 0.0f);
            //crown2.Transform.ScaleY = 0.3f;
            //crown2.Transform.ScaleX = 1.4f;
            //crown2.Transform.ScaleZ = 1.4f;
 

          
            //Models.Add(crown);
            //Models.Add(crown2);
        }
    }
}
