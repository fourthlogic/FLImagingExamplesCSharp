using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FLImagingExamplesCSharp
{
	public partial class FormGraphViewContextMenu : Form
	{
		private List<CheckBox> checkBoxes = new List<CheckBox>();
		private List<KeyValuePair<EAvailableViewGraphContextMenu, string>> menuItems = new List<KeyValuePair<EAvailableViewGraphContextMenu, string>>
		{
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Load, "Load"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Append, "Append"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Save, "Save"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Close, "Close"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Clear, "Clear"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Copy, "Copy"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ClearThenPaste, "Clear The nPaste"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Paste, "Paste"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ClearDisplayedValue, "Clear Displayed Value"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ChangeChartType, "Change Chart Type"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ShowToolBar, "Show Toolbar"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Zoom, "Zoom"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ZoomAxisNone, "Zoom Axis None"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ZoomAxisHorz, "Zoom Axis Horizontal"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ZoomAxisVert, "Zoom Axis Vertical"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Panning, "Panning"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ViewSettings, "View Settings"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.Help, "Help"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ChangeColor, "Change Color"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.EditChartName, "Edit ChartName"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ShowCrosshair, "Show Crosshair"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ShowLegend, "Show Legend"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.MagnetCrosshair, "Magnet Crosshair"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ChangeGraphOrder, "Change Graph Order"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.GetTrendline, "Get Trendline"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.EditAxisLabel, "Edit Axis Label"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.SwitchAxis, "Switch Axis"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.EditExpression, "Edit Expression"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.AddExpression, "Add Expression"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.AddData, "Add Data"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.AddDataByClick, "Add Data By Click"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ShowGraph, "Show Graph"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.RemoveGraph, "Remove Graph"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.RemoveData, "Remove Data"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.EditData, "Edit Data"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.IndicateMinMax, "Indicate Min/Max"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.SetRange, "Set Range"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.SetOpacityOfLegend, "Set Opacity Of Legend"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ShowLayers, "Show layers"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.LayerProperties, "Layer Properties"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ClearLayers, "Clear Layer"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ThemeLightMode, "Theme Light Mode"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.ThemeDarkMode, "Theme Dark Mode"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.SyncView, "Synchronize View"),
			new KeyValuePair<EAvailableViewGraphContextMenu, string>(EAvailableViewGraphContextMenu.SyncWindow, "Synchronize Window")
		};
		public void ErrorMessageBox(CResult cResult, string str)
		{
			string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

			if(str.Length > 1)
				strMessage += str;

			MessageBox.Show(strMessage, "Error");
		}

		private void CreateDynamicCheckboxes()
		{
			int top = 10; // 체크박스의 초기 Y 좌표

			foreach(var menuItem in menuItems)
			{
				CheckBox checkBox = new CheckBox
				{
					Text = menuItem.Value,
					Location = new Point(10, top),
					AutoSize = true,
					Tag = menuItem.Key, // ID로 사용
					Name = $"CheckBox_{menuItem.Key}"
				};

				panelAvailableContextMenu.Controls.Add(checkBox);
				checkBoxes.Add(checkBox);

				top += 20; // 다음 체크박스의 Y 위치
			}
		}

		public FormGraphViewContextMenu()
		{
			InitializeComponent();
			CreateDynamicCheckboxes();
			this.buttonApply.Click += new System.EventHandler(this.ClickButtonApply);

			this.Load += new System.EventHandler(this.FormGraphViewLoad);
			this.CenterToScreen();
		}

		private void radioButtonShow_CheckedChanged(object sender, EventArgs e)
		{
			if(radioButtonShow.Checked)
			{
				// 이용 불가능한 메뉴를 디스플레이 // Display unavailable menu
				m_viewGraph.ShowUnavailableContextMenu(radioButtonShow.Checked);
			}
			else if(radioButtonHide.Checked)
			{
				// 이용 불가능한 메뉴를 숨김 // Hide unavailable menu
				m_viewGraph.ShowUnavailableContextMenu(!radioButtonHide.Checked);
			}
		}
		private void ClickButtonApply(object sender, EventArgs e)
		{
			do
			{
				// 그래프 뷰 유효성 체크
				if(!m_viewGraph.IsAvailable())
					break;

				// 사용 가능한 그래프 뷰 메뉴 // Available Graph View Context Menu 
				EAvailableViewGraphContextMenu eMenu = EAvailableViewGraphContextMenu.None;

				int i = 0;
				foreach(var checkBox in checkBoxes)
				{
					// 체크 선택된 메뉴 아이템을 eMenu 에 OR 연산하여 추가
					// Add the checked menu item to eMenu using OR operation
					if(checkBox.Checked)
						eMenu |= (EAvailableViewGraphContextMenu)menuItems[i].Key;
					i++;
				}
				// 선택된 메뉴 아이템들을 그래프 뷰의 이용 가능한 메뉴에 적용
				// Apply the selected menu items to the available menu in the graph view
				m_viewGraph.SetAvailableViewGraphContextMenu(eMenu);

				// 팁: 아래와 같이 기존 메뉴에서 한두 개의 메뉴만 제외 가능
				// Tip: It is possible to exclude only one or two menus from the existing menu as shown below
				eMenu = m_viewGraph.GetAvailableViewGraphContextMenu();
				eMenu &= ~(EAvailableViewGraphContextMenu.IndicateMinMax | EAvailableViewGraphContextMenu.SetOpacityOfLegend);
			}
			while(false);
		}

		private void FormGraphViewLoad(object sender, EventArgs e)
		{
			m_timer = new Timer();
			m_timer.Tick += new System.EventHandler(this.TimerTick);
			m_timer.Interval = 100;
			m_timer.Start();

			DockGraphViewToThis();
			ClickButtonApply(sender, e);
			InitializeControls();
			UpdateControls();
		}
		private void DockGraphViewToThis()
		{
			m_viewGraph = new CGUIViewGraph();

			// 그래프 뷰 생성
			CResult res = m_viewGraph.CreateAndFitParent((ulong)pictureBoxView.Handle);

			if(res.IsFail())
				ErrorMessageBox(res, "");
		}
		private void UpdateControls()
		{
			// 그래프 뷰 유효성 체크
			buttonApply.Enabled = m_viewGraph.IsAvailable();

		}
		private void InitializeControls()
		{
			if(m_viewGraph.IsAvailable())
			{
				// Check whether the unavailable context menu is displayed
				// 이용 불가능한 컨텍스트 메뉴를 디스플레이하는지 여부를 확인
				if(m_viewGraph.IsUnavailableContextMenuVisible())
				{
					radioButtonShow.Checked = true;
					radioButtonHide.Checked = false;
				}
				else
				{
					radioButtonShow.Checked = false;
					radioButtonHide.Checked = true;
				}

				EAvailableViewGraphContextMenu eAvailableMenu = m_viewGraph.GetAvailableViewGraphContextMenu();

				int i = 0;
				foreach(var checkBox in checkBoxes)
				{
					if((menuItems[i].Key & eAvailableMenu) != EAvailableViewGraphContextMenu.None)
						checkBox.Checked = true;
					else
						checkBox.Checked = false;

					i++;
				}

				if(m_viewGraph.GetAvailableViewGraphContextMenu() == EAvailableViewGraphContextMenu.All)
				{
					radioButtonAll.Checked = true;
					radioButtonNone.Checked = false;
				}
				else if(m_viewGraph.GetAvailableViewGraphContextMenu() == EAvailableViewGraphContextMenu.None)
				{
					radioButtonAll.Checked = false;
					radioButtonNone.Checked = true;
				}
			}


		}
		private void TimerTick(object sender, EventArgs e)
		{
			this.UpdateControls();
		}

		private CGUIViewGraph m_viewGraph;
		private Timer m_timer;

		private void radioButtonAll_CheckedChanged(object sender, EventArgs e)
		{
			if(radioButtonAll.Checked)
			{
				foreach(var checkBox in checkBoxes)
				{
					checkBox.Checked = true;
				}
			}
			else if(radioButtonNone.Checked)
			{
				foreach(var checkBox in checkBoxes)
				{
					checkBox.Checked = false;
				}
			}
		}
	}
}
