using System;
using System.Collections.Generic;
using System.Text;
using JigLibX.Vehicles;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace RC.Physics
{
    public class JigLibXVehicle : JibLibXObject
    {
        public enum WheelId { One, Two, Three, Four };

        private JigLibXWheel[] _wheels = null;
        private Car _car = null;

        public JigLibXVehicle(
            Car vehicleModel,
            RCSpatial vehicleDrawable, 
            JigLibXWheel wheel1, 
            JigLibXWheel wheel2, 
            JigLibXWheel wheel3, 
            JigLibXWheel wheel4
            ) 
            : base()
        {
            Debug.Assert(vehicleModel != null);

            _wheels = new JigLibXWheel[Enum.GetValues(typeof(WheelId)).Length];
            _car = vehicleModel;
            SetBody(_car.Chassis.Body);

            SetWheel(wheel1, WheelId.One);
            SetWheel(wheel2, WheelId.Two);
            SetWheel(wheel3, WheelId.Three);
            SetWheel(wheel4, WheelId.Four);
            SetMass(100.0f);

            SetChildNode(vehicleDrawable);
        }

        public Car VehicleData
        {
            get { return _car; }
        }

        public override void SetChildNode(RCSpatial drawable)
        {
            if (_car == null)
            {
                return;
            }

            if (drawable == null)
            {
                _car.DisableCar();
                return;
            }

            _childNode = drawable;
            _childNode.LocalTrans = Matrix.Identity;
            _childNode.ParentNode = this;

            Vector3 scale, translation;
            Quaternion rotation;

            drawable.WorldTrans.Decompose(out scale, out rotation, out translation);

            _car.Chassis.Body.MoveTo(translation, Matrix.CreateFromQuaternion(rotation));
            _car.EnableCar();
        }

        public void SetWheel(JigLibXWheel wheel, WheelId id)
        {
            Debug.Assert(wheel != null);

            wheel.ParentNode = this;

            wheel.LocalTrans = Matrix.Identity;

            wheel.SetVehicle(
                _car, 
                _car.Wheels[(int)id], 
                (id== WheelId.One||id==WheelId.Two) ? 
                    Matrix.CreateRotationY(MathHelper.ToRadians(180.0f)) : Matrix.Identity
            );

            _wheels[(int)id] = wheel;
        }

        public override void SetMass(float mass)
        {
            Body.Mass = mass;

            Vector3 min, max;
            _car.Chassis.GetDims(out min, out max);

            Vector3 sides = max - min;

            float Ixx = (1.0f / 12.0f) * mass * (sides.Y * sides.Y + sides.Z * sides.Z);
            float Iyy = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Z * sides.Z);
            float Izz = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Y * sides.Y);

            Matrix inertia = Matrix.Identity;
            inertia.M11 = Ixx; inertia.M22 = Iyy; inertia.M33 = Izz;

            _car.Chassis.Body.BodyInertia = inertia;
            _car.SetupDefaultWheels();
        }

        protected override void UpdateWorldData(GameTime gameTime)
        {
            base.UpdateWorldData(gameTime);

            foreach (JigLibXWheel wheel in _wheels)
            {
                wheel.UpdateGS(gameTime, true);   
            }
        }

        public override void Draw(RC.Engine.Rendering.IRCRenderManager render, RC.Engine.ContentManagement.IRCContentRequester contentRqst)
        {
            base.Draw(render, contentRqst);

            foreach (JigLibXWheel wheel in _wheels)
            {
                wheel.Draw(render, contentRqst);
            }
        }

        protected override void UpdateState(RC.Engine.Rendering.RCRenderStateStack stateStack, Stack<RC.Engine.SceneEffects.RCLight> lightStack)
        {
            base.UpdateState(stateStack, lightStack);

            foreach (JigLibXWheel wheel in _wheels)
            {
                wheel.UpdateRS(stateStack, lightStack);
            }
        }
    }
}
