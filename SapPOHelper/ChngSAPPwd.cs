using Ex2PscdInterface.Ex2PscdManageAccountOutService;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SapPOHelper
{
    public class ChngSAPPwd
    {
        // get password time
        public static int GetPwdExpirationTime()
        {

            ManageAccountRequest rq = new ManageAccountRequest();
            rq.NoweHaslo = null;
            ManageAccountResponse resp =  (ManageAccountResponse)ZSRKRequestHelper.CallSAPMethod("ManageAccountOut", rq);
            DateTime tmpDt;

            if (DateTime.TryParseExact(resp.DataWygasnieciaHasla, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDt))
            {
                ;
            }
            else
                return -1;
                
            
            return tmpDt.Subtract(DateTime.Today).Days;


        }

        public static Komunikat SetNewPassword(string password)
        {

            ManageAccountRequest rq = new ManageAccountRequest();
            
            rq.NoweHaslo = password;
            ManageAccountResponse resp = (ManageAccountResponse)ZSRKRequestHelper.CallSAPMethod("ManageAccountOut", rq);
           


            return resp.Komunikaty.FirstOrDefault();


        }


        public static bool VerifySAPPwdExpire(int days)
        {


            int iledni = GetPwdExpirationTime();
            if (iledni <= days)
            {
                if (MessageBox.Show("Ważność twojego hasła w systemie ZSRK upłynie za " + iledni.ToString() + " dni. Czy chcesz zmienić hasło w ZSRK ?", "Pytanie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    return true;


                }


            }
            return false;
        }

            
        }

    
}
