using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace HelloWorldApp
{
    public partial class Welcome : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.Count > 0)
            {
                //Map the querytsring param values to Authorize response to get State and Code
                var response = new AuthorizeResponse(Request.QueryString.ToString());

                //extracts the state and realmId (or company id)
                if (response.State != null && response.RealmId != null)
                {
                    string state = response.State;
                    Application["realmId"] = response.RealmId;
                    string authorizationCode = response.Code;

                    var tokenClient = new TokenClient((string)Application["tokenEndpoint"], 
                                                        ConfigurationManager.AppSettings["clientID"], 
                                                        ConfigurationManager.AppSettings["clientSecret"]);
                    TokenResponse accesstokenCallResponse = await tokenClient.RequestTokenFromCodeAsync(authorizationCode, 
                                                                                                            ConfigurationManager.AppSettings["redirectURI"]);
                    if (accesstokenCallResponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        // Get access token 
                        Application["accessToken"] = accesstokenCallResponse.AccessToken;

                        // Use access token to retrieve company Info
                        OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator((string)Application["accessToken"]);
                        ServiceContext serviceContext = new ServiceContext((string)Application["realmId"], IntuitServicesType.QBO, oauthValidator);
                        QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);
                        CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                        this.DisplayName.Text = string.Format("You have successfully connected to Company Name: {0} with Id : {1}", companyInfo.CompanyName, (string)Application["realmId"]);
                        this.DisplayName.EnableViewState = true;
                        this.SandboxCompany_Hyperlink.Visible = true;
                        this.SandboxCompany_Hyperlink.NavigateUrl = string.Format("https://sandbox.qbo.intuit.com/login?deeplinkcompanyid={0}", (string)Application["realmId"]);

                    }
                    else { 
                        this.DisplayName.Text = "Your app is now authorized !!";
                    }
                }
            }
            else
            {
                this.DisplayName.Text = "Looks like you have not yet connected to QBO company !!";
                this.SandboxCompany_Hyperlink.Visible = false;
            }
        }

        protected void CreateInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Create Invoice");
                CreateInvoice();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// This API creates an Invoice
        /// </summary>
        private void CreateInvoice()
        {
            // Step 1: Initialize OAuth2RequestValidator and ServiceContext
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator((string)Application["accessToken"]);
            ServiceContext serviceContext = new ServiceContext((string)Application["realmId"], IntuitServicesType.QBO, oauthValidator);

            // Step 2: Initialize an Invoice object
            Invoice invoice = new Invoice();
            invoice.Deposit = new Decimal(0.00);
            invoice.DepositSpecified = true;

            // Step 3: Invoice is always created for a customer so lets retrieve reference to a customer and set it in Invoice
            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            Customer customer = querySvc.ExecuteIdsQuery("SELECT * FROM Customer WHERE CompanyName like 'Amy%'").FirstOrDefault();
            invoice.CustomerRef = new ReferenceType()
            {
                Value = customer.Id
            };


            // Step 4: Invoice is always created for an item so lets retrieve reference to an item and a Line item to the invoice
            QueryService<Item> querySvcItem = new QueryService<Item>(serviceContext);
            Item item = querySvcItem.ExecuteIdsQuery("SELECT * FROM Item WHERE Name = 'Lighting'").FirstOrDefault();
            List<Line> lineList = new List<Line>();
            Line line = new Line();
            line.Description = "Description";
            line.Amount = new Decimal(100.00);
            line.AmountSpecified = true;
            lineList.Add(line);
            invoice.Line = lineList.ToArray();

            SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
            salesItemLineDetail.Qty = new Decimal(1.0);
            salesItemLineDetail.ItemRef = new ReferenceType()
            {
                Value = item.Id
            };
            line.AnyIntuitObject = salesItemLineDetail;

            line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
            line.DetailTypeSpecified = true;

            // Step 5: Set other properties such as Total Amount, Due Date, Email status and Transaction Date
            invoice.DueDate = DateTime.UtcNow.Date;
            invoice.DueDateSpecified = true;


            invoice.TotalAmt = new Decimal(10.00);
            invoice.TotalAmtSpecified = true;

            invoice.EmailStatus = EmailStatusEnum.NotSet;
            invoice.EmailStatusSpecified = true;

            invoice.Balance = new Decimal(10.00);
            invoice.BalanceSpecified = true;

            invoice.TxnDate = DateTime.UtcNow.Date;
            invoice.TxnDateSpecified = true;
            invoice.TxnTaxDetail = new TxnTaxDetail()
            {
                TotalTax = Convert.ToDecimal(10),
                TotalTaxSpecified = true,
            };

            // Step 6: Initialize the service object and create Invoice
            DataService service = new DataService(serviceContext);
            Invoice addedInvoice = service.Add<Invoice>(invoice);
        }


    }
}