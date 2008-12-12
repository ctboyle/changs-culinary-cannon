using System;
using System.Collections.Generic;
using System.Text;
using RC;
using RC.Content;
using RC.Engine.ContentManagement;
using RC.Content.Heightmap;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.SceneEffects;
using RC.Physics;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework;

namespace RC.Engine.Test
{
    class RCLevel //: IDisposable
    {
        private RCContent<RCHeightMap> heightMap;
        private RCContent<Texture2D> bottomTexture;
        private RCContent<Texture2D> centerTexture;
        private RCContent<Texture2D> topTexture;
        private JibLibXObject physicsMap;
        private RCLevelParameters loadingParameters;
        private bool isLoaded = false;
        static Random spawnIndexRandomizer;


        internal List<RCLevelSpawnPoint> SpawnPoints
        {
            get 
            {
                List<RCLevelSpawnPoint> adjustedSpawnPoints = 
                    new List<RCLevelSpawnPoint>(loadingParameters.SpawnPoints);

                foreach (RCLevelSpawnPoint point in adjustedSpawnPoints)
                {
                    point.Position *= loadingParameters.HeightMapXYZScaling;
                    point.Poisition.Y *= loadingParameters.HeightMapYScaling;
                }
                return loadingParameters.SpawnPoints; 
            }
            set 
            {
                List<RCLevelSpawnPoint> adjustedSpawnPoints =
                    new List<RCLevelSpawnPoint>(value);

                foreach (RCLevelSpawnPoint point in adjustedSpawnPoints)
                {
                    point.Position *= loadingParameters.HeightMapXYZScaling;
                    point.Poisition.Y *= loadingParameters.HeightMapYScaling;
                }
                loadingParameters.SpawnPoints = value;
            }
        }

            internal RCLevelSpawnPoint RandomSpawnPoint
            {
                get
                {
                    RCLevelSpawnPoint randomizedSpawnPoint =
                        SpawnPoints[spawnIndexRandomizer.Next(loadingParameters.SpawnPoints.Count)];
                    randomizedSpawnPoint.Position *= loadingParameters.HeightMapXYZScaling;
                    randomizedSpawnPoint.Position.Y *= loadingParameters.HeightMapYScaling;
                }
            }

            public JibLibXObject PhysicsMap
            {
                get { return physicsMap; }
            }

            public RCLevel(RCLevelParameters levelParameters)
            {
                loadingParameters = levelParameters;
            }

            public void LoadLevel(IGraphicsDeviceService graphics)
            {
                heightMap = new RCDefaultContent<RCHeightMap>(loadingParameters.Requester, "Content\\Textures\\"
                    + loadingParameters.HeightMapName);
                heightMap.Content.Scaling = loadingParameters.HeightMapXYZScaling;
                heightMap.Content.HeightScaling = loadingParameters.HeightMapYScaling;

                bottomTexture = new RCDefaultContent<Texture2D>(loadingParameters.Requester, "Content\\Textures\\"
                    + loadingParameters.BottomTextureName);
                centerTexture = new RCDefaultContent<Texture2D>(loadingParameters.Requester, "Content\\Textures\\"
                    + loadingParameters.CenterTextureName);
                topTexture = new RCDefaultContent<Texture2D>(loadingParameters.Requester, "Content\\Textures\\"
                    + loadingParameters.TopTextureName);

                HeightMapEffect heightMapEffect = new HeightMapEffect(loadingParameters.Requester,
                    heightMap, bottomTexture, centerTexture, topTexture, loadingParameters.PercentBottomOfCenterTexture,
                    loadingParameters.PercentTopOfBottomTexture, loadingParameters.PercentBottomOfTopTexture, loadingParameters.PercentTopOfMiddleTexture);

                RCGeometry heightMapDrawable = heightMap.Content.CreateGeometry(graphics);

                heightMapDrawable.AddEffect(heightMapEffect);

                physicsMap = JibLibXPhysicsHelper.CreateHeightmap(heightMap, heightMapDrawable);

                loadingParameters.ParentNode.AddChild(physicsMap);
                //loadingParameters.ParentNode.AddChild(

                isLoaded = true;
            }

            public void UnloadLevel()
            {
                //Dispose();
            }

            ~RCLevel()
            {
                UnloadLevel();
            }



            #region IDisposable Members

            //public void Dispose()
            //{
            //    if (isLoaded)
            //    {
            //        bottomTexture.Dispose();
            //        centerTexture.Dispose();
            //        topTexture.Dispose();
            //        heightMap.Dispose();
            //    }
            //}

            #endregion
        }

        class RCLevelCollection : Dictionary<string, RCLevel>
        { }

        struct RCLevelSpawnPoint
        {
            private Vector3 position;

            public Vector3 Position
            {
                get { return position; }
                set { position = value; }
            }
            private Vector3 heading;

            public Vector3 Heading
            {
                get { return heading; }
                set { heading = value; }
            }

            public RCLevelSpawnPoint(Vector3 position, Vector3 heading)
            {
                this.position = position;
                this.heading = heading;
            }

        }

    }

