#nullable enable
#if USECLIENTENTITIES
using TrackableEntities.Client.Core;

//Instructions: This file is auto-generated. Find the source in the TrackableEntities.EF.Core.Tests.FamilyModels project under:
//Generated\TrackableEntities.IncrementalGenerator\TrackableEntities.IncrementalGenerator.TrackableEntityGenerator\ClientTrackableEntities.g.cs

namespace TrackableEntities.EF.Core.Tests.FamilyModels.Client;
public partial class ClientBase : EntityBase 
{
    protected partial void OnPropertySet(string propertyName, Type propertyType, object? value);
}
public partial interface IClientBase {}

public partial class Child : ClientBase, IClientBase
{
    public string Name
    { 
        get => _Name;
        set
        {
            if (Equals(_Name, value)) return;
            _Name = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(Name), typeof(string), value);
        }
    }
    private string _Name = string.Empty;
    private ChangeTrackingCollection<Child> _Children = new();
    public ChangeTrackingCollection<Child> Children
    { 
        get => _Children; 
        set
        {
            if (Equals(_Children, value)) return;
            _Children = value;
            NotifyPropertyChanged();
        }
    }
}

public partial class Parent : ClientBase, IClientBase
{
    public string Name
    { 
        get => _Name;
        set
        {
            if (Equals(_Name, value)) return;
            _Name = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(Name), typeof(string), value);
        }
    }
    private string _Name = string.Empty;
    public string Hobby
    { 
        get => _Hobby;
        set
        {
            if (Equals(_Hobby, value)) return;
            _Hobby = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(Hobby), typeof(string), value);
        }
    }
    private string _Hobby = string.Empty;
    private ChangeTrackingCollection<Child> _Children = new();
    public ChangeTrackingCollection<Child> Children
    { 
        get => _Children; 
        set
        {
            if (Equals(_Children, value)) return;
            _Children = value;
            NotifyPropertyChanged();
        }
    }
}

public partial class Area : ClientBase, IClientBase
{
    public int AreaId
    { 
        get => _AreaId;
        set
        {
            if (Equals(_AreaId, value)) return;
            _AreaId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(AreaId), typeof(int), value);
        }
    }
    private int _AreaId;
    public string AreaName
    { 
        get => _AreaName;
        set
        {
            if (Equals(_AreaName, value)) return;
            _AreaName = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(AreaName), typeof(string), value);
        }
    }
    private string _AreaName = string.Empty;
    private ChangeTrackingCollection<Territory> _Territories = new();
    public ChangeTrackingCollection<Territory> Territories
    { 
        get => _Territories; 
        set
        {
            if (Equals(_Territories, value)) return;
            _Territories = value;
            NotifyPropertyChanged();
        }
    }
}

public partial class Category : ClientBase, IClientBase
{
    public int CategoryId
    { 
        get => _CategoryId;
        set
        {
            if (Equals(_CategoryId, value)) return;
            _CategoryId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(CategoryId), typeof(int), value);
        }
    }
    private int _CategoryId;
    public string CategoryName
    { 
        get => _CategoryName;
        set
        {
            if (Equals(_CategoryName, value)) return;
            _CategoryName = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(CategoryName), typeof(string), value);
        }
    }
    private string _CategoryName = string.Empty;
    private ChangeTrackingCollection<Product> _Products = new();
    public ChangeTrackingCollection<Product> Products
    { 
        get => _Products; 
        set
        {
            if (Equals(_Products, value)) return;
            _Products = value;
            NotifyPropertyChanged();
        }
    }
}

