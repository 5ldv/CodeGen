using CodeGen;
using CodeGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator
{
    public partial class frmCodeGen : Form
    {
        public frmCodeGen()
        {
            InitializeComponent();
        }
        private void _FillDatabaseCombobox()
        {
            List<string> Databases = clsDatabase.GetAllDatabases();

            foreach(string Database in Databases)
            {
                cbDatabases.Items.Add(Database);
            }
        }
        private void _FillTablesListView(string Database)
        {
            lvTables.Items.Clear();

            List<string> Tables = clsDatabase.GetTablesOfDatabase(Database);

            foreach(string Table in Tables)
                lvTables.Items.Add(Table);
        }
        private void _FillColumnsListView(string Database, string Table)
        {

            List<clsColumn> Columns = clsDatabase.GetColumnsOfTable(Database, Table);

            listView1.Items.Clear();

            foreach(clsColumn column in Columns)
            {
                ListViewItem listViewItem = new ListViewItem(column.ColumnName);

                listViewItem.SubItems.Add(column.ColumnDataType);
                listViewItem.SubItems.Add(column.AllowNull ? "Yes" : "");

                if(column.IsPrimaryKey)
                {
                    listViewItem.ImageIndex = 1;
                }
                else
                {
                    listViewItem.ImageIndex = 0;
                }

                listView1.Items.Add(listViewItem);
            }
        }
        private void CodeGen_Load(object sender, EventArgs e)
        {
            _FillDatabaseCombobox();
        }
        private void cbDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvTables.Columns[0].Text = "Choose Table:";
            string SelectedDatbase = cbDatabases.SelectedItem.ToString();
            _FillTablesListView(SelectedDatbase);
        }
        private void lvTables_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(lvTables.SelectedItems.Count > 0)
            {
                string SelectedTable = lvTables.SelectedItems[0].Text;
                _FillColumnsListView(cbDatabases.SelectedItem.ToString(), SelectedTable);
            }

        }
        private void btnGenerateDataAccessLayer_Click(object sender, EventArgs e)
        {
            if(cbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database before proceeding.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDatabases.DroppedDown = true;
                return;
            }

            if(lvTables.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a table before proceeding.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            string SelectedDatabase = cbDatabases.SelectedItem.ToString();
            string SelectedTable = lvTables.SelectedItems[0].Text;



            List<clsColumn> TableColumns = clsDatabase.GetColumnsOfTable(SelectedDatabase, SelectedTable);

            clsDataAccessLayerGenerator DALGenerator = new clsDataAccessLayerGenerator(TableColumns, SelectedTable, SelectedDatabase);
            rtbCodeResult.Text = DALGenerator.GenerateClass().ToString();
        }
        private void btnGenerateDataAccessSettings_Click(object sender, EventArgs e)
        {
            if(cbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database before proceeding.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDatabases.DroppedDown = true;
                return;
            }

            string SelectedDatabase = cbDatabases.SelectedItem.ToString();
            string DataAccessSettingsClass = clsDataAccessLayerGenerator.GenerateDataAccessSettingsClass(SelectedDatabase).ToString();
            rtbCodeResult.Text = DataAccessSettingsClass;
        }
        private void btnGenerateBusinessLayer_Click(object sender, EventArgs e)
        {
            if(cbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database before proceeding.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDatabases.DroppedDown = true;
                return;
            }

            if(lvTables.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a table before proceeding.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string SelectedTable = lvTables.SelectedItems[0].Text;
            string SelectedDatabase = cbDatabases.SelectedItem.ToString();
            List<clsColumn> TableColumns = clsDatabase.GetColumnsOfTable(SelectedDatabase, SelectedTable);

            clsBusinessLayerGenerator BLGenerator = new clsBusinessLayerGenerator(TableColumns, SelectedTable, SelectedDatabase);
            rtbCodeResult.Text = BLGenerator.GenerateClass().ToString();
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbCodeResult.Text);
        }
        private void pbCloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void pbMinimizeForm_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void ShowNotImplementedYetMessage(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature Not Implemented Yet", "Coming Soon");
        }
        private void btnAdvancedGenerating_Click(object sender, EventArgs e)
        {
            if (cbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database before proceeding.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDatabases.DroppedDown = true;
                return;
            }
            string SelectedDatbase = cbDatabases.SelectedItem.ToString();
            frmAdvancedGenerating frm = new frmAdvancedGenerating(SelectedDatbase);
            frm.ShowDialog();
        }
    }
}
