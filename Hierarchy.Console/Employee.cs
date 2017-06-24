using System.Collections.Generic;
using System.Diagnostics;

namespace Hierarchy.Console
{
    [DebuggerDisplay("{Name}")]
    public class Employee
    {
        public int Id { get; set; }
        public int? ManagerId { get; set; }
        public string Name { get; set; }
        public List<Employee> Team { get; set; }

        public Employee()
        {
            Team = new List<Employee>();
        }
    }
}
