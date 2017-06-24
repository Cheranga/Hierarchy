using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hierarchy.Console
{
    class Program
    {
        static HierarchyResult GetHierarchy(IEnumerable<Employee> employees)
        {
            var employeeList = employees as List<Employee> ?? new List<Employee>(employees);
            if (employeeList.Any() == false)
            {
                return new HierarchyResult
                {
                    ErrorMessage = "Please provide employees"
                };
            }

            var employeesMappedById = employeeList.GroupBy(emp => emp.Id).ToDictionary(grp => grp.Key, grp => grp.First());

            var hierarchy = new HierarchyResult();
            //
            // A dictionary to maintain who are the employees which we have already discovered
            //
            var visitedEmployees = new Dictionary<int, Employee>();
            //
            // Recursive function, to go through employees, and oragnize their teams
            //
            Action<IDictionary<int, Employee>, Employee, Employee> recursive = null;
            recursive = (dict, employee, subOrdinate) =>
            {
                //
                // If the structure already has an error, no need to continue
                //
                if (hierarchy.HasError)
                {
                    return;
                }
                //
                // If there's a subordinate to the passed employee add him to the "emp" team
                //
                if (subOrdinate != null)
                {
                    employee.Team.Add(subOrdinate);
                }
                //
                // If we have already processed the current employee, no need to continue further
                //
                if (visitedEmployees.ContainsKey(employee.Id))
                {
                    return;
                }
                //
                // If the employee has a manager, recursively call the function again, passing the employee as a "subordinate"
                // of his manager (in this case the "manager")
                //
                if (employee.ManagerId.HasValue)
                {
                    //
                    // If the manager is not a valid employee, set the error message
                    //
                    if (dict.ContainsKey(employee.ManagerId.Value))
                    {
                        recursive(dict, dict[employee.ManagerId.Value], employee);
                    }
                    else
                    {
                        hierarchy.ErrorMessage = string.Format("Invalid employee with Id:[{0}]", employee.ManagerId);
                    }
                }
                else
                {
                    if (hierarchy.Executives.ContainsKey(employee.Id) == false)
                    {
                        hierarchy.Executives.Add(employee.Id, employee);
                    }
                }

                visitedEmployees.Add(employee.Id, employee);
            };

            //
            // Call the function to build the hierarchy
            //
            foreach (var employee in employeeList)
            {
                recursive(employeesMappedById, employee, null);
            }

            return hierarchy;
        }

        static void Main(string[] args)
        {
            var employees = new List<Employee>
            {
                new Employee() {Name = "Alan", Id = 100, ManagerId = 150},
                new Employee() {Name = "Martin", Id = 220, ManagerId = 100},
                new Employee() {Name = "Jamie", Id = 150, ManagerId = null},
                new Employee() {Name = "Alex", Id = 275, ManagerId = 100},
                new Employee() {Name = "Steve", Id = 400, ManagerId = 150},
                new Employee() {Name = "David", Id = 190, ManagerId = 400}
            };

            var hierarchy = GetHierarchy(employees);
        }
    }
}
