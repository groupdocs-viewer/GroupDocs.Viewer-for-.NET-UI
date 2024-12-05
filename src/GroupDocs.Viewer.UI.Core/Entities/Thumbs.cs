using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class Thumbs : IEnumerable<Thumb>
    {
        readonly List<Thumb> _thumbs;

        public Thumbs()
        {
            _thumbs = new List<Thumb>();
        }

        public Thumbs(IEnumerable<Thumb> thumbs)
        {
            _thumbs = thumbs.ToList();
        }

        public void Add(Thumb thumb) => _thumbs.Add(thumb);

        public Thumb this[int index]
        {
            get => _thumbs[index];
            set => _thumbs.Insert(index, value);
        }

        public IEnumerator<Thumb> GetEnumerator() 
            => _thumbs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}