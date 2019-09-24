using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Models
{
    /// <summary>
    /// Notes实体
    /// </summary>
    public class Notes
    {
        /// <summary>
        /// 标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Create { get; set; }



        /// <summary>
        /// 其他
        /// </summary>
        public string Other { get; set; }
    }
}
