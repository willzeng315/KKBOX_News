using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KKBOX_News
{
    public class SettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate space
        {
            get;
            set;
        }

        public DataTemplate textblockCheckbox
        {
            get;
            set;
        }

        public DataTemplate textblockListpicker
        {
            get;
            set;
        }

        public DataTemplate textblockLink
        {
            get;
            set;
        }

        public DataTemplate textblockContent
        {
            get;
            set;
        }

        public DataTemplate textblock
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            SettingListItem myItem = item as SettingListItem;
            if (myItem != null)
            {
                switch (myItem.Type)
                {
                    case "space":
                        return space;
                    case "textblockCheckbox":
                        return textblockCheckbox;
                    case "textblockListpicker":
                        return textblockListpicker;
                    case "textblocklink":
                        return textblockLink;
                    case "textblockContent":
                        return textblockContent;
                    case "textblock":
                        return textblock;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
    public class SettingListItem : BindableBase
    {
        public String Title
        {
            set;
            get;
        }

        public String Content
        {
            set;
            get;
        }

        public String Link
        {
            set;
            get;
        }

        public String Type
        {
            set;
            get;
        }

        private Boolean isChecked;
        public Boolean IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                SetProperty(ref isChecked, value, "IsChecked");
            }
        }
        public SettingListItem()
        {
            Title = "";
            Content = "";
            Link = "";
            Type = "";
            IsChecked = false;
        }

    }
}
