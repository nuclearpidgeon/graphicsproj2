using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project2.Components
{
    public class Component
    {
        public Project2Game game;
        public GameObject owner;

        public Component(Project2Game game, GameObject owner) {
            this.game = game;
            this.owner = owner;
        }
    }
}
