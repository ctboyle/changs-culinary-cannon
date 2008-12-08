using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using JigLibX.Physics;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement.BoundingVolumes;
using JigLibX.Geometry;

namespace RC.Physics
{
    public class RCPhysicsObject : RCSpatial
    {
        public const float DefaultDynamicRoughness = 0.6f;
        public const float DefaultElasticity = 0.5f;
        public const float DefaultStaticRoughness = 0.7f;
        public const RCAxisAlignedBoundingBox DefaultBoundingBox = null;
        public const RCSpatial DefaultChildNode = null;

        private RCSpatial _childNode = null;
        private float _dynamicRoughness = DefaultDynamicRoughness;
        private float _elasticity = DefaultElasticity;
        private float _staticRoughness = DefaultStaticRoughness;
        protected Body _body = new Body();

        public RCPhysicsObject(
            Vector3 position
        )
            : this(
            position,
            DefaultChildNode
            )
        {
        }

        public RCPhysicsObject(
            Vector3 position,
            RCSpatial drawable
        )
        {
            _childNode = drawable;
            _body.Position = position;
            _body.EnableBody();
        }

        public RCSpatial ChildNode
        {
            get { return _childNode; }
            set { _childNode = value; }
        }

        public RCAxisAlignedBoundingBox PhysicsBoundingBox
        {
            get
            {
                BoundingBox box = new BoundingBox();

                if (_body.CollisionSkin != null)
                {
                    box = _body.CollisionSkin.WorldBoundingBox;
                }

                return new RCAxisAlignedBoundingBox(box);
            }
        }

        public override void Draw(RC.Engine.Rendering.IRCRenderManager render, RC.Engine.ContentManagement.IRCContentRequester contentRqst)
        {
            if (_childNode != null)
            {
                _childNode.Draw(render, contentRqst);
            }
        }

        public override void UpdateGS(Microsoft.Xna.Framework.GameTime gameTime, bool fInitiator)
        {
            if (_childNode != null)
            {
                _childNode.UpdateGS(gameTime, fInitiator);
            }
            base.UpdateGS(gameTime, fInitiator);
        }

        public void AddDefaultPhysicsBoundingBox()
        {
            UpdatePhysicsBoundingBox(_childNode.WorldBound.ToBoundingBox(), DefaultElasticity);
        }

        public void AddDefaultPhysicsBoundingBox(float elasticity)
        {
            AddDefaultPhysicsBoundingBox(elasticity, DefaultStaticRoughness);
        }

        public void AddDefaultPhysicsBoundingBox(
            float elasticity,
            float staticRoughness
        )
        {
            AddDefaultPhysicsBoundingBox(elasticity, DefaultStaticRoughness, DefaultDynamicRoughness);
        }

        public void AddDefaultPhysicsBoundingBox(
            float elasticity,
            float staticRoughness,
            float dynamicRoughness
        )
        {
            UpdatePhysicsBoundingBox(_childNode.WorldBound.ToBoundingBox(), elasticity, staticRoughness, dynamicRoughness);
        }

        public void UpdatePhysicsBoundingBox(RCAxisAlignedBoundingBox boundBox)
        {
            UpdatePhysicsBoundingBox(boundBox, DefaultElasticity);
        }

        public void UpdatePhysicsBoundingBox(
            RCAxisAlignedBoundingBox boundBox,
            float elasticity
        )
        {
            UpdatePhysicsBoundingBox(boundBox, elasticity, DefaultStaticRoughness);
        }

        public void UpdatePhysicsBoundingBox(
            RCAxisAlignedBoundingBox boundBox,
            float elasticity,
            float staticRoughness
        )
        {
            UpdatePhysicsBoundingBox(boundBox, elasticity, staticRoughness, DefaultDynamicRoughness);
        }

        public void UpdatePhysicsBoundingBox(
            RCAxisAlignedBoundingBox boundBox,
            float elasticity,
            float staticRoughness,
            float dynamicRoughness
        )
        {
            if (_body.CollisionSkin == null)
            {
                _body.CollisionSkin = new CollisionSkin(_body);
            }

            _body.CollisionSkin.AddPrimitive(
                new AABox(boundBox.Min, boundBox.Max),
                (int)MaterialTable.MaterialID.UserDefined,
                new MaterialProperties(elasticity, staticRoughness, dynamicRoughness)
            );
        }

        protected override void UpdateWorldData(GameTime gameTime)
        {
            if (_body.CollisionSkin != null)
            {
                _worldTrans =
                    _body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                    _body.Orientation *
                    Matrix.CreateTranslation(_body.Position);
            }
            else
            {
                _worldTrans =
                    _body.Orientation *
                    Matrix.CreateTranslation(_body.Position);
            }
            base.UpdateWorldData(gameTime);
        }

        protected override void UpdateWorldBound()
        {
            if (_childNode != null)
            {
                _worldBound = _childNode.WorldBound;
            }
        }

        protected override void UpdateState(RC.Engine.Rendering.RCRenderStateStack stateStack, Stack<RC.Engine.SceneEffects.RCLight> lightStack)
        {
            if (_childNode != null)
            {
                _childNode.UpdateRS(stateStack, lightStack);
            }
        }
    }
}