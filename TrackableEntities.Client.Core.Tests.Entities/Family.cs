﻿using TrackableEntities.Client.Core;

namespace TrackableEntities.EF.Core.Tests.FamilyModels.Client;

public class Family : ClientBase
{
    private Parent? _mother;
    private Parent? _father;
    private Child? _child;

    public Parent? Mother
    {
        get { return _mother; }
        set
        {
            if (value == _mother) return;
            _mother = value;
            MotherChangeTracker = _mother == null ? null : new ChangeTrackingCollection<Parent>(_mother);
            NotifyPropertyChanged(() => Mother);
        }
    }
    private ChangeTrackingCollection<Parent>? MotherChangeTracker { get; set; }


    public Parent? Father
    {
        get { return _father; }
        set
        {
            if (value == _father) return;
            _father = value;
            FatherChangeTracker = _father == null ? null : [_father];
            NotifyPropertyChanged(() => Father);

        }
    }
    private ChangeTrackingCollection<Parent>? FatherChangeTracker { get; set; }


    public Child? Child
    {
        get { return _child; }
        set
        {
            if (value == _child) return;
            _child = value;
            ChildChangeTracker = _child == null ? null : [_child];
            NotifyPropertyChanged(() => Child);
        }
    }
    private ChangeTrackingCollection<Child>? ChildChangeTracker { get; set; }
}
