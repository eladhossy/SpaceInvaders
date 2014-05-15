using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.ServiceInterfaces
{
    public delegate void PositionChangedEventHandler(ICollidable i_Collidable);

    public interface ICollidable
    {
        event PositionChangedEventHandler PositionChanged;
        event EventHandler<EventArgs> Disposed;
        bool Visible { get; set; }
        bool CheckCollision(ICollidable i_Collidable);
        void Collided(ICollidable i_Collidable);
        
    }
}
