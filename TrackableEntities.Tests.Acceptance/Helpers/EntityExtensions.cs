using System;
using TrackableEntities.Client.Core;
using TrackableEntities.EF.Core.Tests.NorthwindModels;


namespace TrackableEntities.Tests.Acceptance.Helpers;

internal static class EntityExtensions
{
    public static EF.Core.Tests.FamilyModels.Client.Category? ToClientEntity(this Category? category)
    {
        if (category == null) return null;
        return new()
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName
        };
    }

    public static EF.Core.Tests.FamilyModels.Client.Product? ToClientEntity(this Product? product)
    {
        if (product == null) return null;
        return new()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            UnitPrice = product.UnitPrice,
            CategoryId = product.CategoryId,
            Category = product.Category.ToClientEntity()
        };
    }

    public static EF.Core.Tests.FamilyModels.Client.CustomerSetting? ToClientEntity(this CustomerSetting? setting)
    {
        if (setting == null) return null;
        return new()
        {
            CustomerId = setting.CustomerId,
            Setting = setting.Setting
        };
    }

    public static EF.Core.Tests.FamilyModels.Client.Customer? ToClientEntity(this Customer? customer)
    {
        if (customer == null) return null;
        return new EF.Core.Tests.FamilyModels.Client.Customer
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            CustomerSetting = customer.CustomerSetting.ToClientEntity()
        };
    }

    public static EF.Core.Tests.FamilyModels.Client.OrderDetail? ToClientEntity(this OrderDetail? detail, EF.Core.Tests.FamilyModels.Client.Order order)
    {
        if (detail == null) return null;
        return new()
        {
            OrderDetailId = detail.OrderDetailId,
            OrderId = detail.OrderId,
            ProductId = detail.ProductId,
            Quantity = detail.Quantity,
            UnitPrice = detail.UnitPrice,
            Order = order
        };
    }

    public static EF.Core.Tests.FamilyModels.Client.Order? ToClientEntity(this Order? order)
    {
        if (order == null) return null;
        var clientOrder = new EF.Core.Tests.FamilyModels.Client.Order
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            Customer = order.Customer.ToClientEntity()
        };
        if (order.OrderDetails != null)
        {
            clientOrder.OrderDetails = new ChangeTrackingCollection<EF.Core.Tests.FamilyModels.Client.OrderDetail>();
            foreach (var detail in order.OrderDetails.Select(d => d.ToClientEntity(clientOrder)))
            {
                if (detail != null)
                    clientOrder.OrderDetails.Add(detail);
            }
        }
        return clientOrder;
    }

    public static EF.Core.Tests.FamilyModels.Client.Order CreateNewOrder(string customerId, int[] productIds)
    {
        return new()
        {
            CustomerId = customerId,
            OrderDate = DateTime.Today, 
            OrderDetails = new ChangeTrackingCollection<EF.Core.Tests.FamilyModels.Client.OrderDetail>
                {
                    new EF.Core.Tests.FamilyModels.Client.OrderDetail { ProductId = productIds[0], Quantity = 5, UnitPrice = 10 },
                    new EF.Core.Tests.FamilyModels.Client.OrderDetail { ProductId = productIds[1], Quantity = 10, UnitPrice = 20 },
                    new EF.Core.Tests.FamilyModels.Client.OrderDetail { ProductId = productIds[2], Quantity = 40, UnitPrice = 40 }
                }
        };            
    }
}
