﻿using System;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace DiagramDesigner.Controls
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            base.DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
        }

        void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DesignerItemViewModelBase designerItem = this.DataContext as DesignerItemViewModelBase;

            if (designerItem != null && designerItem.IsSelected)
            {
                const double minHeight = 10;
                const double minWidth = 10;

                // we only resize DesignerItems
                foreach (DesignerItemViewModelBase item in designerItem.SelectedItems.OfType<DesignerItemViewModelBase>())
                {
                    switch (this.VerticalAlignment)
                    {
                        case System.Windows.VerticalAlignment.Top:
                            {
                                double deltaVertical = Math.Max((double.IsNaN(item.Top) ? 0 : -item.Top), e.VerticalChange);
                                deltaVertical = Math.Min(deltaVertical, item.ItemHeight - minHeight);
                                item.Top += deltaVertical;
                                item.ItemHeight -= deltaVertical;
                                break;
                            }

                        case System.Windows.VerticalAlignment.Bottom:
                            {
                                item.ItemHeight += Math.Max(e.VerticalChange, minHeight - item.ItemHeight);
                                break;
                            }
                    }

                    switch (this.HorizontalAlignment)
                    {
                        case System.Windows.HorizontalAlignment.Left:
                            {
                                double deltaHorizontal = Math.Max((double.IsNaN(item.Left) ? 0 : -item.Left), e.HorizontalChange);
                                deltaHorizontal = Math.Min(deltaHorizontal, item.ItemWidth - minWidth);
                                item.Left += deltaHorizontal;
                                item.ItemWidth -= deltaHorizontal;
                                break;
                            }

                        case System.Windows.HorizontalAlignment.Right:
                            {
                                item.ItemWidth += Math.Max(e.HorizontalChange, minWidth - item.ItemWidth);
                                break;
                            }
                    }

                    // prevent resizing items out of groupitem
                    if (item.Parent is IDiagramViewModel && item.Parent is DesignerItemViewModelBase)
                    {
                        DesignerItemViewModelBase groupItem = (DesignerItemViewModelBase)item.Parent;
                        if (item.Left + item.ItemWidth >= groupItem.ItemWidth) item.Left = groupItem.ItemWidth - item.ItemWidth;
                        if (item.Top + item.ItemHeight >= groupItem.ItemHeight) item.Top = groupItem.ItemHeight - item.ItemHeight;
                    }

                }
                e.Handled = true;
            }
        }
    }
}
