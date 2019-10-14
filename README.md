

**Xamarin Forms Color Wheel Component**

<a href="https://www.nuget.org/packages/Utmdev.Xf.ColorWheel/1.0.0" target="_blank"><img src="https://img.shields.io/nuget/v/Utmdev.Xf.ColorWheel?style=for-the-badge"/></a>

**Usage**

Namespace:

	xmlns:utmdev="clr-namespace:Utmdev.Xf.Components;assembly=Utmdev.Xf.ColorWheel"
	
Enable touch traking for iOS Project, open AppDelegate -> FinishedLaunching() method and add the following line after LoadApplication(new App());:

    var _ = new TouchTracking.Forms.iOS.TouchEffect();
               
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

------------------------------------------------------------------------------------------------------------------------------
<div>
	<img style="margin:0 auto; display:inline; width:250px;" src="https://github.com/utmdev/xf.color_wheel/blob/master/Component/Demo/color_wheel.gif">	
</div>
