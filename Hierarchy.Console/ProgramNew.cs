using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hierarchy.Console.Algorithms;

namespace Hierarchy.Console
{
    [DebuggerDisplay("{Id} : {Name}")]
    public class Customer
    {
        public int Id { get; set; }
        public int ManagerId { get; set; }
        public string Name { get; set; }
    }

    class ProgramNew
    {
        static void Main(string[] args)
        {
            var customerCollection1 = new List<Customer>
            {
                new Customer{Name = "Alan", Id = 100, ManagerId = 150},
                new Customer{Name = "Martin", Id = 220, ManagerId = 100},
                new Customer{Name = "Jamie", Id = 150, ManagerId = 150},
                new Customer{Name = "Alex", Id = 275, ManagerId = 100},
                new Customer{Name = "Steve", Id = 400, ManagerId = 150},
                new Customer{Name = "David", Id = 190, ManagerId = 400},
                new Customer{Name = "A", Id = 500, ManagerId = 190},
                new Customer{Name = "B", Id = 501, ManagerId = 500},
                new Customer{Name = "Cheranga", Id = 666, ManagerId = 666}
            };

            var relationships = customerCollection1.GetNodes(x => x.Id, x => x.ManagerId);

            Display(relationships.First());

            System.Console.ReadLine();
        }

        static void Display(Node<Customer> hierarchy)
        {
            if (hierarchy == null)
            {
                return;
            }

            if (false)
            {
                //System.Console.ForegroundColor = ConsoleColor.Red;
                //System.Console.WriteLine("ERROR : {0}", hierarchy.Exception.Message);
                //System.Console.ResetColor();
            }
            else
            {
                Action<Node<Customer>, string> displayFunc = null;
                displayFunc = (customer, tabFormat) =>
                {
                    if (customer == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(tabFormat))
                    {
                        System.Console.Write("\n{0}", customer.Data.Name);
                    }
                    else
                    {
                        System.Console.Write($"\n{tabFormat}{customer.Data.Name}");
                    }

                    if (customer.SubNodes != null && customer.SubNodes.Any())
                    {
                        tabFormat += "\t";
                        foreach (var teamMember in customer.SubNodes)
                        {
                            displayFunc(teamMember, tabFormat);
                        }
                    }
                };

                System.Console.WriteLine(hierarchy.Data.Name);
                System.Console.WriteLine();
                foreach (var executiveTeam in hierarchy.SubNodes)
                {
                    displayFunc(executiveTeam, "\t");
                }
            }
        }
    }
}
