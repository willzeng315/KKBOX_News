using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public DataTemplate checkUpdateChose
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
                    case "checkUpdateChose":
                        return checkUpdateChose;
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

        public String Type
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
                   App.ViewModel.IsOpenExternalWeb = value;
                   UserSettings.IsOpenExternalWeb = value;
                }
                if (FunctionOfCheck == "OpenAutoUpdate")
                {
                    App.ViewModel.IsAutoUpdate = value;
                    UserSettings.IsOpenAutoUpdate = value;
                }
                isChecked = value;
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
