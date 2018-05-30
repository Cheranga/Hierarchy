using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hierarchy.Console;
using Hierarchy.Console.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
   

    [TestClass]
    public class RelationshipTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var customerCollection1 = new List<Customer>
            {
                new Customer{Id = 1, ManagerId = 1, Name = "1"},
                new Customer{Id = 1, ManagerId = 1, Name = "CEO"},
            };

            var relationships = customerCollection1.GetRelationships(x=>x.Id, x=>x.ManagerId);

            Display(relationships.First());
        }

        static void Display(Nodes<Customer> hierarchy)
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
                Action<Nodes<Customer>, string> displayFunc = null;
                displayFunc = (customer, tabFormat) =>
                {
                    if (customer == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(tabFormat))
                    {
                        Debug.Write("\n{0}", customer.Item.Name);
                    }
                    else
                    {
                        Debug.Write($"\n{tabFormat}{customer.Item.Name}");
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

                foreach (var executiveTeam in hierarchy.SubNodes)
                {
                    displayFunc(executiveTeam, "");
                }
            }
        }
    }
}
