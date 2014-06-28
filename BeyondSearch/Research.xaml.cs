﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BeyondSearch.Common.FilterFileReader;

namespace BeyondSearch
{
    /// <summary>
    /// Interaction logic for Research.xaml
    /// </summary>
    public partial class Research : Window
    {
        private const string ContainsMatch = "Contains";
        private const string ContainsSansSpaceAndNumberMatch = "Sans Space & Number";
        private const string ExactMatch = "Exact";
        private const string FuzzyContainsMatch = "Fuzzy";
        private readonly KeywordFilter filter = new KeywordFilter();

        public Research()
        {
            InitializeComponent();
            InitializeKeywordList();
            InitializeFilterList();
            DisplaySelectedFilter();
        }

        private void InitializeKeywordList()
        {
            ListBoxKeywords.Items.Add("hotels with pools");
            ListBoxKeywords.Items.Add("hotels in south chicago red light");
            ListBoxKeywords.Items.Add("stores that sell adult toys");
            ListBoxKeywords.Items.Add("adult toys");
            ListBoxKeywords.Items.Add("adult only restaurants");

            ListBoxKeywords.Items.Add("animal shelter dog");
            ListBoxKeywords.Items.Add("animal shelter dogs");
            ListBoxKeywords.Items.Add("animal shelter cat");
            ListBoxKeywords.Items.Add("animal shelter cats");
            ListBoxKeywords.Items.Add("park zebra");

            ListBoxKeywords.Items.Add("park zebras");
            ListBoxKeywords.Items.Add("zoo animal zebra");
            ListBoxKeywords.Items.Add("zoo animal zebras");
            ListBoxKeywords.Items.Add("clothes young girls");
            ListBoxKeywords.Items.Add("young girls");
            
            ListBoxKeywords.Items.Add("zebra");
            ListBoxKeywords.Items.Add("cat");
            ListBoxKeywords.Items.Add("dog");
            ListBoxKeywords.Items.Add("red light");
            ListBoxKeywords.Items.Add("red lights");
        }

        private void InitializeFilterList()
        {
            ListBoxFilters.Items.Add("adult toys");
            ListBoxFilters.Items.Add("zebra");
            ListBoxFilters.Items.Add("young girls");
            ListBoxFilters.Items.Add("red light");
            ListBoxFilters.Items.Add("cat");

            ListBoxFilters.Items.Add("dog");
        }

        private void DisplaySelectedFilter()
        {
            if ( MenuItemExact.IsChecked )
            {
                LabelSelectedFilter.Content = ExactMatch;
                return;
            }
            if ( MenuItemFuzzy.IsChecked )
            {
                LabelSelectedFilter.Content = FuzzyContainsMatch;
                return;
            }
            if ( MenuItemSansSpaceOrNumber.IsChecked )
            {
                LabelSelectedFilter.Content = ContainsSansSpaceAndNumberMatch;
                return;
            }
            if ( MenuItemStrictContains.IsChecked )
            {
                LabelSelectedFilter.Content = ContainsMatch;
                return;
            }
        }

