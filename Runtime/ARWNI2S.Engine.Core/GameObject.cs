using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARWNI2S.Engine
{
    public abstract class GameObject : EntityBase, IEquatable<GameObject>
    {
        public bool Equals(GameObject other)
        {
            throw new NotImplementedException();
        }
    }
}
