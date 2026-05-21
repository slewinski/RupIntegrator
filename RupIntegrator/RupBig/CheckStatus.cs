using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RupBig.ServiceReferenceCheckStatus;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Windows.Forms;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.IO;
using System.Xml;
using System.Net;

namespace RupBig
{


    public class InspectorBehavior : IEndpointBehavior
    {
        public string LastRequestXML
        {
            get
            {


                return myMessageInspector.LastRequestXML;
            }
        }

        public string LastResponseXML
        {
            get
            {
                return myMessageInspector.LastResponseXML;
            }
        }

      



        private MyMessageInspector myMessageInspector = new MyMessageInspector();
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }


        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(myMessageInspector);
        }
    }





    public class MyMessageInspector : IClientMessageInspector
    {
        public string LastRequestXML { get; private set; }
        public string LastResponseXML { get; private set; }

        public string PackageFullId { get; set; }

        private void ChangeMessage(ref System.ServiceModel.Channels.Message message)
        {
            MemoryStream ms = new MemoryStream();
            Encoding encoding = Encoding.UTF8;
            XmlWriterSettings writerSettings = new XmlWriterSettings { Encoding = encoding };
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(ms));
            message.WriteBodyContents(writer);
            writer.Flush();
            string messageBodyString = encoding.GetString(ms.ToArray());

            // change the message body

            messageBodyString = messageBodyString.Replace("<success />", "<success>empty</success>");
            messageBodyString = messageBodyString.Replace("<processingError />", "<processingError>empty</processingError>");
            messageBodyString = messageBodyString.Replace("<authError />", "<authError>empty</authError>");
            messageBodyString = messageBodyString.Replace("<operationError />", "<operationError>empty</operationError>");
            messageBodyString = messageBodyString.Replace("<validationError />", "<validationError>empty</validationError>");
              
            messageBodyString = messageBodyString.Replace("[MS]error", "failed");
            messageBodyString = messageBodyString.Replace("[MS]pending", "pending");
       
            ms = new MemoryStream(encoding.GetBytes(messageBodyString));
            XmlReader bodyReader = XmlReader.Create(ms);
            System.ServiceModel.Channels.Message originalMessage = message;
            message = System.ServiceModel.Channels.Message.CreateMessage(originalMessage.Version, null, bodyReader);
            message.Headers.CopyHeadersFrom(originalMessage);
        }


        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
                LastResponseXML = reply.ToString();

                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    BIG_Log bl = new BIG_Log();
                    bl.ReqResp = true; // response
                    bl.MsgText = LastResponseXML;
                    bl.PackageFullId = PackageFullId;
                    if (LastResponseXML.Contains("packageSubmit"))

                        bl.TypKom = 1;
                    else
                        bl.TypKom = 0;

                    bl.DataOper = DateTime.Now;
                    context.BIG_Log.AddObject(bl);
                    context.SaveChanges();
                }
           // this.ChangeMessage(ref reply);
            //MessageFault mf = new MessageFault();

           // System.ServiceModel.Channels.Message replacedMessage = System.ServiceModel.Channels.Message.CreateMessage(reply.Version, null , NewResp);
           // replacedMessage.Headers.CopyHeadersFrom(reply.Headers);
           // replacedMessage.Properties.CopyProperties(reply.Properties);

            //reply = replacedMessage;
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            LastRequestXML = request.ToString();
            
            PackageFullId = Utils.getPackageId(LastRequestXML);
            // 
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                BIG_Log bl = new BIG_Log();
                bl.ReqResp = false; // request
                bl.MsgText = LastRequestXML;
                bl.PackageFullId = PackageFullId;
                if (LastRequestXML.Contains("packageSubmit") )
                    
                        bl.TypKom = 1;
                    else
                        bl.TypKom = 0 ;


                bl.DataOper = DateTime.Now;
                context.BIG_Log.AddObject(bl);
                context.SaveChanges();
            }

            return request;
        }
    }



    class CheckOperationStatus
    {

        public int RqType { get; set; }

        public string PackageId { get; set; }



        private ServiceReferenceCheckStatus.G2BIG_checkStatus_outClient  theClient;

      //  private ServiceReferenceBigMain.Credentials[] setupCredentials()
        



        private bool setupCilent()
        {// 
            ServicePointManager.SecurityProtocol =  SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
            BIG_Konfig bk = null;
            try
            {
                if (theClient != null) return true;
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    bk = context.BIG_Konfig.FirstOrDefault();

                }
                if (bk == null)
                {
                    MessageBox.Show("Brak konfiguracji usługi sieciowej BIG");
                    return false;
                }
              
                CustomBinding cbind = new CustomBinding("Ex2BIGBinding");
                

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                BasicHttpsBinding basicHttpBinding = new BasicHttpsBinding();
                basicHttpBinding.Security.Mode = BasicHttpsSecurityMode.Transport;
                basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                
                basicHttpBinding.SendTimeout = new TimeSpan(0, 5, 0);//
                basicHttpBinding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                basicHttpBinding.OpenTimeout = new TimeSpan(0, 2, 0);
                basicHttpBinding.MaxReceivedMessageSize = 2147483647;
                basicHttpBinding.MaxBufferSize = 2147483647;
                basicHttpBinding.ReaderQuotas.MaxStringContentLength = 1048576;



                //ServiceReferenceCheckStatus.G2BIG_checkStatus_outClient theClient1 = new ServiceReferenceCheckStatus.G2BIG_checkStatus_outClient("HTTP_Status");
                EndpointAddress basicAuthEndpoint = new EndpointAddress(new Uri(bk.CheckRqEndpoint));//, theClient1.Endpoint.Address.Identity, theClient1.Endpoint.Address.Headers);

                theClient = new ServiceReferenceCheckStatus.G2BIG_checkStatus_outClient(cbind, basicAuthEndpoint);
                theClient.ClientCredentials.UserName.UserName = bk.CheckRqAuthUser;
                theClient.ClientCredentials.UserName.Password = Utils.Decrypt(bk.CheckRqAuthPass,"Application error");

                var requestInterceptor = new InspectorBehavior();
                theClient.Endpoint.Behaviors.Add(requestInterceptor);

                return true;

            }
            catch (Exception ex)
            {

                Utils.SetupExceptionMessage(ex);
                return false;
            }


        }


        private CheckStatus setupCheckRequset(string packageId , List<int> bigs)
        {
            CheckStatus cs = new CheckStatus();
          
            cs.packageId = packageId;
            List<Credentials> crdLst = new List<Credentials>();
            using ( RupIntegratorEntities context = new RupIntegratorEntities())
            {
            foreach ( int id in bigs)
            {
            Credentials crd = new Credentials();    
                    BIG_User buser = context.BIG_User.Where (a=>a.IdBIG == id && a.IdUser == UserInfo.Id ).FirstOrDefault();
                    if (buser ==  null ) 
                    {
                         buser = context.BIG_User.Where (a=>a.IdBIG == id   ).OrderByDescending(a=>a.IdBigUser).FirstOrDefault();
                    }   
                    if (buser == null ) 
                    {
                        MessageBox.Show("Brak uprawnień do BIG, sprawdź konfiguraję uprawnień");
                        return null;
                    }
                    BIG_Big bb = context.BIG_Big.Where (a=>a.IdBig == id).FirstOrDefault();
                    crd.big_id = (CredentialsBig_id)Enum.Parse(typeof(CredentialsBig_id), bb.BIGID, false);
                    crd.password = buser.BigUserSha256; // Utils.Decrypt(buser.BigUserPassword, "Application error");
                    crd.subscriberId = bb.SubscriberId;
                    crd.userId = buser.BigUserName;
                    crdLst.Add(crd);
            }

            }
            cs.credentials = crdLst.ToArray();
            if (cs.credentials == null || !crdLst.Any())
            {
                return null;
            }
            return cs;
        }

        private bool updateStatusDB(List<vw_BIG_OperacjeToCheck> lst, StatusAllStatus[] result, string packageName)
        {
            if (result == null) { MessageBox.Show("Brak danych"); return false; };
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

            foreach (StatusAllStatus st in result)
            {
                if (st.status != null && st.status.packageStatus != null && (st.status.packageStatus.packageStatus == packageStatusEnum.processed || st.status.packageStatus.packageStatus == packageStatusEnum.failed || st.status.packageStatus.packageStatus == packageStatusEnum.MSerror || st.status.packageStatus.packageStatus == packageStatusEnum.MSnoPackageIDfound))
                {
                    bool failed = (st.status.packageStatus.packageStatus == packageStatusEnum.failed || st.status.packageStatus.packageStatus == packageStatusEnum.MSerror || st.status.packageStatus.packageStatus == packageStatusEnum.MSnoPackageIDfound);
                    int IdBig = context.BIG_Big.Where(a=>a.BIGID == st.big).Select(a=>a.IdBig).FirstOrDefault();
                    if (IdBig == null || IdBig == 0 ) 
                    {
                     MessageBox.Show(" Błędne oznaczenie BIG");
                        return false;
                    }

                    if (failed)
                    {
                        List<vw_BIG_OperacjeToCheck> lstFailed = lst.Where(a => a.IdBIG_Big == IdBig && a.PackageFullId == packageName).ToList();
                        if (lstFailed != null && lstFailed.Any())
                        {
                            foreach (vw_BIG_OperacjeToCheck opr in lstFailed)
                            {
                                opr.Status = -1;
                                switch (st.status.packageStatus.packageStatus)
                                { 
                                    case  packageStatusEnum.failed :
                                        opr.Info = "Błąd przekazania pakietu (failed)" ;
                                        
                                        break;
                                    case packageStatusEnum.MSerror:
                                        opr.Info = "Błąd komunikacji platformy z BIG ([MS]Error)";
                                        break;
                                    case packageStatusEnum.MSnoPackageIDfound:
                                        opr.Info = "Nie znaleziono pakietu o takim Id (MSnoPackageIDfound)";
                                        break;
                                    default: opr.Info = "Inny błąd transmisji pakietu ";
                                        break;
                                }
                                if (st.status.packageStatus.operationStatus != null && st.status.packageStatus.operationStatus.Any() )
                                {

                                    OperationStatusEntry os = st.status.packageStatus.operationStatus.Where(a=>a.operationId == opr.OperationId).FirstOrDefault();
                                    if ( os!= null )
                                        opr.Info +=  ((string.IsNullOrWhiteSpace(os.errorMessage) ? "" : " " + os.errorMessage)) ;  


                                }
                                
                            
                            }
                        
                        }
                    
                    }   
                    else
                    {
                        foreach (OperationStatusEntry o in st.status.packageStatus.operationStatus)
                        {
                            vw_BIG_OperacjeToCheck verify = lst.Where(a => a.IdBIG_Big == IdBig && a.OperationId == o.operationId && a.PackageFullId == packageName).FirstOrDefault();
                            if (verify == null)
                                continue;
                            if (o.status != null  && o.status.successSpecified)
                            {
                                verify.Status = 1; // ok
                                verify.Info = "";
                                verify.dSukces = o.processingDateTimeSpecified ? o.processingDateTime : DateTime.Now;


                            }
                            else
                                if (o.status != null && (o.status.authErrorSpecified  || o.status.operationErrorSpecified  || o.status.processingErrorSpecified || o.status.validationErrorSpecified))
                                {
                                    verify.Status = -1;
                                    string exmess = String.Empty;
                                    exmess = (!String.IsNullOrWhiteSpace(o.elementPath) ? o.elementPath :"" )  + " " + (!String.IsNullOrWhiteSpace(o.dependentElementPath) ? o.dependentElementPath :"").Trim();
                                    if (o.status.authErrorSpecified)
                                    {
                                    
                                        verify.Info += o.status.authError.ToString();
                                        
                                    
                                    }
                                    if (o.status.operationErrorSpecified)
                                        verify.Info += ";" + o.status.operationError.ToString();

                                    if (o.status.processingErrorSpecified)
                                        verify.Info += ";" + o.status.processingError.ToString();
                                    if (o.status.validationErrorSpecified)
                                        verify.Info += ";" + o.status.validationError.ToString();

                                    verify.Info += " " + exmess + " " + o.errorMessage;
                                    verify.dSukces = o.processingDateTimeSpecified ? o.processingDateTime : DateTime.Now;
                                }


                        }

                    }
                
                }
            
                }
            // update database
            foreach (vw_BIG_OperacjeToCheck pos in lst)
            {
                if (pos.Status != 0)
                {
                    BIG_Oper_Status bos = context.BIG_Oper_Status.Where(a => a.IdBIG_Oper_status == pos.IdBIG_Oper_status).FirstOrDefault();
                    if (bos != null)
                    {
                        bos.Status = pos.Status;
                        bos.Info = pos.Info;
                        bos.dSukces = pos.dSukces;
                    }
                
                }

            
            }
            

                context.SaveChanges();


            }
            return true;
        
        }

        public void CheckStatusByPackageList(List<string> packgIds)
        {
            try
            {

                if (packgIds == null || !packgIds.Any()) 
                    return;

                if (theClient == null)
                    if (!setupCilent())
                    {
                        MessageBox.Show("Błąd połączenia z usługą sieciową weryfikacji statusów");
                        return;

                    }
                
                List<int> idBIGLst = new List<int>();
                List<vw_BIG_OperacjeToCheck> lstOper = new List<vw_BIG_OperacjeToCheck>();  
                using (RupIntegratorEntities context = new RupIntegratorEntities())
                {
                    foreach (string pcgId in packgIds)
                    {
                        lstOper = context.vw_BIG_OperacjeToCheck.Where(a=>a.PackageFullId ==  pcgId).ToList();
                        idBIGLst = lstOper.Select(a => a.IdBIG_Big).Distinct().ToList();

                        CheckStatus cs = setupCheckRequset(pcgId, idBIGLst);

                        if (cs != null)
                        {
                            this.PackageId = pcgId;
                            this.RqType = 1; // typ 
                            var result = theClient.IG2BIG_checkStatus_out(cs);
                            if (result != null && result.Any())
                            {
                                updateStatusDB(lstOper, result, pcgId);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Błąd przetwarzania statusów");
                return;

            }
        
        }


        public void checkStatusAll()
        {
            try
            {

                if (theClient == null)
                    if (!setupCilent())
                    {
                        MessageBox.Show("Błąd połączenia z usługą sieciową eryfikacji statusów");
                        return;

                    }
              //        List<int> lst1 = new List<int>();
              //        lst1.Add(2);
              //                    CheckStatus cs1 = setupCheckRequset("3232 323232332 32 322 323 32",lst1) ;
              //          if (cs1!= null)
              //          {
              //              var result = theClient.IG2BIG_checkStatus_out(cs1);
              //              if (result != null && result.Any())
              //              {
              //          ;
              //
              //              }
              //          }
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {

                List<vw_BIG_OperacjeToCheck> lst = context.vw_BIG_OperacjeToCheck.OrderBy(a => a.PackageFullId).ToList();

                if ( lst != null )
                {   string packageName= "";
                    List<int> idBIGLst = new List<int>();
                    List<vw_BIG_OperacjeToCheck> packLst = new List<vw_BIG_OperacjeToCheck>();
                    foreach (vw_BIG_OperacjeToCheck item in lst)
                    {
                        if (!String.IsNullOrWhiteSpace(packageName) && item.PackageFullId != packageName)
                        {
                            // wyslij 
                            CheckStatus cs = setupCheckRequset(packageName,idBIGLst);
                            if (cs != null)
                            {
                                var result = theClient.IG2BIG_checkStatus_out(cs);
                                if (result != null && result.Any())
                                {
                                    updateStatusDB(packLst, result, packageName);

                                }
                            }
                            packLst = new List<vw_BIG_OperacjeToCheck>();
                            idBIGLst = new List<int>();
                        }
                                                   
                        if (! idBIGLst.Contains(item.IdBIG_Big) )
                            idBIGLst.Add(item.IdBIG_Big);
                       
                       
                        packLst.Add(item);
                     packageName = item.PackageFullId;
                    
                    }
                    // oststnie obrót
                    // sprawdzenie pozostałości
                    if (packLst.Any() && !String.IsNullOrWhiteSpace(packageName) )
                    {
                        CheckStatus cs = setupCheckRequset(packageName, idBIGLst);
                        if (cs != null)
                        {
                            var result = theClient.IG2BIG_checkStatus_out(cs);
                            if (result != null && result.Any())
                            {
                                updateStatusDB(packLst, result, packageName);

                            }
                        }
                    }
                
                
                }
            
            
            }
            }
        catch(Exception ex)
        {

            MessageBox.Show(ex.Message, "Błąd przetwarzania statusów");
            return;
        
        }
        
        
        }
            


    }
}
