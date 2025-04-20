using Grpc.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using ObjCRuntime;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Channels;

using System.Xml.Linq;

<? xml version = "1.0" encoding = "utf-8" ?>
< ContentPage xmlns = "http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns: x = "http://schemas.microsoft.com/winfx/2009/xaml"
             x: Class = "DecentPass.CommandPage"
             Title = "GoPass Commands" >


    < Grid x: Name = "MainLayout" Margin = "20" RowDefinitions = "Auto,Auto,*,Auto" >


        < !--Command entry-- >
        < Entry x: Name = "CommandEntry"
               Placeholder = "Enter gopass command"
               Completed = "OnCommandEntered"
               Grid.Row = "0" />


        < !--Cancel button(initially hidden)-- >
        < Button x: Name = "CancelButton"
                Text = "Cancel Command"
                IsVisible = "False"
                Clicked = "OnCancelCommand"
                Grid.Row = "1" />


        < !--Output area-- >
        < ScrollView x: Name = "OutputScrollView" Grid.Row = "2" >
            < Label x: Name = "OutputLabel"
                   LineBreakMode = "WordWrap"
                   FontFamily = "Consolas"
                   Padding = "10" />
        </ ScrollView >


        < !--Status indicator-- >
        < ActivityIndicator x: Name = "CommandActivity"
                          IsRunning = "False"
                          Grid.Row = "3" />
    </ Grid >
</ ContentPage >