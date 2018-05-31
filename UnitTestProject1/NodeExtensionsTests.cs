using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hierarchy.Console;
using Hierarchy.Console.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hierarchy.Tests
{
    [TestClass]
    public class NodeExtensionsTests
    {
        [TestMethod]
        public void WhenTheCollectionIsNull()
        {
            //
            // Arrange
            //
            List<Customer> customers = null;
            //
            // Act
            //
            var nodes = customers.GetNodes(x => x.Id, x => x.ManagerId);
            //
            // Assert
            //
            Assert.IsNotNull(nodes);
        }

        [TestMethod]

        public void WhenTheCollectionIsEmpty()
        {
            //
            // Arrange
            //
            var customers = new List<Customer>();
            //
            // Act
            //
            var nodes = customers.GetNodes(x => x.Id, x => x.ManagerId);
            //
            // Assert
            //
            Assert.IsNotNull(nodes);
        }

        [TestMethod]
        public void When_Collection_Contains_Unrelated_Items_Must_Return_Only_Related_Items()
        {
            //
            // Arrange
            //
            var customersWithRelationships = new List<Customer>
            {
                new Customer {Id = 1, ManagerId = 1, Name = "Cheranga"},
                new Customer {Id = 2, ManagerId = 1, Name = "Some one who works with Cheranga"}
            };
            var customersWithoutRelationships = new List<Customer>
            {
                new Customer {Id = 1, ManagerId = 2, Name = "Somebody"},
                new Customer {Id = 3, ManagerId = 4, Name = "Anybody"}
            };

            //
            // Act and Assert
            //
            var nodes = customersWithRelationships.GetNodes(x => x.Id, x => x.ManagerId);

            Assert.IsTrue(nodes.Count > 0);
            Assert.IsTrue(nodes.First().Data == customersWithRelationships.First());

            nodes = customersWithoutRelationships.GetNodes(x => x.Id, x => x.ManagerId);
            Assert.IsTrue(nodes.Count == 0);
        }

        [TestMethod]
        public void When_Collection_Has_Valid_Relationships()
        {
            //
            // Arrange
            //
            var customers = new List<Customer>
            {
                new Customer {Id = 1, ManagerId = 1, Name = "THE Boss"},
                new Customer {Id = 2, ManagerId = 1, Name = "Boss 1"},
                new Customer {Id = 3, ManagerId = 2, Name = "Worker 1 for boss 1"},
                new Customer {Id = 4, ManagerId = 2, Name = "Worker 2 for boss 1"},

                new Customer {Id = 5, ManagerId = 1, Name = "Boss 2"},
                new Customer {Id = 6, ManagerId = 5, Name = "Worker 1 for boss 2"},
                new Customer {Id = 7, ManagerId = 5, Name = "Worker 2 for boss 2"}
            };
            //
            // Act
            //
            var nodes = customers.GetNodes(x => x.Id, x => x.ManagerId);
            //
            // Assert
            //
            // There must be only one boss
            Assert.IsTrue(nodes.Count == 1 && nodes.First().Data.Id == 1);
            // There must be two guys who works for "THE" boss
            Assert.IsTrue(nodes.First().SubNodes.Count == 2);
            // These support bosses must have 2 workers each
            //
            Assert.IsTrue(nodes.First().SubNodes.All(x => x.SubNodes.Count == 2));
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
                        Debug.Write("\n{0}", customer.Data.Name);
                    }
                    else
                    {
                        Debug.Write($"\n{tabFormat}{customer.Data.Name}");
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
