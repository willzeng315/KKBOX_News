using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KKBOX_News
{
    public enum SettingItemTemplate
    {
        TEMPLATE_SPACE,
        TEMPLATE_TXET_CHECK,
        TEMPLATE_TXET_LINK,
        TEMPLATE_TEXT_CONTENT,
        TEMPLATE_TEXT,
    }
    public class SettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Space
        {
            get;
            set;
        }

        public DataTemplate TextblockCheckbox
        {
            get;
            set;
        }

        public DataTemplate TextblockLink
        {
            get;
            set;
        }

        public DataTemplate TextblockContent
        {
            get;
            set;
        }

        public DataTemplate Textblock
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
                    case SettingItemTemplate.TEMPLATE_SPACE:
                        return Space;
                    case SettingItemTemplate.TEMPLATE_TXET_CHECK:
                        return TextblockCheckbox;
                    case SettingItemTemplate.TEMPLATE_TXET_LINK:
                        return TextblockLink;
                    case SettingItemTemplate.TEMPLATE_TEXT_CONTENT:
                        return TextblockContent;
                    case SettingItemTemplate.TEMPLATE_TEXT:
                        return Textblock;
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

        public String PageLink
        {
            set;
            get;
        }

        public String FunctionOfCheck
        {
            set;
            get;
        }

        public SettingItemTemplate Type
        {
            set;
            get;
        }

        private String updateInterval;
        public String UpdateInterval
        {
            get
            {
                return updateInterval;
            }
            set
            {
                SetProperty(ref updateInterval, value, "UpdateInterval");
            }
        }

        private Boolean isAutoUpdateOpen;
        public Boolean IsAutoUpdateOpen
        {
            get
            {
                return isAutoUpdateOpen;
            }
            set
            {
                SetProperty(ref isAutoUpdateOpen, value, "IsAutoUpdateOpen");
            }
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
                if (FunctionOfCheck == "OpenExternalWeb")
                {
                   UserSettings.Instance.IsOpenExternalWeb = value;
                }
                if (FunctionOfCheck == "OpenAutoUpdate")
                {
                    UserSettings.Instance.IsOpenAutoUpdate = value;
                }
                isChecked = value;
            }
        }

        public SettingListItem()
        {
            Title = "";
            Content = "";
            Link = "";
            IsChecked = false;
        }


    }
}
