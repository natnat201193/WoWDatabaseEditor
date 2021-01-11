﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Prism.Mvvm;
using WDE.SmartScriptEditor.Data;

namespace WDE.SmartScriptEditor.Editor.ViewModels
{
    public class SmartSelectViewModel : BindableBase
    {
        private readonly ObservableCollection<SmartItem> allItems = new();

        private readonly CollectionViewSource items;
        private readonly Func<SmartGenericJsonData, bool> predicate;
        private string searchBox;
        private SmartItem selectedItem;

        public SmartSelectViewModel(string file,
            SmartType type,
            Func<SmartGenericJsonData, bool> predicate,
            ISmartDataManager smartDataManager)
        {
            this.predicate = predicate;
            string group = null;
            foreach (var line in File.ReadLines("SmartData/" + file))
                if (line.IndexOf(" ", StringComparison.Ordinal) == 0)
                {
                    if (!smartDataManager.Contains(type, line.Trim()))
                        continue;

                    var i = new SmartItem();
                    var data = smartDataManager.GetDataByName(type, line.Trim());

                    i.Group = group;
                    i.Name = data.NameReadable;
                    i.Id = data.Id;
                    i.Help = data.Help;
                    i.Deprecated = data.Deprecated;
                    i.Data = data;

                    allItems.Add(i);
                }
                else
                    @group = line;

            items = new CollectionViewSource();
            items.Source = allItems;
            items.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            items.Filter += ItemsOnFilter;

            if (items.View.MoveCurrentToFirst()) SelectedItem = items.View.CurrentItem as SmartItem;
        }

        public ICollectionView AllItems => items.View;

        public string SearchBox
        {
            get => searchBox;
            set
            {
                SetProperty(ref searchBox, value);
                items.View.Refresh();
            }
        }

        public SmartItem SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        private void ItemsOnFilter(object sender, FilterEventArgs filterEventArgs)
        {
            var item = filterEventArgs.Item as SmartItem;

            if (predicate != null && !predicate(item.Data))
                filterEventArgs.Accepted = false;
            else
                filterEventArgs.Accepted =
                    string.IsNullOrEmpty(SearchBox) || item.Name.ToLower().Contains(SearchBox.ToLower());
        }
    }

    public class SmartItem
    {
        public SmartGenericJsonData Data;
        public string Name { get; set; }
        public bool Deprecated { get; set; }
        public string Help { get; set; }
        public int Id { get; set; }
        public string Group { get; set; }
    }
}