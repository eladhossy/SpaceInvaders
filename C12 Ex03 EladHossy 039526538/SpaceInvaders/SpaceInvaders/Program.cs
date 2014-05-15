using System;

namespace SpaceInvaders
{
#if WINDOWS || XBOX
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (SpaceInvaders game = new SpaceInvaders())
            {
                game.Run();
            }
        }
    }
#endif
}

