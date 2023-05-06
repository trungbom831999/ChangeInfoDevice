using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using WinSubTrial.Functions;

namespace WinSubTrial
{
    /// <summary>
    /// BACK UP & RESTORE SPAN
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    public partial class Main
    {

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    cboBackupFolder.Items.Clear();
                    cboBackupFolder.Items.AddRange(Directory.EnumerateDirectories("C:\\WINALL\\winbackup").Select(x => x.Substring(x.LastIndexOf('\\') + 1)).ToArray());
                    LoadBackupFiles(null, null);
                    break;
                case 2:
                    cboRestoreFolder.Items.Clear();
                    cboRestoreFolder.Items.AddRange(Directory.EnumerateDirectories("C:\\WINALL\\winrestore").Select(x => x.Substring(x.LastIndexOf('\\') + 1)).ToArray());
                    LoadRestoreFiles(null, null);
                    break;
                case 3:
                    break;
            }
        }

        private void InitDGVBackupsColumns()
        {

            colRestore = new DataGridViewImageColumn
            {
                ToolTipText = "Restore",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.Restore
            };
            dgvBackupFiles.Columns.Add(colRestore);

            colMoveToRestore = new DataGridViewImageColumn
            {
                ToolTipText = "Move to restore folder",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.Move
            };
            dgvBackupFiles.Columns.Add(colMoveToRestore);

            colDelete = new DataGridViewImageColumn
            {
                ToolTipText = "Delete",
                Width = 60,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.Delete
            };
            dgvBackupFiles.Columns.Add(colDelete);
        }

        private void InitDGVRestoresColumns()
        {

            colMoveToBackup = new DataGridViewImageColumn
            {
                ToolTipText = "Move to Backup",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.Move
            };
            dgvRestoreFiles.Columns.Add(colMoveToBackup);

            colDeleteRestore = new DataGridViewImageColumn
            {
                ToolTipText = "Delete",
                Width = 26,
                Resizable = DataGridViewTriState.False,
                Image = Properties.Resources.Delete
            };
            dgvRestoreFiles.Columns.Add(colDeleteRestore);
        }

        private void LoadBackupFiles(string folder, string searchName)
        {
            dgvBackupFiles.Rows.Clear();

            viewModel.threadLoadBackup = new Thread(new ThreadStart(() =>
            {
                Common.LoadBackupList();
                if (Common.ListBackups.Count > 0)
                {
                    int rowId;
                    Invoke(new MethodInvoker(() =>
                    {
                        foreach (BackupData backup in Common.ListBackups.FindAll(x => (string.IsNullOrEmpty(folder) || x.Folder.Equals(folder)) && (string.IsNullOrEmpty(searchName) || x.Name.Contains(searchName))))
                        {
                            rowId = dgvBackupFiles.Rows.Add(new string[] { "false", backup.Name, backup.Size, backup.Folder, backup.Date });
                            dgvBackupFiles.Rows[rowId].Height = 39;
                            if (rowId % 2 == 1) dgvBackupFiles.Rows[rowId].DefaultCellStyle.BackColor = SystemColors.ButtonFace;
                        }
                    }));
                }
            }))
            { IsBackground = true };
            viewModel.threadLoadBackup.Start();
        }

        private void LoadRestoreFiles(string folder, string searchName)
        {
            dgvRestoreFiles.Rows.Clear();

            viewModel.threadLoadBackup = new Thread(new ThreadStart(() =>
            {
                Common.LoadRestoreList();
                if (Common.ListRestores.Count > 0)
                {
                    int rowId;
                    Invoke(new MethodInvoker(() =>
                    {
                        foreach (RestoreData restore in Common.ListRestores.FindAll(x => (string.IsNullOrEmpty(folder) || x.Folder.Equals(folder)) && (string.IsNullOrEmpty(searchName) || x.Name.Contains(searchName))))
                        {
                            rowId = dgvRestoreFiles.Rows.Add(new string[] { "false", restore.Name, restore.Size, restore.Folder, restore.Date });
                            dgvRestoreFiles.Rows[rowId].Height = 39;
                            if (rowId % 2 == 1) dgvRestoreFiles.Rows[rowId].DefaultCellStyle.BackColor = SystemColors.ButtonFace;
                        }
                    }));
                }
            }))
            { IsBackground = true };
            viewModel.threadLoadBackup.Start();
        }

        #region Backup span
        private void btnBackup_Click(object sender, EventArgs e)
        {
            string bk_packages = Common.Settings.AppBackup;

            if (!viewModel.selectedOneDevice()) return;
            string serial = serial = viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial;

            if (viewModel.IsDeviceInTask(serial))
            {
                Common.SetStatus(serial, "Please wait current task completed");
                return;
            }
            string saveDir = "";
            using (Backup frmBackup = new Backup())
            {
                frmBackup.ShowDialog();
                saveDir = frmBackup.SaveDir;
                if (string.IsNullOrEmpty(saveDir))
                {
                    Common.SetStatus(serial, "Backup aborted");
                    return;
                }

                saveDir = @"C:\WINALL\WinBackup" + saveDir;
            }
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Device device = viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                Common.SetStatus(device.Serial, "Backuping...");
                Common.SetStatus(device.Serial, "Backup " + (new Functions.Backup { device = device }.Save(saveDir, bk_packages) ? "done" : "fail"));
                //SaveGGSystem
                //SaveApp
            }))
            { IsBackground = true };
            viewModel.deviceThreads[serial] = thread;
            thread.Start();
        }

        private void btnBackupChange_Click(object sender, EventArgs e)
        {
            string bk_packages = Common.Settings.AppBackup;
            if (!viewModel.selectedOneDevice()) return;
            string serial = serial = viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial;
            if (viewModel.IsDeviceInTask(serial))
            {
                Common.SetStatus(serial, "Please wait current task completed");
                return;
            }

            Thread thread = new Thread(new ThreadStart(() => {
                Device device = viewModel.devicesModel.FirstOrDefault(x => x.Serial.Equals(serial));
                if (device.RequestOption == null)
                {
                    Common.Info("Info không thay đổi");
                    Common.SetStatus(device.Serial, "Info không thay đổi");
                    return;
                }
                string saveDir = "";
                using (Backup frmBackup = new Backup())
                {
                    frmBackup.ShowDialog();
                    saveDir = frmBackup.SaveDir;
                    if (string.IsNullOrEmpty(saveDir))
                    {
                        Common.SetStatus(serial, "Backup aborted");
                        return;
                    }

                    saveDir = @"C:\WINALL\WinBackup" + saveDir;
                }

                //SaveGGSystem
                //SaveApp

                Common.SetStatus(device.Serial, "Saving backup...");
                new Functions.Backup { device = device }.Save(saveDir, bk_packages);

                Common.SetStatus(device.Serial, "Saving info...");
                new Change { device = device }.SaveInfo();

                Common.SetStatus(device.Serial, "Backup & Change done");

                //Remove changed info
                device.RequestOption = null;
            }))
            { IsBackground = true };
            viewModel.deviceThreads[serial] = thread;
            thread.Start();
        }


        private void btnSearchBackup_Click(object sender, EventArgs e)
        {
            LoadBackupFiles(folder: cboBackupFolder.Text, searchName: txtBackupSearch.Text);
        }

        private void btnToolsDevice1_Click(object sender, EventArgs e)
        {
            ShowDeviceTools();
        }

        private void dgvBackupFiles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBackupFiles.Rows.Count == 0 || dgvBackupFiles.SelectedRows.Count == 0)
            {
                return;
            }

            for (int i = 0; i < dgvBackupFiles.Rows.Count; i++)
            {
                dgvBackupFiles.Rows[i].DefaultCellStyle.BackColor = i % 2 == 0 ? SystemColors.Window : SystemColors.ButtonFace;
            }
            if (dgvBackupFiles.SelectedRows.Count > 0) dgvBackupFiles.SelectedRows[0].DefaultCellStyle.BackColor = Color.FromArgb(196, 253, 236);
        }

        private void dgvBackupFiles_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        private void dgvBackupFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            int rowIndex = e.RowIndex;
            DataGridViewRow row = dgvBackupFiles.Rows[rowIndex];
            BackupData file = Common.ListBackups.FirstOrDefault(x => x.Name == row.Cells[1].Value.ToString());

            switch (e.ColumnIndex)
            {
                case 6:

                    string serial = viewModel.devicesModel.FirstOrDefault(x => x.isSelected == true).Serial;
                    if (viewModel.IsDeviceInTask(serial))
                    {
                        Common.Info("Vui lòng đợi device hoàn thành tác vụ trước đó");
                        return;
                    }

                    viewModel.deviceTasks[serial] = Task.Run(() =>
                    {
                        try
                        {
                            string bk_packages = Common.Settings.AppBackup;
                            row.Cells[5].Value = "Restoring...";
                            Device device = viewModel.devicesModel.First(x => x.Serial.Equals(serial));
                            bool ok = new Functions.Backup { device = device }.Restore($@"C:\WINALL\winbackup\{file.Folder}\{file.Name}", bk_packages);
                            row.Cells[5].Value = "Restore " + (ok ? "done" : "fail");
                        }
                        catch (Exception ex)
                        {
                            row.Cells[5].Value = $"Restore fail ({ex.Message})";
                        }
                    });
                    break;
                case 7:
                    try
                    {
                        MyFile.Move($@"C:\WINALL\winbackup\{file.Folder}\{file.Name}", $@"C:\WINALL\winrestore\{file.Folder}\{file.Name}");
                        dgvBackupFiles.Rows.Remove(row);
                    }
                    catch { }
                    break;
                case 8:
                    try
                    {
                        File.Delete($@"C:\WINALL\winbackup\{file.Folder}\{file.Name}");
                        dgvBackupFiles.Rows.Remove(row);
                    }
                    catch { }
                    break;
                default:
                    break;
            }
        }
        #endregion Backup span

        #region Restore span
        private void btnSearchRestoreFiles_Click(object sender, EventArgs e)
        {
            LoadRestoreFiles(cboRestoreFolder.Text, txtSearchRestoreFiles.Text);
        }

        private void btnToolsDevice2_Click(object sender, EventArgs e)
        {
            ShowDeviceTools();
        }

        private void dgvRestoreFiles_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        private void dgvRestoreFiles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRestoreFiles.Rows.Count == 0 || dgvRestoreFiles.SelectedRows.Count == 0)
            {
                return;
            }

            for (int i = 0; i < dgvRestoreFiles.Rows.Count; i++)
            {
                dgvRestoreFiles.Rows[i].DefaultCellStyle.BackColor = i % 2 == 0 ? SystemColors.Window : SystemColors.ButtonFace;
            }
            if (dgvRestoreFiles.SelectedRows.Count > 0) dgvRestoreFiles.SelectedRows[0].DefaultCellStyle.BackColor = Color.FromArgb(196, 253, 236);
        }

        private void dgvRestoreFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            int rowIndex = e.RowIndex;
            DataGridViewRow row = dgvRestoreFiles.Rows[rowIndex];
            RestoreData file = Common.ListRestores.FirstOrDefault(x => x.Name == row.Cells[1].Value.ToString());

            switch (e.ColumnIndex)
            {
                case 6:
                    try
                    {
                        MyFile.Move($@"C:\WINALL\winrestore\{file.Folder}\{file.Name}", $@"C:\WINALL\winbackup\{file.Folder}\{file.Name}");
                        dgvRestoreFiles.Rows.Remove(row);
                    }
                    catch { }
                    break;
                case 7:
                    try
                    {
                        File.Delete($@"C:\WINALL\winrestore\{file.Folder}\{file.Name}");
                        dgvRestoreFiles.Rows.Remove(row);
                    }
                    catch { }
                    break;
                default:
                    break;
            }
        }
        #endregion Restore span

    }
}
