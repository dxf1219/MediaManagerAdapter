using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XnewsAdapter
{
    class MediaInfos
    {
        private string srcFrom;

        public string SrcFrom
        {
            get { return srcFrom; }
            set { srcFrom = value; }
        }
        
        private string sPathvideo;

        public string SPathvideo
        {
            get { return sPathvideo; }
            set { sPathvideo = value; }
        }
       
        private string sPathxml;

        public string SPathxml
        {
            get { return sPathxml; }
            set { sPathxml = value; }
        }

        private int ifcontailsNextFolders;

        public int IfcontailsNextFolders
        {
            get { return ifcontailsNextFolders; }
            set { ifcontailsNextFolders = value; }
        }

        private string transcodeFilePath;

        public string TranscodeFilePath
        {
            get { return transcodeFilePath; }
            set { transcodeFilePath = value; }
        }

        private string linuxTranscodeFilePath;

        public string LinuxTranscodeFilePath
        {
            get { return linuxTranscodeFilePath; }
            set { linuxTranscodeFilePath = value; }
        }

        private string mediafilename;  //素材名称

        public string Mediafilename
        {
            get { return mediafilename; }
            set { mediafilename = value; }
        }

        private string newavidfilename; //checkin interplay 新名称

        public string Newavidfilename
        {
            get { return newavidfilename; }
            set { newavidfilename = value; }
        }

        private string srcxmlpath; //最原始的xml路径

        public string Srcxmlpath
        {
            get { return srcxmlpath; }
            set { srcxmlpath = value; }
        }

        private string srcxmlname;

        public string Srcxmlname
        {
            get { return srcxmlname; }
            set { srcxmlname = value; }
        }


    }

    class ArcParam
    {
        private string apiurl;

        public string Apiurl
        {
            get { return apiurl; }
            set { apiurl = value; }
        }
        private string paramxml;

        public string Paramxml
        {
            get { return paramxml; }
            set { paramxml = value; }
        }

        public string Clipinfo { set; get; }
    }
    class TranscodeFileInfos
    {
        private string srcFrom;

        public string SrcFrom
        {
            get { return srcFrom; }
            set { srcFrom = value; }
        }

        private string sPathvideo;

        public string SPathvideo
        {
            get { return sPathvideo; }
            set { sPathvideo = value; }
        }

        private string sPathxml; 

        public string SPathxml
        {
            get { return sPathxml; }
            set { sPathxml = value; }
        }

        private int ifcontailsNextFolders;

        public int IfcontailsNextFolders
        {
            get { return ifcontailsNextFolders; }
            set { ifcontailsNextFolders = value; }
        }

        private string transcodeFilePath;

        public string TranscodeFilePath
        {
            get { return transcodeFilePath; }
            set { transcodeFilePath = value; }
        }

        private string linuxTranscodeFilePath;

        public string LinuxTranscodeFilePath
        {
            get { return linuxTranscodeFilePath; }
            set { linuxTranscodeFilePath = value; }
        }

        private string mediafilename;  //素材名称

        public string Mediafilename
        {
            get { return mediafilename; }
            set { mediafilename = value; }
        }

        private string newavidfilename; //checkin interplay 新名称

        public string Newavidfilename
        {
            get { return newavidfilename; }
            set { newavidfilename = value; }
        }

        private string srcxmlpath; //最原始的xml路径

        public string Srcxmlpath
        {
            get { return srcxmlpath; }
            set { srcxmlpath = value; }
        }

        private string srcxmlname;

        public string Srcxmlname
        {
            get { return srcxmlname; }
            set { srcxmlname = value; }
        }

        private int transcodeprocess;

        public int Transcodeprocess
        {
            get { return transcodeprocess; }
            set { transcodeprocess = value; }
        }

        private string transcodepid;

        public string Transcodepid
        {
            get { return transcodepid; }
            set { transcodepid = value; }
        }

        private int register;

        public int Register
        {
            get { return register; }
            set { register = value; }
        }


    }

    class XftpInfo
    {
        private string srcFrom;

        public string SrcFrom
        {
            get { return srcFrom; }
            set { srcFrom = value; }
        }
        private string xftpAPI;

        public string XftpAPI
        {
            get { return xftpAPI; }
            set { xftpAPI = value; }
        }

        private string xftpPath;

        public string XftpPath
        {
            get { return xftpPath; }
            set { xftpPath = value; }
        }

        private string nsmlInputPath;

        public string NsmlInputPath
        {
            get { return nsmlInputPath; }
            set { nsmlInputPath = value; }
        }

        private string nsmlOutputPath;

        public string NsmlOutputPath
        {
            get { return nsmlOutputPath; }
            set { nsmlOutputPath = value; }
        }

        private string picInputPath;

        public string PicInputPath
        {
            get { return picInputPath; }
            set { picInputPath = value; }
        }

        private string picOutputPath;

        public string PicOutputPath
        {
            get { return picOutputPath; }
            set { picOutputPath = value; }
        }

        private string transcodeFileOldInPath;

        public string TranscodeFileOldInPath
        {
            get { return transcodeFileOldInPath; }
            set { transcodeFileOldInPath = value; }
        }

        private string transcodeFileInPath;

        public string TranscodeFileInPath
        {
            get { return transcodeFileInPath; }
            set { transcodeFileInPath = value; }
        }
        private string transcodeFileOutPath;

        public string TranscodeFileOutPath
        {
            get { return transcodeFileOutPath; }
            set { transcodeFileOutPath = value; }
        }
        private string mediaXmlPath;

        public string MediaXmlPath
        {
            get { return mediaXmlPath; }
            set { mediaXmlPath = value; }
        }


    }

    public class NXMLFORM
    {

        public List<string> mediafilelist;
        //属性 
        private string platform;

        public string Platform
        {
            get { return platform; }
            set { platform = value; }
        }
        private string site;

        public string Site
        {
            get { return site; }
            set { site = value; }
        }
        private string xmloriname;//原始xml的名称 

        public string Xmloriname
        {
            get { return xmloriname; }
            set { xmloriname = value; }
        }
        private string form_name;

        public string Form_name
        {
            get { return form_name; }
            set { form_name = value; }
        }
        private string story_id;

        public string Story_id
        {
            get { return story_id; }
            set { story_id = value; }
        }
        private string create_date;

        public string Create_date
        {
            get { return create_date; }
            set { create_date = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string wirechan; //新闻来源

        public string Wirechan
        {
            get { return wirechan; }
            set { wirechan = value; }
        }
        private string send_time;

        public string Send_time
        {
            get { return send_time; }
            set { send_time = value; }
        }
        private string audio_time;

        public string Audio_time
        {
            get { return audio_time; }
            set { audio_time = value; }
        }
        private string ready;

        public string Ready
        {
            get { return ready; }
            set { ready = value; }
        }
        private string create_by;

        public string Create_by
        {
            get { return create_by; }
            set { create_by = value; }
        }
        private string writer;

        public string Writer
        {
            get { return writer; }
            set { writer = value; }
        }
        private string modify_date;

        public string Modify_date
        {
            get { return modify_date; }
            set { modify_date = value; }
        }
        private string page_number;

        public string Page_number
        {
            get { return page_number; }
            set { page_number = value; }
        }
        private string video_id;

        public string Video_id
        {
            get { return video_id; }
            set { video_id = value; }
        }
        private string presenter;

        public string Presenter
        {
            get { return presenter; }
            set { presenter = value; }
        }
        private string daoyu;

        public string Daoyu
        {
            get { return daoyu; }
            set { daoyu = value; }
        }
        private string bianhou;

        public string Bianhou
        {
            get { return bianhou; }
            set { bianhou = value; }
        }
        private string jiwei;

        public string Jiwei
        {
            get { return jiwei; }
            set { jiwei = value; }
        }
        private string v_daoyuztc;

        public string V_daoyuztc
        {
            get { return v_daoyuztc; }
            set { v_daoyuztc = value; }
        }
        private string tihuazimu;

        public string Tihuazimu
        {
            get { return tihuazimu; }
            set { tihuazimu = value; }
        }
        private string airtype;

        public string Airtype
        {
            get { return airtype; }
            set { airtype = value; }
        }
        private string second;

        public string Second
        {
            get { return second; }
            set { second = value; }
        }
        private string runs_time;

        public string Runs_time
        {
            get { return runs_time; }
            set { runs_time = value; }
        }
        private string total_time;

        public string Total_time
        {
            get { return total_time; }
            set { total_time = value; }
        }
        private string jingbian;

        public string Jingbian
        {
            get { return jingbian; }
            set { jingbian = value; }
        }
        private string peiyin;

        public string Peiyin
        {
            get { return peiyin; }
            set { peiyin = value; }
        }
        private string event_status;

        public string Event_status
        {
            get { return event_status; }
            set { event_status = value; }
        }
        private string v_crew;

        public string V_crew
        {
            get { return v_crew; }
            set { v_crew = value; }
        }
        private string v_check;

        public string V_check
        {
            get { return v_check; }
            set { v_check = value; }
        }
        private string cameraman;

        public string Cameraman
        {
            get { return cameraman; }
            set { cameraman = value; }
        }
        private string zhizuotishi;

        public string Zhizuotishi
        {
            get { return zhizuotishi; }
            set { zhizuotishi = value; }
        }
        private string source;

        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        private string air_date;

        public string Air_date
        {
            get { return air_date; }
            set { air_date = value; }
        }
        private string v_date;

        public string V_date
        {
            get { return v_date; }
            set { v_date = value; }
        }
        private string v_bumen;

        public string V_bumen
        {
            get { return v_bumen; }
            set { v_bumen = value; }
        }
        private string endorse_by;

        public string Endorse_by
        {
            get { return endorse_by; }
            set { endorse_by = value; }
        }
        private string modify_by;

        public string Modify_by
        {
            get { return modify_by; }
            set { modify_by = value; }
        }
        private string back_time;

        public string Back_time
        {
            get { return back_time; }
            set { back_time = value; }
        }
        private string channel;

        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }
        private string v_miji;

        public string V_miji
        {
            get { return v_miji; }
            set { v_miji = value; }
        }
        private string modify_dev;

        public string Modify_dev
        {
            get { return modify_dev; }
            set { modify_dev = value; }
        }
        private string ntext;

        public string Ntext
        {
            get { return ntext; }
            set { ntext = value; }
        }


    }

    public class ResultInfo
    {
        private int code;

        public int Code
        {
            get { return code; }
            set { code = value; }
        }
        private string info;

        public string Info
        {
            get { return info; }
            set { info = value; }
        }
        private string errorMsg;

        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }
    }

    public class Xftpjson
    {
        private XFTPFile file;

        public XFTPFile File
        {
            get { return file; }
            set { file = value; }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
    }

    public class XFTPFile
    {
        private string createtime;

        public string Createtime
        {
            get { return createtime; }
            set { createtime = value; }
        }
        private string fileId;

        public string FileId
        {
            get { return fileId; }
            set { fileId = value; }
        }
        private string folderId;

        public string FolderId
        {
            get { return folderId; }
            set { folderId = value; }
        }
        private string lastbegintime;

        public string Lastbegintime
        {
            get { return lastbegintime; }
            set { lastbegintime = value; }
        }
        private string lastupdatetime;

        public string Lastupdatetime
        {
            get { return lastupdatetime; }
            set { lastupdatetime = value; }
        }
        private string md5;

        public string Md5
        {
            get { return md5; }
            set { md5 = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string owner;

        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        private long size;

        public long Size
        {
            get { return size; }
            set { size = value; }
        }
        private int status;

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
