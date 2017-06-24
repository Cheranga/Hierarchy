using System.Collections.Generic;

namespace Hierarchy.Console
{
    public class HierarchyResult
    {
        public Dictionary<int, Employee> Executives { get; set; }
        public string ErrorMessage { get; set; }

        public bool HasError
        {
            get { return !string.IsNullOrEmpty(ErrorMessage); }
        }

        public HierarchyResult()
        {
            Executives = new Dictionary<int, Employee>();
        }
    }
}