using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.ErrorHAndling
{
    public class ExcpetionError
    {
        private string _message;
        private int _id;
        private string _stackTrace;

        [Required]
        public string StackTrace
        {
            get { return _stackTrace; }
            set { _stackTrace = value; }
        }

        [Key]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

    }
}
