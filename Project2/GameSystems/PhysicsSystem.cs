using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;
using Project2.GameSystems;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using SingleBodyConstraints = Jitter.Dynamics.Constraints.SingleBody;
using SharpDX.Toolkit.Input;


namespace Project2
{
    /// <summary>
    /// This class encapsulates the physics system for a given world of physics bodies.
    /// A physics body is a (usually) non-drawn, relatively simple shape or mesh that contains properties (mass, inertia, etc) that
    /// the physics engine uses to resolve collisions and solve kinematics for.
    /// A physics body may be dynamic or static. A static body's position cannot be changed by the physics engine, but needs to be moved
    /// externally. A dynamic object's position will be affected by gravity and other forces and collisions, and does not get moved manually.
    /// Its position and orientation are queried externally and used to render representative objects/models on screen (usually more detailed than the physics mesh).
    /// 
    /// This class is implemented as a GameSystem, meaning that once it is registered, it is automagically updated at the correct frequency.
    /// </summary>
    public class PhysicsSystem : GameSystem, IUpdateable
    {
        protected Project2Game game;
        public JitterWorld World;

        private MouseState mouseState;

        public int accuracy { get; set; }
        // collision system used by world (or on its own)
        Jitter.Collision.CollisionSystem collisionSystem = new Jitter.Collision.CollisionSystemPersistentSAP(); // SAP = Scan and Prune (good for large scenes, bruteforce might be fine for small scenes too)

        public PhysicsSystem(Project2Game game)
            : base(game)
        {
            this.game = game;

            World = new JitterWorld(collisionSystem); // whole_new_world.wav
            // gravity defaults to -9.8 m.s^-2
            // World.Gravity = new JVector(0f, -20, 0);
            accuracy = 1;   // lower this for higher FPS (accuracy = 1 still seems to work okay, it's just not ideal)
        }

        #region update - global variables
        // Hold previous input states.
        MouseState mousePreviousState = new MouseState();

        // Store information for drag and drop
        JVector hitPoint, hitNormal;
        SingleBodyConstraints.PointOnPoint grabConstraint;
        RigidBody grabBody;
        float hitDistance = 100.0f;
        int scrollWheel = 0;
        #endregion

        /// <summary>
        /// This method is automagically called as part of GameSystem to update the physics simulation.
        /// Physics system interpolates current running time with at max 1 itteration of the system over
        /// 1/60 seconds (by default).
        /// </summary>
        /// <param name="time"></param>
        override public void Update(GameTime time)
        {
            HandlePicking();
            
            World.Step((float)time.TotalGameTime.TotalSeconds, false, (float)Game.TargetElapsedTime.TotalSeconds / accuracy, accuracy);
        }

        private bool RaycastCallback(RigidBody body, JVector normal, float fraction)
        {
            if (body.IsStatic) return false;
            else return true;
        }


        private void HandlePicking()
        {
            if (game.inputManager.pointerClick)
            {
                var ray = RayTo(game.inputManager.MousePosition()); // shoot a ray from point in camera's view into scene

                float fraction; // represents distance ray travelled before colliding

                bool result = World.CollisionSystem.Raycast(toJVector(ray.Position), toJVector(ray.Direction), RaycastCallback, out grabBody, out hitNormal, out fraction);
                if (result)
                {
                    hitPoint = toJVector(ray.Position + fraction * (ray.Direction));
                    //System.Diagnostics.Debug.WriteLine("Hitpoint: " + hitPoint);
                    if (!grabBody.IsStatic)
                    {
                        grabBody.ApplyImpulse(hitNormal * -10f, JVector.Zero); // invert the normal vector to create a repulsion
                    }
                    // leftover cruft from example code
                    /*if (grabConstraint != null) World.RemoveConstraint(grabConstraint);

                    JVector lanchor = hitPoint - grabBody.Position;
                    lanchor = JVector.Transform(lanchor, JMatrix.Transpose(grabBody.Orientation));

                    grabConstraint = new SingleBodyConstraints.PointOnPoint(grabBody, lanchor);
                    grabConstraint.Softness = 0.01f;
                    grabConstraint.BiasFactor = 0.1f;

                    World.AddConstraint(grabConstraint);
                    hitDistance = (toVector3(hitPoint) - game.camera.position).Length();
                    scrollWheel = mouseState.WheelDelta;
                    grabConstraint.Anchor = hitPoint;*/
                }
            }
        }

        /// <summary>
        /// Takes a screen space point and maps it to a unit length ray vector in world space.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Ray RayTo(Vector2 mousePos)
        {

            // need to convert normalised screen coordinates back into pixel units
            // mousepos = mousepos [0..1, 0..1] * screensize[0..1024, 0..768]
            mousePos *= new Vector2(game.GraphicsDevice.BackBuffer.Width, game.GraphicsDevice.BackBuffer.Height);

            // represents extruded screenspace
            Vector3 nearSource = new Vector3(mousePos.X, mousePos.Y, 0);
            Vector3 farSource = new Vector3(mousePos.X, mousePos.Y, 1);

            Matrix world = Matrix.Identity;
            
            // get the
            Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(nearSource, game.camera.projection, game.camera.view, world);
            Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(farSource, game.camera.projection, game.camera.view, world);

            Vector3 direction = farPoint - nearPoint;
            return new Ray(nearPoint, direction);
        }


