namespace Crimson
{
    public abstract class VirtualAsset
    {
        public string Name   { get; protected set; }
        public int    Width  { get; protected set; }
        public int    Height { get; protected set; }

        internal virtual void Unload()
        {

        }

        internal virtual void Reload()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}
