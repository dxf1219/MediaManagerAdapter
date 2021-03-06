﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;
using System.Collections;
using System.Net;
using System.Web.Script.Serialization;
using StackExchange.Redis;
using MediaInfoLib;
using System.Text.RegularExpressions;

namespace XnewsAdapter
{
    public partial class Form_main : Form
    {
        public Form_main()
        {
            InitializeComponent();

            logpath = Application.StartupPath + "\\logs";
            
            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(logpath);
            }

            if (!Directory.Exists(Application.StartupPath + "\\mediainfo"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\mediainfo");
            }
          
            if ( ! Directory.Exists( Application.StartupPath + "\\arcpreset") )
            {
                Directory.CreateDirectory(Application.StartupPath + "\\arcpreset");
            }

            if (!Directory.Exists(Application.StartupPath + "\\counts"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\counts");
            }

            if (!Directory.Exists(Application.StartupPath + "\\dealxml"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\dealxml");
            }
            htrelation = new Hashtable();
            WriteLogNew.writeLog("软件启动!",logpath,"info");
            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"软件启动!\n");

            smstextpath = Properties.Settings.Default.smspath;
            mobilephones = new List<string>();

            foreach (string phone in Properties.Settings.Default.mobilePhones)
            {
                mobilephones.Add(phone);
            }
            mediaxml = new MediaInfoXmlClass();
            try
            {
                redis = ConnectionMultiplexer.Connect(Properties.Settings.Default.redisConnstring);
                WriteLogNew.writeLog("redis连接成功!" + Properties.Settings.Default.redisConnstring, logpath, "info");
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("redis连接失败!" + Properties.Settings.Default.redisConnstring + ee.ToString(), logpath, "error");
                MessageBox.Show("redis连接失败!" + Properties.Settings.Default.redisConnstring + ee.ToString());
                
            }

            timer_check.Enabled = false;

            this.Text = Properties.Settings.Default.AppTitle;

            xftpin = new XftpInfo();

            //读取xml 
            #region 读取avidin xml
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "\\mmavidinconfig.xml");
            System.Xml.XmlElement root = doc.DocumentElement;

            XmlNode srcFromNode = root.SelectSingleNode("//srcFrom");
            xftpin.SrcFrom = srcFromNode.InnerText;

            XmlNode nsmlOutputPathNode = root.SelectSingleNode("//nsmlOutputPath");
            xftpin.NsmlOutputPath = nsmlOutputPathNode.InnerText;

            XmlNode picOutputPathNode = root.SelectSingleNode("//picOutputPath");
            xftpin.PicOutputPath = picOutputPathNode.InnerText;

            XmlNode transcodeFileInPathNode = root.SelectSingleNode("//transcodeFileInPath");
            xftpin.TranscodeFileInPath = transcodeFileInPathNode.InnerText;

            XmlNode transcodeFileOutPathNode = root.SelectSingleNode("//transcodeFileOutPath");
            xftpin.TranscodeFileOutPath = transcodeFileOutPathNode.InnerText;


            XmlNode mediaXmlOutputPathNode = root.SelectSingleNode("//mediaXmlOutputPath");
            xftpin.MediaXmlPath = mediaXmlOutputPathNode.InnerText;

            #endregion

            //读取inewsRelation xml 
            readRelationXml();

            WriteLogNew.writeLog("读取配置信息成功！", logpath, "info");
            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"读取配置信息成功！\n");

            timer_check.Interval = Properties.Settings.Default.checkStatusInterval;

            timer_check.Enabled = true;

            scriptThread = new Thread(new ThreadStart(scanScriptThread));
            scriptThread.IsBackground = true;
            scriptThread.Start();


            videoThread = new Thread(new ThreadStart(scanVideoThread));
            videoThread.IsBackground = true;
            videoThread.Start();

        }

        #region 变量定义
        private string logpath;
        private XftpInfo xftpin;
        private Hashtable htrelation;
        private Thread scriptThread;
        private Thread videoThread;
        private string smstextpath;
        private List<string> mobilephones;
        private ConnectionMultiplexer redis;
        private MediaInfoXmlClass mediaxml = null;
        #endregion

