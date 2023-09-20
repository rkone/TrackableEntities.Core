namespace TrackableEntities.EF.Core.Tests.FamilyModels.Client;

public class PromotionalProduct : Product
{
    private string _giftCode = string.Empty;
    public string PromoCode
    {
        get { return _giftCode; }
        set
        {
            if (value == _giftCode) return;
            _giftCode = value;
            NotifyPropertyChanged();
        }
    }
}