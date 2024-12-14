﻿using Avalonia.Controls;

namespace Monaco.DragDrop.Abstractions;
public interface IDragOperation
{
    IList<string> InteractionIds { get; set; }
    Control? AttachedControl { get; }
    object? Payload { get; set; }

    void Attach(Control control);
    void Detach(Control control);
}
