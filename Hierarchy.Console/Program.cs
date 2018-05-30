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
        static HierarchyResult<T> GetHierarchy<T>(IEnumerable<T> collection,
            Func<T, object> uniqueIdentifier,
            Func<T, object> parentIdentifier,
            Action<T, T> addFunc) where T : class
        {
            var hierarchy = new HierarchyResult<T>(uniqueIdentifier, parentIdentifier, addFunc);

            try
            {
                var list = collection as List<T> ?? new List<T>(collection);
                if (list.Any() == false)
                {
                    hierarchy.Exception = new Exception("Please provide employees");
                    return hierarchy;
                }

                var entitiesMappedByKey = list.GroupBy(uniqueIdentifier).ToDictionary(grp => grp.Key, grp => grp.Single());

                //
                // A dictionary to maintain who are the employees which we have already discovered
                //
                var visitedEntitiesMappedByKey = new Dictionary<object, T>();
                //
                // Recursive function, to go through employees, and oragnize their teams
                //
                Action<IDictionary<object, T>, T, T> recursive = null;
                recursive = (dict, entity, childEntity) =>
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
                    if (childEntity != null)
                    {
                        addFunc(entity, childEntity);
                    }
                    //
                    // If we have already processed the current employee, no need to continue further
                    //
                    if (visitedEntitiesMappedByKey.ContainsKey(uniqueIdentifier(entity)))
                    {
                        return;
                    }
                    //
                    // If the employee has a manager, recursively call the function again, passing the employee as a "subordinate"
                    // of his manager (in this case the "manager")
                    //
                    var parentKeyValue = parentIdentifier(entity);
                    if (parentKeyValue != null)
                    {
                        //
                        // If the manager is not a valid employee, set the error message
                        //
                        if (dict.ContainsKey(parentKeyValue))
                        {
                            recursive(dict, dict[parentKeyValue], entity);
                        }
                        else
                        {
                            hierarchy.Exception = new Exception(string.Format("Invalid employee with Id:[{0}]", parentKeyValue));
                        }
                    }
                    else
                    {
                        if (hierarchy.Executives.ContainsKey(uniqueIdentifier(entity)) == false)
                        {
                            hierarchy.Executives.Add(uniqueIdentifier(entity), entity);
                        }
                    }

                    visitedEntitiesMappedByKey.Add(uniqueIdentifier(entity), entity);
                };

                //
                // Call the function to build the hierarchy
                //
                foreach (var employee in list)
                {
                    recursive(entitiesMappedByKey, employee, null);
                }
            }
            catch (InvalidOperationException exception)
            {
                hierarchy.Exception = new Exception(@"Provided unique identifier cannot identify entities distinctly");

            }
            catch (Exception exception)
            {
                hierarchy.Exception = new Exception("Cannot create the hierarchy");
            }

            return hierarchy;
        }

        static HierarchyResult GetHierarchy(IEnumerable<Employee> employees)
        {
            var hierarchy = new HierarchyResult();

            try
            {
                var employeeList = employees as List<Employee> ?? new List<Employee>(employees);
                if (employeeList.Any() == false)
                {
                    return new HierarchyResult
                    {
                        Exception = new Exception("Please provide employees")
                    };
                }



                var employeesMappedById = employeeList.GroupBy(emp => emp.Id).ToDictionary(grp => grp.Key, grp => grp.Single());

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
                            hierarchy.Exception = new Exception(string.Format("Invalid employee with Id:[{0}]", employee.ManagerId));
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
            }
            catch (InvalidOperationException exception)
            {
                hierarchy.Exception = new Exception("Employee id's are not unique");

            }
            catch (Exception exception)
            {
                hierarchy.Exception = new Exception("Cannot create the hierarchy");
            }

            return hierarchy;
        }

        static void Main(string[] args)
        {
            var employeesCollection1 = new List<Employee>
            {
                new Employee{Name = "Alan", Id = 100, ManagerId = 150},
                new Employee{Name = "Martin", Id = 220, ManagerId = 100},
                new Employee{Name = "Jamie", Id = 150, ManagerId = null},
                new Employee{Name = "Alex", Id = 275, ManagerId = 100},
                new Employee{Name = "Steve", Id = 400, ManagerId = 150},
                new Employee{Name = "David", Id = 190, ManagerId = 400},
                new Employee{Name = "A", Id = 500, ManagerId = 190},
                new Employee{Name = "B", Id = 501, ManagerId = 500},
                new Employee{Name = "Cheranga", Id = 666, ManagerId = null}
            };

            var employeesCollection2 = new List<Employee>
            {
                new Employee{Name = "Alan", Id = 100, ManagerId = 150},
                new Employee{Name = "Martin", Id = 220, ManagerId = 100},
                new Employee{Name = "Jamie", Id = 150, ManagerId = null},
                new Employee{Name = "Alex", Id = 275, ManagerId = 100},
                new Employee{Name = "Steve", Id = 400, ManagerId = 150},
                new Employee{Name = "David", Id = 190, ManagerId = 400},
                new Employee{Name = "A", Id = 500, ManagerId = 190},
                new Employee{Name = "B", Id = 501, ManagerId = 500},
                new Employee{Name = "Cheranga", Id = 666, ManagerId = null}
            };

            var employeesCollection3 = new List<Employee>
            {
                new Employee{Name = "CEO", Id = 1, ManagerId = null},
                new Employee{Name = "Alan", Id = 2, ManagerId = 1}
            };

            var hierarchy1 = GetHierarchy(employeesCollection1);
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("NON GENERIC APPROACH...\n===========================");
            Display(hierarchy1);

            System.Console.WriteLine("\n\nGENERIC APPROACH....\n===========================");

            System.Console.ForegroundColor = ConsoleColor.Yellow;

            var hierarchy2 = GetHierarchy(employeesCollection2, employee => employee.Id, employee => employee.ManagerId, (employee, subOrdinate) =>
            {
                if (employee != null && subOrdinate != null)
                {
                    employee.Team = employee.Team ?? new List<Employee>();
                    employee.Team.Add(subOrdinate);
                }
            });
            Display(hierarchy2,employee => employee.Team, employee => employee.Name);


            var hierarchy3 = GetHierarchy(employeesCollection3, employee => employee.Id, employee => employee.ManagerId, (employee, subOrdinate) =>
            {
                if (employee != null && subOrdinate != null)
                {
                    employee.Team = employee.Team ?? new List<Employee>();
                    employee.Team.Add(subOrdinate);
                }
            });


            System.Console.ResetColor();
            System.Console.ReadLine();
        }

        static void Display(HierarchyResult hierarchy)
        {
            if (hierarchy == null)
            {
                return;
            }

            if (hierarchy.HasError)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("ERROR : {0}", hierarchy.Exception.Message);
                System.Console.ResetColor();
            }
            else
            {
                Action<Employee, string> displayFunc = null;
                displayFunc = (employee, tabFormat) =>
                {
                    if (employee == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(tabFormat))
                    {
                        System.Console.Write("\n{0}", employee.Name);
                    }
                    else
                    {
                        System.Console.Write("\n{0}{1}", tabFormat, employee.Name);
                    }

                    if (employee.Team != null && employee.Team.Any())
                    {
                        tabFormat += "\t";
                        foreach (var teamMember in employee.Team)
                        {
                            displayFunc(teamMember, tabFormat);
                        }
                    }
                };

                foreach (var executiveTeam in hierarchy.Executives)
                {
                    displayFunc(executiveTeam.Value, "");
                }
            }
        }

        static void Display<T>(HierarchyResult<T> hierarchy, Func<T, IEnumerable<T>> collectionIdentifier, Func<T, string> print ) where T:class 
        {
            if (hierarchy == null)
            {
                return;
            }

            if (hierarchy.HasError)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("ERROR : {0}", hierarchy.Exception.Message);
                System.Console.ResetColor();
            }
            else
            {
                Action<T, string> displayFunc = null;
                displayFunc = (entity, tabFormat) =>
                {
                    if (entity == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(tabFormat))
                    {
                        System.Console.Write("\n{0}", print(entity));
                    }
                    else
                    {
                        System.Console.Write("\n{0}{1}", tabFormat, print(entity));
                    }

                    if (collectionIdentifier(entity).Any())
                    {
                        var collection = collectionIdentifier(entity);
                        tabFormat += "\t";
                        foreach (var teamMember in collection)
                        {
                            displayFunc(teamMember, tabFormat);
                        }
                    }
                };

                foreach (var executive in hierarchy.Executives)
                {
                    displayFunc(executive.Value, "");
                }
            }
        }
    }
}
