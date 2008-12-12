using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using JigLibX.Physics;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement.BoundingVolumes;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace RC.Physics
{
    public class JibLibXObject : RCSpatial
    {
        private class MyBody : Body
        {
            public override void AddExternalForces(float dt)
            {
                AddGravityToExternalForce();
            }
        }

        public const RCSpatial DefaultChildNode = null;

        protected RCSpatial _childNode = null;
        private Body _body = null;
        private Vector3 _centerOfMass = Vector3.Zero;

        public JibLibXObject()
            : this(DefaultChildNode)
        {
        }

        public JibLibXObject(RCSpatial drawable)
        {
            SetBody(new MyBody());
            SetChildNode(drawable);
        }

        public virtual void SetChildNode(RCSpatial drawable)
        {
            if (drawable == null)
            {
                _body.DisableBody();
                return;
            }

            _childNode = drawable;
            _childNode.LocalTrans = Matrix.Identity;
            _childNode.ParentNode = this;

            Vector3 scale, translation;
            Quaternion rotation;

            drawable.WorldTrans.Decompose(out scale, out rotation, out translation);

            SetOrientation(Matrix.CreateFromQuaternion(rotation));
            SetPosition(translation);

            _body.EnableBody();
        }

        public Vector3 CenterOfMass
        {
            get { return _centerOfMass; }
        }

        public Body Body
        {
            get { return _body; }
        }

        public RCSpatial ChildNode
        {
            get { return _childNode; }
        }

        public void AddCollisionSkin()
        {
            _body.CollisionSkin = new JigLibX.Collision.CollisionSkin(_body);
            _body.CollisionSkin.callbackFn += new CollisionCallbackFn(OnCollision);
        }

        public virtual void SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid,
                PrimitiveProperties.MassTypeEnum.Mass, mass);

            float junk;
            Vector3 com;
            Matrix it;
            Matrix itCoM;

            _body.CollisionSkin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);

            _body.BodyInertia = itCoM;
            _body.Mass = junk;

            if (_body.CollisionSkin != null)
            {
                _body.CollisionSkin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            }

            _centerOfMass = com;
        }

        public override void Draw(RC.Engine.Rendering.IRCRenderManager render, RC.Engine.ContentManagement.IRCContentRequester contentRqst)
        {
            if (_childNode != null)
            {
                _childNode.Draw(render, contentRqst);
            }
        }

        protected virtual bool OnCollision(CollisionSkin skin0, CollisionSkin skin1)
        {
            return true;
        }

        protected void SetBody(Body body)
        {
            Debug.Assert(body != null);

            if (_body != null)
            {
                _body.DisableBody();
            }

            _body = body;
            _body.ExternalData = this;
        }

        protected void SetOrientation(Matrix orientation)
        {
            _body.SetOrientation(orientation);
        }

        protected void SetPosition(Vector3 position)
        {
            _body.MoveTo(position, Matrix.Identity);

            if (_body.CollisionSkin != null)
            {
                _body.CollisionSkin.ApplyLocalTransform(new Transform(-CenterOfMass, Matrix.Identity));
            }
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

            if (_childNode != null)
            {
                _childNode.UpdateGS(gameTime, true);
            }
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