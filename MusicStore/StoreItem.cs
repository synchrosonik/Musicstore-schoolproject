using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicStore
{
    class StoreItem
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public Button Button { get; set; }

        public Border CreateItem()
        {           
            Border border = new Border
            {
                Margin = new Thickness(2),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Height = 250,
                Width = 300
            };
            StackPanel storeItem = new StackPanel { Margin = new Thickness(20) };
            border.Child = storeItem;

            TextBlock itemText = new TextBlock { Text = Name };
            storeItem.Children.Add(itemText);

            Image itemImage = new Image
            {
                Height = 130,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(15)
            };
            if (File.Exists(@"..\..\Images\" + Image))
            {
                itemImage.Source = new BitmapImage(new Uri(@"\Images\" + Image, UriKind.Relative));
            }
            else
            {
                itemImage.Source = new BitmapImage(new Uri(@"\Images\noimage.jpg", UriKind.Relative));
            }
            storeItem.Children.Add(itemImage);

            Grid priceGrid = new Grid();
            GridLength length = new GridLength(2, GridUnitType.Star);
            priceGrid.ColumnDefinitions.Add(new ColumnDefinition());
            priceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });

            TextBlock itemPrice = new TextBlock
            {
                Text = "$" + Price.ToString(),
                FontSize = 25
            };
            Grid.SetColumn(itemPrice, 0);
            priceGrid.Children.Add(itemPrice);

            Button itemPriceButton = new Button
            {
                Content = "Add to shopping cart",
                Padding = new Thickness(2),
                Tag = Name + "|" + Price,
                BorderThickness = new Thickness(0)
            };
            Button = itemPriceButton;
            Grid.SetColumn(itemPriceButton, 1);
            priceGrid.Children.Add(Button);
            storeItem.Children.Add(priceGrid);

            return border;
        }

        public Grid CreateShoppingCartItem()
        {
            Grid cartItemGrid = new Grid();
            GridLength length = new GridLength(3, GridUnitType.Star);
            cartItemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
            cartItemGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartItemGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock name = new TextBlock
            {
                Text = Name,
                FontSize = 13,
            };
            Grid.SetColumn(name, 0);
            cartItemGrid.Children.Add(name);
            TextBlock price = new TextBlock
            {
                Text = "$" + Price.ToString(),
                FontSize = 13
            };
            Grid.SetColumn(price, 1);
            cartItemGrid.Children.Add(price);

            Button removeButton = new Button
            {
                Content = "Remove",
                Width = 50,
                Height = 20,
                Tag = Name + "|" + Price,
                Margin = new Thickness(2),
                BorderThickness = new Thickness(0)
            };
            Button = removeButton;
            Grid.SetColumn(Button, 2);
            cartItemGrid.Children.Add(Button);

            return cartItemGrid;
        }
    }
}
