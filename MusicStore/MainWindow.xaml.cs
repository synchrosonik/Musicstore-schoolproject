using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grid itemGrid;
        private List<StoreItem> storeItems;
        private List<StoreItem> shoppingCart;
        private List<User> users;
        private Dictionary<string, int> discounts; 
        private Popup loginPopup;
        private StackPanel loginColumn;
        private User loggedInUser;
        private TextBox userName;
        private PasswordBox password;
        private TextBox codeText;       
        private double discount;
        private double sum;
        private SolidColorBrush mainColor;

        public MainWindow()
        {
            InitializeComponent();           
            Start();
            Closing += new CancelEventHandler(OnWindowClosing);
        }

        private void Start()
        {           
            Title = "Music Store";
            Height = 850;
            Width = 1300;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            itemGrid = new Grid();
            storeItems = new List<StoreItem>();
            shoppingCart = new List<StoreItem>();
            users = new List<User>();
            discounts = new Dictionary<string, int>();
            discount = 0;
            mainColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0CCE8"));
            LoadItemsFromTextFiles();

            Grid contentWindow = (Grid)Content;
          
            Grid firstGrid = new Grid { Margin = new Thickness(10) };
            contentWindow.Children.Add(firstGrid);
            firstGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });
            firstGrid.RowDefinitions.Add(new RowDefinition());
           
            Grid header = new Grid();
            header.ColumnDefinitions.Add(new ColumnDefinition());
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
            TextBlock headerText = new TextBlock
            {
                Margin = new Thickness(10),
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Text = "Awsome Music Store"
            };
            header.Children.Add(headerText);

            loginColumn = new StackPanel();

            Button loginButton = new Button
            {
                Margin = new Thickness(10, 38, 10, 10),
                Background = mainColor,
                Content = "Login",
                Width = 200,
                Height = 25,
                BorderThickness = new Thickness(0)
            };
            loginButton.Click += new RoutedEventHandler(LoginButtonClick);
            loginColumn.Children.Add(loginButton);
            Grid.SetColumn(loginColumn, 1);
            header.Children.Add(loginColumn);

            Button shoppingCartButton = new Button
            {
                Margin = new Thickness(10),
                Background = mainColor,
                Content = "Shopping cart",
                Width = 200,
                Height = 25,
                BorderThickness = new Thickness(0)               
            };
            shoppingCartButton.Click += new RoutedEventHandler(ShoppingCartButtonClick);
            Grid.SetColumn(shoppingCartButton, 2);
            header.Children.Add(shoppingCartButton);
            firstGrid.Children.Add(header);

            Grid mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            Grid.SetRow(mainGrid, 1);
            firstGrid.Children.Add(mainGrid);

            ListBox sideMenu = new ListBox
            {
                BorderThickness = new Thickness(0),
                SelectionMode = SelectionMode.Single,
                FontSize = 22,
            };
            sideMenu.Items.Add(new ListBoxItem { Content = "Guitars" });
            sideMenu.Items.Add(new ListBoxItem { Content = "Basses" });
            sideMenu.Items.Add(new ListBoxItem { Content = "Drums" });
            sideMenu.Items.Add(new ListBoxItem { Content = "Keys" });
            sideMenu.Items.Add(new ListBoxItem { Content = "Microphones" });
            sideMenu.Items.Add(new ListBoxItem { Content = "Brass" });
            sideMenu.Items.Add(new ListBoxItem { Content = "String" });
            sideMenu.SelectionChanged += new SelectionChangedEventHandler(SelectedListboxItem);
            mainGrid.Children.Add(sideMenu);

            ScrollViewer itemGridScrollViewer = new ScrollViewer 
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };            
            Grid.SetColumn(itemGridScrollViewer, 1);
            Grid.SetRow(itemGridScrollViewer, 1);
            itemGridScrollViewer.Content = itemGrid;
            mainGrid.Children.Add(itemGridScrollViewer);          
        }

        private void SelectedListboxItem(object sender, SelectionChangedEventArgs e)
        {
            itemGrid.Children.Clear();
            string listBoxItem = ((sender as ListBox).SelectedItem as ListBoxItem).Content.ToString();
            List<StoreItem> itemList = storeItems.Where(item => item.Type == listBoxItem).ToList();
            AddItemsToGrid(itemList);
        }

        public void LoadItemsFromTextFiles()
        {
            string[] storeItemArray = File.ReadAllLines(@"..\..\StoreItems.txt");
            foreach (string line in storeItemArray)
            {
                if (line[0] != '-')
                {
                    string[] splitLine = line.Split('|');
                    StoreItem newItem = new StoreItem
                    {
                        Name = splitLine[0],
                        Price = int.Parse(splitLine[1]),
                        Type = splitLine[2],
                        Image = splitLine[3]
                    };
                    storeItems.Add(newItem);
                }
            }

            string[] discountArray = File.ReadAllLines(@"..\..\Discounts.txt");
            foreach (string line in discountArray)
            {
                if (line[0] != '-')
                {
                    string[] splitLine = line.Split('|');
                    discounts.Add(splitLine[0], int.Parse(splitLine[1]));
                }
            }

            string[] userArray = File.ReadAllLines(@"..\..\Users.txt");
            foreach (string line in userArray)
            {
                if (line[0] != '-')
                {
                    string[] splitLine = line.Split('|');
                    User newUser = new User
                    {
                        FirstName = splitLine[0],
                        LastName = splitLine[1],
                        UserName = splitLine[2],
                        Password = splitLine[3]
                    };
                    users.Add(newUser);
                }
            }

            if (File.Exists(@"C:\Windows\Temp\_shopping_cart.txt"))
            {
                string[] cartArray = File.ReadAllLines(@"C:\Windows\Temp\_shopping_cart.txt");
                foreach (string line in cartArray)
                {
                    try
                    {
                        StoreItem newItem = storeItems.First(item => item.Name == line);
                        shoppingCart.Add(newItem);
                    }
                    catch
                    {

                    }
                }
            }          
        }

        private void StoreShoppingCartInTextFile()
        {
            string filePath;
            string list = "";
            try
            {
                filePath = @"C:\Windows\Temp\" + loggedInUser.UserName + "_shopping_cart.txt";
                
            }
            catch
            {
                filePath = @"C:\Windows\Temp\_shopping_cart.txt";
            }

            foreach (StoreItem item in shoppingCart)
            {
                list += "\n" + item.Name;
            }
            File.Delete(filePath);
            File.AppendAllText(filePath, list);
        }

        private void GetUserShoppingCart()
        {
            shoppingCart.Clear();
            string[] cartArray = File.ReadAllLines(@"C:\Windows\Temp\" + loggedInUser.UserName + "_shopping_cart.txt");
            foreach (string line in cartArray)
            {
                try
                {
                    StoreItem newItem = storeItems.First(item => item.Name == line);
                    shoppingCart.Add(newItem);
                }
                catch
                {

                }
            }
        }

        private void AddItemsToGrid(List<StoreItem> itemList)
        {
            int column = 0;
            int row = 0;

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 300 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 300 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 300 });

            foreach (StoreItem item in itemList)
            {
                Border storeItem = item.CreateItem();
                storeItem.BorderBrush = mainColor;
                item.Button.Background = mainColor;
                item.Button.Click += new RoutedEventHandler(AddToShoppingCartClick);
                Grid.SetColumn(storeItem, column);
                Grid.SetRow(storeItem, row);
                grid.Children.Add(storeItem);
                if (column == 2)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                    column = 0;
                    row++;
                }
                else
                {
                    column++;
                }
            }
            itemGrid.Children.Add(grid);
        }

        private void ShowItemsInShoppingCart()
        {
            if (shoppingCart.Count > 0)
            {
                itemGrid.Children.Clear();
                Grid grid = new Grid { Margin = new Thickness(15, 15, 15, 15) };
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                StackPanel panel = new StackPanel();
                Grid.SetColumn(panel, 1);
                grid.Children.Add(panel);

                foreach (StoreItem item in shoppingCart)
                {
                    Grid newItem = item.CreateShoppingCartItem();
                    item.Button.Background = mainColor;
                    item.Button.Click += new RoutedEventHandler(RemoveFromShoppingCartClick);
                    panel.Children.Add(newItem);
                }

                sum = GetSumFromShoppingCart();

                if (discount == 0)
                {
                    TextBlock total = new TextBlock
                    {
                        Text = "Total: $" + Math.Round(sum),
                        FontSize = 15,
                        FontWeight = FontWeights.Bold,
                    };
                    panel.Children.Add(total);
                }
                else
                {
                    TextBlock discountText = new TextBlock
                    {
                        Text = "Discount: " + discount + "%",
                        FontSize = 13
                    };
                    panel.Children.Add(discountText);

                    TextBlock total = new TextBlock
                    {
                        Text = "Total: $" + Math.Round(sum),
                        FontSize = 15,
                        FontWeight = FontWeights.Bold,
                    };
                    panel.Children.Add(total);
                }

                codeText = new TextBox
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 200,
                    Height = 30,
                    Padding = new Thickness(10, 4, 10, 3),
                    Text = "Discount Code",
                    Margin = new Thickness(10)
                };
                panel.Children.Add(codeText);

                Button codeButton = new Button
                {
                    Content = "Add discount code",
                    Width = 200,
                    Height = 30,
                    Background = mainColor,
                    BorderThickness = new Thickness(0)
                };
                codeButton.Click += new RoutedEventHandler(DiscountButtonClick);
                panel.Children.Add(codeButton);

                Button checkout = new Button
                {
                    Content = "Checkout",
                    Width = 200,
                    Height = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = mainColor,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(10)
                };
                checkout.Click += new RoutedEventHandler(CheckoutButtonClick);
                panel.Children.Add(checkout);

                itemGrid.Children.Add(grid);
            }
            else
            {
                itemGrid.Children.Clear();
                MessageBox.Show("Your shopping cart is empty");
            }
            
        }

        private void ShoppingCartButtonClick(object sender, RoutedEventArgs e)
        {
            ShowItemsInShoppingCart();
        }

        private void AddToShoppingCartClick(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();
            string[] itemInfo = tag.Split('|');
            shoppingCart.Add(storeItems.First(item => item.Name == itemInfo[0] && item.Price == int.Parse(itemInfo[1])));
        }

        private void RemoveFromShoppingCartClick(object sender, RoutedEventArgs e)
        {        
            string tag = ((Button)sender).Tag.ToString();
            string[] itemInfo = tag.Split('|');
            shoppingCart.Remove(shoppingCart.First(item => item.Name == itemInfo[0] && item.Price == int.Parse(itemInfo[1])));
            ShowItemsInShoppingCart();             
        }
        
        private void CheckoutButtonClick(object sender, RoutedEventArgs e)
        {
            double sum = GetSumFromShoppingCart();
            string order = "Order complete!\n\n\n";
            int cost = shoppingCart.Select(i => i.Price).Sum();

            foreach (StoreItem item in shoppingCart)
            {
                order += item.Name + " - $" + item.Price + "\n";
            }
            if (discount == 0)
            {
                order += "\nTotal cost: $" + Math.Round(sum) + "\n\n\n Thank you!";
            }
            else
            {
                order += "\nCost: $" + cost + " - " + discount + "% discount\nTotal cost: $" + Math.Round(sum) + "\n\n\n Thank you!";
            }
            MessageBox.Show(order);
            discount = 0;
            itemGrid.Children.Clear();
            shoppingCart.Clear();
        }

        private void DiscountButtonClick(object sender, RoutedEventArgs e)
        {
            if (discount == 0)
            {
                foreach (KeyValuePair<string, int> discountCode in discounts)
                {
                    if (discountCode.Key == codeText.Text)
                    {
                        discount = discountCode.Value;
                        ShowItemsInShoppingCart();
                    }
                }
                if (discount == 0)
                {
                    MessageBox.Show("The discount code you're trying to use is invalid");
                } 
            }
            else
            {
                MessageBox.Show("You've already applied a discount code");
            }
        }

        
        private double GetSumFromShoppingCart()
        {
            double x = 100 - discount;
            double y = x / 100;
            double newSum = y * shoppingCart.Select(i => i.Price).Sum();
            return newSum;
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            loginPopup = new Popup 
            {
                Width = 500,
                Height = 200,
                PlacementTarget = itemGrid,
                Placement = PlacementMode.Center,
                VerticalOffset = -200,
                AllowsTransparency = true
            };

            if (!loginPopup.IsOpen)
            {                             
                Border loginBorder = new Border
                {
                    BorderBrush = mainColor,
                    BorderThickness = new Thickness(2),
                    Background = Brushes.White
                };

                StackPanel loginPanel = new StackPanel
                {
                    Margin = new Thickness(10),
                };
                loginPopup.Child = loginBorder;
                loginBorder.Child = loginPanel;

                TextBlock userNameText = new TextBlock 
                { 
                    Text = "Username",
                    FontWeight = FontWeights.Bold,
                    FontSize = 13,
                    TextAlignment = TextAlignment.Center
                };
                loginPanel.Children.Add(userNameText);

                userName = new TextBox 
                { 
                    Width = 450,
                    Height = 25,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                loginPanel.Children.Add(userName);

                TextBlock passwordText = new TextBlock 
                { 
                    Text = "Password",
                    FontWeight = FontWeights.Bold,
                    FontSize = 13,
                    TextAlignment = TextAlignment.Center
                };
                loginPanel.Children.Add(passwordText);

                password = new PasswordBox 
                { 
                    Width = 450,
                    Height = 25,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                loginPanel.Children.Add(password);

                Button loginButton = new Button 
                { 
                    Content = "Login",
                    Width = 450,
                    Height = 25,
                    Background = mainColor,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                loginButton.Click += new RoutedEventHandler(PopupLoginButtonClick);
                loginPanel.Children.Add(loginButton);

                Button cancelButton = new Button
                {
                    Content = "Cancel",
                    Width = 450,
                    Height = 25,
                    Background = mainColor
                };
                cancelButton.Click += new RoutedEventHandler(PopupCancelButtonClick);
                loginPanel.Children.Add(cancelButton);

                loginPopup.IsOpen = true;
            }
        }

        private void PopupCancelButtonClick(object sender, RoutedEventArgs e)
        {
            loginPopup.IsOpen = false;
        }

        private void PopupLoginButtonClick(object sender, RoutedEventArgs e)
        {
            loginPopup.IsOpen = false;
            itemGrid.Children.Clear();

            try
            {
                foreach (User user in users)
                {
             
                    loggedInUser = users.First(u => u.UserName == userName.Text && u.Password == password.Password);
                    loginColumn.Children.Clear();
                    TextBlock loggedIn = new TextBlock
                    {
                        Text = "Welcome " + loggedInUser.FirstName + " " + loggedInUser.LastName,
                        FontSize = 16,
                        Margin = new Thickness(25, 0, 0, 17)
                    };
                    loginColumn.Children.Add(loggedIn);

                    Button logOutButton = new Button
                    {
                        Content = "Log out",
                        Background = mainColor,
                        Width = 200,
                        Height = 25,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        BorderThickness = new Thickness(0)
                    };
                    logOutButton.Click += new RoutedEventHandler(LogOutButtonClick);
                    loginColumn.Children.Add(logOutButton);
                    GetUserShoppingCart();                                      
                }
            }
            catch
            {
                MessageBox.Show("Username and password doesn't match");
            }

        }

        private void LogOutButtonClick(object sender, RoutedEventArgs e)
        {
            StoreShoppingCartInTextFile();
            loginColumn.Children.Clear();
            itemGrid.Children.Clear();
            shoppingCart.Clear();
            discount = 0;
            Start();
        }
        
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            StoreShoppingCartInTextFile();
        }
    }
}
