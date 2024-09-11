using CodeGen.Global;
using CodeGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGen
{
    public partial class frmAdvancedGenerating : Form
    {
        List<List<clsColumn>> _DatabaseTables = new List<List<clsColumn>>();
        List<string> _Tables = new List<string>();
        string _SelectedDatabase = "";
        string _SelectedPath = "";
        const string FileExtension = ".cs";
        public frmAdvancedGenerating(string Database)
        {
            InitializeComponent();
            _SelectedDatabase = Database;

        }
        private void frmAdvancedGenerating_Load(object sender, EventArgs e)
        {

            _Tables = clsDatabase.GetTablesOfDatabase(_SelectedDatabase);

            foreach(string Table in _Tables)
            {
                this._DatabaseTables.Add(clsDatabase.GetColumnsOfTable(_SelectedDatabase, Table));
            }
            lblSelectedDatabase.Text = _SelectedDatabase;
        }
        private void pbSelectFolder_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _SelectedPath = folderBrowserDialog.SelectedPath;
                txtFolderPath.Text = _SelectedPath;
            }

        }
        private void btnGenerateDAL_Click(object sender, EventArgs e)
        {
            if(!clsDatabase.DoesDatabaseExist(_SelectedDatabase))
            {
                MessageBox.Show("The selected database does not exist. Please choose a valid database.",
                       "Database Error",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                return;
            }
            if(_SelectedPath == "")
            {
                MessageBox.Show("No destination folder has been selected. Please choose a folder to save the generated files.",
                "Folder Selection Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
                return;
            }
            if(MessageBox.Show("Are you sure you want to create the Data Access Layer classes in the following folder?\n\n"
                                      + _SelectedPath,
                                      "Confirm Business Layer Generation",
                                      MessageBoxButtons.OKCancel,
                                      MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            

            int TableCounter = 0;

            foreach(List<clsColumn> Table in _DatabaseTables)
            {
                clsDataAccessLayerGenerator DALGenerator = new clsDataAccessLayerGenerator(Table, _Tables[TableCounter], _SelectedDatabase);
                StringBuilder Class = DALGenerator.GenerateClass();

                if(!clsUtil.Createfile(_SelectedPath, DALGenerator.TableClassName + FileExtension, Class.ToString()))
                {
                    if(clsUtil.DoesFileExist(_SelectedPath, DALGenerator.TableClassName + FileExtension))
                    {
                        MessageBox.Show("file with name (" + DALGenerator.TableClassName + FileExtension + ") already exists in the selected path. " +
                                        "Please remove existing file and try again.",
                                        "File Already Exists",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Failed to create file: " + DALGenerator.TableClassName + FileExtension +
                                        " Please check the selected path and try again.",
                                        "File Creation Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }
                }
                TableCounter++;
            }

            StringBuilder DataAccessClass = clsDataAccessLayerGenerator.GenerateDataAccessSettingsClass(_SelectedDatabase);
            clsUtil.Createfile(_SelectedPath, "DataAccessSettings" + FileExtension, DataAccessClass.ToString());

            MessageBox.Show("Data Access Layer Classes have been successfully generated for all tables.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void btnGenerateBL_Click(object sender, EventArgs e)
        {
            if(!clsDatabase.DoesDatabaseExist(_SelectedDatabase))
            {
                MessageBox.Show("The selected database does not exist. Please choose a valid database.",
                       "Database Error",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                return;
            }
            if(_SelectedPath == "")
            {
                MessageBox.Show("No destination folder has been selected. Please choose a folder to save the generated files.",
                "Folder Selection Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
                return;
            }
            if(MessageBox.Show("Are you sure you want to create the Business Layer classes in the following folder?\n\n"
                          + _SelectedPath,
                          "Confirm Business Layer Generation",
                          MessageBoxButtons.OKCancel,
                          MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }

            int TableCounter = 0;

            foreach(List<clsColumn> Table in _DatabaseTables)
            {
                clsBusinessLayerGenerator BLGenerator = new clsBusinessLayerGenerator(Table, _Tables[TableCounter], _SelectedDatabase);
                StringBuilder Class = BLGenerator.GenerateClass();

                if(!clsUtil.Createfile(_SelectedPath, BLGenerator.TableClassName + FileExtension, Class.ToString()))
                {
                    if(clsUtil.DoesFileExist(_SelectedPath, BLGenerator.TableClassName + FileExtension))
                    {
                        MessageBox.Show("file with name (" + BLGenerator.TableClassName + FileExtension + ") already exists in the selected path. " +
                                        "Please remove the existing file and try again.",
                                        "File Already Exists",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Failed to create file: " + BLGenerator.TableClassName + FileExtension +
                                        " Please check the selected path and try again.",
                                        "File Creation Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }
                }
                TableCounter++;
            }
            MessageBox.Show("Business Layer Classes have been successfully generated for all tables.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void pbMinimizeForm_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void pbCloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
