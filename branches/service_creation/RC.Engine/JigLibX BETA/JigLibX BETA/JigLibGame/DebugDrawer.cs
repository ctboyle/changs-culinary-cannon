using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JiggleGame
{
    public class DebugDrawer : DrawableGameComponent
    {
        BasicEffect basicEffect;
        List<VertexPositionColor> vertexData;

        public DebugDrawer(Game game)
            : base(game)
        {
            this.vertexData = new List<VertexPositionColor>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

            basicEffect = new BasicEffect(this.GraphicsDevice, null);
            GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
        }

        public override void Draw(GameTime gameTime)
        {
            if (vertexData.Count == 0 || !Enabled) return;

            JiggleGame playGround = this.Game as JiggleGame;

            this.basicEffect.AmbientLightColor = Vector3.One;
            this.basicEffect.View = playGround.Camera.View;
            this.basicEffect.Projection = playGround.Camera.Projection;

            this.basicEffect.Begin();
            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                    vertexData.ToArray(), 0, vertexData.Count / 2);

                pass.End();
            }
            this.basicEffect.End();

            base.Draw(gameTime);
        }

        public void ClearVertexData()
        {
            vertexData.Clear();
        }

        public void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            vertexData.Add(new VertexPositionColor(from,color));
            vertexData.Add(new VertexPositionColor(to,color));
        }

        public void DrawAabb(Vector3 from, Vector3 to, Color color)
        {
            Vector3 halfExtents = (to - from) * 0.5f;
            Vector3 center = (to + from) * 0.5f;

            Vector3 edgecoord = Vector3.One, pa, pb;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    pa = new Vector3(edgecoord.X * halfExtents.X, edgecoord.Y * halfExtents.Y,
                        edgecoord.Z * halfExtents.Z);
                    pa += center;

                    int othercoord = j % 3;
                    SetElement(ref edgecoord, othercoord, GetElement(edgecoord, othercoord) * -1f);
                    pb = new Vector3(edgecoord.X * halfExtents.X, edgecoord.Y * halfExtents.Y,
                        edgecoord.Z * halfExtents.Z);
                    pb += center;

                    vertexData.Add(new VertexPositionColor(pa, color));
                    vertexData.Add(new VertexPositionColor(pb, color));
                }
                edgecoord = new Vector3(-1f, -1f, -1f);
                if (i < 3) SetElement(ref edgecoord, i, GetElement(edgecoord, i) * -1f);
            }
        }

        internal static float GetElement(Vector3 v, int index)
        {
            if (index == 0)
                return v.X;
            if (index == 1)
                return v.Y;
            if (index == 2)
                return v.Z;

            throw new ArgumentOutOfRangeException("index");
        }

        internal static void SetElement(ref Vector3 v, int index, float value)
        {
            if (index == 0)
                v.X = value;
            else if (index == 1)
                v.Y = value;
            else if (index == 2)
                v.Z = value;
            else
                throw new ArgumentOutOfRangeException("index");
        }

    }
}
