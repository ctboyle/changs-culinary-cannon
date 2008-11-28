using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Physics;

namespace JiggleGame
{
    public class FrameRateCounter : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        PhysicsSystem physics;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game,PhysicsSystem physics)
            : base(game)
        {
            content = new ContentManager(game.Services);
            this.physics = physics;
        }

        protected override void LoadContent()
        {
                spriteBatch = new SpriteBatch(GraphicsDevice);
                spriteFont = content.Load<SpriteFont>("Content/Font");
        }

        protected override void UnloadContent()
        {
                content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = frameRate.ToString();

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, fps, new Vector2(11, 6), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(12, 7), Color.Yellow);

            spriteBatch.End();

            ((JiggleGame)this.Game).RestoreRenderState();
        }
    }
}
