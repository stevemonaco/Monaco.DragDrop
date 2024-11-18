using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monaco.DragDrop;
public class CollectionDragOperation : DragOperation
{
    public CollectionDragOperation()
    {
        _handledEventsToo = true;
    }
}
