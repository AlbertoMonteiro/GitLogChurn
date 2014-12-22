using System;

namespace GitLogChurn
{
    public sealed class Modification
    {
        private readonly DateTime date;
        private readonly string author;

        public Modification(DateTime date, string author)
        {
            this.date = date;
            this.author = author;
        }

        public DateTime Date { get { return date; }  }

        public string Author { get { return author; }  }
    }
}