﻿//-----------------------------------------------------------------------------
// <copyright file="IAsyncEnumerableController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.AspNetCore.OData.E2E.Tests.IAsyncEnumerableTests
{
    public class CustomersController : ODataController
    {
        private readonly IAsyncEnumerableContext _context;

        public CustomersController(IAsyncEnumerableContext context)
        {
            context.Database.EnsureCreated();
            _context = context;

            if (!_context.Customers.Any())
            {
                Generate();
            }
        }

        [EnableQuery]
        [HttpGet("v1/Customers")]
        public IAsyncEnumerable<Customer> CustomersData()
        {
            IAsyncEnumerable<Customer> customers = CreateCollectionAsync<Customer>();

            return customers;
        }

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IAsyncEnumerable<Customer> Get()
        {
            return _context.Customers.AsAsyncEnumerable();
        }

        public async IAsyncEnumerable<Customer> CreateCollectionAsync<T>()
        {
            await Task.Delay(5);
            // Yield the items one by one asynchronously
            yield return new Customer
            {
                Id = 1,
                Name = "Customer1",
                Orders = new List<Order> {
                    new Order {
                        Name = "Order1",
                        Price = 25
                    },
                    new Order {
                         Name = "Order2",
                         Price = 75
                    }
                },
                Address = new Address
                {
                    Name = "City1",
                    Street = "Street1"
                }
            };

            await Task.Delay(5);

            yield return new Customer
            {
                Id = 2,
                Name = "Customer2",
                Orders = new List<Order> {
                    new Order {
                        Name = "Order1",
                        Price = 35
                    },
                    new Order {
                         Name = "Order2",
                         Price = 65
                    }
                },
                Address = new Address
                {
                    Name = "City2",
                    Street = "Street2"
                }
            };
        }

        public void Generate()
        {
            for (int i = 1; i <= 3; i++)
            {
                var customer = new Customer
                {
                    Name = "Customer" + (i + 1) % 2,
                    Orders =
                        new List<Order> {
                            new Order {
                                Name = "Order" + 2*i,
                                Price = i * 25  
                            },
                            new Order {
                                Name = "Order" + 2*i+1,
                                Price = i * 75
                            }
                        },
                    Address = new Address
                    {
                        Name = "City" + i % 2,
                        Street = "Street" + i % 2,
                    }
                };

                _context.Customers.Add(customer);
            }

            _context.SaveChanges();
        }
    }
}
