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
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FLImagingExamplesCSharp
{
	public partial class FormGraphViewContextMenu : Form
	{
		private List<CheckBox> checkBoxes = new List<CheckBox>();
		private List<KeyValuePair<EViewGraphMenuItem, string>> menuItems = new List<KeyValuePair<EViewGraphMenuItem, string>>
		{
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Load, "Load File"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Append, "Append"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Save, "Save"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Close, "Close"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Clear, "Clear"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Copy, "Copy && Paste@Copy to Clipboard"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ClearThenPaste, "Copy && Paste@Paste from Clipboard (Clear then Paste)"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Paste, "Copy && Paste@Paste from Clipboard (Append)"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ToggleLogScale, "Log Scale Mode"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ClearPointAnnotation, "Clear Point Annotations"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ChangeChartType, "Chart Settings@Change Chart Type"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowToolBar, "Show@Show ToolBar"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_Zoom, "Zoom"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ZoomIn, "Point of View@Zoom In Mode"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ZoomOut, "Point of View@Zoom Out Mode"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ZoomFit, "Point of View@Zoom Fit"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ViewSettings, "View Settings.."),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Help, "Help"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ChangeColor, "Change Color"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ChangeColor, "Change Color"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_EditChartName, "Edit Chart Name"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.EditChartName, "Edit Chart Name"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowCrosshair, "Show@Show Crosshair"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowLegend, "Show@Show Legend"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowPointAnnotation, "Show@Show Point Annotations"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MagnetCrosshair, "Show@Magnet Crosshair"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ChangeGraphOrder, "Change Graph Order"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.GetTrendline, "Get Trendline"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ZoomAxisNone, "Point of View@Select Zoom Axis@Both"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ZoomAxisHorz, "Point of View@Select Zoom Axis@Horizontal"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ZoomAxisVert, "Point of View@Select Zoom Axis@Vertical"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.EditAxisLabel, "Edit Axis Label"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SwitchAxis, "Switch Axis"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ViewAndEditExpression, "View && Edit Expression"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.EditExpression, "Edit Expression"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.AddExpression, "Add Graph@Expression"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.AddData, "Add Graph@Data"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.AddDataByClick, "Add Graph@Add Data By Click"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SetAxisTickSpacing, "Set Axis Tick Spacing"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SetAxisTickDecimalPlaces, "Set Axis Tick Decimal Places"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ShowGraph, "Show Graph"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowMultipleGraph, "Show Graph@Show Multiple Graph"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowGraph, "Show Graph"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_RemoveGraph, "Remove Graph"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.RemoveMultipleGraph, "Remove Graph@Remove Multiple Graph"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.RemoveGraph, "Remove Graph"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.RemoveData, "Remove"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.EditData, "Edit"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.Panning, "Point of View@Panning Mode"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.IndicateMinMax, "Indicate Chart Min Max"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SetAxisRange, "Set Range"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SetOpacityOfLegend, "Set Opacity of Legend"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ShowLayers, "Layer@Show Layers"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAllLayers, "Layer@Show Layers@Show All Layers"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.HideAllLayers, "Layer@Show Layers@Hide All Layers"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowLayer_Drawing, "Layer@Show Layers@Layer #"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.LayerProperties, "Layer Properties"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ClearLayers, "Clear Layers"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ClearAllLayers, "Layer@Clear All Layers"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ClearLayer, "Clear Layer"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ClearNamedLayer, "Clear Named Layer"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowNamedLayer, "Show Named Layer"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ThemeLightMode, "Show@Theme@Light Mode"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ThemeDarkMode, "Show@Theme@Dark Mode"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_Synchronization, "Synchronization"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_SyncPointOfView, "Synchronization@Point of View"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SyncViewPointOfView, "Synchronization@Point of View"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_SyncWindow, "Synchronization@Window"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SyncWindow, "Synchronization@Window"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxis_Horz, "Show@Show Axis Components@Horizontal Axis"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxis_Vert, "Show@Show Axis Components@Vertical Axis"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxisLabel_Horz, "Show@Show Axis Components@Horizontal Axis Label"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxisLabel_Vert, "Show@Show Axis Components@Vertical Axis Label"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxisTick_Horz, "Show@Show Axis Components@Horizontal Axis Tick"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxisTick_Vert, "Show@Show Axis Components@Vertical Axis Tick"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxisTickLabel_Horz, "Show@Show Axis Components@Horizontal Axis Tick Labels"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ShowAxisTickLabel_Vert, "Show@Show Axis Components@Vertical Axis Tick Labels"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ChartSettings, "Chart Settings"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ChangeType_BarChart, "Chart Settings@Change Chart Type@Bar"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ChangeType_LineGraph, "Chart Settings@Change Chart Type@Line"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.ChangeType_ScatterChart, "Chart Settings@Change Chart Type@Scatter"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_LineGraphMarkerType, "Line Graph Marker Type"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.LineGraphMarker_ZoomInOnly, "Chart Settings@Line Graph Marker Type@Zoom-in Only"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.LineGraphMarker_Always, "Chart Settings@Line Graph Marker Type@Always"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.LineGraphMarker_Never, "Chart Settings@Line Graph Marker Type@Never"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.LineGraphMarkerSettings, "Chart Settings@Marker Settings.."),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.SetLogBase, "Set Log Base"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_AddGraph, "Add Graph"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_Layer, "Layer"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_CopyAndPaste, "Copy && Paste"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_Show, "Show"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_Theme, "Show@Theme"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_ShowAxisComponents, "Show Axis Components"),
			new KeyValuePair<EViewGraphMenuItem, string>(EViewGraphMenuItem.MenuGroup_SelectZoomAxis, "Select Zoom Axis")
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
			int top = 10; // 체크박스의 초기 Y 좌표 // Initial Y-coordinate of the check box.

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

				top += 20; // 다음 체크박스의 Y 위치 // Y-position of the next check box.
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
				// 그래프 뷰 유효성 체크 // Check the validity of the graph view.
				if(!m_viewGraph.IsAvailable())
					break;

				// 사용 가능한 그래프 뷰 메뉴 // Available Graph View Context Menu 
				var listAvailableMenu = new List<EViewGraphMenuItem> { };

				int i = 0;
				foreach(var checkBox in checkBoxes)
				{
					// 체크 선택된 메뉴 아이템을 추가
					// Add the checked menu item
					if(checkBox.Checked)
						listAvailableMenu.Add((EViewGraphMenuItem)menuItems[i].Key);
					i++;
				}
				// 선택된 메뉴 아이템들을 그래프 뷰의 이용 가능한 메뉴에 적용
				// Apply the selected menu items to the available menu in the graph view
				m_viewGraph.SetAvailableViewGraphContextMenu(listAvailableMenu);



				//////////////////////////////////////////
				// Whether to execute the "Tip" code below
				// 아래 "팁" 코드를 수행할지 여부
				bool bTipCodeExecute = false;

				if(!bTipCodeExecute)
					break;

				// 팁: 아래와 같이 기존 메뉴에서 한두 개의 메뉴만 제외 가능
				// Tip: It is possible to exclude only few menus from the existing menu as shown below
				var listAvailableMenuToRemove = new List<EViewGraphMenuItem>
				{
					EViewGraphMenuItem.IndicateMinMax,
					EViewGraphMenuItem.SetOpacityOfLegend
				};
				m_viewGraph.RemoveAvailableViewGraphContextMenu(listAvailableMenuToRemove);

				// 팁: 아래와 같이 기존 메뉴에서 한두 개의 메뉴만 추가 가능
				// Tip: It is possible to add only few menus from the existing menu as shown below
				var listAvailableMenuToAdd = new List<EViewGraphMenuItem>
				{
					EViewGraphMenuItem.IndicateMinMax,
					EViewGraphMenuItem.SetOpacityOfLegend
				};
				m_viewGraph.AddAvailableViewGraphContextMenu(listAvailableMenuToAdd);
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

			// 그래프 뷰 생성 // Create a graph view.
			CResult res = m_viewGraph.CreateAndFitParent((ulong)pictureBoxView.Handle);

			if(res.IsFail())
				ErrorMessageBox(res, "");
		}
		private void UpdateControls()
		{
			// 그래프 뷰 유효성 체크 // Check the validity of the graph view.
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

				var listAvailableMenu = m_viewGraph.GetAvailableViewGraphContextMenu();

				foreach(var checkBox in checkBoxes)
					checkBox.Checked = false;

				foreach(var checkBox in checkBoxes)
				{
					// 안전한 타입 캐스팅 (Tag에 저장된 EViewGraphMenuItem 가져오기)
					// Safely casts and retrieves the EViewGraphMenuItem stored in Tag.
					if(checkBox.Tag is EViewGraphMenuItem eMenu)
					{
						// m_viewGraph에서 받은 활성화 리스트에 현재 체크박스의 메뉴가 포함되어 있는지 확인
						// Checks whether the menu corresponding to the current check box is included in the active menu list obtained from m_viewGraph.
						checkBox.Checked = listAvailableMenu.Contains(eMenu);
					}
					else
					{
						checkBox.Checked = false;
					}
				}

				if(m_viewGraph.GetAvailableViewGraphContextMenu().Count == (int)EViewGraphMenuItem.Count)
				{
					radioButtonAll.Checked = true;
					radioButtonNone.Checked = false;
				}
				else if(m_viewGraph.GetAvailableViewGraphContextMenu().Count == (int)EViewGraphMenuItem.None)
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
