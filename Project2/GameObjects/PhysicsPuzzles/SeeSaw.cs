using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2.GameObjects.Abstract;
using SharpDX;
using SharpDX.Toolkit;
using Project2.GameObjects.LevelPieces;



namespace Project2.GameObjects.PhysicsPuzzles
{
    class SeeSaw : Project2.GameObjects.Abstract.PhysicsPuzzle
    {
        public SeeSaw(Project2Game game, LevelPiece levelPiece, Vector3 offset) : base(game, levelPiece, offset) {
            
            var baseHeight = 2;

            Box seeSawbase = new Box(
                    game,
                    game.models["box"],
                    this.originPosition + new Vector3(0f, 0.5f * baseHeight, 0f),
                    new Vector3(30f, 3f, 1.5f),
                    true
                );

            Box seeSawPlatform= new Box(
                    game,
                    game.models["box"],
                    this.originPosition + new Vector3(0f, 2f * baseHeight, 0f),
                    new Vector3(50f, 1f, 40f),
                    false
                );
            seeSawPlatform.PhysicsDescription.Mass = 50f;
            //var constraint = new Jitter.Dynamics.Joints.HingeJoint(game.physics.World, seeSawbase.physicsDescription.RigidBody, seeSawPlatform.physicsDescription.RigidBody, PhysicsSystem.toJVector(this.originPosition + new Vector3(0f, baseHeight, 0f)), Jitter.LinearMath.JVector.Right);
            //constraint.PointOnPointConstraint1.Softness = 0.001f;
            var constraint = new Jitter.Dynamics.Constraints.PointOnPoint(seeSawbase.PhysicsDescription, seeSawPlatform.PhysicsDescription, PhysicsSystem.toJVector(originPosition + new Vector3(0, 0.5f * baseHeight, 0)));
            constraint.BiasFactor = 100f;
            //constraint.Softness = 0.001f;
            //game.physics.World.AddConstraint(constraint);
            this.AddChild(seeSawPlatform);
            this.AddChild(seeSawbase);

        }

    }
}
