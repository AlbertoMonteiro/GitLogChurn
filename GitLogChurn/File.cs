using System.Collections.Generic;

namespace GitLogChurn
{
    public sealed class File
    {
        public IList<Modification> Modifications { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0} modified {1} times", Name, Modifications.Count);
        }
    }
}