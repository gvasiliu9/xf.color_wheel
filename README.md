
**Xamarin Forms Color Wheel Component**

<a href="https://www.nuget.org/packages/Utmdev.Xf.Switch/" target="_blank"><img src="https://img.shields.io/nuget/v/Utmdev.Xf.Switch?style=for-the-badge"/></a>

**Usage**

Namespace:

	xmlns:utmdev="clr-namespace:Utmdev.Xf.Components;assembly=Utmdev.Xf.ColorWheel"
               
Component:

    <!--Container-->
    <FlexLayout Direction="Column"
                JustifyContent="Center">

        <!--Color-->
        <Label x:Name="wheelColor"
               HorizontalTextAlignment="Center"
               Text="#FFFFFF"
               TextColor="White"
               FontSize="Title"
               FontAttributes="Bold" />

        <!--Color wheel-->
        <utmdev:ColorWheel x:Name="xfColorWheel"
                           FlexLayout.Basis="45%" />

    </FlexLayout>
                 
Handle value changed:  

    // Color changed command
    xfColorWheel.ValueChangedCommand = new Command<string>((color) =>
    {
        wheelColor.TextColor = Color.FromHex(color);
        wheelColor.Text = color.ToUpper();
    });

    

Handle release:   

    // Released command
    xfColorWheel.ReleasedCommand = new Command<string>((color) =>
    {
        wheelColor.TextColor = Color.FromHex(color);
        wheelColor.Text = color.ToUpper();
    });

                    

**Demo**

<img src="https://github.com/utmdev/xf.color_wheel/blob/master/Component/Demo/color_wheel.gif">
