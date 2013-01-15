﻿/************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2010-2012 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus edition at http://xceed.com/wpf_toolkit

   Visit http://xceed.com and follow @datagrid on Twitter

  **********************************************************************/

using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Xceed.Wpf.Toolkit
{
  public class MagnifierAdorner : Adorner
  {
    #region Members

    private Magnifier _magnifier;
    private Point _currentMousePosition;

    #endregion

    #region Constructors

    public MagnifierAdorner( UIElement element, Magnifier magnifier )
      : base( element )
    {
      _magnifier = magnifier;
      UpdateViewBox();
      AddVisualChild( _magnifier );

      Loaded += ( s, e ) => InputManager.Current.PostProcessInput += OnProcessInput;
      Unloaded += ( s, e ) => InputManager.Current.PostProcessInput -= OnProcessInput;
    }

    #endregion

    #region Private/Internal methods

    private void OnProcessInput( object sender, ProcessInputEventArgs e )
    {
      Point pt = Mouse.GetPosition( this );

      if( _currentMousePosition == pt )
        return;

      _currentMousePosition = pt;
      UpdateViewBox();
      InvalidateArrange();
    }

    internal void UpdateViewBox()
    {
      var viewBoxLocation = CalculateViewBoxLocation();
      _magnifier.ViewBox = new Rect( viewBoxLocation, _magnifier.ViewBox.Size );
    }

    private Point CalculateViewBoxLocation()
    {
      double offsetX = 0, offsetY = 0;

      Point adorner = Mouse.GetPosition( this );
      Point element = Mouse.GetPosition( AdornedElement );

      offsetX = element.X - adorner.X;
      offsetY = element.Y - adorner.Y;

      //An element will use the offset from its parent (StackPanel, Grid...) to be rendered.
      //When this element is put in a VisualBrush, then the element will draw with that offset applied. 
      //To fix this : we add that parent offset to Magnifier location.
      Vector parentOffsetVector = VisualTreeHelper.GetOffset( _magnifier.Target );
      Point parentOffset = new Point( parentOffsetVector.X, parentOffsetVector.Y );

      double left = _currentMousePosition.X - ( ( _magnifier.ViewBox.Width / 2 ) + offsetX ) + parentOffset.X;
      double top = _currentMousePosition.Y - ( ( _magnifier.ViewBox.Height / 2 ) + offsetY ) + parentOffset.Y;
      return new Point( left, top );
    }

    #endregion

    #region Overrides

    protected override Visual GetVisualChild( int index )
    {
      return _magnifier;
    }

    protected override int VisualChildrenCount
    {
      get
      {
        return 1;
      }
    }

    protected override Size MeasureOverride( Size constraint )
    {
      _magnifier.Measure( constraint );
      return base.MeasureOverride( constraint );
    }

    protected override Size ArrangeOverride( Size finalSize )
    {
      double x = _currentMousePosition.X - ( _magnifier.Width / 2 );
      double y = _currentMousePosition.Y - ( _magnifier.Height / 2 );
      _magnifier.Arrange( new Rect( x, y, _magnifier.Width, _magnifier.Height ) );
      return base.ArrangeOverride( finalSize );
    }

    #endregion
  }
}