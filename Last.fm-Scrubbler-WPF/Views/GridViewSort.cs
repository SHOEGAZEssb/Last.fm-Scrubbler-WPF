using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;

namespace Last.fm_Scrubbler_WPF.Views
{
  /// <summary>
  /// Helper class to enable sorting on a WPF GridView.
  /// </summary>
  public class GridViewSort
  {
    #region Public attached properties

    /// <summary>
    /// Gets the value of the <see cref="CommandProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject to get the property from.</param>
    /// <returns><see cref="CommandProperty"/> value of the given <paramref name="obj"/>.</returns>
    public static ICommand GetCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(CommandProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="CommandProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject whose property to set.</param>
    /// <param name="value">Value to set.</param>
    public static void SetCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(CommandProperty, value);
    }

    /// <summary>
    /// DependencyProperty for the command to execute when sorting.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(GridViewSort),
            new UIPropertyMetadata(null,
                (o, e) =>
                {
                  if (o is ItemsControl listView)
                  {
                    if (!GetAutoSort(listView)) // Don't change click handler if AutoSort enabled
                    {
                      if (e.OldValue != null && e.NewValue == null)
                        listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                      else if (e.OldValue == null && e.NewValue != null)
                        listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                    }
                  }
                }
            )
        );

    /// <summary>
    /// Gets the value of the <see cref="AutoSortProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject to get the property from.</param>
    /// <returns><see cref="AutoSortProperty"/> value of the given <paramref name="obj"/>.</returns>
    public static bool GetAutoSort(DependencyObject obj)
    {
      return (bool)obj.GetValue(AutoSortProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="AutoSortProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject whose property to set.</param>
    /// <param name="value">Value to set.</param>
    public static void SetAutoSort(DependencyObject obj, bool value)
    {
      obj.SetValue(AutoSortProperty, value);
    }

    /// <summary>
    /// Dependency property for the auto sorting.
    /// </summary>
    public static readonly DependencyProperty AutoSortProperty = DependencyProperty.RegisterAttached("AutoSort", typeof(bool), typeof(GridViewSort),
      new UIPropertyMetadata(false,
                (o, e) =>
                {
                  if (o is ListView listView)
                  {
                    if (GetCommand(listView) == null) // Don't change click handler if a command is set
                    {
                      bool oldValue = (bool)e.OldValue;
                      bool newValue = (bool)e.NewValue;
                      if (oldValue && !newValue)
                        listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                      else if (!oldValue && newValue)
                        listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                    }
                  }
                }
            )
        );

    /// <summary>
    /// Gets the value of the <see cref="PropertyNameProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject to get the property from.</param>
    /// <returns><see cref="PropertyNameProperty"/> value of the given <paramref name="obj"/>.</returns>
    public static string GetPropertyName(DependencyObject obj)
    {
      return (string)obj.GetValue(PropertyNameProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="PropertyNameProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject whose property to set.</param>
    /// <param name="value">Value to set.</param>
    public static void SetPropertyName(DependencyObject obj, string value)
    {
      obj.SetValue(PropertyNameProperty, value);
    }

    /// <summary>
    /// DependencyProperty for name of the property to sort after.
    /// </summary>
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(GridViewSort), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the value of the <see cref="ShowSortGlyphProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject to get the property from.</param>
    /// <returns><see cref="ShowSortGlyphProperty"/> value of the given <paramref name="obj"/>.</returns>
    public static bool GetShowSortGlyph(DependencyObject obj)
    {
      return (bool)obj.GetValue(ShowSortGlyphProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="ShowSortGlyphProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject whose property to set.</param>
    /// <param name="value">Value to set.</param>
    public static void SetShowSortGlyph(DependencyObject obj, bool value)
    {
      obj.SetValue(ShowSortGlyphProperty, value);
    }

    /// <summary>
    /// DependencyProperty for whether the sort glyph should be visible or not.
    /// </summary>
    public static readonly DependencyProperty ShowSortGlyphProperty = DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewSort), new UIPropertyMetadata(true));

    /// <summary>
    /// Gets the value of the <see cref="SortGlyphAscendingProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject to get the property from.</param>
    /// <returns><see cref="SortGlyphAscendingProperty"/> value of the given <paramref name="obj"/>.</returns>
    public static ImageSource GetSortGlyphAscending(DependencyObject obj)
    {
      return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="SortGlyphAscendingProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject whose property to set.</param>
    /// <param name="value">Value to set.</param>
    public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
    {
      obj.SetValue(SortGlyphAscendingProperty, value);
    }

    /// <summary>
    /// DependencyProperty for the ascending sort glyph image.
    /// </summary>
    public static readonly DependencyProperty SortGlyphAscendingProperty = DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewSort),
                                                                                                               new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the value of the <see cref="SortGlyphDescendingProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject to get the property from.</param>
    /// <returns><see cref="SortGlyphDescendingProperty"/> value of the given <paramref name="obj"/>.</returns>
    public static ImageSource GetSortGlyphDescending(DependencyObject obj)
    {
      return (ImageSource)obj.GetValue(SortGlyphDescendingProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="SortGlyphDescendingProperty"/>.
    /// </summary>
    /// <param name="obj">DependencyObject whose property to set.</param>
    /// <param name="value">Value to set.</param>
    public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
    {
      obj.SetValue(SortGlyphDescendingProperty, value);
    }

    /// <summary>
    /// DependencyProperty for the descending sort glyph image.
    /// </summary>
    public static readonly DependencyProperty SortGlyphDescendingProperty = DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewSort), 
                                                                                                                new UIPropertyMetadata(null));

    #endregion

    #region Private attached properties

    private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj)
    {
      return (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);
    }

    private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value)
    {
      obj.SetValue(SortedColumnHeaderProperty, value);
    }

    // Using a DependencyProperty as the backing store for SortedColumn.  This enables animation, styling, binding, etc...
    private static readonly DependencyProperty SortedColumnHeaderProperty =
        DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSort), new UIPropertyMetadata(null));

