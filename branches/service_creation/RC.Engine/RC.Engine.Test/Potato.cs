using System;
using System.Collections.Generic;
using System.Text;
using RC.Physics;
using RC.Engine.ContentManagement.ContentTypes;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using RC.Engine.Rendering;
using RC.Engine.ContentManagement;
using JigLibX.Collision;

namespace RC.Engine.Test
{
    public class Potato : JibLibXObject
    {
        public const double LifeDuration = 15.0;
        private bool _fired = false;
        private double _curUsedTime = 0.0f;
        PotatoPool _pool;
        RCModelContent _potatoModel;



        protected override bool OnCollision(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin0.Owner.ExternalData is Potato)
            {
            }

            return base.OnCollision(skin0, skin1);
        }

        public Potato(PotatoPool pool, IRCContentRequester content)
        {
            _pool = pool;

            _potatoModel = new RCModelContent("Content\\Models\\potato");
            SetChildNode(_potatoModel);

            this.AddCollisionSkin();

            this.Body.CollisionSkin.AddPrimitive( 
                new Capsule(Vector3.Zero, Matrix.CreateRotationY(MathHelper.PiOver2), 0.05f, 0.5f),
                (int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.2f, 0.8f, 0.7f));


            this.SetMass(500.0f);

        }

        public void Fire(Vector3 worldPos, Matrix worldOrientation, Vector3 velocity)
        {
            _curUsedTime = 0.0f;
            _fired = true;

            this.Body.Position = worldPos;
            this.Body.Orientation = worldOrientation;

            this.Body.Velocity = velocity;
        }

        protected override void UpdateWorldData(GameTime gameTime)
        {
            if (_fired)
            {
                _curUsedTime += gameTime.ElapsedGameTime.TotalSeconds;

                if (_curUsedTime >= LifeDuration)
                {
                    _fired = false;
                    _pool.UsedPotatoExpired(this);
                }
            }

            base.UpdateWorldData(gameTime);
        }
    }
}
