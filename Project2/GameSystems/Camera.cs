using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Project2.GameSystems
{
    public abstract class Camera : IUpdateable
    {
        public Matrix view { get; private set; }
        public Matrix projection { get; private set; }

        public Vector3 position { get; private set; }
        public Vector3 direction { get; private set; }
        public Vector3 movement { get; private set; }

        public abstract void Update(GameTime gameTime);

        // --- IUpdateable stubs below this line ---

        public bool Enabled
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> EnabledChanged;
        
        public int UpdateOrder
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged;
    }
}
