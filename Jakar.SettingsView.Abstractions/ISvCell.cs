﻿using System.ComponentModel;



namespace Jakar.SettingsView.Abstractions;


public interface ISvCell : ISvCellTitle, INotifyPropertyChanged
{
    public WidgetType Type      { get; }
    public bool       IsEnabled { get; set; }
    public bool       IsVisible { get; set; }
}
