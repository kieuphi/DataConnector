using System.Collections.Generic;

namespace DataConnector
{
    public abstract class BaseDataTable<T>
    {
        protected BaseDataTable()
        {
        }

        public abstract IList<T> All { get; }

        public abstract T Find(long ID);

        public abstract IList<T> Get(long[] IDs);

        public abstract long Insert(T row);

        public abstract void Update(T row);
    }
}