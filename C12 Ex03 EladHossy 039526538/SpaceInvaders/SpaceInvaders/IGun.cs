using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceInvaders
{
    public interface IGun
    {
        IGunHolder GunHolder { get; }
    }
}
