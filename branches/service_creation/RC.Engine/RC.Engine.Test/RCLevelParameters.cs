using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.ContentManagement;
using RC.Engine.GraphicsManagement;

namespace RC.Engine.Test
{
    class RCLevelParameters
    {
        private IRCContentRequester requester;
        private string heightMapName;
        private string bottomTextureName;
        private string centerTextureName;
        private string topTextureName;
        private float heightMapXYZScaling;
        private float heightMapYScaling;
        private float percentBottomOfCenterTexture;
        private float percentTopOfBottomTexture;
        private float percentBottomOfTopTexture;
        private float percentTopOfMiddleTexture;
        private RCSceneNode parentNode;
        private List<RCLevelSpawnPoints> spawnPoints;

        internal List<RCLevelSpawnPoints> SpawnPoints
        {
            get
            {
                return spawnPoints;
            }
            set { spawnPoints = value; }
        }

        public string HeightMapName
        {
            get { return heightMapName; }
            set { heightMapName = value; }
        }
        public string BottomTextureName
        {

            get { return bottomTextureName; }
            set { bottomTextureName = value; }
        }
        public string CenterTextureName
        {
            get { return centerTextureName; }
            set { centerTextureName = value; }
        }
        public string TopTextureName
        {
            get { return topTextureName; }
            set { topTextureName = value; }
        }
        public float HeightMapXYZScaling
        {
            get { return heightMapXYZScaling; }
            set { heightMapXYZScaling = value; }
        }
        public float HeightMapYScaling
        {
            get { return heightMapYScaling; }
            set { heightMapYScaling = value; }
        }
        public float PercentBottomOfCenterTexture
        {
            get { return percentBottomOfCenterTexture; }
            set { percentBottomOfCenterTexture = value; }
        }
        public float PercentTopOfBottomTexture
        {
            get { return percentTopOfBottomTexture; }
            set { percentTopOfBottomTexture = value; }
        }
        public float PercentBottomOfTopTexture
        {
            get { return percentBottomOfTopTexture; }
            set { percentBottomOfTopTexture = value; }
        }
        public float PercentTopOfMiddleTexture
        {
            get { return percentTopOfMiddleTexture; }
            set { percentTopOfMiddleTexture = value; }
        }
        public RCSceneNode ParentNode
        {
            get { return parentNode; }
            set { parentNode = value; }
        }
        public IRCContentRequester Requester
        {
            get { return requester; }
            set { requester = value; }
        }

        public RCLevelParameters(IRCContentRequester requester, string heightMapName,
            string bottomTextureName, string centerTextureName, string topTextureName,
            float heightMapXYZScaling, float heightMapYScaling,
            float percentBottomOfCenterTexture, float percentTopOfBottomTexture,
            float percentBottomOfTopTexture, float percentTopOfMiddleTexture,
            RCSceneNode parentNode, List<RCLevelSpawnPoints> spawnPoints)
        {
            this.requester = requester;
            this.heightMapName = heightMapName;
            this.bottomTextureName = bottomTextureName;
            this.centerTextureName = centerTextureName;
            this.topTextureName = topTextureName;
            this.heightMapXYZScaling = heightMapXYZScaling;
            this.heightMapYScaling = heightMapYScaling;
            this.percentBottomOfCenterTexture = percentBottomOfCenterTexture;
            this.percentBottomOfTopTexture = percentBottomOfTopTexture;
            this.percentTopOfBottomTexture = percentTopOfBottomTexture;
            this.percentTopOfMiddleTexture = percentTopOfMiddleTexture;
            this.parentNode = parentNode;
            this.spawnPoints = spawnPoints;

        }

    }
}
