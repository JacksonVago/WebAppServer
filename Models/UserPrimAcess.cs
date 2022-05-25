using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppServer.Models
{
    public class UserPrimAcess
	{
		public Int64 id {get;set;}
		public Int64 id_empresa { get; set; }
		public Int64 id_emp_serv { get; set; }
		public Int64 id_user_app { get; set; }
		public string str_email { get; set; }
		public DateTime dtm_acesso { get; set; }
		public Int64 int_cod_acesso  {get;set;}
		public DateTime? dtm_confirma { get; set; }
		public Int16 int_situacao { get; set; }
	}
}
