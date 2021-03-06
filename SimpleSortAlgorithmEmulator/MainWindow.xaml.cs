﻿using System;
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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SimpleSortAlgorithmEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SortingWindow sortingWindow;
        bool isSorting = false;
        ObservableCollection<SortResultContainer> sortResultContainerList;

        public MainWindow()
        {
            InitializeComponent();

            //set data binding
            sortResultContainerList = new ObservableCollection<SortResultContainer>();
            SortResultListView.ItemsSource = sortResultContainerList;
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
                maxRandomNum = Convert.ToInt32(MaxRandomNumTextBox.Text) + 1;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("数值参数非法！请重新输入！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                RandomNumberComboBox.Text = "10000";
                MinRandomNumTextBox.Text = "0";
                MaxRandomNumTextBox.Text = "10000";
                return;
            }
            if (randomNumber > 10000000 || randomNumber <= 0)
            {
                System.Windows.MessageBox.Show("随机数个数过多或过少！请重新输入！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                RandomNumberComboBox.Text = "10000";
                MinRandomNumTextBox.Text = "0";
                MaxRandomNumTextBox.Text = "10000";
                return;
            }
            if (minRandomNum >= maxRandomNum)
            {
                System.Windows.MessageBox.Show("最大值或最小值非法！请重新输入！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ResultSaveFolderPathTextBox.Text = storeFolderPath + "\\";
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
                if (numberCount > 10000000)
                {
                    System.Windows.MessageBox.Show("随机数个数过多！请重新选择！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    RandomFilePathTextBox.Text = "";
                    return;
                }
                RandomFileNumberInsideTextBlock.Text = numberCount.ToString();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("随机数文件读取失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ResultSaveFolderPathTextBox.Text = folderBrowserDialog.SelectedPath + "\\";
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
            resultFolderPath += "SortingResults_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "\\";
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

            //copy original random number file to result folder
            try
            {
                File.Copy(randomFilePath, resultFolderPath + "Original_" + Path.GetFileName(randomFile.Name), true);
                PrintToTextBox("原随机数文件复制到排序结果存放文件夹成功！\n");
                PrintToTextBox("--------------------------------\n");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("复制原随机数文件失败！请检查！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                sortingWindow.Close();
                isSorting = false;
                return;
            }

            //check algorithm selection
            if (
                Algorithm1CheckBox.IsChecked == false &&
                Algorithm2CheckBox.IsChecked == false &&
                Algorithm3CheckBox.IsChecked == false &&
                Algorithm4CheckBox.IsChecked == false &&
                Algorithm5CheckBox.IsChecked == false &&
                Algorithm6CheckBox.IsChecked == false &&
                Algorithm7CheckBox.IsChecked == false &&
                Algorithm8CheckBox.IsChecked == false
                )
            {
                System.Windows.MessageBox.Show("未选择任何算法！请至少勾选一种排序算法！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                sortingWindow.Close();
                isSorting = false;
                return;
            }

            //initialize sort result container list
            sortResultContainerList.Clear();

            //start sorting
            PrintToTextBox("开始排序...\n");
            PrintToTextBox("\n");

            //algorithm 1
            if (Algorithm1CheckBox.IsChecked == true)
            {
                SortAlgorithm1(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //algorithm 2
            if (Algorithm2CheckBox.IsChecked == true)
            {
                SortAlgorithm2(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //algorithm 3
            if (Algorithm3CheckBox.IsChecked == true)
            {
                SortAlgorithm3(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //algorithm 4
            if (Algorithm4CheckBox.IsChecked == true)
            {
                SortAlgorithm4(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //algorithm 5
            if (Algorithm5CheckBox.IsChecked == true)
            {
                SortAlgorithm5(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //algorithm 6
            if (Algorithm6CheckBox.IsChecked == true)
            {
                SortAlgorithm6(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //algorithm 7
            if (Algorithm7CheckBox.IsChecked == true)
            {
                SortAlgorithm7(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //algorithm 8
            if (Algorithm8CheckBox.IsChecked == true)
            {
                SortAlgorithm8(randomNums, repeatNumber, resultFolderPath);
                PrintToTextBox("\n");
            }

            //save time consumption to file
            try
            {
                string resultFilePath = String.Format("{0}Time_Consumption_Record.txt", resultFolderPath);
                FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                foreach (var item in sortResultContainerList)
                {
                    byte[] buf;

                    buf = Encoding.UTF8.GetBytes(item.Name + "\r\n");
                    resultFile.Write(buf, 0, buf.Length);

                    buf = Encoding.UTF8.GetBytes("重复次数：" + item.Count + "\r\n");
                    resultFile.Write(buf, 0, buf.Length);

                    buf = Encoding.UTF8.GetBytes("总用时：" + item.TimeSum + " 秒\r\n");
                    resultFile.Write(buf, 0, buf.Length);

                    buf = Encoding.UTF8.GetBytes("平均用时：" + item.TimeAvg + " 秒\r\n");
                    resultFile.Write(buf, 0, buf.Length);

                    buf = Encoding.UTF8.GetBytes("详细情况：\r\n");
                    resultFile.Write(buf, 0, buf.Length);

                    for (int i = 0; i < item.Count; i++)
                    {
                        buf = Encoding.UTF8.GetBytes("\t第" + (i + 1) + "次排序用时：" + item.TimeArr[i] + " 秒\r\n");
                        resultFile.Write(buf, 0, buf.Length);
                    }

                    buf = Encoding.UTF8.GetBytes("\r\n");
                    resultFile.Write(buf, 0, buf.Length);
                }
                resultFile.Flush();
                resultFile.Close();
                PrintToTextBox("排序消耗时间保存完成！\n");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("排序消耗时间保存失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PrintToTextBox("\n");

            PrintToTextBox("排序完成！\n");
            isSorting = false;

            System.Windows.MessageBox.Show("排序完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        //直接插入排序
        private void SortAlgorithm1(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(1, "直接插入排序", repeatNum);

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
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm1_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次直接插入排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("直接插入排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        //折半插入排序
        private void SortAlgorithm2(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(2, "折半插入排序", repeatNum);

            PrintToTextBox("正在进行折半插入排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次折半插入排序...\n");
                sw.Restart();

                //go sort
                for (int i = 1; i < innerRandomNums.Length; i++)
                {
                    int low = 0;
                    int high = i - 1;
                    int temp = innerRandomNums[i];
                    //find
                    while (low <= high)
                    {
                        int mid = (low + high) / 2;
                        if (temp < innerRandomNums[mid])
                        {
                            high = mid - 1;
                        }
                        else
                        {
                            low = mid + 1;
                        }
                    }
                    //backward shift
                    for (int j = i - 1; j >= low; j--)
                    {
                        innerRandomNums[j + 1] = innerRandomNums[j];
                    }
                    innerRandomNums[low] = temp;
                }

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次折半插入排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm2_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次折半插入排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("折半插入排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        //希尔排序
        private void SortAlgorithm3(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(3, "希尔排序", repeatNum);

            PrintToTextBox("正在进行希尔排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次希尔排序...\n");
                sw.Restart();

                //go sort
                {
                    int i, j, flag, tmp, gap = innerRandomNums.Length;
                    while (gap > 1)
                    {
                        gap = gap / 2;
                        do
                        {
                            flag = 0;
                            for (i = 0; i < innerRandomNums.Length - gap; i++)
                            {
                                j = i + gap;
                                if (innerRandomNums[i] > innerRandomNums[j])
                                {
                                    tmp = innerRandomNums[i];
                                    innerRandomNums[i] = innerRandomNums[j];
                                    innerRandomNums[j] = tmp;
                                    flag = 1;
                                }
                            }
                        } while (flag != 0);
                    }
                }

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次希尔排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm3_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次希尔排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("希尔排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        //冒泡排序
        private void SortAlgorithm4(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(4, "冒泡排序", repeatNum);

            PrintToTextBox("正在进行冒泡排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次冒泡排序...\n");
                sw.Restart();

                //go sort
                int flag = 0;
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    flag = 0;
                    for (int j = innerRandomNums.Length - 1; j > i; j--)
                    {
                        if (innerRandomNums[j] < innerRandomNums[j - 1])
                        {
                            flag = 1;
                            int temp = innerRandomNums[j];
                            innerRandomNums[j] = innerRandomNums[j - 1];
                            innerRandomNums[j - 1] = temp;
                        }
                    }
                    if (flag == 0)
                    {
                        break;
                    }
                }

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次冒泡排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm4_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次冒泡排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("冒泡排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        //快速排序
        private void SortAlgorithm5(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(5, "快速排序", repeatNum);

            PrintToTextBox("正在进行快速排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次快速排序...\n");
                sw.Restart();

                //go sort
                Quicksort(innerRandomNums, 0, innerRandomNums.Length - 1);

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次快速排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm5_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次快速排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("快速排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        private void Quicksort(int[] a, int low, int high)
        {
            if (low >= high)
            {
                return;
            }
            int first = low, last = high;
            int key = a[low];
            while (first < last)
            {
                while (first < last && a[last] >= key)
                {
                    last--;
                }
                a[first] = a[last];
                while (first < last && a[first] <= key)
                {
                    first++;
                }
                a[last] = a[first];
            }
            a[first] = key;
            Quicksort(a, low, first - 1);
            Quicksort(a, first + 1, high);

        }

        //简单选择排序
        private void SortAlgorithm6(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(6, "简单选择排序", repeatNum);

            PrintToTextBox("正在进行简单选择排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次简单选择排序...\n");
                sw.Restart();

                //go sort
                for (int i = 0; i < innerRandomNums.Length - 1; i++)
                {
                    int min = innerRandomNums[i];
                    int minIndex = i;
                    for (int j = i + 1; j < innerRandomNums.Length; j++)
                    {
                        if (innerRandomNums[j] < min)
                        {
                            min = innerRandomNums[j];
                            minIndex = j;
                        }
                    }
                    if (minIndex != i)
                    {
                        int temp = innerRandomNums[i];
                        innerRandomNums[i] = innerRandomNums[minIndex];
                        innerRandomNums[minIndex] = temp;
                    }
                }

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次简单选择排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm6_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次简单选择排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("简单选择排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        //堆排序
        private void SortAlgorithm7(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(7, "堆排序", repeatNum);

            PrintToTextBox("正在进行堆排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次堆排序...\n");
                sw.Restart();

                //go sort
                HeapSort(innerRandomNums);

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次堆排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm7_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次堆排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("堆排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        private void HeapSort(int[] innerRandomNums)
        {
            BuildMaxHeap(innerRandomNums);
            for (int i = innerRandomNums.Length - 1; i > 0; i--)
            {
                Swap(ref innerRandomNums[0], ref innerRandomNums[i]);
                MaxHeaping(innerRandomNums, 0, i);
            }
        }

        private void BuildMaxHeap(int[] innerRandomNums)
        {
            for (int i = (innerRandomNums.Length / 2) - 1; i >= 0; i--)
            {
                MaxHeaping(innerRandomNums, i, innerRandomNums.Length);
            }
        }

        private void MaxHeaping(int[] innerRandomNums, int i, int heapSize)
        {
            int left = (2 * i) + 1;
            int right = 2 * (i + 1);
            int large = i;

            if (left < heapSize && innerRandomNums[left] > innerRandomNums[large])
            {
                large = left;
            }

            if (right < heapSize && innerRandomNums[right] > innerRandomNums[large])
            {
                large = right;
            }

            if (i != large)
            {
                Swap(ref innerRandomNums[i], ref innerRandomNums[large]);
                MaxHeaping(innerRandomNums, large, heapSize);
            }
        }

        private void Swap(ref int a, ref int b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }

        //二路归并排序
        private void SortAlgorithm8(int[] randomNums, int repeatNum, string resultFolderPath)
        {
            decimal timeSum = 0;
            int[] innerRandomNums = new int[randomNums.Length];
            Stopwatch sw = new Stopwatch();
            SortResultContainer sortResultContainer = new SortResultContainer(8, "二路归并排序", repeatNum);

            PrintToTextBox("正在进行二路归并排序...\n");

            for (int count = 0; count < repeatNum; count++)
            {
                //initialize inner random nums
                for (int i = 0; i < innerRandomNums.Length; i++)
                {
                    innerRandomNums[i] = randomNums[i];
                }

                //initialize time record
                PrintToTextBox("正在进行第" + (count + 1) + "次二路归并排序...\n");
                sw.Restart();

                //go sort
                MergeSortFunction(innerRandomNums, 0, innerRandomNums.Length - 1);

                //record time used
                sw.Stop();
                timeSum += sw.ElapsedTicks / (decimal)Stopwatch.Frequency;
                PrintToTextBox("第" + (count + 1) + "次二路归并排序完成！用时" + (sw.ElapsedTicks / (decimal)Stopwatch.Frequency) + "秒\n");
                sortResultContainer.TimeArr[count] = (sw.ElapsedTicks / (decimal)Stopwatch.Frequency);

                //save sorted numbers to file
                string resultFilePath = String.Format("{0}Sorted_Algorithm8_{1}.txt", resultFolderPath, count + 1);
                try
                {
                    FileStream resultFile = new FileStream(resultFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    for (int i = 0; i < innerRandomNums.Length; i++)
                    {
                        byte[] buf = Encoding.Unicode.GetBytes(innerRandomNums[i] + " ");
                        resultFile.Write(buf, 0, buf.Length);
                    }
                    resultFile.Flush();
                    resultFile.Close();
                    PrintToTextBox("第" + (count + 1) + "次二路归并排序结果已保存到文件\n");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("排序结果文件生成失败！请重试！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    isSorting = false;
                    return;
                }
            }

            PrintToTextBox("二路归并排序完成！共用时" + timeSum + "秒，平均用时" + (timeSum / repeatNum) + "秒\n");
            sortResultContainer.TimeSum = timeSum;
            sortResultContainer.TimeAvg = timeSum / repeatNum;
            sortResultContainerList.Add(sortResultContainer);
        }

        private void MergeSortFunction(int[] array, int first, int last)
        {
            if (first < last)
            {
                int mid = (first + last) / 2;
                MergeSortFunction(array, first, mid);
                MergeSortFunction(array, mid + 1, last);
                MergeSortCore(array, first, mid, last);
            }
        }

        private void MergeSortCore(int[] array, int first, int mid, int last)
        {
            int indexA = first;
            int indexB = mid + 1;
            int[] temp = new int[last + 1];
            int tempIndex = 0;
            while (indexA <= mid && indexB <= last)
            {
                if (array[indexA] <= array[indexB])
                {
                    temp[tempIndex++] = array[indexA++];
                }
                else
                {
                    temp[tempIndex++] = array[indexB++];
                }
            }
            while (indexA <= mid)
            {
                temp[tempIndex++] = array[indexA++];
            }
            while (indexB <= last)
            {
                temp[tempIndex++] = array[indexB++];
            }

            tempIndex = 0;
            for (int i = first; i <= last; i++)
            {
                array[i] = temp[tempIndex++];
            }

        }

        //integrate actions related to print words to textbox
        private void PrintToTextBox(string str)
        {
            if (sortingWindow != null && sortingWindow.SortingTextBox != null)
            {
                sortingWindow.SortingTextBox.AppendText(str);
                sortingWindow.SortingTextBox.ScrollToEnd();
                App.DoEvents();
                Thread.Sleep(50);//to make textbox ui refresh fluently
            }
        }

        private void ShowDetailResultButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            StackPanel parentStackPanel = (StackPanel)button.Parent;
            StackPanel parentParentStackPanel = (StackPanel)parentStackPanel.Parent;
            System.Windows.Controls.ListView listView = (System.Windows.Controls.ListView)parentParentStackPanel.Children[1];
            if (listView.Visibility == Visibility.Collapsed)
            {
                listView.Visibility = Visibility.Visible;
                button.Content = "收起";
                return;
            }
            if (listView.Visibility == Visibility.Visible)
            {
                listView.Visibility = Visibility.Collapsed;
                button.Content = "展开";
                return;
            }
        }

        private void GridViewColumnHeaderName_Click(object sender, RoutedEventArgs e)
        {
            bool isSorted = true;
            List<SortResultContainer> list = sortResultContainerList.OrderBy(u => u.Id).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[0] != sortResultContainerList[0])
                {
                    isSorted = false;
                }
            }
            if (isSorted)
            {
                list.Reverse();
            }

            sortResultContainerList.Clear();
            foreach (var item in list)
            {
                sortResultContainerList.Add(item);
            }
        }

        private void GridViewColumnHeaderTime_Click(object sender, RoutedEventArgs e)
        {
            bool isSorted = true;
            List<SortResultContainer> list = sortResultContainerList.OrderBy(u => u.TimeSum).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[0] != sortResultContainerList[0])
                {
                    isSorted = false;
                }
            }
            if (isSorted)
            {
                list.Reverse();
            }

            sortResultContainerList.Clear();
            foreach (var item in list)
            {
                sortResultContainerList.Add(item);
            }
        }

        private void HelpPageHyperlink_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }
    }

    public class SortResultContainer : INotifyPropertyChanged
    {
        private int id;
        private string name;
        private int count;
        private decimal[] timeArr;
        private decimal timeSum;
        private decimal timeAvg;

        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged(new PropertyChangedEventArgs("Id")); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
        }

        public int Count
        {
            get { return count; }
            set { count = value; OnPropertyChanged(new PropertyChangedEventArgs("Count")); }
        }

        public decimal[] TimeArr
        {
            get { return timeArr; }
            set { timeArr = value; OnPropertyChanged(new PropertyChangedEventArgs("TimeArr")); }
        }

        public decimal TimeSum
        {
            get { return timeSum; }
            set { timeSum = value; OnPropertyChanged(new PropertyChangedEventArgs("TimeArr")); }
        }

        public decimal TimeAvg
        {
            get { return timeAvg; }
            set { timeAvg = value; OnPropertyChanged(new PropertyChangedEventArgs("TimeAvg")); }
        }

        public SortResultContainer(int id, string name, int count)
        {
            Id = id;
            Name = name;
            Count = count;
            TimeArr = new decimal[count];
            TimeSum = 0;
            TimeAvg = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