        public void AddBody(RigidBody rigidBody)
        {
            this.World.AddBody(rigidBody);
        }

        public static ConvexHullShape BuildConvexHullShape(Model model) {
            var vertices = ExtractVertices(model);
            var hull = new ConvexHullShape(vertices);
            //hull.MakeHull(vertices, 1);
            return hull;
        }

        private static List<JVector> ExtractVertices(Model model) {
            List<JVector> vertices = new List<JVector>();
            Matrix[] bones = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones);

            foreach (ModelMesh modelMesh in model.Meshes)
            {
                JMatrix boneTransform = PhysicsSystem.toJMatrix(bones[modelMesh.ParentBone.Index]);
                foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
                {
                    int offset = vertices.Count;
                    var meshVertices = meshPart.VertexBuffer.Resource.Buffer.GetData<JVector>();
                    for (int i = 0; i < meshVertices.Length; ++i)
                    {
                        JVector.Transform(ref meshVertices[i], ref boneTransform, out meshVertices[i]);
                    }
                    vertices.AddRange(meshVertices); // append transformed vertices  
                }
            }
            return vertices;
        }

        public static TriangleMeshShape BuildTriangleMeshShape(Model model)
        {
            TriangleMeshShape result = new TriangleMeshShape(BuildOctree(model));
            return result;
        }

        public static Octree BuildOctree(Model model) {

            List<JVector> vertices = new List<JVector>();
            List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();           
            Matrix[] bones = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones);   
            
            foreach (ModelMesh modelMesh in model.Meshes)
            {               
                JMatrix boneTransform = PhysicsSystem.toJMatrix(bones[modelMesh.ParentBone.Index]);
                foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
                {
                    int offset = vertices.Count;              
                    var meshVertices = meshPart.VertexBuffer.Resource.Buffer.GetData<JVector>();
                    for (int i = 0; i < meshVertices.Length; ++i)
                    {
                        JVector.Transform(ref meshVertices[i], ref boneTransform, out meshVertices[i]);
                    }
                    vertices.AddRange(meshVertices);    // append transformed vertices

                    // there should DEFINITELY be a check here to ensure that the indices used in the model
                    // don't exceed 65535 (max short int). If the model doesn't use shorts, then they get cast all weird.
                    var indexElements = meshPart.IndexBuffer.Resource.GetData<short>();      
             
                    // Each TriangleVertexIndices holds the indices that constitute a triangle primitive
                    TriangleVertexIndices[] tvi = new TriangleVertexIndices[indexElements.Length];
                    for (int i = 0; i <= tvi.Length - 2; i += 3) {
                        tvi[i].I0 = indexElements[i + 0] + offset;
                        tvi[i].I1 = indexElements[i + 1] + offset;
                        tvi[i].I2 = indexElements[i + 2] + offset;
                    }
                    indices.AddRange(tvi);  // append triangles           
                }
            }
            Octree ot = new Octree(vertices, indices);
            //ot.BuildOctree(); // (already happens in Octree constructor)
            return ot;
        }


        /// <summary>
        /// Helper method to interface SharpDX vector class with Jitter vector class.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static JVector toJVector(Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Helper method to interface SharpDX vector class with Jitter vector class.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 toVector3(JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Helper method to interface SharpDX matrix class with Jitter matrix class.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Matrix toMatrix(JMatrix matrix)
        {
            return new Matrix(
                matrix.M11, matrix.M12, matrix.M13, 0f,
                matrix.M21, matrix.M22, matrix.M23, 0f,
                matrix.M31, matrix.M32, matrix.M33, 0f,
                        0f, 0f, 0f, 1.0f);
        }

        /// <summary>
        /// Helper method to interface SharpDX matrix class with Jitter matrix class.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static JMatrix toJMatrix(Matrix matrix)
        {
            return new JMatrix(
                matrix.M11, matrix.M12, matrix.M13,
                matrix.M21, matrix.M22, matrix.M23,
                matrix.M31, matrix.M32, matrix.M33
                );
        }

        /// <summary>
        /// Adds a unit sized test cube to the physics world.
        /// Deprecated.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mass"></param>
        public void addTestBox(Vector3 position, float mass)
        {
            // these can actually be reused between objects to save memory
            Shape boxShape = new Jitter.Collision.Shapes.BoxShape(new JVector(1f, 1f, 1f));

            RigidBody body = new RigidBody(new BoxShape(new JVector(1.0f, 1.0f, 1.0f)));
            body.IsStatic = false;
            body.AffectedByGravity = true;
            body.Position = new JVector(position.X, position.Y, position.Z);
            this.World.AddBody(body);
        }


        internal void RemoveBody(GameObjects.Abstract.PhysicsDescription physicsDescription)
        {
            World.RemoveBody(physicsDescription.RigidBody);
        }
    }
}
