﻿using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseControlSystem.Helpers.Containers.HorizontalListView
{
    public class HorizontalListView : Grid
    {
        protected readonly ICommand SelectedCommand;
        protected readonly StackLayout ItemsStackLayout;

        public event EventHandler SelectedItemChanged;

        public static StackOrientation ListOrientation { get; set; }

        public HorizontalListView()
        {

            SelectedCommand = new Command<object>(item =>
            {
                var selectable = item as ISelectable;
                if (selectable == null)
                {
                    return;
                }
                SetSelected(selectable);
                SelectedItem = selectable.IsSelected ? selectable : null;
            });

            ItemsStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = this.Padding,
                Spacing = 4,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            Children.Add(ItemsStackLayout);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HorizontalListView), null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static void Execute(ICommand command)
        {
            if (command == null)
            {
                return;
            }

            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }


        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(HorizontalListView), defaultValue: default(IEnumerable<object>), propertyChanged: ItemsSourceChanged);

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsLayout = (HorizontalListView)bindable;
            itemsLayout.SetItems();
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(HorizontalListView), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsView = (HorizontalListView)bindable;
            if (newValue == oldValue)
            {
                return;
            }

            var selectable = newValue as ISelectable;
            itemsView.SetSelectedItem(selectable ?? oldValue as ISelectable);
        }


        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate", typeof(object), typeof(HorizontalListView), default(DataTemplate));
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        protected virtual void SetItems()
        {
            ItemsStackLayout.Children.Clear();

            if (ItemsSource == null)
            {
                return;
            }

            foreach (var item in ItemsSource)
            {
                ItemsStackLayout.Children.Add(GetItemView(item));
            }

            SelectedItem = ItemsSource.OfType<ISelectable>().FirstOrDefault(x => x.IsSelected);
        }

        protected virtual Xamarin.Forms.View GetItemView(object item)
        {
            var content = ItemTemplate.CreateContent();
            var view = content as Xamarin.Forms.View;
            if (view == null)
            {
                return null;
            }

            view.BindingContext = item;

            var gesture = new TapGestureRecognizer
            {
                Command = SelectedCommand,
                CommandParameter = item
            };

            AddGesture(view, gesture);

            return view;
        }

        protected void AddGesture(Xamarin.Forms.View view, TapGestureRecognizer gesture)
        {
            view.GestureRecognizers.Add(gesture);

            var layout = view as Layout<Xamarin.Forms.View>;

            if (layout == null)
            {
                return;
            }

            foreach (var child in layout.Children)
                AddGesture(child, gesture);
        }

        protected virtual void SetSelected(ISelectable selectable)
        {
            selectable.IsSelected = true;
        }

        protected virtual void SetSelectedItem(ISelectable selectedItem)
        {
            var items = ItemsSource;

            foreach (var item in items.OfType<ISelectable>())
            {
                item.IsSelected = selectedItem != null && item == selectedItem && selectedItem.IsSelected;
            }

            var handler = SelectedItemChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

    }

    public interface ISelectable
    {
        bool IsSelected { get; set; }

        ICommand SelectCommand { get; set; }
    }
}
