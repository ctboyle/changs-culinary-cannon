using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Vehicles;
using JigLibX.Collision;

namespace JiggleGame.PhysicObjects
{
    class CarObject : PhysicObject
    {

        private Car car;
        private Model wheel;

        public CarObject(Game game, Model model,Model wheels, bool FWDrive,
                       bool RWDrive,
                       float maxSteerAngle,
                       float steerRate,
                       float wheelSideFriction,
                       float wheelFwdFriction,
                       float wheelTravel,
                       float wheelRadius,
                       float wheelZOffset,
                       float wheelRestingFrac,
                       float wheelDampingFrac,
                       int wheelNumRays,
                       float driveTorque,
                       float gravity)
            : base(game, model)
        {
            car = new Car(FWDrive, RWDrive, maxSteerAngle, steerRate,
                wheelSideFriction, wheelFwdFriction, wheelTravel, wheelRadius,
                wheelZOffset, wheelRestingFrac, wheelDampingFrac,
                wheelNumRays, driveTorque, gravity);

            this.body = car.Chassis.Body;
            this.collision = car.Chassis.Skin;
            this.wheel = wheels;

            SetCarMass(100.0f);

          //  car.Chassis.Skin.SetMaterialProperties((int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.4f,0.8f,0.7f));
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


            // Draw wheels!
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Camera camera = ((JiggleGame)this.Game).Camera;
            Wheel wh;

            wh = car.Wheels[0];
            foreach (ModelMesh mesh in wheel.Meshes)
            {
                Matrix rotMat = Matrix.Identity;
                rotMat = Matrix.CreateRotationY(MathHelper.ToRadians(180.0f));

                foreach (BasicEffect effect in mesh.Effects)
                {
                    float steer = wh.SteerAngle;

                    effect.World = boneTransforms[mesh.ParentBone.Index] * rotMat * // create scale
                        Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation


                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }



            wh = car.Wheels[1];
            foreach (ModelMesh mesh in wheel.Meshes)
            {
                Matrix rotMat = Matrix.Identity;
                rotMat = Matrix.CreateRotationY(MathHelper.ToRadians(180.0f));

                foreach (BasicEffect effect in mesh.Effects)
                {
                    float steer = wh.SteerAngle;

                    effect.World = boneTransforms[mesh.ParentBone.Index] * rotMat * // create scale
                        Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation


                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }



            wh = car.Wheels[2];
            foreach (ModelMesh mesh in wheel.Meshes)
            {
                Matrix rotMat = Matrix.Identity;

                foreach (BasicEffect effect in mesh.Effects)
                {
                    float steer = wh.SteerAngle;

                    effect.World = boneTransforms[mesh.ParentBone.Index] * rotMat * // create scale
                        Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation


                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }



            wh = car.Wheels[3];
            foreach (ModelMesh mesh in wheel.Meshes)
            {
                Matrix rotMat = Matrix.Identity;

                foreach (BasicEffect effect in mesh.Effects)
                {
                    float steer = wh.SteerAngle;

                    effect.World = boneTransforms[mesh.ParentBone.Index] * rotMat * // create scale
                        Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation

                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }

        public Car Car
        {
            get { return this.car; }
        }


        private void SetCarMass(float mass)
        {
            body.Mass = mass;
            Vector3 min, max;
            car.Chassis.GetDims(out min, out max);
            Vector3 sides = max - min;

            float Ixx = (1.0f / 12.0f) * mass * (sides.Y * sides.Y + sides.Z * sides.Z);
            float Iyy = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Z * sides.Z);
            float Izz = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Y * sides.Y);

            Matrix inertia = Matrix.Identity;
            inertia.M11 = Ixx; inertia.M22 = Iyy; inertia.M33 = Izz;
            car.Chassis.Body.BodyInertia = inertia;
            car.SetupDefaultWheels();
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            //
        }
    }
}
