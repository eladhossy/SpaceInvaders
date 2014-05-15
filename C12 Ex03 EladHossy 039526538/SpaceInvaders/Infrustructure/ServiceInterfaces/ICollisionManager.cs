using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.ServiceInterfaces
{
    public interface ICollisionManager
    {
        void AddObjectToMonitor(ICollidable i_collidable);
    }
}
