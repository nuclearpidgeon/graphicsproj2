using System.Collections.Generic;
using SharpDX.Toolkit;

namespace Project2.GameObjects.Interface
{
    public interface INode
    {
        INode Parent { get; set; }
        List<INode> Children { get; set; }
        void AddChild(INode childNode);
        void RemoveChild(INode childNode);
        void Draw(GameTime gametime);
        void Update(GameTime gametime);
    }
}
