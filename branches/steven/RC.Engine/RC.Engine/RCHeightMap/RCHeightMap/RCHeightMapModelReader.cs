//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

//namespace RC
//{
//    public class RCHeightMapModelReader : ContentTypeReader<RCHeightMapModel>
//    {

//        protected override RCHeightMapModel Read(ContentReader input, RCHeightMapModel existingInstance)
//        {
//            int xSize = 0;
//            int ySize = 0;

//            //read the mapping's dimensions here
//            xSize = input.ReadInt32();
//            ySize = input.ReadInt32();

//            float[,] mapping = new float[xSize, ySize];

//            for (int xCoordinate = 0; xCoordinate < xSize; xCoordinate++)
//            {
//                for (int yCoordinate = 0; yCoordinate < ySize; yCoordinate++)
//                {
//                    mapping[xCoordinate, yCoordinate] = input.ReadSingle();
//                }
//            }

//            //read the mapping here

//            Texture2D textureMap;
//            //textureMap = input.ReadObject<Texture2D>();
//            textureMap = input.ReadExternalReference<Texture2D>();
//            //read the model here

            
//            return new RCHeightMapModel(textureMap, mapping, new JiggleGame.HeightMapInfo(new float[0,0],0f));
//        }
//    }
//}
