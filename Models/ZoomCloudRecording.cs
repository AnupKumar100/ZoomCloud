using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpotifyWebApiCheck.Models
{
    public class ZoomCloudRecording
    {
        [Display(Name ="From Date")]
        public string From { get; set; }


        [Display(Name ="To Date")]
        public string To { get; set; }


        public int Page_count { get; set; }
        public int Page_size { get; set; }
        public int Total_records { get; set; }
        public string Next_page_token { get; set; }
        public List<Meeting> Meetings { get; set; }
    }
    public class RecordingFile
    {
        public string id { get; set; }
        public string meeting_id { get; set; }
        public DateTime recording_start { get; set; }
        public DateTime recording_end { get; set; }
        public string file_type { get; set; }
        public int file_size { get; set; }
        public string play_url { get; set; }
        public string download_url { get; set; }
        public string status { get; set; }
        public string recording_type { get; set; }
    }

    public class Meeting
    {
        public string uuid { get; set; }
        public long id { get; set; }
        public string account_id { get; set; }
        public string host_id { get; set; }
        public string topic { get; set; }
        public int type { get; set; }
        public DateTime start_time { get; set; }
        public string timezone { get; set; }
        public int duration { get; set; }
        public int total_size { get; set; }
        public int recording_count { get; set; }
        public string share_url { get; set; }
        public List<RecordingFile> recording_files { get; set; }
    }

    
}