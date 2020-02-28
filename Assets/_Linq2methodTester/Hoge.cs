using System;

namespace _Script
{
    public class Hoge : IEquatable<Hoge>
    {
        public string Name;

        public Hoge(string name)
        {
            Name = name;
        }

        public bool Equals(Hoge other)
        {
            return other != null && other.Name == Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Hoge) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Hoge left, Hoge right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Hoge left, Hoge right)
        {
            return !Equals(left, right);
        }
    }
}