public partial class Customer : ClientBase, IClientBase
{
    public string CustomerId
    { 
        get => _CustomerId;
        set
        {
            if (Equals(_CustomerId, value)) return;
            _CustomerId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(CustomerId), typeof(string), value);
        }
    }
    private string _CustomerId = string.Empty;
    public string? CustomerName
    { 
        get => _CustomerName;
        set
        {
            if (Equals(_CustomerName, value)) return;
            _CustomerName = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(CustomerName), typeof(string), value);
        }
    }
    private string? _CustomerName;
    public string? TerritoryId
    { 
        get => _TerritoryId;
        set
        {
            if (Equals(_TerritoryId, value)) return;
            _TerritoryId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(TerritoryId), typeof(string), value);
        }
    }
    private string? _TerritoryId;
    private ChangeTrackingCollection<Territory>? TerritoryChangeTracker { get; set; }
    private Territory? _Territory;
    public Territory? Territory 
    { 
        get => _Territory;
        set
        {
            if (Equals(value, _Territory)) return;
            _Territory = value;
            TerritoryChangeTracker = _Territory == null ? null : new ChangeTrackingCollection<Territory> { _Territory };
            NotifyPropertyChanged();
            OnPropertySet(nameof(Territory), typeof(Territory), value);
        }
    }
    public ICollection<Order>? Orders { get; set; }
    private ChangeTrackingCollection<CustomerSetting>? CustomerSettingChangeTracker { get; set; }
    private CustomerSetting? _CustomerSetting;
    public CustomerSetting? CustomerSetting 
    { 
        get => _CustomerSetting;
        set
        {
            if (Equals(value, _CustomerSetting)) return;
            _CustomerSetting = value;
            CustomerSettingChangeTracker = _CustomerSetting == null ? null : new ChangeTrackingCollection<CustomerSetting> { _CustomerSetting };
            NotifyPropertyChanged();
            OnPropertySet(nameof(CustomerSetting), typeof(CustomerSetting), value);
        }
    }
}

public partial class CustomerSetting : ClientBase, IClientBase
{
    public string CustomerId
    { 
        get => _CustomerId;
        set
        {
            if (Equals(_CustomerId, value)) return;
            _CustomerId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(CustomerId), typeof(string), value);
        }
    }
    private string _CustomerId = string.Empty;
    public string Setting
    { 
        get => _Setting;
        set
        {
            if (Equals(_Setting, value)) return;
            _Setting = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(Setting), typeof(string), value);
        }
    }
    private string _Setting = string.Empty;
    private ChangeTrackingCollection<Customer>? CustomerChangeTracker { get; set; }
    private Customer? _Customer;
    public Customer? Customer 
    { 
        get => _Customer;
        set
        {
            if (Equals(value, _Customer)) return;
            _Customer = value;
            CustomerChangeTracker = _Customer == null ? null : new ChangeTrackingCollection<Customer> { _Customer };
            NotifyPropertyChanged();
            OnPropertySet(nameof(Customer), typeof(Customer), value);
        }
    }
}

public partial class Employee : ClientBase, IClientBase
{
    public int EmployeeId
    { 
        get => _EmployeeId;
        set
        {
            if (Equals(_EmployeeId, value)) return;
            _EmployeeId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(EmployeeId), typeof(int), value);
        }
    }
    private int _EmployeeId;
    public string LastName
    { 
        get => _LastName;
        set
        {
            if (Equals(_LastName, value)) return;
            _LastName = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(LastName), typeof(string), value);
        }
    }
    private string _LastName = string.Empty;
    public string FirstName
    { 
        get => _FirstName;
        set
        {
            if (Equals(_FirstName, value)) return;
            _FirstName = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(FirstName), typeof(string), value);
        }
    }
    private string _FirstName = string.Empty;
    public DateTime? BirthDate
    { 
        get => _BirthDate;
        set
        {
            if (Equals(_BirthDate, value)) return;
            _BirthDate = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(BirthDate), typeof(DateTime), value);
        }
    }
    private DateTime? _BirthDate;
    public DateTime? HireDate
    { 
        get => _HireDate;
        set
        {
            if (Equals(_HireDate, value)) return;
            _HireDate = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(HireDate), typeof(DateTime), value);
        }
    }
    private DateTime? _HireDate;
    public string City
    { 
        get => _City;
        set
        {
            if (Equals(_City, value)) return;
            _City = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(City), typeof(string), value);
        }
    }
    private string _City = string.Empty;
    public string Country
    { 
        get => _Country;
        set
        {
            if (Equals(_Country, value)) return;
            _Country = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(Country), typeof(string), value);
        }
    }
    private string _Country = string.Empty;
    private ChangeTrackingCollection<Territory> _Territories = new();
    public ChangeTrackingCollection<Territory> Territories
    { 
        get => _Territories; 
        set
        {
            value.Parent = this;
            if (Equals(_Territories, value)) return;
            _Territories = value;
            NotifyPropertyChanged();
        }
    }
    public Employee() 
    {
        _Territories.Parent = this;
    }
}

