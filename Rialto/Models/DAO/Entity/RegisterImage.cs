using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Entity
{
    public class RegisterImage
    {
        public int Id { get; set; }
        public int FileSize { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string Md5Hash { get; set; }
        public string AveHash { get; set; }
        
        public int HeightPix { get; set; }
        public int WidthPix { get; set; }
        
        public int DoGet { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
