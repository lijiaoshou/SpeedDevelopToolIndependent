﻿using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace CommonLib
{
    public class Common
    {
        //读取注册表中的键值来确定编译工具的位置，因为每个人安装vs目录可能不同，导致devenv.exe的路径可能不同
        private const string REGISTKEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\devenv.exe";

        private static string[] UpperAndUnderLinePatternArray = {   "CheckedChanged",
                                                                    "CheckStateChanged",
                                                                    "AppearanceChanged",
                                                                    "DropDown",
                                                                    "DrawItem",
                                                                    "MeasureItem",
                                                                    "SelectedIndexChanged",
                                                                    "SelectionChangeCommitted",
                                                                    "Format",
                                                                    "DropDownClosed",
                                                                    "ValueMemberChanged",
                                                                    "DataSourceChanged",
                                                                    "DisplayMemberChanged",
                                                                    "DropDownStyleChanged",
                                                                    "TextUpdate",
                                                                    "FormatStringChanged",
                                                                    "FormattingEnabledChanged",
                                                                    "SelectedIndexChanged",
                                                                    "SelectedValueChanged",
                                                                    "TextAlignChanged",
                                                                    "ReadOnlyChanged",
                                                                    "MultilineChanged",
                                                                    "ModifiedChanged",
                                                                    "HideSelectionChanged",
                                                                    "BorderStyleChanged",
                                                                    "AcceptsTabChanged" };

        private static string[] NoChangedPatternArray = {   "BackgroundImageChanged",
                                                            "BackgroundImageLayoutChanged",
                                                            "BackColorChanged",
                                                            "BindingContextChanged",
                                                            "CausesValidationChanged",
                                                            "ClientSizeChanged",
                                                            "SizeChanged",
                                                            "ContextMenuStripChanged",
                                                            "CursorChanged",
                                                            "DockChanged",
                                                            "EnabledChanged",
                                                            "FontChanged",
                                                            "ForeColorChanged",
                                                            "LocationChanged",
                                                            "ParentChanged",
                                                            "RightToLeftChanged",
                                                            "TabIndexChanged",
                                                            "TabStopChanged",
                                                            "TextChanged",
                                                            "VisibleChanged",
                                                            "ChangeUICues"};

        #region 公用方法

        /// <summary>
        /// 发送邮件提醒
        /// </summary>
        /// <param name="Msg">邮件信息</param>
        /// <returns></returns>
        public static bool SendEmail(string Msg)
        {
            string resultMessage = string.Empty;
            bool isSend = false;
            List<string> mailList = new List<string>();
            mailList.Add("lichaor@yonyou.com");


            // 检测邮件是否发送成功
            try
            {
                SendEmail("快速开发问题反馈", "有客户遇到开发问题:" + "<br/>" + Msg, mailList);
                isSend = true;
            }
            catch (Exception ex)
            {
            }

            return isSend;
        }

        /// <summary>
        /// 发送提醒邮件
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Content"></param>
        /// <param name="listEmail"></param>
        /// <returns></returns>
        public static bool SendEmail(string Title, string Content, List<string> listEmail)
        {

            bool isPass = false;
            try
            {
                SmtpClient client = new SmtpClient("mail.yonyou.com");
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                NetworkCredential myCredentials = new NetworkCredential("lichaor@yonyou.com", "12312");
                client.Credentials = myCredentials;
                MailMessage message = new MailMessage() { Subject = Title, Body = Content, From = new System.Net.Mail.MailAddress("lichaor@yonyou.com", "U8公共平台"), IsBodyHtml = true };
                for (int i = 0; i < listEmail.Count; i++)
                {
                    if (message.To.Count >= 1)
                    {
                        message.To.RemoveAt(0);
                    }
                    message.To.Add(new System.Net.Mail.MailAddress(listEmail[i]));
                    try
                    {
                        client.Send(message);
                        isPass = true;
                    }
                    catch (Exception ex)
                    {
                        isPass = false;
                    }

                }
            }
            catch (Exception e)
            {
                isPass = false;
            }
            return isPass;
        }

        /// <summary>  
        /// 返回指定目录下所有文件信息  
        /// </summary>  
        /// <param name="strDirectory">目录字符串</param>  
        /// <returns></returns>  
        public static List<FileInfo> GetAllFilesInDirectory(string strDirectory)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            listFiles = GetAllFilesInDirectoryPlugin(strDirectory,listFiles);

            return listFiles;
        }

        private static List<FileInfo> GetAllFilesInDirectoryPlugin(string strDirectory,List<FileInfo> listFiles)
        {
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            DirectoryInfo[] directoryArray = directory.GetDirectories();
            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
            foreach (DirectoryInfo _directoryInfo in directoryArray)
            {
                #region old wait to delete
                //DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
                //DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
                //FileInfo[] fileInfoArrayA = directoryA.GetFiles();
                //if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
                #endregion

                GetAllFilesInDirectoryPlugin(_directoryInfo.FullName,listFiles);//递归遍历  
            }

            return listFiles;
        }

        #endregion

        #region 暂存代码到xml文件中，执行时读取xml文件

        /// <summary>
        /// 根据.cs文件生成对应xml文件
        /// 必须写方法注释才可以正确使用，因为注释是该方法的分隔符
        /// 使用时直接将控件的.cs文件放在xml文件夹下就可以了
        /// </summary>
        /// <param name="filePath"></param>
        public static void GenerateXmlDocument(string filePath)
        {
            //获取指定目录下的所有文件
            List<FileInfo> listFileInfo = GetAllFilesInDirectory(filePath);
            for (int i = 0; i < listFileInfo.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version='1.0'?><root>");
                string key = "";
                string value = "";
                //只针对.cs文件生成对应xml文件
                if (listFileInfo[i].Extension == ".cs")
                {
                    //FileStream fs = listFileInfo[i].OpenRead();

                    //将一个文件的内容读取到str中
                    string contents = File.ReadAllText(listFileInfo[i].DirectoryName + @"\" + listFileInfo[i].Name).Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
                    //将文件内容按照 "/// <param name="e"></param>"进行分割，得到一个数组
                    Regex regex = new Regex("///<paramname=\"e\"></param>");
                    string[] funcArray = regex.Split(contents);

                    //循环遍历数组得到方法名为key,方法体为value，并拼接xml，赋值给sb
                    //注意索引从1而不是0开始，因为"///<paramname=\"e\"></param>"之前没有privatevoid的方法
                    for (int j = 1; j < funcArray.Length; j++)
                    {
                        //去掉字符串中的空格
                        funcArray[j] = funcArray[j].Replace(" ", "");

                        //获取privatevoid开头的索引
                        int indexStartKey = funcArray[j].IndexOf("privatevoid");

                        //获取_的索引
                        int endIndexKey = funcArray[j].IndexOf("_");

                        //两者之间的就是key值
                        key = funcArray[j].Substring(indexStartKey + 11, endIndexKey - indexStartKey - 11);


                        //获取EventArgse){的索引
                        int indexStartValue = funcArray[j].IndexOf("EventArgse){");

                        //获取}///<summary>的索引
                        int endIndexValue = funcArray[j].IndexOf("}///<summary>");


                        //对于最后一个方法要特殊处理，因为最后一个方法后边不再有"}///<summary>"
                        if (j == funcArray.Length - 1)
                        {
                            value = funcArray[j].Substring(indexStartValue + 12, funcArray[j].Length - indexStartValue - 15);
                        }
                        else
                        {
                            value = funcArray[j].Substring(indexStartValue + 12, endIndexValue - indexStartValue - 12);
                        }

                        sb.Append("<" + key + ">" + value + "</" + key + ">");
                    }
                }
                else
                {
                    //非cs文件直接跳过
                    continue;
                }

                sb.Append("</root>");
                //将sb写入到xml文件中，文件名为相应类别的控件名
                File.WriteAllText(listFileInfo[i].DirectoryName + @"\" + listFileInfo[i].Name.Replace(".cs", "") + ".xml", sb.ToString());

                //生成完.xml文件后删除.cs文件
                File.Delete(listFileInfo[i].DirectoryName + @"\" + listFileInfo[i].Name);

                //用完StringBuilder要清空
                sb.Length = 0;
            }
        }

        /// <summary>
        /// 获取实时代码
        /// </summary>
        /// <param name="fullFileName">文件完全路径</param>
        /// <param name="key">key值</param>
        /// <returns></returns>
        public static string ReadValueByXml(string fullFileName, string key)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fullFileName);//xmlPath为xml文件路径
            XmlNode xmlNode1 = xmlDoc.SelectSingleNode("/root/" + key);
            string value = xmlNode1.InnerText;

            return value;
        }

        #endregion

        #region 直接分析源代码解决办法

        /// <summary>
        /// 根据传入的控件和事件类型返回绑定到该控件指定事件上的所有方法名
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="EventCategory">事件类型</param>
        /// <returns>绑定到指定事件上的所有方法名，方法名以‘;’分隔</returns>
        public static string GetFunctionNames(Control control, EventInfo EventCategory)
        {
            #region old
            ////通过反射和GetInvocationList()方法来获取绑定到指定控件上指定事件的所有方法
            //PropertyInfo propertyInfo = control.GetType().GetProperty("Events", BindingFlags.NonPublic
            //                                                         | BindingFlags.Static | BindingFlags.Instance);
            //EventHandlerList eventHandlerList = propertyInfo.GetValue(control, new object[] { }) as EventHandlerList;

            //FieldInfo fieldInfo;

            //CheckBox chbox = control as CheckBox;
            //if (chbox != null)
            //{
            //    fieldInfo = typeof(CheckBox).GetField("EVENT_" + EventCategory.ToUpper(), BindingFlags.NonPublic
            //                                                        | BindingFlags.Static);
            //}
            //else
            //{
            //    fieldInfo = typeof(Control).GetField("Event" + EventCategory, BindingFlags.NonPublic
            //                                                        | BindingFlags.Static);
            //}

            //if (fieldInfo != null)
            //{
            //    var eventKey = fieldInfo.GetValue(control);
            //    var eventHandler = eventHandlerList[eventKey] as Delegate;
            //    if (eventHandler == null)
            //    {
            //        return "";
            //    }
            //    Delegate[] invocationList = eventHandler.GetInvocationList();

            //    StringBuilder sb = new StringBuilder();

            //    foreach (var handler in invocationList)
            //    {
            //        sb.Append(handler.GetMethodInfo().Name + ";");
            //    }
            //    return sb.ToString().TrimEnd(';');
            //}
            //else
            //{
            //    return "";
            //}
            #endregion

            #region new
            PropertyInfo propertyInfo = control.GetType().GetProperty("Events", BindingFlags.NonPublic
                                                                    | BindingFlags.Static | BindingFlags.Instance);
            EventHandlerList eventHandlerList = propertyInfo.GetValue(control, new object[] { }) as EventHandlerList;

            FieldInfo fieldInfo;
            StringBuilder sb = new StringBuilder();

            //EventInfo[] events = control.GetType().GetEvents();

            if (Array.IndexOf(UpperAndUnderLinePatternArray, EventCategory.Name)!=-1)
            {
                //递归（拿到fieldinfo）
                //ex:checkedListBox->ListBox->ListControl->Control
                fieldInfo = GetFieldInfo(control.GetType(), EventCategory);
            }
            else
            {
                if (Array.IndexOf(NoChangedPatternArray,EventCategory.Name)!=-1)
                {
                    fieldInfo = typeof(Control).GetField("Event" + EventCategory.Name.Replace("Changed", ""), BindingFlags.NonPublic | BindingFlags.Static);
                }
                else
                {
                    fieldInfo = typeof(Control).GetField("Event" + EventCategory.Name, BindingFlags.NonPublic | BindingFlags.Static);

                }
            }

            if (fieldInfo != null)
            {
                var eventKey = fieldInfo.GetValue(control);
                var eventHandler = eventHandlerList[eventKey] as Delegate;
                if (eventHandler == null)
                {
                    return "";
                }
                Delegate[] invocationList = eventHandler.GetInvocationList();

                foreach (var handler in invocationList)
                {

                    #region 加入对访问修饰符和返回类型的考虑

                    //命名空间
                    string methodNameSpace = handler.Method.ReflectedType.Namespace.ToString();

                    //类名
                    string methodClassName = handler.Method.ReflectedType.Name;

                    //访问修饰符
                    string accessModifier = GetAccessModifierByMethodinfo(handler.Method);

                    //返回类型
                    string returnType = handler.Method.ReturnType.Name.ToLower();

                    //方法名称
                    string methodName = handler.Method.Name;

                    sb.Append(methodNameSpace+"."+methodClassName+"."+ accessModifier+"."+returnType+"."+methodName + ";");

                    #endregion

                    //sb.Append(handler.GetMethodInfo().Name + ";");
                }
                return sb.ToString().TrimEnd(';');
            }
            else
            {
                return "";
            }
            #endregion

        }

        /// <summary>
        /// 递归查找FiledInfo
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="eventinfo">事件种类</param>
        /// <returns></returns>
        private static FieldInfo GetFieldInfo(Type type, EventInfo eventinfo)
        {
            FieldInfo fieldInfo = type.GetField("EVENT_" + eventinfo.Name.ToUpper(), BindingFlags.NonPublic | BindingFlags.Static);

            if (fieldInfo == null && type.BaseType.ToString() != "System.ComponentModel.Component")
            {
                fieldInfo = GetFieldInfo(type.BaseType, eventinfo);
            }

            return fieldInfo;
        }

        /// <summary>
        /// 获得本类下某个控件的某个事件绑定的所有方法的代码
        /// </summary>
        /// <param name="functionNames">绑定的所有函数名，函数名以";"分隔</param>
        /// <param name="CotrolCategory">分类名称</param>
        /// <returns></returns>
        public static string GetFunctionBodys(string functionNames, string CotrolCategory,bool codeIsModified)
        {
            if (string.IsNullOrEmpty(functionNames))
            {
                return "";
            }

            StringBuilder sbCodes = new StringBuilder();

            string categroyPath= Config.GetValueByKey(CotrolCategory, "categoryPath");

            //获取源码路径
            string filePath = "";
            if (codeIsModified)
            {
                filePath = AppDomain.CurrentDomain.BaseDirectory + categroyPath + @"源码_修改\";
            }
            else
            {
                filePath = AppDomain.CurrentDomain.BaseDirectory + categroyPath + @"源码\";
            }
           

            //获取指定目录下的所有文件
            List<FileInfo> listFileInfo = GetAllFilesInDirectory(filePath);
            for (int i = 0; i < listFileInfo.Count; i++)
            {
                //只针对.cs文件
                if (listFileInfo[i].Extension == ".cs")
                {
                    //将一个文件的内容读取到str中
                    string contents = File.ReadAllText(listFileInfo[i].DirectoryName + @"\" + listFileInfo[i].Name).Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");


                    //分隔一个控件上指定事件绑定的所有方法名，得到方法名数组
                    if (functionNames.Contains(";"))
                    {
                        string[] functionArray = functionNames.Split(';');


                        //循环查出每一个绑定方法的方法体，并控制输出格式
                        for (int j = 0; j < functionArray.Length; j++)
                        {
                            string[] tempArray = functionArray[j].Split('.');

                            string methodNameSpace = tempArray[0];
                            string methodClassName = tempArray[1];
                            string accessModifiler = tempArray[2];
                            string returnType = tempArray[3];

                            sbCodes.Append("//" + "第" + (j + 1).ToString() + "个绑定的方法：" + "\r\n"+
                                           "//" + "命名空间:" + methodNameSpace + "\r\n" +
                                           "//" + "所在类类名:" + methodClassName + "\r\n" +
                                           "//--------------------" +"\r\n"+
                                            accessModifiler +" "+returnType+" "+ GetCodesByFunctionName(contents, tempArray[4])+
                                           "//--------------------" +
                                           "\r\n" + "\r\n");

                        }
                    }
                    else
                    {
                        string[] functionArray = new string[1];
                        string[] tempArray = functionNames.Split('.');
                        functionArray[0] = tempArray[4];

                        string methodNameSpace = tempArray[0];
                        string methodClassName = tempArray[1];
                        string accessModifiler = tempArray[2];
                        string returnType = tempArray[3];

                        string codes = GetCodesByFunctionName(contents, functionArray[0]);
                        if (!string.IsNullOrEmpty(codes))
                        {
                            sbCodes.Append("//" + "绑定的方法：" + "\r\n"+
                                            "//" + "命名空间:" +methodNameSpace+"\r\n"+
                                            "//" + "所在类类名:"+methodClassName+"\r\n"+
                                            "//--------------------" + "\r\n" +
                                            accessModifiler +" "+returnType+" "+ codes +
                                            "//--------------------" +
                                            "\r\n" + "\r\n");
                        }

                    }
                }
            }

            return sbCodes.ToString();
        }

        /// <summary>
        /// 通过函数名和代码文件中的文字定位函数体
        /// </summary>
        /// <param name="contents">源码文件中的文字</param>
        /// <param name="functionName">函数名</param>
        /// <returns></returns>
        private static string GetCodesByFunctionName(string contents, string functionName)
        {
            //通过正则表达式匹配出“函数名(obejct sender,EventArgs e)”
            //找出函数名开始到给定文字结尾的代码。
            //可以有任意多个空格
            Regex regex = new Regex(functionName + @"\s*\(\s*object\s*sender\s*,\s*EventArgs\s*e\s*\)");
            string[] contensArray = regex.Split(contents);
            //没有找到对应的方法，直接返回空
            if (contensArray.Length < 2)
            {
                return "";
            }
            contents = regex.Split(contents)[1];

            StringBuilder sb = new StringBuilder();

            Stack stack = new Stack();

            //循环操作每一个字符
            foreach (char cPut in contents)
            {
                sb.Append(cPut.ToString());
                switch (cPut)
                {
                    //左大括号就压栈
                    case '{':
                        stack.Push(cPut);
                        break;
                    //右大括号就出栈
                    case '}':
                        stack.Pop();
                        //栈中没有东西的时候说明函数体已经找完全，返回找到的函数体
                        if (stack.Count == 0)
                        {
                            return functionName + "(object sender, EventArgs e)" +"\r\n" + sb.ToString();
                        }
                        break;
                    default:
                        break;
                }
            }
            return "";
        }

        #endregion

        /// <summary>
        ///  使用代码编译解决方案
        /// </summary>
        /// <param name="solutionPath">解决方案路径</param>
        /// <returns></returns>
        public static bool CompileSolution(string solutionPath)
        {
            try
            {
                //在HKEY_LOCAL_MACHINE路径下找
                RegistryKey rkey = Registry.LocalMachine;

                //找到devenv.exe路径
                RegistryKey rkey1 = rkey.OpenSubKey(REGISTKEY, false);
                string devenvPath = rkey1.GetValue("").ToString();

                ProcessStartInfo startInfo = new ProcessStartInfo(devenvPath);
                startInfo.Arguments = "/rebuild \"debug|x86\" " + solutionPath;

                //执行编译过程
                Process process = new Process();
                process.StartInfo = startInfo;

                process.Start();
                process.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        /// <summary>
        /// 拷贝文件（如果已有同名文件，替换旧的文件）
        /// </summary>
        /// <param name="sourceFullNamePath">源文件路径</param>
        /// <param name="targetFullNamePath">目标文件路径</param>
        /// <returns></returns>
        public static bool CopyFile(string sourceFullNamePath, string targetFullNamePath)
        {
            try
            {
                File.Copy(sourceFullNamePath, targetFullNamePath, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据给定的MethodInfo信息获取方法的访问修饰符
        /// </summary>
        /// <param name="methodinfo">MethodInfo信息</param>
        /// <returns>访问修饰符（string类型）</returns>
        public static string GetAccessModifierByMethodinfo(MethodInfo methodinfo)
        {
            if (methodinfo.IsPrivate)
            {
                return "private";
            }
            else if (methodinfo.IsPublic)
            {
                return "public";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 读取文件到byte数组中
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static byte[] GetByteArrayFromFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            long len = fi.Length;
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, (int)len);
            fs.Close();

            return buffer;
        }

        /// <summary>
        /// 根据给定的FileInfo获取文件组中最新的文件
        /// </summary>
        /// <param name="listFile"></param>
        /// <returns></returns>
        public static FileInfo GetLatelyFile(List<FileInfo> listFile)
        {
            FileInfo temp=listFile[0];
            for (int i = 0; i < listFile.Count-1; i++)
            {
                if (temp.LastWriteTime< listFile[i + 1].LastWriteTime)
                {
                    temp = listFile[i+1];
                }
            }

            return temp;
        }


        /// <summary>
        /// 获取文件内部被替换后的文本内容
        /// </summary>
        /// <param name="contents">原始文本内容</param>
        /// <param name="functionName">要操作的函数名</param>
        /// <param name="sourceCodes">替换后的函数体代码</param>
        /// <returns></returns>
        private static string GetReplaceCodesByFunctionName(string contents, string functionName,string sourceCodes,string accessModifiler,string returnType)
        {
            //通过正则表达式匹配出“函数名(obejct sender,EventArgs e)”
            //找出函数名开始到给定文字结尾的代码。
            //可以有任意多个空格
            string pattern = accessModifiler+ @"\s*"+returnType+ @"\s*"+ functionName + @"\s*\(\s*object\s*sender\s*,\s*EventArgs\s*e\s*\)";
            Regex regex = new Regex(pattern);
            string[] contensArray = regex.Split(contents);
            //没有找到对应的方法，直接返回空
            if (contensArray.Length < 2)
            {
                return contents;
            }
            string contentsResult = regex.Split(contents)[1];

            StringBuilder sb = new StringBuilder();

            Stack stack = new Stack();

            //循环操作每一个字符
            foreach (char cPut in contentsResult)
            {
                sb.Append(cPut.ToString());
                switch (cPut)
                {
                    //左大括号就压栈
                    case '{':
                        stack.Push(cPut);
                        break;
                    //右大括号就出栈
                    case '}':
                        stack.Pop();
                        //栈中没有东西的时候说明函数体已经找完全，返回找到的函数体
                        if (stack.Count == 0)
                        {
                            //return functionName + "(object sender,EventArgs e)" + "\r\n" + sb.ToString();
                            string findCodes = Regex.Replace(sb.ToString(), @"\s+", " ").Replace("\r\n", "");//.Replace(" ","\\s*")
                            string myPattern = accessModifiler + " " + returnType + " " + functionName + "(object sender, EventArgs e)";
                            sourceCodes = Regex.Replace(sourceCodes.Replace("\r\n", ""), @"\s+", " ");
                            //return Regex.Replace(Regex.Replace(contents.Replace("\r\n", ""), @"\s+", " "), pattern+ findCodes, sourceCodes); 
                            //Regex myRegex = new Regex(pattern+findCodes);
                            //return myRegex.Replace(Regex.Replace(contents.Replace("\r\n", ""), @"\s+", " "), sourceCodes);
                            return  Regex.Replace(contents.Replace("\r\n", ""), @"\s+", " ").Replace( myPattern+ findCodes, sourceCodes);
                        }
                        break;
                    default:
                        break;
                }
            }
            return "";
        }

        public static bool ReplaceSourceDoucmentCodes(string functionName, string CotrolCategory,string sourceCodes,string accessModifiler, string returnType)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                return true;
            }

            try
            {
                string categroyPath = Config.GetValueByKey(CotrolCategory, "categoryPath");

                //获取源码_修改路径
                string filePath = AppDomain.CurrentDomain.BaseDirectory + categroyPath + @"源码_修改\";

                //获取指定目录下的所有文件
                List<FileInfo> listFileInfo = GetAllFilesInDirectory(filePath);
                for (int i = 0; i < listFileInfo.Count; i++)
                {
                    //只针对.cs文件
                    if (listFileInfo[i].Extension == ".cs")
                    {
                        //将一个文件的内容读取到str中
                        string contents = File.ReadAllText(listFileInfo[i].DirectoryName + @"\" + listFileInfo[i].Name);

                        //返回替换后的contents
                        contents = Common.GetReplaceCodesByFunctionName(contents, functionName, sourceCodes, accessModifiler, returnType);

                        File.WriteAllText(listFileInfo[i].FullName, contents);

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region 文件拷贝
        public static void copyDirectory(string sourceDirectory, string destDirectory)
        {
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            copyFile(sourceDirectory, destDirectory);

            //拷贝子目录       
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);

            foreach (string directionPath in directionName)
            {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);

                //递归下去
                copyDirectory(directionPath, directionPathTemp);
            }
        }
        public static void copyFile(string sourceDirectory, string destDirectory)
        {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory);

            foreach (string filePath in fileName)
            {
                //根据每个文件名称生成对应的目标文件名称
                string filePathTemp = destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);

                //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp))
                {
                    File.Copy(filePath, filePathTemp, true);
                }
                else
                {
                    File.Copy(filePath, filePathTemp);
                }
            }
        }
        #endregion
    }
}
