using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewAppAFAPP.Model
{
    public class Topic
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TopicType { get; set; }
        public string TopicImage { get; set; }
        public string TopicVideo { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
