using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace BoxRendererTest
{
	public class GraphView : BoxView
	{
		public static readonly BindableProperty DataProperty =
			  BindableProperty.Create(
				  propertyName: "Data",
				  returnType: typeof(IEnumerable<Data>),
				  declaringType: typeof(GraphView),
				  defaultValue: new List<Data>());

		public IEnumerable<Data> Data
		{
			get { return (IEnumerable<Data>)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}

		public static readonly BindableProperty PaddingProperty =
			  BindableProperty.Create(
				  propertyName: "Padding",
				  returnType: typeof(Padding),
				  declaringType: typeof(GraphView),
				  defaultValue: new Padding { Bottom = 5, Left = 5, Right = 10, Top = 10 });

		public Padding Padding
		{
			get { return (Padding)GetValue(PaddingProperty); }
			set { SetValue(DataProperty, value); }
		}
	}

}
