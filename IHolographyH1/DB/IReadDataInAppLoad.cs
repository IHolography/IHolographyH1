using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHolographyH1.DB
{
    interface IReadDataInAppLoad
    {
        DataTable GetAvailableUserList();
    }
}
