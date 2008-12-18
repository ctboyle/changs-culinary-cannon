using System;
using System.Collections.Generic;
using System.Text;
using JigLibX.Vehicles;
using Microsoft.Xna.Framework;
using RC.Engine.GraphicsManagement;

namespace RC.Physics
{
    public class JigLibXWheel : JibLibXObject
    {
        private Car _vehicle = null;
        private Wheel _wheel = null;
        private Matrix _rotMat = Matrix.Identity;

        public JigLibXWheel(RCSpatial spatial)
            : base(spatial)
        {
        }

        public void SetVehicle(Car vehicle, Wheel wheel, Matrix rotMat)
        {
            _vehicle = vehicle;
            _wheel = wheel;
            _rotMat = rotMat;
        }

        protected override void UpdateWorldData(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_wheel == null || _vehicle == null) return;

            _worldTrans =
                Matrix.CreateRotationZ(MathHelper.ToRadians(-_wheel.AxisAngle)) * _rotMat *// rotate the wheels
                Matrix.CreateRotationY(MathHelper.ToRadians(_wheel.SteerAngle)) *
                Matrix.CreateTranslation(_wheel.Pos + _wheel.Displacement * _wheel.LocalAxisUp) * _vehicle.Chassis.Body.Orientation * // oritentation of wheels
                Matrix.CreateTranslation(_vehicle.Chassis.Body.Position); // translation

            if (_childNode != null)
            {
                _childNode.UpdateGS(gameTime, true);
            }
        }
    }
}