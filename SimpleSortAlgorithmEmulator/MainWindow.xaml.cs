using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SimpleSortAlgorithmEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SortingWindow sortingWindow;
        bool isSorting = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateRandomFileButton_Click(object sender, RoutedEventArgs e)
        {
            int randomNumber, minRandomNum, maxRandomNum;
            string storeFolderPath;

            //check validity of parameters
            try
            {
                randomNumber = Convert.ToInt32(RandomNumberComboBox.Text);
                minRandomNum = Convert.ToInt32(MinRandomNumTextBox.Text);
                maxRandomNum = Convert.ToInt32(MaxRandomNumTextBox.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("数值参数非法！请重新输入！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                RandomNumberComboBox.Text = "10000";
                MinRandomNumTextBox.Text = "0";
                MaxRandomNumTextBox.Text = "10000";
                return;
            }

            //select file saving path
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择随机数文件保存位置";
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK && folderBrowserDialog.SelectedPath != null)
            {
                storeFolderPath = folderBrowserDialog.SelectedPath;
            }
            else
            {
                return;
            }

            //generate random file
            Random r = new Random();
            string storeFilePath = String.Format("{0}\\RandomNum_{1}_{2}.txt", storeFolderPath, randomNumber, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            try
            {
                FileStream randomFile = new FileStream(storeFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                for (int i = 0; i < randomNumber; i++)
                {
                    byte[] buf = Encoding.Unicode.GetBytes(r.Next(minRandomNum, maxRandomNum).ToString() + " ");
                    randomFile.Write(buf, 0, buf.Length);
                }
                randomFile.Flush();
                randomFile.Close();
                System.Windows.MessageBox.Show("随机数文件生成完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                //update sort result folder path
                ResultSaveFolderPathTextBox.Text = storeFolderPath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("随机数文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //update load part
            RandomFilePathTextBox.Text = storeFilePath;
            UpdateRandomFileInfo(storeFilePath);
        }

        private void SelectRandomFileButton_Click(object sender, RoutedEventArgs e)
        {
            string randomFilePath;

            //select file saving path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK && openFileDialog.FileName != null)
            {
                randomFilePath = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            //update load part
            RandomFilePathTextBox.Text = randomFilePath;
            UpdateRandomFileInfo(randomFilePath);

            //update sort result folder path
            ResultSaveFolderPathTextBox.Text = openFileDialog.FileName.Replace(openFileDialog.SafeFileName, "");
        }

        private void UpdateRandomFileInfo(string randomFilePath)
        {
            FileInfo randomFileInfo = new FileInfo(randomFilePath);
            RandomFileCreateDateTextBlock.Text = randomFileInfo.CreationTime.ToString();
            RandomFileSizeTextBlock.Text = Math.Ceiling(randomFileInfo.Length / 1024.0) + " KB";

            try
            {
                FileStream randomFile = new FileStream(randomFilePath, FileMode.Open, FileAccess.Read);
                byte[] randomFileBytes = new byte[randomFile.Length];
                randomFile.Read(randomFileBytes, 0, (int)randomFile.Length);
                string randomFileContent = Encoding.Default.GetString(randomFileBytes);
                int numberCount = 0;
                foreach (var item in randomFileContent)
                {
                    if (item == ' ')
                    {
                        numberCount++;
                    }
                }
                RandomFileNumberInsideTextBlock.Text = numberCount.ToString();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("随机数文件读取失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void AlgorithmHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink algorithmHyperlink = (Hyperlink)sender;
            switch (algorithmHyperlink.Name)
            {
                case "Algorithm1Hyperlink":
                    break;
                case "Algorithm2Hyperlink":
                    break;
                case "Algorithm3Hyperlink":
                    break;
                case "Algorithm4Hyperlink":
                    break;
                case "Algorithm5Hyperlink":
                    break;
                case "Algorithm6Hyperlink":
                    break;
                case "Algorithm7Hyperlink":
                    break;
                case "Algorithm8Hyperlink":
                    break;
                default:
                    System.Windows.MessageBox.Show("操作非法！请重试！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }
        }

        private void SelectResultSaveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择排序后文件保存位置";
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK && folderBrowserDialog.SelectedPath != null)
            {
                ResultSaveFolderPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
            else
            {
                return;
            }
        }

        private void StartSortButton_Click(object sender, RoutedEventArgs e)
        {
            //check sorting status
            if (isSorting)
            {
                return;
            }
            else
            {
                isSorting = true;
            }

            int repeatNumber, randomNumCount;
            string resultFolderPath, randomFilePath;
            FileStream randomFile;
            byte[] randomFileBytes;
            string randomFileContent;
            int[] randomNums;

            //check validity of parameters
            try
            {
                repeatNumber = Convert.ToInt32(RepeatNumberComboBox.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("数值参数非法！请重新输入！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                RepeatNumberComboBox.Text = "1";
                isSorting = false;
                return;
            }

            if (repeatNumber <= 0 || repeatNumber > 20)
            {
                System.Windows.MessageBox.Show("重复数值过小或过大！请重新输入！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                RepeatNumberComboBox.Text = "1";
                isSorting = false;
                return;
            }

            //check validity of sort result folder path
            resultFolderPath = ResultSaveFolderPathTextBox.Text;
            try
            {
                if (!Directory.Exists(resultFolderPath))
                {
                    System.Windows.MessageBox.Show("排序结果存放路径无效！请重新输入！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResultSaveFolderPathTextBox.Text = RandomFilePathTextBox.Text;
                    isSorting = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("排序结果存放路径无效！请重新输入！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                ResultSaveFolderPathTextBox.Text = RandomFilePathTextBox.Text;
                isSorting = false;
                return;
            }

            //initialize sorting window
            sortingWindow = new SortingWindow();
            sortingWindow.Show();
            PrintToTextBox("排序算法模拟器\n");

            //print parameters
            PrintToTextBox("--------------------------------\n");
            PrintToTextBox("随机数文件：" + RandomFilePathTextBox.Text + "\n");
            PrintToTextBox("创建日期：" + RandomFileCreateDateTextBlock.Text + "\n");
            PrintToTextBox("文件大小：" + RandomFileSizeTextBlock.Text + "\n");
            PrintToTextBox("随机数个数：" + RandomFileNumberInsideTextBlock.Text + "\n");
            PrintToTextBox("--------------------------------\n");

            //load random num file
            PrintToTextBox("加载随机数文件...\n");
            randomFilePath = RandomFilePathTextBox.Text;
            try
            {
                randomFile = new FileStream(randomFilePath, FileMode.Open, FileAccess.Read);
                randomFileBytes = new byte[randomFile.Length];
                randomFile.Read(randomFileBytes, 0, (int)randomFile.Length);
                randomFileContent = Encoding.Default.GetString(randomFileBytes);

                randomNumCount = Convert.ToInt32(RandomFileNumberInsideTextBlock.Text);
                randomNums = new int[randomNumCount];
                int pos = 0, tmpNum;
                StringBuilder tmpNumSB = new StringBuilder();
                foreach (var item in randomFileContent)
                {
                    if (item == '\0')
                    {
                        continue;
                    }
                    if (item != ' ')
                    {
                        tmpNumSB.Append(item);
                    }
                    else
                    {
                        tmpNum = Convert.ToInt32(tmpNumSB.ToString());
                        randomNums[pos] = tmpNum;
                        tmpNumSB.Clear();
                        pos++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("加载随机数文件出错！请检查文件是否有效！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                sortingWindow.Close();
                isSorting = false;
                return;
            }
            PrintToTextBox("随机数文件加载成功！\n");

            //create sorting results folder
            PrintToTextBox("创建排序结果存放文件夹...\n");
            resultFolderPath += "SortingResults\\";
            try
            {
                Directory.CreateDirectory(resultFolderPath);
                if (!Directory.Exists(resultFolderPath))
                {
                    System.Windows.MessageBox.Show("排序结果存放文件夹路径无效！请重新输入！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResultSaveFolderPathTextBox.Text = RandomFilePathTextBox.Text;
                    sortingWindow.Close();
                    isSorting = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("排序结果存放文件夹路径无效！请检查！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                sortingWindow.Close();
                isSorting = false;
                return;
            }
            PrintToTextBox("排序结果存放文件夹创建成功！\n");
            PrintToTextBox("--------------------------------\n");

            //start sorting
            PrintToTextBox("开始排序...\n");
            PrintToTextBox("\n");

            if (Algorithm1CheckBox.IsChecked == true)
            {
                SortAlgorithm1(randomNums, repeatNumber, resultFolderPath);
            }
            sortingWindow.SortingTextBox.Text += "\n";

            PrintToTextBox("排序完成！\n");
            isSorting = false;
        }

        private void SortAlgorithm1(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            //直接插入排序
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();

            PrintToTextBox("正在进行直接插入排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次直接插入排序...\n");
                sw.Restart();

                //go sort
                for (int i = 1; i < innerRandomNums.Length; i++)
                {
                    if (innerRandomNums[i] < innerRandomNums[i - 1])
                    {
                        int temp = innerRandomNums[i];
                        int j;
                        for (j = i - 1; j >= 0 && temp < innerRandomNums[j]; j--)
                        {
                            innerRandomNums[j + 1] = innerRandomNums[j];
                        }
                        innerRandomNums[j + 1] = temp;
                    }
                }

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次直接插入排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");

            }

            PrintToTextBox("直接插入排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
        }

        private void PrintToTextBox(string str)
        {
            if (sortingWindow != null && sortingWindow.SortingTextBox != null)
            {
                sortingWindow.SortingTextBox.AppendText(str);
                sortingWindow.SortingTextBox.ScrollToEnd();
                App.DoEvents();
            }
        }
    }
}
