using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GamesCatalog.Components;

public partial class RatingBar : VerticalStackLayout
{
    public RatingBar()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty RatingBarIsVisibleProperty = BindableProperty.Create(
       propertyName: nameof(RatingBarIsVisible), returnType: typeof(bool), declaringType: typeof(RatingBar), defaultValue: false);

    public bool RatingBarIsVisible
    {
        get
        {
            return (bool)GetValue(RatingBarIsVisibleProperty);
        }
        set { SetValue(RatingBarIsVisibleProperty, value); }
    }

    public static readonly BindableProperty RateProperty = BindableProperty.Create(
    propertyName: nameof(Rate), returnType: typeof(int), declaringType: typeof(RatingBar), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay);

    public int Rate
    {
        get
        {
            int _rate = (int)GetValue(RateProperty);
            BuildRatingBar(_rate);
            return (int)GetValue(RateProperty);
        }
        set => SetValue(RateProperty, value);
    }

    [RelayCommand]
    private void Star1() => Rate = 1;

    [RelayCommand]
    private void Star2() => Rate = 2;

    [RelayCommand]
    private void Star3() => Rate = 3;

    [RelayCommand]
    private void Star4() => Rate = 4;

    [RelayCommand]
    private void Star5() => Rate = 5;

    [RelayCommand]
    private void Star6() => Rate = 6;

    [RelayCommand]
    private void Star7() => Rate = 7;

    [RelayCommand]
    private void Star8() => Rate = 8;

    [RelayCommand]
    private void Star9() => Rate = 9;

    [RelayCommand]
    private void Star10() => Rate = 10;

    static readonly Color DefaultColor = Color.FromArgb("#ACACAC");
    static readonly Color HigthRatingColor = Color.FromArgb("#2B9B74");
    static readonly Color MediumRatingColor = Color.FromArgb("#CBB52A");
    static readonly Color LowRatingColor = Color.FromArgb("#9B2B2B");

    protected void BuildRatingBar(int rate)
    {
        switch (rate)
        {
            case 0:
                Star1Color = Star2Color = Star3Color = Star4Color = Star5Color = Star6Color = Star7Color = Star8Color = Star9Color = Star10Color = DefaultColor;
                break;
            case 1:
                Star2Color = Star3Color = Star4Color = Star5Color = Star6Color = Star7Color = Star8Color = Star9Color = Star10Color = DefaultColor;
                Star1Color = LowRatingColor;
                break;
            case 2:
                Star3Color = Star4Color = Star5Color = Star6Color = Star7Color = Star8Color = Star9Color = Star10Color = DefaultColor;
                Star1Color = Star2Color = LowRatingColor;
                break;
            case 3:
                Star4Color = Star5Color = Star6Color = Star7Color = Star8Color = Star9Color = Star10Color = DefaultColor;
                Star1Color = Star2Color = Star3Color = LowRatingColor;
                break;
            case 4:
                Star5Color = Star6Color = Star7Color = Star8Color = Star9Color = Star10Color = DefaultColor;
                Star1Color = Star2Color = Star3Color = Star4Color = LowRatingColor;
                break;
            case 5:
                Star6Color = Star7Color = Star8Color = Star9Color = Star10Color = DefaultColor;
                Star1Color = Star2Color = Star3Color = Star4Color = Star5Color = MediumRatingColor;
                break;
            case 6:
                Star7Color = Star8Color = Star9Color = Star10Color = DefaultColor;
                Star1Color = Star2Color = Star3Color = Star4Color = Star5Color = Star6Color = MediumRatingColor;
                break;
            case 7:
                Star8Color = Star9Color = Star10Color = DefaultColor;
                Star1Color = Star2Color = Star3Color = Star4Color = Star5Color = Star6Color = Star7Color = MediumRatingColor;
                break;
            case 8:
                Star9Color = Star10Color = DefaultColor;
                Star1Color = Star2Color = Star3Color = Star4Color = Star5Color = Star6Color = Star7Color = Star8Color = HigthRatingColor;
                break;
            case 9:
                Star10Color = DefaultColor;
                Star1Color = Star2Color = Star3Color = Star4Color = Star5Color = Star6Color = Star7Color = Star8Color = Star9Color = HigthRatingColor;
                break;
            case 10:
                Star1Color = Star2Color = Star3Color = Star4Color = Star5Color = Star6Color = Star7Color = Star8Color = Star9Color = Star10Color = HigthRatingColor;
                break;

        }
    }

    private Color star1Color = DefaultColor, star2Color = DefaultColor, star3Color = DefaultColor, star4Color = DefaultColor, star5Color = DefaultColor,
        star6Color = DefaultColor, star7Color = DefaultColor, star8Color = DefaultColor, star9Color = DefaultColor, star10Color = DefaultColor;

    public Color Star1Color { get => star1Color; set { star1Color = value; OnPropertyChanged(nameof(Star1Color)); } }

    public Color Star2Color { get => star2Color; set { star2Color = value; OnPropertyChanged(nameof(Star2Color)); } }

    public Color Star3Color { get => star3Color; set { star3Color = value; OnPropertyChanged(nameof(Star3Color)); } }

    public Color Star4Color { get => star4Color; set { star4Color = value; OnPropertyChanged(nameof(Star4Color)); } }

    public Color Star5Color { get => star5Color; set { star5Color = value; OnPropertyChanged(nameof(Star5Color)); } }

    public Color Star6Color { get => star6Color; set { star6Color = value; OnPropertyChanged(nameof(Star6Color)); } }

    public Color Star7Color { get => star7Color; set { star7Color = value; OnPropertyChanged(nameof(Star7Color)); } }

    public Color Star8Color { get => star8Color; set { star8Color = value; OnPropertyChanged(nameof(Star8Color)); } }

    public Color Star9Color { get => star9Color; set { star9Color = value; OnPropertyChanged(nameof(Star9Color)); } }

    public Color Star10Color { get => star10Color; set { star10Color = value; OnPropertyChanged(nameof(Star10Color)); } }

}