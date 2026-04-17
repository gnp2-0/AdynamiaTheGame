using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdynamiaTheGame
{
    public abstract class IEnemy
    {
        public abstract void normalAttack(Player player);
        public abstract void specialAttack(Player player);
        public abstract void bigAttack(Player player);

        protected virtual void animate()
        {

        }
    }
}