public partial class Order : ClientBase, IClientBase
{
    public int OrderId
    { 
        get => _OrderId;
        set
        {
            if (Equals(_OrderId, value)) return;
            _OrderId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(OrderId), typeof(int), value);
        }
    }
    private int _OrderId;
    public DateTime OrderDate
    { 
        get => _OrderDate;
        set
        {
            if (Equals(_OrderDate, value)) return;
            _OrderDate = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(OrderDate), typeof(DateTime), value);
        }
    }
    private DateTime _OrderDate;
    public string? CustomerId
    { 
        get => _CustomerId;
        set
        {
            if (Equals(_CustomerId, value)) return;
            _CustomerId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(CustomerId), typeof(string), value);
        }
    }
    private string? _CustomerId;
    private ChangeTrackingCollection<Customer>? CustomerChangeTracker { get; set; }
    private Customer? _Customer;
    public Customer? Customer 
    { 
        get => _Customer;
        set
        {
            if (Equals(value, _Customer)) return;
            _Customer = value;
            CustomerChangeTracker = _Customer == null ? null : new ChangeTrackingCollection<Customer> { _Customer };
            NotifyPropertyChanged();
            OnPropertySet(nameof(Customer), typeof(Customer), value);
        }
    }
    private ChangeTrackingCollection<OrderDetail> _OrderDetails = new();
    public ChangeTrackingCollection<OrderDetail> OrderDetails
    { 
        get => _OrderDetails; 
        set
        {
            if (Equals(_OrderDetails, value)) return;
            _OrderDetails = value;
            NotifyPropertyChanged();
        }
    }
}

public partial class OrderDetail : ClientBase, IClientBase
{
    public int OrderDetailId
    { 
        get => _OrderDetailId;
        set
        {
            if (Equals(_OrderDetailId, value)) return;
            _OrderDetailId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(OrderDetailId), typeof(int), value);
        }
    }
    private int _OrderDetailId;
    public int OrderId
    { 
        get => _OrderId;
        set
        {
            if (Equals(_OrderId, value)) return;
            _OrderId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(OrderId), typeof(int), value);
        }
    }
    private int _OrderId;
    private ChangeTrackingCollection<Order>? OrderChangeTracker { get; set; }
    private Order? _Order;
    public Order? Order 
    { 
        get => _Order;
        set
        {
            if (Equals(value, _Order)) return;
            _Order = value;
            OrderChangeTracker = _Order == null ? null : new ChangeTrackingCollection<Order> { _Order };
            NotifyPropertyChanged();
            OnPropertySet(nameof(Order), typeof(Order), value);
        }
    }
    public int ProductId
    { 
        get => _ProductId;
        set
        {
            if (Equals(_ProductId, value)) return;
            _ProductId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(ProductId), typeof(int), value);
        }
    }
    private int _ProductId;
    private ChangeTrackingCollection<Product>? ProductChangeTracker { get; set; }
    private Product? _Product;
    public Product? Product 
    { 
        get => _Product;
        set
        {
            if (Equals(value, _Product)) return;
            _Product = value;
            ProductChangeTracker = _Product == null ? null : new ChangeTrackingCollection<Product> { _Product };
            NotifyPropertyChanged();
            OnPropertySet(nameof(Product), typeof(Product), value);
        }
    }
    public decimal UnitPrice
    { 
        get => _UnitPrice;
        set
        {
            if (Equals(_UnitPrice, value)) return;
            _UnitPrice = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(UnitPrice), typeof(decimal), value);
        }
    }
    private decimal _UnitPrice;
    public double Quantity
    { 
        get => _Quantity;
        set
        {
            if (Equals(_Quantity, value)) return;
            _Quantity = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(Quantity), typeof(double), value);
        }
    }
    private double _Quantity;
}