        #region 消息框代理
        private delegate void SetTextCallback(string text);
        private delegate void SetSelectCallback(object Msge);
        private void SetText(string text1)
        {
            string text = text1;
            try
            {
                if (this.richTextBox1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    if (this.richTextBox1.Lines.Length < Properties.Settings.Default.textclearLenth)
                    {
                        this.richTextBox1.AppendText(text);
                        of_SetRichCursor(richTextBox1);
                    }
                    else
                    {
                        this.richTextBox1.Clear();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void of_SetRichCursor(object msge)
        {
            try
            {
                RichTextBox richbox = (RichTextBox)msge;
                //设置光标的位置到文本尾
                if (richbox.InvokeRequired)
                {
                    SetSelectCallback d = new SetSelectCallback(of_SetRichCursor);
                    this.Invoke(d, new object[] { msge });
                }
                else
                {
                    richbox.Select(richbox.TextLength, 0);
                    //滚动到控件光标处
                    richbox.ScrollToCaret();
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        private void readRelationXml()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Application.StartupPath + "\\relation.xml");
                System.Xml.XmlElement root = doc.DocumentElement;
                XmlNodeList relationlist = root.SelectNodes("//relation"); //relation serverlist
                foreach (XmlNode relnode in relationlist)
                {
                    XmlNode keynode = relnode.FirstChild;
                    XmlNode valuenode = keynode.NextSibling;
                    htrelation.Add(keynode.InnerText, valuenode.InnerText);
                }
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("读取MManger-AVID对应关系出错!"+ee.ToString(),logpath,"error");
            }

        }
       
        /// <summary>
        /// 虹软转码
        /// </summary>
        /// <param name="arcpreset"></param>
        /// <returns>transcode id </returns>
        public ResultInfo arcTranscode(string arcrestapi, string arcpreset)  //0 成功 -1 失败
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                Uri address = new Uri(arcrestapi + "/task/launch?appId=" + Properties.Settings.Default.arcappid);
                // Create the web request  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                // Set type to POST  
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                request.ContentType = "text/xml";
                string data = "";

                StreamReader sr = new StreamReader(arcpreset, Encoding.UTF8);
                while (!sr.EndOfStream)
                {
                    data += sr.ReadLine();
                }
                sr.Close();

                // Create a byte array of the data we want to send  
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                //WriteLogNew.writeLog("send transcode cmd to arc :"+data.ToString(),logpath);

                // Set the content length in the request headers  
                request.ContentLength = byteData.Length;

                // Write data  
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                    WriteLogNew.writeLog("下发任务到转码! ", logpath, "info");
                    SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"下发任务到转码!\n ");
                    //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 下发任务到转码! \n");
                }

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    // Console application output  
                    string strreturn = reader.ReadToEnd();
                    //WriteLogNew.writeLog("转码返回:"+strreturn,logpath,"info");
                    //成功xml 
                    XmlDocument doctrans = new XmlDocument();
                    doctrans.LoadXml(strreturn);
                    XmlElement root = doctrans.DocumentElement;
                    if (root.Name.Equals("errors"))
                    {
                        WriteLogNew.writeLog("arc transcode failed !", logpath, "error");
                        ri.Code = -1;
                        ri.ErrorMsg = "error";
                        return ri;
                    }
                    else
                    {
                        WriteLogNew.writeLog("arc transcode success!" , logpath, "info");
                        string jobtype = root.Attributes["href"].Value;

                        string[] ids = jobtype.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        //获取转码进度的id 
                        string pid = ids[ids.Length - 1];
                        ri.Code = 1;
                        ri.Info = pid;
                        return ri;

                    }
                }
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("arc send transcode cmd error!" + ee.ToString(), logpath, "error");
                ri.Code = -1;
                ri.ErrorMsg = ee.ToString();
                return ri;
            }
        }
        /// <summary>
        /// 虹软转码进度
        /// </summary>
        /// <param name="arcrestapi"></param>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public ResultInfo arcProcess(string arcrestapi, string ppid) //100 完成 -1 失败
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                Uri address = new Uri(arcrestapi + "/task/" + ppid + "/progress");
                // Create the web request  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                request.Method = "GET";
                //request.ContentType = "application/x-www-form-urlencoded";
                request.ContentType = "text/xml";

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    // Console application output  
                    string strreturn = reader.ReadToEnd();

                    XmlDocument docprocess = new XmlDocument();
                    docprocess.LoadXml(strreturn);
                    XmlElement root = docprocess.DocumentElement;
                    //判断是否出现异常
                    if (root.Name.Equals("errors"))
                    {
                        XmlNode error = docprocess.SelectSingleNode("/errors/error");
                        WriteLogNew.writeLog("转码出错信息:" + error.InnerText, logpath, "error");
                        ri.Code = -2;
                        ri.ErrorMsg = "转码出错error!";
                        return ri; //出错了
                    }
                    XmlNode status = docprocess.SelectSingleNode("/results/result/status");

                    if (status.InnerText.Equals("COMPLETED"))
                    {
                        ri.Code = 1;
                        ri.Info = "100";
                        return ri;
                    }
                    else if (status.InnerText.Equals("RUNNING"))
                    {
                        XmlNode processnode = docprocess.SelectSingleNode("/results/result/progress/input");

                        string process = processnode.InnerText;

                        //WriteLogNew.writeLog("转码进度:" + process,logpath, "info");

                        // SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "转码进度:" + process + "\n");

                        try
                        {
                            ri.Code = 1;
                            ri.Info = process;
                            return ri;
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("转换转码进度出错." + process + ee.ToString(), logpath, "error");
                            //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + " 转换转码进度出错:" + process + "\n");
                            ri.Code = -1;
                            ri.ErrorMsg = ee.ToString();
                            return ri;
                        }
                    }
                    else if (status.InnerText.Equals("CANCELLED")) //CANCELLED
                    {
                        ri.Code = -2;
                        ri.ErrorMsg = "CANCELLED";
                        return ri;
                    }
                    else if (status.InnerText.Equals("ERROR"))
                    {
                        ri.Code = -2;
                        ri.ErrorMsg = "ERROR";
                        return ri;
                    }

                }
                ri.Code = -1;
                return ri;
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("send get process error !" + ee.ToString(), logpath, "error");
                ri.Code = -1;
                ri.ErrorMsg = ee.ToString();
                return ri;
            }
        }

        private ResultInfo getxftpinfo(XftpInfo xi, string type) //peek take 
        {
            ResultInfo ri = new ResultInfo();
               
            string strurl = xi.XftpAPI+"/"+type;

            WriteLogNew.writeLog("url:" + strurl, logpath, "info");

       
            try
            {
                SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "url:" + strurl + "\n");
                Uri address = new Uri(strurl);
                // Create the web request  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                // Set type to POST  
                request.Method = "GET";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "text/xml";

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    // Console application output  
                    string strreturn = reader.ReadToEnd();
                    if (type.Equals("peek"))
                    {
                        WriteLogNew.writeLog("返回值:" + strreturn, logpath, "info");
                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "返回值:" + strreturn + "\n");
                    }
                    ri.Code = 1;
                    ri.Info = strreturn;

                }
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("从xftp 获取信息异常!"+ee.ToString(),logpath,"error");
                ri.Code = -1;
                ri.Info = ee.ToString();
            }

            return ri;
            
        }

        /// <summary>
        /// json 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        private T scriptDeserialize<T>(string jsonstr)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<T>(jsonstr);
        }

        private void scanScriptThread()
        {
            while (true)
            {
                try
                {
                    IDatabase db = redis.GetDatabase();
                    string key = this.Text + "live";
                    string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    db.StringSet(key, value);
                }
                catch (Exception ee)
                {
                    WriteLogNew.writeLog("redis 写入key value 异常!" + ee.ToString(), logpath, "error");
                }
                try
                {
                    string[] scriptxmlfiles = Directory.GetFiles(Properties.Settings.Default.scanScriptPath,"*.xml",SearchOption.TopDirectoryOnly);

                    foreach (string scriptfile in scriptxmlfiles)
                    {
                      
                        WriteLogNew.writeLog("开始处理获取的文稿信息！", logpath, "info");
                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "开始处理获取的文稿信息！\n");
                       
                        #region count处理
                        string countfilename = Path.GetFileName(scriptfile);
                        string newcountfile = Application.StartupPath + "\\counts\\" + countfilename + "_count.xml";
                        try
                        {
                            if (!File.Exists(newcountfile))
                            {
                                File.Copy(Application.StartupPath + "\\counts.xml", newcountfile);
                            }

                            XmlDocument doccount = new XmlDocument();
                            doccount.Load(newcountfile);
                            System.Xml.XmlElement rootcount = doccount.DocumentElement;

                            XmlNode countnode = rootcount.SelectSingleNode("/root/counts");
                            string nowcount = countnode.InnerText;
                            int newcount = Convert.ToInt32(nowcount) + 1;
                            countnode.InnerText = newcount.ToString();
                            doccount.Save(newcountfile);

                            WriteLogNew.writeLog("生成count xml！", logpath, "info");

                            if (newcount > Properties.Settings.Default.errorRetryCounts)
                            {
                                WriteLogNew.writeLog("重试次数大于系统设定值，将该节目设置成error！", logpath, "error");
                                //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + "重试次数大于系统设定值，将该节目设置成error!" + Path.GetFileName(xmlfile) + "\n");

                                //将文件设置成已处理
                                continue;
                            } //超过重试次数
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("生成文件统计失败!" + ee.ToString(), logpath, "error");
                            //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + "生成文件统计失败!" + "\n");
                            continue;
                        }
                        #endregion

                        try
                        {
                            //文稿的xml
                            #region inews
                            XmlDocument doc = new XmlDocument();
                            doc.Load(scriptfile);
                            XmlNode root = doc.DocumentElement;
                            XmlNodeList filesNodelist = root.SelectNodes("//file");
                            List<string> mediafiles = new List<string>();
                            if (filesNodelist != null)
                            {
                                foreach (XmlNode filenode in filesNodelist)
                                {
                                    try
                                    {
                                        mediafiles.Add(filenode.FirstChild.InnerText);
                                    }
                                    catch (Exception ee)
                                    {
                                        WriteLogNew.writeLog("读取文稿中的file节点filename异常!" + ee.ToString(), logpath, "error");
                                    }

                                }
                            }
                            XmlNode scriptNode = root.SelectSingleNode("//script");
                            XmlNode txtNode = scriptNode.FirstChild;
                            if (txtNode != null)
                            {
                                while (txtNode != null)
                                {
                                    NXMLFORM nf = new NXMLFORM();
                                    nf.mediafilelist = mediafiles;
                                    XmlNode nsmlnode = txtNode.FirstChild;
                                    while (nsmlnode != null)
                                    {
                                        #region nsml处理
                                        if (nsmlnode.Name.Equals("title"))
                                        {
                                            nf.Title = nsmlnode.InnerText;
                                        }
                                        else if (nsmlnode.Name.Equals("writer"))
                                        {
                                            nf.Writer = nsmlnode.InnerText; 
                                        }
                                        else if (nsmlnode.Name.Equals("cameraman")) //cameraman
                                        {
                                            nf.Cameraman = nsmlnode.InnerText;
                                        }
                                        else if (nsmlnode.Name.Equals("platform"))
                                        {
                                            nf.Platform = nsmlnode.InnerText.Trim();
                                        }
                                        else if (nsmlnode.Name.Equals("site"))
                                        {
                                            nf.Site = nsmlnode.InnerText.Trim();
                                        }
                                        else if (nsmlnode.Name.Equals("v-bumen"))
                                        {
                                            nf.V_bumen = nsmlnode.InnerText.Trim();
                                        }
                                        else if (nsmlnode.Name.Equals("create-by"))
                                        {
                                            nf.Create_by = nsmlnode.InnerText;
                                        }
                                        else if (nsmlnode.Name.Equals("create-date"))
                                        {
                                            nf.Create_date = nsmlnode.InnerText;
                                        }
                         
                                        else if (nsmlnode.Name.Equals("source"))
                                        {
                                            nf.Source = nsmlnode.InnerText;
                                        }
               
                                        else if (nsmlnode.Name.Equals("channel"))
                                        {
                                            nf.Channel = nsmlnode.InnerText;
                                        }
                                        else if (nsmlnode.Name.Equals("txts"))
                                        {
                                            nf.Ntext = nsmlnode.InnerText;
                                        }
                                        #endregion
                                        nsmlnode = nsmlnode.NextSibling;
                                    }//while (nsmlnode != null)

                                    string nsmlname = "X" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".nsml";
                                    //inews路径 优先以部门为主
                                    string inewspath = "04融合平台.06其他";
                                    try
                                    {
                                        inewspath = htrelation["defaultinews"].ToString();
                                    }
                                    catch (Exception ee)
                                    {
                                        WriteLogNew.writeLog("获取default inews目录异常!" + ee.ToString(), logpath, "error");
                                    }
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(nf.V_bumen))
                                        {
                                            inewspath = htrelation[nf.V_bumen].ToString();
                                            WriteLogNew.writeLog("从部门中获取了对应的Inews目录!" + nf.V_bumen + " inews目录:" + inewspath, logpath, "info");
                                            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "从部门中获取了对应的Inews目录!" + nf.V_bumen + " inews目录:" + inewspath + "\n");
                                        }
                                        else
                                        {
                                            //从 platform.site
                                            string pfsite = nf.Platform + "." + nf.Site;
                                            try
                                            {
                                               
                                                if (!string.IsNullOrEmpty(pfsite))
                                                {
                                                    inewspath = htrelation[pfsite].ToString();
                                                    WriteLogNew.writeLog("从platform 中获取了对应的Inews目录!platform：" + pfsite + " inews目录:" + inewspath, logpath, "info");
                                                    SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "从platform site中获取了对应的Inews目录!platform：" + pfsite + " inews目录:" + inewspath + "\n");
                                                }
                                            }
                                            catch (Exception et)
                                            {
                                                WriteLogNew.writeLog("未能从platform site取得对应的inews目录!" + et.ToString() + "platform :" + pfsite, logpath, "error");
                                            }
                                        }
                                    }
                                    catch (Exception ee)
                                    {
                                        WriteLogNew.writeLog("未能获取部门对应的inews目录!" + ee.ToString() + nf.V_bumen, logpath, "error");
                                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "未能获取部门对应的inews目录!" + nf.V_bumen + "\n");
                                        //从 platform.site

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(nf.Platform))
                                            {
                                                inewspath = htrelation[nf.Platform].ToString();
                                                WriteLogNew.writeLog("从platform site中获取了对应的Inews目录!platform：" + nf.Platform + " inews目录:" + inewspath, logpath, "info");
                                                SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "从platform site中获取了对应的Inews目录!platform：" + nf.Platform + " inews目录:" + inewspath + "\n");
                                            }
                                        }
                                        catch (Exception et)
                                        {
                                            WriteLogNew.writeLog("未能从platform site取得对应的inews目录!" + et.ToString() + "platform :" + nf.Platform, logpath, "error");
                                        }
                                    }

                                    if (!Directory.Exists(xftpin.NsmlOutputPath))
                                    {
                                        Directory.CreateDirectory(xftpin.NsmlOutputPath);
                                    }

                                    if (!Directory.Exists(xftpin.NsmlOutputPath + "\\" + inewspath))
                                    {
                                        Directory.CreateDirectory(xftpin.NsmlOutputPath + "\\" + inewspath);
                                    }

                                    nf.Xmloriname = xftpin.NsmlOutputPath + "\\" + inewspath + "\\" + nsmlname;

                                    WriteLogNew.writeLog("目的nsml 目录:" + nf.Xmloriname, logpath, "info");

                                    nf.Form_name = "SMGSTORYNEWFORM";
                                    createNxml(nf);

                                    //生成nsml的路径
                                    string localnsml = Application.StartupPath + "\\localnsml" + "\\" + Path.GetFileNameWithoutExtension(nf.Xmloriname) + ".nsml";

                                    if (File.Exists(localnsml))
                                    {
                                        WriteLogNew.writeLog("生成nsml文件成功！" + nsmlname, logpath, "info");
                                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "生成nsml文件成功！" + nsmlname + "\n");

                                        File.Copy(localnsml, nf.Xmloriname, true);

                                        WriteLogNew.writeLog("复制nsml到目的地成功!" + nf.Xmloriname, logpath, "info");
                                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "复制nsml到目的地成功!" + nf.Xmloriname + "\n");
                                    }
                                    else
                                    {
                                        WriteLogNew.writeLog("本地nsml不存在!" + localnsml, logpath, "info");
                                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "本地nsml不存在!" + localnsml + "\n");
                                    }

                                    txtNode = txtNode.NextSibling;
                                }//while (txtNode != null)
                            }

                  
                            #endregion
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("处理文稿数据异常!" + ee.ToString(), logpath, "error");
                        }
                        //将文件设置成已处理
                        try
                        {
                            //删除文稿文件
                            string localscirptxml = Application.StartupPath + "\\dealxml\\" + Path.GetFileName(scriptfile);
                            File.Copy(scriptfile, localscirptxml, true);
                            WriteLogNew.writeLog("复制处理文稿到本地!" + localscirptxml, logpath, "info");
                            File.Delete(scriptfile);
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("处理文稿数据异常!" + ee.ToString(), logpath, "error");
                        }
                    }  // foreach (string scriptfile in scriptxmlfiles)


                }
                catch (Exception ee)
                {

                    WriteLogNew.writeLog("处理文稿线程异常!"+ee.ToString(),logpath,"error");
                    SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+ "处理文稿线程异常!" + ee.ToString()+"\n");
                    string loginfo = this.Text + " " + Properties.Settings.Default.localIP + " 处理文稿线程异常!" + ee.ToString();
                    createSMStxt(loginfo);
          
                }

                Thread.Sleep(Properties.Settings.Default.scanInterval);

            }
        }

        private void scanVideoThread()
        {
            while (true)
            {
                //处理视频文件
                string[] videoxmlfiles = Directory.GetFiles(Properties.Settings.Default.scanVideoPath, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (string videoxmlfile in videoxmlfiles)
                {
                    //生成count 文件
                    #region count处理
                    string countfilename = Path.GetFileName(videoxmlfile);
                    string newcountfile = Application.StartupPath + "\\counts\\" + countfilename + "_count.xml";
                    try
                    {
                        if (!File.Exists(newcountfile))
                        {
                            File.Copy(Application.StartupPath + "\\counts.xml", newcountfile);
                        }

                        XmlDocument doccount = new XmlDocument();
                        doccount.Load(newcountfile);
                        System.Xml.XmlElement rootcount = doccount.DocumentElement;

                        XmlNode countnode = rootcount.SelectSingleNode("/root/counts");
                        string nowcount = countnode.InnerText;
                        int newcount = Convert.ToInt32(nowcount) + 1;
                        countnode.InnerText = newcount.ToString();
                        doccount.Save(newcountfile);

                        WriteLogNew.writeLog("生成count xml！", logpath, "info");

                        if (newcount > Properties.Settings.Default.errorRetryCounts)
                        {
                            WriteLogNew.writeLog("重试次数大于系统设定值，将该节目设置成error！", logpath, "error");
                            //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + "重试次数大于系统设定值，将该节目设置成error!" + Path.GetFileName(xmlfile) + "\n");

                            //将文件设置成已处理
                            try
                            {
                                File.Delete(videoxmlfile);
                                WriteLogNew.writeLog("删除video xml 文件！"+ videoxmlfile, logpath, "info");
                            }
                            catch (Exception ee)
                            {
                                WriteLogNew.writeLog("删除video xml 文件 异常！" + videoxmlfile +ee.ToString(), logpath, "error");
                            }
                            continue;
                        } //超过重试次数
                    }
                    catch (Exception ee)
                    {
                        WriteLogNew.writeLog("生成文件统计失败!" + ee.ToString(), logpath, "error");
                        //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + "生成文件统计失败!" + "\n");
                        continue;
                    }
                    #endregion

                    #region 视频文件
                    //解析xml 获取素材路径
                    string mediafile = "";
                    string newfilename = "";
                    XmlDocument docvideoxml = new XmlDocument();
                    try
                    {
                        docvideoxml.Load(videoxmlfile);
                        XmlNode mediafilenode = docvideoxml.SelectSingleNode("/root/fileName");
                        mediafile = mediafilenode.InnerText;
                        XmlNode newfilenamenode = docvideoxml.SelectSingleNode("/root/newName");
                        newfilename = newfilenamenode.InnerText;
                    }
                    catch (Exception ee)
                    {
                        WriteLogNew.writeLog("处理视频xml异常!" + videoxmlfile + ee.ToString(), logpath, "error");
                        continue;
                    }
                    //复制xml文件到指定目录
                    try
                    {
                        if (!Directory.Exists(xftpin.MediaXmlPath))
                        {
                            Directory.CreateDirectory(xftpin.MediaXmlPath);
                        }
                        string destmediafile = xftpin.MediaXmlPath + "\\" + Path.GetFileNameWithoutExtension(newfilename) +".xml";
                        File.Copy(videoxmlfile, destmediafile, true);
                        WriteLogNew.writeLog("复制media xml 到指定目录成功!" + destmediafile, logpath, "info");
                    }
                    catch (Exception ee)
                    {
                        WriteLogNew.writeLog("复制media xml 到指定目录异常!" + ee.ToString(), logpath, "error");
                    }
                    //调用mediainfo 
                    //判断该文件是否需要转码 
                    //调用mediainfo 获取素材长度
                    string newfilepath = Properties.Settings.Default.scanVideoPath + "\\" + mediafile;
                    bool ifneedTranscode = true;
                    if (Path.GetExtension(newfilepath).ToLower().Equals(".mxf"))
                    {
                        try
                        {
                            string xmlmedia = mediaxml.of_GetXmlStr(newfilepath);
                            if (xmlmedia.Equals("Not Media File"))
                            {
                                WriteLogNew.writeLog("获取素材媒体mediainfo信息失败！" + newfilepath + "Not Media File!", logpath, "error");
                                //加入短信报警
                                string loginfo = this.Text + " " + Properties.Settings.Default.localIP + " 该文件非视频文件!" + newfilepath;
                                createSMStxt(loginfo);
                                continue;
                            }
                            else
                            {
                                if (xmlmedia.Contains("error"))
                                {
                                    WriteLogNew.writeLog("获取素材媒体mediainfo信息出错！" + xmlmedia, logpath, "error");
                                    //加入短信报警
                                    string loginfo = this.Text + " " + Properties.Settings.Default.localIP + " 获取素材媒体mediainfo信息出错！" + newfilepath;
                                    createSMStxt(loginfo);
                                    continue;
                                } //if (xmlmedia.Contains("error"))
                                else
                                {
                                    string newxmlmediainfo = replaceSpecialXMLSyntax(xmlmedia);
                                    if (!xmlmedia.Equals(newxmlmediainfo))
                                    {
                                        WriteLogNew.writeLog("获取素材媒体mediainfo信息中含有特殊字符:" + xmlmedia, logpath, "info");
                                        xmlmedia = newxmlmediainfo;
                                        WriteLogNew.writeLog("newmediinfoxml:" + xmlmedia, logpath, "info");
                                    }

                                    System.Xml.XmlDocument docmediainfo = new XmlDocument();
                                    docmediainfo.LoadXml(xmlmedia);
                                    //FrameRate
                                    XmlNode xmlnodeFrameRate = docmediainfo.SelectSingleNode("//item[@Name='FrameRate']");
                                    if (xmlnodeFrameRate != null)
                                    {
                                        if (xmlnodeFrameRate.InnerText.Equals("25.00"))
                                        {

                                            XmlNode xmlnodeFormat_Commercial = docmediainfo.SelectSingleNode("//item[@Name='Format_Commercial']");
                                            if (xmlnodeFormat_Commercial != null)
                                            {
                                                if (xmlnodeFormat_Commercial.InnerText.Equals(Properties.Settings.Default.avidcoder))
                                                {
                                                    //和avid支持的编码格式一致
                                                    ifneedTranscode = false;
                                                    WriteLogNew.writeLog("该素材和avid编码格式一致不需要转码!", logpath, "info");
                                                }//编码一致
                                            }//编码xmlnode非空
                                        }
                                    } //可以获取帧率

                                    //保存该mediainfo xml
                                    string mediainfoxml = Application.StartupPath + "\\mediainfo\\" + Path.GetFileNameWithoutExtension(newfilepath) + ".xml";
                                    if (File.Exists(mediainfoxml))
                                    {
                                        WriteLogNew.writeLog("素材媒体mediainfo文件存在：" + mediainfoxml + " 准备删除！", logpath, "info");
                                        File.Delete(mediainfoxml);
                                        WriteLogNew.writeLog("删除:" + mediainfoxml, logpath, "info");
                                    }
                                    docmediainfo.Save(mediainfoxml);
                                    WriteLogNew.writeLog("保存mediainfoxml：" + mediainfoxml, logpath, "info");
                                }
                            }  //mediainfo获取信息成功

                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("调用mediainfo异常！" + newfilepath + ee.ToString(), logpath, "error");
                            //加入短信报警
                            continue;
                        }
                    }  //扩展名为mxf
                    else
                    {
                        WriteLogNew.writeLog("非avid直接导入格式！" + newfilepath , logpath, "info");
                    }

                    //认为是视频文件 下发任务到转码
                    //需要视频文件是否为xdcam 50Mb/s的素材
                    //调用虹软转码转成50Mb/s MXF文件
                    bool ifvideosuccess = true; 

                    if (ifneedTranscode)
                    {
                        string destxmlpreset = Application.StartupPath + "\\arcpreset\\" + Path.GetFileNameWithoutExtension(videoxmlfile) + ".xml";

                        File.Copy(Application.StartupPath + "\\" + Properties.Settings.Default.arcProfile, destxmlpreset, true);
                        XmlDocument docarcpreset = new XmlDocument();

                        docarcpreset.Load(destxmlpreset);

                        XmlElement rootarcpreset = docarcpreset.DocumentElement;

                        //已经把transcode output name 修改了
                        XmlNode tasknameNode = docarcpreset.SelectSingleNode("/task/name");
                        string arctitle = newfilename;

                        tasknameNode.InnerText = arctitle + "_ding";

                        XmlNode localuri = docarcpreset.SelectSingleNode("/task/inputs/localfile/uri");

                        try
                        {
                            string linuxfilepath = mediafile.Replace("\\", "/");
                            localuri.InnerText = xftpin.TranscodeFileInPath + "/" + linuxfilepath;

                            XmlNode outputuri = docarcpreset.SelectSingleNode("/task/outputgroups/filearchive/uri");
                            outputuri.InnerText = xftpin.TranscodeFileOutPath;

                            XmlNode outputname = docarcpreset.SelectSingleNode("/task/outputgroups/filearchive/targetname");
                            outputname.InnerText = newfilename ;

                            docarcpreset.Save(destxmlpreset);
                            WriteLogNew.writeLog("下发任务到转码!" + arctitle, logpath, "info");
                            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "下发任务到转码!" + arctitle + "\n");

                            ArcParam aparm = new ArcParam();
                            aparm.Apiurl = Properties.Settings.Default.arcTranscodeAPI;
                            WriteLogNew.writeLog("转码IP:" + aparm.Apiurl, logpath, "info");
                            aparm.Paramxml = destxmlpreset;
                            aparm.Clipinfo = localuri.InnerText;

                            ThreadPool.QueueUserWorkItem(new WaitCallback(sendArcTranscodeThread), aparm);

                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("转码异常!" + ee.ToString(), logpath, "error");
                            ifvideosuccess = false;
                        }
                    }
                    else
                    {
                        //不需要转码 需要将素材搬运到avidingest的目录
                        string destfile = xftpin.TranscodeFileOutPath + "\\" + newfilename + ".mxf";
                        try
                        {
                            WriteLogNew.writeLog("开始复制video文件!" + Path.GetFileName(newfilepath), logpath, "info");
                           
                            File.Copy(newfilepath, destfile, true);
                            WriteLogNew.writeLog("复制video文件完成!" + destfile, logpath, "info");
                            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "复制video文件完成!" + destfile + "\n");

                            //生成md5sum文件
                            File.Copy(Application.StartupPath+"\\1.md5sum", xftpin.TranscodeFileOutPath + "\\" + newfilename+".mxf.md5sum");
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("复制文件出错!" + newfilepath +ee.ToString(), logpath, "error");
                            ifvideosuccess = false;
                        }
                    }//不需要转码


                    if (ifvideosuccess)
                    {
                        try
                        {
                            string localvideoxml = Application.StartupPath + "\\dealxml\\" + Path.GetFileName(videoxmlfile);
                            File.Copy(videoxmlfile, localvideoxml, true);
                            WriteLogNew.writeLog("复制处理视频xml到本地!" + localvideoxml, logpath, "info");

                            File.Delete(videoxmlfile);
                            WriteLogNew.writeLog("删除videoxml!" + videoxmlfile , logpath, "info");
                            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "处理完成,删除videoxml!" + videoxmlfile + "\n");
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("删除videoxml 异常:" + videoxmlfile + ee.ToString(), logpath, "error");
                        }
                    }
                    #endregion

                } //foreach (string videoxmlfile in videoxmlfiles)

                Thread.Sleep(Properties.Settings.Default.scanInterval);
            }

        }
        private void createSMStxt(string loginfo)
        {
            //生成txt短信报警
            //生成短信txt  //第一行手机号码 第二行内容
            try
            {
                string smspath = "";

                if (!Directory.Exists(smstextpath))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\smstxt");
                    smspath = Application.StartupPath + "\\smstxt" + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                }
                else
                {
                    smspath = smstextpath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                }

                System.IO.StreamWriter swfn = System.IO.File.AppendText(smspath);
                string writelinetxt = "";
                foreach (string mphone in mobilephones)
                {
                    string[] mp = mphone.Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries); //号码；姓名
                    writelinetxt = mp[0] + ";" + writelinetxt;
                }
                swfn.WriteLine(writelinetxt);
                swfn.WriteLine(loginfo);
                swfn.Flush();
                swfn.Close();
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("生成短信异常!"+ee.ToString(),logpath,"error");
            }
        }

        private void sendArcTranscodeThread(object arcparam)
        {
            ArcParam aparm = (ArcParam)arcparam;

            ResultInfo ria = arcTranscode(aparm.Apiurl, aparm.Paramxml);

            if (ria.Code == -1)
            {
                WriteLogNew.writeLog("转码出错！准备二次下发！" + ria.ErrorMsg, logpath, "error");
                Thread.Sleep(1000);
                ResultInfo ria2 = arcTranscode(aparm.Apiurl, aparm.Paramxml);
                if (ria2.Code == -1)
                {
                    WriteLogNew.writeLog("二次下发转码出错！" + ria2.ErrorMsg, logpath, "error");
                    //调用短信接口 发短信
                    string loginfo = this.Text + " " + Properties.Settings.Default.localIP + " 二次下发转码出错！" +aparm.Clipinfo ;
                    createSMStxt(loginfo);
                }
                else
                {
                    WriteLogNew.writeLog("下发任务到转码成功!", logpath, "info");
                }
            }
            else
            {
                WriteLogNew.writeLog("下发任务到转码成功!", logpath, "info");
            }
        }
   
        private void createNxml(NXMLFORM nf)
        {
            //生成nxml
            //处理xml生成nsml 
            try
            {
                string localnsml = Application.StartupPath + "\\localnsml";
                if (!Directory.Exists(localnsml))
                {
                    Directory.CreateDirectory(localnsml);
                }

                System.IO.StreamWriter sw = new System.IO.StreamWriter(localnsml + "\\" + Path.GetFileNameWithoutExtension(nf.Xmloriname) + ".nsml");
                string s3 = "\"3\" ";
                sw.WriteLine("<nsml version=" + s3 + ">");
                sw.WriteLine("<head>");
                int len = nf.Ntext.Length;
                sw.WriteLine("<meta words=\"" + len.ToString() + "\"" + " rate=\"330\" wordlength=\"1\" version=\"2\"/>");
                sw.WriteLine("<wgroup number=\"1\"></wgroup>");
                sw.WriteLine("<formname>" + nf.Form_name + "</formname>");
                sw.WriteLine("<storyid>" + "</storyid>");
                sw.WriteLine("</head>");
                sw.WriteLine("<fields>");
                
                sw.WriteLine("<date id=\"create-date\">" + nf.Create_date + "</date>");
                if (!string.IsNullOrEmpty(nf.Title))
                {
                    sw.WriteLine("<string id=\"title\">" + nf.Title + "</string>");
                }

                if (!string.IsNullOrEmpty(nf.Wirechan))
                {
                   sw.WriteLine("<string id=\"v-wirechan\">" + nf.Wirechan + "</string>");
                }

                sw.WriteLine("<string id=\"v-sendtime\">" + DateTime.Now.ToString("yyyyMMdd HH:mm") + "</string>");
                //double daudio = 330/len ;
                //int iaudio = Convert.ToInt32(daudio);
                //sw.WriteLine("<duration id=\"audio-time\">" + "</duration>");
                sw.WriteLine("<string id=\"ready\">READY</string>");
                if (!string.IsNullOrEmpty(nf.Create_by))
                {
                   sw.WriteLine("<string id=\"create-by\">" + nf.Create_by + "</string>");
                }

                if (!string.IsNullOrEmpty(nf.Writer))
                {
                    sw.WriteLine("<string id=\"writer\">" + nf.Writer + "</string>");
                }
              
                if (!string.IsNullOrEmpty(nf.Cameraman))
                {
                   sw.WriteLine("<string id=\"cameraman\">"+nf.Cameraman+"</string>");
                }
                double modifytimed = getsecond("");
                int modifytimei = Convert.ToInt32(modifytimed);
                sw.WriteLine("<date id=\"modify-date\">" + modifytimei.ToString() + "</date>");
                sw.WriteLine("<string id=\"page-number\"></string>");
               
                if (!string.IsNullOrEmpty(nf.Channel))
                {
                   sw.WriteLine("<string id=\"channel\">" + nf.Channel + "</string>");
                }
                if (!string.IsNullOrEmpty(nf.Source))
                {
                    sw.WriteLine("<string id=\"source\">" + nf.Source + "</string>");
                }
                if (!string.IsNullOrEmpty(nf.V_bumen))
                {
                    sw.WriteLine("<string id=\"v-bumen\">" + nf.V_bumen + "</string>");
                }

                sw.WriteLine("</fields>");
                sw.WriteLine("<body>");

                string writelines = "";
                writelines = " <p><cc>&lt;&lt;</cc><pi> 警告--红色模板请勿删除！&lt;及&gt;符号方便打印导语稿件，请勿删除!</pi><cc> &gt; &gt;</cc></p>";
                sw.WriteLine(writelines);
                //写入具体内容
                writelines = " <p><cc>&lt;&lt;</cc></p>";
                sw.WriteLine(writelines);
                writelines = " <p><pi>[正文]</pi></p>";
                sw.WriteLine(writelines);
                string outputinfo = getNsmlBody(nf.Ntext);
                sw.WriteLine(outputinfo);

                writelines   = "<p><cc>&gt;&gt;</cc></p>"; 
                sw.WriteLine(writelines);

                writelines = "<p><pi>[导语]</pi></p>";
                sw.WriteLine(writelines);

                writelines = "<p></p>";
                sw.WriteLine(writelines);

                writelines = "<p><pi>[编后]</pi></p>";
                sw.WriteLine(writelines);

                writelines = "<p></p>";
                sw.WriteLine(writelines);

                writelines = "<p>素材列表:</p>";
                sw.WriteLine(writelines);
                //写入素材信息
                foreach (string md in nf.mediafilelist)
                {

                    sw.WriteLine("<p>"+md+"</p>");
                }
                sw.WriteLine("</body>");
                sw.WriteLine("</nsml>");
                
                sw.Close();
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("生成nsml异常!"+ee.ToString(),logpath,"error");
                SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"生成nsml异常!\n");
            }
        }

        double getsecond(string stime)
        {
            DateTime dt70 = Convert.ToDateTime("01/01/1970");
            DateTime dts = DateTime.Now;
            if (stime.Length == 0)
            {
                //
            }
            else
            {
                dts = Convert.ToDateTime(stime);
            }
            TimeSpan ts = dts.AddHours(-8) - dt70;
            double sec = ts.TotalSeconds;
            return sec;
        }

        private string replaceSpecialXMLSyntax(string str)
        {
            string sr = str;
            Regex reg = new Regex("[“”《》·&\u0001]");
            Match m = reg.Match(str);
            if (m.Success)
            {
                sr = reg.Replace(str, "");
            }
            return sr;
        }

        public static string replaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }

        private string getNsmlBody(string texts)
        {
            string output = "";
            string[] strs = texts.Split(new string[] { "</p>" }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length > 0)
            {
                foreach (string str in strs)
                {
                    string outstr = "<p>" + replaceHtmlTag(str) + "</p>";
                    output += outstr;
                }
            }
            else
            {
                output = "<p>" + texts + "</p>";
            }
            return output;

        }

        private void timer_check_Tick(object sender, EventArgs e)
        {

        }

    }
}