        private void Menu_FileExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddKeyword_Click(object sender, RoutedEventArgs e)
        {
            if ( TextBoxStringToAdd.Text.Length > 0 )
            {
                ListBoxKeywords.Items.Add( " " + TextBoxStringToAdd.Text + " " );
            }
            else
            {
                if (TextBoxKeywordFile.Text.Length > 0)
                {
                    var reader = new FilterTermFileReader();
                    var terms =
                        reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxKeywordFolder.Text, TextBoxKeywordFile.Text))
                            .ToList();
                    ListBoxKeywords.Items.Clear();
                    foreach (var term in terms)
                    {
                        ListBoxKeywords.Items.Add(term);
                    }
                }
            }
        }

        private void ClearKeyword_Click(object sender, RoutedEventArgs e)
        {
            ListBoxKeywords.Items.Clear();

            TextBoxKeywordFolder.Text = string.Empty;
            TextBoxKeywordFile.Text = string.Empty;
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            if ( TextBoxStringToAdd.Text.Length > 0 )
            {
                ListBoxFilters.Items.Add( TextBoxStringToAdd.Text );
            }
            else
            {
                if ( TextBoxFilterFile.Text.Length > 0 )
                {
                    var reader = new FilterTermFileReader();
                    var terms =
                        reader.ReadFilterTerms( System.IO.Path.Combine( TextBoxFilterFolder.Text, TextBoxFilterFile.Text ) )
                            .ToList();
                    ListBoxFilters.Items.Clear();
                    foreach ( var term in terms )
                    {
                        ListBoxFilters.Items.Add(term);
                    }
                }
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            ListBoxFilters.Items.Clear();

            TextBoxFilterFolder.Text = string.Empty;
            TextBoxFilterFile.Text = string.Empty;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();
            ListBoxFilteredKeywords.Items.Clear();

            SetSelectedFilters( sw );
        }

        private void SetSelectedFilters(Stopwatch sw)
        {
            if ( MenuItemList.IsChecked )
            {
                if (MenuItemExact.IsChecked) ExactMatchFilter(sw, 0);
                if (MenuItemFuzzy.IsChecked) FuzzyContainsMatchFilter(sw, 0);
                if (MenuItemSansSpaceOrNumber.IsChecked) ContainsSansSpaceAndNumberMatchFilter(sw, 0);
                if (MenuItemStrictContains.IsChecked) ContainsMatchFilter(sw, 0);
            }
            else
            {
                if (MenuItemExact.IsChecked) ExactMatchFilter(sw, 1);
                if (MenuItemFuzzy.IsChecked) FuzzyContainsMatchFilter(sw, 1);
                if (MenuItemSansSpaceOrNumber.IsChecked) ContainsSansSpaceAndNumberMatchFilter(sw, 1);
                if (MenuItemStrictContains.IsChecked) ContainsMatchFilter(sw, 1);
            }
        }

        private void ContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<string>().ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<string>().ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.Contains( keywords ) : filter.Contains1( keywords );
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ContainsSansSpaceAndNumberMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<string>().ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<string>().ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.ContainsSansSpaceAndNumber( keywords )
                    : filter.ContainsSansSpaceAndNumber1( keywords );
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void ExactMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<string>().ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<string>().ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0 ? filter.Exact( keywords ) : filter.Exact1( keywords );
                sw.Stop();

                ListBoxFilteredKeywords.Items.Clear();
                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private void FuzzyContainsMatchFilter(Stopwatch sw, int oneOrMany)
        {
            List<string> filters = ListBoxFilters.Items.Cast<string>().ToList();
            filter.FillFilterList(filters);

            if (ListBoxKeywords.Items.Count > 0)
            {
                List<string> keywords = ListBoxKeywords.Items.Cast<string>().ToList();

                sw.Start();
                var filteredItems = oneOrMany == 0
                    ? filter.FuzzyContains( keywords )
                    : filter.FuzzyContains1( keywords );
                sw.Stop();

                foreach (var filteredItem in filteredItems)
                {
                    ListBoxFilteredKeywords.Items.Add(filteredItem);
                }

                TextBoxElapsed.Text = sw.ElapsedMilliseconds.ToString();
            }
        }

        private IEnumerable<string> DuplicateList(List<string> list, int noTimesToDuplicate)
        {
            var listToReturn = new List<string>();

            for (int i = 0; i < noTimesToDuplicate; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (i > 0)
                    {
                        listToReturn.Add(list[j] + i.ToString() + " " + j.ToString() + " ");
                    }
                    else
                    {
                        listToReturn.Add(list[j]);
                    }
                }
            }

            return listToReturn;
        }

        private void ButtonKeywordsFile_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog {DefaultExt = ".txt", Filter = "Index documents (.txt)|*.txt|*.cfs|*.*"};
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxKeywordFolder.Text = System.IO.Path.GetDirectoryName( dlg.FileName );
                TextBoxKeywordFile.Text = dlg.SafeFileName;
            }
        }

        private void ButtonFilterFile_Click( object sender, RoutedEventArgs e )
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".txt", Filter = "Index documents (.txt)|*.txt|*.cfs|*.*" };
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;
            }
        }

        private void Menu_FilesFilterClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".txt", Filter = "Index documents (.txt)|*.txt|*.cfs|*.*" };
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxFilterFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxFilterFile.Text = dlg.SafeFileName;
            }

            if (TextBoxFilterFile.Text.Length > 0)
            {
                var reader = new FilterTermFileReader();
                var terms =
                    reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxFilterFolder.Text, TextBoxFilterFile.Text))
                        .ToList();
                ListBoxFilters.Items.Clear();
                foreach (var term in terms)
                {
                    ListBoxFilters.Items.Add(term);
                }
            }
        }

        private void Menu_FilesKeywordsClick( object sender, RoutedEventArgs e )
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".txt", Filter = "Index documents (.txt)|*.txt|*.cfs|*.*" };
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                TextBoxKeywordFolder.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                TextBoxKeywordFile.Text = dlg.SafeFileName;
            }

            if (TextBoxKeywordFile.Text.Length > 0)
            {
                var reader = new FilterTermFileReader();
                var terms =
                    reader.ReadFilterTerms(System.IO.Path.Combine(TextBoxKeywordFolder.Text, TextBoxKeywordFile.Text))
                        .ToList();
                ListBoxKeywords.Items.Clear();
                foreach (var term in terms)
                {
                    ListBoxKeywords.Items.Add(term);
                }
            }
        }

        private void MoveFilteredKeywords_Click( object sender, RoutedEventArgs e )
        {
        }

        private void MarkSelectedFilter_Click( object sender, RoutedEventArgs e )
        {
            var item = e.OriginalSource as MenuItem;
            item.IsChecked = true;

            if (item.Name == "MenuItemStrictContains")
            {
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
            }
            if (item.Name == "MenuItemSansSpaceOrNumber")
            {
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
            }
            if (item.Name == "MenuItemExact")
            {
                MenuItemStrictContains.IsChecked = false;
                MenuItemFuzzy.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
            }
            if (item.Name == "MenuItemFuzzy")
            {
                MenuItemStrictContains.IsChecked = false;
                MenuItemExact.IsChecked = false;
                MenuItemSansSpaceOrNumber.IsChecked = false;
            }

            DisplaySelectedFilter();
        }
    }
}