public partial class Product : ClientBase, IClientBase
{
    public int ProductId
    { 
        get => _ProductId;
        set
        {
            if (Equals(_ProductId, value)) return;
            _ProductId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(ProductId), typeof(int), value);
        }
    }
    private int _ProductId;
    public string? ProductName
    { 
        get => _ProductName;
        set
        {
            if (Equals(_ProductName, value)) return;
            _ProductName = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(ProductName), typeof(string), value);
        }
    }
    private string? _ProductName;
    public decimal UnitPrice
    { 
        get => _UnitPrice;
        set
        {
            if (Equals(_UnitPrice, value)) return;
            _UnitPrice = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(UnitPrice), typeof(decimal), value);
        }
    }
    private decimal _UnitPrice;
    public bool Discontinued
    { 
        get => _Discontinued;
        set
        {
            if (Equals(_Discontinued, value)) return;
            _Discontinued = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(Discontinued), typeof(bool), value);
        }
    }
    private bool _Discontinued;
    public int CategoryId
    { 
        get => _CategoryId;
        set
        {
            if (Equals(_CategoryId, value)) return;
            _CategoryId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(CategoryId), typeof(int), value);
        }
    }
    private int _CategoryId;
    private ChangeTrackingCollection<Category>? CategoryChangeTracker { get; set; }
    private Category? _Category;
    public Category? Category 
    { 
        get => _Category;
        set
        {
            if (Equals(value, _Category)) return;
            _Category = value;
            CategoryChangeTracker = _Category == null ? null : new ChangeTrackingCollection<Category> { _Category };
            NotifyPropertyChanged();
            OnPropertySet(nameof(Category), typeof(Category), value);
        }
    }
    public int? PromoId
    { 
        get => _PromoId;
        set
        {
            if (Equals(_PromoId, value)) return;
            _PromoId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(PromoId), typeof(int), value);
        }
    }
    private int? _PromoId;
    public int? ProductInfoKey1
    { 
        get => _ProductInfoKey1;
        set
        {
            if (Equals(_ProductInfoKey1, value)) return;
            _ProductInfoKey1 = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(ProductInfoKey1), typeof(int), value);
        }
    }
    private int? _ProductInfoKey1;
    public int? ProductInfoKey2
    { 
        get => _ProductInfoKey2;
        set
        {
            if (Equals(_ProductInfoKey2, value)) return;
            _ProductInfoKey2 = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(ProductInfoKey2), typeof(int), value);
        }
    }
    private int? _ProductInfoKey2;
}

public partial class Territory : ClientBase, IClientBase
{
    public string TerritoryId
    { 
        get => _TerritoryId;
        set
        {
            if (Equals(_TerritoryId, value)) return;
            _TerritoryId = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(TerritoryId), typeof(string), value);
        }
    }
    private string _TerritoryId = string.Empty;
    public string TerritoryDescription
    { 
        get => _TerritoryDescription;
        set
        {
            if (Equals(_TerritoryDescription, value)) return;
            _TerritoryDescription = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof(TerritoryDescription), typeof(string), value);
        }
    }
    private string _TerritoryDescription = string.Empty;
    private ChangeTrackingCollection<Employee> _Employees = new();
    public ChangeTrackingCollection<Employee> Employees
    { 
        get => _Employees; 
        set
        {
            value.Parent = this;
            if (Equals(_Employees, value)) return;
            _Employees = value;
            NotifyPropertyChanged();
        }
    }
    private ChangeTrackingCollection<Customer> _Customers = new();
    public ChangeTrackingCollection<Customer> Customers
    { 
        get => _Customers; 
        set
        {
            if (Equals(_Customers, value)) return;
            _Customers = value;
            NotifyPropertyChanged();
        }
    }
    private ChangeTrackingCollection<Area> _Areas = new();
    public ChangeTrackingCollection<Area> Areas
    { 
        get => _Areas; 
        set
        {
            if (Equals(_Areas, value)) return;
            _Areas = value;
            NotifyPropertyChanged();
        }
    }
    public Territory() 
    {
        _Employees.Parent = this;
    }
}

#endif