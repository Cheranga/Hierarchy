using System;
using System.Collections.Generic;

namespace Hierarchy.Console
{
    public class HierarchyResult
    {
        public Dictionary<int, Employee> Executives { get; set; }
        public Exception Exception { get; set; }

        public bool HasError
        {
            get { return Exception != null; }
        }

        public HierarchyResult()
        {
            Executives = new Dictionary<int, Employee>();
        }
    }

    public class HierarchyResult<T> where T : class
    {
        public Dictionary<object, T> Executives { get; set; }
        public Func<T, object> UniqueIdentifier { get; set; }
        public Func<T, object> ParentIdentifier { get; set; }
        public Action<T, T> AddFunc { get; set; }
        public Exception Exception { get; set; }

        public bool HasError
        {
            get { return Exception != null; }
        }

        public HierarchyResult(Func<T, object> uniqueIdentifier, Func<T, object> parentIdentifier, Action<T, T> addFunc)
        {
            UniqueIdentifier = uniqueIdentifier;
            ParentIdentifier = parentIdentifier;
            AddFunc = addFunc;

            Executives = new Dictionary<object, T>();

        }
    }
}