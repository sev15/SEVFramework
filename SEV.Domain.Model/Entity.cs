using System;
using System.ComponentModel.DataAnnotations;

namespace SEV.Domain.Model
{
    [Serializable]
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }

        public virtual string EntityId
        {
            get
            {
                return Id.ToString("F0");
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!(obj is Entity) || (Id == default(int)))
            {
                return false;
            }
            return Id == ((Entity)obj).Id;
        }

        public override int GetHashCode()
        {
            return (Id == default(int)) ? base.GetHashCode() : Id.GetHashCode();
        }
    }
}