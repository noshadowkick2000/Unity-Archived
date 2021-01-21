using System;

namespace gameHub
{
    public class ZombiesHolder : IComparable<ZombiesHolder>
    {
        public SharedZombie SharedZombie;
        public float Distance;
        
        public int CompareTo(ZombiesHolder zombiesHolder)
        {       // A null value means that this object is greater.
            if (zombiesHolder == null){
                return 1;  
            }
            else {
                return this.Distance.CompareTo(zombiesHolder.Distance);
            }
        }
    }
}