using System;
using System.Collections.Generic;
using System.Linq;
using TrackableEntities.EF.Core.Tests.NorthwindModels;

namespace TrackableEntities.EF.Core.Tests.Mocks;

public class MockNorthwind
{
    public List<Category> Categories { get; private set; }
    public List<Product> Products { get; private set; }
    public List<Customer> Customers { get; private set; }
    public List<Order> Orders { get; private set; }
    public List<Employee> Employees { get; set; }
    public List<Territory> Territories { get; set; }
    public List<Area> Areas { get; set; }
    public MockNorthwind()
    {
        Categories =
        [
            new() { CategoryId = 1, CategoryName = "Beverages" },
            new() { CategoryId = 2, CategoryName = "Condiments" },
            new() { CategoryId = 3, CategoryName = "Confections" },
            new() { CategoryId = 4, CategoryName = "Dairy Products" },
            new() { CategoryId = 5, CategoryName = "Grains/Cereals" },
            new() { CategoryId = 6, CategoryName = "Meat/Poultry" },
            new() { CategoryId = 7, CategoryName = "Produce" },
            new() { CategoryId = 8, CategoryName = "Seafood" }
        ];
        Products =
        [
            new() { ProductId = 1, ProductName = "Chai", UnitPrice = 21.0000M, Discontinued = false, CategoryId = 1, Category = Categories[0] },
            new() { ProductId = 2, ProductName = "Chang", UnitPrice = 20.0000M, Discontinued = false, CategoryId = 1, Category = Categories[0] },
            new() { ProductId = 3, ProductName = "Aniseed Syrup", UnitPrice = 10.0000M, Discontinued = false, CategoryId = 2, Category = Categories[1] },
            new() { ProductId = 4, ProductName = "Chef Anton's Cajun Seasoning", UnitPrice = 22.0000M, Discontinued = false, CategoryId = 2, Category = Categories[1] },
            new() { ProductId = 5, ProductName = "Chef Anton's Gumbo Mix", UnitPrice = 21.3500M, Discontinued = true, CategoryId = 2, Category = Categories[1] },
            new() { ProductId = 6, ProductName = "Grandma's Boysenberry Spread", UnitPrice = 27.0000M, Discontinued = false, CategoryId = 2, Category = Categories[1] },
            new() { ProductId = 7, ProductName = "Uncle Bob's Organic Dried Pears", UnitPrice = 30.0000M, Discontinued = false, CategoryId = 7, Category = Categories[6] },
            new() { ProductId = 8, ProductName = "Northwoods Cranberry Sauce", UnitPrice = 40.0000M, Discontinued = false, CategoryId = 2, Category = Categories[1] },
            new() { ProductId = 9, ProductName = "Mishi Kobe Niku", UnitPrice = 97.0000M, Discontinued = true, CategoryId = 6, Category = Categories[5] },
            new() { ProductId = 10, ProductName = "Ikura", UnitPrice = 31.0000M, Discontinued = false, CategoryId = 8, Category = Categories[7] },
            new() { ProductId = 11, ProductName = "Queso Cabrales", UnitPrice = 21.0000M, Discontinued = false, CategoryId = 4, Category = Categories[3] },
            new() { ProductId = 12, ProductName = "Queso Manchego La Pastora", UnitPrice = 38.0000M, Discontinued = false, CategoryId = 4, Category = Categories[3] },
            new() { ProductId = 13, ProductName = "Konbu", UnitPrice = 6.0000M, Discontinued = false, CategoryId = 8, Category = Categories[7] },
            new() { ProductId = 14, ProductName = "Tofu", UnitPrice = 23.2500M, Discontinued = false, CategoryId = 7, Category = Categories[6] },
            new() { ProductId = 15, ProductName = "Genen Shouyu", UnitPrice = 15.5000M, Discontinued = false, CategoryId = 2, Category = Categories[1] },
            new() { ProductId = 16, ProductName = "Pavlova", UnitPrice = 17.4500M, Discontinued = false, CategoryId = 3, Category = Categories[2] },
            new() { ProductId = 17, ProductName = "Alice Mutton", UnitPrice = 39.0000M, Discontinued = true, CategoryId = 6, Category = Categories[5] },
            new() { ProductId = 18, ProductName = "Carnarvon Tigers", UnitPrice = 62.5000M, Discontinued = false, CategoryId = 8, Category = Categories[7] },
            new() { ProductId = 19, ProductName = "Teatime Chocolate Biscuits", UnitPrice = 9.2000M, Discontinued = false, CategoryId = 3, Category = Categories[2] },
            new() { ProductId = 20, ProductName = "Sir Rodney's Marmalade", UnitPrice = 81.0000M, Discontinued = false, CategoryId = 3, Category = Categories[2] },
            new() { ProductId = 22, ProductName = "Gustaf's Knäckebröd", UnitPrice = 21.0000M, Discontinued = false, CategoryId = 5, Category = Categories[4] },
            new() { ProductId = 33, ProductName = "Geitost", UnitPrice = 2.50M, Discontinued = false, CategoryId = 4, Category = Categories[3] },
            new() { ProductId = 41, ProductName = "Jack's New England Clam Chowder", UnitPrice = 9.65M, Discontinued = false, CategoryId = 8, Category = Categories[7] },
            new() { ProductId = 42, ProductName = "Singaporean Hokkien Fried Mee", UnitPrice = 14.0000M, Discontinued = true, CategoryId = 5, Category = Categories[4] },
            new() { ProductId = 51, ProductName = "Manjimup Dried Apples", UnitPrice = 53.00M, Discontinued = false, CategoryId = 4, Category = Categories[3] },
            new() { ProductId = 57, ProductName = "Ravioli Angelo", UnitPrice = 19.50M, Discontinued = false, CategoryId = 5, Category = Categories[4] },
            new() { ProductId = 60, ProductName = "Camembert Pierrot", UnitPrice = 34.50M, Discontinued = false, CategoryId = 4, Category = Categories[3] },
            new() { ProductId = 65, ProductName = "Louisiana Fiery Hot Pepper Sauce", UnitPrice = 21.05M, Discontinued = false, CategoryId = 2, Category = Categories[1] },
            new() { ProductId = 72, ProductName = "Mozzarella di Giovanni", UnitPrice = 34.80M, Discontinued = false, CategoryId = 4, Category = Categories[3] },
        ];
        Categories[0].Products = new List<Product>(Products.Where(p => p.CategoryId == 1));
        Categories[1].Products = new List<Product>(Products.Where(p => p.CategoryId == 2));
        Categories[2].Products = new List<Product>(Products.Where(p => p.CategoryId == 2));
        Categories[3].Products = new List<Product>(Products.Where(p => p.CategoryId == 4));
        Categories[4].Products = new List<Product>(Products.Where(p => p.CategoryId == 5));
        Categories[5].Products = new List<Product>(Products.Where(p => p.CategoryId == 6));
        Categories[6].Products = new List<Product>(Products.Where(p => p.CategoryId == 7));
        Categories[7].Products = new List<Product>(Products.Where(p => p.CategoryId == 8));
        Customers =
        [
            new() { CustomerId = "ALFKI", CustomerName = "Alfreds Futterkiste"},
            new() { CustomerId = "ANATR", CustomerName = "Ana Trujillo Emparedados y helados"},
            new() { CustomerId = "ANTON", CustomerName = "Antonio Moreno Taquería"},
            new() { CustomerId = "AROUT", CustomerName = "Around the Horn"},
            new() { CustomerId = "BERGS", CustomerName = "Berglunds snabbköp"},
            new() { CustomerId = "BLAUS", CustomerName = "Blauer See Delikatessen"},
            new() { CustomerId = "BLONP", CustomerName = "Blondesddsl père et fils"},
            new() { CustomerId = "BOLID", CustomerName = "Bólido Comidas preparadas"},
            new() { CustomerId = "BONAP", CustomerName = "Bon app"},
            new() { CustomerId = "BOTTM", CustomerName = "Bottom-Dollar Markets"},
            new() { CustomerId = "HANAR", CustomerName = "Hanari Carnes"},
            new() { CustomerId = "SUPRD", CustomerName = "Suprêmes délices"},
            new() { CustomerId = "TOMSP", CustomerName = "Toms Spezialitäten"},
            new() { CustomerId = "VICTE", CustomerName = "Victuailles en stock"},
            new() { CustomerId = "VINET", CustomerName = "Vins et alcools Chevalier"}
        ];
        Orders =
        [
            new() { OrderId = 10248, OrderDate = DateTime.Parse("1996-07-04"), CustomerId = "VINET", Customer = Customers[14],
                OrderDetails =
                    [
                        new() { OrderDetailId = 1, ProductId = 11, OrderId = 10248, Quantity = 12, UnitPrice = 14.0000M, Product = Products[10] },
                        new() { OrderDetailId = 2, ProductId = 42, OrderId = 10248, Quantity = 10, UnitPrice = 9.8000M, Product = Products[23] },
                        new() { OrderDetailId = 3, ProductId = 72, OrderId = 10248, Quantity = 5, UnitPrice = 34.8000M, Product = Products[28] },
                        new() { OrderDetailId = 4, ProductId = 4, OrderId = 10248, Quantity = 4, UnitPrice = 40.0000M, Product = Products[3] }
                    ]},
            new() { OrderId = 10249, OrderDate = DateTime.Parse("1996-07-05"), CustomerId = "TOMSP", Customer = Customers[12],
                OrderDetails =
                    [
                        new() { OrderDetailId = 5, ProductId = 14, OrderId = 10249, Quantity = 9, UnitPrice = 18.6000M, Product = Products[13] },
                        new() { OrderDetailId = 6, ProductId = 51, OrderId = 10249, Quantity = 40, UnitPrice = 42.4000M, Product = Products[24] }
                    ]},
            new() { OrderId = 10250, OrderDate = DateTime.Parse("1996-07-08"), CustomerId = "HANAR", Customer = Customers[10],
                OrderDetails =
                    [
                        new() { OrderDetailId = 7, ProductId = 41, OrderId = 10250, Quantity = 10, UnitPrice = 7.7000M, Product = Products[22] },
                        new() { OrderDetailId = 8, ProductId = 51, OrderId = 10250, Quantity = 12, UnitPrice = 42.4000M, Product = Products[24] },
                        new() { OrderDetailId = 9, ProductId = 65, OrderId = 10250, Quantity = 12, UnitPrice = 16.8000M, Product = Products[27] }
                    ]},
            new() { OrderId = 10251, OrderDate = DateTime.Parse("1996-07-08"), CustomerId = "VICTE", Customer = Customers[13],
                OrderDetails =
                    [
                        new() { OrderDetailId = 10, ProductId = 22, OrderId = 10251, Quantity = 6, UnitPrice = 16.8000M, Product = Products[20] },
                        new() { OrderDetailId = 11, ProductId = 57, OrderId = 10251, Quantity = 15, UnitPrice = 15.6000M, Product = Products[25] },
                        new() { OrderDetailId = 12, ProductId = 65, OrderId = 10251, Quantity = 20, UnitPrice = 16.8000M, Product = Products[27] }
                    ]},
            new() { OrderId = 10252, OrderDate = DateTime.Parse("1996-07-09"), CustomerId = "SUPRD", Customer = Customers[11],
                OrderDetails =
                    [
                        new() { OrderDetailId = 13, ProductId = 20, OrderId = 10252, Quantity = 40, UnitPrice = 64.8000M, Product = Products[19] },
                        new() { OrderDetailId = 14, ProductId = 33, OrderId = 10252, Quantity = 25, UnitPrice = 2.0000M, Product = Products[21] },
                        new() { OrderDetailId = 15, ProductId = 60, OrderId = 10252, Quantity = 40, UnitPrice = 27.2000M, Product = Products[26] },
                    ]},
        ];
        
        Areas =
        [
            new() { AreaId = 1, AreaName = "Northern"},
            new() { AreaId = 2, AreaName = "Southern"},
        ];

        Territories =
        [
            new() { TerritoryId = "01581", TerritoryDescription = "Westboro"},
            new() { TerritoryId = "01730", TerritoryDescription = "Bedford"},
            new() { TerritoryId = "01833", TerritoryDescription = "Georgetown"},
            new() { TerritoryId = "02116", TerritoryDescription = "Boston"},
            new() { TerritoryId = "02139", TerritoryDescription = "Cambridge"},
            new() { TerritoryId = "02184", TerritoryDescription = "Braintree"},
            new() { TerritoryId = "02903", TerritoryDescription = "Providence"},
            new() { TerritoryId = "03049", TerritoryDescription = "Hollis"},
            new() { TerritoryId = "03801", TerritoryDescription = "Portsmouth"},
            new() { TerritoryId = "06897", TerritoryDescription = "Wilton"},
            new() { TerritoryId = "07960", TerritoryDescription = "Morristown"},
            new() { TerritoryId = "08837", TerritoryDescription = "Edison"},
            new() { TerritoryId = "10019", TerritoryDescription = "New York"},
            new() { TerritoryId = "11747", TerritoryDescription = "Melville"},
            new() { TerritoryId = "14450", TerritoryDescription = "Fairport"},
            new() { TerritoryId = "19428", TerritoryDescription = "Philadelphia"},
            new() { TerritoryId = "19713", TerritoryDescription = "Neward"},
            new() { TerritoryId = "20852", TerritoryDescription = "Rockville"},
            new() { TerritoryId = "27403", TerritoryDescription = "Greensboro"},
            new() { TerritoryId = "27511", TerritoryDescription = "Cary"},
        ];
        Employees =
        [
            new() { EmployeeId = 1, LastName = "Davolio", FirstName = "Nancy", BirthDate = DateTime.Parse("1948-12-08"), HireDate = DateTime.Parse("1992-05-01"), City = "Seattle", Country = "USA",
              Territories = [Territories[0], Territories[1], Territories[2]] },
            new() { EmployeeId = 2, LastName = "Fuller", FirstName = "Andrew", BirthDate = DateTime.Parse("1952-02-19"), HireDate = DateTime.Parse("1992-08-14"), City = "Tacoma", Country = "USA",
              Territories = [Territories[3], Territories[4]] },
            new() { EmployeeId = 3, LastName = "Leverling", FirstName = "Janet", BirthDate = DateTime.Parse("1963-08-30"), HireDate = DateTime.Parse("1992-05-01"), City = "Kirkland", Country = "USA",
              Territories = [Territories[5], Territories[6]] },
            new() { EmployeeId = 4, LastName = "Peacock", FirstName = "Margaret", BirthDate = DateTime.Parse("1937-09-19"), HireDate = DateTime.Parse("1993-05-03"), City = "Redmond", Country = "USA",
              Territories = [Territories[7], Territories[8]] },
            new() { EmployeeId = 5, LastName = "Buchanan", FirstName = "Steven", BirthDate = DateTime.Parse("1955-03-04"), HireDate = DateTime.Parse("1993-10-17"), City = "London", Country = "UK",
              Territories = [Territories[9], Territories[10]] },
            new() { EmployeeId = 6, LastName = "Suyama", FirstName = "Michael", BirthDate = DateTime.Parse("1963-07-02"), HireDate = DateTime.Parse("1993-10-17"), City = "London", Country = "UK",
              Territories = [Territories[11], Territories[12]] },
            new() { EmployeeId = 7, LastName = "King", FirstName = "Robert", BirthDate = DateTime.Parse("1960-05-29"), HireDate = DateTime.Parse("1994-01-02"), City = "London", Country = "UK",
              Territories = [Territories[13], Territories[14]] },
            new() { EmployeeId = 8, LastName = "Callahan", FirstName = "Laura", BirthDate = DateTime.Parse("1958-01-09"), HireDate = DateTime.Parse("1994-03-05"), City = "Seattle", Country = "USA",
              Territories = [Territories[15], Territories[16]] },
            new() { EmployeeId = 9, LastName = "Dodsworth", FirstName = "Anne", BirthDate = DateTime.Parse("1966-01-27"), HireDate = DateTime.Parse("1994-11-15"), City = "London", Country = "UK",
              Territories = [Territories[17], Territories[18]] },
        ];            
    }
}
