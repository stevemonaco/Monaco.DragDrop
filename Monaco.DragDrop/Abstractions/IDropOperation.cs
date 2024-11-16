﻿using Avalonia.Controls;

namespace Monaco.DragDrop.Abstractions;
public interface IDropOperation
{
    IdList InteractionIds { get; set; }
    Control? AttachedControl { get; }

    void Attach(Control control);
    void Detach(Control control);
}
