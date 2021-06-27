using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puyopuyo
{
	public partial class UserControl1 : UserControl
	{
		public UserControl1()
		{
			InitializeComponent();
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if(keyData == Keys.Right || keyData == Keys.Left ||
				keyData == Keys.Up || keyData == Keys.Down)
			{
				return true;
			}

			return base.IsInputKey(keyData);
		}
	}
}
