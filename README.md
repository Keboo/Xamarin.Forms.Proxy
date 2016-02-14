# Xamarin.Forms.Proxy

A proxy PCL with access to Xamarin.Forms internal members to allow for easier extension of the library.
It does this by exploiting the InternalsVisibleTo attribute and sets its assembly name to "Xamarin.Forms.Core.UnitTests".

##MultiBinding
This class adds a MultiBinding (similar to WPF's MultiBinding class). It currently only supports one way bindings using either a string format or a value converter.

Usagge:
```XAML
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:xfProxy="clr-namespace:Xamarin.Forms.Proxy;assembly=Xamarin.Forms.Core.UnitTests">
  <Label>
    <Label.Text>
      <xfProxy:MultiBinding StringFormat="Good evening {0}. You are needed in the {1}">
        <Binding Path="User" />
        <Binding Path="Location" />
      </xfProxy:MultiBinding>
    </Label.Text>
  </Label>
</ContentPage>
```
