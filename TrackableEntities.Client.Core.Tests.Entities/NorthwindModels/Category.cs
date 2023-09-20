﻿using Newtonsoft.Json;

namespace TrackableEntities.Client.Core.Tests.Entities.NorthwindModels;

[JsonObject(IsReference = true)]
public class Category : EntityBase
{
    private int _categoryId;
    public int CategoryId
    {
        get { return _categoryId; }
        set
        {
            if (value == _categoryId) return;
            _categoryId = value;
            NotifyPropertyChanged(() => CategoryId);
        }
    }

    private string _categoryName = string.Empty;
    public string CategoryName
    {
        get { return _categoryName; }
        set
        {
            if (value == _categoryName) return;
            _categoryName = value;
            NotifyPropertyChanged(() => CategoryName);
        }
    }

    private ChangeTrackingCollection<Product> _products = new();
    public ChangeTrackingCollection<Product> Products
    {
        get { return _products; }
        set
        {
            if (Equals(value, _products)) return;
            _products = value;
            NotifyPropertyChanged(() => Products);
        }
    }
}
