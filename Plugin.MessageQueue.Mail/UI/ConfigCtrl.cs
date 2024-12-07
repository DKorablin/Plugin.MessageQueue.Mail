using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue.UI
{
	public partial class ConfigCtrl : UserControl
	{
		private readonly Plugin _plugin;

		private IMessageSourceSettingsItem SelectedItem { get => lvTimers.SelectedItems.Count == 0 ? null : (IMessageSourceSettingsItem)lvTimers.SelectedItems[0].Tag; }
		private String[] EmptySubItems { get => Array.ConvertAll<String, String>(new String[lvTimers.Columns.Count], delegate (String a) { return String.Empty; }); }

		public ConfigCtrl(Plugin plugin)
		{
			this._plugin = plugin;

			InitializeComponent();

			foreach(MessageSourceType type in Enum.GetValues(typeof(MessageSourceType)))
			{
				lvTimers.Groups.Add(type.ToString(), type.ToString());
				tsddlAdd.DropDownItems.Add(type.ToString()).Tag = type;
			}
			this.DataBind();
		}

		private void DataBind()
		{
			this.AddListItem(this._plugin.Settings.Data);
		}

		private ListViewItem CreateListItem(IMessageSourceSettingsItem item)
		{
			return this.CreateListItem(item, this.EmptySubItems);
		}

		private ListViewItem CreateListItem(IMessageSourceSettingsItem item, String[] emptySubItems)
		{
			ListViewItem result = new ListViewItem() { Tag = item };
			result.SubItems.AddRange(emptySubItems);
			result.Group = this.GetGroup(item);
			this.ModifyListItem(result);

			return result;
		}

		private ListViewGroup GetGroup(IMessageSourceSettingsItem item)
		{
			String strType = this._plugin.Settings.Data.GetType(item).ToString();
			return lvTimers.Groups[strType];
		}

		private void ModifyListItem(ListViewItem listItem)
		{
			IMessageSourceSettingsItem settingsItem = (IMessageSourceSettingsItem)listItem.Tag;

			listItem.SubItems[colName.Index].Text = settingsItem.Key == null
				? Constant.NullText
				: settingsItem.Key;

			listItem.ForeColor = settingsItem.IsValid
				? Color.Empty
				: Color.Gray;
		}

		private void AddListItem(IEnumerable<IMessageSourceSettingsItem> proxyItems)
		{
			List<ListViewItem> itemsToAdd = new List<ListViewItem>();
			String[] subItems = this.EmptySubItems;

			foreach(IMessageSourceSettingsItem item in proxyItems)
				itemsToAdd.Add(this.CreateListItem(item, subItems));
			lvTimers.Items.AddRange(itemsToAdd.ToArray());

			/*ColumnHeaderAutoResizeStyle headerAutoResize = itemsToAdd.Count == 0
				? ColumnHeaderAutoResizeStyle.HeaderSize
				: ColumnHeaderAutoResizeStyle.ColumnContent;*/
			lvTimers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private void tsddlAdd_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			MessageSourceType type = (MessageSourceType)e.ClickedItem.Tag;

			IMessageSourceSettingsItem newItem = this._plugin.Settings.Data.NewMapping(type);
			ListViewItem listItem = this.CreateListItem(newItem);
			lvTimers.Items.Add(listItem);
			/*this._plugin.Settings.Data.Add(newItem);
			this._plugin.Settings.SaveSettings();*/
		}

		private void tsbnRemove_Click(Object sender, EventArgs e)
		{
			IMessageSourceSettingsItem item = this.SelectedItem;
			if(item != null)
			{
				String message = String.Format("Are you shure you want to remove mail message source {0}?",
					item.Key);

				if(MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if(this._plugin.Settings.Data.Remove(item))
						this._plugin.Settings.SaveSettings();
					lvTimers.SelectedItems[0].Remove();
				}
			}
		}

		private void tsbnExecute_Click(Object sender, EventArgs e)
		{
			IMessageSourceSettingsItem item = this.SelectedItem;
			throw new NotImplementedException();
			/*if(item != null)
			{
				if(this._runtime == null)
					this._runtime = new RuntimeCollection(this._plugin);

				tsbnExecute.Checked = this._runtime.Find(item) != null;
				if(tsbnExecute.Checked)
					this._runtime.Stop(item);
				else
					this._runtime.Start(item);
				tsbnExecute.Checked = !tsbnExecute.Checked;
			}*/
		}

		private void lvTimers_SelectedIndexChanged(Object sender, EventArgs e)
		{
			IMessageSourceSettingsItem item = this.SelectedItem;
			pgData.SelectedObject = item;
			splitMain.Panel2Collapsed = item == null;
			tsbnRemove.Enabled = tsbnExecute.Enabled = item != null;
		}

		private void pgData_PropertyValueChanged(Object s, PropertyValueChangedEventArgs e)
		{
			ListViewItem listItem = lvTimers.SelectedItems[0];

			IMessageSourceSettingsItem item = (IMessageSourceSettingsItem)listItem.Tag;

			this.ModifyListItem(listItem);
			if(item.IsValid)
			{
				this._plugin.Settings.Data.AddOrUpdate(item);
				this._plugin.Settings.SaveSettings();
			}
		}
	}
}