    #endregion

    #region Column header click event handler

    private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
    {
      if (e.OriginalSource is GridViewColumnHeader headerClicked && headerClicked.Column != null)
      {
        string propertyName = GetPropertyName(headerClicked.Column);
        if (!string.IsNullOrEmpty(propertyName))
        {
          ListView listView = GetAncestor<ListView>(headerClicked);
          if (listView != null)
          {
            ICommand command = GetCommand(listView);
            if (command != null)
            {
              if (command.CanExecute(propertyName))
                command.Execute(propertyName);
            }
            else if (GetAutoSort(listView))
            {
              ApplySort(listView.Items, propertyName, listView, headerClicked);
            }
          }
        }
      }
    }

    #endregion

    #region Helper methods

    /// <summary>
    /// Gets the visual tree ancestor of the given Type from
    /// the given <paramref name="reference"/>.
    /// </summary>
    /// <typeparam name="T">Ancestor type</typeparam>
    /// <param name="reference">Object to get ancestor from.</param>
    /// <returns></returns>
    public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
    {
      DependencyObject parent = VisualTreeHelper.GetParent(reference);
      while (!(parent is T))
      {
        parent = VisualTreeHelper.GetParent(parent);
      }
      if (parent != null)
        return (T)parent;
      else
        return null;
    }

    /// <summary>
    /// Does the actual sorting.
    /// </summary>
    /// <param name="view">Sortable view.</param>
    /// <param name="propertyName">Property to sort after.</param>
    /// <param name="listView">Actual view.</param>
    /// <param name="sortedColumnHeader">Header of the column to sort.</param>
    public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
    {
      ListSortDirection direction = ListSortDirection.Ascending;
      if (view.SortDescriptions.Count > 0)
      {
        SortDescription currentSort = view.SortDescriptions[0];
        if (currentSort.PropertyName == propertyName)
        {
          if (currentSort.Direction == ListSortDirection.Ascending)
            direction = ListSortDirection.Descending;
          else
            direction = ListSortDirection.Ascending;
        }
        view.SortDescriptions.Clear();

        GridViewColumnHeader currentSortedColumnHeader = GetSortedColumnHeader(listView);
        if (currentSortedColumnHeader != null)
          RemoveSortGlyph(currentSortedColumnHeader);
      }

      if (!string.IsNullOrEmpty(propertyName))
      {
        view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        if (GetShowSortGlyph(listView))
          AddSortGlyph(sortedColumnHeader, direction,
              direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
        SetSortedColumnHeader(listView, sortedColumnHeader);
      }
    }

    /// <summary>
    /// Adds a sort glyph to the header.
    /// </summary>
    /// <param name="columnHeader">Header to add the glyph to.</param>
    /// <param name="direction">Direction of the glyph.</param>
    /// <param name="sortGlyph">Image representing the glyph.</param>
    private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
    {
      AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
      adornerLayer.Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
    }

    private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
    {
      AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
      Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);
      if (adorners != null)
      {
        foreach (Adorner adorner in adorners)
        {
          if (adorner is SortGlyphAdorner)
            adornerLayer.Remove(adorner);
        }
      }
    }

    #endregion

    #region SortGlyphAdorner nested class

    private class SortGlyphAdorner : Adorner
    {
      private GridViewColumnHeader _columnHeader;
      private ListSortDirection _direction;
      private ImageSource _sortGlyph;

      public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
          : base(columnHeader)
      {
        _columnHeader = columnHeader;
        _direction = direction;
        _sortGlyph = sortGlyph;
      }

      private Geometry GetDefaultGlyph()
      {
        double x1 = _columnHeader.ActualWidth - 13;
        double x2 = x1 + 10;
        double x3 = x1 + 5;
        double y1 = _columnHeader.ActualHeight / 2 - 3;
        double y2 = y1 + 5;

        if (_direction == ListSortDirection.Ascending)
        {
          double tmp = y1;
          y1 = y2;
          y2 = tmp;
        }

        PathSegmentCollection pathSegmentCollection = new PathSegmentCollection
        {
          new LineSegment(new Point(x2, y1), true),
          new LineSegment(new Point(x3, y2), true)
        };

        PathFigure pathFigure = new PathFigure(new Point(x1, y1), pathSegmentCollection, true);

        PathFigureCollection pathFigureCollection = new PathFigureCollection
        {
          pathFigure
        };

        PathGeometry pathGeometry = new PathGeometry(pathFigureCollection);
        return pathGeometry;
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
        base.OnRender(drawingContext);

        if (_sortGlyph != null)
        {
          double x = _columnHeader.ActualWidth - 13;
          double y = _columnHeader.ActualHeight / 2 - 5;
          Rect rect = new Rect(x, y, 10, 10);
          drawingContext.DrawImage(_sortGlyph, rect);
        }
        else
          drawingContext.DrawGeometry(Brushes.LightGray, new Pen(Brushes.Gray, 1.0), GetDefaultGlyph());
      }
    }

    #endregion
  }
}