using System;
using FancyScrollView;

namespace UI
{
    public class RecycleContext : FancyGridViewContext
    {
        public int SelectedIndex = -1;
        public Action<int> OnCellClicked;
    }
}
